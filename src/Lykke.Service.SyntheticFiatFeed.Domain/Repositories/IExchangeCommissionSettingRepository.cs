using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.SyntheticFiatFeed.Domain.Repositories
{
    public interface IExchangeCommissionSettingRepository
    {
        Task<IReadOnlyList<IExchangeCommissionSetting>> GetAllSettings();

        Task<IExchangeCommissionSetting> GetSettingsByExchange(string exchange);

        Task SetSettings(IExchangeCommissionSetting settings);
    }
}
