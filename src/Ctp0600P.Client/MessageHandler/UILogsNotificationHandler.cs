using AsZero.Core.Services.Messages;
using Ctp0600P.Client.Protocols;
using Ctp0600P.Client.UserControls.DebugTools;
using Ctp0600P.Client.ViewModels;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Yee.Entitys.AlarmMgmt;

namespace Ctp0600P.Client.MessageHandler
{
    internal class UILogsNotificationHandler : INotificationHandler<UILogNotification>
    {
        private readonly ILogger<UILogsNotificationHandler> _logger;
        private readonly IServiceProvider _sp;
        private readonly IMediator mediator;
        private readonly RealtimePageViewModel _realtimevm;
        private readonly ElectricScrewDriverUserControlVM _electricScrewDriverUserControlVM;

        public UILogsNotificationHandler(ILogger<UILogsNotificationHandler> logger, IServiceProvider sp,
            ElectricScrewDriverUserControlVM electricScrewDriverUserControlVM,
            RealtimePageViewModel realtimePageViewModel,
            IMediator mediator)
        {
            this._logger = logger;
            this._sp = sp;
            this.mediator = mediator;
            this._realtimevm = realtimePageViewModel;
            this._electricScrewDriverUserControlVM = electricScrewDriverUserControlVM;
        }

        public Task Handle(UILogNotification notification, CancellationToken cancellationToken)
        {
            UIHelper.RunInUIThread(pl =>
            {
                var msg = notification.LogMessage;

                if (msg != null)
                {

                    if (msg is AlarmMessage alarmmsg)
                    {
                        this._logger.Log(msg.Level, msg.Content);
                        mediator.Publish(new AlarmSYSNotification { Code = AlarmCode.系统运行错误, Name = AlarmCode.系统运行错误.ToString(), Module = AlarmModule.DESOUTTER_MODULE, Description = alarmmsg.Content });
                    }
                    //else if (msg is MidLogMessage midmsg)
                    //{
                    //    if (App._StepStationSetting.StepType != Yee.Common.Library.CommonEnum.StepTypeEnum.自动站)
                    //    {
                    //        _electricScrewDriverUserControlVM.AddMidMsg?.Execute(midmsg);
                    //    }
                    //}
                    else if (msg is LogMessage logmsg)
                    {
                        this._logger.Log(msg.Level, msg.Content);
                        _realtimevm.AddLogMsg?.Execute(logmsg);
                    }
                }
            });
            return Task.CompletedTask;
        }


    }

}
