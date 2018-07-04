using Autofac;
using AzureStorage.Tables;
using Lykke.Common.ExchangeAdapter.Contracts;
using Lykke.Common.Log;
using Lykke.Sdk;
using Lykke.Service.SyntheticFiatFeed.AzureRepositories;
using Lykke.Service.SyntheticFiatFeed.Core.Services;
using Lykke.Service.SyntheticFiatFeed.Managers;
using Lykke.Service.SyntheticFiatFeed.RabbitMq;
using Lykke.Service.SyntheticFiatFeed.Services;
using Lykke.Service.SyntheticFiatFeed.Services.Sim;
using Lykke.Service.SyntheticFiatFeed.Settings;
using Lykke.SettingsReader;
using Microsoft.Extensions.Hosting;

namespace Lykke.Service.SyntheticFiatFeed.Modules
{
    public class ServiceModule : Module
    {
        private readonly IReloadingManager<AppSettings> _appSettings;

        public ServiceModule(IReloadingManager<AppSettings> appSettings)
        {
            _appSettings = appSettings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<StartupManager>()
                .As<IStartupManager>();

            builder.RegisterType<ShutdownManager>()
                .As<IShutdownManager>();

            foreach (var rabbitMqExchangeSource in _appSettings.CurrentValue.SyntheticFiatFeedService.ExchangeSourceList)
            {
                builder.RegisterType<TickPriceSubscriber>()
                    .WithParameter(
                        new TypedParameter(
                            typeof(RabbitMqExchangeSource),
                            rabbitMqExchangeSource))
                    .AsSelf();
            }

            builder.RegisterType<TickPriceStore>()
                .As<ITickPriceStore>()
                .As<ITickPriceHandler>()
                .SingleInstance();


            builder.Register(c =>
                    new SimBaseInstrumentSettingRepository(AzureTableStorage<SimBaseInstrumentSettingEntity>.Create(
                        _appSettings.Nested(e => e.SyntheticFiatFeedService.Db.DataConnString),
                        "SimSettings",
                        c.Resolve<ILogFactory>())))
                .As<ISimBaseInstrumentSettingRepository>()
                .SingleInstance();

            builder.Register(c =>
                    new ExchangeCommissionSettingRepository(AzureTableStorage<ExchangeCommissionSettingEntity>.Create(
                        _appSettings.Nested(e => e.SyntheticFiatFeedService.Db.DataConnString),
                        "ExchangeCommissionSettings",
                        c.Resolve<ILogFactory>())))
                .As<IExchangeCommissionSettingRepository>()
                .SingleInstance();

            builder.RegisterType<SimService>()
                .AsSelf()
                .SingleInstance();

            builder.RegisterType<TickPricePublisher>()
                .WithParameter(
                    new TypedParameter(
                        typeof(RabbitMqPublisherSettings),
                        _appSettings.CurrentValue.SyntheticFiatFeedService.ExchangePublisherSettings))
                .As<ITickPriceProvider>()
                .AsSelf()
                .SingleInstance();

            builder.RegisterType<OrderBookPublisher>()
                .WithParameter(
                    new TypedParameter(
                        typeof(RabbitMqPublisherSettings),
                        _appSettings.CurrentValue.SyntheticFiatFeedService.ExchangePublisherSettings))
                .As<IOrderBookProvider>()
                .AsSelf()
                .SingleInstance();

            
            // Do not register entire settings in container, pass necessary settings to services which requires them
            builder.RegisterType<SyntheticTicksPublishingService>()
                .As<IHostedService>()
                .WithParameter(
                    new TypedParameter(
                        typeof(TickPriceSettings),
                        _appSettings.CurrentValue.SyntheticFiatFeedService.TickPrices))
                .SingleInstance();

            builder.RegisterType<OrderbookGeneratorService>()
                .As<IHostedService>()
                .WithParameter(
                    new TypedParameter(
                        typeof(OrderbooksSettings),
                        _appSettings.CurrentValue.SyntheticFiatFeedService.OrderBooks))
                .SingleInstance();
        }
    }
}
