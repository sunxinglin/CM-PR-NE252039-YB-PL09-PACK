using Automatic.Protocols.TerminalReshape.Middlewares;
using Automatic.Protocols.TerminalReshape.Middlewares.Common;
using Automatic.Protocols.TerminalReshape.Middlewares.Common.PublishNotification;
using Itminus.Middlewares;


namespace Automatic.Protocols.TerminalReshape
{
    /// <summary>
    /// 处理器
    /// </summary>
    public class ScanProcessor
    {
        private WorkDelegate<ScanContext> BuildContainer()
        {
            var container = new WorkBuilder<ScanContext>()
                .Use<PublishNotificationMiddleware>()     // 发布
                .Use<HeartBeatMiddleware>()               // 心跳
            #region 具体业务中间件
                .Use<DealReqVectorEnterMiddleware>()
                .Use<DealReqStartReshapeMiddleware>()
                .Use<DealReqCompleteReshapeMiddleware>()
            #endregion
                .Use<FlushPendingMiddleware>()
                .Build();

            return container;
        }

        public async Task HandleAsync(ScanContext ctx)
        {
            var workcontainer = BuildContainer();
            await workcontainer.Invoke(ctx);
        }

    }
}
