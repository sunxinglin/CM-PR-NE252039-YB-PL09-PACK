//using AsZero.Core.Entities;
//using AsZero.Core.Services.Sys_Logs;
//using AsZero.DbContexts;
//using Microsoft.EntityFrameworkCore;
//using Yee.Entitys.Production;

//namespace Yee.Services.Production
//{
//    /// <summary>
//    /// 压条加压
//    /// </summary>
//    public class Base_StationTaskPressureStripService
//    {
//        private readonly AsZeroDbContext _dbContext;
//        private readonly SysLogService sys_LogService;

//        public Base_StationTaskPressureStripService(AsZeroDbContext dbContext, SysLogService sys_LogService)
//        {
//            this._dbContext = dbContext;
//            this.sys_LogService = sys_LogService;
//        }

//        /// <summary>
//        /// 增加
//        /// </summary>
//        /// <param name="entity"></param>
//        /// <returns></returns>
//        public async Task<Base_AutoStationTaskPressureStrip> Add(Base_AutoStationTaskPressureStrip entity,string op)
//        {
//            var res = await _dbContext.AddAsync(entity);
//            await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.新增配方, Message = $"新增压条加压参数，参数名称:{entity.ParameterName}",Operator = op });
//            await _dbContext.SaveChangesAsync();
//            return res.Entity;
//        }

        
        
//        public async Task DeleteByTaskID(int taskid, string op)
//        {
//            var entitys = await GetByTaskId(taskid);
//            foreach (var item in entitys)
//            {
//                item.IsDeleted = true;

//                _dbContext.UpdateRange(item);
//                await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.删除配方, Message = $"删除压条加压参数，参数名称:{item.ParameterName}",Operator=op });
//            }
           
           
//            await _dbContext.SaveChangesAsync();
//        }
//        /// <summary>
//        /// 修改
//        /// </summary>
//        /// <param name="entity"></param>
//        /// <returns></returns>
//        public async Task<Base_AutoStationTaskPressureStrip> Update(Base_AutoStationTaskPressureStrip entity,string op)
//        {
//            var resold = _dbContext.Base_StationTaskPressureStrips.Where(a => a.Id == entity.Id && !a.IsDeleted).AsNoTracking().FirstOrDefault();
//            await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.修改配方, Message = $"修改前压条加压参数，参数名称:{resold.ParameterName},上传代码:{resold.UpMesCode}", Operator = op });

//            var res = _dbContext.Update(entity);
//            await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.修改配方, Message = $"修改后压条加压参数，参数名称:{entity.ParameterName},上传代码:{entity.UpMesCode}",Operator = op });
//            await _dbContext.SaveChangesAsync();
//            return res.Entity;
//        }

//        public async Task Delete(Base_AutoStationTaskPressureStrip entity,string op)
//        {
//            entity.IsDeleted = true;
//            _dbContext.Base_StationTaskPressureStrips.Update(entity);
//            await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.删除配方, Message = $"删除压条加压参数，参数名称:{entity.ParameterName}", Operator = op});
//            await _dbContext.SaveChangesAsync();
            
//        }
//        /// <summary>
//        /// 通过工位ID查询
//        /// </summary>
//        public async Task<List<Base_AutoStationTaskPressureStrip>> GetStationTaskByStation(int stationID)
//        {
//            var list = await _dbContext.Base_StationTaskPressureStrips.Include(s => s.StationTask).Where(d => d.IsDeleted == false && d.StationTask.StepId == stationID).ToListAsync();
//            return list;
//        }
//        public async Task<List<Base_AutoStationTaskPressureStrip?>> GetByTaskId(int taskid)
//        {
//            var entity = await _dbContext.Base_StationTaskPressureStrips.Where(o => o.StationTaskId == taskid && !o.IsDeleted).Include(o => o.StationTask).Include(o => o.StationTask.Step).ToListAsync();
//            return entity;
//        }
//        /// <summary>
//        /// 通过ID查询
//        /// </summary>
//        public async Task<Base_AutoStationTaskPressureStrip?> GetById(int id)
//        {
//            var entity = _dbContext.Base_StationTaskPressureStrips.FirstOrDefault(o => !o.IsDeleted && o.Id == id);
//            return entity;
//        }
//    }
//}
