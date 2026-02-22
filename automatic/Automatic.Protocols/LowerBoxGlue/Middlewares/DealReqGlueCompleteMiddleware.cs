using Automatic.Entity.DataDtos;
using Automatic.Protocols.Common;
using Automatic.Protocols.LowerBoxGlue.Middlewares.Common;
using Automatic.Protocols.LowerBoxGlue.Models;
using Automatic.Protocols.LowerBoxGlue.Models.Datas;
using Automatic.Protocols.LowerBoxGlue.Models.Wraps;
using Automatic.Protocols.Services;
using Automatic.Shared;
using FutureTech.Protocols;
using Itminus.FSharpExtensions;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.FSharp.Core;
using NPOI.XSSF.Streaming.Values;

namespace Automatic.Protocols.LowerBoxGlue.Middlewares
{
    public class DealReqGlueCompleteMiddleware
        : HandlePlcRequestMiddlewareBase<DevMsg, MstMsg, DealReqGlueCompleteWraps, DealReqGlueCompleteOkWraps, DealReqGlueCompleteNgWraps>
    {
        private readonly LowerBoxGlueService _glueService;
        public readonly IOptionsMonitor<ApiServerSetting> _ApiServerSetting;
        private readonly bool _isEmptyLoop;

        public DealReqGlueCompleteMiddleware(LowerBoxGlueFlusher flusher, ILogger<DealReqGlueCompleteMiddleware> logger, IMediator mediator,
            LowerBoxGlueService glueService, IOptionsMonitor<ApiServerSetting> apiServerSetting) : base(flusher, logger, mediator)
        {
            _glueService = glueService;
            _ApiServerSetting = apiServerSetting;
            _isEmptyLoop = _ApiServerSetting.CurrentValue.IsEmptyLoop;
        }

        public override string PlcName => PLCNames.下箱体涂胶;

        public override string ProcName => PLCNames.下箱体涂胶;

        public override bool HasAck(MstMsg p) => p.GlueComplete.Flag.HasFlag(MstMsgFlag.Ack);

        public override bool HasReq(DevMsg i) => i.GlueComplete.Flag.HasFlag(DevMsgFlag.Req);

        public override DevMsg RefIncoming(ScanContext ctx) => ctx.DevMsg;

        public override MstMsg RefPending(ScanContext ctx) => ctx.MstMsg;

        protected override async Task<FSharpResult<DealReqGlueCompleteOkWraps, DealReqGlueCompleteNgWraps>> HandleArgsAsync(DealReqGlueCompleteWraps args)
        {
            await RecordLogAsync(LogLevel.Information, $"收到PLC请求涂胶完成---[载具编号：{args.VectorCode}]-[产品条码：{args.PackCode}]");

            var okResult = new DealReqGlueCompleteOkWraps();

            if (_isEmptyLoop)
            {
                return okResult.ToOkResult<DealReqGlueCompleteOkWraps, DealReqGlueCompleteNgWraps>();
            }

            if (!DateTime.TryParse(args.StartTime, out _))
            {
                return new DealReqGlueCompleteNgWraps { ErrorCode = 400, ErrorMessage = "涂胶开始时间格式异常", ErrorType = string.Empty }.ToErrResult<DealReqGlueCompleteOkWraps, DealReqGlueCompleteNgWraps>();
            }

            var datas = new LowerBoxGlueDataDto()
            {
                PackCode = args.PackCode,
                VectorCode = args.VectorCode,
                StationCode = PlcStationOpt.StationCode,
                GlueStartTime = args.StartTime,
                GlueDatas = args.GlueDatas
            };
            var q = from saveResp in _glueService.SaveDataAndUploadCATL(datas)
                    select saveResp;
            var r = await q;
            if (r.IsError)
            {
                var errValue = r.ErrorValue;
                return new DealReqGlueCompleteNgWraps { ErrorCode = errValue.ErrorCode, ErrorMessage = errValue.ErrorMessage, ErrorType = errValue.ErrorType }.ToErrResult<DealReqGlueCompleteOkWraps, DealReqGlueCompleteNgWraps>();
            }
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

        protected override FSharpResult<DealReqGlueCompleteWraps, string> TryParseArgs(DevMsg incoming)
        {
            var vectorCode = incoming.GlueComplete.VectorCode;
            var packCode = incoming.GlueComplete.PackCode.EffectiveContent;
            var glueDatas = new Dictionary<string, object>
            {
                { "胶总重", incoming.GlueComplete.GlueTotalWeight }
            };

            for (int i = 0; i < DevMsg_GlueComplete.GlueDataCount; i++)
            {
                var glueData = MarshalHelper.BytesToStruct<GlueData>(incoming.GlueComplete.GlueDatas.Skip(i * DevMsg_GlueComplete.GlueDataSize).Take(DevMsg_GlueComplete.GlueDataSize).ToArray());
                //if (glueData._GlueValueTotal <= 0 && glueData.GlueProportion <= 0)
                //{
                //    continue;
                //}
                glueDatas.Add($"区域{i + 1}胶比例", glueData.GlueProportion.ToString());
                glueDatas.Add($"区域{i + 1}胶重", glueData._GlueValueTotal.ToString());
            }
            var result = new DealReqGlueCompleteWraps
            {
                VectorCode = vectorCode,
                PackCode = packCode,
                StartTime = incoming.GlueComplete.GlueStartTime,
                GlueDatas = glueDatas
            };
            _mediator.Publish(result);
            return result.ToOkResult<DealReqGlueCompleteWraps, string>();
        }
    }
}
