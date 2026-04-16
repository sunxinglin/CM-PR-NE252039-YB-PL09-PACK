using AsZero.Core.Services.Repos;

using Microsoft.AspNetCore.Mvc;

using Yee.Entitys.DBEntity;
using Yee.Entitys.DTOS;
using Yee.Services.ProductionRecord;

namespace Yee.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class StationTaskMainController : ControllerBase
    {
        private StationTaskMainService _serivce;
        public StationTaskMainController(StationTaskMainService service)
        {
            _serivce = service;
        }

        [HttpGet]
        public async Task<Response<List<Proc_StationTask_Main>>> GetList([FromQuery] StationTaskMainGetListDTO dto)
        {
            return await _serivce.GetList(dto);
        }

    }
}
