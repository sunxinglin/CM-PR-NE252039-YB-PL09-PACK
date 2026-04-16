using Ctp0600P.Client.PLC.PLC01.Middlewares;
using Ctp0600P.Client.PLC.PLC01.Middlewares.Common;
using Ctp0600P.Client.PLC.PLC01.Middlewares.Common.PublishNotification;

using Itminus.Middlewares;

namespace Ctp0600P.Client.PLC.PLC01;

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
            .Use<DealClientInitialReqMiddleware>()
            .Use<DealClientAGVBindPackMiddleware>()
            .Use<DealClientGeneralStatusMiddleware>()
            .Use<DealClientReleaseAGVMiddleware>()
            .Use<DealClientTightenStartMiddleware>()
            .Use<DealPLCAGVArriveMiddleware>()
            .Use<DealPLCAGVLeaveMiddleware>()
            .Use<DealPLCAlarmResetMiddleware>()
            .Use<DealPLCLetGoMiddleware>()
            .Use<DealPLCModeMiddleware>()
            .Use<DealPLCTightenCompleteMiddleware>()
            .Use<DealClientLeakStartMiddware>()
            .Use<DealClientLeakCompleteMiddleware>()
            #endregion

            .Use<FlushPendingMiddleware>()
            .Build();

        return container;
    }

    public async Task HandleAsync(ScanContext ctx)
    {
        var workContainer = BuildContainer();
        await workContainer.Invoke(ctx);
    }

}