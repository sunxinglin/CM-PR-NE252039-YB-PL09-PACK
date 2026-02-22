using AsZero.Core.Services.Repos;
using Microsoft.AspNetCore.Mvc;
using Yee.Entitys.Production;
using Yee.Services.Production;

namespace Yee.WebApi.Controllers
{
    
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class StationTaskMaxTimeController : Controller
    {
        private readonly Base_StationTaskGluingTimeService stationTaskGluingTimeService;

        public StationTaskMaxTimeController(Base_StationTaskGluingTimeService stationTaskGluingTimeService)
        {
            this.stationTaskGluingTimeService = stationTaskGluingTimeService;
        }
        /// <summary>
        /// 添加
        /// </summary>
        [HttpPost]
        public async Task<Response> Add(Base_StationTaskCheckTimeOut obj)
        {
            var result = new Response<Base_StationTaskCheckTimeOut>();
            try
            {
                var user = Request.Cookies.Where(p => p.Key == "SET_NAME").First().Value.ToString();
                var newObj = await stationTaskGluingTimeService.Add(obj,user);
                result.Result = newObj;
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }

            return result;

        }


        /// <summary>
        /// 修改
        /// </summary>
        [HttpPost]
        public async Task<Response> Update(Base_StationTaskCheckTimeOut obj)
        {
            var result = new Response<Base_StationTaskCheckTimeOut>();
            try
            {
                var user = Request.Cookies.Where(p => p.Key == "SET_NAME").First().Value.ToString();
                var res = await this.stationTaskGluingTimeService.Update(obj,user);
                result.Result = res;
                
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }

            return result;
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
                var user = Request.Cookies.Where(p => p.Key == "SET_NAME").First().Value.ToString();
                foreach (var Id in input.Ids)
                {
                    var entity = await stationTaskGluingTimeService.GetById(Id);
                    if (entity != null)
                    {
                        await stationTaskGluingTimeService.Delete(entity,user);
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
        public async Task<Response<List<Base_StationTaskScrew>>> LoadByTaskId(int taskid)
        {
            var result = new Response<List<Base_StationTaskScrew>>();
            try
            {
                var list = await this.stationTaskGluingTimeService.GetStationAnyloadTaskByTaskid(taskid);
                result.Data = list;
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
