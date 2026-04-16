using AsZero.Core.Services.Repos;

using Microsoft.AspNetCore.Mvc;

using Yee.Entitys.DBEntity.Production;
using Yee.Services.Production;

namespace Yee.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProductTraceBackController : ControllerBase
    {
        private readonly ProductTraceBackService productTraceBackService;

        public ProductTraceBackController(ProductTraceBackService productTraceBackService)
        {
            this.productTraceBackService = productTraceBackService;
        }
        /// <summary>
        /// 获取正向追溯数据
        /// </summary>
        /// <param name="code">pack条码</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<Response<ForwardTrace?>> GetForward([FromQuery] string code)
        {
            Response<ForwardTrace?> response = new Response<ForwardTrace?>();
            try
            {
                var list = await productTraceBackService.GetForward(code);
                if (list.StationTask_Mains_Realtime == null)
                {
                    var list_His = await productTraceBackService.GetForward(code);
                    if (list_His.StationTask_Mains_Realtime != null)
                    {
                        response.Data = list_His.StationTask_Mains_Realtime;
                    }
                }
                else
                {
                    response.Data = list.StationTask_Mains_Realtime;

                }
            }
            catch (Exception ex)
            {

                response.Code = 500;
                response.Message = ex.InnerException?.Message ?? ex.Message;

            }
            return response;
        }

        [HttpGet]
        public async Task<IActionResult> ModelExpornt(string pack)
        {
            try
            {
                var memory = new MemoryStream();

                var result = await this.productTraceBackService.ExportStationTask(pack);
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
                return File(memory, "application/vnd.ms-excel", $"{DateTime.Now.Date.ToShortDateString()}Pack数据导出.xlsx");

            }
            catch (Exception)
            {

                throw;
            }

        }





    }
}
