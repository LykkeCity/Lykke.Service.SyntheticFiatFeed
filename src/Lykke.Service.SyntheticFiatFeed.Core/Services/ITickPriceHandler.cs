using System.Threading.Tasks;

namespace Lykke.Service.SyntheticFiatFeed.Core.Services
{
    public interface ITickPriceHandler
    {
        Task Handle(Lykke.Common.ExchangeAdapter.Contracts.TickPrice tickPrice);
    }
}