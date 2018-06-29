using System.Threading.Tasks;
using Autofac;
using Common;
using Common.Log;
using Lykke.Common.ExchangeAdapter.Contracts;
using Lykke.RabbitMqBroker.Publisher;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Service.SyntheticFiatFeed.Core.Services;
using Lykke.Service.SyntheticFiatFeed.Settings;

namespace Lykke.Service.SyntheticFiatFeed.RabbitMq
{
    public class OrderBookProvider: IOrderBookProvider, IStartable, IStopable
    {
        private readonly RabbitMqPublisherSettings _rabbitMqPublisherSettings;
        private readonly ILog _log;
        private RabbitMqPublisher<OrderBook> _publisher;

        public OrderBookProvider(RabbitMqPublisherSettings rabbitMqPublisherSettings, ILog log)
        {
            _rabbitMqPublisherSettings = rabbitMqPublisherSettings;
            _log = log;
        }

        public void Start()
        {
            var settings = RabbitMqSubscriptionSettings.CreateForPublisher(_rabbitMqPublisherSettings.ConnectionString, _rabbitMqPublisherSettings.Exchange);
            settings.IsDurable = false;
            settings.DeadLetterExchangeName = null;
            settings.ExchangeName = _rabbitMqPublisherSettings.Exchange;

            _publisher = new RabbitMqPublisher<Lykke.Common.ExchangeAdapter.Contracts.OrderBook>(settings)
                .DisableInMemoryQueuePersistence()
                .SetSerializer(new JsonMessageSerializer<Lykke.Common.ExchangeAdapter.Contracts.OrderBook>())
                .SetPublishStrategy(new DefaultFanoutPublishStrategy(settings))
                .SetLogger(_log)
                .SetConsole(new LogToConsole())
                .Start();
        }

        public void Dispose()
        {
            _publisher?.Dispose();
        }

        public void Stop()
        {
            _publisher.Stop();
        }

        public Task Publish(OrderBook orderBook)
        {
            return _publisher.ProduceAsync(orderBook);
        }
    }
}
