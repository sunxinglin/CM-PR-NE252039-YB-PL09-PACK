using AsZero.Core.Entities;
using AsZero.Core.Services.Sys_Logs;
using AsZero.DbContexts;

using Microsoft.EntityFrameworkCore;

using Yee.Entitys.DBEntity;
using Yee.Entitys.Production;

namespace Yee.Services.Production
{
    public class Base_StationTaskScanAccountCardService
    {
        private readonly AsZeroDbContext _dBContext;
        private readonly SysLogService sys_LogService;

        public Base_StationTaskScanAccountCardService(AsZeroDbContext dBContext,SysLogService sys_LogService)
        {
            this._dBContext = dBContext;
            this.sys_LogService = sys_LogService;
        }
        /// <summary>
        /// 通过ID查询
        /// </summary>
        public async Task<Base_StationTaskScanAccountCard?> GetById(int id)
        {
            var entity = _dBContext.Base_StationTaskScanAccountCards.FirstOrDefault(o=>!o.IsDeleted && o.Id==id);
            return entity;
        }

        public async Task<Base_StationTaskScanAccountCard> Add(Base_StationTaskScanAccountCard entity, string op)
        {
            var res = await _dBContext.Base_StationTaskScanAccountCards.AddAsync(entity);
           
            await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.新增配方, Message = $"新增工位扫描员工卡任务，物料名称:{entity.AccountCardValue}", Operator = op});
            await _dBContext.SaveChangesAsync();
            return res.Entity;
        }

        public async Task Delete(Base_StationTaskScanAccountCard entity,string op)
        {
            entity.IsDeleted = true;
            _dBContext.Base_StationTaskScanAccountCards.Update(entity);
            await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.删除配方, Message = $"删除工位扫描员工卡任务，物料名称:{entity.AccountCardValue}",Operator = op });
            await _dBContext.SaveChangesAsync();
           
        }
        public async Task DeleteByTaskid(int taskid,string op)
        {
            var list = await _dBContext.Base_StationTaskScanAccountCards.Where(d => d.IsDeleted == false && d.StationTaskId == taskid).ToListAsync();
            foreach (var item in list)
            {
                item.IsDeleted = true;
                await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.删除配方, Message = $"删除工位扫描员工卡任务，物料名称:{item.AccountCardValue}", Operator=op });
            }
            _dBContext.UpdateRange(list);
            await _dBContext.SaveChangesAsync();
        }
        public async Task<Base_StationTaskScanAccountCard> Update(Base_StationTaskScanAccountCard entity,string op)
        {
            var resold = _dBContext.Base_StationTaskScanAccountCards.Where(a => a.Id == entity.Id && !a.IsDeleted).AsNoTracking().FirstOrDefault();
            await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.修改配方, Message = $"修改前工位扫描员工卡任务，物料名称:{resold.AccountCardValue},上传代码:{resold.UpMesCode}", Operator = op });

            var res = _dBContext.Base_StationTaskScanAccountCards.Update(entity);
       
            await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.修改配方, Message = $"修改后工位扫描员工卡任务，物料名称:{entity.AccountCardValue},上传代码:{entity.UpMesCode}",Operator = op });
            await _dBContext.SaveChangesAsync();
            return res.Entity;
        }

        public async Task<List<Base_StationTaskScanAccountCard>> GetStationScanAccountCardTaskByTaskid(int stationtaskid)
        {
            return await this._dBContext.Base_StationTaskScanAccountCards.Where(o=>!o.IsDeleted && o.StationTaskId==stationtaskid).ToListAsync();
        }


        public async Task<List<Proc_StationTask_ScanAccountCard>> GetUserInputHis(int stationid, string PackPN)
        {
            return await this._dBContext.Proc_StationTask_ScanAccountCards.Where(o => !o.IsDeleted && o.StationId == stationid && o.PackPN == PackPN).ToListAsync();
        }
    }
}
