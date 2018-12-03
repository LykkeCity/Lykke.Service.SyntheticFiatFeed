using Autofac;
using JetBrains.Annotations;
using Lykke.Sdk;
using Lykke.Service.SyntheticFiatFeed.Domain.Services;
using Lykke.Service.SyntheticFiatFeed.Managers;
using Lykke.Service.SyntheticFiatFeed.RabbitMq;
using Lykke.Service.SyntheticFiatFeed.Settings;
using Lykke.SettingsReader;

namespace Lykke.Service.SyntheticFiatFeed
{
    [UsedImplicitly]
    public class AutofacModule : Module
    {
        private readonly IReloadingManager<AppSettings> _settings;

        public AutofacModule(IReloadingManager<AppSettings> settings)
        {
            _settings = settings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule(new AzureRepositories.AutofacModule(
                _settings.Nested(o => o.SyntheticFiatFeedService.Db.DataConnectionString)));

            builder.RegisterModule(new DomainServices.AutofacModule());

            builder.RegisterType<StartupManager>()
                .As<IStartupManager>();

            builder.RegisterType<ShutdownManager>()
                .As<IShutdownManager>();

            RegisterRabbit(builder);
        }

        private void RegisterRabbit(ContainerBuilder builder)
        {
            foreach (RabbitMqExchangeSource rabbitMqExchangeSource in _settings.CurrentValue.SyntheticFiatFeedService
                .ExchangeSourceList)
            {
                builder.RegisterType<TickPriceSubscriber>()
                    .WithParameter(
                        new TypedParameter(
                            typeof(RabbitMqExchangeSource),
                            rabbitMqExchangeSource))
                    .AsSelf();
            }

            builder.RegisterType<TickPricePublisher>()
                .WithParameter(
                    new TypedParameter(
                        typeof(RabbitMqPublisherSettings),
                        _settings.CurrentValue.SyntheticFiatFeedService.ExchangePublisherSettings))
                .As<ITickPriceProvider>()
                .AsSelf()
                .SingleInstance();

            builder.RegisterType<OrderBookPublisher>()
                .WithParameter(
                    new TypedParameter(
                        typeof(RabbitMqPublisherSettings),
                        _settings.CurrentValue.SyntheticFiatFeedService.ExchangePublisherSettings))
                .As<IOrderBookProvider>()
                .AsSelf()
                .SingleInstance();
        }
    }
}
