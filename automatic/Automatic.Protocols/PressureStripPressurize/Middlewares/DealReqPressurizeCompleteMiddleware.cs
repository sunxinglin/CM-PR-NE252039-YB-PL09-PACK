using Automatic.Entity.DataDtos;
using Automatic.Protocols.Common;
using Automatic.Protocols.PressureStripPressurize.Middlewares.Common;
using Automatic.Protocols.PressureStripPressurize.Models;
using Automatic.Protocols.PressureStripPressurize.Models.Datas;
using Automatic.Protocols.PressureStripPressurize.Models.Wraps;
using Automatic.Protocols.Services;
using Automatic.Shared;
using FutureTech.Protocols;
using Itminus.FSharpExtensions;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.FSharp.Core;
using System.Runtime.InteropServices;

namespace Automatic.Protocols.PressureStripPressurize.Middlewares
{
    public class DealReqPressurizeCompleteMiddleware :
        HandlePlcRequestMiddlewareBase<DevMsg, MstMsg, DealReqPressurizeCompleteWraps, DealReqPressurizeCompleteOkWraps, DealReqPressurizeCompleteNgWraps>
    {
        private readonly AutoPressureService _pressureService;
        private readonly IOptionsMonitor<ApiServerSetting> _ApiServerSetting;
        private readonly bool _isEmptyLoop;

        public DealReqPressurizeCompleteMiddleware(PressureStripFlusher flusher, ILogger<DealReqPressurizeCompleteMiddleware> logger, IMediator mediator, AutoPressureService pressureService, IOptionsMonitor<ApiServerSetting> apiServerSetting) : base(flusher, logger, mediator)
        {
            _pressureService = pressureService;
            _ApiServerSetting = apiServerSetting;
            _isEmptyLoop = _ApiServerSetting.CurrentValue.IsEmptyLoop;
        }

        public override string PlcName => PLCNames.压条加压;

        public override string ProcName => PLCNames.压条加压;

        public override bool HasAck(MstMsg p) => p.PressurizeComplete.Flag.HasFlag(MstMsgFlag.Ack);

        public override bool HasReq(DevMsg i) => i.PressurizeComplete.Flag.HasFlag(DevMsgFlag.Req);

        public override DevMsg RefIncoming(ScanContext ctx) => ctx.DevMsg;

        public override MstMsg RefPending(ScanContext ctx) => ctx.MstMsg;

        protected override FSharpResult<DealReqPressurizeCompleteWraps, string> TryParseArgs(DevMsg incoming)
        {
            var vectorCode = incoming.PressurizeComplete.VectorCode;
            var packCode = incoming.PressurizeComplete.PackCode.EffectiveContent ?? "";

            var completeTime = incoming.PressurizeComplete.CompleteTime;
            var wraps = new DealReqPressurizeCompleteWraps
            {
                AgvCode = vectorCode,
                PackCode = packCode,
                CompleteTime = completeTime
            };
            var dataList = new List<PressureData>();
            var dataSize = Marshal.SizeOf(typeof(PressureData));
            for (int i = 0; i < 20; i++)
            {
                var bytes = incoming.PressurizeComplete.PressureDatas.Skip(i * dataSize).Take(dataSize).ToArray();
                var data = MarshalHelper.BytesToStruct<PressureData>(bytes);
                dataList.Add(data);
            }
            wraps.PressureValue = dataList.Select(s => new PressureValue
            {
                KeepDuration = s.KeepDuration,
                PressureAverage = s.PressureAverage,
                PressureMax = s.PressureMax,
                ShoudlerHeight = 0,
            }).ToList();

            _mediator.Publish(wraps);

            return wraps.ToOkResult<DealReqPressurizeCompleteWraps, string>();
        }

        protected override async Task<FSharpResult<DealReqPressurizeCompleteOkWraps, DealReqPressurizeCompleteNgWraps>> HandleArgsAsync(DealReqPressurizeCompleteWraps args)
        {
            await RecordLogAsync(LogLevel.Information, $"收到PLC请求压条加压完成---[载具编号：{args.AgvCode}]-[产品条码：{args.PackCode}]");

            if (_isEmptyLoop)
            {
                return new DealReqPressurizeCompleteOkWraps().ToOkResult<DealReqPressurizeCompleteOkWraps, DealReqPressurizeCompleteNgWraps>();
            }

            var datas = new PressureDataUploadDto()
            {
                Pin = args.PackCode,
                StationCode = PlcStationOpt.StationCode,
                CompleteTime = args.CompleteTime,
                PressureDatas = args.PressureValue,
                VectorCode = args.AgvCode,
            };
            var q = from saveResp in _pressureService.SaveAndUploadPressureData(datas)
                    select saveResp;
            var r = await q;
            if (r.IsError)
            {
                var errValue = r.ErrorValue;
                return new DealReqPressurizeCompleteNgWraps() { ErrorType = errValue.ErrorType, ErrorMessage = errValue.ErrorMessage, ErrorCode = errValue.ErrorCode }.ToErrResult<DealReqPressurizeCompleteOkWraps, DealReqPressurizeCompleteNgWraps>();
            }
            return new DealReqPressurizeCompleteOkWraps().ToOkResult<DealReqPressurizeCompleteOkWraps, DealReqPressurizeCompleteNgWraps>();
        }

        protected override async Task HandleErrAsync(MstMsg pending, DealReqPressurizeCompleteNgWraps err)
        {
            pending.PressurizeComplete.Flag = new MstMsgFlagBuilder(pending.PressurizeComplete.Flag)
                  .Ack(false)
                  .Build();
            pending.PressurizeComplete.ErrorCode = (ushort)err.ErrorCode;

            await RecordLogAsync(LogLevel.Error, $"PLC请求压条加压完成NG：{err.ErrorCode} - {err.ErrorMessage}");
        }

        protected override async Task HandleOkAsync(MstMsg pending, DealReqPressurizeCompleteOkWraps descriptions)
        {
            pending.PressurizeComplete.Flag = new MstMsgFlagBuilder(pending.PressurizeComplete.Flag)
                .Ack(true)
                .Build();
            pending.PressurizeComplete.ErrorCode = 0;

            await RecordLogAsync(LogLevel.Information, $"PLC请求压条加压完成OK");
        }

        protected override async Task ResetAckAsync(MstMsg pending)
        {
            pending.PressurizeComplete.Flag = new MstMsgFlagBuilder(pending.PressurizeComplete.Flag)
                .Reset()
                .Build();

            await RecordLogAsync(LogLevel.Information, $"处理PLC请求压条加压完成结束");
        }

    }
}
