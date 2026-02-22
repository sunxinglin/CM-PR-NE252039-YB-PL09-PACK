using AsZero.Core.Services.Repos;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Yee.Entitys.Production;
using Yee.Services.Production;

namespace Yee.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class StationTaskScrewController : ControllerBase
    {
        private readonly StationTaskScrewService _stationTaskScrewService;
        public StationTaskScrewController(StationTaskScrewService stationTaskScrewService)
        {
            this._stationTaskScrewService = stationTaskScrewService;
        }


        /// <summary>
        /// 添加
        /// </summary>
        [HttpPost]
        public async Task<Response> Add(Base_StationTaskScrew obj)
        {
            var result = new Response<Base_StationTaskScrew>();
            try
            {
                    var user = Request.Cookies.Where(p => p.Key == "SET_NAME").First().Value.ToString();
                    var newObj = await _stationTaskScrewService.Add(obj, user);
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
        public async Task<Response> Update(Base_StationTaskScrew obj)
        {
            var result = new Response<Base_StationTaskScrew>();
            try
            {
                var user = Request.Cookies.Where(p => p.Key == "SET_NAME").First().Value.ToString();
                var res = await this._stationTaskScrewService.Update(obj, user);
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
                    var entity = await _stationTaskScrewService.GetById(Id);
                    if (entity != null)
                    {
                        await _stationTaskScrewService.Delete(entity, user);
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
                var list = await this._stationTaskScrewService.GetScrewListByStationTaskID(taskid);
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
