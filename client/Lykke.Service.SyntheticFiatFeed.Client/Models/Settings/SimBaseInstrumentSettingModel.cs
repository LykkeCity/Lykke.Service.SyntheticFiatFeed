using System.Collections.Generic;
using JetBrains.Annotations;

namespace Lykke.Service.SyntheticFiatFeed.Client.Models.Settings
{
    /// <summary>
    /// Represents base instrument settings.
    /// </summary>
    [PublicAPI]
    public class SimBaseInstrumentSettingModel
    {
        public SimBaseInstrumentSettingModel()
        {
            FixPrice = 1;
            UseFixPrice = false;
        }

        /// <summary>
        /// The asset pair the settings are used for.
        /// </summary>
        public string BaseAssetPair { get; set; }

        /// <summary>
        /// The count per second.
        /// </summary>
        public int CountPerSecond { get; set; }

        /// <summary>
        /// The accuracy used for price rounding.
        /// </summary>
        public int PriceAccuracy { get; set; }

        /// <summary>
        /// The volume value.
        /// </summary>
        public decimal FakeVolume { get; set; }

        /// <summary>
        /// The koefficient of danger price change.
        /// </summary>
        public decimal DangerChangePriceKoef { get; set; }

        /// <summary>
        /// The numeric order of the base instrument.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// The value which specifies use external spread or not.
        /// </summary>
        public bool UseExternalSpread { get; set; }

        /// <summary>
        /// Use max spread between max ask and min bid.
        /// </summary>
        public bool UseHardGlobalSpread { get; set; }

        /// <summary>
        /// The Alias for send price with alternative asset pair.
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// The coefficient for multiply (scale) price 
        /// </summary>
        public decimal PriceCoef { get; set; }

        /// <summary>
        /// Generate ticks with fix price
        /// </summary>
        public bool UseFixPrice { get; set; }

        /// <summary>
        /// Fix price for pair, if activate flag - UseFixPrice
        /// </summary>
        public decimal FixPrice { get; set; }
        
        /// <summary>
        /// The collection of source exchanges.
        /// </summary>
        public IReadOnlyCollection<string> SourceExchange { get; set; }

        /// <summary>
        /// The collection of cross-instruments.
        /// </summary>
        public IReadOnlyCollection<LinkedInstrumentSettingsModel> CrossInstrument { get; set; }
    }
}
