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
    public class StationTaskResourceController : ControllerBase
    {
        private readonly StationTaskResourceService _stationTaskResourceService;
        public StationTaskResourceController(StationTaskResourceService stationTaskResourceService)
        { 
            this._stationTaskResourceService=stationTaskResourceService;
        
        }


        ///// <summary>
        ///// 添加
        ///// </summary>
        //[HttpPost]
        //public async Task<Response> Add(Base_StationTaskResource obj)
        //{
        //    var result = new Response<Base_StationTaskResource>();
        //    try
        //    {
        //        var newObj = await this._stationTaskResourceService.Add(obj);
        //        result.Result = newObj;

        //    }
        //    catch (Exception ex)
        //    {
        //        result.Code = 500;
        //        result.Message = ex.InnerException?.Message ?? ex.Message;
        //    }

        //    return result;

        //}


        ///// <summary>
        ///// 修改
        ///// </summary>
        //[HttpPost]
        //public async Task<Response> Update(Base_StationTaskResource obj)
        //{
        //    var result = new Response<Base_StationTaskResource>();
        //    try
        //    {
        //        var res = await this._stationTaskResourceService.Update(obj);
        //        result.Result = res;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Code = 500;
        //        result.Message = ex.InnerException?.Message ?? ex.Message;
        //    }

        //    return result;
        //}

        ///// <summary>
        ///// 删除
        ///// </summary>
        //[HttpPost]
        //public async Task<Response<string>> DeleteEntity(DeleteByIdsInput input)
        //{
        //    var result = new Response<string>();
        //    try
        //    {
        //        foreach (var Id in input.Ids)
        //        {
        //            var entity = await _stationTaskResourceService.GetById(Id);
        //            if (entity != null)
        //            {
        //                await _stationTaskResourceService.Delete(entity);
        //            }
        //        }
        //        result.Message = "操作成功";
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Code = 500;
        //        result.Message = ex.InnerException?.Message ?? ex.Message;
        //    }

        //    return result;
        //}
    }
}
