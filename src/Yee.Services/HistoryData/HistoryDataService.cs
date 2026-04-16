using AsZero.Core.Services.Repos;
using AsZero.DbContexts;

using Microsoft.EntityFrameworkCore;

using TimedTask.ClearHisData;

using Yee.Common.Library.CommonEnum;

namespace Yee.Services.HistoryData
{
    public class HistoryDataService : IHistoryDataResolver
    {
        private readonly ILogger<HistoryDataService> _logger;
        public readonly AsZeroDbContext _dBContext;
        public HistoryDataService(IServiceScopeFactory factory, ILogger<HistoryDataService> logger
            )
        {
            _dBContext = factory.CreateScope().ServiceProvider.GetRequiredService<AsZeroDbContext>();
            _logger = logger;
        }


        private async Task<Response> ClearRealTimeHisData_LastDays(int days)
        {
            try
            {
                var result = new Response();
                DateTimeOffset hisDataTime = DateTime.Now.AddDays(0 - days).Date;
                var timeSpan_Q = hisDataTime.ToUnixTimeMilliseconds();
                //Timespan 在涂胶和上盖拧紧数据保存中未赋值 保存是默认保存0值 导致清除数据时 会清除当天Timespan=0的数据 严重影响生产！！！！！！！！！！
                var entityList = await _dBContext.Proc_StationTask_Mains.Where(d => d.Status == StationTaskStatusEnum.已完成 && ((d.Timespan < timeSpan_Q)&&d.Timespan!=0)).ToListAsync();
                if (entityList != null && entityList.Count > 0)
                {
                    result = await DeleteRealTimeStationTaskRecordAsync(entityList.Select(m => m.Id).ToList());
                    if (result.Code != 200)
                        return result;
                    result = await DeleteRealTimeAutoBoltInfoAsync(entityList.Select(m => m.Id).ToList());
                    if (result.Code != 200)
                        return result;
                    result = await DeleteRealTimeGluingInfoAsync(entityList.Select(m => m.Id).ToList());
                    if (result.Code != 200)
                        return result;
                    _dBContext.Proc_StationTask_Mains.RemoveRange(entityList);

                    _dBContext.SaveChanges();
                    return result;
                }
                else
                    return result;
            }
            catch (Exception ex)
            {
                return new Response { Code = 500, Message = ex.Message };

            }

        }

        private async Task<Response> DeleteRealTimeStationTaskRecordAsync(List<int> mainIds)
        {
            var result = new Response();

            var entityList = await _dBContext.Proc_StationTask_Records.Where(d => mainIds.Contains(d.Proc_StationTask_MainId)).ToListAsync();
            if (entityList != null && entityList.Count > 0)
            {
                result = await DeleteRealTimeScanAccountAsync(entityList.Select(m => m.Id).ToList());
                if (result.Code != 200)
                    return result;

                result = await DeleteRealTimeGluingTimeAsync(entityList.Select(m => m.Id).ToList());
                if (result.Code != 200)
                    return result;

                result = await DeleteRealTimeBoltInfoAsync(entityList.Select(m => m.Id).ToList());
                if (result.Code != 200)
                    return result;

                result = await DeleteRealTimeBomInfoAsync(entityList.Select(m => m.Id).ToList());
                if (result.Code != 200)
                    return result;
                _dBContext.Proc_StationTask_Records.RemoveRange(entityList);

                return result;
            }
            else
                return result;
        }

        private async Task<Response> DeleteRealTimeScanAccountAsync(List<int> recordIds)
        {
            var entityList = await _dBContext.Proc_StationTask_ScanAccountCards.Where(d => recordIds.Contains(d.StationTask_RecordId)).ToListAsync();
            if (entityList != null && entityList.Count > 0)
            {
                _dBContext.Proc_StationTask_ScanAccountCards.RemoveRange(entityList);
            }
            return new Response() { Code = 200 };
        }

        private async Task<Response> DeleteRealTimeGluingTimeAsync(List<int> recordIds)
        {
            var entityList = await _dBContext.Proc_StationTask_CheckTimeouts.Where(d => recordIds.Contains(d.StationTask_RecordId)).ToListAsync();
            if (entityList != null && entityList.Count > 0)
            {
                _dBContext.Proc_StationTask_CheckTimeouts.RemoveRange(entityList);
            }
            return new Response() { Code = 200 };
        }

        private async Task<Response> DeleteRealTimeAutoBoltInfoAsync(List<int> mainIds)
        {
            var entityList = await _dBContext.Proc_AutoBoltInfo_Details.Where(d => mainIds.Contains((int)d.Proc_StationTask_MainId)).ToListAsync();
            if (entityList != null && entityList.Count > 0)
            {
                _dBContext.Proc_AutoBoltInfo_Details.RemoveRange(entityList);
            }
            return new Response() { Code = 200 };
        }
        private async Task<Response> DeleteRealTimeGluingInfoAsync(List<int> mainIds)
        {
            var entityList = await _dBContext.Proc_GluingInfos.Where(d => mainIds.Contains((int)d.Proc_StationTask_MainId)).ToListAsync();
            if (entityList != null && entityList.Count > 0)
            {
                _dBContext.Proc_GluingInfos.RemoveRange(entityList);
            }
            return new Response() { Code = 200 };
        }

        private async Task<Response> DeleteRealTimeBoltInfoAsync(List<int> recordIds)
        {
            var result = new Response();
            var entityList = await _dBContext.Proc_StationTask_BlotGuns.Where(d => recordIds.Contains(d.StationTask_RecordId)).ToListAsync();
            if (entityList != null && entityList.Count > 0)
            {
                result = await DeleteRealTimeBlotGunDetailsAsync(entityList.Select(m => m.Id).ToList());
                if (result.Code != 200)
                    return result;

                _dBContext.Proc_StationTask_BlotGuns.RemoveRange(entityList);
            }
            return new Response() { Code = 200 };
        }

        private async Task<Response> DeleteRealTimeBlotGunDetailsAsync(List<int> boltIds)
        {
            var entityList = await _dBContext.Proc_StationTask_BlotGunDetails.Where(d => boltIds.Contains(d.Proc_StationTask_BlotGunId)).ToListAsync();
            if (entityList != null && entityList.Count > 0)
            {
                _dBContext.Proc_StationTask_BlotGunDetails.RemoveRange(entityList);
            }
            return new Response() { Code = 200 };
        }

        private async Task<Response> DeleteRealTimeBomInfoAsync(List<int> recordIds)
        {
            var result = new Response();
            var entityList = await _dBContext.Proc_StationTask_Boms.Where(d => recordIds.Contains(d.StationTask_RecordId)).ToListAsync();
            if (entityList != null && entityList.Count > 0)
            {
                result = await DeleteRealTimeBomDetailsAsync(entityList.Select(m => m.Id).ToList());
                if (result.Code != 200)
                    return result;

                _dBContext.Proc_StationTask_Boms.RemoveRange(entityList);
            }
            return new Response() { Code = 200 };
        }

        private async Task<Response> DeleteRealTimeBomDetailsAsync(List<int> boltIds)
        {
            var entityList = await _dBContext.Proc_StationTask_BomDetails.Where(d => boltIds.Contains(d.Proc_StationTask_BomId)).ToListAsync();
            if (entityList != null && entityList.Count > 0)
            {
                _dBContext.Proc_StationTask_BomDetails.RemoveRange(entityList);
            }
            return new Response() { Code = 200 };
        }
    }
}
