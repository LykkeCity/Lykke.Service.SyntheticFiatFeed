namespace Lykke.Service.SyntheticFiatFeed.Domain
{
    /// <summary>
    /// Structure for discribe commission value on excange
    /// </summary>
    public interface IExchangeCommissionSetting
    {
        /// <summary>
        /// Exchange name
        /// </summary>
        string ExchangeName { get; }

        /// <summary>
        /// average commission for make trade in %
        /// </summary>
        decimal TradeCommissionPerc { get; }

        /// <summary>
        /// average commission for withdraw money or coins from exchange in %
        /// </summary>
        decimal WithdrawCommissionPerc { get; }
    }
}
