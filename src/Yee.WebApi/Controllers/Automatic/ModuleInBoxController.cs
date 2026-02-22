using AsZero.Core.Services.Repos;
using Microsoft.AspNetCore.Mvc;
using Yee.Entitys;
using Yee.Entitys.AutomaticStation;
using Yee.Entitys.DBEntity;
using Yee.Entitys.DTOS.AutomaticStationDTOS;
using Yee.Services.AutomaticStation;

namespace Yee.WebApi.Controllers.Automatic
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ModuleInBoxController : ControllerBase
    {
        private readonly ModuleInBoxService _moduleInBoxService;

        public ModuleInBoxController(ModuleInBoxService moduleInBoxService)
        {
            _moduleInBoxService = moduleInBoxService;
        }

        [HttpGet]
        public async Task<uint> GetGlueDuration(string productPn, string StationCode, string PackCode)
        {
            var resp = await _moduleInBoxService.GetGlueDuration(productPn, StationCode, PackCode);
            return resp.IsError ? 0 : resp.ResultValue;
        }

        [HttpGet]
        public async Task<CatlMESReponse> GetModuleCodeFromCATL(string cellCode, string stationCode)
        {
            var resp = await _moduleInBoxService.GetModuleCodeFromCATL(cellCode, stationCode);
            return resp.IsError ? new CatlMESReponse { code = resp.ErrorValue.ErrorCode, message = resp.ErrorValue.ErrorMessage } : resp.ResultValue;
        }

        [HttpGet]
        public async Task<CatlMESReponse> CheckModuleCodeFromCATL(string moduleCode, string stationCode)
        {
            var resp = await _moduleInBoxService.CheckModuleFromCATL(moduleCode, stationCode);
            return resp.IsError ? new CatlMESReponse { code = resp.ErrorValue.ErrorCode, message = resp.ErrorValue.ErrorMessage } : resp.ResultValue;
        }

        [HttpGet]
        public async Task<ServiceErrResponse> CheckModuleInfo(string moduleCode, string modulePN, int moduleLocation, string stationCode)
        {
            var resp = await _moduleInBoxService.CheckModuleInfo(moduleCode, modulePN, moduleLocation, stationCode);
            return resp.IsError ? new ServiceErrResponse { ErrorCode = resp.ErrorValue.ErrorCode, ErrorMessage = resp.ErrorValue.ErrorMessage } : resp.ResultValue;
        }

        [HttpPost]
        public async Task<ServiceErrResponse> RecordModuleInfo(string modulePN, string blockCode, string cellCode, string stationCode)
        {
            var resp = await _moduleInBoxService.RecordModuleInfo(modulePN, blockCode, cellCode, stationCode);
            return resp.IsError ? resp.ErrorValue : resp.ResultValue;
        }

        [HttpPost]
        public async Task<ServiceErrResponse> SaveAndUploadData(ModuleInBoxDataUploadDto dto)
        {
            var resp = await _moduleInBoxService.SaveAndUploadData(dto);
            return resp.IsError ? resp.ErrorValue : resp.ResultValue;
        }

        [HttpPost]
        public async Task<ServiceErrResponse> SaveAndUploadSingleModule(ModuleInBoxSingleModuleUploadDto dto)
        {
            var resp = await _moduleInBoxService.SaveAndUploadSingleModule(dto);
            return resp.IsError ? resp.ErrorValue : resp.ResultValue;
        }

        [HttpGet]
        public async Task<Response<IList<Proc_ModuleInBox_DataCollect>>> LoadData([FromQuery] ModuleInBoxDataDto dto)
        {
            return await _moduleInBoxService.LoadData(dto);
        }

        [HttpGet]
        public async Task<Response<IList<ModuleInBoxDataJsonModel>>> LoadDataDetail([FromQuery] int dataId)
        {
            return await _moduleInBoxService.LoadDataDetail(dataId);
        }

        [HttpGet]
        public async Task<Response<CatlMESReponse>> UploadDataAgain([FromQuery] string packCode, string stationCode, bool isNeedChangeStatus)
        {
            var resp = await _moduleInBoxService.UploadDataAgain(packCode, stationCode, isNeedChangeStatus);
            return resp;
        }

        [HttpGet]
        public async Task<CatlMESReponse> GetModuleCleanTime(string moduleCode, string stationCode)
        {
            var resp = await _moduleInBoxService.GetModuleCleanTime(moduleCode, stationCode);
            return resp.IsError ? new CatlMESReponse { code = resp.ErrorValue.ErrorCode, message = resp.ErrorValue.ErrorMessage } : resp.ResultValue;
        }
    }
}
