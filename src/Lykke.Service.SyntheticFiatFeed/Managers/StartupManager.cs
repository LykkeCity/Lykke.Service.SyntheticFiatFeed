using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Sdk;
using Lykke.Service.SyntheticFiatFeed.RabbitMq;

namespace Lykke.Service.SyntheticFiatFeed.Managers
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class StartupManager : IStartupManager
    {
        private readonly OrderBookSubscriber[] _subscribers;

        public StartupManager(OrderBookSubscriber[] subscribers)
        {
            _subscribers = subscribers;
        }

        public Task StartAsync()
        {
            foreach (var orderBookSubscriber in _subscribers)
            {
                orderBookSubscriber.Start();
            }

            return Task.CompletedTask;
        }
    }

    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class ShutdownManager : IShutdownManager
    {
        private readonly OrderBookSubscriber[] _subscribers;

        public ShutdownManager(OrderBookSubscriber[] subscribers)
        {
            _subscribers = subscribers;
        }

        public Task StopAsync()
        {
            foreach (var orderBookSubscriber in _subscribers)
            {
                orderBookSubscriber.Start();
            }

            return Task.CompletedTask;
        }
    }
}
