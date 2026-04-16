using AsZero.Core.Services.Repos;

using Microsoft.AspNetCore.Mvc;

using Yee.Entitys.BaseData;
using Yee.Services.BaseData;
using Yee.Services.Request;

namespace Yee.WebApi.Controllers.BaseData
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DictionaryController:ControllerBase
    {
        private readonly DictionaryService _dictionaryService;
        private readonly DictionaryDetailService _dictionaryDetailService;
        /// <summary>
        /// 构造函数
        /// </summary>
        public DictionaryController(DictionaryService dictionaryService,DictionaryDetailService dictionaryDetailService)
        {
            _dictionaryService = dictionaryService;
            _dictionaryDetailService = dictionaryDetailService;
        }

        /// <summary>
        /// 加载
        /// </summary>
        [HttpGet]
        public async Task<Response<List<Dictionary>>> Load([FromQuery]GetByKeyInput input)
        {
            var result = new Response<List<Dictionary>>();
            try
            {
                var list = await _dictionaryService.GetAll(input.Key);
                result.Data = list.Skip((input.Page - 1) * input.Limit).Take(input.Limit);
                result.Count=list.Count;
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 查询所有纪录
        /// </summary>
        [HttpGet]
        public async Task<Response<List<Dictionary>>> GetList([FromQuery] GetByKeyInput input)
        {
            var result = new Response<List<Dictionary>>();
            try
            {
                var list = await _dictionaryService.GetAll(input.Key);
                result.Data = list;
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
        /// 根据ID查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<Response<Dictionary>> Get([FromQuery] int id)
        {
            var result=new Response<Dictionary>();
            try
            {
                result.Data = await _dictionaryService.GetById(id);
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
        public async Task<Response> Add(Dictionary obj)
        {
            var result = new Response<Dictionary>();
            try
            {
                var newObj = await _dictionaryService.Add(obj);
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
        public async Task<Response> Update(Dictionary obj)
        {
            var result = new Response<Dictionary>();
            try
            {
                var res = await _dictionaryService.Update(obj);
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
                    var entity =await _dictionaryService.GetById(Id);
                    if (entity != null)
                    {
                        var entitys=await _dictionaryDetailService.GetByFK(entity.Id);
                        //判断子表中有无关联数据
                        if (entitys.Count==0)
                        {
                            await _dictionaryService.Delete(entity);
                        }
                        else
                        {
                            throw new Exception("详情表未解除关联");
                        }
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
