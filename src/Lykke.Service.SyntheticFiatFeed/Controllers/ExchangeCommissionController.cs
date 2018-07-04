using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Lykke.Common.ExchangeAdapter.Contracts;
using Lykke.Service.SyntheticFiatFeed.Core.Domain;
using Lykke.Service.SyntheticFiatFeed.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Service.SyntheticFiatFeed.Controllers
{
    [Route("api/[controller]")]
    public class ExchangeCommissionController : Controller
    {
        private readonly IExchangeCommissionSettingRepository _commissionSettingRepository;

        public ExchangeCommissionController(IExchangeCommissionSettingRepository commissionSettingRepository)
        {
            _commissionSettingRepository = commissionSettingRepository;
        }

        [HttpGet("GetAllSettings")]
        [SwaggerOperation("GetAllSettings")]
        [ProducesResponseType(typeof(List<ExchangeCommissionSettingDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllSettings()
        {
            var data = await _commissionSettingRepository.GetAllSettings();

            return Ok(data.Select(e => new ExchangeCommissionSettingDto(e)).ToList());
        }

        [HttpGet("GetSettings/{exchange}")]
        [SwaggerOperation("GetSettingsByExchange")]
        [ProducesResponseType(typeof(ExchangeCommissionSettingDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllSettings(string exchange)
        {
            var data = await _commissionSettingRepository.GetSettingsByExchange(exchange);

            return Ok(new ExchangeCommissionSettingDto(data));
        }

        [HttpPost("SetSettings")]
        [SwaggerOperation("SetSettings")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> SetSettings([FromBody]ExchangeCommissionSettingDto setting)
        {
            await _commissionSettingRepository.SetSettings(setting);

            return Ok();
        }
    }

    public class ExchangeCommissionSettingDto : IExchangeCommissionSetting
    {
        public ExchangeCommissionSettingDto(IExchangeCommissionSetting setting)
        {
            ExchangeName = setting.ExchangeName;
            TradeCommissionPerc = setting.TradeCommissionPerc;
            WithdrawCommissionPerc = setting.WithdrawCommissionPerc;
        }

        public ExchangeCommissionSettingDto()
        {
        }

        public string ExchangeName { get; set; }
        public decimal TradeCommissionPerc { get; set; }
        public decimal WithdrawCommissionPerc { get; set; }
    }
}
