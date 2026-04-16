using AsZero.Core.Services.Repos;

using Microsoft.AspNetCore.Mvc;

using Yee.Entitys.DBEntity;
using Yee.Entitys.DTOS;
using Yee.Services.ProductionRecord;

namespace Yee.WebApi.Controllers
{
    /// <summary>
    /// 工位任务Bom Detail
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class StationTaskBomDetailController : ControllerBase
    {
        private readonly StationTask_BomDetailService _stationBomDetailService;

        public StationTaskBomDetailController(StationTask_BomDetailService stationBomDetailService)
        {
            _stationBomDetailService = stationBomDetailService;
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="packPN"></param>
        /// <param name="outerCode"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<Response<List<Proc_StationTask_BomDetail>>> GetPageList([FromQuery] StationTaskBomDetailGetListDTO dto)
        {
            var result = new Response<List<Proc_StationTask_BomDetail>>();
            try
            {
                var list = await this._stationBomDetailService.GetPageList(dto);
          
                    result.Data = list.Data;
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
        /// 组装数据导出
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ModelExpornt(StationTaskBomDetailGetListDTO dto)
        {
            try
            {
                var memory = new MemoryStream();

                var result = await this._stationBomDetailService.ExportBomInfo(dto);
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
                return File(memory, "application/vnd.ms-excel", $"{DateTime.Now.Date.ToShortDateString()}组装数据导出.xls");

            }
            catch (Exception)
            {
                throw;
            }

        }
    }
}
