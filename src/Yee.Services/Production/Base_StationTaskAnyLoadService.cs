using AsZero.Core.Entities;
using AsZero.Core.Services.Sys_Logs;
using AsZero.DbContexts;
using Microsoft.EntityFrameworkCore;
using Yee.Entitys.Production;

namespace Yee.Services.Production
{
    public class Base_StationTaskAnyLoadService
    {
        private readonly AsZeroDbContext _dBContext;
        private readonly SysLogService sys_LogService;

        public Base_StationTaskAnyLoadService(AsZeroDbContext dBContext,SysLogService sys_LogService)
        {
            this._dBContext = dBContext;
            this.sys_LogService = sys_LogService;
        }
        /// <summary>
        /// 通过ID查询
        /// </summary>
        public async Task<Base_StationTaskAnyLoad?> GetById(int id)
        {
            var entity = _dBContext.Base_StationTaskAnyLoads.FirstOrDefault(o=>!o.IsDeleted && o.Id==id);
            return entity;
        }

        public async Task<Base_StationTaskAnyLoad> Add(Base_StationTaskAnyLoad entity, string op)
        {
            var res = await _dBContext.Base_StationTaskAnyLoads.AddAsync(entity);
           
            await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.新增配方, Message = $"新增工位称重任务，物料名称:{entity.AnyLoadName}", Operator = op});
            await _dBContext.SaveChangesAsync();
            return res.Entity;
        }

        public async Task Delete(Base_StationTaskAnyLoad entity,string op)
        {
            entity.IsDeleted = true;
            _dBContext.Base_StationTaskAnyLoads.Update(entity);
            await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.删除配方, Message = $"删除工位称重任务，物料名称:{entity.AnyLoadName}",Operator = op });
            await _dBContext.SaveChangesAsync();
           
        }
        public async Task DeleteByTaskid(int taskid,string op)
        {
            var list = await _dBContext.Base_StationTaskAnyLoads.Where(d => d.IsDeleted == false && d.StationTaskId == taskid).ToListAsync();
            foreach (var item in list)
            {
                item.IsDeleted = true;
                await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.删除配方, Message = $"删除工位称重任务，物料名称:{item.AnyLoadName}", Operator=op });
            }
            _dBContext.UpdateRange(list);
            await _dBContext.SaveChangesAsync();
        }
        public async Task<Base_StationTaskAnyLoad> Update(Base_StationTaskAnyLoad entity,string op)
        {
            var resold = _dBContext.Base_StationTaskAnyLoads.Where(a => a.Id == entity.Id && !a.IsDeleted).AsNoTracking().FirstOrDefault();
            await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.修改配方, Message = $"修改前工位称重任务，物料名称:{resold.AnyLoadName},最大重量:{resold.MaxWeight},最小重量{resold.MinWeight}", Operator = op });

            var res = _dBContext.Base_StationTaskAnyLoads.Update(entity);
       
            await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.修改配方, Message = $"修改后工位称重任务，物料名称:{entity.AnyLoadName},最大重量:{entity.MaxWeight},最小重量{entity.MinWeight}",Operator = op });
            await _dBContext.SaveChangesAsync();
            return res.Entity;
        }

        public async Task<List<Base_StationTaskAnyLoad>> GetStationAnyloadTaskByTaskid(int stationtaskid)
        {
            return await this._dBContext.Base_StationTaskAnyLoads.Where(o=>!o.IsDeleted && o.StationTaskId==stationtaskid).ToListAsync();
        }
    }
}
