using System.Collections.Generic;
using JetBrains.Annotations;
using Lykke.Service.SyntheticFiatFeed.Services;

namespace Lykke.Service.SyntheticFiatFeed.Settings
{

    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class SyntheticFiatFeedSettings
    {
        public DbSettings Db { get; set; }

        public TickPriceSettings TickPrices { get; set; }

        public OrderbooksSettings OrderBooks { get; set; }

        public List<RabbitMqExchangeSource> ExchangeSourceList { get; set; }

        public RabbitMqPublisherSettings ExchangePublisherSettings { get; set; }
    }
}
