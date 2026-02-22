using Microsoft.Extensions.Hosting;

namespace Automatic.Protocols.HeatingFilmPressurize
{
    public class PlcHostedService : BackgroundService
    {
        private readonly HeatingFilmPressurizeScanner _plcScanner;

        public PlcHostedService(HeatingFilmPressurizeScanner PlcScanner)
        {
            _plcScanner = PlcScanner;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _plcScanner.ExecuteAsync(stoppingToken);
        }
    }
}
