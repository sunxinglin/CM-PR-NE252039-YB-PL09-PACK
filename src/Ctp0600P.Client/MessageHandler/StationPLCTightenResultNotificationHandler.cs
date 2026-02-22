using Ctp0600P.Client.PLC.PLC01.Models.Notifications;
using Ctp0600P.Client.Protocols.BoltGun.Models;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ctp0600P.Client.MessageHandler
{
    public class StationPLCTightenResultNotificationHandler : INotificationHandler<StationPLCTightenResultNotification>
    {
        private readonly IMediator _mediator;

        public StationPLCTightenResultNotificationHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Handle(StationPLCTightenResultNotification notification, CancellationToken cancellationToken)
        {
            var request = new BoltGunRequest()
            {
                DeviceNo = notification.DeviceNo.ToString(),
                DeviceBrand = notification.DeviceBrand,
                ResultIsOK = notification.TightenResult,
                ProgramNo = Convert.ToInt32(notification.PSet),
                Angle_Max = Convert.ToDecimal(notification.AngleMax),
                Angle_Min = Convert.ToDecimal(notification.AngleMin),
                AngleStatus = Convert.ToInt32(notification.AngleTrend),
                TargetAngle = Convert.ToDecimal(notification.TargetAngle),
                FinalAngle = Convert.ToDecimal(notification.FinalAngle),
                FinalTorque = Convert.ToDecimal(notification.FinalTorque),
                TargetTorqueRate = Convert.ToDecimal(notification.TargetTorqueRate),
                TorqueRate_Max = Convert.ToDecimal(notification.ToruqeRateMax),
                TorqueRate_Min = Convert.ToDecimal(notification.TorqueRateMin),
                TorqueStatus = Convert.ToInt32(notification.TorqueTrend),
            };
            App.BoltGunRequestSubject.OnNext(request);
        }
    }
}
