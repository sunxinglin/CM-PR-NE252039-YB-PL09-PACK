using Automatic.Entity.DataDtos;
using Automatic.Protocols.Services;
using Automatic.Protocols.TerminalReshape.Middlewares.Common;
using Automatic.Protocols.TerminalReshape.Models;
using Automatic.Protocols.TerminalReshape.Models.Datas;
using Automatic.Protocols.TerminalReshape.Models.WorkRequireFlags;
using Automatic.Protocols.TerminalReshape.Models.Wraps;
using Automatic.Shared;
using FutureTech.Protocols;
using Itminus.FSharpExtensions;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.FSharp.Core;
using System.Runtime.InteropServices;

namespace Automatic.Protocols.TerminalReshape.Middlewares
{
    public class DealReqCompleteReshapeMiddleware
        : HandlePlcRequestMiddlewareBase<DevMsg, MstMsg, DealReqCompleteReshapeWraps, DealReqCompleteGlueOkWraps, DealReqCompleteGlueNgWraps>
    {
        private readonly AutoPressureService _pressureService;
        public readonly IOptionsMonitor<ApiServerSetting> _ApiServerSetting;
        private readonly CommonService _commonService;
        private readonly bool _isEmptyLoop;
        public DealReqCompleteReshapeMiddleware(TerminalReshapeFlusher flusher, ILogger<DealReqCompleteReshapeMiddleware> logger,
            IMediator mediator, AutoPressureService pressureService, IOptionsMonitor<ApiServerSetting> apiServerSetting, CommonService commonService) : base(flusher, logger, mediator)
        {
            _pressureService = pressureService;
            _ApiServerSetting = apiServerSetting;
            _commonService = commonService;
            _isEmptyLoop = _ApiServerSetting.CurrentValue.IsEmptyLoop;
        }

        public override string PlcName => PLCNames.极柱整形;

        public override string ProcName => PLCNames.极柱整形;

        public override bool HasAck(MstMsg p) => p.MstMsgAckComplateReshapeFlag.HasFlag(MstMsgAckComplateReshapeFlag.AckCompleteReshape);

        public override bool HasReq(DevMsg i) => i.DevMsgReqComplateReshapeFlag.HasFlag(DevMsgReqComplateReshapeFlag.ReqCompleteReshape);

        public override DevMsg RefIncoming(ScanContext ctx) => ctx.DevMsg;

        public override MstMsg RefPending(ScanContext ctx) => ctx.MstMsg;


        protected override async Task<FSharpResult<DealReqCompleteGlueOkWraps, DealReqCompleteGlueNgWraps>> HandleArgsAsync(DealReqCompleteReshapeWraps args)
        {
            await RecordLogAsync(LogLevel.Information, $"收到PLC请求整形完成---[载具编号：{args.VectorCode}]-[产品条码：{args.PackCode}]");

            if (_isEmptyLoop)
            {
                return new DealReqCompleteGlueOkWraps().ToOkResult<DealReqCompleteGlueOkWraps, DealReqCompleteGlueNgWraps>();
            }

            var datas = new PressureDataUploadDto()
            {
                Pin = args.PackCode,
                StationCode = PlcStationOpt.StationCode,
                CompleteTime = args.StartTime,
                PressureDatas = args.PressureValue,
                VectorCode = args.VectorCode,
            };
            var q = from newStationCode in _commonService.GetCurrentStation(PlcStationOpt.StationFlag, args.PackCode, s => PlcStationOpt.StationCode = s)
                    let newStationCodeLog = RecordLogAsync(LogLevel.Information, $"当前工位-{newStationCode}")
                    from saveResp in _pressureService.SaveAndUploadPressureData(datas)
                    select saveResp;
            var r = await q;
            if (r.IsError)
            {
                var errValue = r.ErrorValue;
                return new DealReqCompleteGlueNgWraps() { ErrorType = errValue.ErrorType, ErrorMessage = errValue.ErrorMessage, ErrorCode = errValue.ErrorCode }.ToErrResult<DealReqCompleteGlueOkWraps, DealReqCompleteGlueNgWraps>();
            }

            var okResult = new DealReqCompleteGlueOkWraps();
            return okResult.ToOkResult<DealReqCompleteGlueOkWraps, DealReqCompleteGlueNgWraps>();
        }

        protected override async Task HandleErrAsync(MstMsg pending, DealReqCompleteGlueNgWraps err)
        {
            var builder = new MstMsgAckComplateReshapeFlagBuilder(pending.MstMsgAckComplateReshapeFlag);
            pending.MstMsgAckComplateReshapeFlag = builder.AckCompleteReshape(false).Build();
            pending.ComplateErrorCode = (ushort)err.ErrorCode;

            await RecordLogAsync(LogLevel.Error, $"PLC请求整形完成NG：{err.ErrorCode} - {err.ErrorMessage}");
        }

        protected override async Task HandleOkAsync(MstMsg pending, DealReqCompleteGlueOkWraps descriptions)
        {
            var builder = new MstMsgAckComplateReshapeFlagBuilder(pending.MstMsgAckComplateReshapeFlag);
            pending.MstMsgAckComplateReshapeFlag = builder.AckCompleteReshape(true).Build();
            pending.ComplateErrorCode = 0;

            await RecordLogAsync(LogLevel.Information, $"PLC请求整形完成OK");
        }

        protected override async Task ResetAckAsync(MstMsg pending)
        {
            var builder = new MstMsgAckComplateReshapeFlagBuilder(pending.MstMsgAckComplateReshapeFlag);
            pending.MstMsgAckComplateReshapeFlag = builder.ResetCompleteReshape().Build();

            await RecordLogAsync(LogLevel.Information, $"处理PLC请求整形完成结束");
        }

        protected override FSharpResult<DealReqCompleteReshapeWraps, string> TryParseArgs(DevMsg incoming)
        {
            var vectorCode = incoming.ComplateVectorCode;
            var packCode = incoming.ComplatePackCode.EffectiveContent;
            var startTime = incoming.ReshapeStartTime;

            var wraps = new DealReqCompleteReshapeWraps();
            wraps.VectorCode = vectorCode;
            wraps.PackCode = packCode;
            wraps.StartTime = startTime;
            var dataList = new List<ReshapeData>();
            var dataSize = Marshal.SizeOf(typeof(ReshapeData));
            for (int i = 0; i < 4; i++)
            {
                var bytes = incoming.ReshapeData.Skip(i * dataSize).Take(dataSize).ToArray();
                var reshape = MarshalHelper.BytesToStruct<ReshapeData>(bytes);
                dataList.Add(reshape);
            }
            wraps.PressureValue = dataList.Select(s => new PressureValue
            {
                KeepDuration = s.KeepDuration,
                PressureAverage = s.PressureAverage,
                PressureMax = s.PressureMax,
                ShoudlerHeight = s.ShoudlerHeight,
            }).ToList();
            _mediator.Publish(wraps);
            return wraps.ToOkResult<DealReqCompleteReshapeWraps, string>();
        }
    }
}
