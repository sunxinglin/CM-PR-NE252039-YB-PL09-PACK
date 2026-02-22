using Microsoft.Extensions.Hosting;

namespace Ctp0600P.Client.PLC.PLC01
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
