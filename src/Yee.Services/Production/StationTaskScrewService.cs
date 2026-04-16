using AsZero.Core.Entities;
using AsZero.Core.Services.Sys_Logs;
using AsZero.DbContexts;

using Microsoft.EntityFrameworkCore;

using Yee.Entitys.Production;

namespace Yee.Services.Production
{
    public class StationTaskScrewService
    {
        public readonly AsZeroDbContext _dBContext;
        private readonly SysLogService sys_LogService;

        public StationTaskScrewService(AsZeroDbContext dBContext, SysLogService sys_LogService)
        {
            this._dBContext = dBContext;
            this.sys_LogService = sys_LogService;
        }

        public async Task<Base_StationTaskScrew> Add(Base_StationTaskScrew entity, string op)
        {
            var res = await _dBContext.AddAsync(entity);
          
            await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.新增配方, Message = $"新增工位拧紧枪任务，螺丝规格:{entity.ScrewSpecs}",Operator = op });
            await _dBContext.SaveChangesAsync();
            return res.Entity;
        }

        /// <summary>
        /// 通过关键字查询
        /// </summary>
        public async Task<List<Base_StationTaskScrew>> GetScrewListByStationTaskID(int stationTaskID)
        {
            var list = await _dBContext.Base_StationTaskScrews.Where(d => d.IsDeleted == false && d.StationTaskId == stationTaskID).ToListAsync();
            foreach (var item in list)
            {
                item.DeviceNoList = item.DeviceNos.Split(",").ToList();
            }
            return list;
        }


        public async Task Delete(Base_StationTaskScrew entity,string op)
        {
            entity.IsDeleted = true;
            _dBContext.Update(entity);
            await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.删除配方, Message = $"删除工位拧紧枪任务，螺丝规格:{entity.ScrewSpecs}",Operator = op });
            await _dBContext.SaveChangesAsync();
        }
        public async Task DeleteByTaskid(int taskid,string op)
        {
            var list = await _dBContext.Base_StationTaskScrews.Where(d => d.IsDeleted == false && d.StationTaskId == taskid).ToListAsync();
            foreach (var item in list)
            {
                item.IsDeleted = true;
                await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.删除配方, Message = $"删除工位拧紧枪任务，螺丝规格:{item.ScrewSpecs}",Operator = op });
            }
            _dBContext.UpdateRange(list);
            await _dBContext.SaveChangesAsync();
        }
        public async Task<bool> Check(Base_StationTaskScrew obj)
        {
            //foreach (var devNo in obj.DeviceNoList)
            //{
            //    var listHasThisDev = await _dBContext.Base_StationTaskScrews.Where(d => d.IsDeleted == false
            //        && d.StationTaskId == obj.StationTaskId
            //        && d.Id != obj.Id
            //        && d.DeviceNos.Contains(devNo)).Select(d => d.ProgramNo).Distinct().ToListAsync();
            //    if (listHasThisDev.Count > 0 && (listHasThisDev.Count > 1 || listHasThisDev[0].Value != obj.ProgramNo))
            //        return false;
            //}
            return true;
        }

        public async Task<Base_StationTaskScrew> Update(Base_StationTaskScrew entity,string op)
        {
            var resold =  _dBContext.Base_StationTaskScrews.Where(a=>!a.IsDeleted && a.Id==entity.Id).AsNoTracking().FirstOrDefault();
            await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.修改配方, Message = $"修改前工位拧紧枪任务，螺丝规格:{resold.ScrewSpecs},程序号:{resold.ProgramNo},使用次数:{resold.UseNum},上传代码{resold.UpMesCodePN}", Operator = op });
           _dBContext.Update(entity);
            await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.修改配方, Message = $"修改后工位拧紧枪任务，螺丝规格:{entity.ScrewSpecs},程序号:{entity.ProgramNo},使用次数:{entity.UseNum},上传代码{entity.UpMesCodePN}", Operator = op });
            await _dBContext.SaveChangesAsync();

            return entity;
        }

        /// <summary>
        /// 通过ID查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Base_StationTaskScrew> GetById(int id)
        {
            var entity = await _dBContext.Base_StationTaskScrews.FindAsync(id);

            return entity;
        }
    }
}
