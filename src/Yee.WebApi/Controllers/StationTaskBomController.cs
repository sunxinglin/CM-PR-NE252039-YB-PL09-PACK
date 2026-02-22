using AsZero.Core.Services.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Yee.Entitys.Production;
using Yee.Services.Production;

namespace Yee.WebApi.Controllers
{
    /// <summary>
    /// 工位任务bom
    /// </summary>
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class StationTaskBomController : ControllerBase
    {
        private readonly StationTaskBomService _stationBomService;

        public StationTaskBomController(StationTaskBomService stationBomService)
        {

            this._stationBomService = stationBomService;

        }

        /// <summary>
        /// 添加
        /// </summary>
        [HttpPost]
        public async Task<Response> Add(Base_StationTaskBom obj)
        {
            var result = new Response<Base_StationTaskBom>();
            try
            {
                if (!await _stationBomService.HasCode(obj.GoodsPN, obj.StationTaskId))
                {
                    //if (await _stationBomService.CheckTaskBom(obj.StationTaskId.Value)) 
                    //{
                    //    var newObj = await _stationBomService.Add(obj);
                    //    result.Result = newObj;
                    //}
                    //else
                    //{
                    //    result.Code = 500;
                    //    result.Message = "每个扫码任务只能配置一个物料！";
                    //}
                    var user = Request.Cookies.Where(p => p.Key == "SET_NAME").First().Value.ToString();
                    var newObj = await _stationBomService.Add(obj,user);
                    result.Result = newObj;
                }
                else
                {
                    result.Code = 500;
                    result.Message = "该编号已存在，请重新输入一个新的编码";
                }
  
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
        public async Task<Response> Update(Base_StationTaskBom obj)
        {
            var result = new Response<Base_StationTaskBom>();
            try
            {
                var user = Request.Cookies.Where(p => p.Key == "SET_NAME").First().Value.ToString();
                var res = await _stationBomService.Update(obj,user);
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
                foreach (var Id in input.Ids)
                {
                    var entity = await _stationBomService.GetById(Id);
                    if (entity != null)
                    {
                        await _stationBomService.Delete(entity);
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
        public async Task<Response<List<Base_StationTaskBom>>> LoadByTaskId(int taskid)
       {
             var result = new Response<List<Base_StationTaskBom>>();
            try {
                var list = await this._stationBomService.GetBomListByStationTask(taskid);
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
