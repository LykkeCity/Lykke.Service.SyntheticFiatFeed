using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.SyntheticFiatFeed.Client.Models.ExchangeCommission;
using Refit;

namespace Lykke.Service.SyntheticFiatFeed.Client.Api
{
    /// <summary>
    /// Provides methods to work with commission settings.
    /// </summary>
    [PublicAPI]
    public interface IExchangeCommissionApi
    {
        /// <summary>
        /// Returns a collection of commission settings for exchanges.
        /// </summary>
        /// <returns>A collection of commission settings for exchanges.</returns>
        [Get("/api/exchangecommission")]
        Task<IReadOnlyCollection<ExchangeCommissionSettingModel>> GetAllSettingsAsync();

        /// <summary>
        /// Returns commission settings for specified exchange.
        /// </summary>
        /// <param name="exchange">The exchange name.</param>
        /// <returns>Commission settings model.</returns>
        [Get("/api/exchangecommission/{exchange}")]
        Task<ExchangeCommissionSettingModel> GetSettingsByExchangeAsync(string exchange);

        /// <summary>
        /// Updates commission settings.
        /// </summary>
        /// <param name="model">The model describing commission settings.</param>
        [Post("/api/exchangecommission")]
        Task SetSettingsAsync(ExchangeCommissionSettingModel model);
    }
}
