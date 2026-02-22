using Automatic.Protocols.Common;
using Automatic.Protocols.ModuleInBox.Middlewares.Common;
using Automatic.Protocols.ModuleInBox.Models;
using Automatic.Protocols.ModuleInBox.Models.Wraps;
using Automatic.Protocols.Services;
using Automatic.Shared;
using Itminus.FSharpExtensions;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.FSharp.Core;

namespace Automatic.Protocols.ModuleInBox.Middlewares
{
    public class DealReqEnterStationMiddleware
        : HandlePlcRequestMiddlewareBase<DevMsg, MstMsg, DealReqEnterStationWraps, DealReqEnterStationOkWraps, DealReqEnterStationNgWraps>
    {
        private readonly CommonService _commonService;
        private readonly IOptionsMonitor<ApiServerSetting> _ApiServerSetting;
        private readonly ModuleInBoxService _moduleInBoxService;
        private readonly bool _isEmptyLoop;

        public DealReqEnterStationMiddleware(ModuleInBoxFlusher flusher, ILogger<DealReqEnterStationMiddleware> logger, IMediator mediator, CommonService commonService, IOptionsMonitor<ApiServerSetting> apiServerSetting,
            ModuleInBoxService moduleInBoxService) : base(flusher, logger, mediator)
        {
            _commonService = commonService;
            _ApiServerSetting = apiServerSetting;
            _moduleInBoxService = moduleInBoxService;
            _isEmptyLoop = _ApiServerSetting.CurrentValue.IsEmptyLoop;
        }

        public override string PlcName => PLCNames.模组入箱;

        public override string ProcName => PLCNames.模组入箱;

        public override bool HasAck(MstMsg p) => p.EnterStation.Flag.HasFlag(MstMsgFlag.Ack);

        public override bool HasReq(DevMsg i) => i.EnterStation.Flag.HasFlag(DevMsgFlag.Req);

        public override DevMsg RefIncoming(ScanContext ctx) => ctx.DevMsg;

        public override MstMsg RefPending(ScanContext ctx) => ctx.MstMsg;

        protected override FSharpResult<DealReqEnterStationWraps, string> TryParseArgs(DevMsg incoming)
        {
            var VectorCode = incoming.EnterStation.VectorCode;
            var PackCode = incoming.EnterStation.PackCode.EffectiveContent;

            var res = new DealReqEnterStationWraps
            {
                VectorCode = VectorCode,
                PackCode = PackCode,
            };

            return res.ToOkResult<DealReqEnterStationWraps, string>();
        }

        protected override async Task<FSharpResult<DealReqEnterStationOkWraps, DealReqEnterStationNgWraps>> HandleArgsAsync(DealReqEnterStationWraps args)
        {
            await RecordLogAsync(LogLevel.Information, $"收到PLC请求进入工位 - 载具号：{args.VectorCode} - 产品条码：{args.PackCode}");

            if (_isEmptyLoop)
            {
                return new DealReqEnterStationOkWraps().ToOkResult<DealReqEnterStationOkWraps, DealReqEnterStationNgWraps>();
            }

            var q = from productPn in _commonService.GetProductPnFromCatlMes(args.PackCode, PlcStationOpt.StationCode)
                    from cpf in _commonService.CheckProductFlowAndVectorBind(args.PackCode, PlcStationOpt.StationCode, args.VectorCode, PlcStationOpt.StationCode, productPn)
                    let log = RecordLogAsync(LogLevel.Information, $"载具绑定关系校验通过")
                    from glueTime in _moduleInBoxService.GetGlueDuration(productPn, PlcStationOpt.StationCode, args.PackCode)//校验涂胶是否超时
                    select new { glueTime };
            var result = await q;
            if (result.IsError)
            {
                var errValue = result.ErrorValue;
                await RecordLogAsync(LogLevel.Information, $"载具绑定关系校验未通过:{errValue.ErrorMessage}");
                return new DealReqEnterStationNgWraps { ErrorType = errValue.ErrorType, ErrorCode = errValue.ErrorCode, ErrorMessage = errValue.ErrorMessage }.ToErrResult<DealReqEnterStationOkWraps, DealReqEnterStationNgWraps>();
            }
            return new DealReqEnterStationOkWraps() { GlueRemainDuration = result.ResultValue.glueTime }.ToOkResult<DealReqEnterStationOkWraps, DealReqEnterStationNgWraps>();
        }

        protected override async Task HandleErrAsync(MstMsg pending, DealReqEnterStationNgWraps err)
        {
            var Builder = new MstMsgFlagBuilder(pending.EnterStation.Flag);
            pending.EnterStation.Flag = Builder.Ack(false).Build();
            pending.EnterStation.ErrorCode = (ushort)err.ErrorCode;
            await RecordLogAsync(LogLevel.Error, $"PLC请求进入工位NG：{err.ErrorCode} - {err.ErrorMessage}");
        }

        protected override async Task HandleOkAsync(MstMsg pending, DealReqEnterStationOkWraps descriptions)
        {
            var Builder = new MstMsgFlagBuilder(pending.EnterStation.Flag);
            pending.EnterStation.Flag = Builder.Ack(true).Build();
            pending.EnterStation.ErrorCode = 0;

            pending.EnterStation.GlueRemainDuration = descriptions.GlueRemainDuration;
            await RecordLogAsync(LogLevel.Information, $"PLC请求进入工位OK，涂胶剩余时长：{descriptions.GlueRemainDuration}");
        }

        protected override async Task ResetAckAsync(MstMsg pending)
        {
            var Builder = new MstMsgFlagBuilder(pending.EnterStation.Flag);
            pending.EnterStation.Flag = Builder.Reset().Build();

            await RecordLogAsync(LogLevel.Information, "处理PLC请求进入工位结束");
        }

    }
}
