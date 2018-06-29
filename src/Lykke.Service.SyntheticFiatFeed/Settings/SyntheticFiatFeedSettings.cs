using JetBrains.Annotations;
using Lykke.Service.SyntheticFiatFeed.Services;

namespace Lykke.Service.SyntheticFiatFeed.Settings
{

    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class SyntheticFiatFeedSettings
    {
        public DbSettings Db { get; set; }

        public TickPriceSettings TickPrices { get; set; }

        public SynCoinsSettings SynCoins { get; set; }
    }
}
