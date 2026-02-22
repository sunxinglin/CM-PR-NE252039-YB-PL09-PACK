using Automatic.Entity.DataDtos;
using Automatic.Protocols.Common;
using Automatic.Protocols.ModuleInBox.Middlewares.Common;
using Automatic.Protocols.ModuleInBox.Models;
using Automatic.Protocols.ModuleInBox.Models.Datas;
using Automatic.Protocols.ModuleInBox.Models.Notifications;
using Automatic.Protocols.ModuleInBox.Models.Wraps;
using Automatic.Protocols.Services;
using Automatic.Shared;
using FutureTech.Protocols;
using Itminus.FSharpExtensions;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.FSharp.Core;
using System.Runtime.InteropServices;

namespace Automatic.Protocols.ModuleInBox.Middlewares
{
    public class DealReqInBoxCompleteMiddleware
        : HandlePlcRequestMiddlewareBase<DevMsg, MstMsg, DealReqInBoxCompleteWraps, DealReqInBoxCompleteOkWraps, DealReqInBoxCompleteNgWraps>

    {
        private readonly ModuleInBoxService _moduleInBoxService;
        private readonly CommonService _commonService;
        private readonly IOptionsMonitor<ApiServerSetting> _ApiServerSetting;
        private readonly bool _isEmptyLoop;

        public DealReqInBoxCompleteMiddleware(ModuleInBoxFlusher flusher, ILogger<DealReqInBoxCompleteMiddleware> logger, IMediator mediator,
            ModuleInBoxService moduleInBoxService, IOptionsMonitor<ApiServerSetting> apiServerSetting, CommonService commonService) : base(flusher, logger, mediator)
        {
            _moduleInBoxService = moduleInBoxService;
            _ApiServerSetting = apiServerSetting;
            _commonService = commonService;
            _isEmptyLoop = _ApiServerSetting.CurrentValue.IsEmptyLoop;
        }

        public override string PlcName => PLCNames.模组入箱;

        public override string ProcName => PLCNames.模组入箱;

        public override bool HasAck(MstMsg p) => p.InBoxComplete.Flag.HasFlag(MstMsgFlag.Ack);

        public override bool HasReq(DevMsg i) => i.InBoxComplete.Flag.HasFlag(DevMsgFlag.Req);

        public override DevMsg RefIncoming(ScanContext ctx) => ctx.DevMsg;

        public override MstMsg RefPending(ScanContext ctx) => ctx.MstMsg;

        protected override FSharpResult<DealReqInBoxCompleteWraps, string> TryParseArgs(DevMsg incoming)
        {
            var vectorCode = incoming.InBoxComplete.VectorCode;
            var packCode = incoming.InBoxComplete.PackCode.EffectiveContent;

            int structLength = Marshal.SizeOf<ModuleInBoxData>();
            var dataBytes = incoming.InBoxComplete.ModuleInBoxDatas;
            var moduleInBoxData = new List<ModuleInBoxData>();
            for (int i = 0; i < DevMsg_InBoxComplete.ModuleInBoxDataNum; i++)
            {
                var data = MarshalHelper.BytesToStruct<ModuleInBoxData>(dataBytes.Skip(i * DevMsg_InBoxComplete.ModuleInBoxDataSize).Take(DevMsg_InBoxComplete.ModuleInBoxDataSize).ToArray());
                moduleInBoxData.Add(data);
            }
            _mediator.Publish(new ModuleInBoxNotification() { ModuleInBoxDatas = moduleInBoxData });

            var res = new DealReqInBoxCompleteWraps
            {
                VectorCode = vectorCode,
                PackCode = packCode,
                ModuleInBoxDatas = moduleInBoxData
            };

            return res.ToOkResult<DealReqInBoxCompleteWraps, string>();
        }

        protected override async Task<FSharpResult<DealReqInBoxCompleteOkWraps, DealReqInBoxCompleteNgWraps>> HandleArgsAsync(DealReqInBoxCompleteWraps args)
        {
            await RecordLogAsync(LogLevel.Information, $"收到PLC请求入箱完成 - 载具号：{args.VectorCode} - 产品条码：{args.PackCode}");
            if (_isEmptyLoop)
            {
                return new DealReqInBoxCompleteOkWraps().ToOkResult<DealReqInBoxCompleteOkWraps, DealReqInBoxCompleteNgWraps>();
            }
            // 记录入箱数据，上传至CATL MES
            var data = new ModuleInBoxDataUploadDto()
            {
                VectorCode = args.VectorCode,
                StepCode = PlcStationOpt.StepCode,
                StationCode = PlcStationOpt.StationCode,
                PackCode = args.PackCode,
                ModouleDatas = args.ModuleInBoxDatas.Select(s => new ModouleData
                {
                    ModuleCode = s.ModuleCode.EffectiveContent,
                    KeepDuation = s.PressurizeDuration,
                    ModuleLenth = s.ModuleLenth,
                    DownDistance = s.DownDistance,
                    DownPressure = s.DownPressure,
                    LeftPressure = s.LeftPressure,
                    RightPressure = s.RightPressure,
                    CompleteTime = DateTime.TryParse(s.CompleteTime, out DateTime t) ? t : t,
                }).ToList(),
            };

            var q = from blockData in _moduleInBoxService.UploadData(data)
                    select blockData;

            var r = await q;
            if (r.IsError)
            {
                var errValue = r.ErrorValue;
                return new DealReqInBoxCompleteNgWraps() { ErrorCode = errValue.ErrorCode, ErrorType = errValue.ErrorType, ErrorMessage = errValue.ErrorMessage }.ToErrResult<DealReqInBoxCompleteOkWraps, DealReqInBoxCompleteNgWraps>();
            }

            return new DealReqInBoxCompleteOkWraps().ToOkResult<DealReqInBoxCompleteOkWraps, DealReqInBoxCompleteNgWraps>();
        }

        protected override async Task HandleErrAsync(MstMsg pending, DealReqInBoxCompleteNgWraps err)
        {
            var Builder = new MstMsgFlagBuilder(pending.InBoxComplete.Flag);
            pending.InBoxComplete.Flag = Builder.Ack(false).Build();
            pending.InBoxComplete.ErrorCode = (ushort)err.ErrorCode;

            await RecordLogAsync(LogLevel.Error, $"PLC请求入箱完成NG：{err.ErrorCode} - {err.ErrorMessage}");
        }

        protected override async Task HandleOkAsync(MstMsg pending, DealReqInBoxCompleteOkWraps descriptions)
        {
            var Builder = new MstMsgFlagBuilder(pending.InBoxComplete.Flag);
            pending.InBoxComplete.Flag = Builder.Ack(true).Build();
            pending.InBoxComplete.ErrorCode = 0;

            await RecordLogAsync(LogLevel.Information, "PLC请求入箱完成OK");
        }

        protected override async Task ResetAckAsync(MstMsg pending)
        {
            var Builder = new MstMsgFlagBuilder(pending.InBoxComplete.Flag);
            pending.InBoxComplete.Flag = Builder.Reset().Build();

            await RecordLogAsync(LogLevel.Information, "处理PLC请求入箱完成结束");
        }

    }
}
