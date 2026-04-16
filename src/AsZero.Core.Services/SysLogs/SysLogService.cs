using AsZero.Core.Entities;
using AsZero.DbContexts;

using Microsoft.EntityFrameworkCore;

namespace AsZero.Core.Services.Sys_Logs
{
    public class SysLogService
    {
        private readonly AsZeroDbContext _dBContext;

        public SysLogService(AsZeroDbContext dBContext)
        {
            this._dBContext = dBContext;
        }
        /// <summary>
        /// 添加系统日志
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        public async Task<Boolean> AddLog(SysLog log)
        {
            log.CreateTime = DateTime.Now;
            var result= await this._dBContext.AddAsync(log);
            //if (result!=null)
            //{
            //    return true;
            //}
            await this._dBContext.SaveChangesAsync();
            return false;
        }
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<List<SysLog>> GetList(SyslogDto dto)
        {
            var query = this._dBContext.Sys_Logs.Where(o=>!o.IsDeleted);
            if ((int)dto.Type!=-1)
            {
                query = query.Where(o => o.LogType == dto.Type);
            }
            if (dto.EndTime!=null && dto.BeginTime!=null)
            {
                query = query.Where(o => o.CreateTime.Date >= dto.BeginTime.Value.Date && o.CreateTime.Date <= dto.EndTime.Value.Date);
            }
            return await query.ToListAsync();
        }
    }
}
