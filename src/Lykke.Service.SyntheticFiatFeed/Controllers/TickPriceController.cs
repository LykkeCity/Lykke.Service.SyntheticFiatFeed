using AutoMapper;
using Lykke.Common.ApiLibrary.Exceptions;
using Lykke.Service.SyntheticFiatFeed.Client.Models.TickPrice;
using Lykke.Service.SyntheticFiatFeed.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Lykke.Common.ExchangeAdapter.Contracts;
using Lykke.Service.SyntheticFiatFeed.Client.Api;

namespace Lykke.Service.SyntheticFiatFeed.Controllers
{
    [Route("api/[controller]")]
    public class TickPriceController : Controller, ITickPriceApi
    {
        private readonly ITickPriceStore _tickPriceStore;

        public TickPriceController(ITickPriceStore tickPriceStore)
        {
            _tickPriceStore = tickPriceStore;
        }

        /// <inheritdoc/>
        /// <response code="200">The model that describes tick price.</response>
        /// <response code="409">Tick price does not exist.</response>
        [HttpGet("{assetPair}/{exchange}")]
        [ProducesResponseType(typeof(TickPriceModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public Task<TickPriceModel> GetTickPriceAsync(string exchange, string assetPair)
        {
            TickPrice ob = _tickPriceStore.GetTickPrice(exchange, assetPair);

            if (ob == null)
                throw new ValidationApiException(HttpStatusCode.NotFound, "Tick price does not exist.");

            return Task.FromResult(Mapper.Map<TickPriceModel>(ob));
        }

        /// <inheritdoc/>
        /// <response code="200">A collection of tick prices.</response>
        [HttpGet("assetpair/{assetPair}")]
        [ProducesResponseType(typeof(IReadOnlyCollection<TickPriceModel>), (int)HttpStatusCode.OK)]
        public Task<IReadOnlyCollection<TickPriceModel>> GetTickPriceByAssetPairAsync(string assetPair)
        {
            List<TickPrice> ob = _tickPriceStore.GetTickPricesByAssetPair(assetPair).ToList();

            return Task.FromResult<IReadOnlyCollection<TickPriceModel>>(
                Mapper.Map<List<TickPriceModel>>(ob));
        }

        /// <inheritdoc/>
        /// <response code="200">A collection of tick prices.</response>
        [HttpGet("exchange/{exchange}")]
        [ProducesResponseType(typeof(IReadOnlyCollection<TickPriceModel>), (int)HttpStatusCode.OK)]
        public Task<IReadOnlyCollection<TickPriceModel>> GetTickPriceByExchangeAsync(string exchange)
        {
            List<TickPrice> ob = _tickPriceStore.GetTickPricesByExchange(exchange).ToList();

            return Task.FromResult<IReadOnlyCollection<TickPriceModel>>(
                Mapper.Map<List<TickPriceModel>>(ob));
        }

        /// <inheritdoc/>
        /// <response code="200">A collection of exchange names.</response>
        [HttpGet("exchanges")]
        [ProducesResponseType(typeof(IReadOnlyCollection<string>), (int)HttpStatusCode.OK)]
        public Task<IReadOnlyCollection<string>> GetAllExchangesAsync()
        {
            List<string> list = _tickPriceStore.GetExchangeList();

            return Task.FromResult<IReadOnlyCollection<string>>(list);
        }
    }
}
