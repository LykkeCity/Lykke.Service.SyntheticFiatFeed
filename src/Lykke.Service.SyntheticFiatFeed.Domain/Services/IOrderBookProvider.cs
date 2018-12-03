using System.Threading.Tasks;
using Lykke.Common.ExchangeAdapter.Contracts;

namespace Lykke.Service.SyntheticFiatFeed.Domain.Services
{
    public interface IOrderBookProvider
    {
        Task Send(OrderBook orderBook);
    }
}
