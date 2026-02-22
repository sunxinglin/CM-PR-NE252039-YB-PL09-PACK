using AsZero.Core.Entities;
using AsZero.Core.Services.Repos;
using AsZero.Core.Services.Sys_Logs;
using AsZero.DbContexts;

using Microsoft.EntityFrameworkCore;

using System.Threading.Tasks;

using Yee.Entitys.DBEntity.Production;
using Yee.Entitys.Production;

namespace Yee.Services.Production
{
    public class Base_StationTaskRecordTimeService
    {
        private readonly AsZeroDbContext _dbContext;
        private readonly SysLogService sys_LogService;

        public Base_StationTaskRecordTimeService(AsZeroDbContext dbContext, SysLogService sys_LogService)
        {
            _dbContext = dbContext;
            this.sys_LogService = sys_LogService;
        }
        public async Task<Response> AddConfig(Base_StationTask_RecordTime obj,  string op)
        {
            var resp = new Response();
            try
            {
                var res = await _dbContext.AddAsync(obj);

                await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.新增配方, Message = $"新增任务时间记录，任务名称:{obj.RecordTimeTaskName}", Operator = op });
                await _dbContext.SaveChangesAsync();
                return resp;
            }
            catch (Exception ex) 
            {
                resp.Code = 500;
                resp.Message = ex.Message;
                return resp;
            }
        }


        public async Task<Response> DeleteByTaskid(int taskid, string op)
        {
            var resp = new Response();
            try
            {
                var list = await _dbContext.Base_StationTask_RecordTimes.Where(d => d.IsDeleted == false && d.StationTaskId == taskid).ToListAsync();
                foreach (var item in list)
                {
                    item.IsDeleted = true;
                    await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.删除配方, Message = $"删除任务时间记录，任务名称:{item.RecordTimeTaskName}", Operator = op });
                }
                _dbContext.UpdateRange(list);
                await _dbContext.SaveChangesAsync();
                return resp;
            }
            catch (Exception ex)
            {
                resp.Code = 500;
                resp.Message = ex.Message;
                return resp;
            }
        }
        public async Task<Response> UpdateConfig(Base_StationTask_RecordTime entity, string op)
        {
            var resp = new Response();
            try
            {
                var resold = _dbContext.Base_StationTask_RecordTimes.Where(a => a.Id == entity.Id && !a.IsDeleted).AsNoTracking().FirstOrDefault();
                if(resold == null)
                {
                    resp.Code = 500;
                    resp.Message = $"未查询到任务详情";
                    return resp;
                }

                await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.修改配方, Message = $"修改前工任务时间记录，任务名称:{resold.RecordTimeTaskName}", Operator = op });
                var res = _dbContext.Base_StationTask_RecordTimes.Update(entity);
                await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.修改配方, Message = $"修改后任务时间记录，任务名称:{entity.RecordTimeTaskName}", Operator = op });
                await _dbContext.SaveChangesAsync();
                return resp;
            }
            catch (Exception ex)
            {
                resp.Code = 500;
                resp.Message = ex.Message;
                return resp;
            }
        }

        public async Task<Response> DeleteConfig(Base_StationTask_RecordTime entity, string op)
        {
            var resp = new Response();
            try
            {
                entity.IsDeleted = true;
                _dbContext.Base_StationTask_RecordTimes.Update(entity);
                await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.删除配方, Message = $"删除任务时间记录，物料名称:{entity.RecordTimeTaskName}", Operator = op });
                await _dbContext.SaveChangesAsync();
                return resp;
            }
            catch (Exception ex)
            {
                resp.Code = 500;
                resp.Message = ex.Message;
                return resp;
            }

        }

        public async Task<Response<IList<Base_StationTask_RecordTime>>> GetStationAnyloadTaskByTaskid(int stationtaskid)
        {
            var response = new Response<IList<Base_StationTask_RecordTime>>();
            try
            {
                var result = await this._dbContext.Base_StationTask_RecordTimes.Where(o => !o.IsDeleted && o.StationTaskId == stationtaskid).ToListAsync();
                response.Result = result;
                return response;
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Message = ex.Message;
                return response;
            }
        }
    }
}
