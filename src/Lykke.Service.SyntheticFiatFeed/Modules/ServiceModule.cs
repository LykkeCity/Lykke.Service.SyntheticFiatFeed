using Autofac;
using Lykke.Common.ExchangeAdapter.Contracts;
using Lykke.Service.SyntheticFiatFeed.Services;
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
