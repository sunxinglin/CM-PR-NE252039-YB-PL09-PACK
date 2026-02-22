using Ctp0600P.Client.Apis;
using Ctp0600P.Client.Protocols;
using Ctp0600P.Client.ViewModels;
using Ctp0600P.Shared;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Yee.Entitys.AlarmMgmt;
using Yee.Entitys.DTOS;

namespace Ctp0600P.Client.MessageHandler
{
    internal class AlarmNotificationHandler : INotificationHandler<AlarmSYSNotification>
    {
        private readonly ILogger<AlarmNotificationHandler> _logger;
        private readonly APIHelper _apiHelper;
        private readonly IServiceProvider _sp;
        private readonly StepStationSetting stationSetting;

        public AlarmNotificationHandler(ILogger<AlarmNotificationHandler> logger, APIHelper apiHelper, IServiceProvider sp, IOptionsMonitor<StepStationSetting> stationSetting)
        {
            this._logger = logger;
            _apiHelper = apiHelper;
            _sp = sp;
            this.stationSetting = stationSetting.CurrentValue;
        }

        public Task Handle(AlarmSYSNotification notification, CancellationToken cancellationToken)
        {
            try
            {
                var scope = this._sp.CreateScope();
                var sp = scope.ServiceProvider;
                var _realtimeManualvm = sp.GetRequiredService<RealtimePageViewModel>();
                ObservableCollection<Alarm> alarms;
                alarms = _realtimeManualvm.Alarms;
                var alarmmessage = notification;
                var alarm = alarms
                    .Where(o => !o.IsFinish
                        && o.Module == alarmmessage.Module
                        && o.Code == alarmmessage.Code
                        && o.DeviceNo == alarmmessage.DeviceNo
                    )
                    .FirstOrDefault();
                if (notification.action == AlarmAction.Occur)
                {
                    if (alarm != null)
                    {
                        alarm.OccurTime = DateTime.Now;
                    }
                    else
                    {
                        alarm = new Alarm()
                        {
                            Code = alarmmessage.Code,
                            Name = alarmmessage.Name,
                            Description = alarmmessage.Description,
                            Module = alarmmessage.Module,
                            OccurTime = DateTime.Now,
                            DeviceNo = alarmmessage.DeviceNo,
                            TightenNGExtra = alarmmessage.TightenNGExtra,
                        };
                    }
                }

                if (notification.action == AlarmAction.Clear && alarm != null)
                {
                    alarm.IsFinish = true;
                }

                UIHelper.RunInUIThread(pl =>
                {
                    using var scope = this._sp.CreateScope();
                    var sp = scope.ServiceProvider;
                    var _realtimeManualvm = sp.GetRequiredService<RealtimePageViewModel>();

                    _realtimeManualvm.UpdateAlarms?.Execute(alarm);
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return Task.CompletedTask;

        }
    }
}
