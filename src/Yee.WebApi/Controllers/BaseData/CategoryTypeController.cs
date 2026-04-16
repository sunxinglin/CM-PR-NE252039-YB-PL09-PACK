using AsZero.Core.Services.Repos;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Yee.Entitys.BaseData;
using Yee.Services.BaseData;

namespace Yee.WebApi.Controllers.BaseData
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class CategoryTypeController : ControllerBase
    {
        private readonly CategoryTypeService _categoryTypeService;

        /// <summary>
        /// 构造函数
        /// </summary>
        public CategoryTypeController(CategoryTypeService categoryTypeService)
        {
            _categoryTypeService = categoryTypeService;
        }

        /// <summary>
        /// 添加
        /// </summary>
        [HttpPost]
        public async Task<Response> Add(CategoryType obj)
        {
            var result = new Response<CategoryType>();
            try
            {
                var newObj = await _categoryTypeService.Add(obj);
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
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<Response<Boolean>> Update(CategoryType obj)
        {
            var result = new Response<Boolean>();
            try
            {
                CategoryType res = new CategoryType();
                var entity = _categoryTypeService.GetById(obj.Id);
                if (entity != null)
                {
                   res= await _categoryTypeService.Update(obj);
                   
                   result.Result = true;
                }
                else
                {
                    result.Message = $"修改的数据不存在,请刷新后重试!";
                    result.Result=false;
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
                    var entity = _categoryTypeService.GetById(Id);
                    if (entity != null)
                        await _categoryTypeService.Delete(entity);
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
        /// <summary>
        /// 加载全部类型列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<Response< List<CategoryType>>> LordList()
        {
            var response = new Response<List<CategoryType>>();
            try
            {
               var data= await _categoryTypeService.LordList();
                response.Data = data;
                response.Count = data.Count;
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
