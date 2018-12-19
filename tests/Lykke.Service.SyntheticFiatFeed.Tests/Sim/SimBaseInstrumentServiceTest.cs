using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Common.ExchangeAdapter.Contracts;
using Lykke.Logs;
using Lykke.Service.SyntheticFiatFeed.Domain;
using Lykke.Service.SyntheticFiatFeed.Domain.Repositories;
using Lykke.Service.SyntheticFiatFeed.Domain.Services;
using Lykke.Service.SyntheticFiatFeed.DomainServices.Sim;
using Moq;
using NUnit.Framework;

namespace Lykke.Service.SyntheticFiatFeed.Tests.Sim
{
    public class SimBaseInstrumentServiceTest
    {
        [Test]
        public void ProvidePriceFromOneSource()
        {
            var setting = new SimBaseInstrumentSetting();
            setting.SourceExchange.Add("bitstamp");

            var store = new Mock<ITickPriceStore>();

            store.Setup(e => e.GetTickPrice("bitstamp", "BTCUSD")).Returns(CreateCtickPrice(6000, 6100, "bitstamp", "BTCUSD"));

            var orderBookData = new FakeOrderBookProvider();
            var tickPriceData = new FakeTickPriceProvider();

            var service = new SimBaseInstrumentService(orderBookData, tickPriceData, store.Object, setting, GetEmptyCommission(), EmptyLogFactory.Instance);

            service.CalculateMarket().GetAwaiter().GetResult();

            Assert.AreEqual(1, orderBookData.Data.Count);
            Assert.AreEqual(1, tickPriceData.Data.Count);

            var ob = orderBookData.Data.First();
            var tp = tickPriceData.Data.First();

            Assert.AreEqual(1, ob.Asks.Count());
            Assert.AreEqual(1, ob.Bids.Count());
            Assert.AreEqual(6050.01m, ob.Asks.First().Price);
            Assert.AreEqual(6049.99m, ob.Bids.First().Price);
            Assert.AreEqual(setting.FakeVolume, ob.Asks.First().Volume);
            Assert.AreEqual(setting.FakeVolume, ob.Bids.First().Volume);

            Assert.AreEqual(6050.01m, tp.Ask);
            Assert.AreEqual(6049.99m, tp.Bid);
        }

        [Test]
        public void ProvidePriceFromTwoSource_WithoutArbitrage()
        {
            var setting = new SimBaseInstrumentSetting();
            setting.SourceExchange.Add("bitstamp");
            setting.SourceExchange.Add("gdax");

            var store = new Mock<ITickPriceStore>();

            store.Setup(e => e.GetTickPrice("bitstamp", "BTCUSD")).Returns(CreateCtickPrice(6000, 6100, "bitstamp", "BTCUSD"));
            store.Setup(e => e.GetTickPrice("gdax", "BTCUSD")).Returns(CreateCtickPrice(6050, 6200, "gdax", "BTCUSD"));

            var orderBookData = new FakeOrderBookProvider();
            var tickPriceData = new FakeTickPriceProvider();

            var service = new SimBaseInstrumentService(orderBookData, tickPriceData, store.Object, setting, GetEmptyCommission(), EmptyLogFactory.Instance);

            service.CalculateMarket().GetAwaiter().GetResult();
            service.CalculateMarket().GetAwaiter().GetResult();

            Assert.AreEqual(2, orderBookData.Data.Count);
            Assert.AreEqual(2, tickPriceData.Data.Count);

            var ob = orderBookData.Data.First();
            var tp = tickPriceData.Data.First();

            Assert.AreEqual(1, ob.Asks.Count());
            Assert.AreEqual(1, ob.Bids.Count());
            Assert.AreEqual(6075.01m, ob.Asks.First().Price);
            Assert.AreEqual(6074.99m, ob.Bids.First().Price);
            Assert.AreEqual(setting.FakeVolume, ob.Asks.First().Volume);
            Assert.AreEqual(setting.FakeVolume, ob.Bids.First().Volume);

            Assert.AreEqual(6075.01m, tp.Ask);
            Assert.AreEqual(6074.99m, tp.Bid);
        }

