using AsZero.DbContexts;
using Yee.Entitys.DBEntity;
using Microsoft.EntityFrameworkCore;
using Yee.Entitys.DTOS;
using Ctp0600P.Shared.Helper;
using Newtonsoft.Json;

namespace Yee.Services.Production
{
    public class Proc_StationTask_PeiFangService
    {
        private AsZeroDbContext _dbContext;
        public Proc_StationTask_PeiFangService(AsZeroDbContext dbContext)
        {
            this._dbContext = dbContext;
        }
        /// <summary>
        /// 获取配方Json对应的ID
        /// </summary>
        /// <param name="pack"></param>
        /// <returns></returns>
        public async Task<int> GetOrCreateNewPeiFangJson_MD5(string jsonStr)
        {
            try
            {
                var peiFang_MD5 = StringToMD5Hepler.StrConversionMD5(jsonStr);
                var exsit = await _dbContext.Proc_StationTask_PeiFangs.FirstOrDefaultAsync(m => m.PeiFang_MD5 == peiFang_MD5);
                if (exsit == null)
                {
                    _dbContext.Proc_StationTask_PeiFangs.Add(new Proc_StationTask_PeiFang
                    {
                        PeiFang_Json = jsonStr,
                        PeiFang_MD5 = peiFang_MD5
                    });
                    _dbContext.SaveChanges();
                    exsit = await _dbContext.Proc_StationTask_PeiFangs.FirstOrDefaultAsync(m => m.PeiFang_MD5 == peiFang_MD5);
                }

                return exsit.ID;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        /// <summary>
        /// 获取配方Json对应的ID
        /// </summary>
        /// <param name="pack"></param>
        /// <returns></returns>
        public async Task<List<StationTaskDTO>> GetPeiFangDataByID(int Id)
        {
            try
            {
                var exsit = await _dbContext.Proc_StationTask_PeiFangs.FirstOrDefaultAsync(m => m.ID == Id);
                if (exsit == null)
                {
                    return null;
                }
                else
                {
                    return JsonConvert.DeserializeObject<List<StationTaskDTO>>(exsit.PeiFang_Json);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
