using Automatic.Protocols.Common;
using Automatic.Protocols.LowerBoxGlue.Middlewares.Common;
using Automatic.Protocols.LowerBoxGlue.Models;
using Automatic.Protocols.LowerBoxGlue.Models.Wraps;
using Automatic.Protocols.Services;
using Automatic.Shared;
using Itminus.FSharpExtensions;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.FSharp.Core;

namespace Automatic.Protocols.LowerBoxGlue.Middlewares
{
    public class DealReqStartGlueMiddleware : HandlePlcRequestMiddlewareBase<DevMsg, MstMsg, DealReqStartGlueWraps, DealReqStartGlueOkWraps, DealReqStartGlueNgWraps>
    {
        private readonly CommonService _commonService;
        public readonly IOptionsMonitor<ApiServerSetting> _ApiServerSetting;
        private readonly bool _isEmptyLoop;

        public DealReqStartGlueMiddleware(LowerBoxGlueFlusher flusher, ILogger<DealReqStartGlueMiddleware> logger, IMediator mediator, CommonService commonService, IOptionsMonitor<ApiServerSetting> apiServerSetting) : base(flusher, logger, mediator)
        {
            _commonService = commonService;
            _ApiServerSetting = apiServerSetting;
            _isEmptyLoop = _ApiServerSetting.CurrentValue.IsEmptyLoop;
        }

        public override string PlcName => PLCNames.下箱体涂胶;

        public override string ProcName => PLCNames.下箱体涂胶;

        public override bool HasAck(MstMsg p) => p.StartGlue.Flag.HasFlag(MstMsgFlag.Ack);

        public override bool HasReq(DevMsg i) => i.StartGlue.Flag.HasFlag(DevMsgFlag.Req);

        public override DevMsg RefIncoming(ScanContext ctx) => ctx.DevMsg;

        public override MstMsg RefPending(ScanContext ctx) => ctx.MstMsg;

        protected override FSharpResult<DealReqStartGlueWraps, string> TryParseArgs(DevMsg incoming)
        {
            var VectorCode = incoming.StartGlue.VectorCode;
            var PackCode = incoming.StartGlue.PackCode.EffectiveContent;
            var result = new DealReqStartGlueWraps
            {
                VectorCode = VectorCode,
                PackCode = PackCode,
            };

            return result.ToOkResult<DealReqStartGlueWraps, string>();
        }

        protected override async Task<FSharpResult<DealReqStartGlueOkWraps, DealReqStartGlueNgWraps>> HandleArgsAsync(DealReqStartGlueWraps args)
        {
            await RecordLogAsync(LogLevel.Information, $"收到PLC请求开始涂胶---[载具编号：{args.VectorCode}]-[产品条码：{args.PackCode}]");
            if (_isEmptyLoop)
            {
                return new DealReqStartGlueOkWraps().ToOkResult<DealReqStartGlueOkWraps, DealReqStartGlueNgWraps>();
            }

            var q = from productPn in _commonService.GetProductPnFromCatlMes(args.PackCode, PlcStationOpt.StationCode)
                    from checkVector in _commonService.CheckProductFlowAndVectorBind(args.PackCode, PlcStationOpt.StationCode, args.VectorCode, PlcStationOpt.StationCode, productPn)
                    let checkVectorLog = RecordLogAsync(LogLevel.Information, $"载具绑定关系校验通过")
                    from start in _commonService.MakePackStart(args.PackCode, PlcStationOpt.StationCode)
                    from cmr in _commonService.EnsureExiestOrCreateMessionRecord(args.PackCode, PlcStationOpt.StationCode, ProcName, args.VectorCode, productPn)
                    let cmrResponseLog = RecordLogAsync(LogLevel.Information, $"创建生产追溯信息成功")
                    select new { cmr };

            var r = await q;
            if (r.IsError)
            {
                var errValue = r.ErrorValue;
                return new DealReqStartGlueNgWraps() { ErrorType = errValue.ErrorType, ErrorCode = errValue.ErrorCode, ErrorMessage = errValue.ErrorMessage }.ToErrResult<DealReqStartGlueOkWraps, DealReqStartGlueNgWraps>();
            }

            var okResult = new DealReqStartGlueOkWraps() { };
            return okResult.ToOkResult<DealReqStartGlueOkWraps, DealReqStartGlueNgWraps>();
        }

        protected override async Task HandleErrAsync(MstMsg pending, DealReqStartGlueNgWraps err)
        {
            var builder = new MstMsgFlagBuilder(pending.StartGlue.Flag);
            pending.StartGlue.Flag = builder.Ack(false).Build();
            pending.StartGlue.ErrorCode = (ushort)err.ErrorCode;

            await RecordLogAsync(LogLevel.Error, $"PLC请求开始涂胶NG：{err.ErrorCode} - {err.ErrorMessage}");
        }

        protected override async Task HandleOkAsync(MstMsg pending, DealReqStartGlueOkWraps descriptions)
        {
            var builder = new MstMsgFlagBuilder(pending.StartGlue.Flag);
            pending.StartGlue.Flag = builder.Ack(true).Build();
            pending.StartGlue.ErrorCode = 0;
            await RecordLogAsync(LogLevel.Information, $"PLC请求开始涂胶OK");
        }

        protected override async Task ResetAckAsync(MstMsg pending)
        {
            var builder = new MstMsgFlagBuilder(pending.StartGlue.Flag);
            pending.StartGlue.Flag = builder.Reset().Build();
            await RecordLogAsync(LogLevel.Information, $"处理PLC请求开始涂胶结束");
        }

    }
}
