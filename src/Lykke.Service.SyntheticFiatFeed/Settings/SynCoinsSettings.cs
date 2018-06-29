using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.SyntheticFiatFeed.Settings
{
    public class SynCoinsSettings
    {
        public string BaseSourceExchange { get; set; }
        public string CrosssourceExchange { get; set; }
        public RabbitMqSettings ExternalExchange { get; set; }
        public RabbitMqSettings LykkeExchange { get; set; }
        public RabbitMqPublisherSettings Publisher { get; set; }
    }

    public class RabbitMqSettings
    {
        [AmqpCheck]
        public string ConnectionString { get; set; }

        public string Exchange { get; set; }

        public string QueueSuffix { get; set; }
    }

    public class RabbitMqPublisherSettings
    {
        [AmqpCheck]
        public string ConnectionString { get; set; }

        public string Exchange { get; set; }
    }
}
