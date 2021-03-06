﻿using System.Collections.Generic;
using System.Linq;

namespace Lykke.Service.SyntheticFiatFeed.Domain
{
    public class SimBaseInstrumentSetting : ISimBaseInstrumentSetting
    {
        public SimBaseInstrumentSetting()
        {
            CrossInstrument = new List<LinkedInstrumentSettings>();
            SourceExchange = new List<string>();
            PriceCoef = 1;
            UseFixPrice = false;
            FixPrice = 1;
        }

        public SimBaseInstrumentSetting(ISimBaseInstrumentSetting setting)
        {
            BaseAssetPair = setting.BaseAssetPair;
            CountPerSecond = setting.CountPerSecond;
            PriceAccuracy = setting.PriceAccuracy;
            FakeVolume = setting.FakeVolume;
            DangerChangePriceKoef = setting.DangerChangePriceKoef;
            SourceExchange = setting.SourceExchange.ToList();
            CrossInstrument = setting.CrossInstrument.Select(e => new LinkedInstrumentSettings(e)).ToList();
            Order = setting.Order;
            UseExternalSpread = setting.UseExternalSpread;
            Alias = setting.Alias;
            PriceCoef = setting.PriceCoef;
            UseHardGlobalSpread = setting.UseHardGlobalSpread;
            UseFixPrice = setting.UseFixPrice;
            FixPrice = setting.FixPrice;
        }

        public string BaseAssetPair { get; set; }

        public int CountPerSecond { get; set; }

        public int PriceAccuracy { get; set; }

        public decimal FakeVolume { get; set; }

        public decimal DangerChangePriceKoef { get; set; }

        public int Order { get; set; }

        public bool UseExternalSpread { get; set; }

        public bool UseHardGlobalSpread { get; set; }

        public string Alias { get; set; }

        public decimal PriceCoef { get; set; }

        public bool UseFixPrice { get; set; }

        public decimal FixPrice { get; set; }

        public IReadOnlyList<string> SourceExchange { get; set; }

        public IReadOnlyList<ILinkedInstrumentSettings> CrossInstrument { get; set; }
    }
}
