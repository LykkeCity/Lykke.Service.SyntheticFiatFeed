using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Lykke.Common.Log;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;

namespace Lykke.Service.SyntheticFiatFeed.Services
{
    public static class RmqHelper
    {
        public static IObservable<T> ReadAsJson<T>(RabbitMqSubscriptionSettings settings, ILogFactory log)
        {
            return Observable.Create<T>(async (obs, ct) =>
            {
                var subscriber = new RabbitMqSubscriber<T>(
                        log,
                        settings,
                        new ResilientErrorHandlingStrategy(
                            settings: settings,
                            logFactory: log,
                            retryTimeout: TimeSpan.FromSeconds(10),
                            next: new DeadQueueErrorHandlingStrategy(log, settings)))
                    .SetMessageDeserializer(new JsonMessageDeserializer<T>())
                    .Subscribe(x =>
                    {
                        obs.OnNext(x);
                        return Task.CompletedTask;
                    })
                    .CreateDefaultBinding();

                using (subscriber.Start())
                {
                    var cts = new TaskCompletionSource<Unit>();
                    ct.Register(() => cts.SetResult(Unit.Default));
                    await cts.Task;
                }

                obs.OnCompleted();
            });
        }
    }
}
