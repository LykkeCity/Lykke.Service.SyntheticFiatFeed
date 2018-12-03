namespace Lykke.Service.SyntheticFiatFeed.Domain
{
    public class ExchangeCommissionSetting : IExchangeCommissionSetting
    {
        public ExchangeCommissionSetting(IExchangeCommissionSetting setting)
        {
            ExchangeName = setting.ExchangeName;
            TradeCommissionPerc = setting.TradeCommissionPerc;
            WithdrawCommissionPerc = setting.WithdrawCommissionPerc;
        }

        public ExchangeCommissionSetting()
        {
        }

        public string ExchangeName { get; set; }

        public decimal TradeCommissionPerc { get; set; }

        public decimal WithdrawCommissionPerc { get; set; }
    }
}
