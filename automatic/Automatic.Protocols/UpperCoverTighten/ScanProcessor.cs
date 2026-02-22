using Automatic.Protocols.UpperCoverTighten.Middlewares;
using Automatic.Protocols.UpperCoverTighten.Middlewares.Common;
using Automatic.Protocols.UpperCoverTighten.Middlewares.Common.PublishNotification;
using Itminus.Middlewares;

namespace Automatic.Protocols.UpperCoverTighten
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

            #endregion
                .Use<FlushPendingMiddleware>()
                .Use<DealReqEnterStationMiddleware>()
                .Use<DealReqStartTightenMiddleware>()
                .Use<DealReqTightenCompleteMiddleware>()
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
