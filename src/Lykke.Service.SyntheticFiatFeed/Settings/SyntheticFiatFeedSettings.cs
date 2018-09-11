using System.Collections.Generic;
using JetBrains.Annotations;

namespace Lykke.Service.SyntheticFiatFeed.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class SyntheticFiatFeedSettings
    {
        public DbSettings Db { get; set; }

        public List<RabbitMqExchangeSource> ExchangeSourceList { get; set; }

        public RabbitMqPublisherSettings ExchangePublisherSettings { get; set; }
    }
}
