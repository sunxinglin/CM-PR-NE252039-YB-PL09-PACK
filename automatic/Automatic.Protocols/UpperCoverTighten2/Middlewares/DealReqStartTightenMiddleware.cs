using Automatic.Protocols.Common;
using Automatic.Protocols.Services;
using Automatic.Protocols.UpperCoverTighten2.Middlewares.Common;
using Automatic.Protocols.UpperCoverTighten2.Models;
using Automatic.Protocols.UpperCoverTighten2.Models.Wraps;
using Automatic.Shared;
using Itminus.FSharpExtensions;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.FSharp.Core;

namespace Automatic.Protocols.UpperCoverTighten2.Middlewares
{
    public class DealReqStartTightenMiddleware :
        HandlePlcRequestMiddlewareBase<DevMsg, MstMsg, DealReqStartTightenWraps, DealReqStartBoltOkWraps, DealReqStartBoltNgWraps>
    {
        private readonly CommonService _commonService;
        private readonly IOptionsMonitor<ApiServerSetting> _ApiServerSetting;
        private readonly bool _isEmptyLoop;

        public DealReqStartTightenMiddleware(PlcFlusher flusher, ILogger<DealReqStartTightenMiddleware> logger, IMediator mediator, CommonService commonService, IOptionsMonitor<ApiServerSetting> apiServerSetting) : base(flusher, logger, mediator)
        {
            _commonService = commonService;
            _ApiServerSetting = apiServerSetting;
            _isEmptyLoop = _ApiServerSetting.CurrentValue.IsEmptyLoop;
        }

        public override string PlcName => PLCNames.上盖拧紧2;

        public override string ProcName => PLCNames.上盖拧紧2;

        public override bool HasAck(MstMsg p) => p.StartTighten.Flag.HasFlag(MstMsgFlag.Ack);

        public override bool HasReq(DevMsg i) => i.StartTighten.Flag.HasFlag(DevMsgFlag.Req);

        public override DevMsg RefIncoming(ScanContext ctx) => ctx.DevMsg;

        public override MstMsg RefPending(ScanContext ctx) => ctx.MstMsg;

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

        protected override async Task<FSharpResult<DealReqStartBoltOkWraps, DealReqStartBoltNgWraps>> HandleArgsAsync(DealReqStartTightenWraps args)
        {
            await RecordLogAsync(LogLevel.Information, $"PLC请求开始工作 [载具编号：{args.VectorCode}]-[产品条码：{args.PackCode}]");


            if (_isEmptyLoop)
            {
                return new DealReqStartBoltOkWraps().ToOkResult<DealReqStartBoltOkWraps, DealReqStartBoltNgWraps>();
            }

            var q = from productPn in _commonService.GetProductPnFromCatlMes(args.PackCode, PlcStationOpt.StationCode)
                    from checkVector in _commonService.CheckProductFlowAndVectorBind(args.PackCode, PlcStationOpt.StationCode, args.VectorCode, PlcStationOpt.StationCode, productPn)
                    let checkVectorLog = RecordLogAsync(LogLevel.Information, $"载具绑定关系校验通过")
                    from start in _commonService.MakePackStart(args.PackCode, PlcStationOpt.StationCode)
                    from cmr in _commonService.EnsureExiestOrCreateMessionRecord(args.PackCode, PlcStationOpt.StationCode, ProcName, args.VectorCode, productPn)
                    let cmrResponseLog = RecordLogAsync(LogLevel.Information, $"创建生产追溯信息成功")
                    select checkVector;

            var r = await q;
            if (r.IsError)
            {
                var errValue = r.ErrorValue;
                return new DealReqStartBoltNgWraps() { ErrorCode = errValue.ErrorCode, ErrorType = errValue.ErrorType, ErrorMessage = errValue.ErrorMessage }.ToErrResult<DealReqStartBoltOkWraps, DealReqStartBoltNgWraps>();
            }

            return new DealReqStartBoltOkWraps().ToOkResult<DealReqStartBoltOkWraps, DealReqStartBoltNgWraps>();
        }

        protected override async Task HandleErrAsync(MstMsg pending, DealReqStartBoltNgWraps err)
        {
            var builder = new MstMsgFlagBuilder(pending.StartTighten.Flag);
            pending.StartTighten.Flag = builder.Ack(false).Build();
            pending.StartTighten.ErrorCode = (ushort)err.ErrorCode;

            await RecordLogAsync(LogLevel.Error, $"PLC请求开始工作NG：{err.ErrorCode} - {err.ErrorMessage}");

        }

        protected override async Task HandleOkAsync(MstMsg pending, DealReqStartBoltOkWraps descriptions)
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

    }
}
