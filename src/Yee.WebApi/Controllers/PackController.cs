using AsZero.Core.Services.Repos;
using Microsoft.AspNetCore.Mvc;
using Yee.Entitys.Production;
using Yee.Services.Production;
using Yee.Services.Request;

namespace Yee.WebApi.Controllers.BaseData
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PackController : ControllerBase
    {
        private readonly PackService _packService;
        /// <summary>
        /// 构造函数
        /// </summary>
        public PackController(PackService packService)
        {
            _packService = packService;
        }

        /// <summary>
        /// 加载
        /// </summary>
        [HttpGet]
        public async Task<Response<List<Base_Pack>>> Load([FromQuery] GetByKeyInput input)
        {
            var result = new Response<List<Base_Pack>>();
            try
            {
                var list = await _packService.GetAll(input.Key);
                result.Data = list.Skip((input.Page - 1) * input.Limit).Take(input.Limit);
                result.Count = list.Count;
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 添加
        /// </summary>
        [HttpPost]
        public async Task<Response> Add(Base_Pack obj)
        {
            var result = new Response<Base_Pack?>();
            try
            {
                var newObj = await _packService.Add(obj);
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
        public async Task<Response> Update(Base_Pack obj)
        {
            var result = new Response<Base_Pack?>();
            try
            {
                var res = await _packService.Update(obj);
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
                    var entity = await _packService.GetById(Id);
                    if (entity != null)
                    {
                        await _packService.Delete(entity);
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
    }
}
