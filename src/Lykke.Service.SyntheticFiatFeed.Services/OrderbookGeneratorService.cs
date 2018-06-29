using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.ExchangeAdapter;
using Lykke.Common.ExchangeAdapter.Contracts;
using Lykke.Common.Log;
using Lykke.RabbitMqBroker.Publisher;
using Lykke.RabbitMqBroker.Subscriber;
using Microsoft.Extensions.Hosting;

namespace Lykke.Service.SyntheticFiatFeed.Services
{
    public sealed class OrderbookGeneratorService : IHostedService
    {
        private readonly ILogFactory _logFactory;
        private readonly OrderbooksSettings _settings;
        private readonly ILog _log;
        private IDisposable _worker;
        private RabbitMqPublisher<OrderBook> _publisher;

        public OrderbookGeneratorService(
            ILogFactory logFactory,
            OrderbooksSettings settings)
        {
            _logFactory = logFactory;
            _log = logFactory.CreateLog(this);
            _settings = settings;
        }

        private static RabbitMqPublisher<OrderBook> CreatePublisher(
            string connectionString,
            string exchanger,
            ILogFactory log)
        {
            var settings = RabbitMqSubscriptionSettings.CreateForPublisher(
                connectionString,
                exchanger);

            return new RabbitMqPublisher<OrderBook>(log, settings)
                .SetSerializer(new JsonMessageSerializer<OrderBook>())
                .SetPublishStrategy(new DefaultFanoutPublishStrategy(settings))
                .DisableInMemoryQueuePersistence()
                .Start();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var lykkeTickPrices = GetLykkeTickPrices().Share();

            var bitstampOrderbooks = GetBitstampOrderbooks().Share();

            _publisher = CreatePublisher(_settings.RabbitMq, _settings.OutputExchanger, _logFactory);

            _worker = GetFeeds(lykkeTickPrices, bitstampOrderbooks)
                .Merge()
                .SelectMany(async ob =>
                {
                    await _publisher.ProduceAsync(ob);
                    return Unit.Default;
                })
                .RetryWithBackoff(TimeSpan.FromSeconds(1), TimeSpan.FromMinutes(10))
                .Subscribe(
                    _ => { },
                    err => _log.Error(err));

            return Task.CompletedTask;
        }

        private IEnumerable<IObservable<OrderBook>> GetFeeds(
            IObservable<TickPrice> sourceTickPrices,
            IObservable<OrderBook> sourceOrdeBook)
        {
            foreach (var expected in _settings.CrossFiatRates)
            {
                yield return GetSyntheticOrderBooks(
                    sourceTickPrices,
                    sourceOrdeBook,
                    new OrderbookGenerationSettings(
                        _settings.CryptoCurrency,
                        expected.Source, expected.Target));
            }
        }

        private IObservable<OrderBook> GetSyntheticOrderBooks(
            IObservable<TickPrice> lykkeTickPrices,
            IObservable<OrderBook> bitstampOrderbooks,
            OrderbookGenerationSettings orderbookGeneration)
        {
            var crossTicks = GetCrossTicks(lykkeTickPrices, orderbookGeneration);

            var sourceBooks = bitstampOrderbooks.Where(x =>
                string.Equals(
                    x.Asset,
                    $"{orderbookGeneration.Crypto}{orderbookGeneration.BaseFiat}",
                    StringComparison.InvariantCultureIgnoreCase));

            return Observable.CombineLatest(
                crossTicks,
                sourceBooks,
                (t, b) => CreateSyntheticOrderBook(
                    t,
                    b,
                    $"{orderbookGeneration.Crypto}{orderbookGeneration.ExpectedFiat}",
                    $"synthetic-{_settings.SourceName}-{orderbookGeneration.Crypto.ToLowerInvariant()}" +
                    $"{orderbookGeneration.BaseFiat.ToLowerInvariant()}"));
        }

        private IObservable<TickPrice> GetCrossTicks(
            IObservable<TickPrice> lykkeTickPrices,
            OrderbookGenerationSettings settings)
        {
            var direct = $"{settings.BaseFiat}{settings.ExpectedFiat}";
            var reverse = $"{settings.ExpectedFiat}{settings.BaseFiat}";

            return
                Observable.Merge(

                    lykkeTickPrices
                        .Where(x => string.Equals(x.Asset, direct, StringComparison.InvariantCultureIgnoreCase)),

                    lykkeTickPrices
                        .Where(x => string.Equals(x.Asset, reverse, StringComparison.InvariantCultureIgnoreCase))
                        .Select(x => Reverse(x, direct)));
        }

        private static TickPrice Reverse(TickPrice tickPrice, string reverseAsset)
        {
            return new TickPrice
            {
                Ask = tickPrice.Ask == 0 ? 0 : 1 / tickPrice.Ask,
                Bid = tickPrice.Bid == 0 ? 0 : 1 / tickPrice.Bid,
                Asset = reverseAsset,
                Timestamp = tickPrice.Timestamp,
                Source = tickPrice.Source
            };
        }

        private OrderBook CreateSyntheticOrderBook(
            TickPrice tickPrice,
            OrderBook orderBook,
            string resultPair,
            string source)
        {
            return new OrderBook(
                source,
                resultPair,
                tickPrice.Timestamp > orderBook.Timestamp ? tickPrice.Timestamp : orderBook.Timestamp,
                orderBook.Asks.Select(x => new OrderBookItem
                {
                    Price = x.Price * tickPrice.Ask,
                    Volume = x.Volume
                }),
                orderBook.Bids.Select(x => new OrderBookItem
                {
                    Price = x.Price * tickPrice.Bid,
                    Volume = x.Volume
                }));
        }

        private IObservable<OrderBook> GetBitstampOrderbooks()
        {
            var settings = new RabbitMqSubscriptionSettings
            {
                ConnectionString = _settings.RabbitMq,
                ExchangeName = "lykke.exchangeconnector.orderBooks.bitstamp",
                QueueName = $"lykke.exchangeconnector.orderBooks.bitstamp.synthetic-fiat-{Guid.NewGuid()}",
                IsDurable = false
            };

            return RmqHelper.ReadAsJson<OrderBook>(settings, _logFactory);
        }

        private IObservable<TickPrice> GetLykkeTickPrices()
        {
            var settings = new RabbitMqSubscriptionSettings
            {
                ConnectionString = _settings.RabbitMq,
                ExchangeName = "lykke.exchangeconnector.tickPrices.lykke",
                QueueName = $"lykke.exchangeconnector.tickPrices.lykke.synthetic-fiat-{Guid.NewGuid()}",
                IsDurable = false
            };

            return  RmqHelper.ReadAsJson<TickPrice>(settings, _logFactory);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _publisher?.Stop();
            _worker?.Dispose();
            return Task.CompletedTask;
        }
    }
}
