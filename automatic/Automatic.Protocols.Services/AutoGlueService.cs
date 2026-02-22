using Automatic.Entity;
using Automatic.Entity.DataDtos;
using Automatic.Shared;
using Itminus.FSharpExtensions;
using Microsoft.Extensions.Options;
using Microsoft.FSharp.Core;

namespace Automatic.Protocols.Services
{
    public class AutoGlueService
    {
        private readonly IAutomaticTraceApi _traceApi;
        private readonly IOptionsMonitor<ApiServerSetting> _AppSetting;
        private readonly bool _isDebug;

        public AutoGlueService(IAutomaticTraceApi traceApi, IOptionsMonitor<ApiServerSetting> appSetting)
        {
            _traceApi = traceApi;
            _AppSetting = appSetting;
            _isDebug = _AppSetting.CurrentValue.IsDebug;
        }

        public async Task<FSharpResult<bool, ServiceErrResponse>> SaveGlueDataAndUploadCATL(GlueDataDto dto)
        {
            try
            {
                if (_isDebug)
                {
                    return true.ToOkResult<bool, ServiceErrResponse>();
                }

                var result = await _traceApi.SaveGlueDataAndUploadCATL(dto);
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

        /// <summary>
        /// 获取肩部涂胶剩余时间
        /// </summary>
        /// <param name="packCode"></param>
        /// <param name="stationCode"></param>
        /// <returns></returns>
        public async Task<FSharpResult<uint, ServiceErrResponse>> GetGlueRemainDuration(string packCode)
        {
            try
            {
                var resp = new ServiceErrResponse();
                var result = await _traceApi.GetGlueDuration(packCode);
                return result.ToOkResult<uint, ServiceErrResponse>();
            }
            catch (Exception ex)
            {
                var resp = new ServiceErrResponse();
                resp.ErrorMessage = ex.Message;
                resp.ErrorType = ReponseErrorType.上位机错误;
                resp.ErrorCode = 500;
                return resp.ToErrResult<uint, ServiceErrResponse>();
            }
        }
    }
}
