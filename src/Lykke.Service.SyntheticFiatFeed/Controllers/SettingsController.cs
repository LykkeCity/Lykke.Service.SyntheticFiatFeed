using AutoMapper;
using Lykke.Common.ApiLibrary.Exceptions;
using Lykke.Service.SyntheticFiatFeed.Client.Models.Settings;
using Lykke.Service.SyntheticFiatFeed.Domain;
using Lykke.Service.SyntheticFiatFeed.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Lykke.Service.SyntheticFiatFeed.Client.Api;

namespace Lykke.Service.SyntheticFiatFeed.Controllers
{
    [Route("api/[controller]")]
    public class SettingsController : Controller, ISettingsApi
    {
        private readonly ISimBaseInstrumentSettingRepository _instrumentSettingRepository;

        public SettingsController(ISimBaseInstrumentSettingRepository instrumentSettingRepository)
        {
            _instrumentSettingRepository = instrumentSettingRepository;
        }

        /// <inheritdoc/>
        /// <response code="200">A collection of instrument settings.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyCollection<SimBaseInstrumentSettingModel>), (int)HttpStatusCode.OK)]
        public async Task<IReadOnlyCollection<SimBaseInstrumentSettingModel>> GetAllSettingsAsync()
        {
            IReadOnlyList<ISimBaseInstrumentSetting> data = await _instrumentSettingRepository.GetAllSettings();

            return Mapper.Map<List<SimBaseInstrumentSettingModel>>(data);
        }

        /// <inheritdoc/>
        /// <response code="200">The instrument settings.</response>
        /// <response code="409">Settings for asset pair do not exist.</response>
        [HttpGet("{assetPair}")]
        [ProducesResponseType(typeof(SimBaseInstrumentSettingModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<SimBaseInstrumentSettingModel> GetSettingsAsync(string assetPair)
        {
            var data = await _instrumentSettingRepository.GetAllSettings();

            var item = data.FirstOrDefault(e => e.BaseAssetPair == assetPair);

            if (item == null)
                throw new ValidationApiException(HttpStatusCode.NotFound, "Settings for asset pair do not exist.");

            return Mapper.Map<SimBaseInstrumentSettingModel>(item);
        }

        /// <inheritdoc/>
        /// <response code="204">Instrument settings successfully updated.</response>
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task SetSettingsAsync([FromBody]SimBaseInstrumentSettingModel model)
        {
            var setting = Mapper.Map<SimBaseInstrumentSetting>(model);

            await _instrumentSettingRepository.AddOrUpdateSettings(setting);
        }

        /// <inheritdoc/>
        /// <response code="204">Instrument settings successfully updated.</response>
        [HttpPost("SetMaySettings")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task SetMaySettingsAsync([FromBody]IReadOnlyCollection<SimBaseInstrumentSettingModel> model)
        {
            var settings = Mapper.Map<List<SimBaseInstrumentSetting>>(model);

            foreach (SimBaseInstrumentSetting item in settings)
            {
                await _instrumentSettingRepository.AddOrUpdateSettings(item);
            }
        }
    }
}
