using System.Collections.Generic;
using System.Linq;

namespace Lykke.Service.SyntheticFiatFeed.Domain
{
    public class SimBaseInstrumentSetting : ISimBaseInstrumentSetting
    {
        public SimBaseInstrumentSetting()
        {
            CrossInstrument = new List<LinkedInstrumentSettings>();
            SourceExchange = new List<string>();
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
        }

        public string BaseAssetPair { get; set; }

        public int CountPerSecond { get; set; }

        public int PriceAccuracy { get; set; }

        public decimal FakeVolume { get; set; }

        public decimal DangerChangePriceKoef { get; set; }

        public int Order { get; set; }

        public bool UseExternalSpread { get; set; }

        public IReadOnlyList<string> SourceExchange { get; set; }

        public IReadOnlyList<ILinkedInstrumentSettings> CrossInstrument { get; set; }
    }
}
