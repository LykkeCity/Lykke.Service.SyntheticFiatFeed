using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.SyntheticFiatFeed.Settings
{
    public class RabbitMqPublisherSettings
    {
        [AmqpCheck]
        public string ConnectionString { get; set; }

        public string ExchangeOrderBook { get; set; }

        public string ExchangeTickPrice { get; set; }

        public string SourceName { get; set; }
    }
}
