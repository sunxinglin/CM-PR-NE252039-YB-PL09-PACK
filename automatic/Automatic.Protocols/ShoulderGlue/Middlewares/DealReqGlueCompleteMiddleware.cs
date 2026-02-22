using Automatic.Entity.DataDtos;
using Automatic.Protocols.Common;
using Automatic.Protocols.Services;
using Automatic.Protocols.ShoulderGlue.Middlewares.Common;
using Automatic.Protocols.ShoulderGlue.Models;
using Automatic.Protocols.ShoulderGlue.Models.Datas;
using Automatic.Protocols.ShoulderGlue.Models.Wraps;
using Automatic.Shared;
using FutureTech.Protocols;
using Itminus.FSharpExtensions;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.FSharp.Core;

namespace Automatic.Protocols.ShoulderGlue.Middlewares
{
    public class DealReqGlueCompleteMiddleware
        : HandlePlcRequestMiddlewareBase<DevMsg, MstMsg, DealReqGlueCompleteWraps, DealReqGlueCompleteOkWraps, DealReqGlueCompleteNgWraps>
    {
        private readonly AutoGlueService _glueService;
        private readonly CommonService _commonService;
        private readonly IOptionsMonitor<ApiServerSetting> _ApiServerSetting;
        private readonly bool _isEmptyLoop;

        public DealReqGlueCompleteMiddleware(ShoulderGlueFlusher flusher, ILogger<DealReqGlueCompleteMiddleware> logger, IMediator mediator, 
            AutoGlueService glueService, CommonService commonService, IOptionsMonitor<ApiServerSetting> apiServerSetting) : base(flusher, logger, mediator)
        {
            _glueService = glueService;
            _commonService = commonService;
            _ApiServerSetting = apiServerSetting;
            _isEmptyLoop = _ApiServerSetting.CurrentValue.IsEmptyLoop;
        }

        public override string PlcName => PLCNames.肩部涂胶;

        public override string ProcName => PLCNames.肩部涂胶;

        public override bool HasAck(MstMsg p) => p.GlueComplete.Flag.HasFlag(MstMsgFlag.Ack);

        public override bool HasReq(DevMsg i) => i.GlueComplete.Flag.HasFlag(DevMsgFlag.Req);

        public override DevMsg RefIncoming(ScanContext ctx) => ctx.DevMsg;

        public override MstMsg RefPending(ScanContext ctx) => ctx.MstMsg;

        protected override FSharpResult<DealReqGlueCompleteWraps, string> TryParseArgs(DevMsg incoming)
        {
            var vectorCode = incoming.GlueComplete.VectorCode;
            var packCode = incoming.GlueComplete.PackCode.EffectiveContent;
            var glueData = new List<GlueData>();
            for (int i = 0; i < DevMsg_GlueComplete.GlueDataCount; i++)
            {
                var glueD = MarshalHelper.BytesToStruct<GlueData>(incoming.GlueComplete.GlueDatas.Skip(i * DevMsg_GlueComplete.GlueDataSize).Take(DevMsg_GlueComplete.GlueDataSize).ToArray());
                //if (glueD.GlueValueA <= 0 && glueD.GlueValueB <= 0)
                //{
                //    continue;
                //}
                glueData.Add(glueD);
            }
            var result = new DealReqGlueCompleteWraps
            {
                VectorCode = vectorCode,
                PackCode = packCode,
                StartTime = incoming.GlueComplete.GlueStartTime,
                GlueDatas = glueData
            };
            _mediator.Publish(result);
            return result.ToOkResult<DealReqGlueCompleteWraps, string>();
        }

        protected override async Task<FSharpResult<DealReqGlueCompleteOkWraps, DealReqGlueCompleteNgWraps>> HandleArgsAsync(DealReqGlueCompleteWraps args)
        {
            await RecordLogAsync(LogLevel.Information, $"收到PLC请求涂胶完成---[载具编号：{args.VectorCode}]-[产品条码：{args.PackCode}]");

            if (_isEmptyLoop)
            {
                return new DealReqGlueCompleteOkWraps().ToOkResult<DealReqGlueCompleteOkWraps, DealReqGlueCompleteNgWraps>();
            }

            var datas = new GlueDataDto()
            {
                PackCode = args.PackCode,
                VectorCode = args.VectorCode,
                //StepCode = PlcStationOpt.StepCode,
                StationCode = PlcStationOpt.StationCode,
                GlueTime = args.StartTime,
                GlueDatas = args.GlueDatas.Select(s => new UploadGlueData(s.GlueValueA, s.GlueValueB, s.GlueProportion, s.GlueValueTotal)).ToList()
            };

            var q = from saveResp in _glueService.SaveGlueDataAndUploadCATL(datas)
                    select saveResp;
            var r = await q;
            if (r.IsError)
            {
                var errValue = r.ErrorValue;
                return new DealReqGlueCompleteNgWraps { ErrorCode = errValue.ErrorCode, ErrorType = errValue.ErrorType, ErrorMessage = errValue.ErrorMessage }.ToErrResult<DealReqGlueCompleteOkWraps, DealReqGlueCompleteNgWraps>();
            }
            var okResult = new DealReqGlueCompleteOkWraps();
            return okResult.ToOkResult<DealReqGlueCompleteOkWraps, DealReqGlueCompleteNgWraps>();
        }

        protected override async Task HandleErrAsync(MstMsg pending, DealReqGlueCompleteNgWraps err)
        {
            var builder = new MstMsgFlagBuilder(pending.GlueComplete.Flag);
            pending.GlueComplete.Flag = builder.Ack(false).Build();
            pending.GlueComplete.ErrorCode = (ushort)err.ErrorCode;

            await RecordLogAsync(LogLevel.Error, $"PLC请求涂胶完成NG：{err.ErrorCode} - {err.ErrorMessage}");
        }

        protected override async Task HandleOkAsync(MstMsg pending, DealReqGlueCompleteOkWraps descriptions)
        {
            var builder = new MstMsgFlagBuilder(pending.GlueComplete.Flag);
            pending.GlueComplete.Flag = builder.Ack(true).Build();
            pending.GlueComplete.ErrorCode = 0;

            await RecordLogAsync(LogLevel.Information, $"PLC请求涂胶完成OK");
        }

        protected override async Task ResetAckAsync(MstMsg pending)
        {
            var builder = new MstMsgFlagBuilder(pending.GlueComplete.Flag);
            pending.GlueComplete.Flag = builder.Reset().Build();

            await RecordLogAsync(LogLevel.Information, $"处理PLC请求涂胶完成结束");
        }

    }
}
