﻿using System.Collections.Generic;

namespace Lykke.Service.SyntheticFiatFeed.Domain
{
    public interface ISimBaseInstrumentSetting
    {
        string BaseAssetPair { get; }

        int CountPerSecond { get; }

        int PriceAccuracy { get; }

        decimal FakeVolume { get; }

        decimal DangerChangePriceKoef { get; }

        int Order { get; }

        bool UseExternalSpread { get; }

        bool UseHardGlobalSpread { get; }

        string Alias { get; }

        decimal PriceCoef { get; }

        bool UseFixPrice { get; }

        decimal FixPrice { get; }

        IReadOnlyList<string> SourceExchange { get; }

        IReadOnlyList<ILinkedInstrumentSettings> CrossInstrument { get; }
    }
}
