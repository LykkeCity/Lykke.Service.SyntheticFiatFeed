using System;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Common.ExchangeAdapter.Contracts;
using Lykke.Common.Log;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Service.SyntheticFiatFeed.Core.Services;
using Lykke.Service.SyntheticFiatFeed.Settings;

namespace Lykke.Service.SyntheticFiatFeed.RabbitMq
{
    public class TickPriceSubscriber
    {
        private readonly ITickPriceHandler[] _tickPriceHandler;
        private readonly RabbitMqExchangeSource _settings;
        private readonly ILogFactory _logFactory;
        private readonly ILog _log;
        private RabbitMqSubscriber<TickPrice> _subscriber;

        public TickPriceSubscriber(ITickPriceHandler[] tickPriceHandler, RabbitMqExchangeSource settings, ILogFactory logFactory)
        {
            _tickPriceHandler = tickPriceHandler;
            _settings = settings;
            _logFactory = logFactory;
            _log = _logFactory.CreateLog(this);
        }

        public void Start()
        {
            var settings = new RabbitMqSubscriptionSettings
            {
                ConnectionString = _settings.ConnectionString,
                ExchangeName = _settings.Exchange,
                QueueName = $"{_settings.Exchange}.{_settings.QueueSuffix}",
                IsDurable = false,
                DeadLetterExchangeName = null
            };

            _subscriber = new RabbitMqSubscriber<TickPrice>(
                    _logFactory,
                    settings,
                    new ResilientErrorHandlingStrategy(
                        settings: settings,
                        logFactory: _logFactory,
                        retryTimeout: TimeSpan.FromSeconds(10),
                        next: new DeadQueueErrorHandlingStrategy(_logFactory, settings)))
                .SetMessageDeserializer(new JsonMessageDeserializer<TickPrice>())
                .Subscribe(HandleTickPrice)
                .CreateDefaultBinding();

            _subscriber.Start();
        }

        public void Stop()
        {
            _subscriber?.Stop();
        }

        private async Task HandleTickPrice(TickPrice arg)
        {
            if (!string.IsNullOrEmpty(_settings.Name))
            {
                arg.Source = _settings.Name;
            }

            foreach (var tickPriceHandler in _tickPriceHandler)
            {
                try
                {
                    await tickPriceHandler.Handle(arg);
                }
                catch (Exception ex)
                {
                    _log.Error(nameof(HandleTickPrice), ex, context: new
                    {
                        _settings.Exchange,
                        Packet = arg,
                        Handler = tickPriceHandler.GetType().FullName
                    }.ToJson());
                }
            }
        }
    }
}
