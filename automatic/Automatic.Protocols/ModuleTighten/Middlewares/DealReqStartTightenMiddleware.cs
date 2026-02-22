using Automatic.Protocols.Common;
using Automatic.Protocols.ModuleTighten.Middlewares.Common;
using Automatic.Protocols.ModuleTighten.Models;
using Automatic.Protocols.ModuleTighten.Models.Wraps;
using Automatic.Protocols.Services;
using Automatic.Shared;
using Itminus.FSharpExtensions;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.FSharp.Core;

namespace Automatic.Protocols.ModuleTighten.Middlewares
{
    public class DealReqStartTightenMiddleware :
        HandlePlcRequestMiddlewareBase<DevMsg, MstMsg, DealReqStartTightenWraps, DealReqStartTightenOkWraps, DealReqStartTightenNgWraps>
    {
        private readonly CommonService _commonService;
        private readonly IOptionsMonitor<ApiServerSetting> _ApiServerSetting;
        private readonly bool _isEmptyLoop;
        public DealReqStartTightenMiddleware(ModuleTightenFlusher flusher, ILogger<DealReqStartTightenMiddleware> logger, IMediator mediator, CommonService commonService, IOptionsMonitor<ApiServerSetting> apiServerSetting) : base(flusher, logger, mediator)
        {
            _commonService = commonService;
            _ApiServerSetting = apiServerSetting;
            _isEmptyLoop = _ApiServerSetting.CurrentValue.IsEmptyLoop;
        }

        public override string PlcName => PLCNames.模组拧紧;

        public override string ProcName => PLCNames.模组拧紧;

        public override bool HasAck(MstMsg p) => p.StartTighten.Flag.HasFlag(MstMsgFlag.Ack);

        public override bool HasReq(DevMsg i) => i.StartTighten.Flag.HasFlag(DevMsgFlag.Req);

        public override DevMsg RefIncoming(ScanContext ctx) => ctx.DevMsg;

        public override MstMsg RefPending(ScanContext ctx) => ctx.MstMsg;

        protected override async Task<FSharpResult<DealReqStartTightenOkWraps, DealReqStartTightenNgWraps>> HandleArgsAsync(DealReqStartTightenWraps args)
        {
            await RecordLogAsync(LogLevel.Information, $"PLC请求开始工作 [载具编号：{args.VectorCode}]-[产品条码：{args.PackCode}]");

            if (_isEmptyLoop)
            {
                return new DealReqStartTightenOkWraps().ToOkResult<DealReqStartTightenOkWraps, DealReqStartTightenNgWraps>();

            }

            var q = from productPn in _commonService.GetProductPnFromCatlMes(args.PackCode, PlcStationOpt.StationCode)
                    from checkVector in _commonService.CheckProductFlowAndVectorBind(args.PackCode, PlcStationOpt.StationCode, args.VectorCode, PlcStationOpt.StationCode, productPn)
                    let checkVectorLog = RecordLogAsync(LogLevel.Information, $"载具绑定关系校验通过")
                    from cmr in _commonService.EnsureExiestOrCreateMessionRecord(args.PackCode, PlcStationOpt.StationCode, "上盖拧紧", args.VectorCode, productPn)
                    let cmrResponseLog = RecordLogAsync(LogLevel.Information, $"创建生产追溯信息成功")
                    select new { cmr };

            var r = await q;
            if (r.IsError)
            {
                var errValue = r.ErrorValue;
                return new DealReqStartTightenNgWraps() { ErrorCode = errValue.ErrorCode, ErrorType = errValue.ErrorType, ErrorMessage = errValue.ErrorMessage }.ToErrResult<DealReqStartTightenOkWraps, DealReqStartTightenNgWraps>();
            }

            return new DealReqStartTightenOkWraps() { }.ToOkResult<DealReqStartTightenOkWraps, DealReqStartTightenNgWraps>();
        }

        protected override async Task HandleErrAsync(MstMsg pending, DealReqStartTightenNgWraps err)
        {
            var builder = new MstMsgFlagBuilder(pending.StartTighten.Flag);
            pending.StartTighten.Flag = builder.Ack(false).Build();
            pending.StartTighten.ErrorCode = (ushort)err.ErrorCode;

            await RecordLogAsync(LogLevel.Error, $"PLC请求开始工作NG：{err.ErrorCode} - {err.ErrorMessage}");

        }

        protected override async Task HandleOkAsync(MstMsg pending, DealReqStartTightenOkWraps descriptions)
        {
            var builder = new MstMsgFlagBuilder(pending.StartTighten.Flag);
            pending.StartTighten.Flag = builder.Ack(true).Build();
            pending.StartTighten.ErrorCode = 0;

            await RecordLogAsync(LogLevel.Information, $"PLC请求开始工作OK");

        }

        protected override async Task ResetAckAsync(MstMsg pending)
        {
            var builder = new MstMsgFlagBuilder(pending.StartTighten.Flag);
            pending.StartTighten.Flag = builder.Reset().Build();

            await RecordLogAsync(LogLevel.Information, $"处理PLC请求开始工作结束");
        }

        protected override FSharpResult<DealReqStartTightenWraps, string> TryParseArgs(DevMsg incoming)
        {
            var vectorCode = incoming.StartTighten.VectorCode;
            var packCode = incoming.StartTighten.PackCode.EffectiveContent;

            var result = new DealReqStartTightenWraps
            {
                VectorCode = vectorCode,
                PackCode = packCode,
            };

            return result.ToOkResult<DealReqStartTightenWraps, string>();
        }
    }
}
