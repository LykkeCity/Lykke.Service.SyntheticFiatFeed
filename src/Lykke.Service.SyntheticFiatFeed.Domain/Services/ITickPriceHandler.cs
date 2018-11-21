using System.Threading.Tasks;
using Lykke.Common.ExchangeAdapter.Contracts;

namespace Lykke.Service.SyntheticFiatFeed.Domain.Services
{
    public interface ITickPriceHandler
    {
        Task Handle(Common.ExchangeAdapter.Contracts.TickPrice tickPrice);
    }
}
