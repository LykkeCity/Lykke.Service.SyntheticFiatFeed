using JetBrains.Annotations;

namespace Lykke.Service.SyntheticFiatFeed.Client.Models.ExchangeCommission
{
    /// <summary>
    /// Structure for discribe commission value on excange
    /// </summary>
    [PublicAPI]
    public class ExchangeCommissionSettingModel
    {
        /// <summary>
        /// Exchange name
        /// </summary>
        public string ExchangeName { get; set; }

        /// <summary>
        /// average commission for make trade in %
        /// </summary>
        public decimal TradeCommissionPerc { get; set; }

        /// <summary>
        /// average commission for withdraw money or coins from exchange in %
        /// </summary>
        public decimal WithdrawCommissionPerc { get; set; }
    }
}
