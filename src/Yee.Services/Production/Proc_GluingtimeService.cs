using AsZero.Core.Services.Repos;
using AsZero.DbContexts;

using Microsoft.EntityFrameworkCore;

using Yee.Entitys.DBEntity.ProductionRecords;
using Yee.Entitys.DTOS;

namespace Yee.Services.Production
{
    public class Proc_GluingtimeService
    {
        private AsZeroDbContext _dbContext;
        public Proc_GluingtimeService(AsZeroDbContext dbContext)
        {
            this._dbContext = dbContext;
        }
        /// <summary>
        /// 通过Pack码查询涂胶超时时间
        /// </summary>
        /// <param name="pack"></param>
        /// <returns></returns>
        public async Task<IList<GluingTimeDTO>> GetByPack(string pack)
        {
            try
            {
                var gluingtimeList = await _dbContext.Proc_StationTask_CheckTimeouts.OrderByDescending(s => s.Id).Where(s => s.PackPN == pack && !s.IsDeleted).ToListAsync();
                List<GluingTimeDTO> gluingTimeDTOs = new List<GluingTimeDTO>();
                foreach (var gluingtime in gluingtimeList)
                {
                    gluingTimeDTOs.Add(new GluingTimeDTO()
                    {
                        Id = gluingtime.Id,
                        GluingTime = gluingtime.Time,
                        GluingTimeName = gluingtime.TimeName,
                        PackPN = gluingtime.PackPN,
                        CreateTime = gluingtime.CreateTime,
                    });

                }

                return gluingTimeDTOs;
            }
            catch (Exception ex)
            {
                return null;
            }
        }    
        
        public async Task<Response> GetRecordTime(string? pack)
        {
            var response = new Response();
            try
            {
                if (string.IsNullOrEmpty(pack))
                {
                    response.Data = null;
                    return response;
                }

                var gluingtimeList = _dbContext.Proc_StationTask_TimeRecords.OrderByDescending(s => s.Id ).Where(s => !s.IsDeleted && s.SerialCode == pack);
                
                response.Data = await gluingtimeList.ToListAsync();
                
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<Response> UpdateTime(Proc_StationTask_TimeRecord record)
        {
            var response = new Response();
            try
            {
                var recordTime =  await _dbContext.Proc_StationTask_TimeRecords.FirstOrDefaultAsync(s => !s.IsDeleted && s.Id == record.Id);
                if(recordTime == null)
                {
                    response.Code = 500;
                    response.Message = "数据未找到";
                    return response;
                }
                recordTime.TimeValue = record.TimeValue;
                _dbContext.Update(recordTime);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Message = ex.Message;
            }
            return response;
        }

        /// <summary>
        /// 修改涂胶超时时间
        /// </summary>
        /// <param name="task_GluingTime"></param>
        /// <returns></returns>
        public async Task<(bool, string)> Updata(int id, decimal time)
        {
            try
            {
                var gluingTime = await _dbContext.Proc_StationTask_CheckTimeouts.FirstOrDefaultAsync(s => s.Id == id);
                if (gluingTime != null)
                {
                    gluingTime.Time = time;
                    await _dbContext.SaveChangesAsync();
                }
                return (true, "");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}
