using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.SyntheticFiatFeed.Client 
{
    /// <summary>
    /// SyntheticFiatFeed client settings.
    /// </summary>
    public class SyntheticFiatFeedServiceClientSettings 
    {
        /// <summary>Service url.</summary>
        [HttpCheck("api/isalive")]
        public string ServiceUrl {get; set;}
    }
}
