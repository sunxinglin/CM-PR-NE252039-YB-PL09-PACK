using AsZero.Core.Services.Repos;
using AsZero.DbContexts;

using Itminus.FSharpExtensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.FSharp.Core;

using Yee.Common.Library.CommonEnum;
using Yee.Entitys;
using Yee.Entitys.AutomaticStation;
using Yee.Entitys.CATL;
using Yee.Entitys.DBEntity;
using Yee.Entitys.DBEntity.ProductionRecords;
using Yee.Entitys.DTOS.AutomaticStationDTOS;
using Yee.Entitys.Production;
using Yee.Services.CatlMesInvoker;

namespace Yee.Services.AutomaticStation
{
    public class LowerBoxGlueService
    {
        private readonly AsZeroDbContext _dbContext;
        private readonly AutomicCommonService _commonService;
        private readonly ICatlMesInvoker _mesInvoker;

        public LowerBoxGlueService(AsZeroDbContext dbContext, AutomicCommonService commonService, ICatlMesInvoker mesInvoker)
        {
            _dbContext = dbContext;
            _commonService = commonService;
            _mesInvoker = mesInvoker;
        }

        public async Task<FSharpResult<ServiceErrResponse, ServiceErrResponse>> SaveDataAndUploadCATL(LowerBoxGlueSaveDataDto dto)
        {
            try
            {
                var q = from main in _commonService.HasMainRecord(dto.PackCode, dto.StationCode)
                        from productPn in _commonService.GetProductPnFromCatlMes(dto.PackCode, dto.StationCode)
                        from formulas in _commonService.GetBaseStationTasks(productPn.BarCode_GoodsPN, dto.StationCode)
                        from glueStartTimeDC in this.DealStartTime(formulas.FirstOrDefault(f => f.Type == StationTaskTypeEnum.时间记录), dto, main.Id)
                        from glueDataDC in this.DealData(formulas.Where(f => f.Type == StationTaskTypeEnum.下箱体涂胶).ToList(), dto, main.Id)
                        from upMes in _commonService.UploadCatlMes(dto.PackCode, glueStartTimeDC.Concat(glueDataDC).ToList(), true, dto.StationCode)
                        from setMainComp in _commonService.SetStationComplete(main.Id)//设置main完工
                        select productPn;

                var r = await q;
                if (r.IsError)
                {
                    return r.ErrorValue.ToErrResult<ServiceErrResponse, ServiceErrResponse>();
                }
                var resp = new ServiceErrResponse();
                return resp.ToSuccess().ToOkResult<ServiceErrResponse, ServiceErrResponse>();
            }
            catch (Exception ex)
            {
                var resp = new ServiceErrResponse();
                return resp.ToError(ResponseErrorType.上位机错误, 500, ex.Message).ToErrResult<ServiceErrResponse, ServiceErrResponse>();
            }
        }

