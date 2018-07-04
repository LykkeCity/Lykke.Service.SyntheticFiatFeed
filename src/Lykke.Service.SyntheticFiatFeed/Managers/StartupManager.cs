﻿using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Sdk;
using Lykke.Service.SyntheticFiatFeed.RabbitMq;
using Lykke.Service.SyntheticFiatFeed.Services.Sim;

namespace Lykke.Service.SyntheticFiatFeed.Managers
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class StartupManager : IStartupManager
    {
        private readonly OrderBookSubscriber[] _orderBookSubscribers;
        private readonly TickPriceSubscriber[] _tickPriceSubscribers;
        private readonly SimService _simService;
        private readonly TickPricePublisher _tickPricePublisher;
        private readonly OrderBookPublisher _orderBookPublisher;

        public StartupManager(
            OrderBookSubscriber[] orderBookOrderBookSubscribers, 
            TickPriceSubscriber[] tickPriceSubscribers,
            SimService simService,
            TickPricePublisher tickPricePublisher,
            OrderBookPublisher orderBookPublisher)
        {
            _orderBookSubscribers = orderBookOrderBookSubscribers;
            _tickPriceSubscribers = tickPriceSubscribers;
            _simService = simService;
            _tickPricePublisher = tickPricePublisher;
            _orderBookPublisher = orderBookPublisher;
        }

        public Task StartAsync()
        {
            foreach (var orderBookSubscriber in _orderBookSubscribers)
            {
                orderBookSubscriber.Start();
            }

            foreach (var subscriber in _tickPriceSubscribers)
            {
                subscriber.Start();
            }

            _tickPricePublisher.Start();

            _orderBookPublisher.Start();

            _simService.Start();

            return Task.CompletedTask;
        }
    }
}
