using AsZero.Core.Services.Repos;

using Microsoft.AspNetCore.Mvc;

using Yee.Entitys.DBEntity;
using Yee.Entitys.DBEntity.ProductionRecords;
using Yee.Entitys.DTOS;
using Yee.Services.ProductionRecord;

namespace Yee.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class Proc_ModuleInBoxRecordController : ControllerBase
    {
        private readonly Proc_ModuleInBoxRecordService _serivce;

        public Proc_ModuleInBoxRecordController(Proc_ModuleInBoxRecordService serivce)
        {
            _serivce = serivce;
        }
        /// <summary>
        /// 获取分页列表
        /// </summary>
        [HttpPost]
        public async Task<Response<List<Proc_ModuleInBox_ModuleRecord>>> Load(ModuleInBoxInfosDto input)
        {
            var result = new Response<List<Proc_ModuleInBox_ModuleRecord>>();
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
        /// Block入箱数据导出
        /// </summary>
        /// <param name="pack"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ModelExpornt(ModuleInBoxInfosDto dto)
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
                return File(memory, "application/vnd.ms-excel", $"{DateTime.Now.Date.ToShortDateString()}Block入箱数据导出.xls");

            }
            catch (Exception)
            {

                throw;
            }

        }

        [HttpGet]
        public async Task<Response<List<Proc_ModuleInBox_GrapRecord>>> GetGrapRecords([FromQuery] Proc_ModuleInBoxRecordService.GetModuleGrapRecordsRequest request)
        {
            var result = new Response<List<Proc_ModuleInBox_GrapRecord>>();
            try
            {
                var datas = await _serivce.GetModuleGrapRecords(request);
                result.Data = datas.Item1;
                result.Count = datas.Item2;
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }

            return result;
        }

        [HttpPost]
        public async Task<Response<int>> AssembleModule(string moduleCode, string packCode, bool hasUsed = true)
        {
            var result = new Response<int>();
            try
            {
                var datas = await _serivce.AssembleModule(moduleCode, packCode, hasUsed);
                result.Data = datas;
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }

            return result;
        }

        [HttpPost]
        public async Task<Response<int>> KickModule(string moduleCode, string? packCode, bool isDeleted = false)
        {
            var result = new Response<int>();
            try
            {
                var datas = await _serivce.KickModule(moduleCode, packCode, isDeleted);
                result.Data = datas;
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
