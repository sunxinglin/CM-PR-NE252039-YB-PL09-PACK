using AsZero.Core.Services.Repos;
using AsZero.DbContexts;
using Microsoft.EntityFrameworkCore;
using Yee.Entitys.DBEntity.ProductionRecords;
using Yee.Entitys.DTOS;

namespace Yee.Services.ProductionRecord
{
    public class Proc_ScrewNGResetRecordService
    {

        private readonly AsZeroDbContext _dbContext;

        public Proc_ScrewNGResetRecordService(AsZeroDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<Response> Save(Proc_ScrewNGResetRecordSaveDTO dto)
        {
            var result = new Response();
            try
            {
                //获取工站对应工序
                var station = await _dbContext.Base_Stations.AsNoTracking()
                    .Where(e => e.IsDeleted == false && e.Code == dto.StationCode)
                    .FirstOrDefaultAsync();
                if (station == null)
                {
                    result.Code = 1404;
                    result.Message = $"未找到工站信息，{dto.StationCode}";
                    return result;
                }

                //获取当前工站螺丝NG复位配置
                var configList = await _dbContext.Base_ScrewNGResetConfigs.AsNoTracking()
                    .Where(e => e.IsDeleted == false && e.StepId == station.StepId)
                    .ToListAsync();
                if (configList.Count <= 0) //未配置则直接返回OK
                {
                    result.Code = 200;
                    result.Message = $"工位[{dto.StationCode}]无拧紧NG复位配置";
                    return result;
                }

                //获取用户所属权限
                var user = await _dbContext.Users.AsNoTracking().Where(e => e.Account == dto.Account).FirstOrDefaultAsync();
                if (user == null)
                {
                    result.Code = 1404;
                    result.Message = $"未找到用户[{dto.Account}]";
                    return result;
                }
                var userClaimIds = await _dbContext.UserClaims.AsNoTracking().Include(e => e.User).Include(e => e.ClaimEntity)
                    .Where(e => e.User.IsDeleted == false && e.UserId == user.Id && e.ClaimEntity.IsDeleted == false)
                    .Select(e => e.ClaimEntity.Id)
                    .ToListAsync();

                //筛选用户权限对应的配置
                configList = configList.Where(e => e.RoleIdArray.Intersect(userClaimIds).Any())
                    .OrderByDescending(e => e.SingleScrewResetNum)
                    .ToList();
                if (configList.Count <= 0)
                {
                    result.Code = 1403;
                    result.Message = $"用户[{dto.Account}]在工站[{dto.StationCode}]无拧紧NG复位权限";
                    return result;
                }

                //获取用户权限对应的复位次数
                var resetNumLimit = (configList.Where(e => e.SingleScrewResetNum == 0).FirstOrDefault() ?? configList.FirstOrDefault())?.SingleScrewResetNum ?? 0;
                if (resetNumLimit <= 0)
                {
                    result.Code = 200;
                    result.Message = $"用户[{dto.Account}]在工站[{dto.StationCode}]无拧紧NG复位次数限制";
                    return result;
                }

                //获取当前螺丝已复位次数
                var resetNum = await _dbContext.Proc_ScrewNGResetRecords.AsNoTracking()
                    .Where(e => e.IsDeleted == false && e.PackCode == dto.PackCode && e.StepId == station.StepId && e.ScrewSerialNo == dto.ScrewSerialNo)
                    .CountAsync();

                //校验螺丝复位次数是否已超限
                if (resetNum >= resetNumLimit)
                {
                    result.Code = 1403;
                    result.Message = $"用户[{dto.Account}]在工站[{dto.StationCode}]复位NG螺丝[{dto.ScrewSerialNo}]次数已达上限[{resetNumLimit}]";
                    return result;
                }

                //新增复位记录
                await _dbContext.AddAsync(new Proc_ScrewNGResetRecord
                {
                    PackCode = dto.PackCode,
                    ScrewSerialNo = dto.ScrewSerialNo,
                    StepId = station.StepId,
                    UserId = user.Id,
                    CreateTime = DateTime.Now,
                    IsDeleted = false,
                });
                await _dbContext.SaveChangesAsync();

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