        private async Task<FSharpResult<IList<DcParamValue>, ServiceErrResponse>> DealStartTime(Base_StationTask? stationTask, LowerBoxGlueSaveDataDto dto, int MainId)
        {
            using var tras = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var dcParam = new List<DcParamValue>();
                if (stationTask == null)
                {   //没有涂胶开始时间配方则直接返回
                    return dcParam.ToOkResult<IList<DcParamValue>, ServiceErrResponse>();
                }
                var resp = new ServiceErrResponse();
                if (stationTask.Type != StationTaskTypeEnum.时间记录)
                {
                    return resp.ToError(ResponseErrorType.上位机错误, 500, $"配方类型有误，需要类型-{StationTaskTypeEnum.时间记录}， 实际类型-{stationTask.Type}").ToErrResult<IList<DcParamValue>, ServiceErrResponse>();
                }

                var timeRecordFormu = await _dbContext.Base_StationTask_RecordTimes.FirstOrDefaultAsync(f => f.StationTaskId == stationTask.Id && !f.IsDeleted);
                if (timeRecordFormu == null)
                {
                    return resp.ToError(ResponseErrorType.上位机错误, 500, $"{stationTask.Name}的任务详情未配置").ToErrResult<IList<DcParamValue>, ServiceErrResponse>();
                }
                var record = await _dbContext.Proc_StationTask_Records.FirstOrDefaultAsync(f => !f.IsDeleted && f.Proc_StationTask_MainId == MainId && f.Base_StationTaskId == stationTask.Id);
                if (record == null)
                {
                    record = new Proc_StationTask_Record();
                    record.Proc_StationTask_MainId = MainId;
                    record.Base_StationTaskId = stationTask.Id;
                    record.TaskName = stationTask.Name ?? "";
                    record.Status = StationTaskStatusEnum.已完成;
                    await _dbContext.AddAsync(record);
                    await _dbContext.SaveChangesAsync();
                }

                var timeHis = await _dbContext.Proc_StationTask_TimeRecords.Where(w => w.Proc_StationTask_RecordId == record.Id && !w.IsDeleted).ToListAsync();
                _dbContext.RemoveRange(timeHis);

                var newTime = new Proc_StationTask_TimeRecord()
                {
                    Proc_StationTask_RecordId = record.Id,
                    TimeFlag = timeRecordFormu.TimeOutFlag,
                    SerialCode = dto.PackCode,
                    StationCode = dto.StationCode,
                    UploadMesCode = timeRecordFormu.UpMesCode,
                    TimeValue = dto.GlueStartTime,
                };
                await _dbContext.AddAsync(newTime);
                await _dbContext.SaveChangesAsync();

                await tras.CommitAsync();
                dcParam.Add(new DcParamValue
                {
                    DataType = ValueTypeEnum.TEXT,
                    ParamValue = dto.GlueStartTime,
                    UpMesCode = timeRecordFormu.UpMesCode,

                });
                return dcParam.ToOkResult<IList<DcParamValue>, ServiceErrResponse>();
            }
            catch (Exception ex)
            {
                await tras.RollbackAsync();

                var resp = new ServiceErrResponse();
                return resp.ToError(ResponseErrorType.上位机错误, 500, ex.Message).ToErrResult<IList<DcParamValue>, ServiceErrResponse>();
            }
        }

