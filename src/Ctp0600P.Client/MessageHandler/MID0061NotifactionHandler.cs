//using AsZero.Core.Services.Messages;
//using Ctp0600P.Client.Protocols.BoltGun.Models;
//using MediatR;
//using Microsoft.Extensions.Logging;
//using Newtonsoft.Json;
//using System;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Ctp0600P.Client.MessageHandler
//{
//    public class MID0061NotifactionHandler : INotificationHandler<MID0061_LastTighteningResultDataNotifaction>, INotificationHandler<MID0061_LastTighteningResultData_BSNotifaction>
//    {
//        private readonly ILogger<MID0061NotifactionHandler> _logger;
//        private readonly IMediator _mediator;

//        public MID0061NotifactionHandler(ILogger<MID0061NotifactionHandler> logger, IMediator mediator)
//        {
//            this._logger = logger;
//            _mediator = mediator;
//        }

//        //马头
//        public async Task Handle(MID0061_LastTighteningResultDataNotifaction notification, CancellationToken cancellationToken)
//        {
//            var result = notification.LastTighteningResultData.dataField;
//            var msg = new LogMessage
//            {
//                Content = $"当前拧紧结果结果：{JsonConvert.SerializeObject(result)}",
//                Level = LogLevel.Information,
//                Timestamp = DateTime.Now,
//            };
//            await _mediator.Publish(new UILogNotification(msg));

//            var request = new BoltGunRequest()
//            {
//                ResultIsOK = result._11_TighteningStatus == "1",
//                DeviceNo = notification.DeviceNo,
//                ProgramNo = int.Parse(result._06_ParameterSetNumber),
//                Angle_Max = decimal.Parse(result._26_AngleMax),
//                Angle_Min = decimal.Parse(result._25_AngleMin),
//                AngleStatus = int.Parse(result._14_AngleStatus),
//                TargetAngle = decimal.Parse(result._27_FinalAngleTarget),
//                ///角度处理后的值应/100，为控制器显示的值
//                FinalAngle = decimal.Parse(result._57_FinalAngleDecimal) / 100,
//                ///扭矩处理后的值应/100，为控制器显示的值
//                FinalTorque = decimal.Parse(result._24_Torque) / 100,
//                TargetTorqueRate = decimal.Parse(result._23_TorqueFinalTarget),
//                TorqueRate_Max = decimal.Parse(result._22_TorqueMaxLimit),
//                TorqueRate_Min = decimal.Parse(result._21_TorqueMinLimit),
//                TorqueStatus = int.Parse(result._13_TorqueStatus),
//            };
//            App.BoltGunRequestSubject.OnNext(request);
//        }

//        //博世
//        public async Task Handle(MID0061_LastTighteningResultData_BSNotifaction notification, CancellationToken cancellationToken)
//        {
//            var result = notification.LastTighteningResultData.dataField;

//            var msg = new LogMessage
//            {
//                Content = $"当前拧紧结果结果：{JsonConvert.SerializeObject(result)}",
//                Level = LogLevel.Information,
//                Timestamp = DateTime.Now,
//            };
//            await _mediator.Publish(new UILogNotification(msg));

//            var request = new BoltGunRequest()
//            {
//                ResultIsOK = result._09_TightenStatus == "1",
//                DeviceNo = notification.DeviceNo,
//                ProgramNo = int.Parse(result._06_ParameterSetNumber),
//                Angle_Max = decimal.Parse(result._17_MaxAngle),
//                Angle_Min = decimal.Parse(result._16_MinAngle),
//                AngleStatus = int.Parse(result._11_AngleStatus),
//                TargetAngle = decimal.Parse(result._18_TargetAngle),
//                FinalAngle = decimal.Parse(result._19_FinalAngle),
//                FinalTorque = decimal.Parse(result._15_FinalTorque) / 100,
//                TargetTorqueRate = decimal.Parse(result._14_TargetTorque),
//                TorqueRate_Max = decimal.Parse(result._13_MaxTorque),
//                TorqueRate_Min = decimal.Parse(result._12_MinTorque),
//                TorqueStatus = int.Parse(result._10_TorqueStatus),
//                TightenID = int.Parse(result._23_TightenID)
//            };
//            App.BoltGunRequestSubject.OnNext(request);
//        }
//    }
//}
