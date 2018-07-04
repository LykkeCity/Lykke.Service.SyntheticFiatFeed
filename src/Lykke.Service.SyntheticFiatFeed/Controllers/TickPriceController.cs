using System.Collections.Generic;
using System.Linq;
using System.Net;
using Lykke.Common.ExchangeAdapter.Contracts;
using Lykke.Service.SyntheticFiatFeed.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Service.SyntheticFiatFeed.Controllers
{
    [Route("api/[controller]")]
    public class TickPriceController : Controller
    {
        private readonly ITickPriceStore _tickPriceStore;

        public TickPriceController(ITickPriceStore tickPriceStore)
        {
            _tickPriceStore = tickPriceStore;
        }

        [HttpGet("GetTickPrice/{assetPair}/{exchange}")]
        [SwaggerOperation("GetTickPrice")]
        [ProducesResponseType(typeof(TickPrice), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public IActionResult GetTickPrice(string exchange, string assetPair)
        {
            var ob = _tickPriceStore.GetTickPrice(exchange, assetPair);

            if (ob == null)
                return NotFound();

            return Ok(ob);
        }

        [HttpGet("GetTickPrice/{assetPair}")]
        [SwaggerOperation("GetTickPriceListByAssetPair")]
        [ProducesResponseType(typeof(List<TickPrice>), (int)HttpStatusCode.OK)]
        public IActionResult GetTickPriceByAssetPair(string assetPair)
        {
            var ob = _tickPriceStore.GetTickPricesByAssetPair(assetPair).ToList();

            return Ok(ob);
        }

        [HttpGet("GetTickPriceByExchange/{exchange}")]
        [SwaggerOperation("GetTickPriceListByExchange")]
        [ProducesResponseType(typeof(List<TickPrice>), (int)HttpStatusCode.OK)]
        public IActionResult GetTickPriceByExchange(string exchange)
        {
            var ob = _tickPriceStore.GetTickPricesByExchange(exchange).ToList();

            return Ok(ob);
        }

        [HttpGet("GetExchangeList")]
        [SwaggerOperation("GetExchangeList")]
        [ProducesResponseType(typeof(List<string>), (int)HttpStatusCode.OK)]
        public IActionResult GetExchangeList()
        {
            List<string> list = _tickPriceStore.GetExchangeList();

            return Ok(list);
        }
    }
}
