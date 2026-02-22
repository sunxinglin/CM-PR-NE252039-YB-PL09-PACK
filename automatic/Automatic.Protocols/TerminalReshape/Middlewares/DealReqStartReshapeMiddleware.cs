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
    public class DealReqStartReshapeMiddleware
        : HandlePlcRequestMiddlewareBase<DevMsg, MstMsg, DealReqStartReshapeWraps, DealReqStartGlueOkWraps, DealReqStartGlueNgWraps>
    {
        private readonly CommonService _commonService;
        private readonly IOptionsMonitor<ApiServerSetting> _ApiServerSetting;
        private readonly bool _isEmptyLoop;
        public DealReqStartReshapeMiddleware(TerminalReshapeFlusher flusher, ILogger<DealReqStartReshapeMiddleware> logger, IMediator mediator, CommonService commonService, IOptionsMonitor<ApiServerSetting> apiServerSetting) : base(flusher, logger, mediator)
        {
            _commonService = commonService;
            _ApiServerSetting = apiServerSetting;
            _isEmptyLoop = _ApiServerSetting.CurrentValue.IsEmptyLoop;
        }

        public override string PlcName => PLCNames.极柱整形;

        public override string ProcName => PLCNames.极柱整形;

        public override bool HasAck(MstMsg p) => p.MstMsgAckStartReshapeFlag.HasFlag(MstMsgAckStartReshapeFlag.AckStartReshape);

        public override bool HasReq(DevMsg i) => i.DevMsgReqStartReshapeFlag.HasFlag(DevMsgReqStartReshapeFlag.ReqStartReshape);

        public override DevMsg RefIncoming(ScanContext ctx) => ctx.DevMsg;

        public override MstMsg RefPending(ScanContext ctx) => ctx.MstMsg;


        protected override async Task<FSharpResult<DealReqStartGlueOkWraps, DealReqStartGlueNgWraps>> HandleArgsAsync(DealReqStartReshapeWraps args)
        {
            await RecordLogAsync(LogLevel.Information, $"收到PLC请求开始整形---[载具编号：{args.VectorCode}]-[产品条码：{args.PackCode}]");

            if (_isEmptyLoop)
            {
                return new DealReqStartGlueOkWraps().ToOkResult<DealReqStartGlueOkWraps, DealReqStartGlueNgWraps>();
            }

            var q = from newStationCode in _commonService.GetCurrentStation(PlcStationOpt.StationFlag, args.PackCode, s => PlcStationOpt.StationCode = s)
                    let newStationCodeLog = RecordLogAsync(LogLevel.Information, $"当前工位-{newStationCode}")
                    from productPn in _commonService.GetProductPnFromCatlMes(args.PackCode, PlcStationOpt.StationCode)
                    from checkVector in _commonService.CheckProductFlowAndVectorBind(args.PackCode, PlcStationOpt.StationCode, args.VectorCode, PlcStationOpt.StationFlag, productPn)
                    let checkVectorLog = RecordLogAsync(LogLevel.Information, $"载具绑定关系校验通过")
                    from start in _commonService.MakePackStart(args.PackCode, PlcStationOpt.StationCode)
                    from cmr in _commonService.EnsureExiestOrCreateMessionRecord(args.PackCode, PlcStationOpt.StationCode, "压条加压", args.VectorCode, productPn)
                    let cmrResponseLog = RecordLogAsync(LogLevel.Information, $"创建生产追溯信息成功")
                    select new { newStationCode.Item2 };

            var r = await q;
            if (r.IsError)
            {
                var errValue = r.ErrorValue;
                return new DealReqStartGlueNgWraps() { ErrorCode = errValue.ErrorCode, ErrorType = errValue.ErrorType, ErrorMessage = errValue.ErrorMessage }.ToErrResult<DealReqStartGlueOkWraps, DealReqStartGlueNgWraps>();
            }

            var okResult = new DealReqStartGlueOkWraps()
            {
                ShapeLevel = (ushort)r.ResultValue.Item2
            };
            return okResult.ToOkResult<DealReqStartGlueOkWraps, DealReqStartGlueNgWraps>();
        }

        protected override async Task HandleErrAsync(MstMsg pending, DealReqStartGlueNgWraps err)
        {
            var builder = new MstMsgAckStartReshapeFlagBuilder(pending.MstMsgAckStartReshapeFlag);
            pending.MstMsgAckStartReshapeFlag = builder.AckStartReshape(false).Build();
            pending.StartErrorCode = (ushort)err.ErrorCode;

            await RecordLogAsync(LogLevel.Error, $"PLC请求开始整形NG：{err.ErrorCode} - {err.ErrorMessage}");
        }

        protected override async Task HandleOkAsync(MstMsg pending, DealReqStartGlueOkWraps descriptions)
        {
            var builder = new MstMsgAckStartReshapeFlagBuilder(pending.MstMsgAckStartReshapeFlag);
            pending.MstMsgAckStartReshapeFlag = builder.AckStartReshape(true).Build();
            pending.StartErrorCode = 0;
            pending.StartReshapeLevel = descriptions.ShapeLevel;

            await RecordLogAsync(LogLevel.Information, $"PLC请求开始整形OK");
        }

        protected override async Task ResetAckAsync(MstMsg pending)
        {
            var builder = new MstMsgAckStartReshapeFlagBuilder(pending.MstMsgAckStartReshapeFlag);
            pending.MstMsgAckStartReshapeFlag = builder.ResetStartReshape().Build();
            pending.StartErrorCode = 0;
            pending.StartReshapeLevel = 0;
            await RecordLogAsync(LogLevel.Information, $"处理PLC请求开始整形结束");
        }

        protected override FSharpResult<DealReqStartReshapeWraps, string> TryParseArgs(DevMsg incoming)
        {
            var VectorCode = incoming.StartVectorCode;
            var PackCode = incoming.StartPackCode.EffectiveContent;
            var result = new DealReqStartReshapeWraps
            {
                VectorCode = VectorCode,
                PackCode = PackCode,
            };

            return result.ToOkResult<DealReqStartReshapeWraps, string>();
        }
    }
}
