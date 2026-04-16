using AsZero.Core.Services.Repos;

using Microsoft.AspNetCore.Mvc;

using Yee.Entitys.DBEntity;
using Yee.Entitys.DTOS;
using Yee.Services.ProductionRecord;

namespace Yee.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class StationTask_BlotGunDetailController : ControllerBase
    {
        private readonly StationTask_BlotGunDetailService stationTask_BlotGunDetailService;

        public StationTask_BlotGunDetailController(StationTask_BlotGunDetailService stationTask_BlotGunDetailService)
        {
            this.stationTask_BlotGunDetailService = stationTask_BlotGunDetailService;
        }
        /// <summary>
        /// 获取拧紧详情
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<Response<List<Proc_StationTask_BlotGunDetail>>> GetBlotGunDetail(BlotGunDetailDto dto)
        {
            var result = new Response<List<Proc_StationTask_BlotGunDetail>>();
            try
            {
                var (list, count) = await stationTask_BlotGunDetailService.GetBlotGunDetail(dto);
                result.Data = list;
                result.Count = count;
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        [HttpPost]
        public async Task<Response<List<Proc_AutoBoltInfo_Detail>>> GetAutoBlotGunDetail(BlotGunDetailDto dto)
        {
            var result = new Response<List<Proc_AutoBoltInfo_Detail>>();
            try
            {
                var (list, count) = await stationTask_BlotGunDetailService.GetAutoBlotGunDetail(dto);


                result.Data = list;
                result.Count = count;
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 文件下载接口
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet("{fileName}")]
        public IActionResult DownloadExcel(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return BadRequest();

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "ExportExcel", fileName);

            if (!System.IO.File.Exists(filePath)) return NotFound();

            try
            {
                var filestream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

                return File(filestream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 导出人工拧紧数据
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<Response<string>> BlotGunModelExpornt(BlotGunDetailDto dto)
        {
            try
            {
                var result = await this.stationTask_BlotGunDetailService.ExportBlotGunInfo(dto);
                if (!result.Item1)
                {
                    Response.StatusCode = 500;
                    return new Response<string>()
                    {
                        Code = 500,
                        Message = result.Item2
                    };
                }

                Response.StatusCode = 200;
                return new Response<string>()
                {
                    Code = 200,
                    Data = result.Item2,
                    Result = result.Item2
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 自动工位螺丝数据导出
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<Response<string>> AutoBlotGunModelExpornt(BlotGunDetailDto dto)
        {
            try
            {
                var result = await this.stationTask_BlotGunDetailService.ExportAutoBlotGunInfo(dto);
                if (!result.Item1)
                {
                    Response.StatusCode = 500;
                    return new Response<string>()
                    {
                        Code = 500,
                        Message = result.Item2
                    };
                }

                Response.StatusCode = 200;
                return new Response<string>()
                {
                    Code = 200,
                    Data = result.Item2,
                    Result = result.Item2
                };
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
