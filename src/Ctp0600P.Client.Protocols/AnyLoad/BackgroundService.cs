using Ctp0600P.Client.Protocols.AnyLoad;

using MediatR;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Yee.Entitys.AlarmMgmt;

namespace Ctp0600P.Client.Protocols;

public class AnyLoadService : BackgroundService
{
    private readonly AnyLoadMgr _AnyLoadMgr;
    private readonly ILogger<AnyLoadService> _logger;
    private readonly IMediator _mediator;

    public AnyLoadService(ILogger<AnyLoadService> logger, IMediator mediator, AnyLoadMgr AnyLoadMgr)
    {
        _logger = logger;
        _mediator = mediator;
        _AnyLoadMgr = AnyLoadMgr;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _AnyLoadMgr.LoadProtocols();
        var thread = new Thread(() =>
        {
            if (_AnyLoadMgr != null && _AnyLoadMgr._AnyLoadCtrl != null)
                _ = RunAsync(stoppingToken, _AnyLoadMgr._AnyLoadCtrl);
        });
        thread.IsBackground = true;
        thread.Start();
    }

    private async Task RunAsync(CancellationToken ct, AnyLoadCtrl ctrl)
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
                
            await _mediator.Publish(new AlarmSYSNotification() { Code = AlarmCode.称重错误, Name = AlarmCode.称重错误.ToString(), Module = AlarmModule.DESOUTTER_MODULE, Description = $"【{ ctrl._AnyLoadResource.Name }】{ ex.Message }" });
        }
    }
}