        [Test]
        public void ProvidePriceFromTwoSource_WithArbitrage()
        {
            var setting = new SimBaseInstrumentSetting();
            setting.SourceExchange.Add("bitstamp");
            setting.SourceExchange.Add("gdax");

            var store = new Mock<ITickPriceStore>();

            store.Setup(e => e.GetTickPrice("bitstamp", "BTCUSD")).Returns(CreateCtickPrice(6000, 6050, "bitstamp", "BTCUSD"));
            store.Setup(e => e.GetTickPrice("gdax", "BTCUSD")).Returns(CreateCtickPrice(6070, 6200, "gdax", "BTCUSD"));

            var orderBookData = new FakeOrderBookProvider();
            var tickPriceData = new FakeTickPriceProvider();

            var service = new SimBaseInstrumentService(orderBookData, tickPriceData, store.Object, setting, GetEmptyCommission(), EmptyLogFactory.Instance);

            service.CalculateMarket().GetAwaiter().GetResult();
            service.CalculateMarket().GetAwaiter().GetResult();

            Assert.AreEqual(2, orderBookData.Data.Count);
            Assert.AreEqual(2, tickPriceData.Data.Count);

            var ob = orderBookData.Data.First();
            var tp = tickPriceData.Data.First();

            Assert.AreEqual(1, ob.Asks.Count());
            Assert.AreEqual(1, ob.Bids.Count());
            Assert.AreEqual(6070.00m, ob.Asks.First().Price);
            Assert.AreEqual(6050.00m, ob.Bids.First().Price);
            Assert.AreEqual(setting.FakeVolume, ob.Asks.First().Volume);
            Assert.AreEqual(setting.FakeVolume, ob.Bids.First().Volume);

            Assert.AreEqual(6070.00m, tp.Ask);
            Assert.AreEqual(6050.00m, tp.Bid);
        }

        [Test]
        public void CalculateWithoutData()
        {
            var setting = new SimBaseInstrumentSetting();
            setting.SourceExchange.Add("bitfinex");
            setting.SourceExchange.Add("lykke");

            var store = new Mock<ITickPriceStore>();

            store.Setup(e => e.GetTickPrice("bitstamp", "BTCUSD")).Returns(CreateCtickPrice(6000, 6050, "bitstamp", "BTCUSD"));
            store.Setup(e => e.GetTickPrice("bitstamp", "BTCUSD")).Returns(CreateCtickPrice(6000, 6050, "gdax", "BTCUSD"));

            var orderBookData = new FakeOrderBookProvider();
            var tickPriceData = new FakeTickPriceProvider();

            var service = new SimBaseInstrumentService(orderBookData, tickPriceData, store.Object, setting, GetEmptyCommission(), EmptyLogFactory.Instance);

            service.CalculateMarket().GetAwaiter().GetResult();
            service.CalculateMarket().GetAwaiter().GetResult();

            Assert.AreEqual(0, orderBookData.Data.Count);
            Assert.AreEqual(0, tickPriceData.Data.Count);
        }

        [Test]
        public void SkipAction_IfBigChangePrice()
        {
            var setting = new SimBaseInstrumentSetting();
            setting.SourceExchange.Add("bitstamp");
            setting.DangerChangePriceKoef = 0.1m;

            var store = new Mock<ITickPriceStore>();

            store.Setup(e => e.GetTickPrice("bitstamp", "BTCUSD")).Returns(CreateCtickPrice(6049, 6051, "bitstamp", "BTCUSD"));

            var orderBookData = new FakeOrderBookProvider();
            var tickPriceData = new FakeTickPriceProvider();

            var service = new SimBaseInstrumentService(orderBookData, tickPriceData, store.Object, setting, GetEmptyCommission(), EmptyLogFactory.Instance);

            service.CalculateMarket().GetAwaiter().GetResult();

            Assert.AreEqual(1, orderBookData.Data.Count);
            Assert.AreEqual(1, tickPriceData.Data.Count);

            store.Setup(e => e.GetTickPrice("bitstamp", "BTCUSD")).Returns(CreateCtickPrice(6049, 6051, "bitstamp", "BTCUSD"));

            service.CalculateMarket().GetAwaiter().GetResult();

            Assert.AreEqual(2, orderBookData.Data.Count);
            Assert.AreEqual(2, tickPriceData.Data.Count);
        }

