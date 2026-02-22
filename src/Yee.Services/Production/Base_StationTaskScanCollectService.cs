using AsZero.Core.Entities;
using AsZero.Core.Services.Sys_Logs;
using AsZero.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Yee.Entitys.Production;

namespace Yee.Services.Production
{
    public class Base_StationTaskScanCollectService
    {
        private readonly AsZeroDbContext _dBContext;
        private readonly SysLogService sys_LogService;

        public Base_StationTaskScanCollectService(AsZeroDbContext dBContext, SysLogService sys_LogService)
        {
            this._dBContext = dBContext;
            this.sys_LogService = sys_LogService;
        }
        /// <summary>
        /// 通过ID查询
        /// </summary>
        public async Task<Base_StationTaskScanCollect?> GetById(int id)
        {
            var entity = _dBContext.Base_StationTaskScanCollects.FirstOrDefault(o => !o.IsDeleted && o.Id == id);
            return entity;
        }

        public async Task<Base_StationTaskScanCollect> Add(Base_StationTaskScanCollect entity, string op)
        {
            var res = await _dBContext.Base_StationTaskScanCollects.AddAsync(entity);

            await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.新增配方, Message = $"新增工位用户输入任务，物料名称:{entity.ScanCollectName}", Operator = op });
            await _dBContext.SaveChangesAsync();
            return res.Entity;
        }

        public async Task Delete(Base_StationTaskScanCollect entity, string op)
        {
            entity.IsDeleted = true;
            _dBContext.Base_StationTaskScanCollects.Update(entity);
            await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.删除配方, Message = $"删除工位用户输入任务，物料名称:{entity.ScanCollectName}", Operator = op });
            await _dBContext.SaveChangesAsync();

        }
        public async Task DeleteByTaskid(int taskid, string op)
        {
            var list = await _dBContext.Base_StationTaskScanCollects.Where(d => d.IsDeleted == false && d.StationTaskId == taskid).ToListAsync();
            foreach (var item in list)
            {
                item.IsDeleted = true;
                await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.删除配方, Message = $"删除工位用户输入任务，物料名称:{item.ScanCollectName}", Operator = op });
            }
            _dBContext.UpdateRange(list);
            await _dBContext.SaveChangesAsync();
        }
        public async Task<Base_StationTaskScanCollect> Update(Base_StationTaskScanCollect entity, string op)
        {
            var resold = _dBContext.Base_StationTaskScanCollects.Where(a => a.Id == entity.Id && !a.IsDeleted).AsNoTracking().FirstOrDefault();
            await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.修改配方, Message = $"修改前工位用户输入任务，物料名称:{resold.ScanCollectName},上传代码:{resold.UpMesCode}", Operator = op });

            var res = _dBContext.Base_StationTaskScanCollects.Update(entity);

            await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.修改配方, Message = $"修改后工位用户输入任务，物料名称:{entity.ScanCollectName},上传代码:{resold.UpMesCode}", Operator = op });
            //await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.修改配方, Message = $"修改工位用户输入任务，物料名称:{entity.UserScanName}",Operator = op });
            await _dBContext.SaveChangesAsync();
            return res.Entity;
        }

        public async Task<List<Base_StationTaskScanCollect>> GetStationScanCollectTaskByTaskid(int stationtaskid)
        {
            return await this._dBContext.Base_StationTaskScanCollects.Where(o => !o.IsDeleted && o.StationTaskId == stationtaskid).ToListAsync();
        }

        /// <summary>
        /// 校验扫码输入
        /// </summary>
        /// <param name="stepid"></param>
        /// <param name="packpn"></param>
        /// <param name="scanCollectData"></param>
        /// <returns></returns>
        public async Task<bool> GetScanCollectInfo(int stepid, string packpn, string scanCollectData)
        {
            var scanCollectInfo = await this._dBContext.Proc_StationTask_ScanCollects.Where(o => o.StepId == stepid && o.ScanCollectData == scanCollectData && o.IsDeleted == false).FirstOrDefaultAsync();
            if (scanCollectInfo != null)
            {
                //小车扫码不防呆
                if (scanCollectInfo.UpMesCode == "XCSM"
                    || scanCollectInfo.ScanCollectName == "小车扫码"
                    || scanCollectInfo.ScanCollectName == "扫描NTC支架定位工装-1"
                    || scanCollectInfo.ScanCollectName == "扫描NTC支架定位工装-2"
                    || scanCollectInfo.ScanCollectName == "扫码输入员工号"
                    || (scanCollectInfo.ScanCollectName?.StartsWith("扫描套筒") ?? false)
                    || (scanCollectInfo?.ScanCollectName?.EndsWith("-F") ?? false)
                    )
                {
                    return true;
                }
                return false;
            }
            return true;
        }
    }
}