        private async Task<FSharpResult<IList<DcParamValue>, ServiceErrResponse>> DealData(IList<Base_StationTask> stationTask, LowerBoxGlueSaveDataDto dto, int MainId)
        {
            try
            {
                var resp = new ServiceErrResponse();

                if (dto.GlueDatas == null)
                {
                    return resp.ToError(ResponseErrorType.数据异常, 500, $"没有提供涂胶数据，请检查后重试").ToErrResult<IList<DcParamValue>, ServiceErrResponse>();
                }

                //没有配方则直接返回
                if (stationTask.Count == 0)
                {
                    return resp.ToError(ResponseErrorType.上位机错误, 500, $"没有配置下箱体涂胶任务，请检查后重试").ToErrResult<IList<DcParamValue>, ServiceErrResponse>();
                }

                var formulaType = stationTask.Any(a => a.Type != StationTaskTypeEnum.下箱体涂胶);
                if (formulaType)
                {
                    return resp.ToError(ResponseErrorType.上位机错误, 500, $"存在配方类型不正确的项，请检查后重试").ToErrResult<IList<DcParamValue>, ServiceErrResponse>();
                }
                //删除旧的
                var historyDatas = await _dbContext.Proc_LowerBoxGlueInfos.Where(e => e.IsDeleted == false && e.Proc_StationTask_MainId == MainId).ToListAsync();
                _dbContext.RemoveRange(historyDatas);

                //创建新的
                var glueInfos = new List<Proc_LowerBoxGlueInfo>();
                foreach (var item in stationTask)
                {
                    var glueFormulas = await _dbContext.Base_StationTask_LowerBoxGlues.Where(w => w.StationTaskId == item.Id && !w.IsDeleted).ToListAsync();
                    foreach (var formu in glueFormulas)
                    {
                        var glueData = dto.GlueDatas[formu.ParameterName];
                        if (glueData == null)
                        {
                            return resp.ToError(ResponseErrorType.上位机错误, 500, $"{formu.ParameterName}未找到对应的上传数据，请检查后重试").ToErrResult<IList<DcParamValue>, ServiceErrResponse>();
                        }

                        //校验值范围
                        _ = decimal.TryParse(glueData, out decimal data);
                        if (formu.MinValue.HasValue && data < formu.MinValue)
                        {
                            return resp.ToError(ResponseErrorType.上位机错误, 500, $"{formu.ParameterName}不在管控范围[{formu.MinValue}, {formu.MaxValue}]内，请检查后重试").ToErrResult<IList<DcParamValue>, ServiceErrResponse>();
                        }
                        if (formu.MaxValue.HasValue && data > formu.MaxValue)
                        {
                            return resp.ToError(ResponseErrorType.上位机错误, 500, $"{formu.ParameterName}不在管控范围[{formu.MinValue}, {formu.MaxValue}]内，请检查后重试").ToErrResult<IList<DcParamValue>, ServiceErrResponse>();
                        }

                        var glueInfo = new Proc_LowerBoxGlueInfo
                        {
                            Proc_StationTask_MainId = MainId,
                            PackPN = dto.PackCode,
                            StationCode = dto.StationCode ?? string.Empty,
                            ParamName = formu.ParameterName,
                            UploadMesCode = formu.UpMesCode,
                            Value = glueData,
                            CreateTime = DateTime.Now,
                        };
                        glueInfos.Add(glueInfo);
                    }
                }

                await _dbContext.AddRangeAsync(glueInfos);
                await _dbContext.SaveChangesAsync();
                var dcParams = glueInfos.Select(s => new DcParamValue { DataType = ValueTypeEnum.NUMBER, ParamValue = s.Value.ToString(), UpMesCode = s.UploadMesCode }).ToList();

                return dcParams.ToOkResult<IList<DcParamValue>, ServiceErrResponse>();
            }
            catch (Exception ex)
            {
                var resp = new ServiceErrResponse();
                return resp.ToError(ResponseErrorType.上位机错误, 500, ex.Message).ToErrResult<IList<DcParamValue>, ServiceErrResponse>();
            }
        }

        public async Task<Response<IList<Proc_LowerBoxGlueInfo>>> LoadData(LowerBoxGlueLoadDataDto dto)
        {
            var resp = new Response<IList<Proc_LowerBoxGlueInfo>>();
            try
            {
                var datas = await _dbContext.Proc_LowerBoxGlueInfos.Where(w => !w.IsDeleted).OrderByDescending(o => o.Id).ToListAsync();
                if (!string.IsNullOrWhiteSpace(dto.PackCode))
                {
                    datas = datas.Where(w => w.PackPN == dto.PackCode).ToList();
                }
                if (dto.BeginTime.HasValue)
                {
                    datas = datas.Where(w => w.CreateTime >= dto.BeginTime).ToList();
                }
                if (dto.EndTime.HasValue)
                {
                    datas = datas.Where(w => w.CreateTime < dto.EndTime.Value.AddDays(1)).ToList();
                }

                //if (!string.IsNullOrEmpty(dto.StationCode))
                //{
                //    datas = datas.Where(w => w.StationCode == dto.StationCode).ToList();
                //}
                resp.Count = datas.Count();
                resp.Result = datas.Skip(dto.Limit * (dto.Page - 1)).Take(dto.Limit).ToList();
            }
            catch (Exception ex)
            {
                resp.Code = 500;
                resp.Message = ex.Message;

            }
            return resp;
        }


