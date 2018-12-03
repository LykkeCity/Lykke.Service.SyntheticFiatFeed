using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.SyntheticFiatFeed.Client.Models.Settings;
using Refit;

namespace Lykke.Service.SyntheticFiatFeed.Client.Api
{
    /// <summary>
    /// Provides method to work with service's settings.
    /// </summary>
    [PublicAPI]
    public interface ISettingsApi
    {
        /// <summary>
        /// Return a collection of instrument settings.
        /// </summary>
        /// <returns>A collection of instrument settings.</returns>
        [Get("/api/settings")]
        Task<IReadOnlyCollection<SimBaseInstrumentSettingModel>> GetAllSettingsAsync();

        /// <summary>
        /// Returns settings for the specified asset par.
        /// </summary>
        /// <param name="assetPair">The asset pair name.</param>
        /// <returns>The instrument settings.</returns>
        [Get("/api/settings/{assetPair}")]
        Task<SimBaseInstrumentSettingModel> GetSettingsAsync(string assetPair);

        /// <summary>
        /// Updates instrument settings.
        /// </summary>
        /// <param name="model">Model that describes instrument settings.</param>
        [Post("/api/settings")]
        Task SetSettingsAsync(SimBaseInstrumentSettingModel model);

        /// <summary>
        /// Updates instrument settings.
        /// </summary>
        /// <param name="model">A collection of instrument settings.</param>
        [Post("/api/settings/SetMaySettings")]
        Task SetMaySettingsAsync(IReadOnlyCollection<SimBaseInstrumentSettingModel> model);
    }
}
