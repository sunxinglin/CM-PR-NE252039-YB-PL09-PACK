using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Ctp0600P.Client.Apis;
using Ctp0600P.Client.Protocols;
using Ctp0600P.Client.ViewModels;
using Ctp0600P.Shared;

using MediatR;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Yee.Entitys.AlarmMgmt;
using Yee.Entitys.DTOS;

namespace Ctp0600P.Client.MessageHandler;

internal class AlarmNotificationHandler : INotificationHandler<AlarmSYSNotification>
{
    private readonly ILogger<AlarmNotificationHandler> _logger;
    private readonly APIHelper _apiHelper;
    private readonly IServiceProvider _serviceProvider;
    private readonly StepStationSetting _stationSetting;

    public AlarmNotificationHandler(ILogger<AlarmNotificationHandler> logger, APIHelper apiHelper,
        IServiceProvider serviceProvider, IOptionsMonitor<StepStationSetting> stationSetting)
    {
        _logger = logger;
        _apiHelper = apiHelper;
        _serviceProvider = serviceProvider;
        _stationSetting = stationSetting.CurrentValue;
    }

    public Task Handle(AlarmSYSNotification notification, CancellationToken cancellationToken)
    {
        try
        {
            var scope = _serviceProvider.CreateScope();
            var serviceProvider = scope.ServiceProvider;
            var realtimePageViewModel = serviceProvider.GetRequiredService<RealtimePageViewModel>();
            var alarms = realtimePageViewModel.Alarms;
            var alarmMessage = notification;
            // 在当前报警列表中找一条“未处理”的同类报警
            var alarm = alarms
                .FirstOrDefault(o => !o.IsFinish
                                     && o.Module == alarmMessage.Module
                                     && o.Code == alarmMessage.Code
                                     && o.DeviceNo == alarmMessage.DeviceNo);
            switch (notification.action)
            {
                // 如果一个错误反复发生，不要无限新增报警，而是更新原报警的发生时间
                case AlarmAction.Occur when alarm != null:
                    alarm.OccurTime = DateTime.Now;
                    break;
                case AlarmAction.Occur:
                    alarm = new Alarm
                    {
                        Code = alarmMessage.Code,
                        Name = alarmMessage.Name,
                        Description = alarmMessage.Description,
                        Module = alarmMessage.Module,
                        OccurTime = DateTime.Now,
                        DeviceNo = alarmMessage.DeviceNo,
                        TightenNGExtra = alarmMessage.TightenNGExtra,
                    };
                    break;
                case AlarmAction.Clear when alarm != null:
                    alarm.IsFinish = true;
                    break;
                default:
                    throw new Exception();
            }

            UIHelper.RunInUIThread(pl =>
            {
                using var serviceScope = _serviceProvider.CreateScope();
                var scopeServiceProvider = serviceScope.ServiceProvider;
                var realtimeManualVm = scopeServiceProvider.GetRequiredService<RealtimePageViewModel>();

                realtimeManualVm.UpdateAlarms?.Execute(alarm);
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }

        return Task.CompletedTask;
    }
}