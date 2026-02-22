using Microsoft.Extensions.Hosting;

namespace Automatic.Protocols.TerminalReshape
{
    public class PlcHostedService : BackgroundService
    {
        private readonly TerminalReshapeScanner _plcScanner;

        public PlcHostedService(TerminalReshapeScanner PlcScanner)
        {
            _plcScanner = PlcScanner;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _plcScanner.ExecuteAsync(stoppingToken);
        }
    }
}
