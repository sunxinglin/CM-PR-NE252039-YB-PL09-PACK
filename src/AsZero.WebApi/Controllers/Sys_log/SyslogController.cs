using AsZero.Core.Entities;
using AsZero.Core.Services.Repos;
using AsZero.Core.Services.Sys_Logs;

using Microsoft.AspNetCore.Mvc;

namespace AsZero.WebApi.Controllers.Sys_log
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class SyslogController : Controller
    {
        private readonly SysLogService sys_LogService;

        public SyslogController(SysLogService sys_LogService)
        {
            this.sys_LogService = sys_LogService;
        }
        [HttpPost]
        public async Task<Response<List<SysLog>>> GetList(SyslogDto dto)
        {
            var result = new Response<List<SysLog>>();
            try
            {
                var list = await sys_LogService.GetList(dto);
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
     }
}