        public async Task<Response<IList<Proc_StationTask_TimeRecord>>> LoadTimeData(LowerBoxGlueLoadDataDto dto)
        {
            var resp = new Response<IList<Proc_StationTask_TimeRecord>>();
            try
            {
                var datas = await _dbContext.Proc_StationTask_TimeRecords.Where(w => !w.IsDeleted && w.TimeFlag == "LowerBoxGlueStartTime").OrderByDescending(o => o.Id).ToListAsync();
                if (!string.IsNullOrWhiteSpace(dto.PackCode))
                {
                    datas = datas.Where(w => w.SerialCode == dto.PackCode).ToList();
                }
                if (dto.BeginTime.HasValue)
                {
                    datas = datas.Where(w => Convert.ToDateTime(w.TimeValue) >= dto.BeginTime).ToList();
                }
                if (dto.EndTime.HasValue)
                {
                    datas = datas.Where(w => Convert.ToDateTime(w.TimeValue) < dto.EndTime.Value.AddDays(1)).ToList();
                }

                //if (!string.IsNullOrEmpty(dto.StationCode))
                //{
                //    datas = datas.Where(w => w.StationCode == dto.StationCode).ToList();
                //}
                resp.Count = datas.Count();
                resp.Result = datas.Skip(dto.Limit * (dto.Page - 1)).Take(dto.Limit).ToList();
            }
            catch (Exception ex)
            {
                resp.Code = 500;
                resp.Message = ex.Message;

            }
            return resp;
        }

        public async Task<Response<IList<Proc_StationTask_TimeRecord>>> LoadTimeDataUpdate(LowerBoxGlueLoadDataDto dto)
        {
            var resp = new Response<IList<Proc_StationTask_TimeRecord>>();
            try
            {
                var datas = await _dbContext.Proc_StationTask_TimeRecords.Where(w => !w.IsDeleted && w.TimeFlag == "LowerBoxGlueStartTime").OrderByDescending(o => o.Id).ToListAsync();
                if (!string.IsNullOrWhiteSpace(dto.PackCode))
                {
                    datas = datas.Where(w => w.SerialCode == dto.PackCode).ToList();
                }
                if (dto.BeginTime.HasValue)
                {
                    datas = datas.Where(w => Convert.ToDateTime(w.TimeValue) >= dto.BeginTime).ToList();
                }
                if (dto.EndTime.HasValue)
                {
                    datas = datas.Where(w => Convert.ToDateTime(w.TimeValue) < dto.EndTime.Value.AddDays(1)).ToList();
                }

                //if (!string.IsNullOrEmpty(dto.StationCode))
                //{
                //    datas = datas.Where(w => w.StationCode == dto.StationCode).ToList();
                //}
                resp.Count = datas.Count();
                resp.Result = datas.Skip(dto.Limit * (dto.Page - 1)).Take(dto.Limit).ToList();
            }
            catch (Exception ex)
            {
                resp.Code = 500;
                resp.Message = ex.Message;

            }
            return resp;
        }

        public async Task<Response<CatlMESReponse>> UploadDataAgain(GlueDataDto dto)
        {
            var resp = new Response<CatlMESReponse>();
            try
            {
                var main = await _dbContext.Proc_StationTask_Mains.FirstOrDefaultAsync(f => f.PackCode == dto.PackCode && f.StationCode == dto.StationCode && !f.IsDeleted);
                if (main == null)
                {
                    resp.Code = 500;
                    resp.Message = $"主记录记录未找到";
                    return resp;
                }
                var datas = await _dbContext.Proc_LowerBoxGlueInfos.Where(w => w.Proc_StationTask_MainId == main.Id && !w.IsDeleted).ToListAsync();
                if (datas.Count <= 0)
                {
                    resp.Code = 500;
                    resp.Message = $"记录未找到";
                    return resp;
                }
                var dcparams = datas.Select(s => new DcParamValue { DataType = ValueTypeEnum.NUMBER, ParamValue = s.Value.ToString(), UpMesCode = s.UploadMesCode }).ToList();
                var records = await _dbContext.Proc_StationTask_Records.Where(w => w.Proc_StationTask_MainId == main.Id).ToListAsync();
                foreach (var item in records)
                {
                    var time = await _dbContext.Proc_StationTask_TimeRecords.Where(w => w.Proc_StationTask_RecordId == item.Id && !w.IsDeleted).ToListAsync();
                    dcparams = dcparams.Concat(time.Select(s => new DcParamValue { DataType = ValueTypeEnum.TEXT, ParamValue = s.TimeValue.ToString(), UpMesCode = s.UploadMesCode })).ToList();
                    break;
                }
                resp.Result = await _mesInvoker.dataCollect(main.PackCode, dcparams, dto.IsNeedChangeStatus, dto.StationCode ?? "");
            }
            catch (Exception ex)
            {
                resp.Code = 500;
                resp.Message = ex.Message;
            }
            return resp;
        }