        [Test]
        public void ProvidePriceFromOneSource_withLinkedAssetPair()
        {
            var setting = new SimBaseInstrumentSetting();
            setting.SourceExchange.Add("bitstamp");
            setting.CrossInstrument.Add(new LinkedInstrumentSettings("BTCEUR", "EURUSD", "lykke", true));
            setting.CrossInstrument.Add(new LinkedInstrumentSettings("BTCCHF", "USDCHF", "lykke", false));

            var store = new Mock<ITickPriceStore>();

            store.Setup(e => e.GetTickPrice("bitstamp", "BTCUSD")).Returns(CreateCtickPrice(6000, 6100, "bitstamp", "BTCUSD"));
            store.Setup(e => e.GetTickPrice("lykke", "EURUSD")).Returns(CreateCtickPrice(10, 100, "lykke", "EURUSD"));
            store.Setup(e => e.GetTickPrice("lykke", "USDCHF")).Returns(CreateCtickPrice(10, 100, "lykke", "USDCHF"));

            var orderBookData = new FakeOrderBookProvider();
            var tickPriceData = new FakeTickPriceProvider();

            var service = new SimBaseInstrumentService(orderBookData, tickPriceData, store.Object, setting, GetEmptyCommission(), EmptyLogFactory.Instance);

            service.CalculateMarket().GetAwaiter().GetResult();

            Assert.AreEqual(3, orderBookData.Data.Count);
            Assert.AreEqual(3, tickPriceData.Data.Count);

            var btcusdOb = orderBookData.Data.First(e => e.Asset == "BTCUSD");
            var btcusdTp = tickPriceData.Data.First(e => e.Asset == "BTCUSD");

            var btceurOb = orderBookData.Data.First(e => e.Asset == "BTCEUR");
            var btceurTp = tickPriceData.Data.First(e => e.Asset == "BTCEUR");

            var btcchfOb = orderBookData.Data.First(e => e.Asset == "BTCCHF");
            var btcchfTp = tickPriceData.Data.First(e => e.Asset == "BTCCHF");

            // check btcusd

            Assert.AreEqual(1, btcusdOb.Asks.Count());
            Assert.AreEqual(1, btcusdOb.Bids.Count());
            Assert.AreEqual(6050.01m, btcusdOb.Asks.First().Price);
            Assert.AreEqual(6049.99m, btcusdOb.Bids.First().Price);
            Assert.AreEqual(setting.FakeVolume, btcusdOb.Asks.First().Volume);
            Assert.AreEqual(setting.FakeVolume, btcusdOb.Bids.First().Volume);

            Assert.AreEqual(6050.01m, btcusdTp.Ask);
            Assert.AreEqual(6049.99m, btcusdTp.Bid);

            // check eurusd
        
            var eurAsk = 605.00m;
            var eurBid = 60.50m;

            Assert.AreEqual(1, btceurOb.Asks.Count());
            Assert.AreEqual(1, btceurOb.Bids.Count());
            Assert.AreEqual(eurAsk, btceurOb.Asks.First().Price);
            Assert.AreEqual(eurBid, btceurOb.Bids.First().Price);
            Assert.AreEqual(setting.FakeVolume, btceurOb.Asks.First().Volume);
            Assert.AreEqual(setting.FakeVolume, btceurOb.Bids.First().Volume);

            Assert.AreEqual(eurAsk, btceurTp.Ask);
            Assert.AreEqual(eurBid, btceurTp.Bid);

            // check usdchf

            var chfAsk = 605001m;
            var chfBid = 60499.9m;

            Assert.AreEqual(1, btcchfOb.Asks.Count());
            Assert.AreEqual(1, btcchfOb.Bids.Count());
            Assert.AreEqual(chfAsk, btcchfOb.Asks.First().Price);
            Assert.AreEqual(chfBid, btcchfOb.Bids.First().Price);
            Assert.AreEqual(setting.FakeVolume, btcchfOb.Asks.First().Volume);
            Assert.AreEqual(setting.FakeVolume, btcchfOb.Bids.First().Volume);

            Assert.AreEqual(chfAsk, btcchfTp.Ask);
            Assert.AreEqual(chfBid, btcchfTp.Bid);
        }

