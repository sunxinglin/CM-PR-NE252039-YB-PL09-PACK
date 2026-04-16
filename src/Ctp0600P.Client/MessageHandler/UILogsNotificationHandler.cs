using System;
using System.Threading;
using System.Threading.Tasks;

using AsZero.Core.Services.Messages;

using Ctp0600P.Client.Protocols;
using Ctp0600P.Client.UserControls.DebugTools;
using Ctp0600P.Client.ViewModels;

using MediatR;

using Microsoft.Extensions.Logging;

using Yee.Entitys.AlarmMgmt;

namespace Ctp0600P.Client.MessageHandler;

internal class UILogsNotificationHandler : INotificationHandler<UILogNotification>
{
    private readonly ILogger<UILogsNotificationHandler> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IMediator _mediator;
    private readonly RealtimePageViewModel _realtimeViewModel;
    private readonly ElectricScrewDriverUserControlVM _electricScrewDriverUserControlVM;

    public UILogsNotificationHandler(ILogger<UILogsNotificationHandler> logger, IServiceProvider serviceProvider,
        ElectricScrewDriverUserControlVM electricScrewDriverUserControlVM,
        RealtimePageViewModel realtimePageViewModel,
        IMediator mediator)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _mediator = mediator;
        _realtimeViewModel = realtimePageViewModel;
        _electricScrewDriverUserControlVM = electricScrewDriverUserControlVM;
    }

    public Task Handle(UILogNotification notification, CancellationToken cancellationToken)
    {
        UIHelper.RunInUIThread(pl =>
        {
            var msg = notification.LogMessage;

            if (msg != null)
            {
                // 处理报警日志
                if (msg is AlarmMessage alarmMsg)
                {
                    _logger.Log(msg.Level, msg.Content);
                    _mediator.Publish(new AlarmSYSNotification
                    {
                        Code = AlarmCode.系统运行错误, Name = nameof(AlarmCode.系统运行错误),
                        Module = AlarmModule.DESOUTTER_MODULE, Description = alarmMsg.Content
                    }, cancellationToken);
                }
                // 处理普通日志
                else if (msg is LogMessage logMsg)
                {
                    _logger.Log(msg.Level, msg.Content);
                    _realtimeViewModel.AddLogMsg?.Execute(logMsg);
                }
            }
        });
        return Task.CompletedTask;
    }
}