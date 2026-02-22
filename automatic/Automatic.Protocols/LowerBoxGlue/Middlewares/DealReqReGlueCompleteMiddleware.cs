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
    public class DealReqReGlueCompleteMiddleware
        : HandlePlcRequestMiddlewareBase<DevMsg, MstMsg, DealReqReGlueCompleteWraps, DealReqReGlueCompleteOkWraps, DealReqReGlueCompleteNgWraps>
    {
        private readonly LowerBoxGlueService _lowerBoxGlueService;
        private readonly LowerBoxReGlueService _reGlueService;
        public readonly IOptionsMonitor<ApiServerSetting> _ApiServerSetting;
        private readonly bool _isEmptyLoop;

        public DealReqReGlueCompleteMiddleware(LowerBoxGlueFlusher flusher, ILogger<DealReqReGlueCompleteMiddleware> logger, IMediator mediator,
            LowerBoxReGlueService reGlueService, IOptionsMonitor<ApiServerSetting> apiServerSetting, LowerBoxGlueService lowerBoxGlueService) : base(flusher, logger, mediator)
        {
            _reGlueService = reGlueService;
            _ApiServerSetting = apiServerSetting;
            _isEmptyLoop = _ApiServerSetting.CurrentValue.IsEmptyLoop;
            _lowerBoxGlueService = lowerBoxGlueService;
        }

        public override string PlcName => PLCNames.下箱体涂胶;

        public override string ProcName => PLCNames.下箱体涂胶;

        public override bool HasAck(MstMsg p) => p.ReGlueComplete.Flag.HasFlag(MstMsgFlag.Ack);

        public override bool HasReq(DevMsg i) => i.ReGlueComplete.Flag.HasFlag(DevMsgFlag.Req);

        public override DevMsg RefIncoming(ScanContext ctx) => ctx.DevMsg;

        public override MstMsg RefPending(ScanContext ctx) => ctx.MstMsg;

        protected override async Task<FSharpResult<DealReqReGlueCompleteOkWraps, DealReqReGlueCompleteNgWraps>> HandleArgsAsync(DealReqReGlueCompleteWraps args)
        {
            await RecordLogAsync(LogLevel.Information, $"收到PLC请求补胶完成---[载具编号：{args.VectorCode}]-[产品条码：{args.PackCode}]");

            var okResult = new DealReqReGlueCompleteOkWraps();

            if (_isEmptyLoop)
            {
                return okResult.ToOkResult<DealReqReGlueCompleteOkWraps, DealReqReGlueCompleteNgWraps>();
            }

            if (!DateTime.TryParse(args.StartTime, out _))
            {
                return new DealReqReGlueCompleteNgWraps { ErrorCode = 400, ErrorMessage = "补胶开始时间格式异常", ErrorType = string.Empty }.ToErrResult<DealReqReGlueCompleteOkWraps, DealReqReGlueCompleteNgWraps>();
            }

            var datas = new LowerBoxReGlueDataDto()
            {
                PackCode = args.PackCode,
                VectorCode = args.VectorCode,
                StationCode = PlcStationOpt.StationCode,
                GlueStartTime = args.StartTime,
            };

            var q = from checkTime in _lowerBoxGlueService.CheckAllowReGlue(args.PackCode, "LowerBoxGlueStartTime")
                    let checkTimeLog = RecordLogAsync(LogLevel.Information, $"补胶时间间隔校验通过")
                    from saveResp in _reGlueService.SaveDataAndUploadCATL(datas)
                    select saveResp;
            var r = await q;
            if (r.IsError)
            {
                var errValue = r.ErrorValue;
                return new DealReqReGlueCompleteNgWraps { ErrorCode = errValue.ErrorCode, ErrorMessage = errValue.ErrorMessage, ErrorType = errValue.ErrorType }.ToErrResult<DealReqReGlueCompleteOkWraps, DealReqReGlueCompleteNgWraps>();
            }
            return okResult.ToOkResult<DealReqReGlueCompleteOkWraps, DealReqReGlueCompleteNgWraps>();
        }

        protected override async Task HandleErrAsync(MstMsg pending, DealReqReGlueCompleteNgWraps err)
        {
            var builder = new MstMsgFlagBuilder(pending.ReGlueComplete.Flag);
            pending.ReGlueComplete.Flag = builder.Ack(false).Build();
            pending.ReGlueComplete.ErrorCode = (ushort)err.ErrorCode;

            await RecordLogAsync(LogLevel.Error, $"PLC请求补胶完成NG：{err.ErrorCode} - {err.ErrorMessage}");
        }

        protected override async Task HandleOkAsync(MstMsg pending, DealReqReGlueCompleteOkWraps descriptions)
        {
            var builder = new MstMsgFlagBuilder(pending.ReGlueComplete.Flag);
            pending.ReGlueComplete.Flag = builder.Ack(true).Build();
            pending.ReGlueComplete.ErrorCode = 0;

            await RecordLogAsync(LogLevel.Information, $"PLC请求补胶完成OK");
        }

        protected override async Task ResetAckAsync(MstMsg pending)
        {
            var builder = new MstMsgFlagBuilder(pending.ReGlueComplete.Flag);
            pending.ReGlueComplete.Flag = builder.Reset().Build();

            await RecordLogAsync(LogLevel.Information, $"处理PLC请求补胶完成结束");
        }

        protected override FSharpResult<DealReqReGlueCompleteWraps, string> TryParseArgs(DevMsg incoming)
        {
            var vectorCode = incoming.ReGlueComplete.VectorCode;
            var packCode = incoming.ReGlueComplete.PackCode.EffectiveContent;

            var result = new DealReqReGlueCompleteWraps
            {
                VectorCode = vectorCode,
                PackCode = packCode,
                StartTime = incoming.ReGlueComplete.ReGlueStartTime,
            };
            _mediator.Publish(result);
            return result.ToOkResult<DealReqReGlueCompleteWraps, string>();
        }
    }
}
