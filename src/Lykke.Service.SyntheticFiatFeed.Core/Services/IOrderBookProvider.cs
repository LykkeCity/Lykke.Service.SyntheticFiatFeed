using System.Threading.Tasks;

namespace Lykke.Service.SyntheticFiatFeed.Core.Services
{
    public interface IOrderBookProvider
    {
        Task Publish(Lykke.Common.ExchangeAdapter.Contracts.OrderBook orderBook);
    }
}