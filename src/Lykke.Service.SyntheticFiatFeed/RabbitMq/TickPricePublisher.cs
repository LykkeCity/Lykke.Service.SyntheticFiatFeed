using System;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.ExchangeAdapter.Contracts;
using Lykke.Common.Log;
using Lykke.RabbitMqBroker.Publisher;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Service.SyntheticFiatFeed.Core.Services;
using Lykke.Service.SyntheticFiatFeed.Settings;

namespace Lykke.Service.SyntheticFiatFeed.RabbitMq
{
    public class TickPricePublisher : ITickPriceProvider, IDisposable
    {
        private readonly RabbitMqPublisherSettings _settings;
        private readonly ILogFactory _logFactory;
        private RabbitMqPublisher<TickPrice> _publisher;

        public TickPricePublisher(RabbitMqPublisherSettings settings, ILogFactory logFactory)
        {
            _settings = settings;
            _logFactory = logFactory;
        }

        public void Start()
        {
            var settings = RabbitMqSubscriptionSettings.CreateForPublisher(_settings.ConnectionString, _settings.ExchangeTickPrice);
            settings.ExchangeName = _settings.ExchangeTickPrice;
            settings.DeadLetterExchangeName = string.Empty;
            settings.IsDurable = false;


            _publisher = new RabbitMqPublisher<TickPrice>(_logFactory, settings, false)
                .DisableInMemoryQueuePersistence()
                .SetSerializer(new JsonMessageSerializer<TickPrice>())
                .SetPublishStrategy(new DefaultFanoutPublishStrategy(settings))
                .SetConsole(new LogToConsole());

            _publisher.Start();
        }

        public void Stop()
        {
            _publisher?.Stop();
        }

        public async Task Send(TickPrice tickPrice)
        {
            tickPrice.Source = _settings.SourceName;
            await _publisher.ProduceAsync(tickPrice);
        }

        public void Dispose()
        {
            _publisher?.Dispose();
        }
    }
}
