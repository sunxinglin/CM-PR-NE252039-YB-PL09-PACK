using Ctp0600P.Client.Protocols.ScanCode;

using MediatR;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Yee.Entitys.AlarmMgmt;

namespace Ctp0600P.Client.Protocols;

public class ScanCodeService : BackgroundService
{
    private readonly ScanCodeMgr _scanCodeMgr;
    private readonly ILogger<ScanCodeService> _logger;
    private readonly IMediator _mediator;

    public ScanCodeService(ILogger<ScanCodeService> logger, IMediator mediator, ScanCodeMgr scanCodeMgr)
    {
        _logger = logger;
        _mediator = mediator;
        _scanCodeMgr = scanCodeMgr;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _scanCodeMgr.LoadProtocols();
        foreach (var ctrl in _scanCodeMgr.ScanCodeCtrls)
        {
            var thread = new Thread(() =>
            {
                _ = RunAsync(stoppingToken, ctrl.Value);
            });
            thread.IsBackground = true;
            thread.Start();
        }

    }

    private async Task RunAsync(CancellationToken ct, ScanCodeCtrl ctrl)
    {
        try
        {
                
            if (!ctrl._serialPort.IsOpen)
            {
                await ctrl.ConnectAsync();
            }

            ctrl.ReadPort();
        }
        catch (Exception ex)
        {
               
            await _mediator.Publish(new AlarmSYSNotification() { Code = AlarmCode.扫码枪错误, Name = AlarmCode.扫码枪错误.ToString(), Module = AlarmModule.DESOUTTER_MODULE, Description = $"【{ ctrl._scanCodeResource.Name }】{ ex.Message }" });
                
        }
        await Task.Delay(200);
    }
}