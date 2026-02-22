using Microsoft.Extensions.Hosting;

namespace Automatic.Protocols.LowerBoxGlue
{
    public class PlcHostedService : BackgroundService
    {
        private readonly LowerBoxGlueScanner _plcScanner;

        public PlcHostedService(LowerBoxGlueScanner PlcScanner)
        {
            _plcScanner = PlcScanner;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _plcScanner.ExecuteAsync(stoppingToken);
        }
    }
}
