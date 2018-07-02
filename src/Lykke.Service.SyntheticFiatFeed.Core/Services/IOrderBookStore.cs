using System.Collections.Generic;
using Lykke.Common.ExchangeAdapter.Contracts;

namespace Lykke.Service.SyntheticFiatFeed.Core.Services
{
    public interface IOrderBookStore
    {
        OrderBook GetOrderBook(string exchangeName, string assetPair);
        IReadOnlyList<OrderBook> GetOrderBooksByAssetPair(string assetPair);
        IReadOnlyList<OrderBook> GetOrderBooksByExchange(string exchange);
        List<string> GetExchangeList();
    }
}
