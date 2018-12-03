using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.SyntheticFiatFeed.Client.Models.TickPrice;
using Refit;

namespace Lykke.Service.SyntheticFiatFeed.Client.Api
{
    /// <summary>
    /// Provides methods to get tick prices.
    /// </summary>
    [PublicAPI]
    public interface ITickPriceApi
    {
        /// <summary>
        /// Returns the current tick price for the specified exchange and asset pair.
        /// </summary>
        /// <param name="exchange">The exchange name.</param>
        /// <param name="assetPair">The asset pair name.</param>
        /// <returns>The model that describes tick price.</returns>
        [Get("/api/tickprice/{assetPair}/{exchange}")]
        Task<TickPriceModel> GetTickPriceAsync(string exchange, string assetPair);

        /// <summary>
        /// Returns a collection of tick prices for the specified asset pair.
        /// </summary>
        /// <param name="assetPair">The asset pair name.</param>
        /// <returns>A collection of tick prices.</returns>
        [Get("/api/tickprice/assetpair/{assetPair}")]
        Task<IReadOnlyCollection<TickPriceModel>> GetTickPriceByAssetPairAsync(string assetPair);

        /// <summary>
        /// Returns a collection of tick prices for the specified exchange by all asset pairs.
        /// </summary>
        /// <param name="exchange">The exchange name.</param>
        /// <returns>A collection of tick prices.</returns>
        [Get("/api/tickprice/exchange/{exchange}")]
        Task<IReadOnlyCollection<TickPriceModel>> GetTickPriceByExchangeAsync(string exchange);

        /// <summary>
        /// Returns a collection of exchange names.
        /// </summary>
        /// <returns>A collection of exchange names.</returns>
        [Get("/api/tickprice/exchanges")]
        Task<IReadOnlyCollection<string>> GetAllExchangesAsync();
    }
}
