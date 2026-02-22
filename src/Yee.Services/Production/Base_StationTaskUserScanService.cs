using AsZero.Core.Entities;
using AsZero.Core.Services.Sys_Logs;
using AsZero.DbContexts;
using Microsoft.EntityFrameworkCore;
using Yee.Entitys.DBEntity;
using Yee.Entitys.Production;

namespace Yee.Services.Production
{
    public class Base_StationTaskUserInputService
    {
        private readonly AsZeroDbContext _dBContext;
        private readonly SysLogService sys_LogService;

        public Base_StationTaskUserInputService(AsZeroDbContext dBContext,SysLogService sys_LogService)
        {
            this._dBContext = dBContext;
            this.sys_LogService = sys_LogService;
        }
        /// <summary>
        /// 通过ID查询
        /// </summary>
        public async Task<Base_StationTaskUserInput?> GetById(int id)
        {
            var entity = _dBContext.Base_StationTaskUserInputs.FirstOrDefault(o=>!o.IsDeleted && o.Id==id);
            return entity;
        }

        public async Task<Base_StationTaskUserInput> Add(Base_StationTaskUserInput entity, string op)
        {
            var res = await _dBContext.Base_StationTaskUserInputs.AddAsync(entity);
           
            await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.新增配方, Message = $"新增工位用户输入任务，物料名称:{entity.UserScanName}", Operator = op});
            await _dBContext.SaveChangesAsync();
            return res.Entity;
        }

        public async Task Delete(Base_StationTaskUserInput entity,string op)
        {
            entity.IsDeleted = true;
            _dBContext.Base_StationTaskUserInputs.Update(entity);
            await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.删除配方, Message = $"删除工位用户输入任务，物料名称:{entity.UserScanName}",Operator = op });
            await _dBContext.SaveChangesAsync();
           
        }
        public async Task DeleteByTaskid(int taskid,string op)
        {
            var list = await _dBContext.Base_StationTaskUserInputs.Where(d => d.IsDeleted == false && d.StationTaskId == taskid).ToListAsync();
            foreach (var item in list)
            {
                item.IsDeleted = true;
                await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.删除配方, Message = $"删除工位用户输入任务，物料名称:{item.UserScanName}", Operator=op });
            }
            _dBContext.UpdateRange(list);
            await _dBContext.SaveChangesAsync();
        }
        public async Task<Base_StationTaskUserInput> Update(Base_StationTaskUserInput entity,string op)
        {
            var resold = _dBContext.Base_StationTaskUserInputs.Where(a => a.Id == entity.Id && !a.IsDeleted).AsNoTracking().FirstOrDefault();
            await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.修改配方, Message = $"修改前工位用户输入任务，物料名称:{resold.UserScanName},上传代码:{resold.UpMesCode},最大值:{resold.MaxRange},最小值:{resold.MinRange}", Operator = op });

            var res = _dBContext.Base_StationTaskUserInputs.Update(entity);
       
            await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.修改配方, Message = $"修改后工位用户输入任务，物料名称:{entity.UserScanName},上传代码:{entity.UpMesCode},最大值:{entity.MaxRange},最小值:{entity.MinRange}",Operator = op });
            //await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.修改配方, Message = $"修改工位用户输入任务，物料名称:{entity.UserScanName}",Operator = op });
            await _dBContext.SaveChangesAsync();
            return res.Entity;
        }

        public async Task<List<Base_StationTaskUserInput>> GetStationUserScanTaskByTaskid(int stationtaskid)
        {
            return await this._dBContext.Base_StationTaskUserInputs.Where(o=>!o.IsDeleted && o.StationTaskId==stationtaskid).ToListAsync();
        }


        public async Task<List<Proc_StationTask_UserInput>> GetUserInputHis(int stationid, string PackPN)
        {
            return await this._dBContext.Proc_StationTask_UserInputs.Where(o => !o.IsDeleted && o.StationId == stationid && o.PackPN == PackPN).ToListAsync();
        }
    }
}
