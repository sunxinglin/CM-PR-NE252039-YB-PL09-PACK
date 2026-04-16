using AsZero.Core.Services.Repos;

using Microsoft.AspNetCore.Mvc;

using Yee.Entitys.DBEntity;
using Yee.Entitys.DTOS;
using Yee.Services.ProductionRecord;

namespace Yee.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class Proc_PressureInfosController : ControllerBase
    {
        private readonly Proc_PressureInfosService _serivce;

        public Proc_PressureInfosController(Proc_PressureInfosService serivce)
        {
            _serivce = serivce;
        }
        /// <summary>
        /// 获取分页列表
        /// </summary>
        [HttpPost]
        public async Task<Response<List<Proc_PressureInfo>>> Load(PressureInfosDto input)
        {
            var result = new Response<List<Proc_PressureInfo>>();
            try
            {
                var list = await _serivce.GetPageList(input);
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
        /// 导出Pack加压数据
        /// </summary>
        /// <param name="pack"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ModelExpornt(PressureInfosDto dto)
        {
            try
            {
                var memory = new MemoryStream();
                var result = await this._serivce.ExportGluingInfo(dto);
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
                return File(memory, "application/vnd.ms-excel", $"{DateTime.Now.Date.ToShortDateString()}Pack加压数据导出.xls");

            }
            catch (Exception)
            {

                throw;
            }

        }

    }
}
