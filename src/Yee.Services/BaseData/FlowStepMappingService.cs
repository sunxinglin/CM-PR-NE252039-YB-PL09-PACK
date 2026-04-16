using AsZero.DbContexts;

using Microsoft.EntityFrameworkCore;

using Yee.Entitys.Production;
using Yee.Services.Request;

namespace Yee.Services.BaseData
{
    public class FlowStepMappingService
    {
        public readonly AsZeroDbContext _dBContext;
        public FlowStepMappingService(AsZeroDbContext dBContext)
        {
            _dBContext = dBContext;
        }

        /// <summary>
        /// 通过关键字查询
        /// </summary>
        public async Task<List<Base_FlowStepMapping>> GetList(GetFlowStepMappingListReq request)
        {
            var query = _dBContext.Base_FlowStepMappings.Where(a => !a.IsDeleted );

            if (request.FlowId != null)
            {
                query = query.Where(a => a.FlowId == request.FlowId);
            }
            if (request.StepId != null)
            {
                query= query.Where(u => u.StepId == request.StepId);
            }
            return await query.Include(o=>o.Step).Include(o=>o.Flow).Include(o=>o.Flow.Product).OrderBy(a => a.OrderNo).AsNoTracking().ToListAsync();
        }
        public async Task<Base_FlowStepMapping> GetMapByStepId(int stepid,int flowId)
        {
            var query = _dBContext.Base_FlowStepMappings.Where(a => !a.IsDeleted);

           
                query = query.Where(u => u.StepId == stepid && u.FlowId==flowId);
            
            return await query.Include(o => o.Step).Include(o => o.Flow).Include(o => o.Flow.Product).OrderBy(a => a.OrderNo).AsNoTracking().FirstOrDefaultAsync();
        }
        public async Task<Base_FlowStepMapping?> Add(Base_FlowStepMapping entity)
        {
            var res = await _dBContext.AddAsync(entity);
            await _dBContext.SaveChangesAsync();
            return res.Entity;
        }

        public async Task Delete(Base_FlowStepMapping entity)
        {
            entity.IsDeleted = true;
            var oldbase = _dBContext.Base_FlowStepMappings.Where(o => o.Id == entity.Id).AsNoTracking().FirstOrDefault();
            oldbase.IsDeleted = true;
            _dBContext.Update(oldbase);
            await _dBContext.SaveChangesAsync();
        }

        public async Task<Base_FlowStepMapping?> Update(Base_FlowStepMapping entity)
        {
            var updata = _dBContext.Entry(entity);
            updata.State = EntityState.Modified;
            var res = _dBContext.Update(entity);
            await _dBContext.SaveChangesAsync();
            return res.Entity;
        }
    }
}