        public async Task<FSharpResult<ServiceErrResponse, ServiceErrResponse>> UpdateDataAndUploadCATL(LowerBoxReGlueDataDto dto)
        {
            try
            {
                var resp = new ServiceErrResponse();
                var station = await _dbContext.Base_Stations.Include(e => e.Step).FirstOrDefaultAsync(f => f.Code == dto.StationCode);
                if (station == null)
                {
                    var ErrorMessage = $"工位{dto.StationCode}不存在";
                    return resp.ToError(ResponseErrorType.上位机错误, 500, ErrorMessage).ToErrResult<ServiceErrResponse, ServiceErrResponse>();
                }
                var main = await _dbContext.Proc_StationTask_Mains.FirstOrDefaultAsync(w => !w.IsDeleted && w.PackCode == dto.PackCode && w.StepId == station.StepId);
                if (main == null)
                {
                    var ErrorMessage = $"PACK码{dto.PackCode}没有在工位{dto.StationCode}的开始记录";
                    return resp.ToError(ResponseErrorType.上位机错误, 500, ErrorMessage).ToErrResult<ServiceErrResponse, ServiceErrResponse>();
                }
                var q = from productPn in _commonService.GetProductPnFromCatlMes(dto.PackCode, dto.StationCode)
                        from formulas in _commonService.GetBaseStationTasks(productPn.BarCode_GoodsPN, dto.StationCode)
                        from glueStartTimeDC in this.UpdateStartTime(formulas.FirstOrDefault(f => f.Type == StationTaskTypeEnum.时间记录), dto, main.Id)
                        from glueDataDC in this.DealUpdateData(formulas.Where(f => f.Type == StationTaskTypeEnum.下箱体涂胶).ToList(), dto, main.Id)
                        from upMes in _commonService.UploadCatlMes(dto.PackCode, glueStartTimeDC.Concat(glueDataDC).ToList(), false, dto.StationCode)
                        select productPn;

                var r = await q;
                if (r.IsError)
                {
                    return r.ErrorValue.ToErrResult<ServiceErrResponse, ServiceErrResponse>();
                }
                return resp.ToSuccess().ToOkResult<ServiceErrResponse, ServiceErrResponse>();
            }
            catch (Exception ex)
            {
                var resp = new ServiceErrResponse();
                return resp.ToError(ResponseErrorType.上位机错误, 500, ex.Message).ToErrResult<ServiceErrResponse, ServiceErrResponse>();
            }
        }

