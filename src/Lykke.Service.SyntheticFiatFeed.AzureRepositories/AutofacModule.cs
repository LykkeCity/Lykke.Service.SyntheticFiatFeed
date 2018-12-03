using Autofac;
using AzureStorage.Tables;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Lykke.Service.SyntheticFiatFeed.Domain.Repositories;
using Lykke.SettingsReader;

namespace Lykke.Service.SyntheticFiatFeed.AzureRepositories
{
    [UsedImplicitly]
    public class AutofacModule : Module
    {
        private readonly IReloadingManager<string> _connectionString;

        public AutofacModule(IReloadingManager<string> connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c =>
                    new SimBaseInstrumentSettingRepository(AzureTableStorage<SimBaseInstrumentSettingEntity>.Create(
                        _connectionString, "SimSettings", c.Resolve<ILogFactory>())))
                .As<ISimBaseInstrumentSettingRepository>()
                .SingleInstance();

            builder.Register(c =>
                    new ExchangeCommissionSettingRepository(AzureTableStorage<ExchangeCommissionSettingEntity>.Create(
                        _connectionString, "ExchangeCommissionSettings", c.Resolve<ILogFactory>())))
                .As<IExchangeCommissionSettingRepository>()
                .SingleInstance();
        }
    }
}
