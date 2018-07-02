using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.SyntheticFiatFeed.Settings
{
    public class RabbitMqExchangeSource
    {
        public RabbitMqExchangeSource()
        {
            Name = string.Empty;
        }

        [AmqpCheck]
        public string ConnectionString { get; set; }

        public string Exchange { get; set; }

        public string QueueSuffix { get; set; }

        [Optional]
        public string Name { get; set; }
    }
}