using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.SyntheticFiatFeed.Core.Domain;

namespace Lykke.Service.SyntheticFiatFeed.Core.Services
{
    public interface ISimBaseInstrumentSettingRepository
    {
        Task<IReadOnlyList<ISimBaseInstrumentSetting>> GetAllSettings();

        Task AddOrUpdateSettings(ISimBaseInstrumentSetting setting);
    }
}
