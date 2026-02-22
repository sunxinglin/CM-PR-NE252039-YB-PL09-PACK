//using AsZero.Core.Services.Messages;
//using Ctp0600P.Client.Protocols.BoltGun;
//using Ctp0600P.Client.Protocols.BoltGun.Models;
//using Ctp0600P.Shared;
//using MediatR;
//using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.Options;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Yee.Entitys.AlarmMgmt;
//using Yee.Entitys.CommonEntity;
//using Yee.Entitys.Production;

//namespace Ctp0600P.Client.Protocols
//{
//    public class BoltGunService : BackgroundService
//    {
//        public BoltGunService(IAPIHelper apiHelper, IOptionsMonitor<StationSetting> cloudSetting, IMediator mediator)
//        {
//            ApiHelper = apiHelper;
//            this._stationSetting = cloudSetting.Get("StationSetting") ?? throw new ArgumentNullException(nameof(cloudSetting));
//            Mediator = mediator;
//        }

//        public IAPIHelper ApiHelper { get; }
//        private readonly StationSetting _stationSetting;
//        public IMediator Mediator { get; }
//        private StationProResource _ScanConfigList { get; set; }

//        protected override Task ExecuteAsync(CancellationToken stoppingToken)
//        {
//            //_ScanConfigList = ApiHelper.LoadStationProResource_ScanGun(_stationSetting);
//            //if (_ScanConfigList != null && _ScanConfigList.StationProResourceList != null && _ScanConfigList.StationProResourceList.Count > 0)
//            //{
//            //    foreach (var config in _ScanConfigList.StationProResourceList)
//            //    {
//            //        var thread = new Thread(() =>
//            //        {
//            //            _ = RunAsync(config);
//            //        });
//            //        thread.Start();
//            //    }
//            //}

//            return Task.CompletedTask;
//        }

//        private void Serial_ErrorMsg(string message)
//        {
//            Mediator.Publish(new AlarmSYSNotification() { Code = AlarmCode.拧紧枪错误, Name = AlarmCode.拧紧枪错误.ToString(), Module = AlarmModule.DESOUTTER_MODULE, Description = $"{message}" });
//            //this.Mediator.Publish(new UILogNotification(new LogMessage { Content = message }));
//        }

//        private void Serial_DataReceived(string content, string portName)
//        {
//            BoltGunRequest request = new BoltGunRequest
//            {
//                BoltGunState = "OK",
//                BoltGunValue = "3.0N",
//                DeviceNo = "1",
//            };
//            var response = Mediator.Send(request);
//        }
//        private async Task RunAsync(Base_ProResource config)
//        {
//            var portName = config.Port;
//            var baudRate = config.Baud;

//            SerialPortCommHelper comm = new SerialPortCommHelper(portName, baudRate);
//            comm.DataReceived += new SerialPortCommHelper.EventHandle(Serial_DataReceived);
//            comm.ComError += new SerialPortCommHelper.ErrorEventHandler(Serial_ErrorMsg);
//            comm.Open();
//        }

//    }
//}
