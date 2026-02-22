using Microsoft.Extensions.Hosting;

namespace Automatic.Protocols.ShoulderGlue
{
    public class PlcHostedService : BackgroundService
    {
        private readonly ShoulderGlueScanner _plcScanner;

        public PlcHostedService(ShoulderGlueScanner PlcScanner)
        {
            _plcScanner = PlcScanner;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _plcScanner.ExecuteAsync(stoppingToken);
        }
    }
}
