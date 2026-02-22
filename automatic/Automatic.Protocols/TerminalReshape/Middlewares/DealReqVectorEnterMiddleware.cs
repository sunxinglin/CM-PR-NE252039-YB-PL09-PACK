using Automatic.Protocols.Services;
using Automatic.Protocols.TerminalReshape.Middlewares.Common;
using Automatic.Protocols.TerminalReshape.Models;
using Automatic.Protocols.TerminalReshape.Models.WorkRequireFlags;
using Automatic.Protocols.TerminalReshape.Models.Wraps;
using Automatic.Shared;
using Itminus.FSharpExtensions;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.FSharp.Core;

namespace Automatic.Protocols.TerminalReshape.Middlewares
{
    public class DealReqVectorEnterMiddleware
        : HandlePlcRequestMiddlewareBase<DevMsg, MstMsg, DealReqVectorEnterWraps, DealReqVectorEnterOkWraps, DealReqVectorEnterNgWraps>
    {
        private readonly CommonService _commonService;
        private readonly IOptionsMonitor<ApiServerSetting> _ApiServerSetting;
        private readonly ModuleInBoxService _moduleInBoxService;
        private readonly bool _isEmptyLoop;
        public DealReqVectorEnterMiddleware(TerminalReshapeFlusher flusher, ILogger<DealReqVectorEnterMiddleware> logger, IMediator mediator, CommonService commonService, IOptionsMonitor<ApiServerSetting> apiServerSetting, ModuleInBoxService moduleInBoxService) : base(flusher, logger, mediator)
        {
            _commonService = commonService;
            _ApiServerSetting = apiServerSetting;
            _moduleInBoxService = moduleInBoxService;
            _isEmptyLoop = _ApiServerSetting.CurrentValue.IsEmptyLoop;
        }

        public override string PlcName => PLCNames.极柱整形;

        public override string ProcName => PLCNames.极柱整形;

        public override bool HasAck(MstMsg p) => p.MstMsgAckVectorEnterFlag.HasFlag(MstMsgAckVectorEnterFlag.AckVectorEnter);

        public override bool HasReq(DevMsg i) => i.DevMsgReqVectorEnterFlag.HasFlag(DevMsgReqVectorEnterFlag.ReqVectorEnter);

        public override DevMsg RefIncoming(ScanContext ctx) => ctx.DevMsg;

        public override MstMsg RefPending(ScanContext ctx) => ctx.MstMsg;

        protected override async Task<FSharpResult<DealReqVectorEnterOkWraps, DealReqVectorEnterNgWraps>> HandleArgsAsync(DealReqVectorEnterWraps args)
        {
            await RecordLogAsync(LogLevel.Information, $"收到PLC请求载具进入工位---[载具编号：{args.VectorCode}]-[产品条码：{args.PackCode}]");
            if (_isEmptyLoop)
            {
                return new DealReqVectorEnterOkWraps().ToOkResult<DealReqVectorEnterOkWraps, DealReqVectorEnterNgWraps>();
            }

            var q = from newStationCode in _commonService.GetCurrentStation(PlcStationOpt.StationFlag, args.PackCode, s => PlcStationOpt.StationCode = s)
                    let newStationCodeLog = RecordLogAsync(LogLevel.Information, $"当前工位-{newStationCode}")
                    from productPn in _commonService.GetProductPnFromCatlMes(args.PackCode, PlcStationOpt.StationCode)
                    from cpf in _commonService.CheckProductFlowAndVectorBind(args.PackCode, PlcStationOpt.StationCode, args.VectorCode, PlcStationOpt.StationFlag, productPn)
                    let log = RecordLogAsync(LogLevel.Information, $"载具绑定关系校验通过")
                    from glueTime in _moduleInBoxService.GetGlueDuration(productPn, PlcStationOpt.StationCode, args.PackCode)//校验涂胶是否超时
                    select new { newStationCode.Item2, glueTime };

            var result = await q;
            if (result.IsError)
            {
                var errValue = result.ErrorValue;
                return new DealReqVectorEnterNgWraps { ErrorType = errValue.ErrorType, ErrorCode = errValue.ErrorCode, ErrorMessage = errValue.ErrorMessage }.ToErrResult<DealReqVectorEnterOkWraps, DealReqVectorEnterNgWraps>();
            }

            var okResult = new DealReqVectorEnterOkWraps()
            {
                ShapeLevel = (ushort)result.ResultValue.Item2,
                LeftTime = result.ResultValue.glueTime,
            };
            return okResult.ToOkResult<DealReqVectorEnterOkWraps, DealReqVectorEnterNgWraps>();
        }

        protected override async Task HandleErrAsync(MstMsg pending, DealReqVectorEnterNgWraps err)
        {
            var builder = new MstMsgAckVectorEnterFlagBuilder(pending.MstMsgAckVectorEnterFlag);
            pending.MstMsgAckVectorEnterFlag = builder.AckVectorEnterReq(false).Build();
            pending.EnterErrorCode = (ushort)err.ErrorCode;

            await RecordLogAsync(LogLevel.Error, $"[{PlcStationOpt.StationCode}极柱整形工位]-PLC请求载具进入工位NG：{err.ErrorCode} - {err.ErrorMessage}");
        }

        protected override async Task HandleOkAsync(MstMsg pending, DealReqVectorEnterOkWraps descriptions)
        {
            var builder = new MstMsgAckVectorEnterFlagBuilder(pending.MstMsgAckVectorEnterFlag);
            pending.MstMsgAckVectorEnterFlag = builder.AckVectorEnterReq(true).Build();
            pending.EnterErrorCode = 0;
            pending.EnterReshapeLevel = descriptions.ShapeLevel;
            pending.LeftTime = descriptions.LeftTime;

            await RecordLogAsync(LogLevel.Information, $"[{PlcStationOpt.StationCode}极柱整形工位]-PLC请求载具进入工位OK");
        }

        protected override async Task ResetAckAsync(MstMsg pending)
        {
            var builder = new MstMsgAckVectorEnterFlagBuilder(pending.MstMsgAckVectorEnterFlag);
            pending.MstMsgAckVectorEnterFlag = builder.RestVectorEnterReq().Build();
            pending.EnterReshapeLevel = 0;
            pending.EnterErrorCode = 0;
            pending.LeftTime = 0;
            await RecordLogAsync(LogLevel.Information, $"[{PlcStationOpt.StationCode}极柱整形工位]-处理PLC请求载具进入工位结束");
        }

        protected override FSharpResult<DealReqVectorEnterWraps, string> TryParseArgs(DevMsg incoming)
        {
            var VectorCode = incoming.EnterVectorCode;
            var PackCode = incoming.EnterPackCode.EffectiveContent;

            var result = new DealReqVectorEnterWraps
            {
                VectorCode = VectorCode,
                PackCode = PackCode,
            };

            return result.ToOkResult<DealReqVectorEnterWraps, string>();

        }
    }
}
