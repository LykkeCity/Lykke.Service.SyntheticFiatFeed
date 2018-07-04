using System.Threading.Tasks;
using Lykke.Common.ExchangeAdapter.Contracts;

namespace Lykke.Service.SyntheticFiatFeed.Core.Services
{
    public interface IOrderBookProvider
    {
        Task Send(OrderBook orderBook);
    }
}
