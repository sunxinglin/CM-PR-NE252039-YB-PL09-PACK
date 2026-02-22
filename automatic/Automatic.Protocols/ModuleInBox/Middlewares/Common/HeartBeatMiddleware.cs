using Automatic.Protocols.Common;
using Itminus.Middlewares;

namespace Automatic.Protocols.ModuleInBox.Middlewares.Common
{

    public class HeartBeatMiddleware : IWorkMiddleware<ScanContext>
    {
        public HeartBeatMiddleware()
        {
        }

        public async Task InvokeAsync(ScanContext context, WorkDelegate<ScanContext> next)
        {
            var builder = new MstMsg_GeneralCmdFlagsBuilder(context.MstMsg.CmdFlags);
            //响应PLC心跳请求
            builder.SetHeartBeatAnswerOnOff(context.DevMsg.CmdFlags.HasFlag(DevMsg_GeneralCmdFlags.Heartbeat_Req));

            //mes当前心跳请求
            var mstReq = context.MstMsg.CmdFlags.HasFlag(MstMsg_GeneralCmdFlags.HeartBeatReq);
            var plcAck = context.DevMsg.CmdFlags.HasFlag(DevMsg_GeneralCmdFlags.Heartbeat_Answer);
            if (mstReq == plcAck)
            {
                builder.SetHeartBeatReqOnOff(!mstReq);
            }

            context.MstMsg.CmdFlags = builder.Build();

            await next(context);
        }

    }

}
