using AsZero.Core.Services.Repos;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Yee.Entitys.DBEntity.Production;
using Yee.Entitys.Production;

using Yee.Services.Production;

namespace Yee.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class StationTaskTightenReworkController : ControllerBase
    {
        private readonly Base_StationTaskTightenReworkService _service;

        public StationTaskTightenReworkController(Base_StationTaskTightenReworkService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<Response> Add(Base_StationTask_TightenRework obj)
        {
            var result = new Response<Base_StationTask_TightenRework>();
            try
            {
                var user = Request.Cookies.Where(p => p.Key == "SET_NAME").First().Value.ToString();
                var newObj = await _service.Add(obj, user);
                result.Result = newObj;
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }

            return result;

        }
        [HttpPost]
        public async Task<Response> Update(Base_StationTask_TightenRework obj)
        {
            var result = new Response<Base_StationTask_TightenRework>();
            try
            {
                var user = Request.Cookies.Where(p => p.Key == "SET_NAME").First().Value.ToString();
                var res = await this._service.Update(obj, user);
                result.Result = res;
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }

            return result;
        }

        [HttpPost]
        public async Task<Response> DeleteEntity(DeleteByIdsInput input)
        {
            var result = new Response();
            try
            {
                var user = Request.Cookies.Where(p => p.Key == "SET_NAME").First().Value.ToString();
                foreach (var Id in input.Ids)
                {
                    var entity = await _service.GetById(Id);
                    if (entity != null)
                    {
                        await _service.Delete(entity, user);
                    }
                }
                result.Message = "操作成功";
                return result;
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }

            return result;
        }

        [HttpGet]
        public async Task<Response<List<Base_StationTask_TightenRework>>> LoadByTaskId(int taskid)
        {
            var result = new Response<List<Base_StationTask_TightenRework>>();
            try
            {
                var list = await this._service.GetScrewListByStationTaskID(taskid);
                result.Result = list;
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }

            return result;
        }

    }
}
