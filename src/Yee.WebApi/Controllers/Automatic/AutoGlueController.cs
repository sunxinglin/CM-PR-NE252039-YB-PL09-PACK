using AsZero.Core.Services.Repos;
using Microsoft.AspNetCore.Mvc;
using Yee.Entitys;
using Yee.Entitys.AutomaticStation;
using Yee.Entitys.DBEntity;
using Yee.Entitys.DTOS;
using Yee.Entitys.DTOS.AutomaticStationDTOS;
using Yee.Services.AutomaticStation;

namespace Yee.WebApi.Controllers.Automatic
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AutoGlueController : ControllerBase
    {
        //StationTask_GlueDetail
        private readonly AutoGlueService _autoGlueService;

        public AutoGlueController(AutoGlueService autoGlueService)
        {
            _autoGlueService = autoGlueService;
        }

        [HttpPost]
        public async Task<ServiceErrResponse> SaveGlueDataAndUploadCATL(GlueDataDto dto)
        {
            var r = await _autoGlueService.SaveGlueDataAndUploadCATL(dto);
            return r.IsError ? r.ErrorValue : r.ResultValue;
        }

        /// <summary>
        /// 获取肩部涂胶数据
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<Response<IList<Proc_GluingInfo>>> LoadGlueData([FromQuery] GlueDetailDto dto)
        {
            return await _autoGlueService.LoadGlueData(dto);
        }

        [HttpGet]
        public async Task<Response<IList<GlueDataSql>>> LoadGlueDataDetail([FromQuery] int dataId)
        {
            return await _autoGlueService.LoadGlueDataDetail(dataId);
        }
        [HttpPost]
        public async Task<Response<CatlMESReponse>> UploadGlueDataAgain(GlueDataDto dto)
        {
            return await _autoGlueService.UploadGlueDataAgain(dto);
        }

        [HttpGet]
        public async Task<uint> GetGlueRemainDuration(string PackCode)
        {
            var resp = await _autoGlueService.GetGlueRemainDuration(PackCode);
            return resp.IsError ? 0 : resp.ResultValue;
        }
    }
}
