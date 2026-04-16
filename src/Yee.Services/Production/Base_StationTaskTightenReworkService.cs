using AsZero.Core.Entities;
using AsZero.Core.Services.Sys_Logs;
using AsZero.DbContexts;

using Microsoft.EntityFrameworkCore;

using Yee.Entitys.DBEntity.Production;

namespace Yee.Services.Production
{
    public class Base_StationTaskTightenReworkService
    {
        private readonly AsZeroDbContext _dbContext;
        private readonly SysLogService sys_LogService;

        public Base_StationTaskTightenReworkService(AsZeroDbContext dbContext, SysLogService sys_LogService)
        {
            _dbContext = dbContext;
            this.sys_LogService = sys_LogService;
        }

        public async Task<Base_StationTask_TightenRework> Add(Base_StationTask_TightenRework entity, string op)
        {
            var res = await _dbContext.AddAsync(entity);

            await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.新增配方, Message = $"新增工位补拧任务，任务名称:{entity.TaskName}", Operator = op });
            await _dbContext.SaveChangesAsync();
            return res.Entity;
        }

        /// <summary>
        /// 通过关键字查询
        /// </summary>
        public async Task<List<Base_StationTask_TightenRework>> GetScrewListByStationTaskID(int stationTaskID)
        {
            var list = await _dbContext.Base_StationTask_TightenReworks.Where(d => d.IsDeleted == false && d.StationTaskId == stationTaskID).ToListAsync();
            return list;
        }


        public async Task Delete(Base_StationTask_TightenRework entity, string op)
        {
            entity.IsDeleted = true;
            _dbContext.Update(entity);
            await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.删除配方, Message = $"删除工位补拧任务，任务名称:{entity.TaskName}", Operator = op });
            await _dbContext.SaveChangesAsync();
        }


        public async Task<Base_StationTask_TightenRework> Update(Base_StationTask_TightenRework entity, string op)
        {
            var resold = _dbContext.Base_StationTask_TightenReworks.Where(a => !a.IsDeleted && a.Id == entity.Id).AsNoTracking().FirstOrDefault();
            await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.修改配方, Message = $"修改前工位拧紧枪任务，任务名称:{resold.TaskName},程序号:{resold.ProgramNo},螺栓总量:{resold.ScrewNum},上传代码{resold.UpMesCode}", Operator = op });
            _dbContext.Update(entity);
            await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.修改配方, Message = $"修改前工位拧紧枪任务，任务名称:{resold.TaskName},程序号:{resold.ProgramNo},螺栓总量:{resold.ScrewNum},上传代码{resold.UpMesCode}", Operator = op });
            await _dbContext.SaveChangesAsync();

            return entity;
        }

        /// <summary>
        /// 通过ID查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Base_StationTask_TightenRework> GetById(int id)
        {
            var entity = await _dbContext.Base_StationTask_TightenReworks.FindAsync(id);
            return entity;
        }
    }
}
