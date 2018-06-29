namespace Lykke.Service.SyntheticFiatFeed.Services
{
    public sealed class CrossFiat
    {
        public string Source { get; set; }
        public string Target { get; set; }
        public int Decimals { get; set; }
    }
}