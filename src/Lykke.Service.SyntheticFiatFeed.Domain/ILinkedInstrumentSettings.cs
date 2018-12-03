namespace Lykke.Service.SyntheticFiatFeed.Domain
{
    public interface ILinkedInstrumentSettings
    {
        string AssetPair { get; }

        string CrossAssetPair { get; }

        string SourceExchange { get; }

        bool IsReverse { get; }

        int PriceAccuracy { get; }

        bool IsInternal { get; }
    }
}
