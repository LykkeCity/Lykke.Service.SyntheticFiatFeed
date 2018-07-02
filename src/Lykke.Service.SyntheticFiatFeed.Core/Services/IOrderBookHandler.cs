using System.Threading.Tasks;

namespace Lykke.Service.SyntheticFiatFeed.Core.Services
{
    public interface IOrderBookHandler
    {
        Task Handle(Lykke.Common.ExchangeAdapter.Contracts.OrderBook orderBook);
    }
}
