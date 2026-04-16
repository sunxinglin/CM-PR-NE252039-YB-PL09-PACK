using AsZero.Core.Entities;
using AsZero.Core.Services.Repos;
using AsZero.Core.Services.Sys_Logs;
using AsZero.DbContexts;

using Microsoft.EntityFrameworkCore;

using Yee.Entitys.Production;

namespace Yee.Services.Production
{
    public class Base_StationTaskAutoService
    {
        private readonly AsZeroDbContext _dBContext;
        private readonly SysLogService sys_LogService;

        public Base_StationTaskAutoService(AsZeroDbContext dBContext, SysLogService sys_LogService)
        {
            this._dBContext = dBContext;
            this.sys_LogService = sys_LogService;
        }
        public async Task DeleteByTaskid(int taskid)
        {
            var list = await _dBContext.Base_AutoStationTaskTightens.Where(d => d.IsDeleted == false && d.StationTaskId == taskid).ToListAsync();
            foreach (var item in list)
            {
                item.IsDeleted = true;
            }
            _dBContext.UpdateRange(list);
            await _dBContext.SaveChangesAsync();
        }

        public async Task<Response> Add(Base_AutoStationTaskTighten entity, string op)
        {
            var response = new Response();
            try
            {
                await _dBContext.AddAsync(entity);
                await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.新增配方, Message = $"新增自动拧紧任务:{entity.ParamName}" });
                await _dBContext.SaveChangesAsync();
            }
            catch (Exception ex) 
            {
                response.Code = 500;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<Response> Delete(Base_AutoStationTaskTighten entity, string op)
        {
            var response = new Response();
            try
            {
                entity.IsDeleted = true;
                _dBContext.Update(entity);
                await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.删除配方, Message = $"删除自动拧紧任务:{entity.ParamName}" });
                await _dBContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<Base_AutoStationTaskTighten> Update(Base_AutoStationTaskTighten entity, string op)
        {

            var res = _dBContext.Update(entity);
            if (entity.StationTask!=null && entity.StationTask.Step != null)
            {
                await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.修改配方, Message = $"修改后任务名称:{entity.StationTask.Name},工位名称{entity.StationTask.Step.Name},程序号:{entity.ProgramNo},螺丝数量:{entity.UseNum},上传代码:{entity.UpMesCode}",Operator = op });
            } 
            await _dBContext.SaveChangesAsync();
            return res.Entity;
        }

        public async Task<Response<IList<Base_AutoStationTaskTighten>>> GetByTaskId(int taskid)
        {
            var resp = new Response<IList<Base_AutoStationTaskTighten>>();
            try
            {
                var entity = await _dBContext.Base_AutoStationTaskTightens.Where(o => !o.IsDeleted && o.StationTaskId == taskid).Include(o => o.StationTask).ToListAsync();
                resp.Result = entity;
            }
            catch (Exception ex) 
            {
                resp.Code = 500;
                resp.Message = ex.Message;
            }

            return resp;
        }
    }
}
