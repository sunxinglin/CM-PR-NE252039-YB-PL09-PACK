using Microsoft.Extensions.Hosting;

namespace Automatic.Protocols.UpperCoverTighten
{
    public class PlcHostedService : BackgroundService
    {
        private readonly UpperCoverTightenScanner _plcScanner;

        public PlcHostedService(UpperCoverTightenScanner PlcScanner)
        {
            _plcScanner = PlcScanner;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _plcScanner.ExecuteAsync(stoppingToken);
        }
    }
}
