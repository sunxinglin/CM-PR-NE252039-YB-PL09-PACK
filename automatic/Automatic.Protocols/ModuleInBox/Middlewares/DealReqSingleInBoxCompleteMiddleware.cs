using Automatic.Entity.DataDtos;
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
    public class DealReqSingleInBoxCompleteMiddleware
        : HandlePlcRequestMiddlewareBase<DevMsg, MstMsg, DealReqSingleInBoxCompleteWraps, DealReqSingleInBoxCompleteOkWraps, DealReqSingleInBoxCompleteNgWraps>

    {
        private readonly ModuleInBoxService _moduleInBoxService;
        private readonly IOptionsMonitor<ApiServerSetting> _ApiServerSetting;
        private readonly bool _isEmptyLoop;

        public DealReqSingleInBoxCompleteMiddleware(ModuleInBoxFlusher flusher, ILogger<DealReqSingleInBoxCompleteMiddleware> logger, IMediator mediator,
            ModuleInBoxService moduleInBoxService, IOptionsMonitor<ApiServerSetting> apiServerSetting) : base(flusher, logger, mediator)
        {
            _moduleInBoxService = moduleInBoxService;
            _ApiServerSetting = apiServerSetting;
            _isEmptyLoop = _ApiServerSetting.CurrentValue.IsEmptyLoop;
        }

        public override string PlcName => PLCNames.模组入箱;

        public override string ProcName => PLCNames.模组入箱;

        public override bool HasAck(MstMsg p) => p.SingleInBoxComplete.Flag.HasFlag(MstMsgFlag.Ack);

        public override bool HasReq(DevMsg i) => i.SingleInBoxComplete.Flag.HasFlag(DevMsgFlag.Req);

        public override DevMsg RefIncoming(ScanContext ctx) => ctx.DevMsg;

        public override MstMsg RefPending(ScanContext ctx) => ctx.MstMsg;

        protected override FSharpResult<DealReqSingleInBoxCompleteWraps, string> TryParseArgs(DevMsg incoming)
        {
            var vectorCode = incoming.SingleInBoxComplete.VectorCode;
            var packCode = incoming.SingleInBoxComplete.PackCode.EffectiveContent;
            var moduleCode = incoming.SingleInBoxComplete.ModuleCode.EffectiveContent;
            var moduleLocation = incoming.SingleInBoxComplete.ModuleLocation;

            var res = new DealReqSingleInBoxCompleteWraps
            {
                VectorCode = vectorCode,
                PackCode = packCode,
                ModuleCode = moduleCode,
                ModuleLocation = moduleLocation
            };

            return res.ToOkResult<DealReqSingleInBoxCompleteWraps, string>();
        }

        protected override async Task<FSharpResult<DealReqSingleInBoxCompleteOkWraps, DealReqSingleInBoxCompleteNgWraps>> HandleArgsAsync(DealReqSingleInBoxCompleteWraps args)
        {
            await RecordLogAsync(LogLevel.Information, $"收到PLC单个入箱完成请求 - 载具号：{args.VectorCode} - 产品条码：{args.PackCode} - 模组条码：{args.ModuleCode} - 入箱位置：{args.ModuleLocation}");
            if (_isEmptyLoop)
            {
                return new DealReqSingleInBoxCompleteOkWraps().ToOkResult<DealReqSingleInBoxCompleteOkWraps, DealReqSingleInBoxCompleteNgWraps>();
            }

            var data = new ModuleInBoxSingleModuleUploadDto()
            {
                StepCode = PlcStationOpt.StepCode,
                StationCode = PlcStationOpt.StationCode,
                VectorCode = args.VectorCode,
                PackCode = args.PackCode,
                ModuleCode = args.ModuleCode,
                ModuleLocation = args.ModuleLocation
            };

            var q = from uploadResult in _moduleInBoxService.UploadSingleModule(data)
                    select uploadResult;

            var r = await q;
            if (r.IsError)
            {
                var errValue = r.ErrorValue;
                return new DealReqSingleInBoxCompleteNgWraps() { ErrorCode = errValue.ErrorCode, ErrorType = errValue.ErrorType, ErrorMessage = errValue.ErrorMessage }.ToErrResult<DealReqSingleInBoxCompleteOkWraps, DealReqSingleInBoxCompleteNgWraps>();
            }

            return new DealReqSingleInBoxCompleteOkWraps().ToOkResult<DealReqSingleInBoxCompleteOkWraps, DealReqSingleInBoxCompleteNgWraps>();
        }

        protected override async Task HandleErrAsync(MstMsg pending, DealReqSingleInBoxCompleteNgWraps err)
        {
            var Builder = new MstMsgFlagBuilder(pending.SingleInBoxComplete.Flag);
            pending.SingleInBoxComplete.Flag = Builder.Ack(false).Build();
            pending.SingleInBoxComplete.ErrorCode = (ushort)err.ErrorCode;

            await RecordLogAsync(LogLevel.Error, $"PLC请求单个入箱完成NG：{err.ErrorCode} - {err.ErrorMessage}");
        }

        protected override async Task HandleOkAsync(MstMsg pending, DealReqSingleInBoxCompleteOkWraps descriptions)
        {
            var Builder = new MstMsgFlagBuilder(pending.SingleInBoxComplete.Flag);
            pending.SingleInBoxComplete.Flag = Builder.Ack(true).Build();
            pending.SingleInBoxComplete.ErrorCode = 0;

            await RecordLogAsync(LogLevel.Information, "PLC请求单个入箱完成OK");
        }

        protected override async Task ResetAckAsync(MstMsg pending)
        {
            var Builder = new MstMsgFlagBuilder(pending.SingleInBoxComplete.Flag);
            pending.SingleInBoxComplete.Flag = Builder.Reset().Build();

            await RecordLogAsync(LogLevel.Information, "处理PLC请求单个入箱完成结束");
        }

    }
}
