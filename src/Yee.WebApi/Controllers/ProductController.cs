using AsZero.Core.Services.Repos;
using Microsoft.AspNetCore.Mvc;
using Yee.Entitys.Production;
using Yee.Services.Production;
using Yee.Services.Request;

namespace Yee.WebApi.Controllers.BaseData
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _productService;
        /// <summary>
        /// 构造函数
        /// </summary>
        public ProductController(ProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// 加载
        /// </summary>
        [HttpGet]
        public async Task<Response<List<Base_Product>>> Load([FromQuery] GetByKeyInput input)
        {
            var result = new Response<List<Base_Product>>();
            try
            {
                var list = await _productService.GetAll(input.Key);
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
        /// 关键字查询
        /// </summary>
        [HttpGet]
        public async Task<Response<List<Base_Product>>> GetList([FromQuery] GetByKeyInput input)
        {
            var result = new Response<List<Base_Product>>();
            try 
            { 
                result.Data = await _productService.GetAll(input.Key);
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        [HttpGet]
        public async Task<Response<List<Base_Product>>> LoadListForNoFlow()
        {
            var result = new Response<List<Base_Product>>();
            try
            {
                result.Data = await _productService.LoadListForNoFlow();
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
        public async Task<Response> Add(Base_Product obj)
        {
            var result = new Response<Base_Product>();
            try
            {
                var user = Request.Cookies.Where(p => p.Key == "SET_NAME").First().Value.ToString();
                var newObj = await _productService.Add(obj,user);
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
        public async Task<Response> Update(Base_Product obj)
        {
            var result = new Response<Base_Product>();
            try
            {
                var user = Request.Cookies.Where(p => p.Key == "SET_NAME").First().Value.ToString();
                var res = await _productService.Update(obj,user);
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
                    var entity = await _productService.GetById(Id);
                    if (entity != null)
                    {
                        await _productService.Delete(entity,user);
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


        /// <summary>
        /// 根据条码查找Pack
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<Response<Base_Product>> GetProdectByPackRule(string Code)
        {
            var result = new Response<Base_Product>();
            try
            {
                var respone = await _productService.GetProductByPackRule(Code);
                if(respone!= null)
                {
                    result.Code = respone.Code;
                    result.Message = respone?.Message;
                    result.Result = respone?.Result;
                }
            }
            catch(Exception ex)
            {
                result.Code=500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }
    }
}
