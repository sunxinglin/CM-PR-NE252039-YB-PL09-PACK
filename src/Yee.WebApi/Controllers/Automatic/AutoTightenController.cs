using AsZero.Core.Services.Repos;
using Microsoft.AspNetCore.Mvc;
using Yee.Entitys.AutomaticStation;
using Yee.Entitys.DBEntity;
using Yee.Entitys.DTOS;
using Yee.Services.AutomaticStation;

namespace Yee.WebApi.Controllers.Automatic
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AutoTightenController : ControllerBase
    {
        private readonly AutoTightenService _service;
        public AutoTightenController(AutoTightenService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<ServiceErrResponse> UploadData(AutoTightenDataUploadDto dto)
        {
            var result = await _service.UploadData(dto);
            return result.IsError ? result.ErrorValue : result.ResultValue;
        }

        [HttpGet]
        public async Task<Response<IList<Proc_AutoBoltInfo_Detail>>> LoadAutoTightenData([FromQuery] AutoTightenInfoDto dto)
        {
            return await _service.LoadAutoTightenData(dto);
        }

        [HttpGet]
        public async Task<Response<IList<AutoBlotInfo>>> LoadAutoTightenDataDetail([FromQuery] int dataId)
        {
            return await _service.LoadAutoTightenDataDetail(dataId);
        }

    }
}
