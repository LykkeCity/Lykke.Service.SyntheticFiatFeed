using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Common.ExchangeAdapter.Contracts;
using Lykke.Service.SyntheticFiatFeed.Core.Services;

namespace Lykke.Service.SyntheticFiatFeed.Services.Sim
{
    public class TickPriceStore : ITickPriceStore, ITickPriceHandler
    {
        private readonly object _gate = new object();
        private readonly Dictionary<string, Dictionary<string, TickPrice>> _lastTickPrices = new Dictionary<string, Dictionary<string, TickPrice>>();
        
        public Task Handle(TickPrice tickPrice)
        {
            lock (_gate)
            {


                if (!_lastTickPrices.ContainsKey(tickPrice.Source))
                {
                    _lastTickPrices[tickPrice.Source] = new Dictionary<string, TickPrice>
                    {
                        [tickPrice.Asset] = tickPrice
                    };
                }
                else
                {
                    _lastTickPrices[tickPrice.Source][tickPrice.Asset] = tickPrice;
                }

                return Task.CompletedTask;
            }
        }


        public TickPrice GetTickPrice(string exchangeName, string assetPair)
        {
            lock (_gate)
            {
                if (_lastTickPrices.ContainsKey(exchangeName) && _lastTickPrices[exchangeName].ContainsKey(assetPair))
                {
                    return _lastTickPrices[exchangeName][assetPair];
                }

                return null;
            }
        }

        public IReadOnlyList<TickPrice> GetTickPricesByAssetPair(string assetPair)
        {
            lock (_gate)
            {
                return _lastTickPrices
                    .Values
                    .SelectMany(e => e.Where(i => i.Key == assetPair).Select(i => i.Value))
                    .ToList();
            }
        }

        public IReadOnlyList<TickPrice> GetTickPricesByExchange(string exchange)
        {
            lock (_gate)
            {
                if (_lastTickPrices.ContainsKey(exchange))
                {
                    return _lastTickPrices[exchange].Values.ToList();
                }

                return new List<TickPrice>();
            }
        }

        public List<string> GetExchangeList()
        {
            lock (_gate)
            {
                return _lastTickPrices.Keys.ToList();
            }
        }
    }
}
