using AsZero.Core.Entities;
using AsZero.Core.Services.Repos;
using AsZero.Core.Services.Sys_Logs;
using AsZero.DbContexts;

using Microsoft.EntityFrameworkCore;

using Yee.Common.Library.CommonEnum;
using Yee.Entitys.DBEntity;
using Yee.Entitys.DTOS;

namespace Yee.Services.Production
{
    public class Proc_StationTask_Main_StatusQueryService
    {
        private readonly AsZeroDbContext _dBContext;
        private readonly SysLogService sysLogService;

        public Proc_StationTask_Main_StatusQueryService(AsZeroDbContext asZeroDbContext, SysLogService sysLogService)
        {
            this._dBContext = asZeroDbContext;
            this.sysLogService = sysLogService;
        }

        public async Task<(bool, List<Proc_StationTask_Main>?)> Load(string? packNumber)
        {
            try
            {
                var entitys = await this._dBContext.Proc_StationTask_Mains
                    .AsNoTracking()
                    .Where(s => !s.IsDeleted && s.PackCode == packNumber)
                    .Include(s => s.Station)
                    .Include(s => s.CreateUser)
                    .OrderBy(s => s.StationId)
                    .ToListAsync();
                if (entitys.Count == 0)
                {
                    return (false, null);
                }
                else
                {
                    return (true, entitys);
                }
            }
            catch (Exception ex)
            {
                return (false, null);
            }
        }
        /// <summary>
        /// 根据Id强制完成工位完成状态
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<(bool, string)> OnStatusById(int? id,string? user)
        {
            try
            {
                var entitys = await this._dBContext.Proc_StationTask_Mains.Where(s => s.Id == id && !s.IsDeleted).FirstOrDefaultAsync();
                if (entitys != null)
                {
                    entitys.Status = StationTaskStatusEnum.已完成;
                    entitys.UpdateTime = DateTime.Now;
                    var entitys_Record = await this._dBContext.Proc_StationTask_Records
                        .Where(s => s.Proc_StationTask_MainId == entitys.Id && !s.IsDeleted && s.Status != StationTaskStatusEnum.已完成)
                        .ToListAsync();
                    foreach (var record in entitys_Record)
                    {
                        record.Status = StationTaskStatusEnum.已完成;
                        record.UpdateTime = DateTime.Now;
                    }
                    var stationname = await this._dBContext.Base_Stations.Where(s => s.Id == entitys.StationId && !s.IsDeleted).FirstOrDefaultAsync();
                    await sysLogService.AddLog(new SysLog() { LogType = Sys_LogType.强制完工, Message = $"站点：{stationname.Name},Pack码{entitys.PackCode} ", Operator = user });
                    await this._dBContext.SaveChangesAsync();
                    return (true, string.Empty);
                }
                return (false, $"找不到该Id={id}数据，未开始？已完成？已删除？");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<Response> ClearBomById(int? id)
        {
            var result = new Response();
            try
            {
                var records = await _dBContext.Proc_StationTask_Records.Include(i => i.Base_StationTask).Where(w => !w.IsDeleted && w.Proc_StationTask_MainId == id && w.Base_StationTask.Type == StationTaskTypeEnum.扫码).ToListAsync();

                foreach(var item in records)
                {
                    item.IsDeleted = true;

                    var bomdetail = await _dBContext.Proc_StationTask_BomDetails.Include(i => i.Proc_StationTask_Bom).Where(w => !w.IsDeleted && w.Proc_StationTask_Bom!.StationTask_RecordId == item.Id).FirstOrDefaultAsync();
                    bomdetail.Proc_StationTask_Bom!.IsDeleted = true;
                    bomdetail.IsDeleted = true;
                    _dBContext.Update(bomdetail);
                }
                _dBContext.UpdateRange(records);
                await _dBContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.Message;
            }

            return result;
        }

        public async Task<Response> ClearDataById(int? id)
        {
            var result = new Response();
            try
            {
                var records = await _dBContext.Proc_StationTask_Records.Include(i => i.Base_StationTask).Where(w => !w.IsDeleted && w.Proc_StationTask_MainId == id && w.Base_StationTask.Type != StationTaskTypeEnum.扫码).ToListAsync();

                foreach (var item in records)
                {
                    item.IsDeleted = true;
                }
                _dBContext.UpdateRange(records);
                await _dBContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.Message;
            }

            return result;
        }
        /// <summary>
        /// 根据Id强制未完成工位完成状态
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<(bool, string)> OffStatusById(int? id, string? user)
        {
            try
            {
                var entitys = await this._dBContext.Proc_StationTask_Mains.Where(s => s.Id == id && !s.IsDeleted).FirstOrDefaultAsync();
                if (entitys != null)
                {
                    entitys.Status = StationTaskStatusEnum.进行中;
                    entitys.UpdateTime = DateTime.Now;
                    var entitys_Record = await this._dBContext.Proc_StationTask_Records
                        .Where(s => s.Proc_StationTask_MainId == entitys.Id && !s.IsDeleted && s.Status == StationTaskStatusEnum.已完成)
                        .Include(s => s.Base_StationTask)
                        .ToListAsync();
                    foreach (var record in entitys_Record)
                    {
                        if (record.Base_StationTask != null && record.Base_StationTask.Type == StationTaskTypeEnum.放行)
                        {
                            record.Status = StationTaskStatusEnum.进行中;
                            record.UpdateTime = DateTime.Now;
                        }
                    }
                    var stationname = await this._dBContext.Base_Stations.Where(s => s.Id == entitys.StationId && !s.IsDeleted).FirstOrDefaultAsync();
                    await sysLogService.AddLog(new SysLog() { LogType = Sys_LogType.强制未完工, Message = $"站点：{stationname.Name},Pack码{entitys.PackCode} ", Operator = user });
                    await this._dBContext.SaveChangesAsync();
                    return (true, string.Empty);
                }
                return (false, $"找不到该Id={id}数据，未开始？已完成？");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// 根据Main表Id查询所有精追扫码记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<(bool, StationPack_Task_Record_DTO?)> Load_RecordByMainId(int? id)
        {

            var resultDTO = new StationPack_Task_Record_DTO { Proc_StationTask_Record_DTO_List = new List<Proc_StationTask_Record_DTO>() };

            try
            {
               var main= await this._dBContext.Proc_StationTask_Mains.Include(s=>s.Station).FirstOrDefaultAsync(s => s.Id == id&&!s.IsDeleted);
                if (main != null && main.Station != null && main.Station.Code != null)
                {
                    resultDTO.PackCode = main.PackCode;
                    resultDTO.StationCode = main.Station.Code;
                }

                var entitys = await this._dBContext.Proc_StationTask_Records
                    .AsNoTracking()
                    .Where(s => !s.IsDeleted && s.Proc_StationTask_MainId == id&&!s.Base_StationTask.IsDeleted)
                    .Include(s => s.Base_StationTask)
                    .ToListAsync();
                foreach (var entity in entitys)
                {
                    if (entity.Base_StationTask != null)
                    {
                        var record = new Proc_StationTask_Record_DTO();
                        record.Id = entity.Id;
                        record.OrderNo = entity.Base_StationTask.Sequence;
                        record.TaskName = entity.TaskName;
                        record.Status = entity.Status;
                        record.TracingType = TracingTypeEnum.无;
                        record.CreateTime = entity.CreateTime;

                        if (entity.Base_StationTask.Type == StationTaskTypeEnum.扫码)
                        {
                            var boms = await _dBContext.Proc_StationTask_Boms.Where(s => s.StationTask_RecordId == entity.Id && !s.IsDeleted).ToListAsync();
                            foreach (var bom in boms)
                            {
                                var bomDetail = await _dBContext.Proc_StationTask_BomDetails.FirstOrDefaultAsync(s => s.Proc_StationTask_BomId == bom.Id && !s.IsDeleted);
                                if (bomDetail != null)
                                {
                                    record.TracingType = bomDetail.TracingType;
                                    record.GoodsOuterCode = bomDetail.GoodsOuterCode;
                                    record.BatchBarCode = bomDetail.BatchBarCode;
                                    record.UniBarCode = bomDetail.UniBarCode;
                                    record.HasUpMesDone = bomDetail.HasUpMesDone;
                                }
                            }
                            resultDTO.Proc_StationTask_Record_DTO_List.Add(record);
                        }
                    }
                }
                return (true, resultDTO);
            }
            catch (Exception ex)
            {
                return (false, null);
            }
        }
        /// <summary>
        /// 踢料
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<(bool, string)> Delete_RecordById(int? id,string? user)
        {
            var res = await _dBContext.Proc_StationTask_Records.Include(s => s.Base_StationTask).FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
            if (res == null)
            {
                return (false, $"找不到此Id={id}的数据");
            }
            if (res.Base_StationTask == null)
            {
                return (false, $"找不到id={id}的工位配方");
            }
            var type = res.Base_StationTask.Type;
            switch (type)
            {
                case StationTaskTypeEnum.扫码:
                    var boms = await _dBContext.Proc_StationTask_Boms.Where(s => s.StationTask_RecordId == res.Id && !s.IsDeleted).ToListAsync();
                    foreach (var bom in boms)
                    {
                        bom.IsDeleted = true;
                        var bomDetail = await _dBContext.Proc_StationTask_BomDetails.FirstOrDefaultAsync(s => s.Proc_StationTask_BomId == bom.Id && !s.IsDeleted);
                        if (bomDetail != null)
                            bomDetail.IsDeleted = true;
                        var main = await this._dBContext.Proc_StationTask_Mains.Where(s => s.Id == res.Proc_StationTask_MainId && !s.IsDeleted).FirstOrDefaultAsync();
                        var stationname = await this._dBContext.Base_Stations.Where(s => s.Id == main.StationId && !s.IsDeleted).FirstOrDefaultAsync();
                        await sysLogService.AddLog(new SysLog() { LogType = Sys_LogType.踢料, Message = $"站点：{stationname.Name},Pack码{main.PackCode},批次码{bomDetail.BatchBarCode}，精追码{bomDetail.GoodsOuterCode},PN{bomDetail.GoodsPN},任务名称{bomDetail.GoodsName} ", Operator = user });
                    }

                    break;
                default:
                    return (false, "只有扫码类型可踢料");

            }
            await _dBContext.SaveChangesAsync();
            return (true, string.Empty);
        }
        /// <summary>
        /// 强制已完成上传MES
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<(bool, string)> HasUpMes_RecordById(int? id,bool status)
        {
            var res = await _dBContext.Proc_StationTask_Records.Include(s => s.Base_StationTask).FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
            if (res == null)
            {
                return (false, $"找不到此Id={id}的数据");
            }
            if (res.Base_StationTask == null)
            {
                return (false, $"找不到id={id}的工位配方");
            }
            var type = res.Base_StationTask.Type;
            switch (type)
            {
                case StationTaskTypeEnum.扫码:
                    var boms = await _dBContext.Proc_StationTask_Boms.Where(s => s.StationTask_RecordId == res.Id && !s.IsDeleted).ToListAsync();
                    foreach (var bom in boms)
                    {
                        var bomDetail = await _dBContext.Proc_StationTask_BomDetails.FirstOrDefaultAsync(s => s.Proc_StationTask_BomId == bom.Id && !s.IsDeleted);
                        if (bomDetail != null)
                            bomDetail.HasUpMesDone = status;
                    }
                    break;
                default:
                    return (false, "只有扫码类型可强制以完成上传MES");

            }
            await _dBContext.SaveChangesAsync();
            return (true, string.Empty);
        }

        public async Task<Response> CheckPreTighteningCompleted(string SFC)
        {
            var result = new Response();
            try
            {
                var mainRecord = await _dBContext.Proc_StationTask_Mains
                    .Include(s => s.Step)
                    .Where(s => s.PackCode == SFC && !s.IsDeleted && s.Step != null && s.Step.Name == "上盖预拧")
                    .FirstOrDefaultAsync();

                if (mainRecord == null)
                {
                    result.Code = 500;
                    result.Data = false;
                    result.Message = $"未找到Pack码[{SFC}]对应的上盖预拧记录";
                    return result;
                }

                var isCompleted = mainRecord.Status == StationTaskStatusEnum.已完成;

                result.Code = 200;
                result.Data = isCompleted;
                result.Message = isCompleted ? $"Pack码[{SFC}]的上盖预拧已完成" : $"Pack码[{SFC}]的上盖预拧未完成";
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
