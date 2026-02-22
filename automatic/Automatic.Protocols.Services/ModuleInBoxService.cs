using Automatic.Entity;
using Automatic.Entity.DataDtos;
using Automatic.Shared;
using Itminus.FSharpExtensions;
using Microsoft.Extensions.Options;
using Microsoft.FSharp.Core;

namespace Automatic.Protocols.Services
{
    public class ModuleInBoxService
    {
        private readonly IAutomaticTraceApi _trApi;
        private readonly IOptionsMonitor<ApiServerSetting> _AppSetting;
        private readonly bool _isDebug;

        public ModuleInBoxService(IAutomaticTraceApi trApi, IOptionsMonitor<ApiServerSetting> appSetting)
        {
            _trApi = trApi;
            _AppSetting = appSetting;
            _isDebug = _AppSetting.CurrentValue.IsDebug;
        }

        public async Task<FSharpResult<bool, ServiceErrResponse>> CheckGlueTimeOut(string productPn, string StationCode, string PackCode)
        {
            try
            {
                var result = await _trApi.CheckGlueTimeOut(productPn, StationCode, PackCode);
                if (!result.IsError)
                {
                    return true.ToOkResult<bool, ServiceErrResponse>();
                }
                return result.ToErrResult<bool, ServiceErrResponse>();
            }
            catch (Exception ex)
            {
                var resp = new ServiceErrResponse();
                resp.ErrorMessage = ex.Message;
                resp.ErrorType = ReponseErrorType.上位机错误;
                resp.ErrorCode = 500;
                return resp.ToErrResult<bool, ServiceErrResponse>();
            }
        }

        public async Task<FSharpResult<uint, ServiceErrResponse>> GetGlueDuration(string productPn, string StationCode, string PackCode)
        {
            try
            {
                if (_isDebug)
                {
                    return ((uint)10).ToOkResult<uint, ServiceErrResponse>();
                }

                var resp = new ServiceErrResponse();
                var result = await _trApi.GetGlueDuration(productPn, StationCode, PackCode);
                if (result == 0)
                {
                    resp.ErrorCode = 500;
                    resp.ErrorMessage = $"涂胶已超时";
                    resp.ErrorType = ReponseErrorType.上位机错误;
                    return resp.ToErrResult<uint, ServiceErrResponse>();
                }
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

        public async Task<FSharpResult<CatlMesResponse, ServiceErrResponse>> GetModuleCodeFromCATL(string cellCode, string stationCode)
        {
            try
            {
                if (_isDebug)
                {
                    return new CatlMesResponse().ToOkResult<CatlMesResponse, ServiceErrResponse>();
                }

                var result = await _trApi.GetModuleCodeFromCATL(cellCode, stationCode);

                if (result.code != 0)
                {
                    var resp = new ServiceErrResponse();
                    resp.ErrorMessage = result.message;
                    resp.ErrorType = ReponseErrorType.上位机错误;
                    resp.ErrorCode = result.code;
                    return resp.ToErrResult<CatlMesResponse, ServiceErrResponse>();
                }

                return result.ToOkResult<CatlMesResponse, ServiceErrResponse>();
            }
            catch (Exception ex)
            {
                var resp = new ServiceErrResponse();
                resp.ErrorMessage = ex.Message;
                resp.ErrorType = ReponseErrorType.上位机错误;
                resp.ErrorCode = 500;
                return resp.ToErrResult<CatlMesResponse, ServiceErrResponse>();
            }
        }

        public async Task<FSharpResult<CatlMesResponse, ServiceErrResponse>> CheckModuleCodeFromCATL(string moduleCode, string stationCode)
        {
            try
            {
                if (_isDebug)
                {
                    return new CatlMesResponse().ToOkResult<CatlMesResponse, ServiceErrResponse>();
                }

                var result = await _trApi.CheckModuleCodeFromCATL(moduleCode, stationCode);

                if (result.code != 0)
                {
                    var resp = new ServiceErrResponse();
                    resp.ErrorMessage = result.message;
                    resp.ErrorType = ReponseErrorType.上位机错误;
                    resp.ErrorCode = result.code;
                    return resp.ToErrResult<CatlMesResponse, ServiceErrResponse>();
                }

                return result.ToOkResult<CatlMesResponse, ServiceErrResponse>();
            }
            catch (Exception ex)
            {
                var resp = new ServiceErrResponse();
                resp.ErrorMessage = ex.Message;
                resp.ErrorType = ReponseErrorType.上位机错误;
                resp.ErrorCode = 500;
                return resp.ToErrResult<CatlMesResponse, ServiceErrResponse>();
            }
        }

        public async Task<FSharpResult<bool, ServiceErrResponse>> RecordModuleInfo(string modulePN, string moduleCode, string cellCode, string stationCode)
        {
            try
            {
                if (_isDebug)
                {
                    return true.ToOkResult<bool, ServiceErrResponse>();
                }

                var result = await _trApi.RecordModuleInfo(modulePN, moduleCode, cellCode, stationCode);
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
                resp.ErrorType = ReponseErrorType.上位机错误;
                resp.ErrorCode = 500;
                return resp.ToErrResult<bool, ServiceErrResponse>();
            }
        }

        public async Task<FSharpResult<bool, ServiceErrResponse>> UploadData(ModuleInBoxDataUploadDto dto)
        {
            try
            {
                var result = await _trApi.SaveAndUploadData(dto);
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
                resp.ErrorType = ReponseErrorType.上位机错误;
                resp.ErrorCode = 500;
                return resp.ToErrResult<bool, ServiceErrResponse>();
            }
        }

        public async Task<FSharpResult<bool, ServiceErrResponse>> UploadSingleModule(ModuleInBoxSingleModuleUploadDto dto)
        {
            try
            {
                var result = await _trApi.SaveAndUploadSingleModule(dto);
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
                resp.ErrorType = ReponseErrorType.上位机错误;
                resp.ErrorCode = 500;
                return resp.ToErrResult<bool, ServiceErrResponse>();
            }
        }

        public async Task<FSharpResult<bool, ServiceErrResponse>> CheckModuleInfo(string moduleCode, string modulePN, int moduleLocation, string stationCode)
        {
            try
            {
                var result = await _trApi.CheckModuleInfo(moduleCode, modulePN, moduleLocation, stationCode);
                if (!result.IsError)
                {
                    return true.ToOkResult<bool, ServiceErrResponse>();
                }
                return result.ToErrResult<bool, ServiceErrResponse>();
            }
            catch (Exception ex)
            {
                var resp = new ServiceErrResponse();
                resp.ErrorMessage = ex.Message;
                resp.ErrorType = ReponseErrorType.上位机错误;
                resp.ErrorCode = 500;
                return resp.ToErrResult<bool, ServiceErrResponse>();
            }
        }

    }
}
