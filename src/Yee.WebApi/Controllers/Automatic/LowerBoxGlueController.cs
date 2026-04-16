using AsZero.Core.Services.Repos;

using Microsoft.AspNetCore.Mvc;

using Yee.Entitys;
using Yee.Entitys.AutomaticStation;
using Yee.Entitys.DBEntity.ProductionRecords;
using Yee.Entitys.DTOS.AutomaticStationDTOS;
using Yee.Services.AutomaticStation;

namespace Yee.WebApi.Controllers.Automatic
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LowerBoxGlueController : ControllerBase
    {
        private readonly LowerBoxGlueService _service;

        public LowerBoxGlueController(LowerBoxGlueService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<ServiceErrResponse> SaveDataAndUploadCATL(LowerBoxGlueSaveDataDto dto)
        {
            var r = await _service.SaveDataAndUploadCATL(dto);
            return r.IsError ? r.ErrorValue : r.ResultValue;
        }

        [HttpGet]
        public async Task<Response<IList<Proc_LowerBoxGlueInfo>>> LoadData([FromQuery] LowerBoxGlueLoadDataDto dto)
        {
            return await _service.LoadData(dto);
        }

        [HttpGet]
        public async Task<Response<IList<Proc_StationTask_TimeRecord>>> LoadTimeData([FromQuery] LowerBoxGlueLoadDataDto dto)
        {
            return await _service.LoadTimeData(dto);
        }

        [HttpPost]
        public async Task<Response<CatlMESReponse>> UploadDataAgain(GlueDataDto dto)
        {
            return await _service.UploadDataAgain(dto);
        }

        [HttpPost]
        public async Task<ServiceErrResponse> UpdateDataAndUploadCATL(LowerBoxReGlueDataDto dto)
        {
            var r = await _service.UpdateDataAndUploadCATL(dto);
            return r.IsError ? r.ErrorValue : r.ResultValue;
        }

        [HttpGet]
        public async Task<DateTime> GetGlueTime(string packCode, string timeFlag)
        {
            var r = await _service.GetGlueTime(packCode, timeFlag);
            return r.IsError ? DateTime.MinValue : r.ResultValue;
        }
    }
}
