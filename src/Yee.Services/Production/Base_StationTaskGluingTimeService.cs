using AsZero.Core.Entities;
using AsZero.Core.Services.Sys_Logs;
using AsZero.DbContexts;
using Microsoft.EntityFrameworkCore;
using Yee.Entitys.Production;

namespace Yee.Services.Production
{
    public class Base_StationTaskGluingTimeService
    {
        private readonly AsZeroDbContext _dBContext;
        private readonly SysLogService sys_LogService;

        public Base_StationTaskGluingTimeService(AsZeroDbContext dBContext,SysLogService sys_LogService)
        {
            this._dBContext = dBContext;
            this.sys_LogService = sys_LogService;
        }
        /// <summary>
        /// 通过ID查询
        /// </summary>
        public async Task<Base_StationTaskCheckTimeOut?> GetById(int id)
        {
            var entity = _dBContext.Base_StationTaskCheckTimeOuts.FirstOrDefault(o=>!o.IsDeleted && o.Id==id);
            return entity;
        }

        public async Task<Base_StationTaskCheckTimeOut> Add(Base_StationTaskCheckTimeOut entity,string op)
        {
            var res = await _dBContext.Base_StationTaskCheckTimeOuts.AddAsync(entity);
           
            await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.新增配方, Message = $"新增工位涂胶超时检测任务，{entity.TimeOutTaskName}",Operator = op });
            await _dBContext.SaveChangesAsync();
            return res.Entity;
        }

        public async Task Delete(Base_StationTaskCheckTimeOut entity,string op)
        {
            entity.IsDeleted = true;
            _dBContext.Base_StationTaskCheckTimeOuts.Update(entity);
            await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.删除配方, Message = $"删除工位涂胶超时检测任务，{entity.TimeOutTaskName}", Operator = op });
            await _dBContext.SaveChangesAsync();
           
        }
        public async Task DeleteByTaskid(int taskid,string op)
        {
            var list = await _dBContext.Base_StationTaskCheckTimeOuts.Where(d => d.IsDeleted == false && d.StationTaskId == taskid).ToListAsync();
            foreach (var item in list)
            {
                item.IsDeleted = true;
                await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.删除配方, Message = $"删除工位涂胶超时检测任务，{item.TimeOutTaskName}",Operator = op });
            }
            _dBContext.UpdateRange(list);
            await _dBContext.SaveChangesAsync();
        }
        public async Task<Base_StationTaskCheckTimeOut> Update(Base_StationTaskCheckTimeOut entity,string op)
        {
            var resold = _dBContext.Base_StationTaskCheckTimeOuts.Where(a => a.Id == entity.Id && !a.IsDeleted).AsNoTracking().FirstOrDefault();
            await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.修改配方, Message = $"修改前工位涂胶超时检测任务，{resold.TimeOutTaskName},最大时间:{resold.MaxDuration},上传代码:{resold.UpMesCode}", Operator = op });

            var res = _dBContext.Base_StationTaskCheckTimeOuts.Update(entity);
       
            await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.修改配方, Message = $"修改后工位涂胶超时检测任务，{entity.TimeOutTaskName},最大时间:{resold.MaxDuration},上传代码:{resold.UpMesCode}", Operator = op });
            await _dBContext.SaveChangesAsync();
            return res.Entity;
        }

        public async Task<List<Base_StationTaskCheckTimeOut>> GetStationAnyloadTaskByTaskid(int stationtaskid)
        {
            return await this._dBContext.Base_StationTaskCheckTimeOuts.Where(o=>!o.IsDeleted && o.StationTaskId==stationtaskid).ToListAsync();
        }
    }
}
