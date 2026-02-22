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
    public class DealReqStartInBoxMiddleware
        : HandlePlcRequestMiddlewareBase<DevMsg, MstMsg, DealReqStartInBoxWraps, DealReqStartInBoxOkWraps, DealReqStartInBoxNgWraps>
    {
        private readonly CommonService _commonService;
        private readonly ModuleInBoxService _moduleInBoxService;
        private readonly IOptionsMonitor<ApiServerSetting> _ApiServerSetting;
        private readonly bool _isEmptyLoop;

        public DealReqStartInBoxMiddleware(ModuleInBoxFlusher flusher, ModuleInBoxService moduleInBoxService, ILogger<DealReqStartInBoxMiddleware> logger, IMediator mediator, CommonService commonService, IOptionsMonitor<ApiServerSetting> apiServerSetting) : base(flusher, logger, mediator)
        {
            _commonService = commonService;
            _moduleInBoxService = moduleInBoxService;
            _ApiServerSetting = apiServerSetting;
            _isEmptyLoop = _ApiServerSetting.CurrentValue.IsEmptyLoop;
        }

        public override string PlcName => PLCNames.模组入箱;

        public override string ProcName => PLCNames.模组入箱;

        public override bool HasAck(MstMsg p) => p.StartInBox.Flag.HasFlag(MstMsgFlag.Ack);

        public override bool HasReq(DevMsg i) => i.StartInBox.Flag.HasFlag(DevMsgFlag.Req);

        public override DevMsg RefIncoming(ScanContext ctx) => ctx.DevMsg;

        public override MstMsg RefPending(ScanContext ctx) => ctx.MstMsg;

        protected override FSharpResult<DealReqStartInBoxWraps, string> TryParseArgs(DevMsg incoming)
        {
            var vectorCode = incoming.StartInBox.VectorCode;
            var packCode = incoming.StartInBox.PackCode.EffectiveContent;
            var res = new DealReqStartInBoxWraps
            {
                VectorCode = vectorCode,
                PackCode = packCode,
            };
            return res.ToOkResult<DealReqStartInBoxWraps, string>();
        }

        protected override async Task<FSharpResult<DealReqStartInBoxOkWraps, DealReqStartInBoxNgWraps>> HandleArgsAsync(DealReqStartInBoxWraps args)
        {
            await RecordLogAsync(LogLevel.Information, $"收到PLC请求开始入箱 - 载具号：{args.VectorCode} - 产品条码：{args.PackCode}");

            if (_isEmptyLoop)
            {
                return new DealReqStartInBoxOkWraps().ToOkResult<DealReqStartInBoxOkWraps, DealReqStartInBoxNgWraps>();

            }

            var q = from productPn in _commonService.GetProductPnFromCatlMes(args.PackCode, PlcStationOpt.StationCode)//获取产品PN
                                                                                                                      //from productMAT in _commonService.GetPackMAT(productPn)
                                                                                                                      //let boxMAtRecord = RecordLogAsync(LogLevel.Information, $"当前箱体料号：{productMAT}")
                    from checkVector in _commonService.CheckProductFlowAndVectorBind(args.PackCode, PlcStationOpt.StationCode, args.VectorCode, PlcStationOpt.StationCode, productPn)//校验载具绑定关系
                    let checkVectorLog = RecordLogAsync(LogLevel.Information, $"载具绑定关系校验通过")
                    from start in _commonService.MakePackStart(args.PackCode, PlcStationOpt.StationCode)
                    from cmr in _commonService.EnsureExiestOrCreateMessionRecord(args.PackCode, PlcStationOpt.StationCode, ProcName, args.VectorCode, productPn)//创建主记录
                    let cmrResponseLog = RecordLogAsync(LogLevel.Information, $"创建生产追溯信息成功")

                    select new { productPn };
            var r = await q;
            if (r.IsError)
            {
                var errValue = r.ErrorValue;
                return new DealReqStartInBoxNgWraps { ErrorCode = errValue.ErrorCode, ErrorMessage = errValue.ErrorMessage, ErrorType = errValue.ErrorType }.ToErrResult<DealReqStartInBoxOkWraps, DealReqStartInBoxNgWraps>();
            }
            var result = r.ResultValue;
            return new DealReqStartInBoxOkWraps() { }.ToOkResult<DealReqStartInBoxOkWraps, DealReqStartInBoxNgWraps>();
        }

        protected override async Task HandleErrAsync(MstMsg pending, DealReqStartInBoxNgWraps err)
        {
            var Builder = new MstMsgFlagBuilder(pending.StartInBox.Flag);
            pending.StartInBox.Flag = Builder.Ack(false).Build();
            pending.StartInBox.ErrorCode = (ushort)err.ErrorCode;

            await RecordLogAsync(LogLevel.Error, $"PLC请求开始入箱NG：{err.ErrorCode} - {err.ErrorMessage}");
        }

        protected override async Task HandleOkAsync(MstMsg pending, DealReqStartInBoxOkWraps descriptions)
        {
            var Builder = new MstMsgFlagBuilder(pending.StartInBox.Flag);
            pending.StartInBox.Flag = Builder.Ack(true).Build();
            pending.StartInBox.ErrorCode = 0;

            await RecordLogAsync(LogLevel.Information, $"PLC请求开始入箱OK");
        }

        protected override async Task ResetAckAsync(MstMsg pending)
        {
            var Builder = new MstMsgFlagBuilder(pending.StartInBox.Flag);
            pending.StartInBox.Flag = Builder.Reset().Build();

            await RecordLogAsync(LogLevel.Information, $"处理PLC请求开始入箱结束");
        }

    }
}
