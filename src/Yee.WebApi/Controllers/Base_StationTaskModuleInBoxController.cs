using AsZero.Core.Services.Repos;
using Microsoft.AspNetCore.Mvc;
using Yee.Entitys.Production;
using Yee.Services.Production;

namespace Yee.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class Base_StationTaskModuleInBoxController : Controller
    {
        private readonly Base_StationTaskModuleInBoxService _moduleInBoxService;

        public Base_StationTaskModuleInBoxController(Base_StationTaskModuleInBoxService moduleInBoxService)
        {
            this._moduleInBoxService = moduleInBoxService;
        }

        [HttpPost]
        public async Task<Response<Base_StationTask_AutoModuleInBox>> Update(Base_StationTask_AutoModuleInBox entity)
        {
            var response = new Response<Base_StationTask_AutoModuleInBox>();
            try
            {
                var user = Request.Cookies.Where(p => p.Key == "SET_NAME").First().Value.ToString();
                response.Data = await _moduleInBoxService.Update(entity, user);
            }
            catch (Exception ex)
            {

                response.Code = 500;
                response.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return response;

        }

        [HttpPost]
        public async Task<Response<Base_StationTask_AutoModuleInBox>> Add(Base_StationTask_AutoModuleInBox entity)
        {
            var response = new Response<Base_StationTask_AutoModuleInBox>();
            try
            {
                var user = Request.Cookies.Where(p => p.Key == "SET_NAME").First().Value.ToString();
                response.Data = await _moduleInBoxService.Add(entity, user);
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
                    var entity = await _moduleInBoxService.GetById(Id);
                    if (entity != null)
                    {
                        var user = Request.Cookies.Where(p => p.Key == "SET_NAME").First().Value.ToString();
                        await _moduleInBoxService.Delete(entity, user);
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
        public async Task<Response<List<Base_StationTask_AutoModuleInBox?>>> GetByTaskId(int taskid)
        {
            var response = new Response<List<Base_StationTask_AutoModuleInBox?>>();
            try
            {
                response.Result = await _moduleInBoxService.GetByTaskId(taskid);
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
