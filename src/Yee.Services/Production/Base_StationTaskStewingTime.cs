using AsZero.Core.Entities;
using AsZero.Core.Services.Sys_Logs;
using AsZero.DbContexts;

using Microsoft.EntityFrameworkCore;

using Yee.Entitys.Production;

namespace Yee.Services.Production
{
    public class Base_StationTaskStewingTimeService
    {
        private readonly AsZeroDbContext _dbContext;
        private readonly SysLogService sys_LogService;

        public Base_StationTaskStewingTimeService(AsZeroDbContext dbContext, SysLogService sys_LogService)
        {
            this._dbContext = dbContext;
            this.sys_LogService = sys_LogService;
        }

        /// <summary>
        /// 增加
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<Base_StationTaskStewingTime> Add(Base_StationTaskStewingTime entity,string op)
        {
            var res = await _dbContext.AddAsync(entity);
            await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.新增配方, Message = $"新增静置时长参数，参数名称:{entity.TaskStewingTimedName}",Operator = op });
            await _dbContext.SaveChangesAsync();
            return res.Entity;
        }

        
        
        public async Task DeleteByTaskID(int taskid, string op)
        {
            var entitys = await GetByTaskId(taskid);
            foreach (var item in entitys)
            {
                item.IsDeleted = true;

                _dbContext.UpdateRange(item);
                await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.删除配方, Message = $"删除静置时长参数，参数名称:{item.TaskStewingTimedName}",Operator=op });
            }
           
           
            await _dbContext.SaveChangesAsync();
        }
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<Base_StationTaskStewingTime> Update(Base_StationTaskStewingTime entity,string op)
        {
            var resold = _dbContext.Base_StationTaskStewingTimes.Where(a => a.Id == entity.Id && !a.IsDeleted).AsNoTracking().FirstOrDefault();
            await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.修改配方, Message = $"修改前静置时长参数，参数名称:{resold.TaskStewingTimedName},上传代码:{resold.UpMesCode}", Operator = op });

            var res = _dbContext.Update(entity);
            await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.修改配方, Message = $"修改后静置时长参数，参数名称:{entity.TaskStewingTimedName},上传代码:{entity.UpMesCode}",Operator = op });
            await _dbContext.SaveChangesAsync();
            return res.Entity;
        }

        public async Task Delete(Base_StationTaskStewingTime entity,string op)
        {
            entity.IsDeleted = true;
            _dbContext.Base_StationTaskStewingTimes.Update(entity);
            await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.删除配方, Message = $"删除静置时长参数，参数名称:{entity.TaskStewingTimedName}", Operator = op});
            await _dbContext.SaveChangesAsync();
            
        }
        /// <summary>
        /// 通过工位ID查询
        /// </summary>
        public async Task<List<Base_StationTaskStewingTime>> GetStationTaskByStation(int stationID)
        {
            var list = await _dbContext.Base_StationTaskStewingTimes.Include(s => s.StationTask).Where(d => d.IsDeleted == false && d.StationTask.StepId == stationID).ToListAsync();
            return list;
        }
        public async Task<List<Base_StationTaskStewingTime?>> GetByTaskId(int taskid)
        {
            var entity = await _dbContext.Base_StationTaskStewingTimes.Where(o => o.StationTaskId == taskid && !o.IsDeleted).Include(o => o.StationTask).Include(o => o.StationTask.Step).ToListAsync();
            return entity;
        }
        /// <summary>
        /// 通过ID查询
        /// </summary>
        public async Task<Base_StationTaskStewingTime?> GetById(int id)
        {
            var entity = _dbContext.Base_StationTaskStewingTimes.FirstOrDefault(o => !o.IsDeleted && o.Id == id);
            return entity;
        }
    }
}
