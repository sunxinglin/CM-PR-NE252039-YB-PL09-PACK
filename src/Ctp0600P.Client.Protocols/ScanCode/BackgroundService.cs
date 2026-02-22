using AsZero.Core.Services.Messages;
using Ctp0600P.Client.Protocols.ScanCode;
using Ctp0600P.Client.Protocols.ScanCode.Models;
using Ctp0600P.Shared;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yee.Entitys.CommonEntity;
using Yee.Entitys.Production;
using System.Windows;
using Yee.Entitys.AlarmMgmt;

namespace Ctp0600P.Client.Protocols
{
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
}
