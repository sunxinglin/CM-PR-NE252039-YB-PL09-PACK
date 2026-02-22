using Microsoft.Extensions.Hosting;

namespace Automatic.Protocols.ModuleInBox
{
    public class PlcHostedService : BackgroundService
    {
        private readonly ModuleInBoxScanner _plcScanner;

        public PlcHostedService(ModuleInBoxScanner PlcScanner)
        {
            _plcScanner = PlcScanner;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _plcScanner.ExecuteAsync(stoppingToken);
        }
    }
}
