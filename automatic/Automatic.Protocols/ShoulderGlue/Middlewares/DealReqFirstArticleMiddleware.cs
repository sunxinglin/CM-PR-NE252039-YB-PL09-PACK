using Automatic.Protocols.Common;
using Automatic.Protocols.Services;
using Automatic.Protocols.ShoulderGlue.Middlewares.Common;
using Automatic.Protocols.ShoulderGlue.Models;
using Automatic.Protocols.ShoulderGlue.Models.Wraps;
using Automatic.Shared;
using Itminus.FSharpExtensions;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.FSharp.Core;

namespace Automatic.Protocols.ShoulderGlue.Middlewares
{
    public class DealReqFirstArticleMiddleware
        : HandlePlcRequestMiddlewareBase<DevMsg, MstMsg, DealReqFirstArticleWraps, DealReqFirstArticleOkWraps, DealReqFirstArticleNgWraps>
    {
        private readonly AutoGlueService _glueService;
        private readonly IOptionsMonitor<ApiServerSetting> _ApiServerSetting;
        private readonly bool _isEmptyLoop;

        public DealReqFirstArticleMiddleware(ShoulderGlueFlusher flusher, ILogger<DealReqFirstArticleMiddleware> logger, IMediator mediator, AutoGlueService glueService, IOptionsMonitor<ApiServerSetting> apiServerSetting) : base(flusher, logger, mediator)
        {
            _glueService = glueService;
            _ApiServerSetting = apiServerSetting;
            _isEmptyLoop = _ApiServerSetting.CurrentValue.IsEmptyLoop;
        }

        public override string PlcName => PLCNames.肩部涂胶;

        public override string ProcName => PLCNames.肩部涂胶;

        public override bool HasAck(MstMsg p) => p.FirstArticle.Flag.HasFlag(MstMsgFlag.Ack);

        public override bool HasReq(DevMsg i) => i.FirstArticle.Flag.HasFlag(DevMsgFlag.Req);

        public override DevMsg RefIncoming(ScanContext ctx) => ctx.DevMsg;

        public override MstMsg RefPending(ScanContext ctx) => ctx.MstMsg;

        protected override FSharpResult<DealReqFirstArticleWraps, string> TryParseArgs(DevMsg incoming)
        {
            var result = new DealReqFirstArticleWraps
            {
            };

            _mediator.Publish(result);
            return result.ToOkResult<DealReqFirstArticleWraps, string>();
        }

        protected override async Task<FSharpResult<DealReqFirstArticleOkWraps, DealReqFirstArticleNgWraps>> HandleArgsAsync(DealReqFirstArticleWraps args)
        {
            await RecordLogAsync(LogLevel.Information, $"PLC请求涂胶首件");
            if (_isEmptyLoop)
            {
                return new DealReqFirstArticleOkWraps().ToOkResult<DealReqFirstArticleOkWraps, DealReqFirstArticleNgWraps>();
            }
            return new DealReqFirstArticleOkWraps().ToOkResult<DealReqFirstArticleOkWraps, DealReqFirstArticleNgWraps>();
        }

        protected override async Task HandleErrAsync(MstMsg pending, DealReqFirstArticleNgWraps err)
        {
            var builder = new MstMsgFlagBuilder(pending.FirstArticle.Flag);
            pending.FirstArticle.Flag = builder.Ack(false).Build();
            pending.FirstArticle.ErrorCode = (ushort)err.ErrorCode;

            await RecordLogAsync(LogLevel.Error, $"PLC请求涂胶首件NG：{err.ErrorCode} - {err.ErrorMessage}");
        }

        protected override async Task HandleOkAsync(MstMsg pending, DealReqFirstArticleOkWraps descriptions)
        {
            var builder = new MstMsgFlagBuilder(pending.FirstArticle.Flag);
            pending.FirstArticle.Flag = builder.Ack(true).Build();
            pending.FirstArticle.ErrorCode = 0;

            await RecordLogAsync(LogLevel.Information, $"PLC请求涂胶首件OK");
        }

        protected override async Task ResetAckAsync(MstMsg pending)
        {
            var builder = new MstMsgFlagBuilder(pending.FirstArticle.Flag);
            pending.FirstArticle.Flag = builder.Reset().Build();

            await RecordLogAsync(LogLevel.Information, $"处理PLC请求涂胶首件结束");
        }

    }
}
