using AsZero.Core.Services.Repos;
using Microsoft.AspNetCore.Mvc;
using Yee.Entitys.Production;
using Yee.Services.Production;
using Yee.Services.Request;

namespace Yee.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class Base_StationTaskLeakController : Controller
    {
        private readonly Base_StationTaskLeakService base_StationTaskLeakService;

        public Base_StationTaskLeakController(Base_StationTaskLeakService base_StationTaskLeakService)
        {
            this.base_StationTaskLeakService = base_StationTaskLeakService;
        }
        [HttpPost]
        public async Task<Response<Base_StationTaskLeak>> Update(Base_StationTaskLeak entity)
        {
            var response = new Response<Base_StationTaskLeak>();
            try
            {
                var user = Request.Cookies.Where(p => p.Key == "SET_NAME").First().Value.ToString();
                response.Data = await base_StationTaskLeakService.Update(entity, user);
            }
            catch (Exception ex)
            {

                response.Code = 500;
                response.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return response;

        }
        [HttpPost]
        public async Task<Response<Base_StationTaskLeak>> Add(Base_StationTaskLeak entity)
        {
            var response = new Response<Base_StationTaskLeak>();
            try
            {
                var user = Request.Cookies.Where(p => p.Key == "SET_NAME").First().Value.ToString();
                response.Data = await base_StationTaskLeakService.Add(entity, user);
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
                    var entity = await base_StationTaskLeakService.GetById(Id);
                    if (entity != null)
                    {
                        var user = Request.Cookies.Where(p => p.Key == "SET_NAME").First().Value.ToString();
                        await base_StationTaskLeakService.Delete(entity, user);
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
        public async Task<Response<List<Base_StationTaskLeak?>>> GetByTaskId(int taskid)
        {
            var response = new Response<List<Base_StationTaskLeak?>>();
            try
            {
                response.Result = await base_StationTaskLeakService.GetByTaskId(taskid);


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