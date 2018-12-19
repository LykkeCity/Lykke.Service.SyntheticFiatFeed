using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureStorage;
using Common;
using Lykke.AzureStorage.Tables;
using Lykke.Service.SyntheticFiatFeed.Domain;
using Lykke.Service.SyntheticFiatFeed.Domain.Repositories;

namespace Lykke.Service.SyntheticFiatFeed.AzureRepositories
{
    public class SimBaseInstrumentSettingRepository : ISimBaseInstrumentSettingRepository
    {
        private readonly INoSQLTableStorage<SimBaseInstrumentSettingEntity> _tableStorage;
        private Dictionary<string, SimBaseInstrumentSettingEntity> _cache;

        public SimBaseInstrumentSettingRepository(INoSQLTableStorage<SimBaseInstrumentSettingEntity> tableStorage)
        {
            _tableStorage = tableStorage;
        }

        public async Task<IReadOnlyList<ISimBaseInstrumentSetting>> GetAllSettings()
        {
            if (_cache == null)
            {
                var data = await _tableStorage.GetDataAsync();
                _cache = data.ToDictionary(e => e.BaseAssetPair, e => e);
            }

            return _cache.Values.ToList();
        }

        public async Task AddOrUpdateSettings(ISimBaseInstrumentSetting setting)
        {
            if (_cache.TryGetValue(setting.BaseAssetPair, out var entity))
            {
                entity.CopyFrom(setting);
                await _tableStorage.ReplaceAsync(entity);
            }
            else
            {
                entity = new SimBaseInstrumentSettingEntity(setting);
                await _tableStorage.InsertOrReplaceAsync(entity);
                _cache[entity.BaseAssetPair] = entity;
            }
        }
    }

    public class SimBaseInstrumentSettingEntity : AzureTableEntity, ISimBaseInstrumentSetting
    {
        private IReadOnlyList<string> _sourceExchange = new List<string>();
        private IReadOnlyList<ILinkedInstrumentSettings> _crossInstrument = new ILinkedInstrumentSettings[]{};

        public SimBaseInstrumentSettingEntity()
        {
            PriceCoef = 1;
        }

        public SimBaseInstrumentSettingEntity(ISimBaseInstrumentSetting setting)
        {
            PartitionKey = GeneratePartitionKey(setting.BaseAssetPair);
            RowKey = GenerateRowKey(setting.BaseAssetPair);
            BaseAssetPair = setting.BaseAssetPair;
            _sourceExchange = setting.SourceExchange;
            CountPerSecond = setting.CountPerSecond;
            PriceAccuracy = setting.PriceAccuracy;
            FakeVolume = setting.FakeVolume;
            _crossInstrument = setting.CrossInstrument;
            DangerChangePriceKoef = setting.DangerChangePriceKoef;
            Order = setting.Order;
            UseExternalSpread = setting.UseExternalSpread;
            ETag = "*";
            PriceCoef = setting.PriceCoef;
            Alias = setting.Alias;
        }

        public void CopyFrom(ISimBaseInstrumentSetting setting)
        {
            _sourceExchange = setting.SourceExchange;
            CountPerSecond = setting.CountPerSecond;
            PriceAccuracy = setting.PriceAccuracy;
            FakeVolume = setting.FakeVolume;
            _crossInstrument = setting.CrossInstrument;
            DangerChangePriceKoef = setting.DangerChangePriceKoef;
            Order = setting.Order;
            UseExternalSpread = setting.UseExternalSpread;
            ETag = "*";
            PriceCoef = setting.PriceCoef;
            Alias = setting.Alias;
        }

        public static string GeneratePartitionKey(string baseAssetPair)
        {
            return baseAssetPair;
        }

        public static string GenerateRowKey(string baseAssetPair)
        {
            return baseAssetPair;
        }

        public string BaseAssetPair { get; set; }

        public string SourceExchange
        {
            get => _sourceExchange.ToJson();
            set => _sourceExchange = value.DeserializeJson<List<string>>();
        }

        public int CountPerSecond { get; set; }

        public int PriceAccuracy { get; set; }

        public decimal FakeVolume { get; set; }

        public string CrossInstrument
        {
            get => _crossInstrument.ToJson();
            set => _crossInstrument = value.DeserializeJson<List<LinkedInstrumentSettings>>();
        }

        public decimal DangerChangePriceKoef { get; set; }
        public int Order { get; set; }
        public bool UseExternalSpread { get; set; }
        public decimal PriceCoef { get; set; }
        public string Alias { get; set; }

        IReadOnlyList<string> ISimBaseInstrumentSetting.SourceExchange => _sourceExchange;
        IReadOnlyList<ILinkedInstrumentSettings> ISimBaseInstrumentSetting.CrossInstrument => _crossInstrument;

        public class LinkedInstrumentSettings : ILinkedInstrumentSettings
        {
            public string AssetPair { get; set; }
            public string CrossAssetPair { get; set; }
            public string SourceExchange { get; set; }
            public bool IsReverse { get; set; }
            public int PriceAccuracy { get; set; }
            public bool IsInternal { get; set; }
        }
    }
}
