using Automatic.Entity.DataDtos;
using Automatic.Entity;
using Automatic.Shared;
using Itminus.FSharpExtensions;
using Microsoft.Extensions.Options;
using Microsoft.FSharp.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automatic.Protocols.Services
{
    public class LowerBoxReGlueService
    {
        private readonly IAutomaticTraceApi _traceApi;
        private readonly IOptionsMonitor<ApiServerSetting> _AppSetting;
        private readonly bool _isDebug;

        public LowerBoxReGlueService(IAutomaticTraceApi traceApi, IOptionsMonitor<ApiServerSetting> appSetting)
        {
            _traceApi = traceApi;
            _AppSetting = appSetting;
            _isDebug = _AppSetting.CurrentValue.IsDebug;
        }

        public async Task<FSharpResult<bool, ServiceErrResponse>> SaveDataAndUploadCATL(LowerBoxReGlueDataDto dto)
        {
            try
            {
                if (_isDebug)
                {
                    return true.ToOkResult<bool, ServiceErrResponse>();
                }

                var result = await _traceApi.UpdateDataAndUploadCATL(dto);
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
    }
}
