using System.Collections.Generic;

namespace Lykke.Service.SyntheticFiatFeed.Core.Domain
{
    public interface ISimBaseInstrumentSetting
    {
        string BaseAssetPair { get; }
        IReadOnlyList<string> SourceExchange { get; }
        int CountPerSecond { get; }
        int PriceAccuracy { get; }
        decimal FakeVolume { get; }
        IReadOnlyList<ILinkedInstrumentSettings> CrossInstrument { get; }
        decimal DangerChangePriceKoef { get; }
        int Order { get; }
        bool UseExternalSpread { get; }
        string MainSource { get; }
        bool UseMainSourceAsBase { get; }
    }
}
