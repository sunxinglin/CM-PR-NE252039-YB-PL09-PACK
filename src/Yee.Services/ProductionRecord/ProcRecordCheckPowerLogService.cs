using AsZero.Core.Services.Repos;
using AsZero.DbContexts;

using Microsoft.EntityFrameworkCore;

using Yee.Entitys.DBEntity;
using Yee.Entitys.DTOS;
using Yee.Entitys.Request;

namespace Yee.Services.ProductionRecord
{
    public class ProcRecordCheckPowerLogService
    {
        private readonly AsZeroDbContext _dbContext;

        public ProcRecordCheckPowerLogService(AsZeroDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Record_CheckPower_Log(RecordCheckPowerDTO dto)
        {
            string userName = string.Empty;
            var user = await _dbContext.Users.FirstOrDefaultAsync(a => a.Account == dto.Account);
            if (user == null) userName = dto.Account;
            else userName = user.Name;
            if (dto.Alarms != null && dto.Alarms.Count > 0)
            {
                foreach (var alarm in dto.Alarms)
                {
                    _dbContext.Proc_CheckPowerRecords.Add(new Proc_CheckPowerRecord
                    {
                        AccountName = userName,
                        ModuleName = dto.ModuleName,
                        PackCode = dto.PackCode,
                        StationCode = dto.StationCode,
                        Alarm = alarm.Description,
                        CreateTime = DateTime.Now
                    });
                }
            }
            else
            {
                _dbContext.Proc_CheckPowerRecords.Add(new Proc_CheckPowerRecord
                {
                    AccountName = userName,
                    ModuleName = dto.ModuleName,
                    PackCode = dto.PackCode,
                    StationCode = dto.StationCode,
                    CreateTime = DateTime.Now
                });
            }
            _dbContext.SaveChanges();
        }

        public async Task<Response<List<Proc_CheckPowerRecord>>> GetPageList(ProcCheckPowerRecordGetPageListRequest request)
        {
            var result = new Response<List<Proc_CheckPowerRecord>>();
            try
            {
                var query = _dbContext.Proc_CheckPowerRecords.Where(e => e.IsDeleted == false);

                if (request.BeginDate.HasValue)
                {
                    request.BeginDate = request.BeginDate.GetValueOrDefault().Date;
                    query = query.Where(e => e.CreateTime >= request.BeginDate);
                }
                if (request.EndDate.HasValue)
                {
                    request.EndDate = request.EndDate.GetValueOrDefault().Date.AddDays(1);
                    query = query.Where(e => e.CreateTime < request.EndDate);
                }
                if (!string.IsNullOrEmpty(request.PackCode))
                {
                    query = query.Where(e => e.PackCode == request.PackCode);
                }
                if (!string.IsNullOrEmpty(request.StationCode))
                {
                    query = query.Where(e => e.StationCode == request.StationCode);
                }
                if (!string.IsNullOrEmpty(request.ModuleName) && !request.ModuleName.Equals("全部"))
                {
                    query = query.Where(e => e.ModuleName.StartsWith(request.ModuleName));
                }
                //总数
                result.Count = await query.CountAsync();

                //列表
                var list = await query
                    .OrderByDescending(e => e.Id)
                    .Skip((request.Page - 1) * request.Limit)
                    .Take(request.Limit)
                    .ToListAsync();

                result.Data = list;
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
