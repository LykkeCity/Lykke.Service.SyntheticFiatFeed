using Autofac;
using Common.Log;
using Lykke.Service.SyntheticFiatFeed.Core.Services;
using Lykke.Service.SyntheticFiatFeed.RabbitMq;
using Lykke.Service.SyntheticFiatFeed.Services;
using Lykke.Service.SyntheticFiatFeed.Services.SynCoins;
using Lykke.Service.SyntheticFiatFeed.Settings;
using Lykke.SettingsReader;
using Microsoft.Extensions.Hosting;

namespace Lykke.Service.SyntheticFiatFeed.Modules
{
    public class ServiceModule : Module
    {
        private readonly IReloadingManager<AppSettings> _appSettings;
        private readonly ILog _log;

        public ServiceModule(IReloadingManager<AppSettings> appSettings, ILog log)
        {
            _appSettings = appSettings;
            _log = log;
        }

        protected override void Load(ContainerBuilder builder)
        {
            // Do not register entire settings in container, pass necessary settings to services which requires them
            builder.RegisterType<SyntheticTicksPublishingService>()
                .As<IHostedService>()
                .WithParameter(
                    new TypedParameter(
                        typeof(TickPriceSettings),
                        _appSettings.CurrentValue.SyntheticFiatFeedService.TickPrices))
                .SingleInstance();

            var settings = _appSettings.CurrentValue.SyntheticFiatFeedService.SynCoins;


            var publisher = new OrderBookProvider(settings.Publisher, _log);

            builder.RegisterInstance(publisher)
                .As<IOrderBookProvider>()
                .AutoActivate()
                .SingleInstance();

            builder.RegisterInstance(new SynCoinsJob(settings.BaseSourceExchange, settings.CrosssourceExchange,
                    publisher))
                .As<IOrderBookHandler>()
                .As<ITickPriceHandler>()
                .SingleInstance();

            builder.RegisterType<OrderBookSubscriber>()
                .WithParameter(
                    new TypedParameter(
                        typeof(RabbitMqSettings),
                        settings.ExternalExchange))
                .AutoActivate()
                .SingleInstance();

            builder.RegisterType<TickPriceSubscriber>()
                .WithParameter(
                    new TypedParameter(
                        typeof(RabbitMqSettings),
                        settings.LykkeExchange))
                .AutoActivate()
                .SingleInstance();
        }
    }
}
