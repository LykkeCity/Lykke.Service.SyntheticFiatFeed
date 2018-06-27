using JetBrains.Annotations;
using Lykke.Sdk.Settings;

namespace Lykke.Service.SyntheticFiatFeed.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AppSettings : BaseAppSettings
    {
        public SyntheticFiatFeedSettings SyntheticFiatFeedService { get; set; }        
    }
}
