using MediatR;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Yee.Entitys.AlarmMgmt;

namespace Ctp0600P.Client.Protocols.AnyLoad_Wifi;

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
            if (_AnyLoadMgr is { _AnyLoadCtrl: not null })
            {
                _ = RunAsync(_AnyLoadMgr._AnyLoadCtrl, stoppingToken);
            }
        });
        thread.IsBackground = true;
        thread.Start();
    }

    private async Task RunAsync(AnyLoadCtrl ctrl, CancellationToken ct)
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
            await _mediator.Publish(new AlarmSYSNotification
            {
                Code = AlarmCode.称重错误, Name = nameof(AlarmCode.称重错误), Module = AlarmModule.ELECTRONIC_SCALE,
                Description = $"【{ctrl._AnyLoadResource.Name}】{ex.Message}"
            }, ct);
        }
    }
}