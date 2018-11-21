using AutoMapper;
using Lykke.Service.SyntheticFiatFeed.Client.Models.ExchangeCommission;
using Lykke.Service.SyntheticFiatFeed.Domain;
using Lykke.Service.SyntheticFiatFeed.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Lykke.Service.SyntheticFiatFeed.Client.Api;

namespace Lykke.Service.SyntheticFiatFeed.Controllers
{
    [Route("api/[controller]")]
    public class ExchangeCommissionController : Controller, IExchangeCommissionApi
    {
        private readonly IExchangeCommissionSettingRepository _commissionSettingRepository;

        public ExchangeCommissionController(IExchangeCommissionSettingRepository commissionSettingRepository)
        {
            _commissionSettingRepository = commissionSettingRepository;
        }

        /// <inheritdoc/>
        /// <response code="200">A collection of commission settings for exchanges.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyCollection<ExchangeCommissionSettingModel>), (int)HttpStatusCode.OK)]
        public async Task<IReadOnlyCollection<ExchangeCommissionSettingModel>> GetAllSettingsAsync()
        {
            var data = await _commissionSettingRepository.GetAllSettings();

            return Mapper.Map<List<ExchangeCommissionSettingModel>>(data);
        }

        /// <inheritdoc/>
        /// <response code="200">Commission settings model.</response>
        [HttpGet("{exchange}")]
        [ProducesResponseType(typeof(ExchangeCommissionSettingModel), (int)HttpStatusCode.OK)]
        public async Task<ExchangeCommissionSettingModel> GetSettingsByExchangeAsync(string exchange)
        {
            var data = await _commissionSettingRepository.GetSettingsByExchange(exchange);

            return Mapper.Map<ExchangeCommissionSettingModel>(data);
        }

        /// <inheritdoc/>
        /// <response code="204">Commission settings successfully updated.</response>
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task SetSettingsAsync([FromBody]ExchangeCommissionSettingModel model)
        {
            var setting = Mapper.Map<ExchangeCommissionSetting>(model);

            await _commissionSettingRepository.SetSettings(setting);
        }
    }
}
