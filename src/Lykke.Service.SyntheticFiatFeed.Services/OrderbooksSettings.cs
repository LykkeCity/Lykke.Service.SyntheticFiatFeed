using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.SyntheticFiatFeed.Services
{
    public sealed class OrderbooksSettings
    {
        [AmqpCheck]
        public string RabbitMq { get; set; }

        public string OutputExchanger { get; set; }

        public string FiatTickPricesExchanger { get; set; }

        [Optional]
        public string FiatTicksSourceFilter { get; set; }

        public string CryptoCurrency { get; set; }

        public string SourceName { get; set; }

        public CrossFiat[] CrossFiatRates { get; set; }
    }
}
