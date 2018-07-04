using System.Threading.Tasks;
using Lykke.Common.ExchangeAdapter.Contracts;

namespace Lykke.Service.SyntheticFiatFeed.Core.Services
{
    public interface ITickPriceHandler
    {
        Task Handle(TickPrice tickPrice);
    }
}
