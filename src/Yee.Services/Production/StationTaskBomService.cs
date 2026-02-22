using AsZero.Core.Entities;
using AsZero.Core.Services.Sys_Logs;
using AsZero.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yee.Entitys.Production;
using Yee.Services.Request;

namespace Yee.Services.Production
{
    public class StationTaskBomService
    {
        public readonly AsZeroDbContext _dBContext;
        private readonly SysLogService sys_LogService;

        public StationTaskBomService(AsZeroDbContext dBContext, SysLogService sys_LogService)
        {
            _dBContext = dBContext;
            this.sys_LogService = sys_LogService;
        }

        /// <summary>
        /// 通过关键字查询
        /// </summary>
        public async Task<List<Base_StationTaskBom>> GetBomListByStationTask(int stationtaskid)
        {
            var list = await _dBContext.Base_StationTaskBoms.Where(d => d.IsDeleted == false && d.StationTaskId== stationtaskid).ToListAsync();
            return list;
        }

        public async Task<Base_StationTaskBom> Add(Base_StationTaskBom entity,string op)
        {
            var res = await _dBContext.AddAsync(entity);
            
            await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.新增配方, Message = $"新增工位扫码任务，物料名称:{entity.GoodsName}",Operator=op });
            await _dBContext.SaveChangesAsync();
            return res.Entity;
        }
        /// <summary>
        /// 验证当前工位任务是否已配置扫码任务
        /// </summary>
        /// <param name="taskid"></param>
        /// <returns></returns>
        public async Task<Boolean> CheckTaskBom(int taskid)
        {
            var alltaskbomcount = (await _dBContext.Base_StationTaskBoms.Where(o => !o.IsDeleted && o.StationTaskId == taskid).ToListAsync()).Count;
            if (alltaskbomcount > 0)
                return false;
            else
                return true;
        }
        public async Task Delete(Base_StationTaskBom entity)
        {
            entity.IsDeleted = true;
            _dBContext.Update(entity);
            await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.删除配方, Message = $"删除工位扫码任务，物料名称:{entity.GoodsName}" });
            await _dBContext.SaveChangesAsync();
        }
        public async Task DeleteByTaskid(int taskid,string op)
        {

            var list = await _dBContext.Base_StationTaskBoms.Where(d => d.IsDeleted == false && d.StationTaskId == taskid).ToListAsync();
            foreach (var item in list)
            {
                item.IsDeleted = true;
                await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.删除配方, Message = $"删除工位扫码任务，物料名称:{item.GoodsName}",Operator = op });
            }
            _dBContext.UpdateRange(list);
           
            await _dBContext.SaveChangesAsync();
        }
        public async Task<Base_StationTaskBom> Update(Base_StationTaskBom entity,string op)
        {
            var resold = _dBContext.Base_StationTaskBoms.Where(a => a.Id == entity.Id && !a.IsDeleted).AsNoTracking().FirstOrDefault();
            await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.修改配方, Message = $"修改前工位扫码任务，物料名称:{resold.GoodsName},条码规则:{resold.GoodsPNRex},{resold.OuterGoodsPNRex},{resold.GoodsPN}", Operator = op });

            var res = _dBContext.Update(entity);
           
            await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.修改配方, Message = $"修改后工位扫码任务，物料名称:{entity.GoodsName},条码规则:{entity.GoodsPNRex},{entity.OuterGoodsPNRex},{entity.GoodsPN}",Operator = op });
            await _dBContext.SaveChangesAsync();
            return res.Entity;
        }


        public async Task<bool> HasCode(string code, int? stationTaskId)
        {
            var entity = await _dBContext.Base_StationTaskBoms.Where(d => !d.IsDeleted && d.GoodsPN == code && d.StationTaskId == stationTaskId).FirstOrDefaultAsync();
            if (entity != null)
                return true;
            else
                return false;
        }


        /// <summary>
        /// 通过ID查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Base_StationTaskBom> GetById(int id)
        {
            var entity = await _dBContext.Base_StationTaskBoms.FindAsync(id);

            return entity;
        }



    }
}
