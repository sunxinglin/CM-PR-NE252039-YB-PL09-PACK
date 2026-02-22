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
    public class DealReqEnterStationMiddleware : HandlePlcRequestMiddlewareBase<DevMsg, MstMsg, DealReqEnterStationWraps, DealReqEnterStationOkWraps, DealReqEnterStationNgWraps>
    {
        private readonly CommonService _commonService;
        public readonly IOptionsMonitor<ApiServerSetting> _ApiServerSetting;
        private readonly bool _isEmptyLoop;

        public DealReqEnterStationMiddleware(LowerBoxGlueFlusher flusher, ILogger<DealReqEnterStationMiddleware> logger, IMediator mediator, CommonService commonService, IOptionsMonitor<ApiServerSetting> apiServerSetting) : base(flusher, logger, mediator)
        {
            _commonService = commonService;
            _ApiServerSetting = apiServerSetting;
            _isEmptyLoop = _ApiServerSetting.CurrentValue.IsEmptyLoop;
        }

        public override string PlcName => PLCNames.下箱体涂胶;

        public override string ProcName => PLCNames.下箱体涂胶;

        public override bool HasAck(MstMsg p) => p.EnterStation.Flag.HasFlag(MstMsgFlag.Ack);

        public override bool HasReq(DevMsg i) => i.EnterStation.Flag.HasFlag(DevMsgFlag.Req);

        public override DevMsg RefIncoming(ScanContext ctx) => ctx.DevMsg;

        public override MstMsg RefPending(ScanContext ctx) => ctx.MstMsg;

        protected override FSharpResult<DealReqEnterStationWraps, string> TryParseArgs(DevMsg incoming)
        {
            var VectorCode = incoming.EnterStation.VectorCode;
            var PackCode = incoming.EnterStation.PackCode.EffectiveContent;

            var result = new DealReqEnterStationWraps
            {
                VectorCode = VectorCode,
                PackCode = PackCode,
            };

            return result.ToOkResult<DealReqEnterStationWraps, string>();
        }

        protected override async Task<FSharpResult<DealReqEnterStationOkWraps, DealReqEnterStationNgWraps>> HandleArgsAsync(DealReqEnterStationWraps args)
        {
            await RecordLogAsync(LogLevel.Information, $"收到PLC请求载具进入工位---[载具编号：{args.VectorCode}]-[产品条码：{args.PackCode}]");

            if (_isEmptyLoop)
            {
                return new DealReqEnterStationOkWraps().ToOkResult<DealReqEnterStationOkWraps, DealReqEnterStationNgWraps>();
            }

            var q = from productPn in _commonService.GetProductPnFromCatlMes(args.PackCode, PlcStationOpt.StationCode)
                    from cpf in _commonService.CheckProductFlowAndVectorBind(args.PackCode, PlcStationOpt.StationCode, args.VectorCode, PlcStationOpt.StationCode, productPn)
                    let log = RecordLogAsync(LogLevel.Information, $"载具绑定关系校验通过")
                    select new { cpf };
            var result = await q;
            if (result.IsError)
            {
                var errValue = result.ErrorValue;
                return new DealReqEnterStationNgWraps { ErrorType = errValue.ErrorType, ErrorCode = errValue.ErrorCode, ErrorMessage = errValue.ErrorMessage }.ToErrResult<DealReqEnterStationOkWraps, DealReqEnterStationNgWraps>();
            }

            var okResult = new DealReqEnterStationOkWraps() { };
            return okResult.ToOkResult<DealReqEnterStationOkWraps, DealReqEnterStationNgWraps>();
        }

        protected override async Task HandleOkAsync(MstMsg pending, DealReqEnterStationOkWraps descriptions)
        {
            var builder = new MstMsgFlagBuilder(pending.EnterStation.Flag);
            pending.EnterStation.Flag = builder.Ack(true).Build();
            pending.EnterStation.ErrorCode = 0;
            await RecordLogAsync(LogLevel.Information, $"PLC请求载具进入工位OK");
        }

        protected override async Task HandleErrAsync(MstMsg pending, DealReqEnterStationNgWraps err)
        {
            var builder = new MstMsgFlagBuilder(pending.EnterStation.Flag);
            pending.EnterStation.Flag = builder.Ack(false).Build();
            pending.EnterStation.ErrorCode = (ushort)err.ErrorCode;

            await RecordLogAsync(LogLevel.Error, $"PLC请求载具进入工位NG：{err.ErrorCode} - {err.ErrorMessage}");
        }

        protected override async Task ResetAckAsync(MstMsg pending)
        {
            var builder = new MstMsgFlagBuilder(pending.EnterStation.Flag);
            pending.EnterStation.Flag = builder.Reset().Build();
            await RecordLogAsync(LogLevel.Information, $"处理PLC请求载具进入工位结束");
        }


    }
}
