using Autofac;
using JetBrains.Annotations;
using Lykke.Service.SyntheticFiatFeed.Domain.Services;
using Lykke.Service.SyntheticFiatFeed.DomainServices.Sim;

namespace Lykke.Service.SyntheticFiatFeed.DomainServices
{
    [UsedImplicitly]
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<TickPriceStore>()
                .As<ITickPriceStore>()
                .As<ITickPriceHandler>()
                .SingleInstance();

            builder.RegisterType<SimService>()
                .AsSelf()
                .SingleInstance();
        }
    }
}
