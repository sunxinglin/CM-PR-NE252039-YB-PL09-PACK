using Automatic.Entity;
using Automatic.Entity.DataDtos;
using Automatic.Shared;
using Itminus.FSharpExtensions;
using Microsoft.Extensions.Options;
using Microsoft.FSharp.Core;

namespace Automatic.Protocols.Services
{
    public class AutoTightenService
    {
        private readonly IAutomaticTraceApi _trApi;
        private readonly IOptionsMonitor<ApiServerSetting> _AppSetting;
        private readonly bool _isDebug;
        private readonly bool _isEmptyLoop;

        public AutoTightenService(IAutomaticTraceApi trApi, IOptionsMonitor<ApiServerSetting> appSetting)
        {
            _trApi = trApi;
            _AppSetting = appSetting;
            _isDebug = _AppSetting.CurrentValue.IsDebug;//调试模式下所有数据均不保存
            _isEmptyLoop = _AppSetting.CurrentValue.IsEmptyLoop;//调试模式下所有数据均不保存
        }

        public async Task<FSharpResult<bool, ServiceErrResponse>> SaveData(AutoTightenDataUploadDto dto)
        {
            try
            {
                if (_isEmptyLoop)
                {
                    return true.ToOkResult<bool, ServiceErrResponse>();
                }

                var res = await _trApi.SaveAndUploadTightenData(dto);
                if (res.IsError)
                {
                    return res.ToErrResult<bool, ServiceErrResponse>();
                }
                return true.ToOkResult<bool, ServiceErrResponse>();
            }
            catch (Exception ex)
            {
                var response = new ServiceErrResponse
                {
                    ErrorCode = 500,
                    ErrorMessage = ex.Message,
                    ErrorType = ReponseErrorType.上位机错误,
                };
                return response.ToErrResult<bool, ServiceErrResponse>();
            }
        }

        public async Task<FSharpResult<bool, ServiceErrResponse>> StationTaskComplate(AutoTightenDataUploadDto dto)
        {
            return true.ToOkResult<bool, ServiceErrResponse>();
        }
    }
}
