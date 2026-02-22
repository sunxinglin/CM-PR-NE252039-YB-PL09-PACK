using AsZero.Core.Entities;
using AsZero.Core.Services.Repos;
using AsZero.Core.Services.Sys_Logs;
using AsZero.DbContexts;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Yee.Entitys.AlarmMgmt;
using Yee.Entitys.DTOS;
using Yee.Services.AlarmMgmt;
using Yee.Services.Request;

namespace Yee.WebApi.Controllers.BaseData
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AlarmController : ControllerBase
    {
        private readonly ILogger<AlarmController> _logger;
        private readonly AlarmService _alarmService;
        private readonly RolesService _rolesService;
        private readonly SysLogService sysLogService;
        private readonly AsZeroDbContext dBContext;

        /// <summary>
        /// 构造函数
        /// </summary>
        public AlarmController(ILogger<AlarmController> logger, AlarmService alarmService, RolesService rolesService, SysLogService sysLogService, AsZeroDbContext dBContext)
        {
            _logger = logger;
            _alarmService = alarmService;
            _rolesService = rolesService;
            this.sysLogService = sysLogService;
            this.dBContext = dBContext;
        }

        [HttpGet]
        public async Task<Response<List<Alarm>>> GetAlarmsByStation([FromQuery] GetAlarmsByStationRequest input)
        {
            var result = new Response<List<Alarm>>();
            
            try
            {
                result.Result = await _alarmService.GetAlarmsByStation(input.StationName);
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        [HttpPost]
        public async Task<Response<List<Alarm>>> Occur(AlarmDTO dto)
        {
            var result = new Response<List<Alarm>>();
            try
            {
                _logger.LogWarning($"错误日志上报:{JsonConvert.SerializeObject(dto)}");
                result.Result = await _alarmService.Occur(dto);
            }
            catch(Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        [HttpPost]
        public async Task<Response<List<Alarm>>> Clear(AlarmDTO dto)
        {
            var result = new Response<List<Alarm>>();
            try
            {
                _logger.LogWarning($"错误日志上报(错误结束):{JsonConvert.SerializeObject(dto)}");
                result.Result = await _alarmService.Clear(dto);
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }
        [HttpPost]
        public async Task<Response<List<Alarm>>> ClearALL(List<AlarmDTO> dtos)
        {
            var result = new Response<List<Alarm>>();
            try
            {
                _logger.LogWarning($"错误日志上报(结束当前工位全部错误):{JsonConvert.SerializeObject(dtos)}");
                result.Result = await _alarmService.ClearALL(dtos);
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        [HttpPost]
        public async Task<Response<List<Alarm>>> GetList(AlarmDto dto)
        {
            var result = new Response<List<Alarm>>();
            try
            {
                var list = await _alarmService.GetList(dto);
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
        /// 验证产线管理权限
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<(Boolean, string)> CheckPower([FromQuery] string account, [FromQuery] string modlename)
        {

            var result = await this._rolesService.CheckPower(account, modlename);
            await sysLogService.AddLog(new SysLog() { LogType = Sys_LogType.用户登录, Message = $"用户权限验证:{account},权限：{modlename}通过", Operator = account });
            this.dBContext.SaveChanges();
            return result;
        }
    }
}
