using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.SyntheticFiatFeed.Core.Domain;

namespace Lykke.Service.SyntheticFiatFeed.Core.Services
{
    public interface IExchangeCommissionSettingRepository
    {
        Task<IReadOnlyList<IExchangeCommissionSetting>> GetAllSettings();
        Task<IExchangeCommissionSetting> GetSettingsByExchange(string exchange);
        Task SetSettings(IExchangeCommissionSetting settings);
    }
}
