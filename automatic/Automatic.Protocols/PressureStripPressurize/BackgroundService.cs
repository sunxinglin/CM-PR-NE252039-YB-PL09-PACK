using Microsoft.Extensions.Hosting;

namespace Automatic.Protocols.PressureStripPressurize
{
    public class PlcHostedService : BackgroundService
    {
        private readonly PressureStripScanner _plcScanner;

        public PlcHostedService(PressureStripScanner PlcScanner)
        {
            _plcScanner = PlcScanner;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _plcScanner.ExecuteAsync(stoppingToken);
        }
    }
}
