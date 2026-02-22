using Automatic.Entity;
using Automatic.Entity.DataDtos;
using Automatic.Shared;
using Itminus.FSharpExtensions;
using Microsoft.Extensions.Options;
using Microsoft.FSharp.Core;

namespace Automatic.Protocols.Services
{
    public class CommonService
    {
        private readonly IAutomaticTraceApi _traceApi;
        private readonly IOptionsMonitor<ApiServerSetting> _AppSetting;
        private readonly bool _isDebug;
        private readonly bool _isEmptyLoop;

        public CommonService(IAutomaticTraceApi traceApi, IOptionsMonitor<ApiServerSetting> appSetting)
        {
            _traceApi = traceApi;
            _AppSetting = appSetting;
            _isDebug = _AppSetting.CurrentValue.IsDebug;
            _isEmptyLoop = _AppSetting.CurrentValue.IsEmptyLoop;
        }

        /// <summary>
        /// 校验载具绑定信息与流程顺序是否正确
        /// </summary>
        /// <param name="Pin"></param>
        /// <param name="StationCode"></param>
        /// <param name="VectorCode"></param>
        /// <returns></returns>
        public async Task<FSharpResult<bool, ServiceErrResponse>> CheckProductFlowAndVectorBind(string Pin, string StationCode, int VectorCode, string vectorStation, string productPn)
        {
            try
            {
                if (_isEmptyLoop)
                {
                    return true.ToOkResult<bool, ServiceErrResponse>();
                }

                var res = await _traceApi.CheckVectorBindAndFlowOrder(Pin, VectorCode, StationCode, vectorStation, productPn);
                if (res.IsError)
                {
                    return res.ToErrResult<bool, ServiceErrResponse>();
                }
                return true.ToOkResult<bool, ServiceErrResponse>();
            }
            catch (Exception ex)
            {
                var resp = new ServiceErrResponse()
                {
                    ErrorType = ReponseErrorType.上位机错误,
                    ErrorCode = 500,
                    ErrorMessage = ex.Message,
                };
                return resp.ToErrResult<bool, ServiceErrResponse>();
            }
        }

        public async Task<FSharpResult<string, ServiceErrResponse>> MakePackStart(string packCode, string stationCode)
        {
            if (_isDebug)
            {
                return "00000".ToOkResult<string, ServiceErrResponse>();
            }
            try
            {
                var result = await _traceApi.MakePackStart(packCode, stationCode);
                if (!result.Success)
                {
                    return new ServiceErrResponse() { ErrorCode = Convert.ToInt32(result.ErrorCode), ErrorMessage = result.ErrorMessage }.ToErrResult<string, ServiceErrResponse>();
                }
                var resp = result.Data;

                if (resp == null || resp.code != 0)
                {
                    return new ServiceErrResponse() { ErrorCode = Convert.ToInt32(resp?.code ?? Convert.ToInt32(result.ErrorCode)), ErrorMessage = resp?.message ?? result.ErrorMessage, ErrorType = ReponseErrorType.CatlMes错误 }.ToErrResult<string, ServiceErrResponse>();
                }
                return resp.BarCode_GoodsPN.ToOkResult<string, ServiceErrResponse>();
            }
            catch (Exception ex)
            {
                return new ServiceErrResponse() { ErrorCode = Convert.ToInt32(500), ErrorMessage = ex.Message, ErrorType = ReponseErrorType.上位机错误 }.ToErrResult<string, ServiceErrResponse>();
            }
        }

        public async Task<FSharpResult<string, ServiceErrResponse>> GetProductPnFromCatlMes(string packCode, string stationCode)
        {
            if (_isDebug)
            {
                return "00000".ToOkResult<string, ServiceErrResponse>();
            }
            try
            {
                var result = await _traceApi.GetProductPnFromCatlMes(packCode, stationCode);
                if (!result.Success)
                {
                    return new ServiceErrResponse() { ErrorCode = Convert.ToInt32(result.ErrorCode), ErrorMessage = result.ErrorMessage }.ToErrResult<string, ServiceErrResponse>();
                }
                var resp = result.Data;

                if (resp.code != 0)
                {
                    return new ServiceErrResponse() { ErrorCode = Convert.ToInt32(resp.code), ErrorMessage = resp.message, ErrorType = ReponseErrorType.CatlMes错误 }.ToErrResult<string, ServiceErrResponse>();
                }
                return resp.BarCode_GoodsPN.ToOkResult<string, ServiceErrResponse>();
            }
            catch (Exception ex)
            {
                return new ServiceErrResponse() { ErrorCode = Convert.ToInt32(500), ErrorMessage = ex.Message, ErrorType = ReponseErrorType.上位机错误 }.ToErrResult<string, ServiceErrResponse>();
            }
        }

