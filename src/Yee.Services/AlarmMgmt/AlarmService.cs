using AsZero.Core.Services.Repos;
using AsZero.DbContexts;

using Microsoft.EntityFrameworkCore;

using Yee.Entitys.AlarmMgmt;
using Yee.Entitys.DTOS;
using Yee.Entitys.Request;
using Yee.Services.Request;

namespace Yee.Services.AlarmMgmt
{
    public class AlarmService
    {
        public readonly AsZeroDbContext _dBContext;
        public AlarmService(AsZeroDbContext dBContext)
        {
            _dBContext = dBContext;
        }

        public async Task<List<Alarm>> GetAll()
        {
            var list = await _dBContext.Alarms.Where(a => a.IsDeleted == false && a.IsFinish == false).ToListAsync();
            return list;
        }
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<List<Alarm>> GetList(AlarmDto dto)
        {
            var query = this._dBContext.Alarms.Where(o => !o.IsDeleted);
            if (dto.StationCode == "全部") 
            {
                dto.StationCode = string.Empty;
            }
            if (!string.IsNullOrEmpty(dto.StationCode))
            {
                query = query.Where(o => o.StationCode == dto.StationCode);
            }

            if (dto.EndTime != null)
            {
                query = query.Where(o => o.OccurTime.Date <= dto.EndTime.Value.Date);
            }

            if (dto.BeginTime != null)
            {
                query = query.Where(o => o.OccurTime.Date >= dto.BeginTime.Value.Date);
            }

            //query = query.Include(o => o.Station);
            return await query.ToListAsync();
        }
        public async Task<List<Alarm>> GetAlarmsByStation(string stationCode)
        {
            List<Alarm> list = new List<Alarm>();
            if (!string.IsNullOrEmpty(stationCode))
            {
                list = await _dBContext.Alarms.Where(a => a.IsDeleted == false && a.StationCode == stationCode && a.IsFinish == false).ToListAsync();
            }
            return list;
        }

        public async Task<List<Alarm>> Occur(AlarmDTO dto)
        {
            List<Alarm> list = new List<Alarm>();
            if (dto != null)
            {
                Alarm entity = new Alarm();
                entity.StationCode = dto.StationCode;
                entity.Code = dto.Code;
                entity.Name = dto.Name;
                entity.Description = dto.Description;
                entity.Module = dto.Module;
                entity.OccurTime = dto.OccurTime;
                entity.IsFinish = dto.IsFinish;
                entity.ClearDate = DateTime.Now;
                await _dBContext.AddAsync(entity);
                _dBContext.SaveChanges();

                list = await GetAlarmsByStation(dto.StationCode);
            }
            return list;
        }

        public async Task<List<Alarm>> Clear(AlarmDTO dto)
        {
            List<Alarm> list = new List<Alarm>();
            if (dto != null)
            {
                var station = await _dBContext.Base_Stations.Where(s => s.Code == dto.StepCode).FirstOrDefaultAsync();
                if (station != null)
                {
                    var entity = _dBContext.Alarms.Where(a => a.IsDeleted == false && a.IsFinish == false && a.Code == dto.Code).FirstOrDefault();
                    if (entity != null)
                    {
                        entity.ClearDate = DateTime.Now;
                        entity.IsFinish = true;
                        _dBContext.Alarms.Update(entity);
                        _dBContext.SaveChanges();
                    }
                }
                list = await GetAlarmsByStation(dto.StepCode);
            }
            return list;
        }
        /// <summary>
        /// 关闭工位的全部错误消息
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<List<Alarm>> ClearALL(List<AlarmDTO> dtos)
        {
            List<Alarm> list = new List<Alarm>();
            if (dtos != null)
            {
                //var station = await _dBContext.Base_Stations.Where(s => s.Code == dtos.First().StepCode).FirstOrDefaultAsync();
                foreach (var dto in dtos)
                {
                    //if (station != null)
                    {
                        Alarm entity = new Alarm();
                        entity.StationCode = dto.StationCode;
                        entity.Code = dto.Code;
                        entity.Name = dto.Name;
                        entity.Description = dto.Description;
                        entity.Module = dto.Module;   
                        entity.PackCode = dto.PackCode;
                        entity.OccurTime = dto.OccurTime;
                        entity.IsFinish = true;
                        entity.ClearDate = DateTime.Now;
                        await _dBContext.AddAsync(entity);

                    }
                }
                _dBContext.SaveChanges();
                //list = await GetAlarmsByStation(dto.StepCode);
            }
            return list;
        }
        /// <summary>
        /// 通过ID查询
        /// </summary>
        public async Task<Alarm?> GetById(int id)
        {
            var entity = await _dBContext.Alarms.FindAsync(id);
            return entity;
        }

        public async Task Delete(Alarm alarm)
        {
            alarm.IsDeleted = true;
            _dBContext.Alarms.Update(alarm);
            await _dBContext.SaveChangesAsync();
        }

        public async Task<Response<List<Alarm>>> GetPageList(AlarmGetPageListRequest request)
        {
            var result = new Response<List<Alarm>>();
            try
            {
                var query = _dBContext.Alarms.Where(e => e.IsDeleted == false);

                if (request.BeginDate.HasValue)
                {
                    request.BeginDate = request.BeginDate.GetValueOrDefault().Date;
                    query = query.Where(e => e.OccurTime >= request.BeginDate);
                }
                if (request.EndDate.HasValue)
                {
                    request.EndDate = request.EndDate.GetValueOrDefault().Date.AddDays(1);
                    query = query.Where(e => e.OccurTime < request.EndDate);
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
