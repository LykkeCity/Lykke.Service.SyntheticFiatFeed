namespace Lykke.Service.SyntheticFiatFeed.Services
{
    public class OrderbookGenerationSettings
    {
        public string Crypto { get; }
        public string BaseFiat { get; }
        public string ExpectedFiat { get; }
        public int Decimals { get; }

        public OrderbookGenerationSettings(
            string crypto,
            string baseFiat,
            string expectedFiat,
            int decimals)
        {
            Crypto = crypto;
            BaseFiat = baseFiat;
            ExpectedFiat = expectedFiat;
            Decimals = decimals;
        }
    }
}
