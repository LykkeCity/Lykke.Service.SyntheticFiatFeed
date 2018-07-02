using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Common.ExchangeAdapter.Contracts;
using Lykke.Service.SyntheticFiatFeed.Core.Services;

namespace Lykke.Service.SyntheticFiatFeed.Services.Sim
{
    public class OrderBookStore : IOrderBookStore, IOrderBookHandler
    {
        private readonly Dictionary<string, Dictionary<string, OrderBook>> _lastOrderBooks = new Dictionary<string, Dictionary<string, OrderBook>>();
        
        public Task Handle(OrderBook orderBook)
        {
            if (!_lastOrderBooks.ContainsKey(orderBook.Source))
            {
                _lastOrderBooks[orderBook.Source] = new Dictionary<string, OrderBook>
                {
                    [orderBook.Asset] = orderBook
                }; ;
            }
            else
            {
                _lastOrderBooks[orderBook.Source][orderBook.Asset] = orderBook;
            }

            return Task.CompletedTask;
        }


        public OrderBook GetOrderBook(string exchangeName, string assetPair)
        {
            if (_lastOrderBooks.ContainsKey(exchangeName) && _lastOrderBooks[exchangeName].ContainsKey(assetPair))
            {
                return _lastOrderBooks[exchangeName][assetPair];
            }

            return null;
        }

        public IReadOnlyList<OrderBook> GetOrderBooksByAssetPair(string assetPair)
        {
            return _lastOrderBooks
                .Values
                .SelectMany(e => e.Where(i => i.Key == assetPair).Select(i => i.Value))
                .ToList();
        }

        public IReadOnlyList<OrderBook> GetOrderBooksByExchange(string exchange)
        {
            if (_lastOrderBooks.ContainsKey(exchange))
            {
                return _lastOrderBooks[exchange].Values.ToList();
            }
            return new List<OrderBook>();
        }

        public List<string> GetExchangeList()
        {
            return _lastOrderBooks.Keys.ToList();
        }
    }
}