        [Test]
        public void ApplyCommissionInPrices_1()
        {
            var setting = new SimBaseInstrumentSetting();
            setting.SourceExchange.Add("bitstamp");

            var store = new Mock<ITickPriceStore>();

            store.Setup(e => e.GetTickPrice("bitstamp", "BTCUSD")).Returns(CreateCtickPrice(6000, 6100, "bitstamp", "BTCUSD"));

            var orderBookData = new FakeOrderBookProvider();
            var tickPriceData = new FakeTickPriceProvider();

            var commisiionRepo = new Mock<IExchangeCommissionSettingRepository>();
            commisiionRepo.Setup(e => e.GetSettingsByExchange(It.IsAny<string>())).Returns(Task.FromResult<IExchangeCommissionSetting>(new FakeExchangeCommissionSetting()));
            commisiionRepo.Setup(e => e.GetSettingsByExchange("bitstamp")).Returns(Task.FromResult<IExchangeCommissionSetting>(new FakeExchangeCommissionSetting()
            {
                ExchangeName = "bitstamp",
                TradeCommissionPerc = 10,
                WithdrawCommissionPerc = 0
            }));

            var service = new SimBaseInstrumentService(orderBookData, tickPriceData, store.Object, setting, commisiionRepo.Object, EmptyLogFactory.Instance);

            service.CalculateMarket().GetAwaiter().GetResult();

            Assert.AreEqual(1, orderBookData.Data.Count);
            Assert.AreEqual(1, tickPriceData.Data.Count);

            var tp = tickPriceData.Data.First();
            Assert.AreEqual(6055.01m, tp.Ask);
            Assert.AreEqual(6054.99m, tp.Bid);
        }

        [Test]
        public void ApplyCommissionInPrices_2()
        {
            var setting = new SimBaseInstrumentSetting();
            setting.SourceExchange.Add("bitstamp");
            setting.SourceExchange.Add("aaa");

            var store = new Mock<ITickPriceStore>();

            store.Setup(e => e.GetTickPrice("bitstamp", "BTCUSD")).Returns(CreateCtickPrice(6100, 6200, "bitstamp", "BTCUSD"));
            store.Setup(e => e.GetTickPrice("aaa", "BTCUSD")).Returns(CreateCtickPrice(5000, 5050, "aaa", "BTCUSD"));

            var orderBookData = new FakeOrderBookProvider();
            var tickPriceData = new FakeTickPriceProvider();

            var commisiionRepo = new Mock<IExchangeCommissionSettingRepository>();
            commisiionRepo.Setup(e => e.GetSettingsByExchange(It.IsAny<string>())).Returns(Task.FromResult<IExchangeCommissionSetting>(new FakeExchangeCommissionSetting()));
            commisiionRepo.Setup(e => e.GetSettingsByExchange("bitstamp")).Returns(Task.FromResult<IExchangeCommissionSetting>(new FakeExchangeCommissionSetting()
            {
                ExchangeName = "bitstamp",
                TradeCommissionPerc = 10,
                WithdrawCommissionPerc = 0
            }));

            var service = new SimBaseInstrumentService(orderBookData, tickPriceData, store.Object, setting, commisiionRepo.Object, EmptyLogFactory.Instance);

            service.CalculateMarket().GetAwaiter().GetResult();

            Assert.AreEqual(1, orderBookData.Data.Count);
            Assert.AreEqual(1, tickPriceData.Data.Count);

            var tp = tickPriceData.Data.First();
            Assert.AreEqual(6100.00m-610m, tp.Ask);
            Assert.AreEqual(5050.00m, tp.Bid);
        }



        private IExchangeCommissionSettingRepository GetEmptyCommission()
        {
            var moq = new Mock<IExchangeCommissionSettingRepository>();
            moq.Setup(e => e.GetSettingsByExchange(It.IsAny<string>())).Returns(Task.FromResult<IExchangeCommissionSetting>(new FakeExchangeCommissionSetting()));
            return moq.Object;
        }


