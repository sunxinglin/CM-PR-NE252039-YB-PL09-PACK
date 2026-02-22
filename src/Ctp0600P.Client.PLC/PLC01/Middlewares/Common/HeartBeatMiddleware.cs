using Ctp0600P.Client.PLC.Common;
using Itminus.Middlewares;

namespace Ctp0600P.Client.PLC.PLC01.Middlewares.Common
{

    public class HeartBeatMiddleware : IWorkMiddleware<ScanContext>
    {
        public HeartBeatMiddleware()
        {
        }

        public async Task InvokeAsync(ScanContext context, WorkDelegate<ScanContext> next)
        {
            //响应PLC心跳请求
            context.MstMsg.CmdFlags = new MstMsg_GeneralCmdFlagsBuilder(context.MstMsg.CmdFlags)
                .SetHeartBeatAnswerOnOff(context.DevMsg.CmdFlags.HasFlag(DevMsg_GeneralCmdFlags.HeartbeatReq))
                .Build();

            await next(context);
        }

    }

}
