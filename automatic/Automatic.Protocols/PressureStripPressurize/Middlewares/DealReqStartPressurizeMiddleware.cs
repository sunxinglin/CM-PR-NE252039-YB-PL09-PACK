using Automatic.Protocols.Common;
using Automatic.Protocols.PressureStripPressurize.Middlewares.Common;
using Automatic.Protocols.PressureStripPressurize.Models;
using Automatic.Protocols.PressureStripPressurize.Models.Wraps;
using Automatic.Protocols.Services;
using Automatic.Shared;
using Itminus.FSharpExtensions;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.FSharp.Core;

namespace Automatic.Protocols.PressureStripPressurize.Middlewares
{
    public class DealReqStartPressurizeMiddleware :
        HandlePlcRequestMiddlewareBase<DevMsg, MstMsg, DealReqStartPressurizeWraps, DealReqStartPressurizeOkWraps, DealReqStartPressurizeNgWraps>
    {
        private readonly CommonService _commonService;
        private readonly AutoPressureService _pressureService;
        private readonly IOptionsMonitor<ApiServerSetting> _ApiServerSetting;
        private readonly bool _isEmptyLoop;

        public DealReqStartPressurizeMiddleware(PressureStripFlusher flusher, ILogger<DealReqStartPressurizeMiddleware> logger,
            IMediator mediator, CommonService commonService, AutoPressureService pressureService, IOptionsMonitor<ApiServerSetting> apiServerSetting) : base(flusher, logger, mediator)
        {
            _commonService = commonService;
            _pressureService = pressureService;
            _ApiServerSetting = apiServerSetting;
            _isEmptyLoop = _ApiServerSetting.CurrentValue.IsEmptyLoop;
        }

        public override string PlcName => PLCNames.压条加压;

        public override string ProcName => PLCNames.压条加压;

        public override bool HasAck(MstMsg p) => p.StartPressurize.Flag.HasFlag(MstMsgFlag.Ack);

        public override bool HasReq(DevMsg i) => i.StartPressurize.Flag.HasFlag(DevMsgFlag.Req);

        public override DevMsg RefIncoming(ScanContext ctx) => ctx.DevMsg;

        public override MstMsg RefPending(ScanContext ctx) => ctx.MstMsg;

        protected override FSharpResult<DealReqStartPressurizeWraps, string> TryParseArgs(DevMsg incoming)
        {
            var VectorCode = incoming.StartPressurize.VectorCode;
            var PackCode = incoming.StartPressurize.PackCode.EffectiveContent;
            var wraps = new DealReqStartPressurizeWraps
            {
                VectorCode = VectorCode,
                PackCode = PackCode
            };
            return wraps.ToOkResult<DealReqStartPressurizeWraps, string>();
        }

        protected override async Task<FSharpResult<DealReqStartPressurizeOkWraps, DealReqStartPressurizeNgWraps>> HandleArgsAsync(DealReqStartPressurizeWraps args)
        {
            await RecordLogAsync(LogLevel.Information, $"收到PLC请求开始加压---[载具编号：{args.VectorCode}]-[产品条码：{args.PackCode}]");
            if (_isEmptyLoop)
            {
                return new DealReqStartPressurizeOkWraps().ToOkResult<DealReqStartPressurizeOkWraps, DealReqStartPressurizeNgWraps>();
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
                return new DealReqStartPressurizeNgWraps() { ErrorCode = errValue.ErrorCode, ErrorType = errValue.ErrorType, ErrorMessage = errValue.ErrorMessage }.ToErrResult<DealReqStartPressurizeOkWraps, DealReqStartPressurizeNgWraps>();
            }
            return new DealReqStartPressurizeOkWraps() { }.ToOkResult<DealReqStartPressurizeOkWraps, DealReqStartPressurizeNgWraps>();
        }

        protected override async Task HandleErrAsync(MstMsg pending, DealReqStartPressurizeNgWraps err)
        {
            pending.StartPressurize.Flag = new MstMsgFlagBuilder(pending.StartPressurize.Flag)
                .Ack(false)
                .Build();
            pending.StartPressurize.ErrorCode = (ushort)err.ErrorCode;

            await RecordLogAsync(LogLevel.Error, $"PLC请求开始加压NG：{err.ErrorCode} - {err.ErrorMessage}");
        }

        protected override async Task HandleOkAsync(MstMsg pending, DealReqStartPressurizeOkWraps descriptions)
        {
            pending.StartPressurize.Flag = new MstMsgFlagBuilder(pending.StartPressurize.Flag)
                .Ack(true)
                .Build();
            pending.StartPressurize.ErrorCode = 0;

            await RecordLogAsync(LogLevel.Information, $"PLC请求开始加压OK");
        }

        protected override async Task ResetAckAsync(MstMsg pending)
        {
            pending.StartPressurize.Flag = new MstMsgFlagBuilder(pending.StartPressurize.Flag)
                   .Reset()
                   .Build();
            await RecordLogAsync(LogLevel.Information, $"处理PLC请求开始加压结束");
        }

    }
}
