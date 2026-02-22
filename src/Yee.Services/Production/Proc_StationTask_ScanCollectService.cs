using AsZero.Core.Services.Sys_Logs;
using AsZero.DbContexts;
using Microsoft.EntityFrameworkCore;
using Yee.Entitys.DBEntity;

namespace Yee.Services.Production
{
    public class Proc_StationTask_ScanCollectService
    {
        private readonly AsZeroDbContext dbContext;
        private readonly SysLogService sys_LogService;

        public Proc_StationTask_ScanCollectService(AsZeroDbContext dBContext, SysLogService sys_LogService)
        {
            this.dbContext = dBContext;
            this.sys_LogService = sys_LogService;
        }

        #region 获取分页数据
        public class GetScanCollectServiceRequest
        {
            public int Limit { get; set; } = 20;
            public int Page { get; set; } = 1;
            public int? Id { get; set; }
            public string? ScanData { get; set; }
            public string? PackPN { get; set; }
            public bool ContainDeleted { get; set; } = false;
        }

        public async Task<(List<Proc_StationTask_ScanCollect>, int)> GetScanCollectsAsync(GetScanCollectServiceRequest? request = null)
        {
            var query = dbContext.Proc_StationTask_ScanCollects.AsQueryable();
            if (request != null)
            {
                if (request.Id != null)
                {
                    query = query.Where(x => x.Id == request.Id.Value);
                }
                if (!string.IsNullOrWhiteSpace(request.ScanData))
                {
                    query = query.Where(x => x.ScanCollectData == request.ScanData);
                }
                if (!string.IsNullOrWhiteSpace(request.PackPN))
                {
                    query = query.Where(x => x.PackPN == request.PackPN);
                }
            }
            if (request == null || !request.ContainDeleted)
            {
                query = query.Where(x => !x.IsDeleted);
            }
            var limit = 20;
            var page = 1;
            if (request != null)
            {
                limit = request.Limit <= 0 ? 20 : request.Limit;
                page = request.Page <= 0 ? 1 : request.Page;
            }

            var total = await query.CountAsync();
            var list = await query.OrderBy(x => x.Id).Skip((page - 1) * limit).Take(limit).ToListAsync();

            return (list, total);
        }
        #endregion

        /// <summary>
        /// 踢料接口
        /// </summary>
        /// <param name="scanData"></param>
        /// <param name="packPN"></param>
        /// <param name="isDeleted"></param>
        /// <returns></returns>
        public async Task<int> ChangeDatasState(string scanData, string packPN, bool isDeleted)
        {
            var needUpdate = await dbContext.Proc_StationTask_ScanCollects.Where(x => x.ScanCollectData == scanData && x.PackPN == packPN).OrderBy(x => x.Id).ToListAsync();
            if (needUpdate.Count > 0)
            {
                foreach (var item in needUpdate)
                {
                    item.IsDeleted = isDeleted;
                }
                dbContext.Proc_StationTask_ScanCollects.UpdateRange(needUpdate);
                await dbContext.SaveChangesAsync();
            }
            return needUpdate.Count;
        }
    }
}
