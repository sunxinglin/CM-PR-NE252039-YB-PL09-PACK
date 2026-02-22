using AsZero.Core.Services.Repos;
using Microsoft.AspNetCore.Mvc;
using Yee.Entitys.BaseData;
using Yee.Services.BaseData;
using Yee.Services.Request;
using Yee.WebApi.Models;

namespace Yee.WebApi.Controllers.BaseData
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UnitController : ControllerBase
    {
        private readonly UnitService __unitService;
        private readonly DictionaryService _dictionaryService;
        private readonly DictionaryDetailService _dictionaryDetailService;
        /// <summary>
        /// 构造函数
        /// </summary>
        public UnitController(UnitService unitService, DictionaryService dictionaryService, DictionaryDetailService dictionaryDetailService)
        {
            __unitService = unitService;
            _dictionaryService = dictionaryService;
            _dictionaryDetailService = dictionaryDetailService;
        }

        /// <summary>
        /// 加载
        /// </summary>
        [HttpGet]
        public async Task<Response<List<Unit>>> Load([FromQuery] GetByKeyInput input)
        {
            var result = new Response<List<Unit>>();
            try
            {
                var list = await __unitService.GetAll(input.Key);
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
        public async Task<Response> Add(Unit obj)
        {
            var result = new Response<Unit>();
            try
            {
                var newObj = await __unitService.Add(obj);
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
        public async Task<Response> Update(Unit obj)
        {
            var result = new Response<Unit>();
            try
            {
                var res = await __unitService.Update(obj);
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
                    var entity = await __unitService.GetById(Id);
                    if (entity != null)
                    {
                        await __unitService.Delete(entity);
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

        [HttpPost]
        public async Task<Response> AddUnitType(UnitTypeModel unitTypeModel)
        {
            var result = new Response<string>();
            try
            {
                DictionaryDetail dictionaryDetail = new DictionaryDetail();
                var dictionary = await _dictionaryService.GetByCode("UnitType");
                dictionaryDetail.DictionaryId = dictionary.Id;
                dictionaryDetail.Code = unitTypeModel.Code;
                dictionaryDetail.Name = unitTypeModel.Name;
                dictionaryDetail.Description = unitTypeModel.Description; 
                await _dictionaryDetailService.Add(dictionaryDetail);
                result.Message = "操作成功";
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        [HttpPost]
        public async Task<Response> UpdateUnitType(UnitTypeModel unitTypeModel)
        {
            var result = new Response<string>();
            try
            {
                DictionaryDetail detail = new DictionaryDetail();
                detail.Id = unitTypeModel.Id;
                detail.Code = unitTypeModel.Code;
                detail.Name = unitTypeModel.Name;
                detail.Description = unitTypeModel.Description;
                await _dictionaryDetailService.Update(detail);
                result.Message = "操作成功";
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }




        [HttpGet]
        public async Task<Response<List<DictionaryDetail>>> GetUnitType()
        {
            var result = new Response<List<DictionaryDetail>>();

            var dictionary = await _dictionaryService.GetByCode("UnitType");

            if(dictionary != null)
            {
                var list = await _dictionaryDetailService.GetAllByDictionaryId(dictionary.Id);
                result.Data = list;
            }
            return result;
        }

        [HttpGet]
        public async Task<Response<List<Unit>>> GetUnitByType(int unitTypeId)
        {
            var result = new Response<List<Unit>>();
            var list = await __unitService.GetUnitByType(unitTypeId);
            result.Data = list;
            return result;
        }


        //[HttpGet]
        //public async Task<Response<string>> GetUnitType()
        //{

        //}
    }
}
