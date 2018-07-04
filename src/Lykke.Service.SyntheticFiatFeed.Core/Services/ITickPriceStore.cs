using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Common.ExchangeAdapter.Contracts;

namespace Lykke.Service.SyntheticFiatFeed.Core.Services
{
    public interface ITickPriceStore
    {
        TickPrice GetTickPrice(string exchangeName, string assetPair);
        IReadOnlyList<TickPrice> GetTickPricesByAssetPair(string assetPair);
        IReadOnlyList<TickPrice> GetTickPricesByExchange(string exchange);
        List<string> GetExchangeList();

        Task Handle(TickPrice tickPrice);
    }
}
