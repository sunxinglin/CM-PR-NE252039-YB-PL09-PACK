using AsZero.Core.Entities;
using AsZero.Core.Services.Repos;
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

namespace Yee.Services.BaseData
{
    public class StepService
    {
        public readonly AsZeroDbContext _dBContext;
        private readonly SysLogService sys_LogService;

        public StepService(AsZeroDbContext dBContext, SysLogService sys_LogService)
        {
            _dBContext = dBContext;
            this.sys_LogService = sys_LogService;
        }

        /// <summary>
        /// 通过关键字查询
        /// </summary>
        public async Task<List<Base_Step>> GetAll(string? key)
        {
            if (key == null)
            {
                var list = await _dBContext.Base_Steps.Where(d => d.IsDeleted == false).ToListAsync();
                return list;
            }
            else
            {
                var list = await _dBContext.Base_Steps.Where(d => d.IsDeleted == false && (d.Code.Contains(key) || d.Name.Contains(key))).ToListAsync();
                return list;
            }
        }
        /// <summary>
        /// 通过ID查询
        /// </summary>
        public async Task<Base_Step?> GetById(int id)
        {
            var entity = await _dBContext.Base_Steps.FindAsync(id);
            return entity;
        }

        public async Task<Base_Step> Add(Base_Step entity, string op)
        {
            var res = await _dBContext.AddAsync(entity);

            await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.新增工序, Message = $"新增工序:{entity.Name}", Operator = op });
            await _dBContext.SaveChangesAsync();
            return res.Entity;
        }

        public async Task Delete(Base_Step entity, string op)
        {
            entity.IsDeleted = true;
            _dBContext.Update(entity);

            await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.删除工序, Message = $"删除工序:{entity.Name}", Operator = op });
            await _dBContext.SaveChangesAsync();
        }


        public async Task<bool> HasCode(string code)
        {
            var entity = await _dBContext.Base_Steps.Where(d => !d.IsDeleted && d.Code == code).FirstOrDefaultAsync();
            if (entity != null)
                return true;
            else
                return false;
        }

        public async Task<Base_Step> Update(Base_Step entity, string op)
        {
            var resold = _dBContext.Base_Steps.Where(a => a.Id == entity.Id && !a.IsDeleted).AsNoTracking().FirstOrDefault();
            await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.修改工序, Message = $"修改前工序:{resold.Name}", Operator = op });

            var res = _dBContext.Update(entity);

            await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.修改工序, Message = $"修改后工序:{entity.Name}", Operator = op });
            await _dBContext.SaveChangesAsync();
            return res.Entity;
        }

        public async Task<Base_Step> LoadStepInfo(string code)
        {
            var res = _dBContext.Base_Steps.FirstOrDefault(f => !f.IsDeleted && f.Code == code);
            return res;
        }

        public async Task<Base_Station> GetStationByStep(int id)
        {
            var res = _dBContext.Base_Stations.FirstOrDefault(f => !f.IsDeleted && f.StepId == id);
            return res;
        }

        public async Task<Response<List<Base_Step>>> GetStepsByProductId(int productId)
        {
            var result = new Response<List<Base_Step>>();
            var flow = _dBContext.Base_Flows.FirstOrDefault(f => !f.IsDeleted && f.ProductId == productId);
            if (flow == null)
            {
                result.Code = 500;
                result.Message = "请先配置此产品的工艺路线";
                result.Result = null;
                return result;
            }

            var q = from step in _dBContext.Base_Steps
                    join map in _dBContext.Base_FlowStepMappings on step.Id equals map.StepId
                    where map.FlowId == flow.Id && map.IsDeleted == false && step.IsDeleted == false
                    orderby map.OrderNo
                    select step;

            var list = await q.ToListAsync();


            //var s = await _dBContext.Base_FlowStepMappings.Include(e => e.Step).AsNoTracking()
            //    .Where(e => !e.IsDeleted && e.FlowId == flow.Id && e.Step.IsDeleted == false)
            //    .OrderBy(e => e.OrderNo).Select(e => e.Step).ToListAsync();


            //var stepIds = _dBContext.Base_FlowStepMappings.Where(f => !f.IsDeleted && f.FlowId == flow.Id).Select(f => f.StepId).ToList();
            //var res = await _dBContext.Base_Steps.Where(f => !f.IsDeleted && stepIds.Contains(f.Id)).ToListAsync();

            result.Result = list;
            result.Code = 200;
            return result;
        }
    }
}
