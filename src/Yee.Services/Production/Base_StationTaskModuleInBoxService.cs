using AsZero.Core.Entities;
using AsZero.Core.Services.Sys_Logs;
using AsZero.DbContexts;

using Microsoft.EntityFrameworkCore;

using Yee.Entitys.Production;

namespace Yee.Services.Production
{
    public class Base_StationTaskModuleInBoxService
    {
        private readonly AsZeroDbContext _dbContext;
        private readonly SysLogService sys_LogService;

        public Base_StationTaskModuleInBoxService(AsZeroDbContext dbContext, SysLogService sys_LogService)
        {
            this._dbContext = dbContext;
            this.sys_LogService = sys_LogService;
        }

        /// <summary>
        /// 增加
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<Base_StationTask_AutoModuleInBox> Add(Base_StationTask_AutoModuleInBox entity,string op)
        {
            var res = await _dbContext.AddAsync(entity);
            await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.新增配方, Message = $"新增模组入箱参数，参数名称:{entity.ParameterName}",Operator = op });
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
                await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.删除配方, Message = $"删除模组入箱参数，参数名称:{item.ParameterName}",Operator=op });
            }
           
           
            await _dbContext.SaveChangesAsync();
        }
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<Base_StationTask_AutoModuleInBox> Update(Base_StationTask_AutoModuleInBox entity,string op)
        {
            var resold = _dbContext.Base_StationTask_AutoModuleInBoxs.Where(a => a.Id == entity.Id && !a.IsDeleted).AsNoTracking().FirstOrDefault();
            await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.修改配方, Message = $"修改前模组入箱参数，参数名称:{resold.ParameterName},上传代码:{resold.UpMesCode}", Operator = op });

            var res = _dbContext.Update(entity);
            await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.修改配方, Message = $"修改后模组入箱参数，参数名称:{entity.ParameterName},上传代码:{entity.UpMesCode}",Operator = op });
            await _dbContext.SaveChangesAsync();
            return res.Entity;
        }

        public async Task Delete(Base_StationTask_AutoModuleInBox entity,string op)
        {
            entity.IsDeleted = true;
            _dbContext.Base_StationTask_AutoModuleInBoxs.Update(entity);
            await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.删除配方, Message = $"删除模组入箱参数，参数名称:{entity.ParameterName}", Operator = op});
            await _dbContext.SaveChangesAsync();
            
        }

        public async Task<List<Base_StationTask_AutoModuleInBox?>> GetByTaskId(int taskid)
        {
            var entity = await _dbContext.Base_StationTask_AutoModuleInBoxs.Where(o => o.StationTaskId == taskid && !o.IsDeleted).Include(o => o.StationTask).ToListAsync();
            return entity;
        }
        /// <summary>
        /// 通过ID查询
        /// </summary>
        public async Task<Base_StationTask_AutoModuleInBox?> GetById(int id)
        {
            var entity = _dbContext.Base_StationTask_AutoModuleInBoxs.FirstOrDefault(o => !o.IsDeleted && o.Id == id);
            return entity;
        }
    }
}
