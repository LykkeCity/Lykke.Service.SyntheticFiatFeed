using Lykke.HttpClientGenerator;

namespace Lykke.Service.SyntheticFiatFeed.Client
{
    /// <summary>
    /// SyntheticFiatFeed API aggregating interface.
    /// </summary>
    public class SyntheticFiatFeedClient : ISyntheticFiatFeedClient
    {
        // Note: Add similar Api properties for each new service controller

        /// <summary>Inerface to SyntheticFiatFeed Api.</summary>
        public ISyntheticFiatFeedApi Api { get; private set; }

        /// <summary>C-tor</summary>
        public SyntheticFiatFeedClient(IHttpClientGenerator httpClientGenerator)
        {
            Api = httpClientGenerator.Generate<ISyntheticFiatFeedApi>();
        }
    }
}