        private TickPrice CreateCtickPrice(decimal bid, decimal ask, string source, string assetPair)
        {
            return new TickPrice()
            {
                Ask = ask,
                Bid = bid,
                Source = source,
                Asset = assetPair,
                Timestamp = DateTime.UtcNow
            };
        }
    }

    public class FakeOrderBookProvider : IOrderBookProvider
    {
        public List<OrderBook> Data = new List<OrderBook>();

        public Task Send(OrderBook orderBook)
        {
            Data.Add(orderBook);
            return Task.CompletedTask;
        }
    }

    public class FakeTickPriceProvider : ITickPriceProvider
    {
        public List<TickPrice> Data = new List<TickPrice>();

        public Task Send(TickPrice tickPrice)
        {
            Data.Add(tickPrice);
            return Task.CompletedTask;
        }
    }

    public static class OrderBookHelper
    {
        public static OrderBook AddAskLevel(this OrderBook ob, decimal price, decimal volume)
        {
            var askLevels = ob.AskLevels;

            if (askLevels.TryGetValue(price, out var currentVolume))
            {
                askLevels = askLevels.SetItem(price, currentVolume + volume);
            }
            else
            {
                askLevels = askLevels.Add(price, volume);
            }

            ob.AskLevels = askLevels;

            return ob;
        }

        public static OrderBook AddBidLevel(this OrderBook ob, decimal price, decimal volume)
        {
            var bidLevels = ob.BidLevels;

            if (bidLevels.TryGetValue(price, out var currentVolume))
            {
                bidLevels = bidLevels.SetItem(price, currentVolume + volume);
            }
            else
            {
                bidLevels = bidLevels.Add(price, volume);
            }

            ob.BidLevels = bidLevels;

            return ob;
        }

        public static OrderBook CreateOrderBook(string source, string assetpair)
        {
            return new OrderBook(source, assetpair, DateTime.UtcNow,
                new List<OrderBookItem>(),
                new List<OrderBookItem>());
        }
    }

    public class SimBaseInstrumentSetting : ISimBaseInstrumentSetting
    {
        public SimBaseInstrumentSetting()
        {
            BaseAssetPair = "BTCUSD";
            SourceExchange = new List<string>();
            CountPerSecond = 1;
            PriceAccuracy = 2;
            FakeVolume = 1;
            DangerChangePriceKoef = 0;
            CrossInstrument = new List<LinkedInstrumentSettings>();
            Order = 0;
            UseExternalSpread = false;
            PriceCoef = 1;
            UseHardGlobalSpread = false;
        }

        public string BaseAssetPair { get; set; }
        public List<string> SourceExchange { get; set; }
        public int CountPerSecond { get; set; }
        public int PriceAccuracy { get; set;  }
        public decimal FakeVolume { get; set; }
        public List<LinkedInstrumentSettings> CrossInstrument { get; set; }
        public decimal DangerChangePriceKoef { get; set; }
        public int Order { get; set; }
        public bool UseExternalSpread { get; set; }
        public bool UseHardGlobalSpread { get; }
        public string Alias { get; set; }
        public decimal PriceCoef { get; set; }

        IReadOnlyList<string> ISimBaseInstrumentSetting.SourceExchange => SourceExchange;
        IReadOnlyList<ILinkedInstrumentSettings> ISimBaseInstrumentSetting.CrossInstrument => CrossInstrument;
    }

    public class LinkedInstrumentSettings : ILinkedInstrumentSettings
    {
        public LinkedInstrumentSettings(string assetPair, string crossAssetPair, string sourceExchange, bool isReverse)
        {
            AssetPair = assetPair;
            CrossAssetPair = crossAssetPair;
            SourceExchange = sourceExchange;
            IsReverse = isReverse;
            PriceAccuracy = 2;
            IsInternal = false;
        }

        public string AssetPair { get; set; }
        public string CrossAssetPair { get; set; }
        public string SourceExchange { get; set; }
        public bool IsReverse { get; set; }
        public int PriceAccuracy { get; set; }
        public bool IsInternal { get; set; }
    }

    public class FakeExchangeCommissionSetting : IExchangeCommissionSetting
    {
        public string ExchangeName { get; set; }
        public decimal TradeCommissionPerc { get; set; }
        public decimal WithdrawCommissionPerc { get; set; }
    }
}
