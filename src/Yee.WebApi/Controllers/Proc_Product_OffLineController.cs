using AsZero.Core.Services.Repos;
using Microsoft.AspNetCore.Mvc;

using Yee.Entitys.DBEntity.ProductionRecords;
using Yee.Entitys.DTOS;
using Yee.Entitys.Response;
using Yee.Services.Production;
using Yee.Services.Request;

namespace Yee.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class Proc_Product_OffLineController
    {
        private readonly Proc_Product_OffLineService proc_Product_In_StatisticsService;


        public Proc_Product_OffLineController(Proc_Product_OffLineService proc_Product_In_StatisticsService)
        {
            this.proc_Product_In_StatisticsService = proc_Product_In_StatisticsService;

        }
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<Response<Boolean>> Add(Proc_Product_Offline entity)
        {
            Response<Boolean> response = new Response<Boolean>();
            try
            {
                var result = await this.proc_Product_In_StatisticsService.Add(entity);
                response.Result = result.Success;
                response.Message = result.ResultMsg;
            }
            catch (Exception ex)
            {

                response.Code = 500;
                response.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return response;
        }
        /// <summary>
        /// 加载列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<Response<List<Proc_Product_Offline>>> Load([FromQuery] Prodct_StatisticsInput request)
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
        /// <summary>
        /// 获取一段时间内下线的产品包的饼状图数据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<Response<List<ChartModel>>> GetRoundCakeData(InStatisticsDto dto)
        {
            Response<List<ChartModel>> response = new Response<List<ChartModel>>();
            try
            {
                response.Result = await this.proc_Product_In_StatisticsService.GetRoundCakeData(dto);
            }
            catch (Exception ex)
            {

                response.Code = 500;
                response.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return response;
        }
        /// <summary>
        /// 获取一段时间内下线的产品包的折线图数据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<Response<List<ChartModel>>> GetBrokenLineData(InStatisticsDto dto)
        {
            Response<List<ChartModel>> response = new Response<List<ChartModel>>();
            try
            {
                response.Result = await this.proc_Product_In_StatisticsService.GetBrokenLineData(dto);
            }
            catch (Exception ex)
            {

                response.Code = 500;
                response.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return response;
        }
        /// <summary>
        /// 根据产品ID获取数据列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<Response<List<Proc_Product_Offline>>> GetListByProductId(InStatisticsDto dto)
        {
            Response<List<Proc_Product_Offline>> response = new Response<List<Proc_Product_Offline>>();
            try
            {
                var list = await this.proc_Product_In_StatisticsService.GetListByProductId(dto);
                response.Data = list.Skip((dto.Page.Value - 1) * dto.Limit.Value).Take(dto.Limit.Value);
                response.Count = list.Count;
            }
            catch (Exception ex)
            {

                response.Code = 500;
                response.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return response;
        }

        /// <summary>
        /// 查询产量
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<Response<OutPut>> GetOutPut()
        {
            Response<OutPut> response = new Response<OutPut>();
            try
            {
                response.Data = await this.proc_Product_In_StatisticsService.GetOutPut();
            }
            catch (Exception ex)
            {

                response.Code = 500;
                response.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return response;
        }

        /// <summary>
        /// 首页统计数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<Response<IndexStatisticsResponse>> GetIndexStatistics()
        {
            return await proc_Product_In_StatisticsService.GetIndexStatistics();
        }

    }
}
