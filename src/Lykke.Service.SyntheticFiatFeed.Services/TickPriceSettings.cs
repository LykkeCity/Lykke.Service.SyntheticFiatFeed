using System.Collections.Generic;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.SyntheticFiatFeed.Services
{
    public sealed class TickPriceSettings
    {
        [AmqpCheck]
        public string RabbitMq { get; set; }

        public IReadOnlyCollection<TickPriceInput> Inputs { get; set; }

        public IReadOnlyCollection<FiatAsset> Fiat { get; set; }

        public string OutputExchanger { get; set; }
    }
}