        private async Task<FSharpResult<IList<DcParamValue>, ServiceErrResponse>> UpdateStartTime(Base_StationTask? stationTask, LowerBoxReGlueDataDto dto, int MainId)
        {
            try
            {
                var dcParam = new List<DcParamValue>();
                if (stationTask == null)
                {   //没有涂胶开始时间配方则直接返回
                    return dcParam.ToOkResult<IList<DcParamValue>, ServiceErrResponse>();
                }
                var resp = new ServiceErrResponse();
                if (stationTask.Type != StationTaskTypeEnum.时间记录)
                {
                    return resp.ToError(ResponseErrorType.上位机错误, 500, $"配方类型有误，需要类型-{StationTaskTypeEnum.时间记录}， 实际类型-{stationTask.Type}").ToErrResult<IList<DcParamValue>, ServiceErrResponse>();
                }

                var timeRecordFormu = await _dbContext.Base_StationTask_RecordTimes.FirstOrDefaultAsync(f => f.StationTaskId == stationTask.Id && !f.IsDeleted);
                if (timeRecordFormu == null)
                {
                    return resp.ToError(ResponseErrorType.上位机错误, 500, $"{stationTask.Name}的任务详情未配置").ToErrResult<IList<DcParamValue>, ServiceErrResponse>();
                }
                var record = await _dbContext.Proc_StationTask_Records.FirstOrDefaultAsync(f => !f.IsDeleted && f.Proc_StationTask_MainId == MainId && f.Base_StationTaskId == stationTask.Id);
                if (record == null)
                {
                    return resp.ToError(ResponseErrorType.上位机错误, 500, $"未找到涂胶开始记录，无法进行补胶，请走正常涂胶模式").ToErrResult<IList<DcParamValue>, ServiceErrResponse>();
                }
                var timeHis = await _dbContext.Proc_StationTask_TimeRecords.FirstOrDefaultAsync(w => w.Proc_StationTask_RecordId == record.Id && !w.IsDeleted);
                if (timeHis == null)
                {
                    return resp.ToError(ResponseErrorType.上位机错误, 500, $"未找到涂胶时间，无法进行补胶，请走正常涂胶模式").ToErrResult<IList<DcParamValue>, ServiceErrResponse>();
                }
                timeHis.TimeValue = dto.GlueStartTime;
                timeHis.UpdateTime = DateTime.Now;

                await _dbContext.SaveChangesAsync();

                dcParam.Add(new DcParamValue
                {
                    DataType = ValueTypeEnum.TEXT,
                    ParamValue = dto.GlueStartTime,
                    UpMesCode = timeRecordFormu.UpMesCode,

                });
                return dcParam.ToOkResult<IList<DcParamValue>, ServiceErrResponse>();
            }
            catch (Exception ex)
            {
                var resp = new ServiceErrResponse();
                return resp.ToError(ResponseErrorType.上位机错误, 500, ex.Message).ToErrResult<IList<DcParamValue>, ServiceErrResponse>();
            }
        }

        private async Task<FSharpResult<IList<DcParamValue>, ServiceErrResponse>> DealUpdateData(IList<Base_StationTask> stationTask, LowerBoxReGlueDataDto dto, int MainId)
        {
            try
            {
                var resp = new ServiceErrResponse();
                var datas = await _dbContext.Proc_LowerBoxGlueInfos.Where(w => w.Proc_StationTask_MainId == MainId && !w.IsDeleted).ToListAsync();
                if (datas.Count <= 0)
                {
                    return resp.ToError(ResponseErrorType.上位机错误, 500, $"未找到涂胶上传历史数据，无法进行补胶，请走正常涂胶模式").ToErrResult<IList<DcParamValue>, ServiceErrResponse>();
                }
                var dcparams = datas.Select(s => new DcParamValue { DataType = ValueTypeEnum.NUMBER, ParamValue = s.Value.ToString(), UpMesCode = s.UploadMesCode }).ToList();
                return dcparams.ToOkResult<IList<DcParamValue>, ServiceErrResponse>();
            }
            catch (Exception ex)
            {
                var resp = new ServiceErrResponse();
                return resp.ToError(ResponseErrorType.上位机错误, 500, ex.Message).ToErrResult<IList<DcParamValue>, ServiceErrResponse>();
            }

        }

        public async Task<FSharpResult<DateTime, ServiceErrResponse>> GetGlueTime(string packCode, string timeFlag)
        {
            using var trans = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var resp = new ServiceErrResponse();
                var timeValue = await _dbContext.Proc_StationTask_TimeRecords.FirstOrDefaultAsync(f => !f.IsDeleted && f.SerialCode == packCode && f.TimeFlag == timeFlag);
                if (timeValue == null)
                {
                    return resp.ToError(ResponseErrorType.上位机错误, 500, $"PACK条码{packCode}不存在时间标志为{timeFlag}的时间记录").ToErrResult<DateTime, ServiceErrResponse>();
                }
                var time = DateTime.Parse(timeValue.TimeValue);
                return time.ToOkResult<DateTime, ServiceErrResponse>();
            }
            catch (Exception ex)
            {
                await trans.RollbackAsync();
                var resp = new ServiceErrResponse();
                return resp.ToError(ResponseErrorType.上位机错误, 500, ex.Message).ToErrResult<DateTime, ServiceErrResponse>();
            }
        }

    }
}
