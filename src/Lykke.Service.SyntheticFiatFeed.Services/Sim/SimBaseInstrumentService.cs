using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Common.PasswordTools;
using Lykke.Common.ExchangeAdapter.Contracts;
using Lykke.Common.Log;
using Lykke.Service.SyntheticFiatFeed.Core.Domain;
using Lykke.Service.SyntheticFiatFeed.Core.Services;

namespace Lykke.Service.SyntheticFiatFeed.Services.Sim
{
    public class SimBaseInstrumentService
    {
        public const string ExchangeName = "internal";
        public const string LykkeExchangeName = "lykke";

        private readonly IOrderBookProvider _orderBookProvider;
        private readonly ITickPriceProvider _tickPriceProvider;
        private readonly ITickPriceStore _tickPriceStore;
        private readonly ISimBaseInstrumentSetting _setting;
        private readonly IExchangeCommissionSettingRepository _commissionSettingRepository;
        private readonly ILog _log;

        private readonly Dictionary<string, TickPrice> _lastPrices = new Dictionary<string, TickPrice>();

        public SimBaseInstrumentService(
            IOrderBookProvider orderBookProvider,
            ITickPriceProvider tickPriceProvider,
            ITickPriceStore tickPriceStore,
            ISimBaseInstrumentSetting setting,
            IExchangeCommissionSettingRepository commissionSettingRepository,
            ILogFactory logFactory)
        {
            _orderBookProvider = orderBookProvider;
            _tickPriceProvider = tickPriceProvider;
            _tickPriceStore = tickPriceStore;
            _setting = setting;
            _commissionSettingRepository = commissionSettingRepository;
            _log = logFactory.CreateLog(this);
        }

        public int Order => _setting.Order;
        public string Name => _setting.BaseAssetPair;

        public static decimal GetAskWithApplyComm(IExchangeCommissionSettingRepository commissionSettingRepository, TickPrice tickPrice)
        {
            var lykke = commissionSettingRepository.GetSettingsByExchange(LykkeExchangeName).GetAwaiter().GetResult();
            var exchange = commissionSettingRepository.GetSettingsByExchange(tickPrice.Source).GetAwaiter().GetResult();

            return tickPrice.Ask * (1 + exchange.TradeCommissionPerc/100) * (1 + exchange.WithdrawCommissionPerc / 100) * (1 + lykke.WithdrawCommissionPerc/100);
        }

        public static decimal GetBidWithApplyComm(IExchangeCommissionSettingRepository commissionSettingRepository, TickPrice tickPrice)
        {
            var lykke = commissionSettingRepository.GetSettingsByExchange(LykkeExchangeName).GetAwaiter().GetResult();
            var exchange = commissionSettingRepository.GetSettingsByExchange(tickPrice.Source).GetAwaiter().GetResult();

            return tickPrice.Bid * (1 - exchange.TradeCommissionPerc / 100) * (1 - exchange.WithdrawCommissionPerc / 100) * (1 - lykke.WithdrawCommissionPerc / 100);
        }

        public async Task CalculateMarket()
        {
            var baseTickPrice = _setting.SourceExchange
                .Select(e => _tickPriceStore.GetTickPrice(e, _setting.BaseAssetPair))
                .Where(e => e != null && e.Ask > 0 && e.Bid > 0 && e.Ask > e.Bid)
                .ToList();

            if (!baseTickPrice.Any())
                return;

            var minTick = 1m / (decimal) Math.Pow(10, _setting.PriceAccuracy);

            var ask = baseTickPrice.Select(e => GetBidWithApplyComm(_commissionSettingRepository, e)).Max() + minTick;
            var bid = baseTickPrice.Select(e => GetAskWithApplyComm(_commissionSettingRepository, e)).Min() - minTick;

            if (ask <= bid)
            {
                var mid = Math.Round((ask + bid) / 2, _setting.PriceAccuracy);
                ask = mid + minTick;
                bid = mid - minTick;
            }

            await SendData(ask, bid, _setting.BaseAssetPair, false);

            foreach (var cross in _setting.CrossInstrument)
            {
                var crossTick = _tickPriceStore.GetTickPrice(cross.SourceExchange, cross.CrossAssetPair);
                if (crossTick != null && crossTick.Ask > crossTick.Bid && crossTick.Bid > 0)
                {
                    var crossAsk = !cross.IsReverse
                        ? ask * crossTick.Ask
                        : ask / crossTick.Bid;

                    var crossBid = !cross.IsReverse
                        ? bid * crossTick.Bid
                        : bid / crossTick.Ask;

                    crossAsk = Math.Round(crossAsk, cross.PriceAccuracy);
                    crossBid = Math.Round(crossBid, cross.PriceAccuracy);
                    if (crossAsk == crossBid)
                        crossAsk += 1m / (decimal)Math.Pow(10, cross.PriceAccuracy);

                    await SendData(crossAsk, crossBid, cross.AssetPair, cross.IsInternal);
                }
            }
        }

        private async Task SendData(decimal ask, decimal bid, string assetPair, bool crossIsInternal)
        {
            if (_lastPrices.TryGetValue(assetPair, out var prevTickPrice)
                && ask == prevTickPrice.Ask
                && bid == prevTickPrice.Bid)
                prevTickPrice = null;

            if (prevTickPrice != null)
            {
                var prevMid = (prevTickPrice.Ask + prevTickPrice.Bid) / 2;
                var mid = (ask + bid) / 2;

                if (_setting.DangerChangePriceKoef > 0 &&
                    Math.Abs(mid - prevMid) >= _setting.DangerChangePriceKoef * mid)
                {
                    var context = new
                    {
                        AssetPair = assetPair,
                        PrevTickPrice = prevTickPrice,
                        Ask = ask,
                        Bid = bid
                    };
                    _log.Error(nameof(SendData), context: context.ToJson(),
                        message: "Danger change price, skip action");
                    return;
                }
            }

            var asks = new List<OrderBookItem>()
            {
                new OrderBookItem(ask, _setting.FakeVolume)
            };

            var bids = new List<OrderBookItem>()
            {
                new OrderBookItem(bid, _setting.FakeVolume)
            };

            var orderBook = new OrderBook(ExchangeName, assetPair, DateTime.UtcNow, asks, bids);

            var tickPrice = new TickPrice()
            {
                Source = ExchangeName,
                Asset = assetPair,
                Timestamp = orderBook.Timestamp,
                Ask = ask,
                Bid = bid
            };

            if (!crossIsInternal)
            {
                await _orderBookProvider.Send(orderBook);
                await _tickPriceProvider.Send(tickPrice);
            }

            _lastPrices[tickPrice.Asset] = tickPrice;

            await _tickPriceStore.Handle(tickPrice);
        }

    }
}
