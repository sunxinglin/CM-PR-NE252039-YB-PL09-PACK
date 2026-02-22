using Automatic.Entity;
using Automatic.Entity.DataDtos;
using Automatic.Shared;
using Itminus.FSharpExtensions;
using Microsoft.Extensions.Options;
using Microsoft.FSharp.Core;

namespace Automatic.Protocols.Services
{
    public class LowerBoxGlueService
    {
        private readonly IAutomaticTraceApi _traceApi;
        private readonly IOptionsMonitor<ApiServerSetting> _AppSetting;
        private readonly bool _isDebug;

        public LowerBoxGlueService(IAutomaticTraceApi traceApi, IOptionsMonitor<ApiServerSetting> appSetting)
        {
            _traceApi = traceApi;
            _AppSetting = appSetting;
            _isDebug = _AppSetting.CurrentValue.IsDebug;
        }

        public async Task<FSharpResult<bool, ServiceErrResponse>> SaveDataAndUploadCATL(LowerBoxGlueDataDto dto)
        {
            try
            {
                if (_isDebug)
                {
                    return true.ToOkResult<bool, ServiceErrResponse>();
                }

                var result = await _traceApi.SaveLowerBoxGlueDataAndUploadCATL(dto);
                if (result.IsError)
                {
                    return result.ToErrResult<bool, ServiceErrResponse>();
                }
                return true.ToOkResult<bool, ServiceErrResponse>();
            }
            catch (Exception ex)
            {
                var resp = new ServiceErrResponse();
                resp.ErrorCode = 500;
                resp.ErrorMessage = ex.Message;
                resp.ErrorType = ReponseErrorType.上位机错误;
                return resp.ToErrResult<bool, ServiceErrResponse>();
            }
        }

        public async Task<FSharpResult<bool, ServiceErrResponse>> CheckAllowReGlue(string packCode, string timeFlag)
        {
            try
            {
                if (_isDebug)
                {
                    return true.ToOkResult<bool, ServiceErrResponse>();
                }

                var glueTime = await _traceApi.GetLowerBoxGlueTime(packCode, timeFlag);

                if (glueTime == DateTime.MinValue)
                {
                    var resp = new ServiceErrResponse();
                    resp.ErrorCode = 1403;
                    resp.ErrorMessage = $"获取上次涂胶时间异常";
                    resp.ErrorType = ReponseErrorType.流程顺序错误;
                    return resp.ToErrResult<bool, ServiceErrResponse>();
                }

                var duration = (DateTime.Now - glueTime).TotalMinutes;
                if (duration > 60 && duration < 26 * 60)
                {
                    var resp = new ServiceErrResponse();
                    resp.ErrorCode = 1403;
                    resp.ErrorMessage = $"当前PACK不允许补胶，间隔上次涂胶{duration}分钟，补胶要求间隔时间小于60分钟或大于1560分钟（26小时）";
                    resp.ErrorType = ReponseErrorType.流程顺序错误;
                    return resp.ToErrResult<bool, ServiceErrResponse>();
                }

                return true.ToOkResult<bool, ServiceErrResponse>();
            }
            catch (Exception ex)
            {
                var resp = new ServiceErrResponse();
                resp.ErrorCode = 500;
                resp.ErrorMessage = ex.Message;
                resp.ErrorType = ReponseErrorType.上位机错误;
                return resp.ToErrResult<bool, ServiceErrResponse>();
            }
        }
    }
}
