using Microsoft.AspNetCore.Mvc;
using Yee.Entitys;
using Yee.Entitys.AutomaticStation;
using Yee.Services.AutomaticStation;

namespace Yee.WebApi.Controllers.Automatic
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AutomaticCommonController : ControllerBase
    {
        private readonly AutomicCommonService _commonService;

        public AutomaticCommonController(AutomicCommonService commonService)
        {
            _commonService = commonService;
        }

        [HttpGet]
        public async Task<ServiceErrResponse> CheckVectorBindAndFlowOrder(string pin, int vectorCode, string stationCode, string vectorStation, string productPn)
        {
            var result = await _commonService.CheckVectorBindAndFlowOrder(pin, vectorCode, stationCode, vectorStation, productPn, "");
            return result.IsError ? result.ErrorValue : result.ResultValue;
        }

        [HttpGet]
        public async Task<ServiceErrResponse> CheckFlowOrder(string pin, string stationCode, string productPn)
        {
            var result = await _commonService.CheckFlowOrder(pin, stationCode, productPn);
            return result.IsError ? result.ErrorValue : result.ResultValue;
        }

        [HttpGet]
        public async Task<Resp<CatlMESReponse>> GetProductPnFromCatlMes(string packCode, string stationCode)
        {
            var resp = new Resp<CatlMESReponse>();
            var result = await _commonService.GetProductPnFromCatlMes(packCode, stationCode);
            if (result.IsError)
            {
                resp.Success = false;
                resp.ErrorInfo = new ErrorInfo() { Message = result.ErrorValue.ErrorMessage, Code = result.ErrorValue.ErrorCode.ToString() };
                return resp;
            }
            resp.Success = true;
            resp.Data = result.ResultValue;
            return resp;
        }

        [HttpGet]
        public async Task<Resp<CatlMESReponse>> MakePackStart(string packCode, string stationCode)
        {
            var resp = new Resp<CatlMESReponse>();
            var result = await _commonService.MakePackStart(packCode, stationCode);
            if (result.IsError)
            {
                resp.Success = false;
                resp.ErrorInfo = new ErrorInfo() { Message = result.ErrorValue.ErrorMessage, Code = result.ErrorValue.ErrorCode.ToString() };
                return resp;
            }
            resp.Success = true;
            resp.Data = result.ResultValue;
            return resp;
        }

        /// <summary>
        /// 获取当前工位
        /// </summary>
        /// <param name="stationFlag"></param>
        /// <param name="PackCode"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<Resp<(string, int)>> GetCurrentStation(string stationFlag, string PackCode)
        {
            var result = await _commonService.GetCurrentStation(stationFlag, PackCode);
            return result.ToSucc();
        }

        /// <summary>
        /// 创建、更新追溯信息为进行中
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ServiceErrResponse> CreateMessionRecord(CreateMessionRecordDto dto)
        {
            var result = await _commonService.CreateMessionRecord(dto);
            if (result.IsError)
                return result.ErrorValue;
            return new ServiceErrResponse().ToSuccess();
        }

        [HttpGet]
        public async Task<ServiceErrResponse> CheckLastBoxLevel(string StationFlag, string PackCode, int CurrentLevel)
        {
            var result = await _commonService.CheckLastBoxLevel(StationFlag, PackCode, CurrentLevel);
            return result.IsError ? result.ErrorValue : result.ResultValue;
        }

        [HttpPost]
        public async Task<ServiceErrResponse> ChangeLastBoxLevel(string StationFlag, string PackCode, int CurrentLevel)
        {
            var result = await _commonService.ChangeLastBoxLevel(StationFlag, PackCode, CurrentLevel);
            return result.IsError ? result.ErrorValue : result.ResultValue;
        }

        [HttpGet]
        public async Task<ServiceErrResponse> CheckBomInventory(string packCode, string materialPN, string materialCode, int useNum, string stationCode)
        {
            var result = await _commonService.CheckBomInventory(packCode, materialPN, materialCode, useNum, stationCode);
            return result.IsError ? result.ErrorValue : result.ResultValue;
        }

        [HttpGet]
        public async Task<int> GetPackMAT(string productPn)
        {
            return await _commonService.GetPackMAT(productPn);
        }


        [HttpGet]

        public async Task<CatlMESReponse> CheckSfcStatus(string packCode, string StationCode)
        {
            var result = await _commonService.CheckSfcStatus(packCode, StationCode);
            return result;
        }
    }
}

