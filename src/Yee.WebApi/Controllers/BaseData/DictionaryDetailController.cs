using AsZero.Core.Services.Repos;

using Microsoft.AspNetCore.Mvc;

using Yee.Entitys.BaseData;
using Yee.Services.BaseData;
using Yee.Services.Request;

namespace Yee.WebApi.Controllers.BaseData
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DictionaryDetailController : ControllerBase
    {
        private readonly DictionaryDetailService _dictionaryDetailService;

        /// <summary>
        /// 构造函数
        /// </summary>
        public DictionaryDetailController(DictionaryDetailService dictionaryDetailService)
        {
            _dictionaryDetailService = dictionaryDetailService;
        }
        /// <summary>
        /// 加载
        /// </summary>
        [HttpGet]
        public async Task<Response<List<DictionaryDetail>>> Load([FromQuery] GetByKeyInput input)
        {
            var result = new Response<List<DictionaryDetail>>();
            try
            {
                var list = await _dictionaryDetailService.GetAll(input.Key);
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
        /// 加载
        /// </summary>
        [HttpGet]
        public async Task<Response<List<DictionaryDetail>>> LoadDetail([FromQuery] GetByKeyInput input)
        {
            var result = new Response<List<DictionaryDetail>>();
            try
            {
                var list = await this._dictionaryDetailService.GetAllByDictionaryId(int.Parse( input.Key));
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
        public async Task<Response> Add(DictionaryDetail obj)
        {
            var result = new Response<DictionaryDetail>();
            try
            {
                var newObj = await _dictionaryDetailService.Add(obj);
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
        public async Task<Response> Update(DictionaryDetail obj)
        {
            var result = new Response<DictionaryDetail>();
            try
            {
                var res = await _dictionaryDetailService.Update(obj);
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
                    var entity = await _dictionaryDetailService.GetById(Id);
                    if (entity != null)
                        await _dictionaryDetailService.Delete(entity);
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
        public async Task<Response<List<DictionaryDetail>>> GetListByType([FromQuery] GetListByTypeInput input)
        {
            var result = new Response<List<DictionaryDetail>>();
            try
            {
                result.Data = await _dictionaryDetailService.GetListByType(input);
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
