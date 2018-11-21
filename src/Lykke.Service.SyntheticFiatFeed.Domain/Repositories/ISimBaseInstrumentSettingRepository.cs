using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.SyntheticFiatFeed.Domain.Repositories
{
    public interface ISimBaseInstrumentSettingRepository
    {
        Task<IReadOnlyList<ISimBaseInstrumentSetting>> GetAllSettings();

        Task AddOrUpdateSettings(ISimBaseInstrumentSetting setting);
    }
}
