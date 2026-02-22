using Microsoft.Extensions.Hosting;

namespace Automatic.Protocols.UpperCoverTighten2
{
    public class PlcHostedService : BackgroundService
    {
        private readonly PlcScanner _plcScanner;

        public PlcHostedService(PlcScanner PlcScanner)
        {
            _plcScanner = PlcScanner;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _plcScanner.ExecuteAsync(stoppingToken);
        }
    }
}
