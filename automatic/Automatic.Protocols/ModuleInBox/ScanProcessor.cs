using Automatic.Protocols.ModuleInBox.Middlewares;
using Automatic.Protocols.ModuleInBox.Middlewares.Common;
using Automatic.Protocols.ModuleInBox.Middlewares.Common.PublishNotification;
using Itminus.Middlewares;

namespace Automatic.Protocols.ModuleInBox
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
                .Use<DealReqEnterStationMiddleware>()
                .Use<DealReqTakePhotoCompleteMiddleware>()
                .Use<DealReqStartInBoxMiddleware>()
                .Use<DealReqSingleInBoxCompleteMiddleware>()
                .Use<DealReqInBoxCompleteMiddleware>()
                //.Use<DealReqPurificationBlockCompleteMiddleware>()
                //.Use<DealReqPurificationBlockMiddleware>()
                //.Use<DealReqTakePhotoMiddleware>()
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
