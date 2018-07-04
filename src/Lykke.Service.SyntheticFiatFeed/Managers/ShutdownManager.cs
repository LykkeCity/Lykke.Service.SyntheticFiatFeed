﻿using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Sdk;
using Lykke.Service.SyntheticFiatFeed.RabbitMq;
using Lykke.Service.SyntheticFiatFeed.Services.Sim;

namespace Lykke.Service.SyntheticFiatFeed.Managers
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class ShutdownManager : IShutdownManager
    {
        private readonly OrderBookSubscriber[] _orderBookSubscribers;
        private readonly TickPriceSubscriber[] _tickPriceSubscribers;
        private readonly SimService _simService;
        private readonly TickPricePublisher _tickPricePublisher;
        private readonly OrderBookPublisher _orderBookPublisher;

        public ShutdownManager(
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

        public Task StopAsync()
        {
            _simService.Stop();

            foreach (var subscriber in _orderBookSubscribers)
            {
                subscriber.Stop();
            }

            foreach (var subscriber in _tickPriceSubscribers)
            {
                subscriber.Stop();
            }

            _tickPricePublisher.Stop();

            _orderBookPublisher.Stop();

            return Task.CompletedTask;
        }
    }
}
