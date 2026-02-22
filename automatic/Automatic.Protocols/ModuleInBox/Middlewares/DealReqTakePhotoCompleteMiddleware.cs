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
    public class DealReqTakePhotoCompleteMiddleware
        : HandlePlcRequestMiddlewareBase<DevMsg, MstMsg, DealReqTakePhotoCompleteWraps, DealReqTakePhotoCompleteOkWraps, DealReqTakePhotoCompleteNgWraps>
    {
        private readonly ModuleInBoxService _moduleInBoxService;
        private readonly CommonService _commonService;
        private readonly IOptionsMonitor<ApiServerSetting> _ApiServerSetting;
        private readonly bool _isEmptyLoop;

        public DealReqTakePhotoCompleteMiddleware(
            ModuleInBoxFlusher flusher, ILogger<DealReqTakePhotoCompleteMiddleware> logger,
            IMediator mediator,
            ModuleInBoxService moduleInBoxService, IOptionsMonitor<ApiServerSetting> apiServerSetting, CommonService commonService
            ) : base(flusher, logger, mediator)
        {
            _moduleInBoxService = moduleInBoxService;
            _ApiServerSetting = apiServerSetting;
            _commonService = commonService;
            _isEmptyLoop = _ApiServerSetting.CurrentValue.IsEmptyLoop;
        }

        public override string PlcName => PLCNames.模组入箱;

        public override string ProcName => PLCNames.模组入箱;

        public override bool HasAck(MstMsg p) => p.TakePhotoComplete.Flag.HasFlag(MstMsgFlag.Ack);

        public override bool HasReq(DevMsg i) => i.TakePhotoComplete.Flag.HasFlag(DevMsgFlag.Req);

        public override DevMsg RefIncoming(ScanContext ctx) => ctx.DevMsg;

        public override MstMsg RefPending(ScanContext ctx) => ctx.MstMsg;

        protected override FSharpResult<DealReqTakePhotoCompleteWraps, string> TryParseArgs(DevMsg incoming)
        {
            var cellCode = incoming.TakePhotoComplete.CellCode.EffectiveContent;
            var moduleLocation = incoming.TakePhotoComplete.Location;

            var res = new DealReqTakePhotoCompleteWraps
            {
                CellCode = cellCode,
                ModuleLocation = moduleLocation
            };
            return res.ToOkResult<DealReqTakePhotoCompleteWraps, string>();
        }

        protected override async Task<FSharpResult<DealReqTakePhotoCompleteOkWraps, DealReqTakePhotoCompleteNgWraps>> HandleArgsAsync(DealReqTakePhotoCompleteWraps args)
        {
            await RecordLogAsync(LogLevel.Information, $"收到PLC请求拍电芯码完成-电芯码：{args.CellCode}-模组入位置：{args.ModuleLocation}");

            if (_isEmptyLoop)
            {
                return new DealReqTakePhotoCompleteOkWraps().ToOkResult<DealReqTakePhotoCompleteOkWraps, DealReqTakePhotoCompleteNgWraps>();
            }

            var q = from moduleInfoResult in _moduleInBoxService.GetModuleCodeFromCATL(args.CellCode, PlcStationOpt.StationCode)
                    let moduleInfoLog = RecordLogAsync(LogLevel.Information, $"获取到的模组PN：{moduleInfoResult.BarCode_GoodsPN}-模组条码:{moduleInfoResult.BarCode}")
                    from checkModuleResult in _moduleInBoxService.CheckModuleInfo(moduleInfoResult.BarCode, moduleInfoResult.BarCode_GoodsPN, args.ModuleLocation, PlcStationOpt.StationCode)
                    let checkModuleLog = RecordLogAsync(LogLevel.Information, $" 模组校验通过，模组条码:{moduleInfoResult.BarCode}-模组PN:{moduleInfoResult.BarCode_GoodsPN}-模组入位置:{args.ModuleLocation}")
                    from recordModule in _moduleInBoxService.RecordModuleInfo(moduleInfoResult.BarCode_GoodsPN, moduleInfoResult.BarCode, args.CellCode, PlcStationOpt.StationCode)
                    select new { moduleInfoResult.BarCode };

            var r = await q;
            if (r.IsError)
            {
                var errValue = r.ErrorValue;
                return new DealReqTakePhotoCompleteNgWraps { ErrorType = errValue.ErrorType, ErrorCode = errValue.ErrorCode, ErrorMessage = errValue.ErrorMessage }.ToErrResult<DealReqTakePhotoCompleteOkWraps, DealReqTakePhotoCompleteNgWraps>();
            }

            return new DealReqTakePhotoCompleteOkWraps() { ModuleCode = r.ResultValue.BarCode, }.ToOkResult<DealReqTakePhotoCompleteOkWraps, DealReqTakePhotoCompleteNgWraps>();
        }

        protected override async Task HandleErrAsync(MstMsg pending, DealReqTakePhotoCompleteNgWraps err)
        {
            var Builder = new MstMsgFlagBuilder(pending.TakePhotoComplete.Flag);
            pending.TakePhotoComplete.Flag = Builder.Ack(false).Build();
            pending.TakePhotoComplete.ErrorCode = (ushort)err.ErrorCode;

            await RecordLogAsync(LogLevel.Error, $"PLC请求拍电芯码完成NG：{err.ErrorCode} - {err.ErrorMessage}");
        }

        protected override async Task HandleOkAsync(MstMsg pending, DealReqTakePhotoCompleteOkWraps descriptions)
        {
            var Builder = new MstMsgFlagBuilder(pending.TakePhotoComplete.Flag);
            pending.TakePhotoComplete.Flag = Builder.Ack(true).Build();
            pending.TakePhotoComplete.ErrorCode = 0;
            pending.TakePhotoComplete.ModuleCode = String40.New(descriptions.ModuleCode);
            //pending.TakePhotoComplete.ModuleType = descriptions.ModuleType;

            await RecordLogAsync(LogLevel.Information, $"PLC请求拍电芯码完成OK");
        }

        protected override async Task ResetAckAsync(MstMsg pending)
        {
            var Builder = new MstMsgFlagBuilder(pending.TakePhotoComplete.Flag);
            pending.TakePhotoComplete.Flag = Builder.Reset().Build();
            pending.TakePhotoComplete.ModuleCode = String40.New(string.Empty);
            pending.TakePhotoComplete.ModuleType = 0;

            await RecordLogAsync(LogLevel.Information, $"处理PLC请求拍电芯码完成结束");
        }
    }
}
