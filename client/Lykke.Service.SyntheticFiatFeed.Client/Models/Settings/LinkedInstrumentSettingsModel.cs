using JetBrains.Annotations;

namespace Lykke.Service.SyntheticFiatFeed.Client.Models.Settings
{
    /// <summary>
    /// Represents linked instrument settings.
    /// </summary>
    [PublicAPI]
    public class LinkedInstrumentSettingsModel
    {
        /// <summary>
        /// The asset pair the settings used for.
        /// </summary>
        public string AssetPair { get; set; }

        /// <summary>
        /// The cross asset pair identifier.
        /// </summary>
        public string CrossAssetPair { get; set; }

        /// <summary>
        /// The exchange used for getting cross asset pairs.
        /// </summary>
        public string SourceExchange { get; set; }

        /// <summary>
        /// The flag that specifies if cross asset pair is inverse.
        /// </summary>
        public bool IsReverse { get; set; }

        /// <summary>
        /// The accuracy of the price.
        /// </summary>
        public int PriceAccuracy { get; set; }

        /// <summary>
        /// The flag that specifies if the linked instrument is internal.
        /// </summary>
        public bool IsInternal { get; set; }
    }
}
