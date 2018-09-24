using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Lykke.Service.SyntheticFiatFeed.Core.Domain;
using Lykke.Service.SyntheticFiatFeed.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Service.SyntheticFiatFeed.Controllers
{
    [Route("api/[controller]")]
    public class SettingsController : Controller
    {
        private readonly ISimBaseInstrumentSettingRepository _instrumentSettingRepository;

        public SettingsController(ISimBaseInstrumentSettingRepository instrumentSettingRepository)
        {
            _instrumentSettingRepository = instrumentSettingRepository;
        }

        [HttpGet("GetAllSettings")]
        [SwaggerOperation("GetAllSettings")]
        [ProducesResponseType(typeof(List<SimBaseInstrumentSettingDto>), (int) HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllSettings()
        {
            var data = await _instrumentSettingRepository.GetAllSettings();
            return Ok(data.Select(e => new SimBaseInstrumentSettingDto(e)).ToList());
        }

        [HttpPost("SetSettings")]
        [SwaggerOperation("SetSettings")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> SetSettings([FromBody]SimBaseInstrumentSettingDto setting)
        {
            await _instrumentSettingRepository.AddOrUpdateSettings(setting);
            return NoContent();
        }
    }

    public class SimBaseInstrumentSettingDto : ISimBaseInstrumentSetting
    {
        public SimBaseInstrumentSettingDto()
        {
            CrossInstrument = new List<LinkedInstrumentSettingsDto>();
            SourceExchange = new List<string>();
        }

        public SimBaseInstrumentSettingDto(ISimBaseInstrumentSetting setting)
        {
            BaseAssetPair = setting.BaseAssetPair;
            CountPerSecond = setting.CountPerSecond;
            PriceAccuracy = setting.PriceAccuracy;
            FakeVolume = setting.FakeVolume;
            DangerChangePriceKoef = setting.DangerChangePriceKoef;
            SourceExchange = setting.SourceExchange.ToList();
            CrossInstrument = setting.CrossInstrument.Select(e => new LinkedInstrumentSettingsDto(e)).ToList();
            Order = setting.Order;
            UseExternalSpread = setting.UseExternalSpread;
        }

        public string BaseAssetPair { get; set; }
        public int CountPerSecond { get; set; }
        public int PriceAccuracy { get; set; }
        public decimal FakeVolume { get; set; }
        public decimal DangerChangePriceKoef { get; set; }
        public int Order { get; set; }
        public bool UseExternalSpread { get; set; }

        public List<string> SourceExchange { get; set; }
        public List<LinkedInstrumentSettingsDto> CrossInstrument { get; set; }

        IReadOnlyList<string> ISimBaseInstrumentSetting.SourceExchange => SourceExchange;
        IReadOnlyList<ILinkedInstrumentSettings> ISimBaseInstrumentSetting.CrossInstrument => CrossInstrument;

        public class LinkedInstrumentSettingsDto : ILinkedInstrumentSettings
        {
            public LinkedInstrumentSettingsDto()
            {
            }

            public LinkedInstrumentSettingsDto(ILinkedInstrumentSettings settings)
            {
                AssetPair = settings.AssetPair;
                CrossAssetPair = settings.CrossAssetPair;
                SourceExchange = settings.SourceExchange;
                IsReverse = settings.IsReverse;
                PriceAccuracy = settings.PriceAccuracy;
                IsInternal = settings.IsInternal;
            }

            public string AssetPair { get; set; }
            public string CrossAssetPair { get; set; }
            public string SourceExchange { get; set; }
            public bool IsReverse { get; set; }
            public int PriceAccuracy { get; set; }
            public bool IsInternal { get; set; }
        }
    }
}
