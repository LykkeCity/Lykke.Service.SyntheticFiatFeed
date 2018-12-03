using System.Threading.Tasks;
using Lykke.Common.ExchangeAdapter.Contracts;

namespace Lykke.Service.SyntheticFiatFeed.Domain.Services
{
    public interface ITickPriceProvider
    {
        Task Send(Common.ExchangeAdapter.Contracts.TickPrice tickPrice);
    }
}
