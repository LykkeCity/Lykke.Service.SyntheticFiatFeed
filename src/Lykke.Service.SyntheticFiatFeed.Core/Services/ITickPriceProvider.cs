using System.Threading.Tasks;
using Lykke.Common.ExchangeAdapter.Contracts;

namespace Lykke.Service.SyntheticFiatFeed.Core.Services
{
    public interface ITickPriceProvider
    {
        Task Send(TickPrice tickPrice);
    }
}