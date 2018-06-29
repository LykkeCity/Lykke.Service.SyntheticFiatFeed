using System;
using System.Threading.Tasks;
using Autofac;
using Common;
using Common.Log;
using Lykke.Common.ExchangeAdapter.Contracts;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Service.SyntheticFiatFeed.Core.Services;
using Lykke.Service.SyntheticFiatFeed.Services.SynCoins;

namespace Lykke.Service.SyntheticFiatFeed.RabbitMq
{
    public class TickPriceSubscriber : IDisposable, IStartable, IStopable
    {
        private readonly ITickPriceHandler _tickPriceHandler;
        private readonly RabbitMqSettings _rabbitMqSettings;
        private readonly ILog _log;
        private RabbitMqSubscriber<Lykke.Common.ExchangeAdapter.Contracts.TickPrice> _subscriber;

        public TickPriceSubscriber(ITickPriceHandler tickPriceHandler, RabbitMqSettings rabbitMqSettings, ILog log)
        {
            _tickPriceHandler = tickPriceHandler;
            _rabbitMqSettings = rabbitMqSettings;
            _log = log;
        }

        public void Start()
        {
            var settings = RabbitMqSubscriptionSettings.CreateForSubscriber(_rabbitMqSettings.ConnectionString, _rabbitMqSettings.Exchange, _rabbitMqSettings.QueueSuffix);
            settings.IsDurable = false;
            settings.DeadLetterExchangeName = null;
            settings.QueueName = $"{_rabbitMqSettings.Exchange}.{_rabbitMqSettings.QueueSuffix}";
            settings.ExchangeName = _rabbitMqSettings.Exchange;

            _subscriber = new RabbitMqSubscriber<Lykke.Common.ExchangeAdapter.Contracts.TickPrice>(settings,
                    new ResilientErrorHandlingStrategy(_log, settings, TimeSpan.FromSeconds(10)))
                .SetMessageDeserializer(new JsonMessageDeserializer<Lykke.Common.ExchangeAdapter.Contracts.TickPrice>())
                .SetMessageReadStrategy(new MessageReadQueueStrategy())
                .Subscribe(ProcessMessageAsync)
                .CreateDefaultBinding()
                .SetLogger(_log)
                .Start();
        }

        private Task ProcessMessageAsync(TickPrice arg)
        {
            return _tickPriceHandler.Handle(arg);
        }

        public void Stop()
        {
            _subscriber.Stop();
        }

        public void Dispose()
        {
            _subscriber?.Dispose();
        }
    }
}