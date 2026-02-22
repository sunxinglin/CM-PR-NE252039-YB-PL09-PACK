using AsZero.Core.Services.Messages;

using Ctp0600P.Client.Protocols;
using Ctp0600P.Shared;

using Desoutter.ElectricScrewDriver;

using MediatR;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System.Net.Sockets;
using System.Text;
using System.Threading;

using Yee.Entitys.AlarmMgmt;
using Yee.Entitys.DTOS;
using Yee.Entitys.Production;

namespace Desoutter.ElectricScrewDriver.BackGroundService
{
    public class DesoutterBackgroundService : BackgroundService
    {
        private readonly ILogger<DesoutterBackgroundService> _logger;
        private readonly IMediator _mediator;
        private readonly SendMessage _sendMessage;
        private readonly DesoutterMgr _desoutterMgr;
        private readonly StepStationSetting _stationSetting;
        //public int duringKeepAlive = 0;
        public int duringSetTime = 100000;
        public DesoutterBackgroundService(ILogger<DesoutterBackgroundService> logger, IMediator mediator,
                        RecvMessage recvMessage, SendMessage sendMessage, IAPIHelper apiHelper, IOptionsMonitor<StepStationSetting> stationSetting,
                        DesoutterMgr desoutterMgr)
        {
            _logger = logger;
            _mediator = mediator;
            _sendMessage = sendMessage;
            _desoutterMgr = desoutterMgr;
            _stationSetting = stationSetting.CurrentValue;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _desoutterMgr.LoadProtocols();
            foreach (var ctrl in _desoutterMgr.DesoutterCtrls)
            {
                var thread = new Thread(() =>
                {
                    _ = RunAsync(stoppingToken, ctrl.Value);
                });
                thread.IsBackground = true;
                thread.Start();
            }
        }

        private async Task RunAsync(CancellationToken ct, DesoutterCtrl ctrl)
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    if (!ctrl._tcpClient.Connected)
                    {
                        await ctrl.ConnectAsync();
                    }

                    var readBuffer = await ctrl.Read();
                    var listMessage = ctrl.SplitMessage(readBuffer);

                    foreach (var message in listMessage)
                    {
                        await ctrl.DispatchMessage(message);
                    }

                    //if (duringKeepAlive > 20)
                    //{
                    await ctrl.SendNoLog("MID9999_KeepAliveMessage", _sendMessage.MID9999_Keep_Alive_Message());
                    //duringKeepAlive = 0;
                    //}

                    if (duringSetTime > 100000)
                    {
                        await ctrl.Send("MID0082_SetTime", _sendMessage.MID0082_Set_Time());
                        duringSetTime = 0;
                    }

                    //duringKeepAlive++;
                    duringSetTime++;
                    if (!ctrl._tcpClient.Connected)
                    {
                        if (!ctrl.HasAlarm)
                        {
                            await ctrl.SendAlarm(_stationSetting.StationCode);
                            ctrl.HasAlarm = true;
                        }
                    }
                    else
                    {
                        if (ctrl.HasAlarm)
                        {
                            await ctrl.ClearAlarm(_stationSetting.StationCode);
                            ctrl.HasAlarm = false;
                        }
                        else
                        {
                        }
                    }

                }
                catch (Exception ex)
                {
                    if (!ctrl.HasAlarm)
                    {
                        await ctrl.SendAlarm(_stationSetting.StationCode);
                        ctrl.HasAlarm = false;
                    }
                    _logger.LogError("【" + ctrl._desoutterResource.Name + "】" + ex.Message);
                    await _mediator.Publish(new UILogNotification(new MidLogMessage { Content = ex.Message }));
                }
                await Task.Delay(200);
            }
        }
    }
}
