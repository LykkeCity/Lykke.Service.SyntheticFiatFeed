using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.SyntheticFiatFeed.Core.Domain;
using Lykke.Service.SyntheticFiatFeed.Core.Services;
using MoreLinq;

namespace Lykke.Service.SyntheticFiatFeed.Services.Sim
{
    public class SimService : IDisposable
    {
        private readonly ISimBaseInstrumentSettingRepository _settings;
        private readonly ILogFactory _logFactory;
        private readonly IOrderBookProvider _orderBookProvider;
        private readonly ITickPriceProvider _tickPriceProvider;
        private readonly ITickPriceStore _tickPriceStore;
        private readonly IExchangeCommissionSettingRepository _commissionSettingRepository;
        private readonly List<SimBaseInstrumentService> _services = new List<SimBaseInstrumentService>();

        private readonly TimerTrigger _timerTrigger;
        private readonly ILog _log;

        public SimService(
            ISimBaseInstrumentSettingRepository settings, 
            ILogFactory logFactory,
            IOrderBookProvider orderBookProvider,
            ITickPriceProvider tickPriceProvider,
            ITickPriceStore tickPriceStore,
            IExchangeCommissionSettingRepository commissionSettingRepository)
        {
            _settings = settings;
            _logFactory = logFactory;
            _orderBookProvider = orderBookProvider;
            _tickPriceProvider = tickPriceProvider;
            _tickPriceStore = tickPriceStore;
            _commissionSettingRepository = commissionSettingRepository;
            _log = _logFactory.CreateLog(this);

            _timerTrigger = new TimerTrigger(nameof(SimService), TimeSpan.FromMilliseconds(500), _logFactory, DoTime);
        }

        private async Task DoTime(ITimerTrigger timer, TimerTriggeredHandlerArgs args, CancellationToken cancellationtoken)
        {
            foreach (var service in _services.OrderBy(e => e.Order, OrderByDirection.Ascending))
            {
                try
                {
                    await service.CalculateMarket();
                }
                catch (Exception ex)
                {
                    _log.Error(ex, process: nameof(DoTime), context: service.Name);
                }
            }
        }

        public void Start()
        {
            var settings = _settings.GetAllSettings().GetAwaiter().GetResult();

            foreach (ISimBaseInstrumentSetting setting in settings)
            {
                var item = new SimBaseInstrumentService(_orderBookProvider, _tickPriceProvider, _tickPriceStore, setting, _commissionSettingRepository, _logFactory);
                _services.Add(item);
            }

            _timerTrigger.Start();
        }

        public void Stop()
        {
            _timerTrigger.Stop();
        }

        public void Dispose()
        {
            _timerTrigger?.Dispose();
        }
    }
}
