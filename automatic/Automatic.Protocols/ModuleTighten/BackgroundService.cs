using Microsoft.Extensions.Hosting;

namespace Automatic.Protocols.ModuleTighten
{
    public class PlcHostedService : BackgroundService
    {
        private readonly ModuleTightenScanner _plcScanner;

        public PlcHostedService(ModuleTightenScanner PlcScanner)
        {
            _plcScanner = PlcScanner;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _plcScanner.ExecuteAsync(stoppingToken);
        }
    }
}
