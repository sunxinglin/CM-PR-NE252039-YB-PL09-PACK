using AsZero.Core.Services.Repos;

using Microsoft.AspNetCore.Mvc;

using Yee.Entitys.DBEntity.Production;
using Yee.Services.Production;

namespace Yee.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class Base_StationTaskRecordTimeController : ControllerBase
    {
        private readonly Base_StationTaskRecordTimeService _service;

        public Base_StationTaskRecordTimeController(Base_StationTaskRecordTimeService service)
        {
            _service = service;
        }

       
        [HttpPost]
        public async Task<Response> AddConfig(Base_StationTask_RecordTime obj)
        {
            var user = Request.Cookies.Where(p => p.Key == "SET_NAME").First().Value.ToString();
            return await _service.AddConfig(obj, user);
        }

        [HttpPost]
        public async Task<Response> UpdateConfig(Base_StationTask_RecordTime obj)
        {
            var user = Request.Cookies.Where(p => p.Key == "SET_NAME").First().Value.ToString();
            return await _service.UpdateConfig(obj, user);
        }

        [HttpPost]
        public async Task<Response> DeleteConfig(Base_StationTask_RecordTime obj)
        {
            var user = Request.Cookies.Where(p => p.Key == "SET_NAME").First().Value.ToString();
            return await _service.DeleteConfig(obj, user);
        }

        [HttpGet]
        public async Task<Response<IList<Base_StationTask_RecordTime>>> LoadConfig(int stationTaskId)
        {
            return await _service.GetStationAnyloadTaskByTaskid(stationTaskId);
        }
    }
}
