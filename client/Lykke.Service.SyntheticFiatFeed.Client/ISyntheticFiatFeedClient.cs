using JetBrains.Annotations;
using Lykke.Service.SyntheticFiatFeed.Client.Api;

namespace Lykke.Service.SyntheticFiatFeed.Client
{
    /// <summary>
    /// SyntheticFiatFeed client interface.
    /// </summary>
    [PublicAPI]
    public interface ISyntheticFiatFeedClient
    {
        /// <summary>
        /// Exchange commissions API.
        /// </summary>
        IExchangeCommissionApi ExchangeCommission { get; }

        /// <summary>
        /// Settings API.
        /// </summary>
        ISettingsApi Settings { get; }

        /// <summary>
        /// Tick price API.
        /// </summary>
        ITickPriceApi TickPrice { get; }
    }
}
