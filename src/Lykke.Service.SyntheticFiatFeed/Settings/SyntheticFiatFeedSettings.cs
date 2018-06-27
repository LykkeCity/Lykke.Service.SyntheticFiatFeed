using JetBrains.Annotations;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.SyntheticFiatFeed.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class SyntheticFiatFeedSettings
    {
        public DbSettings Db { get; set; }
    }
}
