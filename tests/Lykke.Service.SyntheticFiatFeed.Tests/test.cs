using System;
using Lykke.Common.ExchangeAdapter.Contracts;
using Lykke.Service.SyntheticFiatFeed.Services;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Lykke.Service.SyntheticFiatFeed.Tests
{
    public class test
    {
        [Test]
        public void test1()
        {
            var s = "{\"source\":\"bitstamp\",\"asset\":\"BTCEUR\",\"timestamp\":\"2018-06-29T13:56:54Z\",\"asks\":[{\"price\":5036.97,\"volume\":0.60000000},{\"price\":5036.99,\"volume\":0.50000000},{\"price\":5037.03,\"volume\":2.00000000},{\"price\":5037.70,\"volume\":3.41540000},{\"price\":5040.16,\"volume\":9.51100000},{\"price\":5040.17,\"volume\":1.98100000},{\"price\":5040.89,\"volume\":0.50000000},{\"price\":5042.24,\"volume\":4.60000000},{\"price\":5044.47,\"volume\":0.00600000},{\"price\":5045.00,\"volume\":0.50000000},{\"price\":5045.07,\"volume\":3.24000000},{\"price\":5047.29,\"volume\":1.00000000},{\"price\":5047.30,\"volume\":5.12300000},{\"price\":5047.41,\"volume\":3.58000000},{\"price\":5048.10,\"volume\":3.28600000},{\"price\":5048.25,\"volume\":2.01000000},{\"price\":5049.16,\"volume\":0.25300000},{\"price\":5049.53,\"volume\":0.05011883},{\"price\":5049.99,\"volume\":0.19404764},{\"price\":5050.00,\"volume\":0.10029342},{\"price\":5050.38,\"volume\":1.36000000},{\"price\":5051.43,\"volume\":4.20000000},{\"price\":5052.18,\"volume\":6.02658120},{\"price\":5054.15,\"volume\":0.85200000},{\"price\":5054.39,\"volume\":2.30000000},{\"price\":5054.88,\"volume\":1.52100000},{\"price\":5055.10,\"volume\":1.50000000},{\"price\":5056.95,\"volume\":2.00000000},{\"price\":5057.04,\"volume\":3.45000000},{\"price\":5057.46,\"volume\":0.05011884},{\"price\":5057.84,\"volume\":2.30900000},{\"price\":5059.57,\"volume\":0.17143964},{\"price\":5059.58,\"volume\":2.24701000},{\"price\":5060.19,\"volume\":5.00000000},{\"price\":5060.20,\"volume\":23.77700000},{\"price\":5060.22,\"volume\":4.30000000},{\"price\":5062.33,\"volume\":0.24927259},{\"price\":5062.46,\"volume\":3.00000000},{\"price\":5063.32,\"volume\":4.00000000},{\"price\":5066.20,\"volume\":0.05011884},{\"price\":5071.70,\"volume\":6.20200000},{\"price\":5073.76,\"volume\":0.04700000},{\"price\":5074.62,\"volume\":0.05011884},{\"price\":5079.10,\"volume\":0.16666667},{\"price\":5082.33,\"volume\":5.00000000},{\"price\":5082.34,\"volume\":22.47010000},{\"price\":5083.05,\"volume\":1.92312977},{\"price\":5085.80,\"volume\":0.50000000},{\"price\":5089.33,\"volume\":0.25000000},{\"price\":5090.35,\"volume\":0.42000000},{\"price\":5097.06,\"volume\":7.16500000},{\"price\":5099.00,\"volume\":1.03000000},{\"price\":5100.00,\"volume\":2.10267869},{\"price\":5100.42,\"volume\":0.42000000},{\"price\":5108.78,\"volume\":0.39984006},{\"price\":5110.00,\"volume\":0.12000000},{\"price\":5110.47,\"volume\":0.16000000},{\"price\":5118.70,\"volume\":0.42000000},{\"price\":5123.00,\"volume\":0.33508094},{\"price\":5128.76,\"volume\":0.32000000},{\"price\":5135.23,\"volume\":0.01002233},{\"price\":5137.01,\"volume\":0.00517230},{\"price\":5145.74,\"volume\":0.00117750},{\"price\":5150.00,\"volume\":0.05000000},{\"price\":5156.86,\"volume\":3.57124589},{\"price\":5169.53,\"volume\":0.40000000},{\"price\":5173.91,\"volume\":0.70000000},{\"price\":5186.98,\"volume\":3.00000000},{\"price\":5188.91,\"volume\":0.00992209},{\"price\":5199.00,\"volume\":0.00500000},{\"price\":5199.98,\"volume\":0.05000000},{\"price\":5200.00,\"volume\":0.16882577},{\"price\":5202.00,\"volume\":0.50000000},{\"price\":5205.33,\"volume\":0.00510390},{\"price\":5213.00,\"volume\":1.00000000},{\"price\":5215.55,\"volume\":0.39305550},{\"price\":5230.00,\"volume\":50.00000000},{\"price\":5234.20,\"volume\":0.15000000},{\"price\":5238.90,\"volume\":0.25000000},{\"price\":5240.00,\"volume\":0.40736594},{\"price\":5241.33,\"volume\":0.00982286},{\"price\":5270.00,\"volume\":0.20000000},{\"price\":5270.86,\"volume\":0.00160000},{\"price\":5271.00,\"volume\":1.00000000},{\"price\":5274.56,\"volume\":0.00503700},{\"price\":5275.00,\"volume\":0.00400000},{\"price\":5277.77,\"volume\":0.04736800},{\"price\":5280.39,\"volume\":1.71918847},{\"price\":5285.51,\"volume\":0.12802657},{\"price\":5286.20,\"volume\":0.01500000},{\"price\":5288.00,\"volume\":0.75950000},{\"price\":5291.12,\"volume\":0.03120570},{\"price\":5294.28,\"volume\":0.00972462},{\"price\":5299.24,\"volume\":0.03859226},{\"price\":5300.00,\"volume\":0.13400000},{\"price\":5307.54,\"volume\":0.00500000},{\"price\":5318.00,\"volume\":0.00500000},{\"price\":5319.53,\"volume\":0.03000000},{\"price\":5323.00,\"volume\":1.00800000},{\"price\":5330.15,\"volume\":0.58734012}],\"bids\":[{\"price\":5027.99,\"volume\":0.50000000},{\"price\":5025.91,\"volume\":0.61049835},{\"price\":5025.90,\"volume\":1.70690000},{\"price\":5024.84,\"volume\":0.67900000},{\"price\":5024.83,\"volume\":0.50000000},{\"price\":5023.60,\"volume\":3.41570000},{\"price\":5022.53,\"volume\":2.00000000},{\"price\":5021.93,\"volume\":0.00109290},{\"price\":5020.58,\"volume\":2.24701000},{\"price\":5018.70,\"volume\":0.50000000},{\"price\":5017.10,\"volume\":5.12300000},{\"price\":5016.99,\"volume\":8.37692600},{\"price\":5016.27,\"volume\":4.00000000},{\"price\":5015.44,\"volume\":1.86500000},{\"price\":5013.25,\"volume\":4.30000000},{\"price\":5012.53,\"volume\":1.62500000},{\"price\":5010.81,\"volume\":3.00000000},{\"price\":5010.43,\"volume\":0.48800000},{\"price\":5009.96,\"volume\":4.10000000},{\"price\":5009.59,\"volume\":3.89700000},{\"price\":5008.33,\"volume\":0.70900000},{\"price\":5007.81,\"volume\":0.00600000},{\"price\":5007.40,\"volume\":0.16770141},{\"price\":5007.39,\"volume\":23.77700000},{\"price\":5007.38,\"volume\":22.47010000},{\"price\":5007.15,\"volume\":3.28000000},{\"price\":5005.59,\"volume\":0.00153989},{\"price\":5005.00,\"volume\":1.50000000},{\"price\":5003.85,\"volume\":5.00000000},{\"price\":5003.84,\"volume\":3.34000000},{\"price\":5000.89,\"volume\":3.80000000},{\"price\":5000.01,\"volume\":8.93765432},{\"price\":5000.00,\"volume\":0.17955020},{\"price\":4998.07,\"volume\":2.40000000},{\"price\":4996.23,\"volume\":2.56400000},{\"price\":4995.08,\"volume\":2.21000000},{\"price\":4993.95,\"volume\":0.24864728},{\"price\":4990.20,\"volume\":0.04800000},{\"price\":4990.00,\"volume\":1.00000000},{\"price\":4988.35,\"volume\":5.00000000},{\"price\":4988.34,\"volume\":1.98586157},{\"price\":4988.00,\"volume\":0.02000000},{\"price\":4987.53,\"volume\":1.00000000},{\"price\":4987.00,\"volume\":0.01000000},{\"price\":4986.56,\"volume\":0.10001885},{\"price\":4986.55,\"volume\":0.10003309},{\"price\":4985.79,\"volume\":4.59100000},{\"price\":4985.69,\"volume\":0.42000000},{\"price\":4985.00,\"volume\":2.17516915},{\"price\":4981.95,\"volume\":0.25000000},{\"price\":4980.11,\"volume\":0.10010000},{\"price\":4980.00,\"volume\":0.03961747},{\"price\":4979.89,\"volume\":0.32008739},{\"price\":4977.00,\"volume\":0.03000000},{\"price\":4975.64,\"volume\":0.42000000},{\"price\":4975.00,\"volume\":0.00994974},{\"price\":4974.81,\"volume\":0.02005101},{\"price\":4971.71,\"volume\":0.01100000},{\"price\":4970.38,\"volume\":0.01000000},{\"price\":4970.00,\"volume\":0.17481335},{\"price\":4967.89,\"volume\":2.42642935},{\"price\":4966.23,\"volume\":0.10689882},{\"price\":4965.58,\"volume\":0.16000000},{\"price\":4965.30,\"volume\":0.50000000},{\"price\":4962.82,\"volume\":5.54300000},{\"price\":4962.00,\"volume\":0.01000000},{\"price\":4960.02,\"volume\":0.10000000},{\"price\":4960.00,\"volume\":0.17505709},{\"price\":4955.56,\"volume\":0.02017935},{\"price\":4953.12,\"volume\":0.03000000},{\"price\":4952.00,\"volume\":0.01000000},{\"price\":4951.26,\"volume\":39.62800000},{\"price\":4951.25,\"volume\":8.15700000},{\"price\":4951.23,\"volume\":0.20000000},{\"price\":4951.00,\"volume\":0.51000000},{\"price\":4950.50,\"volume\":0.00700000},{\"price\":4950.28,\"volume\":0.00949624},{\"price\":4950.00,\"volume\":1.77035440},{\"price\":4948.47,\"volume\":0.10000000},{\"price\":4948.31,\"volume\":0.01536154},{\"price\":4943.00,\"volume\":0.01264160},{\"price\":4941.39,\"volume\":0.18080539},{\"price\":4940.43,\"volume\":0.05040563},{\"price\":4940.00,\"volume\":0.10000000},{\"price\":4937.41,\"volume\":0.00102190},{\"price\":4937.24,\"volume\":0.01012709},{\"price\":4937.00,\"volume\":0.02100000},{\"price\":4935.33,\"volume\":11.23505000},{\"price\":4933.00,\"volume\":0.10000000},{\"price\":4931.00,\"volume\":0.01000000},{\"price\":4930.00,\"volume\":0.10000000},{\"price\":4928.73,\"volume\":0.03784545},{\"price\":4927.30,\"volume\":0.00335989},{\"price\":4925.00,\"volume\":0.10435000},{\"price\":4920.00,\"volume\":0.02000000},{\"price\":4918.66,\"volume\":0.01000000},{\"price\":4917.19,\"volume\":0.00506997},{\"price\":4915.50,\"volume\":0.56000000},{\"price\":4915.00,\"volume\":0.00244048},{\"price\":4914.05,\"volume\":0.02029894}]}";

            var ob = JsonConvert.DeserializeObject<OrderBook>(s);

            Assert.IsNotEmpty(ob.Asks);
        }

        [Test]
        public void usd_orderbook()
        {
            var now = DateTime.UtcNow;
            var ob = new OrderBook("bitstamp", "BTCUSD", now, new[]
                {
                    new OrderBookItem(6244.96M, 1.5M),
                },
                new[]
                {
                    new OrderBookItem(6239.85M, 0.8M)
                });

            var tp = new TickPrice
            {
                Source = "lykke",
                Asset = "EURUSD",
                Timestamp = now,
                Ask = 0.9M,
                Bid = 0.8M
            };

            var synOb = OrderbookGeneratorService.CreateSyntheticOrderBook(
                tp,
                ob,
                "BTCEUR",
                "synthetic-bitstamp-BTCUSD", 5);

            Console.WriteLine(JsonConvert.SerializeObject(synOb));
        }
    }
}
