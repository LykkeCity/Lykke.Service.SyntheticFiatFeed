using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Common.ExchangeAdapter.Contracts;
using Lykke.Service.SyntheticFiatFeed.Core.Services;

namespace Lykke.Service.SyntheticFiatFeed.Services.SynCoins
{
    public class SynCoinsJob : IOrderBookHandler, ITickPriceHandler
    {
        private readonly string _baseSourceExchange;
        private readonly string _crossSourceExchange;
        private readonly IOrderBookProvider _orderBookProvider;
        private OrderBook _lastBtcUSd;
        private readonly Dictionary<string, TickPrice> _lastTickPrices = new Dictionary<string, TickPrice>();
        private List<string> _pairs = new List<string>()
        {
            "EURUSD", "GBPUSD", "USDCHF", "USDJPY"
        };
        

        public SynCoinsJob(string baseSourceExchange, string crossSourceExchange, IOrderBookProvider orderBookProvider)
        {
            _baseSourceExchange = baseSourceExchange;
            _crossSourceExchange = crossSourceExchange;
            _orderBookProvider = orderBookProvider;
        }


        public async Task Handle(OrderBook orderBook)
        {
            if (orderBook.Source == _baseSourceExchange && orderBook.Asset == "BTCUSD")
            {
                _lastBtcUSd = orderBook;
                await Calculate("BTCUSD");
            }
        }

        public async Task Handle(TickPrice tickPrice)
        {
            if (tickPrice.Source == _crossSourceExchange && _pairs.Contains(tickPrice.Asset))
            {
                _lastTickPrices[tickPrice.Source] = tickPrice;
                await Calculate(tickPrice.Asset);
            }
        }

        public async Task Calculate(string changedPair)
        {
            if ((changedPair == "BTCUSD" || changedPair == "EURUSD") && (_lastBtcUSd != null) && _lastTickPrices.ContainsKey("EURUSD"))
            {
                var tp = _lastTickPrices["EURUSD"];
                var ob = Convert(_lastBtcUSd, 1 / tp.Bid, 1 / tp.Ask, "BTCEUR", 5);
                await _orderBookProvider.Publish(ob);
            }

            if ((changedPair == "BTCUSD" || changedPair == "GBPUSD") && (_lastBtcUSd != null) && _lastTickPrices.ContainsKey("GBPUSD"))
            {
                var tp = _lastTickPrices["GBPUSD"];
                var ob = Convert(_lastBtcUSd, 1 / tp.Bid, 1 / tp.Ask, "BTCGBP", 5);
                await _orderBookProvider.Publish(ob);
            }

            if ((changedPair == "BTCUSD" || changedPair == "USDCHF") && (_lastBtcUSd != null) && _lastTickPrices.ContainsKey("USDCHF"))
            {
                var tp = _lastTickPrices["USDCHF"];
                var ob = Convert(_lastBtcUSd, tp.Ask, tp.Bid, "BTCCHF", 5);
                await _orderBookProvider.Publish(ob);
            }

            if ((changedPair == "BTCUSD" || changedPair == "USDJPY") && (_lastBtcUSd != null) && _lastTickPrices.ContainsKey("USDJPY"))
            {
                var tp = _lastTickPrices["USDJPY"];
                var ob = Convert(_lastBtcUSd, tp.Ask, tp.Bid, "BTCJPY", 3);
                await _orderBookProvider.Publish(ob);
            }

            if (changedPair == "BTCUSD")
            {
                var ob = Convert(_lastBtcUSd, 1, 1, "BTCUSD", 8);
                await _orderBookProvider.Publish(ob);
            }
        }

        private OrderBook Convert(OrderBook source, decimal askKoef, decimal bidKoef, [CanBeNull] string pair, int accuracy)
        {
            var result = new OrderBook()
            {
                Source = $"Synthetic-{_baseSourceExchange}-btcusd",
                Timestamp = DateTime.UtcNow,
                Asset = pair,
                Asks = source.Asks.Select(e => new OrderBookItem(Math.Round(e.Price * askKoef, accuracy), e.Volume)),
                Bids = source.Bids.Select(e => new OrderBookItem(Math.Round(e.Price * bidKoef, accuracy), e.Volume))
            };

            return result;
        }
    }
}
