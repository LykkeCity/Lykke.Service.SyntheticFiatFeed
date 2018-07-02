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
    public class OrderBookController : Controller
    {
        private readonly IOrderBookStore _orderBookStore;

        public OrderBookController(IOrderBookStore orderBookStore)
        {
            _orderBookStore = orderBookStore;
        }

        [HttpGet("GetOrderBook/{assetPair}/{exchange}")]
        [SwaggerOperation("GetOrderBook")]
        [ProducesResponseType(typeof(OrderBook), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public IActionResult GetOrderBook(string exchange, string assetPair)
        {
            var ob = _orderBookStore.GetOrderBook(exchange, assetPair);

            if (ob == null)
                return NotFound();

            return Ok(ob);
        }

        [HttpGet("GetOrderBook/{assetPair}")]
        [SwaggerOperation("GetOrderBookListByAssetPair")]
        [ProducesResponseType(typeof(List<OrderBook>), (int)HttpStatusCode.OK)]
        public IActionResult GetOrderBookByAssetPair(string assetPair)
        {
            var ob = _orderBookStore.GetOrderBooksByAssetPair(assetPair).ToList();

            return Ok(ob);
        }

        [HttpGet("GetOrderBookByExchange/{exchange}")]
        [SwaggerOperation("GetOrderBookListByExchange")]
        [ProducesResponseType(typeof(List<OrderBook>), (int)HttpStatusCode.OK)]
        public IActionResult GetOrderBookByExchange(string exchange)
        {
            var ob = _orderBookStore.GetOrderBooksByExchange(exchange).ToList();

            return Ok(ob);
        }

        [HttpGet("GetExchangeList")]
        [SwaggerOperation("GetExchangeList")]
        [ProducesResponseType(typeof(List<string>), (int)HttpStatusCode.OK)]
        public IActionResult GetExchangeList()
        {
            List<string> list = _orderBookStore.GetExchangeList();

            return Ok(list);
        }
    }
}
