using Ctp0600P.Client.PLC.Common;
using Ctp0600P.Client.PLC.Context;
using Itminus.Middlewares;
using Microsoft.Extensions.Logging;

namespace Ctp0600P.Client.PLC.PLC01.Middlewares
{
    public class DealClientGeneralStatusMiddleware : IWorkMiddleware<ScanContext>
    {
        private readonly StationPLCContext _manualStationNotifyPLCContext;

        public DealClientGeneralStatusMiddleware(ILogger<DealClientGeneralStatusMiddleware> logger, StationPLCContext manualStationNotifyPLCContext)
        {
            _manualStationNotifyPLCContext = manualStationNotifyPLCContext;
        }

        public async Task InvokeAsync(ScanContext context, WorkDelegate<ScanContext> next)
        {
            var letGo = _manualStationNotifyPLCContext.LetGo;
            var alarm = _manualStationNotifyPLCContext.Alarm;
            var overrunAlarm = _manualStationNotifyPLCContext.OverrunAlarm;


            //将消息写入PLC
            context.MstMsg.Status = new MstMsg_GeneralStatusBuilder(context.MstMsg.Status)
                .SetLetGoOnOff(letGo)
                .SetAlarmOnOff(alarm)
                .SetOverrunAlarmOnOff(overrunAlarm)
                .Build();


            await next(context);
        }
    }
}
