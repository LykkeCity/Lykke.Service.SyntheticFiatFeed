using JetBrains.Annotations;

namespace Lykke.Service.SyntheticFiatFeed.Client
{
    /// <summary>
    /// SyntheticFiatFeed client interface.
    /// </summary>
    [PublicAPI]
    public interface ISyntheticFiatFeedClient
    {
        // Make your app's controller interfaces visible by adding corresponding properties here.
        // NO actual methods should be placed here (these go to controller interfaces, for example - ISyntheticFiatFeedApi).
        // ONLY properties for accessing controller interfaces are allowed.

        /// <summary>Application Api interface</summary>
        ISyntheticFiatFeedApi Api { get; }
    }
}
