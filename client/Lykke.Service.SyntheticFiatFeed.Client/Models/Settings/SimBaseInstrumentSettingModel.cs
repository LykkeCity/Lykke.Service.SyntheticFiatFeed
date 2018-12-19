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
        /// The Alias for send price with alternative asset pair.
        /// </summary>
        string Alias { get; }

        /// <summary>
        /// The coefficient for multiply (scale) price 
        /// </summary>
        decimal PriceCoef { get; }

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
