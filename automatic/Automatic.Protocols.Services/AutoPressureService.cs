using Automatic.Entity;
using Automatic.Entity.DataDtos;
using Automatic.Shared;
using Itminus.FSharpExtensions;
using Microsoft.Extensions.Options;
using Microsoft.FSharp.Core;

namespace Automatic.Protocols.Services
{
    public class AutoPressureService
    {
        private readonly IAutomaticTraceApi _traceApi;
        private readonly IOptionsMonitor<ApiServerSetting> _AppSetting;
        private readonly bool _isDebug;
        public AutoPressureService(IAutomaticTraceApi traceApi, IOptionsMonitor<ApiServerSetting> appSetting)
        {
            _traceApi = traceApi;
            _AppSetting = appSetting;
            _isDebug = _AppSetting.CurrentValue.IsDebug;//调试模式下所有数据均不保存
        }

        public async Task<FSharpResult<bool, ServiceErrResponse>> SaveAndUploadPressureData(PressureDataUploadDto dto)
        {
            try
            {
                if (_isDebug)
                {
                    return true.ToOkResult<bool, ServiceErrResponse>();
                }

                var result = await _traceApi.SaveAndUploadPressureData(dto);
                if (result.IsError)
                {
                    return result.ToErrResult<bool, ServiceErrResponse>();
                }
                return true.ToOkResult<bool, ServiceErrResponse>();
            }
            catch (Exception ex)
            {
                var resp = new ServiceErrResponse();
                resp.ErrorMessage = ex.Message;
                resp.ErrorCode = 500;
                resp.ErrorType = ReponseErrorType.上位机错误;
                return resp.ToErrResult<bool, ServiceErrResponse>();
            }
        }
    }
}
