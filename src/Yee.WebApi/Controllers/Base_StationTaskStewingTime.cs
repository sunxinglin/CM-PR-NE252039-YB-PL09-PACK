using AsZero.Core.Services.Repos;
using Microsoft.AspNetCore.Mvc;
using Yee.Entitys.Production;
using Yee.Services.Production;
using Yee.Services.Request;

namespace Yee.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class Base_StationTaskStewingTimeController : Controller
    {
        private readonly Base_StationTaskStewingTimeService base_StationTaskStewingTimeService;

        public Base_StationTaskStewingTimeController(Base_StationTaskStewingTimeService base_StationTaskStewingTimeService)
        {
            this.base_StationTaskStewingTimeService = base_StationTaskStewingTimeService;
        }
        [HttpPost]
        public async Task<Response<Base_StationTaskStewingTime>> Update(Base_StationTaskStewingTime entity)
        {
            var response = new Response<Base_StationTaskStewingTime>();
            try
            {
                var user = Request.Cookies.Where(p => p.Key == "SET_NAME").First().Value.ToString();
                response.Data = await base_StationTaskStewingTimeService.Update(entity,user);
            }
            catch (Exception ex)
            {

                response.Code = 500;
                response.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return response;

        }
        [HttpPost]
        public async Task<Response<Base_StationTaskStewingTime>> Add(Base_StationTaskStewingTime entity)
        {
            var response = new Response<Base_StationTaskStewingTime>();
            try
            {
                var user = Request.Cookies.Where(p => p.Key == "SET_NAME").First().Value.ToString();
                response.Data = await base_StationTaskStewingTimeService.Add(entity,user);
            }
            catch (Exception ex)
            {

                response.Code = 500;
                response.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return response;

        }
        /// <summary>
        /// 删除
        /// </summary>
        [HttpPost]
        public async Task<Response<string>> DeleteEntity(DeleteByIdsInput input)
        {
            var result = new Response<string>();
            try
            {
                foreach (var Id in input.Ids)
                {
                    var entity = await base_StationTaskStewingTimeService.GetById(Id);
                    if (entity != null)
                    {
                        var user = Request.Cookies.Where(p => p.Key == "SET_NAME").First().Value.ToString();
                        await base_StationTaskStewingTimeService.Delete(entity,user);
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
        public async Task<Response<List<Base_StationTaskStewingTime?>>> GetByTaskId(int taskid)
        {
            var response = new Response<List<Base_StationTaskStewingTime?>>();
            try
            {
                response.Result = await base_StationTaskStewingTimeService.GetByTaskId(taskid);
            }
            catch (Exception ex)
            {

                response.Code = 500;
                response.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return response;
        }
    }
}
