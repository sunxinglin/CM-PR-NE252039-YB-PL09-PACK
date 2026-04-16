using AsZero.Core.Entities;
using AsZero.Core.Services.Sys_Logs;
using AsZero.DbContexts;

using Microsoft.EntityFrameworkCore;

using Yee.Entitys.Production;

namespace Yee.Services.BaseData
{
    public class FlowService
    {
        public readonly AsZeroDbContext _dBContext;
        private readonly SysLogService sys_LogService;

        public FlowService(AsZeroDbContext dBContext, SysLogService sys_LogService)
        {
            _dBContext = dBContext;
            this.sys_LogService = sys_LogService;
        }

        /// <summary>
        /// 通过关键字查询所有
        /// </summary>
        public async Task<List<Base_Flow>> GetAll(string? key)
        {
            List<Base_Flow> list;
            if (key == null)
            {
                list = _dBContext.Base_Flows.Include(F => F.Product).Where(d => d.IsDeleted == false).ToList();
            }
            else
            {
                list = await _dBContext.Base_Flows.Include(F => F.Product).Where(d => d.IsDeleted == false && (d.Code.Contains(key) || d.Name.Contains(key))).AsNoTracking().ToListAsync();
            }
            return list;
        }

        public async Task<bool> HasCode(string code)
        {
            var entity = await _dBContext.Base_Flows.Where(d => !d.IsDeleted && d.Code == code).FirstOrDefaultAsync();
            if (entity != null)
                return true;
            else
                return false;
        }

        public async Task<Base_Flow?> GetById(int id)
        {
            var entity = await _dBContext.Base_Flows.FindAsync(id);
            return entity;
        }

        public async Task<Base_Flow?> Add(Base_Flow entity, string op)
        {
            var res = await _dBContext.AddAsync(entity);
            await _dBContext.SaveChangesAsync();
            await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.新增工艺, Message = $"新增工艺:{entity.Name}", Operator = op });
            return res.Entity;
        }

        public async Task Delete(Base_Flow entity, string op)
        {
            entity.IsDeleted = true;
            _dBContext.Update(entity);
            await _dBContext.SaveChangesAsync();
            await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.删除工艺, Message = $"删除工艺:{entity.Name}", Operator = op });
        }

        public async Task<Base_Flow?> Update(Base_Flow entity, string op)
        {
            var resold = _dBContext.Base_Flows.Where(a => a.Id == entity.Id && !a.IsDeleted).AsNoTracking().FirstOrDefault();
            await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.修改工艺, Message = $"修改前工艺:{resold.Name}", Operator = op });

            var res = _dBContext.Update(entity);
            await _dBContext.SaveChangesAsync();
            await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.修改工艺, Message = $"修改后工艺:{entity.Name}", Operator = op });

            return res.Entity;
        }

        public async Task<List<Base_FlowStepMapping>?> GetStepsByFlowAndStepCode(string packPN)
        {
            var entityList = _dBContext.Products.Where(f => !string.IsNullOrEmpty(f.PackPNRule) && !f.IsDeleted).ToList();

            Base_Product? product = null;
            foreach (var item in entityList)
            {
                if (item.PackPNRule.Length == packPN.Length && packPN.StartsWith(item.PackPNRule.TrimEnd('*')))
                {
                    product = item;
                    break;
                }
                var modelrules = item.ModelRules;
                var model = modelrules.FirstOrDefault(o => packPN.StartsWith(o.Value.TrimEnd('*')));
                if (model != null)
                {
                    product = item;
                    break;
                }
            }

            if (product != null)
            {
                var entityFlow = _dBContext.Base_Flows.FirstOrDefault(f => f.ProductId == product.Id && !f.IsDeleted);
                return await _dBContext.Base_FlowStepMappings.Where(f => f.FlowId == entityFlow.Id).OrderByDescending(o => o.OrderNo).ToListAsync();
            }
            else
                return null;
        }



        public async Task<List<Base_FlowStepMapping>?> GetStepsByFlowCode(string packPN)
        {
            var entityList = _dBContext.Products.Where(f => !string.IsNullOrEmpty(f.PackPNRule) && !f.IsDeleted).ToList();

            Base_Product? product = null;

            foreach (var item in entityList)
            {
                if (item.PackPNRule.Length == packPN.Length && packPN.StartsWith(item.PackPNRule.TrimEnd('*')))
                {
                    product = item;
                    break;
                }
                var modelrules = item.ModelRules;
                var model = modelrules.FirstOrDefault(o => packPN.StartsWith(o.Value.TrimEnd('*')));
                if (model != null)
                {
                    product = item;
                    break;
                }
            }

            if (product != null)
            {
                var entityFlow = _dBContext.Base_Flows.FirstOrDefault(f => f.ProductId == product.Id && !f.IsDeleted);
                return await _dBContext.Base_FlowStepMappings.Where(f => f.FlowId == entityFlow.Id).OrderByDescending(o => o.OrderNo).ToListAsync();
            }
            else
                return null;
        }


        public async Task<List<Base_FlowStepMapping>?> GetStepsByProduct(Base_Product product)
        {
            if (product != null)
            {
                var entityFlow = _dBContext.Base_Flows.FirstOrDefault(f => f.ProductId == product.Id && !f.IsDeleted);
                return await _dBContext.Base_FlowStepMappings.Where(f => f.FlowId == entityFlow.Id).OrderByDescending(o => o.OrderNo).ToListAsync();
            }
            else
                return null;
        }

        public async Task<Base_Flow> GetByFlowId(int productId)
        {
            var entity = await _dBContext.Base_Flows.Where(f => f.ProductId == productId && !f.IsDeleted).FirstOrDefaultAsync();
            return entity;
        }
    }
}
