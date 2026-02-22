using AsZero.Core.Services.Repos;
using AsZero.DbContexts;
using Microsoft.EntityFrameworkCore;
using Yee.Entitys.DBEntity.Production;
using Yee.Entitys.Request;
using Yee.Entitys.Response;

namespace Yee.Services.ProductionRecord
{
    public class Base_ScrewNGResetConfigService
    {
        private readonly AsZeroDbContext _dbContext;

        public Base_ScrewNGResetConfigService(AsZeroDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Response<List<ScrewNGResetConfigPageListResponse>>> GetPageList(int pageIndex = 1, int pageSize = 20)
        {
            var result = new Response<List<ScrewNGResetConfigPageListResponse>>();
            try
            {
                var query = _dbContext.Base_ScrewNGResetConfigs.Include(e => e.Step).AsNoTracking().Where(d => d.IsDeleted == false);

                var configs = await query.OrderBy(e => e.StepId)
                    .Skip((pageIndex - 1) * pageSize).Take(pageSize)
                    .ToListAsync();
                var count = await query.CountAsync();

                var roleIdArray = configs.SelectMany(e => e.RoleIdArray).ToArray();
                var claims = await _dbContext.Claims.AsNoTracking().Where(e => roleIdArray.Contains(e.Id)).ToListAsync();

                var list = new List<ScrewNGResetConfigPageListResponse>();
                foreach (var item in configs)
                {
                    var newItem = new ScrewNGResetConfigPageListResponse
                    {
                        Id = item.Id,
                        RoleIds = item.RoleIds,
                        Step = item.Step,
                        SingleScrewResetNum = item.SingleScrewResetNum,
                        StepId = item.StepId,
                        RoleNameArray = claims.Where(e => item.RoleIdArray.Contains(e.Id)).Select(e => e.ClaimValue).ToArray()
                    };

                    list.Add(newItem);
                }

                result.Data = list;
                result.Count = count;
            }
            catch (Exception ex)
            {
                result.Code = 1500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        public async Task<Response> Add(ScrewNGResetConfigAddRequest request)
        {
            var result = new Response();
            var item = new Base_ScrewNGResetConfig
            {
                StepId = request.StepId,
                SingleScrewResetNum = request.SingleScrewResetNum,
                RoleIds = string.Join(",", request.RoleIdArray),
                CreateTime = DateTime.Now,
                IsDeleted = false
            };

            await _dbContext.AddAsync(item);
            await _dbContext.SaveChangesAsync();
            return result;
        }

        public async Task<Response> Update(ScrewNGResetConfigUpdateRequest request)
        {
            var result = new Response();
            var item = await _dbContext.Base_ScrewNGResetConfigs.Where(e => e.Id == request.Id).FirstOrDefaultAsync();
            if (item == null)
            {
                result.Code = 1404;
                result.Message = $"未找到配置，{request.Id}";
                return result;
            }

            item.StepId = request.StepId;
            item.SingleScrewResetNum = request.SingleScrewResetNum;
            item.RoleIds = string.Join(",", request.RoleIdArray);

            await _dbContext.SaveChangesAsync();
            return result;
        }

        public async Task<Response> Delete(DeleteByIdsInput request)
        {
            var response = new Response();
            var list = await _dbContext.Base_ScrewNGResetConfigs.Where(e => e.IsDeleted == false && request.Ids.Contains(e.Id)).ToListAsync();
            foreach (var item in list)
            {
                item.IsDeleted = true;
                item.DeleteTime = DateTime.Now;
            }
            await _dbContext.SaveChangesAsync();
            return response;
        }

    }
}
