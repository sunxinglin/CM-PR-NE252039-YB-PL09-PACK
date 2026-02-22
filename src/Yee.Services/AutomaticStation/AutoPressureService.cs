using AsZero.Core.Services.Repos;
using AsZero.DbContexts;
using Itminus.FSharpExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.FSharp.Core;
using Newtonsoft.Json;
using Yee.Common.Library.CommonEnum;
using Yee.Entitys;
using Yee.Entitys.AutomaticStation;
using Yee.Entitys.CATL;
using Yee.Entitys.DBEntity;
using Yee.Entitys.DBEntity.ProductionRecords;
using Yee.Entitys.DTOS;
using Yee.Entitys.Production;
using Yee.Services.CatlMesInvoker;

namespace Yee.Services.AutomaticStation
{
    public class AutoPressureService
    {
        private readonly AsZeroDbContext _dbContext;
        private readonly AutomicCommonService _commonService;
        private readonly ICatlMesInvoker _mesInvoker;

        public AutoPressureService(AsZeroDbContext dbContext, AutomicCommonService commonService, ICatlMesInvoker mesInvoker)
        {
            _dbContext = dbContext;
            _commonService = commonService;
            _mesInvoker = mesInvoker;
        }

        public async Task<FSharpResult<ServiceErrResponse, ServiceErrResponse>> SaveAndUploadPressureData(PressureDataUploadDto dto)
        {
            try
            {
                var q = from main in _commonService.HasMainRecord(dto.Pin, dto.StationCode)
                        from productPn in _commonService.GetProductPnFromCatlMes(dto.Pin, dto.StationCode)
                        from formulas in _commonService.GetBaseStationTasks(productPn.BarCode_GoodsPN, dto.StationCode)
                        from timeOutValue in this.CheckGlueTimeOut(productPn.BarCode_GoodsPN, dto.StationCode, dto.Pin, main.Id, dto)
                        from pressureStartTimeDC in this.DealCompleteTime(formulas.FirstOrDefault(f => f.Type == StationTaskTypeEnum.时间记录), dto, main.Id)
                        from pressureDataDC in this.DealPressureData(formulas.Where(f => f.Type == StationTaskTypeEnum.自动加压).ToList(), dto, main.Id)
                        let DCparam = pressureStartTimeDC.Concat(pressureDataDC).ToList().Concat(timeOutValue).ToList()
                        from upMes in _commonService.UploadCatlMes(dto.Pin, DCparam, true, dto.StationCode)
                        from mainComplate in _commonService.SetStationComplete(main.Id)
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

        public async Task<FSharpResult<IList<DcParamValue>, ServiceErrResponse>> CheckGlueTimeOut(string productPn, string StationCode, string PackCode, int mainId, PressureDataUploadDto dto)
        {
            using var trans = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var resp = new ServiceErrResponse();
                var product = await _dbContext.Products.FirstOrDefaultAsync(f => !f.IsDeleted && f.Code == productPn);
                if (product == null)
                {
                    return resp.ToError(ResponseErrorType.上位机错误, 500, $"产品{productPn}未找到").ToErrResult<IList<DcParamValue>, ServiceErrResponse>();
                }

                var station = await _dbContext.Base_Stations.FirstOrDefaultAsync(f => !f.IsDeleted && f.Code == StationCode);
                if (station == null)
                {
                    return resp.ToError(ResponseErrorType.上位机错误, 500, $"工位{StationCode}未找到").ToErrResult<IList<DcParamValue>, ServiceErrResponse>();
                }

                var stationTask = await _dbContext.Base_StationTasks.FirstOrDefaultAsync(f => !f.IsDeleted && f.ProductId == product.Id && f.StepId == station.StepId && f.Type == Common.Library.CommonEnum.StationTaskTypeEnum.超时检测);
                if (stationTask == null)
                {
                    return new List<DcParamValue>().ToOkResult<IList<DcParamValue>, ServiceErrResponse>();
                }

                var timeCheck = await _dbContext.Base_StationTaskCheckTimeOuts.FirstOrDefaultAsync(f => !f.IsDeleted && f.StationTaskId == stationTask.Id);
                if (timeCheck == null)
                {
                    return resp.ToError(ResponseErrorType.上位机错误, 500, $"超时检测任务未配置任务详情").ToErrResult<IList<DcParamValue>, ServiceErrResponse>();
                }

                var timeValue = await _dbContext.Proc_StationTask_TimeRecords.FirstOrDefaultAsync(f => !f.IsDeleted && f.SerialCode == PackCode && f.TimeFlag == timeCheck.TimeOutFlag);
                if (timeValue == null)
                {
                    return resp.ToError(ResponseErrorType.上位机错误, 500, $"PACK条码{PackCode}不存在时间标志为{timeCheck.TimeOutFlag}的时间记录").ToErrResult<IList<DcParamValue>, ServiceErrResponse>();
                }

                var glueStartTime = DateTime.Parse(timeValue.TimeValue);
                _ = DateTime.TryParse(dto.CompleteTime, out DateTime pressurizeCompleteTime);

                var duration = (pressurizeCompleteTime - glueStartTime).TotalMinutes;
                if (duration > Convert.ToDouble(timeCheck.MaxDuration) || duration < Convert.ToDouble(timeCheck.MinDuration))
                {
                    return resp.ToError(ResponseErrorType.数据异常, 500, $"时长不在管控范围内，管控范围{timeCheck.MinDuration}-{timeCheck.MaxDuration}").ToErrResult<IList<DcParamValue>, ServiceErrResponse>();
                }

                var record = await _dbContext.Proc_StationTask_Records.FirstOrDefaultAsync(f => f.Proc_StationTask_MainId == mainId && f.Base_StationTaskId == stationTask.Id && !f.IsDeleted);
                if (record == null)
                {
                    var newRecord = new Proc_StationTask_Record
                    {
                        Proc_StationTask_MainId = mainId,
                        Base_StationTaskId = stationTask.Id,
                        TaskName = stationTask.Name ?? "",
                        CreateTime = DateTime.Now,
                        Status = StationTaskStatusEnum.已完成,
                    };
                    await _dbContext.AddAsync(newRecord);
                    await _dbContext.SaveChangesAsync();
                    record = newRecord;
                }

                var timeResults = await _dbContext.Proc_StationTask_CheckTimeouts.Where(w => !w.IsDeleted && w.StationTask_RecordId == record.Id).ToListAsync();
                foreach (var timeResult in timeResults)
                {
                    timeResult.IsDeleted = true;
                }
                _dbContext.UpdateRange(timeResults);
                var newResult = new Proc_StationTask_CheckTimeout
                {
                    PackPN = PackCode,
                    StationTask_RecordId = record.Id,
                    StationTaskId = stationTask.Id,
                    Status = StationTaskStatusEnum.已完成,
                    TimeName = stationTask.Name,
                    CreateTime = DateTime.Now,
                    Pass = true,
                    UpMesCode = timeCheck.UpMesCode,
                    Time = Convert.ToDecimal(duration),
                    StartTime = glueStartTime,
                    CollectTime = DateTime.Now,
                };
                await _dbContext.AddAsync(newResult);
                await _dbContext.SaveChangesAsync();
                await trans.CommitAsync();
                var dataResp = new List<DcParamValue>()
                {
                   new DcParamValue
                   {
                        DataType= ValueTypeEnum.NUMBER,
                        ParamValue = newResult.Time.ToString(),
                        UpMesCode = newResult.UpMesCode?? ""
                   }
                };
                return dataResp.ToOkResult<IList<DcParamValue>, ServiceErrResponse>();
            }
            catch (Exception ex)
            {
                await trans.RollbackAsync();
                var resp = new ServiceErrResponse();
                return resp.ToError(ResponseErrorType.上位机错误, 500, ex.Message).ToErrResult<IList<DcParamValue>, ServiceErrResponse>();
            }
        }

        private async Task<FSharpResult<IList<DcParamValue>, ServiceErrResponse>> DealCompleteTime(Base_StationTask? stationTask, PressureDataUploadDto dto, int MainId)
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
                    SerialCode = dto.Pin,
                    StationCode = dto.StationCode,
                    UploadMesCode = timeRecordFormu.UpMesCode,
                    TimeValue = dto.CompleteTime,
                };
                await _dbContext.AddAsync(newTime);
                await _dbContext.SaveChangesAsync();
                await tras.CommitAsync();
                dcParam.Add(new DcParamValue
                {
                    DataType = ValueTypeEnum.TEXT,
                    ParamValue = dto.CompleteTime,
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

        private async Task<FSharpResult<IList<DcParamValue>, ServiceErrResponse>> DealPressureData(IList<Base_StationTask> stationTask, PressureDataUploadDto dto, int MainId)
        {
            using var tras = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var resp = new ServiceErrResponse();

                if (dto.PressureDatas == null)
                {
                    return resp.ToError(ResponseErrorType.数据异常, 500, $"没有提供压力数据，请检查后重试").ToErrResult<IList<DcParamValue>, ServiceErrResponse>();
                }

                if (stationTask.Count == 0)
                {   //没有涂胶开始时间配方则直接返回
                    return resp.ToError(ResponseErrorType.上位机错误, 500, $"没有配置自动加压任务，请检查后重试").ToErrResult<IList<DcParamValue>, ServiceErrResponse>();
                }
                var formulaType = stationTask.Any(a => a.Type != StationTaskTypeEnum.自动加压);
                if (formulaType)
                {
                    return resp.ToError(ResponseErrorType.上位机错误, 500, $"处理自动加压时存在配方类型不正确的项，请检查后重试").ToErrResult<IList<DcParamValue>, ServiceErrResponse>();
                }
                var PressureDatas = new List<PressureData>();
                foreach (var item in stationTask)
                {
                    var glueFormulas = await _dbContext.Base_AutoStationTaskPressures.Where(w => w.StationTaskId == item.Id && !w.IsDeleted).ToListAsync();
                    foreach (var formu in glueFormulas)
                    {
                        var pressureData = dto.PressureDatas[formu.PressureLocate - 1];

                        var pressureValue = new Entitys.DBEntity.PressureData
                        {
                            ParamName = formu.ParameterName,
                            Locate = formu.PressureLocate,
                            UpMesCode = formu.UpMesCode,
                        };

                        switch (formu.PressurizeDataType)
                        {
                            case PressurizeDataType.肩部高度:
                                if (!CheckAvilied(pressureData.ShoudlerHeight, formu.MinValue, formu.MaxValue))
                                {
                                    return resp.ToError(ResponseErrorType.数据异常, 500, $"数据-{formu.ParameterName}不在管控范围内").ToErrResult<IList<DcParamValue>, ServiceErrResponse>();
                                }
                                pressureValue.Value = pressureData.ShoudlerHeight;
                                break;
                            case PressurizeDataType.保压时长:
                                var dura = (float)pressureData.KeepDuration;
                                if (!CheckAvilied(dura, formu.MinValue, formu.MaxValue))
                                {
                                    return resp.ToError(ResponseErrorType.数据异常, 500, $"数据-{formu.ParameterName}不在管控范围内").ToErrResult<IList<DcParamValue>, ServiceErrResponse>();
                                }
                                pressureValue.Value = dura;
                                break;
                            case PressurizeDataType.平均压力:
                                if (!CheckAvilied(pressureData.PressureAverage, formu.MinValue, formu.MaxValue))
                                {
                                    return resp.ToError(ResponseErrorType.数据异常, 500, $"数据-{formu.ParameterName}不在管控范围内").ToErrResult<IList<DcParamValue>, ServiceErrResponse>();
                                }
                                pressureValue.Value = pressureData.PressureAverage;
                                break;
                            case PressurizeDataType.最大压力:
                                if (!CheckAvilied(pressureData.PressureMax, formu.MinValue, formu.MaxValue))
                                {
                                    return resp.ToError(ResponseErrorType.数据异常, 500, $"数据-{formu.ParameterName}不在管控范围内").ToErrResult<IList<DcParamValue>, ServiceErrResponse>();
                                }
                                pressureValue.Value = pressureData.PressureMax;
                                break;
                        }

                        PressureDatas.Add(pressureValue);
                    }
                }

                var pressureInfo = await _dbContext.Proc_PressureInfos.Where(w => w.Proc_StationTask_MainId == MainId && !w.IsDeleted).FirstOrDefaultAsync();
                if (pressureInfo == null)
                {
                    pressureInfo = new Proc_PressureInfo();
                    pressureInfo.PackPN = dto.Pin;
                    pressureInfo.StationCode = dto.StationCode;
                    pressureInfo.Proc_StationTask_MainId = MainId;
                    pressureInfo.PressureDataJson = JsonConvert.SerializeObject(PressureDatas);
                    await _dbContext.AddAsync(pressureInfo);
                }
                pressureInfo.PressureDataJson = JsonConvert.SerializeObject(PressureDatas);
                await _dbContext.SaveChangesAsync();
                await tras.CommitAsync();
                var dcParams = PressureDatas.Select(s => new DcParamValue { DataType = ValueTypeEnum.NUMBER, ParamValue = s.Value.ToString(), UpMesCode = s.UpMesCode }).ToList();

                return dcParams.ToOkResult<IList<DcParamValue>, ServiceErrResponse>();
            }
            catch (Exception ex)
            {
                await tras.RollbackAsync();
                var resp = new ServiceErrResponse();
                return resp.ToError(ResponseErrorType.上位机错误, 500, ex.Message).ToErrResult<IList<DcParamValue>, ServiceErrResponse>();
            }
        }

        public bool CheckAvilied(float data, decimal min, decimal max)
        {
            return data >= (float)min && data <= (float)max;
        }
        public async Task<Response<IList<Proc_PressureInfo>>> LoadPressureData(PressureInfosDto dto)
        {
            var resp = new Response<IList<Proc_PressureInfo>>();
            try
            {
                var datas = await _dbContext.Proc_PressureInfos.Where(w => !w.IsDeleted).OrderByDescending(o => o.Id).ToListAsync();
                if (!string.IsNullOrEmpty(dto.PackCode))
                {
                    datas = datas.Where(w => w.PackPN == dto.PackCode).ToList();
                }
                if (!string.IsNullOrEmpty(dto.StationCode))
                {
                    datas = datas.Where(w => w.StationCode == dto.StationCode).ToList();
                }
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

        public async Task<Response<IList<PressureData>>> LoadPressureDataDetail(int dataId)
        {
            var resp = new Response<IList<PressureData>>();
            try
            {
                var data = await _dbContext.Proc_PressureInfos.FirstOrDefaultAsync(w => w.Id == dataId);
                if (data == null)
                {
                    resp.Code = 500;
                    resp.Message = $"记录未找到";
                    return resp;
                }
                var json = JsonConvert.DeserializeObject<IList<PressureData>>(data.PressureDataJson);
                resp.Result = json;
            }
            catch (Exception ex)
            {
                resp.Code = 500;
                resp.Message = ex.Message;

            }
            return resp;
        }

        public async Task<Response<CatlMESReponse>> UploadPressureDataAgain(string packCode, string stationCode, bool isNeedChangeStatus)
        {
            var resp = new Response<CatlMESReponse>();
            try
            {
                var main = await _dbContext.Proc_StationTask_Mains.FirstOrDefaultAsync(f => f.PackCode == packCode && f.StationCode == stationCode && !f.IsDeleted);
                if (main == null)
                {
                    resp.Code = 500;
                    resp.Message = $"主记录记录未找到";
                    return resp;
                }
                var data = await _dbContext.Proc_PressureInfos.FirstOrDefaultAsync(w => w.Proc_StationTask_MainId == main.Id && !w.IsDeleted);
                if (data == null)
                {
                    resp.Code = 500;
                    resp.Message = $"记录未找到";
                    return resp;
                }
                var json = JsonConvert.DeserializeObject<IList<Yee.Entitys.DBEntity.PressureData>>(data.PressureDataJson);

                var dcparams = json.Select(s => new DcParamValue { DataType = ValueTypeEnum.NUMBER, ParamValue = s.Value.ToString(), UpMesCode = s.UpMesCode }).ToList();
                var records = await _dbContext.Proc_StationTask_Records.Where(w => w.Proc_StationTask_MainId == main.Id).ToListAsync();
                foreach (var item in records)
                {
                    var time = await _dbContext.Proc_StationTask_TimeRecords.Where(w => w.Proc_StationTask_RecordId == item.Id && !w.IsDeleted).ToListAsync();
                    dcparams = dcparams.Concat(time.Select(s => new DcParamValue { DataType = ValueTypeEnum.TEXT, ParamValue = s.TimeValue.ToString(), UpMesCode = s.UploadMesCode })).ToList();
                    break;
                }

                foreach (var item in records)
                {
                    var time = await _dbContext.Proc_StationTask_CheckTimeouts.Where(w => w.StationTask_RecordId == item.Id && !w.IsDeleted).ToListAsync();
                    dcparams = dcparams.Concat(time.Select(s => new DcParamValue { DataType = ValueTypeEnum.TEXT, ParamValue = s.Time.ToString(), UpMesCode = s.UpMesCode })).ToList();
                    break;
                }
                resp.Result = await _mesInvoker.dataCollect(main.PackCode, dcparams, isNeedChangeStatus, stationCode);
            }
            catch (Exception ex)
            {
                resp.Code = 500;
                resp.Message = ex.Message;

            }
            return resp;
        }
    }
}
