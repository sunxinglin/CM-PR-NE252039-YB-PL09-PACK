using Automatic.Entity.DataDtos;
using Automatic.Protocols.Common;
using Automatic.Protocols.HeatingFilmPressurize.Middlewares.Common;
using Automatic.Protocols.HeatingFilmPressurize.Models;
using Automatic.Protocols.HeatingFilmPressurize.Models.Datas;
using Automatic.Protocols.HeatingFilmPressurize.Models.Wraps;
using Automatic.Protocols.Services;
using Automatic.Shared;
using FutureTech.Protocols;
using Itminus.FSharpExtensions;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.FSharp.Core;
using NPOI.POIFS.Crypt.Dsig;
using System.Runtime.InteropServices;

namespace Automatic.Protocols.HeatingFilmPressurize.Middlewares
{
    public class DealReqPressurizeCompleteMiddleware : HandlePlcRequestMiddlewareBase<DevMsg, MstMsg, DealReqPressurizeCompleteWraps, DealReqPressurizeCompleteOkWraps, DealReqPressurizeCompleteNgWraps>
    {
        private readonly HeatingFilmPressurizeService _pressureService;
        public readonly IOptionsMonitor<ApiServerSetting> _ApiServerSetting;
        private readonly bool _isEmptyLoop;

        public DealReqPressurizeCompleteMiddleware(
            HeatingFilmPressurizeFlusher flusher, ILogger<DealReqPressurizeCompleteMiddleware> logger, IMediator mediator,
            HeatingFilmPressurizeService pressureService,
            IOptionsMonitor<ApiServerSetting> apiServerSetting
            ) : base(flusher, logger, mediator)
        {
            _pressureService = pressureService;
            _ApiServerSetting = apiServerSetting;
            _isEmptyLoop = _ApiServerSetting.CurrentValue.IsEmptyLoop;
        }

        public override string PlcName => PLCNames.加热膜加压;

        public override string ProcName => PLCNames.加热膜加压;

        public override bool HasAck(MstMsg p) => p.PressurizeComplete.Flag.HasFlag(MstMsgFlag.Ack);

        public override bool HasReq(DevMsg i) => i.PressurizeComplete.Flag.HasFlag(DevMsgFlag.Req);

        public override DevMsg RefIncoming(ScanContext ctx) => ctx.DevMsg;

        public override MstMsg RefPending(ScanContext ctx) => ctx.MstMsg;

        protected override FSharpResult<DealReqPressurizeCompleteWraps, string> TryParseArgs(DevMsg incoming)
        {
            var vectorCode = incoming.PressurizeComplete.VectorCode;
            var packCode = incoming.PressurizeComplete.PackCode.EffectiveContent ?? "";
            var startTime = incoming.PressurizeComplete.StartTime;

            var wraps = new DealReqPressurizeCompleteWraps
            {
                VectorCode = vectorCode,
                PackCode = packCode,
                StartTime = startTime
            };

            wraps.PressurizeDatas.Add($"加压开始时间", startTime);

            var dataSize = Marshal.SizeOf(typeof(PressureData));
            for (int i = 0; i < 20; i++)
            {
                var bytes = incoming.PressurizeComplete.PressureDatas.Skip(i * dataSize).Take(dataSize).ToArray();
                var data = MarshalHelper.BytesToStruct<PressureData>(bytes);

                if (data.KeepDuration == default) continue;

                wraps.PressurizeDatas.Add($"保压时长{i + 1}", data.KeepDuration);
                wraps.PressurizeDatas.Add($"最大压力{i + 1}", data.PressureMax);
                wraps.PressurizeDatas.Add($"平均压力{i + 1}", data.PressureAverage);
            }
            _mediator.Publish(wraps);

            return wraps.ToOkResult<DealReqPressurizeCompleteWraps, string>();
        }

        protected override async Task<FSharpResult<DealReqPressurizeCompleteOkWraps, DealReqPressurizeCompleteNgWraps>> HandleArgsAsync(DealReqPressurizeCompleteWraps args)
        {
            await RecordLogAsync(LogLevel.Information, $"收到PLC请求加热膜加压完成---[载具编号：{args.VectorCode}]-[产品条码：{args.PackCode}]");

            if (_isEmptyLoop)
            {
                return new DealReqPressurizeCompleteOkWraps().ToOkResult<DealReqPressurizeCompleteOkWraps, DealReqPressurizeCompleteNgWraps>();
            }

            var datas = new HeatingFilmPressurizeDataDto()
            {
                Pin = args.PackCode,
                StationCode = PlcStationOpt.StationCode,
                StartTime = args.StartTime,
                PressureDatas = args.PressurizeDatas,
                VectorCode = args.VectorCode,
            };
            var q = from saveResp in _pressureService.SaveAndUploadData(datas)
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

            await RecordLogAsync(LogLevel.Error, $"PLC请求加热膜加压完成NG：{err.ErrorCode} - {err.ErrorMessage}");
        }

        protected override async Task HandleOkAsync(MstMsg pending, DealReqPressurizeCompleteOkWraps descriptions)
        {
            pending.PressurizeComplete.Flag = new MstMsgFlagBuilder(pending.PressurizeComplete.Flag)
                    .Ack(true)
                    .Build();
            pending.PressurizeComplete.ErrorCode = 0;

            await RecordLogAsync(LogLevel.Information, $"PLC请求加热膜加压完成OK");
        }

        protected override async Task ResetAckAsync(MstMsg pending)
        {
            pending.PressurizeComplete.Flag = new MstMsgFlagBuilder(pending.PressurizeComplete.Flag)
                    .Reset()
                    .Build();

            await RecordLogAsync(LogLevel.Information, $"处理PLC请求加热膜加压完成结束");
        }


    }
}
