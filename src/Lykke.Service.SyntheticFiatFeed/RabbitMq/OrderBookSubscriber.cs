using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using Common;
using Common.Log;
using Lykke.Common.ExchangeAdapter.Contracts;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Service.SyntheticFiatFeed.Core.Services;
using Lykke.Service.SyntheticFiatFeed.Services.SynCoins;
using Lykke.Service.SyntheticFiatFeed.Settings;

namespace Lykke.Service.SyntheticFiatFeed.RabbitMq
{
    public class OrderBookSubscriber :IDisposable, IStartable, IStopable
    {
        private readonly IOrderBookHandler _orderBookHandler;
        private readonly RabbitMqSettings _rabbitMqSettings;
        private readonly ILog _log;
        private RabbitMqSubscriber<Lykke.Common.ExchangeAdapter.Contracts.OrderBook> _subscriber;

        public OrderBookSubscriber(IOrderBookHandler orderBookHandler, RabbitMqSettings rabbitMqSettings, ILog log)
        {
            _orderBookHandler = orderBookHandler;
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

            _subscriber = new RabbitMqSubscriber<Lykke.Common.ExchangeAdapter.Contracts.OrderBook>(settings,
                    new ResilientErrorHandlingStrategy(_log, settings, TimeSpan.FromSeconds(10)))
                .SetMessageDeserializer(new JsonMessageDeserializer<Lykke.Common.ExchangeAdapter.Contracts.OrderBook>())
                .SetMessageReadStrategy(new MessageReadQueueStrategy())
                .Subscribe(ProcessMessageAsync)
                .CreateDefaultBinding()
                .SetLogger(_log)
                .Start();
        }

        private Task ProcessMessageAsync(OrderBook arg)
        {
            return _orderBookHandler.Handle(arg);
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
