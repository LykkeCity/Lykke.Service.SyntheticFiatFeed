using System;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Lykke.Service.SyntheticFiatFeed.Client.Models.TickPrice
{
    /// <summary>
    /// Represents tick price data.
    /// </summary>
    [PublicAPI]
    public class TickPriceModel
    {
        /// <summary>
        /// The source exchange.
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// The asset pair identifier.
        /// </summary>
        public string AssetPair { get; set; }

        /// <summary>
        /// The timestamp of the tick price.
        /// </summary>
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// The ask price value.
        /// </summary>
        public decimal Ask { get; set; }

        /// <summary>
        /// The bid price value.
        /// </summary>
        public decimal Bid { get; set; }
    }
}
