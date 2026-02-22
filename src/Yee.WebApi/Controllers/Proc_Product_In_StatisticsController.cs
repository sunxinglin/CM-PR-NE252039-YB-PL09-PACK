using AsZero.Core.Services.Repos;
using Microsoft.AspNetCore.Mvc;
using Yee.Entitys.DBEntity.ProductionRecords;
using Yee.Services.Production;
using Yee.Services.Request;

namespace Yee.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class Proc_Product_NG_StatisticsController
    {
        private readonly Proc_Product_NG_StatisticsService proc_Product_In_StatisticsService;
      

        public Proc_Product_NG_StatisticsController(Proc_Product_NG_StatisticsService proc_Product_In_StatisticsService)
        {
            this.proc_Product_In_StatisticsService = proc_Product_In_StatisticsService;
            
        }
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<Response< Boolean>> Add(Proc_Product_Offline entity)
        {
            Response<Boolean> response = new Response<Boolean>();
            try
            {
                response.Result = await this.proc_Product_In_StatisticsService.Add(entity);
            }
            catch (Exception ex)
            {

                response.Code = 500;
                response.Message = ex.Message;
            }
            return response;
        }
        /// <summary>
        /// 加载列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<Response< List<Proc_Product_Offline>>> Load([FromQuery]Prodct_StatisticsInput request)
        {
            Response<List<Proc_Product_Offline>> response = new Response<List<Proc_Product_Offline>>();
            try
            {
                var list = await this.proc_Product_In_StatisticsService.Load(request);
                response.Data = list.Skip((request.Page - 1) * request.Limit).Take(request.Limit);
                response.Count = list.Count;
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
