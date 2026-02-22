using Automatic.Entity.DataDtos;
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
    public class DealReqReGlueStartMiddleware
        : HandlePlcRequestMiddlewareBase<DevMsg, MstMsg, DealReqReGlueStartWraps, DealReqReGlueStartOkWraps, DealReqReGlueStartNgWraps>
    {
        private readonly CommonService _commonService;
        private readonly LowerBoxGlueService _glueService;
        public readonly IOptionsMonitor<ApiServerSetting> _ApiServerSetting;
        private readonly bool _isEmptyLoop;

        public DealReqReGlueStartMiddleware(LowerBoxGlueFlusher flusher, ILogger<DealReqReGlueStartMiddleware> logger, IMediator mediator,
            LowerBoxGlueService glueService, IOptionsMonitor<ApiServerSetting> apiServerSetting, CommonService commonService) : base(flusher, logger, mediator)
        {
            _glueService = glueService;
            _ApiServerSetting = apiServerSetting;
            _isEmptyLoop = _ApiServerSetting.CurrentValue.IsEmptyLoop;
            _commonService = commonService;
        }

        public override string PlcName => PLCNames.下箱体涂胶;

        public override string ProcName => PLCNames.下箱体涂胶;

        public override bool HasAck(MstMsg p) => p.ReGlueStart.Flag.HasFlag(MstMsgFlag.Ack);

        public override bool HasReq(DevMsg i) => i.ReGlueStart.Flag.HasFlag(DevMsgFlag.Req);

        public override DevMsg RefIncoming(ScanContext ctx) => ctx.DevMsg;

        public override MstMsg RefPending(ScanContext ctx) => ctx.MstMsg;

        protected override async Task<FSharpResult<DealReqReGlueStartOkWraps, DealReqReGlueStartNgWraps>> HandleArgsAsync(DealReqReGlueStartWraps args)
        {
            await RecordLogAsync(LogLevel.Information, $"收到PLC请求补胶开始---[载具编号：{args.VectorCode}]-[产品条码：{args.PackCode}]");

            var okResult = new DealReqReGlueStartOkWraps();

            if (_isEmptyLoop)
            {
                return okResult.ToOkResult<DealReqReGlueStartOkWraps, DealReqReGlueStartNgWraps>();
            }

            var datas = new LowerBoxReGlueDataDto()
            {
                PackCode = args.PackCode,
                VectorCode = args.VectorCode,
                StationCode = PlcStationOpt.StationCode,
            };

            var q = from productPn in _commonService.GetProductPnFromCatlMes(args.PackCode, PlcStationOpt.StationCode)
                    from checkVector in _commonService.CheckProductFlowAndVectorBind(args.PackCode, PlcStationOpt.StationCode, args.VectorCode, PlcStationOpt.StationCode, productPn)
                    let checkVectorLog = RecordLogAsync(LogLevel.Information, $"载具绑定关系校验通过")
                    from checkTime in _glueService.CheckAllowReGlue(args.PackCode, "LowerBoxGlueStartTime")
                    let checkTimeLog = RecordLogAsync(LogLevel.Information, $"补胶时间间隔校验通过")
                    select productPn;
            var r = await q;
            if (r.IsError)
            {
                var errValue = r.ErrorValue;
                return new DealReqReGlueStartNgWraps { ErrorCode = errValue.ErrorCode, ErrorMessage = errValue.ErrorMessage, ErrorType = errValue.ErrorType }.ToErrResult<DealReqReGlueStartOkWraps, DealReqReGlueStartNgWraps>();
            }
            return okResult.ToOkResult<DealReqReGlueStartOkWraps, DealReqReGlueStartNgWraps>();
        }

        protected override async Task HandleErrAsync(MstMsg pending, DealReqReGlueStartNgWraps err)
        {
            var builder = new MstMsgFlagBuilder(pending.ReGlueStart.Flag);
            pending.ReGlueStart.Flag = builder.Ack(false).Build();
            pending.ReGlueStart.ErrorCode = (ushort)err.ErrorCode;

            await RecordLogAsync(LogLevel.Error, $"PLC请求补胶开始NG：{err.ErrorCode} - {err.ErrorMessage}");
        }

        protected override async Task HandleOkAsync(MstMsg pending, DealReqReGlueStartOkWraps descriptions)
        {
            var builder = new MstMsgFlagBuilder(pending.ReGlueStart.Flag);
            pending.ReGlueStart.Flag = builder.Ack(true).Build();
            pending.ReGlueStart.ErrorCode = 0;

            await RecordLogAsync(LogLevel.Information, $"PLC请求补胶开始OK");
        }

        protected override async Task ResetAckAsync(MstMsg pending)
        {
            var builder = new MstMsgFlagBuilder(pending.ReGlueStart.Flag);
            pending.ReGlueStart.Flag = builder.Reset().Build();

            await RecordLogAsync(LogLevel.Information, $"处理PLC请求补胶开始结束");
        }

        protected override FSharpResult<DealReqReGlueStartWraps, string> TryParseArgs(DevMsg incoming)
        {
            var vectorCode = incoming.ReGlueStart.VectorCode;
            var packCode = incoming.ReGlueStart.PackCode.EffectiveContent;

            var result = new DealReqReGlueStartWraps
            {
                VectorCode = vectorCode,
                PackCode = packCode,
            };
            return result.ToOkResult<DealReqReGlueStartWraps, string>();
        }
    }
}
