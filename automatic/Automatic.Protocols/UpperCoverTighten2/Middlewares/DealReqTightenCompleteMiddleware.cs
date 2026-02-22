using Automatic.Entity.DataDtos;
using Automatic.Protocols.Common;
using Automatic.Protocols.Services;
using Automatic.Protocols.UpperCoverTighten2.Middlewares.Common;
using Automatic.Protocols.UpperCoverTighten2.Models;
using Automatic.Protocols.UpperCoverTighten2.Models.Datas;
using Automatic.Protocols.UpperCoverTighten2.Models.Wraps;
using Automatic.Shared;
using FutureTech.Protocols;
using Itminus.FSharpExtensions;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.FSharp.Core;
using static Automatic.Entity.DataDtos.AutoTightenDataUploadDto;

namespace Automatic.Protocols.UpperCoverTighten2.Middlewares
{
    public class DealReqTightenCompleteMiddleware :
        HandlePlcRequestMiddlewareBase<DevMsg, MstMsg, DealReqTightenCompleteWraps, DealReqTightenCompleteOkWraps, DealReqTightenCompleteNgWraps>
    {
        private readonly AutoTightenService _autoTightenService;
        private readonly IOptionsMonitor<ApiServerSetting> _ApiServerSetting;
        private readonly bool _isEmptyLoop;

        public DealReqTightenCompleteMiddleware(PlcFlusher flusher, ILogger<DealReqTightenCompleteMiddleware> logger,
            IMediator mediator, AutoTightenService autoTightenService, IOptionsMonitor<ApiServerSetting> apiServerSetting) : base(flusher, logger, mediator)
        {
            _autoTightenService = autoTightenService;
            _ApiServerSetting = apiServerSetting;
            _isEmptyLoop = _ApiServerSetting.CurrentValue.IsEmptyLoop;
        }

        public override string PlcName => PLCNames.上盖拧紧2;

        public override string ProcName => PLCNames.上盖拧紧2;

        public override bool HasAck(MstMsg p) => p.TightenComplete.Flag.HasFlag(MstMsgFlag.Ack);

        public override bool HasReq(DevMsg i) => i.TightenComplete.Flag.HasFlag(DevMsgFlag.Req);

        public override DevMsg RefIncoming(ScanContext ctx) => ctx.DevMsg;

        public override MstMsg RefPending(ScanContext ctx) => ctx.MstMsg;

        protected override FSharpResult<DealReqTightenCompleteWraps, string> TryParseArgs(DevMsg incoming)
        {
            var vectorCode = incoming.TightenComplete.VectorCode;
            var packCode = incoming.TightenComplete.PackCode.EffectiveContent;

            var tighteningData = new List<AutoTightenDataUploadTightenItem>();
            for (int i = 0; i < DevMsg_TightenComplete.TightenNum; i++)
            {
                var data = MarshalHelper.BytesToStruct<TighteningData>(
                    incoming.TightenComplete.TightenDatas
                    .Skip(i * DevMsg_TightenComplete.TightenStructSize)
                    .Take(DevMsg_TightenComplete.TightenStructSize).ToArray()
                    );

                if (!data.TighteningResultFlag.HasFlag(TighteningFlag.Tightening_OK) && !data.TighteningResultFlag.HasFlag(TighteningFlag.Tightening_NOK))
                {
                    continue;
                }

                var realData = new AutoTightenDataUploadTightenItem
                {
                    OrderNo = i + 1,
                    Angle_Max = data.AngleMax,
                    ResultIsOK = data.TightenPass,
                    Angle_Min = data.AngleMin,
                    TorqueRate_Max = data.TorqueRate_Max,
                    TorqueRate_Min = data.TorqueRate_Min,
                    FinalAngle = data.Final_angle,
                    FinalTorque = data.Final_torque,
                    ProgramNo = data.Pset_selected,
                    Constant1 = data.Constant1,
                    Constant2 = data.Constant2,
                    Angle_trend = data.Angle_trend,
                    TargetAngle = data.TargetAngle,
                    TargetTorqueRate = data.TargetTorqueRate,
                    Torque_trend = data.Torque_trend,
                };

                tighteningData.Add(realData);
            }
            var reuslt = new DealReqTightenCompleteWraps
            {
                VectorCode = vectorCode,
                PackCode = packCode,
                TightenDatas = tighteningData
            };
            _mediator.Publish(reuslt);
            return reuslt.ToOkResult<DealReqTightenCompleteWraps, string>();
        }

        protected override async Task<FSharpResult<DealReqTightenCompleteOkWraps, DealReqTightenCompleteNgWraps>> HandleArgsAsync(DealReqTightenCompleteWraps args)
        {
            await RecordLogAsync(LogLevel.Information, $"PLC请求拧紧完成---[载具编号：{args.VectorCode}]-[产品条码：{args.PackCode}]");
            var okResult = new DealReqTightenCompleteOkWraps();
            if (_isEmptyLoop)
            {
                return okResult.ToOkResult<DealReqTightenCompleteOkWraps, DealReqTightenCompleteNgWraps>();
            }
            var datas = new AutoTightenDataUploadDto()
            {
                Pin = args.PackCode,
                VectorCode = args.VectorCode,
                StepCode = PlcStationOpt.StepCode,
                StationCode = PlcStationOpt.StationCode,
                BoltType = "Lid",
                TightenDatas = args.TightenDatas
            };
            var q = from saveResp in _autoTightenService.SaveData(datas)
                    from complate in _autoTightenService.StationTaskComplate(datas)
                    select saveResp;
            var r = await q;
            if (r.IsError)
            {
                var errValue = r.ErrorValue;
                return new DealReqTightenCompleteNgWraps() { ErrorCode = errValue.ErrorCode, ErrorType = errValue.ErrorType, ErrorMessage = errValue.ErrorMessage }.ToErrResult<DealReqTightenCompleteOkWraps, DealReqTightenCompleteNgWraps>();
            }
            return okResult.ToOkResult<DealReqTightenCompleteOkWraps, DealReqTightenCompleteNgWraps>();
        }

        protected override async Task HandleErrAsync(MstMsg pending, DealReqTightenCompleteNgWraps err)
        {
            var builder = new MstMsgFlagBuilder(pending.TightenComplete.Flag);
            pending.TightenComplete.Flag = builder.Ack(false).Build();
            pending.TightenComplete.ErrorCode = (ushort)err.ErrorCode;
            await RecordLogAsync(LogLevel.Error, $"PLC请求拧紧完成NG：{err.ErrorCode} - {err.ErrorMessage}");
        }

        protected override async Task HandleOkAsync(MstMsg pending, DealReqTightenCompleteOkWraps descriptions)
        {
            var builder = new MstMsgFlagBuilder(pending.TightenComplete.Flag);
            pending.TightenComplete.Flag = builder.Ack(true).Build();
            pending.TightenComplete.ErrorCode = 0;
            await RecordLogAsync(LogLevel.Information, $"PLC请求拧紧完成OK");
        }

        protected override async Task ResetAckAsync(MstMsg pending)
        {
            var builder = new MstMsgFlagBuilder(pending.TightenComplete.Flag);
            pending.TightenComplete.Flag = builder.Reset().Build();

            await RecordLogAsync(LogLevel.Information, $"处理PLC请求拧紧完成结束");
        }

    }
}
