using System.Collections.Generic;

namespace Lykke.Service.SyntheticFiatFeed.Services
{
    public sealed class TickPriceInput
    {
        public bool Enabled { get; set; }
        public string Name { get; set; }
        public string Exchanger { get; set; }
        public IReadOnlyCollection<string> Crypto { get; set; }
    }
}
