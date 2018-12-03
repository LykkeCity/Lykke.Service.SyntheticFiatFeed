using System;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.ExchangeAdapter.Contracts;
using Lykke.Common.Log;
using Lykke.RabbitMqBroker.Publisher;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Service.SyntheticFiatFeed.Domain.Services;
using Lykke.Service.SyntheticFiatFeed.Settings;

namespace Lykke.Service.SyntheticFiatFeed.RabbitMq
{
    public class OrderBookPublisher : IOrderBookProvider, IDisposable
    {
        private readonly RabbitMqPublisherSettings _settings;
        private readonly ILogFactory _logFactory;
        private RabbitMqPublisher<OrderBook> _publisher;

        public OrderBookPublisher(RabbitMqPublisherSettings settings, ILogFactory logFactory)
        {
            _settings = settings;
            _logFactory = logFactory;
        }

        public void Start()
        {
            var settings = RabbitMqSubscriptionSettings.CreateForPublisher(_settings.ConnectionString, _settings.ExchangeOrderBook);
            settings.ExchangeName = _settings.ExchangeOrderBook;
            settings.DeadLetterExchangeName = string.Empty;
            settings.IsDurable = false;
            
            _publisher = new RabbitMqPublisher<OrderBook>(_logFactory, settings, false)
                .DisableInMemoryQueuePersistence()
                .SetSerializer(new JsonMessageSerializer<OrderBook>())
                .SetPublishStrategy(new DefaultFanoutPublishStrategy(settings))
                .SetConsole(new LogToConsole());

            _publisher.Start();
        }

        public void Stop()
        {
            _publisher?.Stop();
        }

        public async Task Send(OrderBook orderBook)
        {
            orderBook.Source = _settings.SourceName;
            await _publisher.ProduceAsync(orderBook);
        }

        public void Dispose()
        {
            _publisher?.Dispose();
        }
    }
}