        public async Task<FSharpResult<(string, int), ServiceErrResponse>> GetCurrentStation(string stationFlag, string packCode, Action<string> action)
        {
            try
            {
                var result = await _traceApi.GetCurrentStation(stationFlag, packCode);
                if (!result.Success)
                {
                    return new ServiceErrResponse() { ErrorCode = Convert.ToInt32(result.ErrorCode), ErrorMessage = result.ErrorMessage }.ToErrResult<(string, int), ServiceErrResponse>();
                }
                action(result.Data.Item1);
                return result.Data.ToOkResult<(string, int), ServiceErrResponse>();
            }
            catch (Exception ex)
            {
                var resp = new ServiceErrResponse()
                {
                    ErrorType = ReponseErrorType.上位机错误,
                    ErrorCode = 500,
                    ErrorMessage = ex.Message,
                };
                return resp.ToErrResult<(string, int), ServiceErrResponse>();
            }
        }

        public async Task<FSharpResult<bool, ServiceErrResponse>> EnsureExiestOrCreateMessionRecord(string Pin, string StationCode, string MessionType, int VectorCode, string productPn)
        {
            try
            {
                var dto = new CreateMessionRecordDto
                {
                    Pin = Pin,
                    ProductPn = productPn,
                    StationCode = StationCode,
                    MessionType = MessionType,
                    VectorCode = VectorCode
                };
                var result = await _traceApi.CreateMessionRecord(dto);
                if (result.IsError)
                {
                    return result.ToErrResult<bool, ServiceErrResponse>();
                }

                return true.ToOkResult<bool, ServiceErrResponse>();
            }

            catch (Exception ex)
            {
                var resp = new ServiceErrResponse()
                {
                    ErrorType = ReponseErrorType.上位机错误,
                    ErrorCode = 500,
                    ErrorMessage = ex.Message,
                };
                return resp.ToErrResult<bool, ServiceErrResponse>();
            }
        }

        public async Task<FSharpResult<bool, ServiceErrResponse>> CheckBoxLevel(string StationFlag, string PackCode, int CurrentLevel)
        {
            try
            {
                var str = Environment.GetEnvironmentVariable("IsLevelControl");
                if (str != "Enable")
                {
                    return true.ToOkResult<bool, ServiceErrResponse>();
                }
                var result = await _traceApi.CheckLastBoxLevel(StationFlag, PackCode, CurrentLevel);
                if (result.IsError)
                {
                    return result.ToErrResult<bool, ServiceErrResponse>();
                }

                return true.ToOkResult<bool, ServiceErrResponse>();
            }

            catch (Exception ex)
            {
                var resp = new ServiceErrResponse()
                {
                    ErrorType = ReponseErrorType.上位机错误,
                    ErrorCode = 500,
                    ErrorMessage = ex.Message,
                };
                return resp.ToErrResult<bool, ServiceErrResponse>();
            }
        }

        public async Task<FSharpResult<bool, ServiceErrResponse>> ChangeLastBoxLevel(string StationFlag, string PackCode, int CurrentLevel)
        {
            try
            {
                var result = await _traceApi.ChangeLastBoxLevel(StationFlag, PackCode, CurrentLevel);
                if (result.IsError)
                {
                    return result.ToErrResult<bool, ServiceErrResponse>();
                }

                return true.ToOkResult<bool, ServiceErrResponse>();
            }

            catch (Exception ex)
            {
                var resp = new ServiceErrResponse()
                {
                    ErrorType = ReponseErrorType.上位机错误,
                    ErrorCode = 500,
                    ErrorMessage = ex.Message,
                };
                return resp.ToErrResult<bool, ServiceErrResponse>();
            }
        }

        public async Task<FSharpResult<bool, ServiceErrResponse>> CheckBomInventory(string packCode, string MaterialPn, string MaterialCode, int useNum, string stationCode)
        {
            if (_isDebug)
            {
                return true.ToOkResult<bool, ServiceErrResponse>();
            }
            try
            {
                var result = await _traceApi.CheckBomInventory(packCode, MaterialPn, MaterialCode, useNum, stationCode);
                if (result.IsError)
                {
                    return result.ToErrResult<bool, ServiceErrResponse>();
                }

                return true.ToOkResult<bool, ServiceErrResponse>();
            }

            catch (Exception ex)
            {
                var resp = new ServiceErrResponse()
                {
                    ErrorType = ReponseErrorType.上位机错误,
                    ErrorCode = 500,
                    ErrorMessage = ex.Message,
                };
                return resp.ToErrResult<bool, ServiceErrResponse>();
            }
        }

        public async Task<FSharpResult<int, ServiceErrResponse>> GetPackMAT(string productPN)
        {
            try
            {
                var result = await _traceApi.GetPackMAT(productPN);
                if (result == 0)
                {
                    var resp = new ServiceErrResponse()
                    {
                        ErrorType = ReponseErrorType.数据异常,
                        ErrorCode = 500,
                        ErrorMessage = $"未查询到产品{productPN}的料号",
                    };

                    return resp.ToErrResult<int, ServiceErrResponse>();
                }

                return result.ToOkResult<int, ServiceErrResponse>();
            }

            catch (Exception ex)
            {
                var resp = new ServiceErrResponse()
                {
                    ErrorType = ReponseErrorType.上位机错误,
                    ErrorCode = 500,
                    ErrorMessage = ex.Message,
                };
                return resp.ToErrResult<int, ServiceErrResponse>();
            }
        }
    }
}
