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
using Yee.Entitys.DTOS.AutomaticStationDTOS;
using Yee.Entitys.Production;
using Yee.Services.CatlMesInvoker;

namespace Yee.Services.AutomaticStation
{
    public class AutoGlueService
    {
        private readonly AsZeroDbContext _dbContext;
        private readonly AutomicCommonService _commonService;
        private readonly ICatlMesInvoker _mesInvoker;

        public AutoGlueService(AsZeroDbContext dbContext, AutomicCommonService commonService, ICatlMesInvoker mesInvoker)
        {
            _dbContext = dbContext;
            _commonService = commonService;
            _mesInvoker = mesInvoker;
        }

        public async Task<FSharpResult<ServiceErrResponse, ServiceErrResponse>> SaveGlueDataAndUploadCATL(GlueDataDto dto)
        {
            try
            {
                var q = from main in _commonService.HasMainRecord(dto.PackCode, dto.StationCode)
                        from productPn in _commonService.GetProductPnFromCatlMes(dto.PackCode, dto.StationCode)
                        from formulas in _commonService.GetBaseStationTasks(productPn.BarCode_GoodsPN, dto.StationCode)
                        from glueStartTimeDC in DealStartTime(formulas.FirstOrDefault(f => f.Type == StationTaskTypeEnum.时间记录), dto, main.Id)
                        from glueDataDC in DealGlueData(formulas.Where(f => f.Type == StationTaskTypeEnum.自动涂胶).ToList(), dto, main.Id)
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

        private async Task<FSharpResult<IList<DcParamValue>, ServiceErrResponse>> DealStartTime(Base_StationTask? stationTask, GlueDataDto dto, int MainId)
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
                    TimeValue = dto.GlueTime,
                };
                await _dbContext.AddAsync(newTime);
                await _dbContext.SaveChangesAsync();

                await tras.CommitAsync();
                dcParam.Add(new DcParamValue
                {
                    DataType = ValueTypeEnum.TEXT,
                    ParamValue = dto.GlueTime,
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

        private async Task<FSharpResult<IList<DcParamValue>, ServiceErrResponse>> DealGlueData(IList<Base_StationTask> stationTask, GlueDataDto dto, int MainId)
        {
            using var tras = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var resp = new ServiceErrResponse();

                if (dto.GlueDatas == null)
                {
                    return resp.ToError(ResponseErrorType.数据异常, 500, $"没有提供涂胶数据，请检查后重试").ToErrResult<IList<DcParamValue>, ServiceErrResponse>();
                }

                if (stationTask.Count == 0)
                {   //没有涂胶开始时间配方则直接返回
                    return resp.ToError(ResponseErrorType.上位机错误, 500, $"没有配置自动涂胶任务，请检查后重试").ToErrResult<IList<DcParamValue>, ServiceErrResponse>();
                }

                var formulaType = stationTask.Any(a => a.Type != StationTaskTypeEnum.自动涂胶);
                if (formulaType)
                {
                    return resp.ToError(ResponseErrorType.上位机错误, 500, $"处理自动涂胶时存在配方类型不正确的项，请检查后重试").ToErrResult<IList<DcParamValue>, ServiceErrResponse>();
                }
                var GlueDatas = new List<Entitys.DBEntity.GlueDataSql>();
                foreach (var item in stationTask)
                {
                    var glueFormulas = await _dbContext.Base_AutoStationTaskGlues.Where(w => w.StationTaskId == item.Id && !w.IsDeleted).ToListAsync();
                    foreach (var formu in glueFormulas)
                    {
                        var glueData = dto.GlueDatas[formu.GlueLocate - 1];
                        float data = 0;
                        switch (formu.GlueType)
                        {
                            case GlueType.A胶:
                                if (!CheckAvilied(glueData.GlueA, formu.MinValue, formu.MaxValue))
                                {
                                    return resp.ToError(ResponseErrorType.数据异常, 500, $"数据-{formu.ParameterName}不在管控范围内").ToErrResult<IList<DcParamValue>, ServiceErrResponse>();
                                }
                                data = glueData.GlueA;
                                break;
                            case GlueType.B胶:
                                if (!CheckAvilied(glueData.GlueB, formu.MinValue, formu.MaxValue))
                                {
                                    return resp.ToError(ResponseErrorType.数据异常, 500, $"数据-{formu.ParameterName}不在管控范围内").ToErrResult<IList<DcParamValue>, ServiceErrResponse>();
                                }
                                data = glueData.GlueB;
                                break;
                            case GlueType.胶比例:
                                if (!CheckAvilied(glueData.GlueProportion, formu.MinValue, formu.MaxValue))
                                {
                                    return resp.ToError(ResponseErrorType.数据异常, 500, $"数据-{formu.ParameterName}不在管控范围内").ToErrResult<IList<DcParamValue>, ServiceErrResponse>();
                                }
                                data = glueData.GlueProportion;
                                break;
                            case GlueType.总胶重:
                                if (!CheckAvilied(glueData.GlueTotal, formu.MinValue, formu.MaxValue))
                                {
                                    return resp.ToError(ResponseErrorType.数据异常, 500, $"数据-{formu.ParameterName}不在管控范围内").ToErrResult<IList<DcParamValue>, ServiceErrResponse>();
                                }
                                data = glueData.GlueTotal;
                                break;
                        }

                        var glueValue = new GlueDataSql
                        {
                            ParamName = formu.ParameterName,
                            GlueLocate = formu.GlueLocate,
                            GlueType = formu.GlueType,
                            UploadMesCode = formu.UpMesCode,
                            Value = data
                        };
                        GlueDatas.Add(glueValue);
                    }
                }

                var glueInfo = await _dbContext.Proc_GluingInfos.Where(w => w.Proc_StationTask_MainId == MainId && !w.IsDeleted).FirstOrDefaultAsync();
                if (glueInfo == null)
                {
                    glueInfo = new Proc_GluingInfo();
                    glueInfo.PackPN = dto.PackCode;
                    glueInfo.StationCode = dto.StationCode;
                    glueInfo.Proc_StationTask_MainId = MainId;
                    glueInfo.CreateTime = DateTime.Now;
                    glueInfo.UpdateTime = DateTime.Now;
                    await _dbContext.AddAsync(glueInfo);
                }
                glueInfo.GlueDataJson = JsonConvert.SerializeObject(GlueDatas);
                await _dbContext.SaveChangesAsync();
                await tras.CommitAsync();
                var dcParams = GlueDatas.Select(s => new DcParamValue { DataType = ValueTypeEnum.NUMBER, ParamValue = s.Value.ToString(), UpMesCode = s.UploadMesCode }).ToList();

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
            return data > (float)min && data < (float)max;
        }

        public async Task<Response<IList<Proc_GluingInfo>>> LoadGlueData(GlueDetailDto dto)
        {
            
            var resp = new Response<IList<Proc_GluingInfo>>();
            try
            {
                var result = await _dbContext.Proc_GluingInfos.Where(w => !w.IsDeleted).ToListAsync();
                if (!string.IsNullOrEmpty(dto.PackPN))
                {
                    result = result.Where(res => res.PackPN == dto.PackPN).ToList();
                }
                if (dto.BeginTime != null)
                {
                    result.Where(res => res.CreateTime > dto.BeginTime);
                }
                if (dto.EndTime != null)
                {
                    result.Where(res => res.CreateTime < dto.EndTime);
                }
                resp.Count = result.Count();
                resp.Result = result.Skip(dto.Limit * (dto.Page - 1)).Take(dto.Limit).ToList();
            }
            catch (Exception ex)
            {
                resp.Code = 500;
                resp.Message = ex.Message;

            }
            return resp;
        }

        public async Task<Response<IList<GlueDataSql>>> LoadGlueDataDetail(int dataId)
        {
            var resp = new Response<IList<Yee.Entitys.DBEntity.GlueDataSql>>();
            try
            {
                var data = await _dbContext.Proc_GluingInfos.FirstOrDefaultAsync(w => w.Id == dataId);
                if (data == null)
                {
                    resp.Code = 500;
                    resp.Message = $"记录未找到";
                    return resp;
                }
                var json = JsonConvert.DeserializeObject<IList<Yee.Entitys.DBEntity.GlueDataSql>>(data.GlueDataJson);
                resp.Result = json;
            }
            catch (Exception ex)
            {
                resp.Code = 500;
                resp.Message = ex.Message;

            }
            return resp;
        }

        public async Task<Response<CatlMESReponse>> UploadGlueDataAgain(GlueDataDto dto)
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
                var data = await _dbContext.Proc_GluingInfos.FirstOrDefaultAsync(w => w.Proc_StationTask_MainId == main.Id && !w.IsDeleted);
                if (data == null)
                {
                    resp.Code = 500;
                    resp.Message = $"记录未找到";
                    return resp;
                }
                var json = JsonConvert.DeserializeObject<IList<Yee.Entitys.DBEntity.GlueDataSql>>(data.GlueDataJson);
                var dcparams = json.Select(s => new DcParamValue { DataType = ValueTypeEnum.NUMBER, ParamValue = s.Value.ToString(), UpMesCode = s.UploadMesCode }).ToList();
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

        public async Task<FSharpResult<uint, ServiceErrResponse>> GetGlueRemainDuration(string packCode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(packCode))
                {
                    return new ServiceErrResponse { ErrorCode = 400, ErrorMessage = "Pack码站位不能为空", ErrorType = ResponseErrorType.上位机错误 }.ToErrResult<uint, ServiceErrResponse>();
                }

                var stationTask_TimeRecord = await _dbContext.Proc_StationTask_TimeRecords.OrderByDescending(o => o.Id)
                      .Where(s => !s.IsDeleted && s.SerialCode == packCode && s.TimeFlag == "ShoulderGlueTime")
                      .FirstOrDefaultAsync();
                if (stationTask_TimeRecord == null)
                {
                    return new ServiceErrResponse { ErrorCode = 404, ErrorMessage = "找不到对应的涂胶时间", ErrorType = ResponseErrorType.上位机错误 }.ToErrResult<uint, ServiceErrResponse>();
                }

                if (!DateTime.TryParse(stationTask_TimeRecord.TimeValue, out DateTime glueStartTime))
                {
                    return new ServiceErrResponse { ErrorCode = 400, ErrorMessage = "肩部涂胶时间异常", ErrorType = ResponseErrorType.上位机错误 }.ToErrResult<uint, ServiceErrResponse>();
                }

                var remainDuration = (uint)(DateTime.Now - glueStartTime).TotalMilliseconds;

                return remainDuration.ToOkResult<uint, ServiceErrResponse>();

            }
            catch (Exception ex)
            {
                return new ServiceErrResponse { ErrorCode = 500, ErrorMessage = ex.Message, ErrorType = ResponseErrorType.上位机错误 }.ToErrResult<uint, ServiceErrResponse>();
            }
        }
    }
}
