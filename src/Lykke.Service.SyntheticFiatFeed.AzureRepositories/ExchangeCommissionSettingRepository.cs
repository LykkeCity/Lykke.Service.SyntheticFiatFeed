using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.AzureStorage.Tables;
using Lykke.Service.SyntheticFiatFeed.Domain;
using Lykke.Service.SyntheticFiatFeed.Domain.Repositories;

namespace Lykke.Service.SyntheticFiatFeed.AzureRepositories
{
    public class ExchangeCommissionSettingRepository : IExchangeCommissionSettingRepository
    {
        private readonly INoSQLTableStorage<ExchangeCommissionSettingEntity> _tableStorage;
        private Dictionary<string, ExchangeCommissionSettingEntity> _cache;

        public ExchangeCommissionSettingRepository(INoSQLTableStorage<ExchangeCommissionSettingEntity> tableStorage)
        {
            _tableStorage = tableStorage;
        }

        public async Task<IReadOnlyList<IExchangeCommissionSetting>> GetAllSettings()
        {
            await LoadCache();
            return _cache.Values.ToList();
        }

        public async Task<IExchangeCommissionSetting> GetSettingsByExchange(string exchange)
        {
            await LoadCache();
            if (_cache.TryGetValue(exchange, out var data))
                return data;

            return new ExchangeCommissionSettingEntity()
            {
                ExchangeName = exchange,
                WithdrawCommissionPerc = 0,
                TradeCommissionPerc = 0
            };
        }

        public async Task SetSettings(IExchangeCommissionSetting settings)
        {
            if (_cache.TryGetValue(settings.ExchangeName, out var data))
            {
                data.TradeCommissionPerc = settings.TradeCommissionPerc;
                data.WithdrawCommissionPerc = settings.WithdrawCommissionPerc;
                await _tableStorage.ReplaceAsync(data);
            }
            else
            {
                data = new ExchangeCommissionSettingEntity(settings);
                await _tableStorage.InsertOrReplaceAsync(data);
                _cache[data.ExchangeName] = data;
            }
        }

        private async Task LoadCache()
        {
            if (_cache == null)
            {
                var data = await _tableStorage.GetDataAsync();
                _cache = data.Select(e => new ExchangeCommissionSettingEntity(e))
                    .ToDictionary(e => e.ExchangeName, e => e);
            }
        }
    }

    public class ExchangeCommissionSettingEntity : AzureTableEntity, IExchangeCommissionSetting
    {
        public static string GeneratePartitionKey(string exchangeName)
        {
            return exchangeName;
        }

        public static string GenerateRowKey()
        {
            return "commissions";
        }

        public ExchangeCommissionSettingEntity()
        {
        }

        public ExchangeCommissionSettingEntity(IExchangeCommissionSetting settings)
        {
            PartitionKey = GeneratePartitionKey(settings.ExchangeName);
            RowKey = GenerateRowKey();
            ExchangeName = settings.ExchangeName;
            TradeCommissionPerc = settings.TradeCommissionPerc;
            WithdrawCommissionPerc = settings.WithdrawCommissionPerc;
            ETag = "*";
        }

        public string ExchangeName { get; set; }
        public decimal TradeCommissionPerc { get; set; }
        public decimal WithdrawCommissionPerc { get; set; }
    }
}
