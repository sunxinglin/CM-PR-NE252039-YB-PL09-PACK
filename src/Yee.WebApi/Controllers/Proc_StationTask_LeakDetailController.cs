using AsZero.Core.Services.Repos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Yee.Entitys.DBEntity;
using Yee.Entitys.DTOS;
using Yee.Entitys.Request;
using Yee.Services.ProductionRecord;
using Yee.Services.Request;

namespace Yee.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class Proc_StationTask_LeakDetailController : ControllerBase
    {
        private readonly Proc_StationTask_LeakDetailService _serivce;

        public Proc_StationTask_LeakDetailController(Proc_StationTask_LeakDetailService serivce)
        {
            _serivce = serivce;
        }
        /// <summary>
        /// 获取分页列表
        /// </summary>
        [HttpPost]
        public async Task<Response<List<Proc_StationTask_LeakDetail>>> Load(LeakInfoDTO input)
        {
            var result = new Response<List<Proc_StationTask_LeakDetail>>();
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
        /// 导出数据
        /// </summary>
        /// <param name="pack"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ModelExpornt(LeakInfoDTO dto)
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

        /// <summary>
        /// 根据主记录ID查询详情数据
        /// </summary>
        /// <param name="mainId">主记录ID</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<Response<List<Proc_StationTask_LeakDetail>>> LoadDetailByMainId(int mainId)
        {
            return await _serivce.LoadDetailByMainId(mainId);
        }

        /// <summary>
        /// 充气数据重传
        /// </summary>
        /// <param name="dto">重传参数</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<Response<string>> UploadLeakDataAgain(LeakReuploadDto dto)
        {
            var result = new Response<string>();
            try
            {
                // TODO: 实现充气数据重传逻辑
                // 1. 根据 packCode 和 stationCode 查询充气数据
                // 2. 调用 MES 接口上传数据

                result.Message = "充气数据重传功能待实现";
                result.Code = 200;
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