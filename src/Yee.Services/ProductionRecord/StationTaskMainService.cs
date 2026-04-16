using AsZero.Core.Services.Repos;
using AsZero.DbContexts;

using Microsoft.EntityFrameworkCore;

using Yee.Entitys.DBEntity;
using Yee.Entitys.DTOS;

namespace Yee.Services.ProductionRecord
{
    public class StationTaskMainService
    {
        private readonly AsZeroDbContext _dbContext;

        public StationTaskMainService(AsZeroDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Response<List<Proc_StationTask_Main>>> GetList(StationTaskMainGetListDTO dto)
        {
            var result = new Response<List<Proc_StationTask_Main>>();
            try
            {
                var query = _dbContext.Proc_StationTask_Mains.Where(e => e.IsDeleted == false);

                var Steps = _dbContext.Base_Steps.Where(a => a.Code.Contains("OP030")).ToList();

                query = query.Where(e => Steps.Contains(e.Step));

                if (!string.IsNullOrWhiteSpace(dto.PackPN))
                {
                    query = query.Where(e => e.PackCode == dto.PackPN);
                }

                result.Data = await query.ToListAsync();
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

    }
}
