using Lykke.HttpClientGenerator;
using Lykke.Service.SyntheticFiatFeed.Client.Api;

namespace Lykke.Service.SyntheticFiatFeed.Client
{
    /// <summary>
    /// SyntheticFiatFeed API aggregating interface.
    /// </summary>
    public class SyntheticFiatFeedClient : ISyntheticFiatFeedClient
    {
        /// <summary>
        /// Initializes a new instance of <see cref="SyntheticFiatFeedClient"/> with <param name="httpClientGenerator"></param>.
        /// </summary> 
        public SyntheticFiatFeedClient(IHttpClientGenerator httpClientGenerator)
        {
            ExchangeCommission = httpClientGenerator.Generate<IExchangeCommissionApi>();
            Settings = httpClientGenerator.Generate<ISettingsApi>();
            TickPrice = httpClientGenerator.Generate<ITickPriceApi>();
        }

        /// <inheritdoc/>
        public IExchangeCommissionApi ExchangeCommission { get; }

        /// <inheritdoc/>
        public ISettingsApi Settings { get; }

        /// <inheritdoc/>
        public ITickPriceApi TickPrice { get; set; }
    }
}
