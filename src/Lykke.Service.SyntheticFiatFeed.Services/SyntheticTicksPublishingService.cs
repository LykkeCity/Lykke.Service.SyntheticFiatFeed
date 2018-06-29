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
using Newtonsoft.Json;

namespace Lykke.Service.SyntheticFiatFeed.Services
{
    public sealed class SyntheticTicksPublishingService : IHostedService
    {
        private readonly TickPriceSettings _settings;
        private readonly ILog _log;
        private IDisposable _worker;
        private RabbitMqPublisher<TickPrice> _publisher;
        private readonly ILogFactory _logFactory;

        public SyntheticTicksPublishingService(
            ILogFactory lf,
            TickPriceSettings settings)
        {
            _settings = settings;
            _logFactory = lf;
            _log = lf.CreateLog(this);
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _worker = CreateWorker().Subscribe();
            
            return Task.CompletedTask;
        }

        private IObservable<Unit> CreateWorker()
        {
            var feeds = GetFeeds().ToArray();

            _log.Info($"Producing {feeds.Length} feeds");

           _publisher = CreatePublisher(_settings.RabbitMq, _settings.OutputExchanger, _logFactory);

            return feeds
                .Merge()
                .SelectMany(async x =>
                {
                    await _publisher.ProduceAsync(x);
                    return x;
                })
                .Do(x => _log.Debug(JsonConvert.SerializeObject(x)),
                    err => _log.Error(err))
                .RetryWithBackoff(TimeSpan.FromSeconds(1), TimeSpan.FromMinutes(10))
                .Select(_ => Unit.Default);
        }

        private static RabbitMqPublisher<TickPrice> CreatePublisher(
            string connectionString,
            string exchanger,
            ILogFactory log)
        {
            var settings = RabbitMqSubscriptionSettings.CreateForPublisher(
                connectionString,
                exchanger);

            return new RabbitMqPublisher<TickPrice>(log, settings)
                    .SetSerializer(new JsonMessageSerializer<TickPrice>())
                    .SetPublishStrategy(new DefaultFanoutPublishStrategy(settings))
                    .DisableInMemoryQueuePersistence()
                    .Start();
        }

        private IEnumerable<IObservable<TickPrice>> GetFeeds()
        {
            foreach (var input in _settings.Inputs.Where(x => x.Enabled))
            {
                var settings = new RabbitMqSubscriptionSettings
                {
                    ConnectionString = _settings.RabbitMq,
                    ExchangeName = input.Exchanger,
                    QueueName = $"{input.Exchanger}.synthetic-fiat-{Guid.NewGuid()}",
                    IsDurable = false
                };

                var ticks = RmqHelper.ReadAsJson<TickPrice>(settings, _logFactory)
                    .Share();

                foreach (var crypto in input.Crypto)
                {
                    foreach (var fiat in _settings.Fiat)
                    {
                        var (source, target) = ParseFiatAsset(fiat.Asset);

                        yield return ProduceFiatTickPrices(
                            ticks,
                            input.Name,
                            crypto,
                            source,
                            target,
                            fiat.Decimals);
                    }
                }
            }
        }

        private static (string, string) ParseFiatAsset(string fiatAsset)
        {
            if (fiatAsset.Length != 6)
                throw new ArgumentException("Expected fiat asset of two 3-chars currencies, e.g. eurusd");

            return (fiatAsset.Substring(0, 3), fiatAsset.Substring(3));
        }

        private static IObservable<TickPrice> ProduceFiatTickPrices(
            IObservable<TickPrice> source,
            string sourceName,
            string crypto,
            string sourceFiat,
            string targetFiat,
            int decimals)
        {
            var sourceTicks = source.Where(x => x.Asset.Equals($"{crypto}{sourceFiat}",
                StringComparison.InvariantCultureIgnoreCase));

            var targetTicks = source.Where(x => x.Asset.Equals($"{crypto}{targetFiat}",
                StringComparison.InvariantCultureIgnoreCase));

            var feed = Observable.CombineLatest(
                sourceTicks,
                targetTicks,
                (s, t) => GetCrossTickPrice(
                    $"synthetic-{sourceName}-{crypto}",
                    $"{sourceFiat}{targetFiat}",
                    decimals,
                    s,
                    t));

            return feed;
        }

        private static TickPrice GetCrossTickPrice(
            string syntheticSourceName,
            string asset,
            int decimals,
            TickPrice source,
            TickPrice target)
        {
            return new TickPrice
            {
                Source = syntheticSourceName,
                Asset = asset,
                Timestamp = source.Timestamp > target.Timestamp ? source.Timestamp : target.Timestamp,
                Ask = source.Bid == 0M? 0: Math.Round(target.Ask / source.Bid, decimals),
                Bid = source.Ask == 0M? 0: Math.Round(target.Bid / source.Ask, decimals)
            };
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _worker?.Dispose();
            _publisher.Dispose();

            return Task.CompletedTask;
        }
    }
}
