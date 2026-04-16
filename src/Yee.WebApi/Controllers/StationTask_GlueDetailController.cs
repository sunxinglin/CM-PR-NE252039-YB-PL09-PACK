using AsZero.Core.Services.Repos;

using Microsoft.AspNetCore.Mvc;

using Yee.Entitys.DBEntity;
using Yee.Entitys.DTOS;
using Yee.Services.ProductionRecord;

namespace Yee.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class StationTask_GlueDetailController: ControllerBase
    {
        private readonly StationTask_GlueDetailService stationTask_GlueDetailService;
        //private readonly AutomicStationDataUpCatlAgainService _commonService;

        public StationTask_GlueDetailController(StationTask_GlueDetailService stationTask_BlotGunDetailService/*, AutomicStationDataUpCatlAgainService commonService*/)
        {
            this.stationTask_GlueDetailService = stationTask_BlotGunDetailService;
            //_commonService = commonService;
        }
    /// <summary>
        /// 获取涂胶详情
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<Response<List<Proc_GluingInfo>>> GetRealTimeGlueDetail(GlueDetailDto dto)
        {
            var result = new Response<List<Proc_GluingInfo>>();
            try
            {
                var list = await stationTask_GlueDetailService.GetRealTimeGlueDetail(dto);
                result.Data = list.Skip((dto.Page - 1) * dto.Limit).Take(dto.Limit);
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
                /// 删除涂胶数据和涂胶主记录
                /// </summary>
                /// <param name="dto"></param>
                /// <returns></returns>
                [HttpPost]
        public async Task<Response<int>> DeleteGluingInfo([FromBody]GluingInfoDeleteDTO dto)
        {
            return await stationTask_GlueDetailService.DeleteGluingInfo(dto);
        }
        /// <summary>
        /// 修改涂胶时间
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<Response<int>> UpGluingTime([FromBody] UpGluingDto dto)
        {
            return await stationTask_GlueDetailService.UpGluingInfo(dto);
        }
        /// <summary>
        /// 导出涂胶数据
        /// </summary>
        /// <param name="pack"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ModelExpornt(GlueDetailDto dto)
        {
            try
            {
                var memory = new MemoryStream();
                var result = await this.stationTask_GlueDetailService.ExportGluingInfo(dto);
                if (!result.Item1)
                {
                    return this.StatusCode(200, new { data = 208, message = result.Item2 });
                }

                using (var filestream = new FileStream(result.Item2, FileMode.Open))
                {
                    filestream.CopyTo(memory);
                }

                memory.Position = 0;
                this.Request.ContentType = "blob";
                return File(memory, "application/vnd.ms-excel", $"{DateTime.Now.Date.ToShortDateString()}Pack涂胶数据导出.xls");

            }
            catch (Exception)
            {

                throw;
            }
        }
        
    }
}


