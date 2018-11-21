namespace Lykke.Service.SyntheticFiatFeed.Domain
{
    public class LinkedInstrumentSettings : ILinkedInstrumentSettings
    {
        public LinkedInstrumentSettings()
        {
        }

        public LinkedInstrumentSettings(ILinkedInstrumentSettings settings)
        {
            AssetPair = settings.AssetPair;
            CrossAssetPair = settings.CrossAssetPair;
            SourceExchange = settings.SourceExchange;
            IsReverse = settings.IsReverse;
            PriceAccuracy = settings.PriceAccuracy;
            IsInternal = settings.IsInternal;
        }

        public string AssetPair { get; set; }

        public string CrossAssetPair { get; set; }

        public string SourceExchange { get; set; }

        public bool IsReverse { get; set; }

        public int PriceAccuracy { get; set; }

        public bool IsInternal { get; set; }
    }
}
