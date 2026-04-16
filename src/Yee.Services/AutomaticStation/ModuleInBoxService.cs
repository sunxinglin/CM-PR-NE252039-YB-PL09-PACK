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
using Yee.Entitys.DTOS.AutomaticStationDTOS;
using Yee.Entitys.Production;
using Yee.Services.CatlMesInvoker;

namespace Yee.Services.AutomaticStation
{
    public class ModuleInBoxService
    {
        private readonly AsZeroDbContext _dbContext;
        private readonly ICatlMesInvoker _mesInvoker;
        private readonly AutomicCommonService _commonService;

        public ModuleInBoxService(AsZeroDbContext dbContext, ICatlMesInvoker mesInvoker, AutomicCommonService commonService)
        {
            _dbContext = dbContext;
            _mesInvoker = mesInvoker;
            _commonService = commonService;
        }

        public async Task<FSharpResult<string, ServiceErrResponse>> GetPackCodeForCheck()
        {
            //从字典表获取配置的固定PackCode
            var dictionary = await _dbContext.Dictionaries.AsNoTracking()
                .Where(e => e.IsDeleted == false && e.Code == "ModuleInBoxPackCode")
                .FirstOrDefaultAsync();
            if (dictionary == null)
            {
                return new ServiceErrResponse().ToError(ResponseErrorType.上位机错误, 404, "未找到字典表[ModuleInBoxPackCode]配置信息").ToErrResult<string, ServiceErrResponse>();
            }
            var dicDetail = await _dbContext.DictionaryDetails.AsNoTracking()
                .Where(e => e.IsDeleted == false && e.DictionaryId == dictionary.Id)
                .OrderBy(e => e.Id)
                .FirstOrDefaultAsync();
            if (dicDetail == null)
            {
                return new ServiceErrResponse().ToError(ResponseErrorType.上位机错误, 404, "未找到字典明细表配置信息").ToErrResult<string, ServiceErrResponse>();
            }
            if (string.IsNullOrWhiteSpace(dicDetail.Code))
            {
                return new ServiceErrResponse().ToError(ResponseErrorType.上位机错误, 404, "字典明细表配置Code为空").ToErrResult<string, ServiceErrResponse>();
            }
            var packCode = dicDetail.Code;

            return packCode.ToOkResult<string, ServiceErrResponse>();
        }

        public async Task<FSharpResult<uint, ServiceErrResponse>> GetGlueDuration(string productPn, string StationCode, string PackCode)
        {
            using var trans = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var resp = new ServiceErrResponse();
                var product = await _dbContext.Products.FirstOrDefaultAsync(f => !f.IsDeleted && f.Code == productPn);
                if (product == null)
                {
                    return resp.ToError(ResponseErrorType.上位机错误, 500, $"产品{productPn}未找到").ToErrResult<uint, ServiceErrResponse>();
                }

                var station = await _dbContext.Base_Stations.FirstOrDefaultAsync(f => !f.IsDeleted && f.Code == StationCode);
                if (station == null)
                {
                    return resp.ToError(ResponseErrorType.上位机错误, 500, $"工位{StationCode}未找到").ToErrResult<uint, ServiceErrResponse>();
                }

                var stationTask = await _dbContext.Base_StationTasks.FirstOrDefaultAsync(f => !f.IsDeleted && f.ProductId == product.Id && f.StepId == station.StepId && f.Type == StationTaskTypeEnum.超时检测);
                if (stationTask == null)
                {
                    return ((uint)99999999).ToOkResult<uint, ServiceErrResponse>();
                }

                var timeCheck = await _dbContext.Base_StationTaskCheckTimeOuts.FirstOrDefaultAsync(f => !f.IsDeleted && f.StationTaskId == stationTask.Id);
                if (timeCheck == null)
                {
                    return resp.ToError(ResponseErrorType.上位机错误, 500, $"超时检测任务未配置任务详情").ToErrResult<uint, ServiceErrResponse>();
                }

                var timeValue = await _dbContext.Proc_StationTask_TimeRecords.FirstOrDefaultAsync(f => !f.IsDeleted && f.SerialCode == PackCode && f.TimeFlag == timeCheck.TimeOutFlag);
                if (timeValue == null)
                {
                    return resp.ToError(ResponseErrorType.上位机错误, 500, $"PACK条码{PackCode}不存在时间标志为{timeCheck.TimeOutFlag}的时间记录").ToErrResult<uint, ServiceErrResponse>();
                }

                var time = DateTime.Parse(timeValue.TimeValue);
                var nowTime = DateTime.Now;

                var duration = (nowTime - time).TotalMinutes;
                if (duration > Convert.ToDouble(timeCheck.MaxDuration) || duration < Convert.ToDouble(timeCheck.MinDuration))
                {
                    return resp.ToError(ResponseErrorType.数据异常, 500, $"时长不在管控范围内，管控范围{timeCheck.MinDuration}-{timeCheck.MaxDuration}").ToErrResult<uint, ServiceErrResponse>();
                }

                var left = (uint)((Convert.ToDouble(timeCheck.MaxDuration) - duration) * 60 * 1000);


                return left.ToOkResult<uint, ServiceErrResponse>();
            }
            catch (Exception ex)
            {
                await trans.RollbackAsync();
                var resp = new ServiceErrResponse();
                return resp.ToError(ResponseErrorType.上位机错误, 500, ex.Message).ToErrResult<uint, ServiceErrResponse>();
            }
        }

        public async Task<FSharpResult<(IList<DcParamValue>, DateTime), ServiceErrResponse>> CheckGlueTimeOut(string productPn, string StationCode, string PackCode, int mainId)
        {
            using var trans = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var resp = new ServiceErrResponse();
                var product = await _dbContext.Products.FirstOrDefaultAsync(f => !f.IsDeleted && f.Code == productPn);
                if (product == null)
                {
                    return resp.ToError(ResponseErrorType.上位机错误, 500, $"产品{productPn}未找到").ToErrResult<(IList<DcParamValue>, DateTime), ServiceErrResponse>();
                }

                var station = await _dbContext.Base_Stations.FirstOrDefaultAsync(f => !f.IsDeleted && f.Code == StationCode);
                if (station == null)
                {
                    return resp.ToError(ResponseErrorType.上位机错误, 500, $"工位{StationCode}未找到").ToErrResult<(IList<DcParamValue>, DateTime), ServiceErrResponse>();
                }

                var stationTask = await _dbContext.Base_StationTasks.FirstOrDefaultAsync(f => !f.IsDeleted && f.ProductId == product.Id && f.StepId == station.StepId && f.Type == StationTaskTypeEnum.超时检测);
                if (stationTask == null)
                {
                    return (new List<DcParamValue>(), DateTime.Now).ToOkResult<(IList<DcParamValue>, DateTime), ServiceErrResponse>();
                }

                var timeCheck = await _dbContext.Base_StationTaskCheckTimeOuts.FirstOrDefaultAsync(f => !f.IsDeleted && f.StationTaskId == stationTask.Id);
                if (timeCheck == null)
                {
                    return resp.ToError(ResponseErrorType.上位机错误, 500, $"超时检测任务未配置任务详情").ToErrResult<(IList<DcParamValue>, DateTime), ServiceErrResponse>();
                }

                var timeValue = await _dbContext.Proc_StationTask_TimeRecords.FirstOrDefaultAsync(f => !f.IsDeleted && f.SerialCode == PackCode && f.TimeFlag == timeCheck.TimeOutFlag);
                if (timeValue == null)
                {
                    return resp.ToError(ResponseErrorType.上位机错误, 500, $"PACK条码{PackCode}不存在时间标志为{timeCheck.TimeOutFlag}的时间记录").ToErrResult<(IList<DcParamValue>, DateTime), ServiceErrResponse>();
                }

                var time = DateTime.Parse(timeValue.TimeValue);
                var nowTime = DateTime.Now;

                var duration = (nowTime - time).TotalMinutes;
                if (duration > Convert.ToDouble(timeCheck.MaxDuration) || duration < Convert.ToDouble(timeCheck.MinDuration))
                {
                    return resp.ToError(ResponseErrorType.数据异常, 500, $"时长不在管控范围内，管控范围{timeCheck.MinDuration}-{timeCheck.MaxDuration}").ToErrResult<(IList<DcParamValue>, DateTime), ServiceErrResponse>();
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
                    StartTime = time,
                    CollectTime = nowTime,
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
                return (dataResp, time).ToOkResult<(IList<DcParamValue>, DateTime), ServiceErrResponse>();
            }
            catch (Exception ex)
            {
                await trans.RollbackAsync();
                var resp = new ServiceErrResponse();
                return resp.ToError(ResponseErrorType.上位机错误, 500, ex.Message).ToErrResult<(IList<DcParamValue>, DateTime), ServiceErrResponse>();
            }
        }

        public async Task<FSharpResult<CatlMESReponse, ServiceErrResponse>> GetModuleCodeFromCATL(string cellCode, string stationCode)
        {
            try
            {
                var result = await _mesInvoker.GetModuleCodeByCellCode(cellCode, stationCode);
                return result.ToOkResult<CatlMESReponse, ServiceErrResponse>();
            }
            catch (Exception ex)
            {
                var resp = new ServiceErrResponse();
                return resp.ToError(ResponseErrorType.上位机错误, 500, ex.Message).ToErrResult<CatlMESReponse, ServiceErrResponse>();
            }
        }

        public async Task<FSharpResult<CatlMESReponse, ServiceErrResponse>> CheckModuleFromCATL(string moduleCode, string stationCode)
        {
            try
            {
                //从字典表获取配置的固定PackCode
                var packCodeResult = await GetPackCodeForCheck();
                if (packCodeResult.IsError)
                {
                    return packCodeResult.ErrorValue.ToErrResult<CatlMESReponse, ServiceErrResponse>();
                }
                var packCode = packCodeResult.ResultValue;

                //校验模组
                var checkResult = await _mesInvoker.CheckInventoryAttributes(packCode, moduleCode, stationCode);
                return checkResult.ToOkResult<CatlMESReponse, ServiceErrResponse>();
            }
            catch (Exception ex)
            {
                var resp = new ServiceErrResponse();
                return resp.ToError(ResponseErrorType.上位机错误, 500, ex.Message).ToErrResult<CatlMESReponse, ServiceErrResponse>();
            }
        }

        public async Task<FSharpResult<ServiceErrResponse, ServiceErrResponse>> CheckModuleInfo(string moduleCode, string modulePN, int moduleLocation, string stationCode)
        {
            try
            {
                var resp = new ServiceErrResponse();

                var q = from packCode in GetPackCodeForCheck() //从字典表获取配置的固定PackCode
                        from checkModule in CheckModule(packCode, moduleCode, stationCode) //使用CATL MES接口校验模组
                        from productPn in _commonService.GetProductPnFromCatlMes(packCode, stationCode)
                        from formulas in _commonService.GetBaseStationTasks(productPn.BarCode_GoodsPN, stationCode)
                        from checkPNResult in CheckModulePN(formulas.Where(f => f.Type == StationTaskTypeEnum.模组入箱).ToList(), moduleCode, modulePN, moduleLocation)
                        select checkPNResult;

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

        public async Task<FSharpResult<ServiceErrResponse, ServiceErrResponse>> RecordModuleInfo(string modulePN, string moduleCode, string CellCode, string StationCode)
        {
            try
            {
                var resp = new ServiceErrResponse();
                var blockExiest = await _dbContext.Proc_ModuleInBox_GrapRecords.FirstOrDefaultAsync(f => !f.IsDeleted && f.ModuleCode == moduleCode);

                if (blockExiest != null)
                {
                    if(blockExiest.HasUsed)
                    {
                        return resp.ToError(ResponseErrorType.数据异常, 500, "该模组已被装配,请勿重复拍照").ToErrResult<ServiceErrResponse, ServiceErrResponse>();

                    }

                    blockExiest.ModulePN = modulePN;
                    blockExiest.CellCode = CellCode;
                    blockExiest.GrabTime = DateTime.Now;
                    blockExiest.StationCode = StationCode;
                }
                else
                {
                    await _dbContext.Proc_ModuleInBox_GrapRecords.AddAsync(new Proc_ModuleInBox_GrapRecord
                    {
                        ModuleCode = moduleCode,
                        ModulePN = modulePN,
                        CellCode = CellCode,
                        GrabTime = DateTime.Now,
                        StationCode = StationCode,
                    });
                }


                await _dbContext.SaveChangesAsync();
                return resp.ToSuccess().ToOkResult<ServiceErrResponse, ServiceErrResponse>();

            }
            catch (Exception ex)
            {
                var resp = new ServiceErrResponse();
                return resp.ToError(ResponseErrorType.上位机错误, 500, ex.Message).ToErrResult<ServiceErrResponse, ServiceErrResponse>();
            }
        }

        public async Task<FSharpResult<ServiceErrResponse, ServiceErrResponse>> SaveAndUploadData(ModuleInBoxDataUploadDto dto)
        {
            try
            {
                var resp = new ServiceErrResponse();
                var q = from main in _commonService.HasMainRecord(dto.PackCode, dto.StationCode)
                        from productPn in _commonService.GetProductPnFromCatlMes(dto.PackCode, dto.StationCode)
                        from formulas in _commonService.GetBaseStationTasks(productPn.BarCode_GoodsPN, dto.StationCode)
                        from timeOutValue in this.CheckGlueTimeOut(productPn.BarCode_GoodsPN, dto.StationCode, dto.PackCode, main.Id)
                        from dataDC in this.DealData(formulas.Where(f => f.Type == StationTaskTypeEnum.模组入箱).ToList(), dto, main.Id, timeOutValue.Item2)
                        from upMes in _commonService.UploadCatlMes(dto.PackCode, dataDC.Concat(timeOutValue.Item1).ToList(), true, dto.StationCode)
                        from setMainComp in _commonService.SetStationComplete(main.Id)//设置main完工
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

        public async Task<FSharpResult<ServiceErrResponse, ServiceErrResponse>> SaveAndUploadSingleModule(ModuleInBoxSingleModuleUploadDto dto)
        {
            try
            {
                var resp = new ServiceErrResponse();
                var q = from main in _commonService.HasMainRecord(dto.PackCode, dto.StationCode)
                        from productPn in _commonService.GetProductPnFromCatlMes(dto.PackCode, dto.StationCode)
                        from formulas in _commonService.GetBaseStationTasks(productPn.BarCode_GoodsPN, dto.StationCode)
                        from assembleAndUpload in DealAssembleDataAndUpload(formulas.Where(f => f.Type == StationTaskTypeEnum.模组入箱).ToList(), dto)//组装
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

        private async Task<FSharpResult<IList<DcParamValue>, ServiceErrResponse>> DealData(IList<Base_StationTask> stationTask, ModuleInBoxDataUploadDto dto, int MainId, DateTime glueStartTime)
        {
            using var tras = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var resp = new ServiceErrResponse();

                if (dto.ModouleDatas == null)
                {
                    return resp.ToError(ResponseErrorType.数据异常, 500, $"没有提供模组入箱数据，请检查后重试").ToErrResult<IList<DcParamValue>, ServiceErrResponse>();
                }

                if (stationTask.Count == 0)
                {
                    return resp.ToError(ResponseErrorType.上位机错误, 500, $"没有配置模组入箱任务，请检查后重试").ToErrResult<IList<DcParamValue>, ServiceErrResponse>();
                }
                var inBoxDatas = new List<ModuleInBoxDataJsonModel>();
                foreach (var item in stationTask)
                {
                    var incaseFormulas = await _dbContext.Base_StationTask_AutoModuleInBoxs.Where(w => w.StationTaskId == item.Id && !w.IsDeleted).ToListAsync();
                    foreach (var formu in incaseFormulas)
                    {
                        var blockData = dto.ModouleDatas[formu.Location - 1];
                        var incaseData = new ModuleInBoxDataJsonModel()
                        {
                            ParameterName = formu.ParameterName,
                            ModuleInBoxDataType = formu.ModuleInBoxDataType,
                            Location = formu.Location,
                            UpMesCode = formu.UpMesCode,

                        };
                        switch (formu.ModuleInBoxDataType)
                        {
                            case ModuleInBoxDataTypeEnum.模组码:
                                incaseData.ModuleCode = blockData.ModuleCode;

                                if (!string.IsNullOrWhiteSpace(formu.ModulePN))
                                {
                                    var moduleInfo = await _dbContext.Proc_ModuleInBox_GrapRecords.AsNoTracking()
                                        .Where(e => e.ModuleCode == blockData.ModuleCode)
                                        .FirstOrDefaultAsync();
                                    if (moduleInfo == null)
                                    {
                                        return resp.ToError(ResponseErrorType.数据异常, 1400, $"模组{blockData.ModuleCode}未找到抓取记录").ToErrResult<IList<DcParamValue>, ServiceErrResponse>();
                                    }
                                    if (moduleInfo.ModuleLocation != formu.Location)
                                    {
                                        return resp.ToError(ResponseErrorType.数据异常, 1400, $"模组{blockData.ModuleCode}入箱位置[{formu.Location}]与单个入记录的位置[{moduleInfo.ModuleLocation}]不一致，请检查后重试").ToErrResult<IList<DcParamValue>, ServiceErrResponse>();
                                    }
                                    if (moduleInfo.ModulePN != formu.ModulePN)
                                    {
                                        return resp.ToError(ResponseErrorType.数据异常, 1400, $"模组{blockData.ModuleCode}的PN{moduleInfo.ModulePN}与配置的PN{formu.ModulePN}不一致").ToErrResult<IList<DcParamValue>, ServiceErrResponse>();
                                    }
                                }

                                incaseData.ModulePN = formu.ModulePN;
                                break;
                            case ModuleInBoxDataTypeEnum.模组长度:
                                if (!CheckAvilied(blockData.ModuleLenth, formu.MinValue, formu.MaxValue))
                                {
                                    return resp.ToError(ResponseErrorType.数据异常, 1400, $"数据-{formu.ParameterName}不在管控范围[{formu.MinValue},{formu.MaxValue}]内").ToErrResult<IList<DcParamValue>, ServiceErrResponse>();
                                }
                                incaseData.DataValue = blockData.ModuleLenth.ToString();
                                break;
                            case ModuleInBoxDataTypeEnum.保压时间:
                                var dura = (float)blockData.KeepDuation;
                                if (!CheckAvilied(dura, formu.MinValue, formu.MaxValue))
                                {
                                    return resp.ToError(ResponseErrorType.数据异常, 1400, $"数据-{formu.ParameterName}不在管控范围[{formu.MinValue},{formu.MaxValue}]内").ToErrResult<IList<DcParamValue>, ServiceErrResponse>();
                                }
                                incaseData.DataValue = blockData.KeepDuation.ToString();
                                break;
                            case ModuleInBoxDataTypeEnum.下压距离:
                                if (!CheckAvilied(blockData.DownDistance, formu.MinValue, formu.MaxValue))
                                {
                                    return resp.ToError(ResponseErrorType.数据异常, 1400, $"数据-{formu.ParameterName}不在管控范围[{formu.MinValue},{formu.MaxValue}]内").ToErrResult<IList<DcParamValue>, ServiceErrResponse>();
                                }
                                incaseData.DataValue = blockData.DownDistance.ToString();
                                break;
                            case ModuleInBoxDataTypeEnum.下压压力:
                                if (!CheckAvilied(blockData.DownPressure, formu.MinValue, formu.MaxValue))
                                {
                                    return resp.ToError(ResponseErrorType.数据异常, 1400, $"数据-{formu.ParameterName}不在管控范围[{formu.MinValue},{formu.MaxValue}]内").ToErrResult<IList<DcParamValue>, ServiceErrResponse>();
                                }
                                incaseData.DataValue = blockData.DownPressure.ToString();
                                break;
                            case ModuleInBoxDataTypeEnum.左侧压力:
                                if (!CheckAvilied(blockData.LeftPressure, formu.MinValue, formu.MaxValue))
                                {
                                    return resp.ToError(ResponseErrorType.数据异常, 1400, $"数据-{formu.ParameterName}不在管控范围[{formu.MinValue},{formu.MaxValue}]内").ToErrResult<IList<DcParamValue>, ServiceErrResponse>();
                                }
                                incaseData.DataValue = blockData.LeftPressure.ToString();
                                break;
                            case ModuleInBoxDataTypeEnum.右侧压力:
                                if (!CheckAvilied(blockData.RightPressure, formu.MinValue, formu.MaxValue))
                                {
                                    return resp.ToError(ResponseErrorType.数据异常, 1400, $"数据-{formu.ParameterName}不在管控范围[{formu.MinValue},{formu.MaxValue}]内").ToErrResult<IList<DcParamValue>, ServiceErrResponse>();
                                }
                                incaseData.DataValue = blockData.RightPressure.ToString();
                                break;
                            case ModuleInBoxDataTypeEnum.入箱完成时间:
                                var completeTime = blockData.CompleteTime.GetValueOrDefault();
                                incaseData.DataValue = completeTime.ToString("yyyy-MM-dd HH:mm:ss");
                                break;
                            case ModuleInBoxDataTypeEnum.模组入箱时长:
                                var duration = (float)((blockData.CompleteTime - glueStartTime)?.TotalMinutes ?? 0d);
                                if (!CheckAvilied(duration, formu.MinValue, formu.MaxValue))
                                {
                                    return resp.ToError(ResponseErrorType.数据异常, 1400, $"数据-{formu.ParameterName}不在管控范围[{formu.MinValue},{formu.MaxValue}]内").ToErrResult<IList<DcParamValue>, ServiceErrResponse>();
                                }
                                incaseData.DataValue = duration.ToString();
                                break;
                        }
                        inBoxDatas.Add(incaseData);
                    }
                }

                var moduleInBoxInfo = await _dbContext.Proc_ModuleInBox_DataCollects.Where(w => w.Proc_StationTask_MainId == MainId && !w.IsDeleted).FirstOrDefaultAsync();
                if (moduleInBoxInfo == null)
                {
                    moduleInBoxInfo = new Proc_ModuleInBox_DataCollect
                    {
                        PackCode = dto.PackCode,
                        StationCode = dto.StationCode,
                        Stauts = StationTaskStatusEnum.已完成,
                        ModuleInBoxDataJson = JsonConvert.SerializeObject(inBoxDatas),
                        Proc_StationTask_MainId = MainId
                    };
                    await _dbContext.AddAsync(moduleInBoxInfo);
                }
                moduleInBoxInfo.ModuleInBoxDataJson = JsonConvert.SerializeObject(inBoxDatas);
                await _dbContext.SaveChangesAsync();
                await tras.CommitAsync();

                //组织收数数据
                var dcParams = inBoxDatas.Where(w => w.ModuleInBoxDataType != ModuleInBoxDataTypeEnum.模组码 && w.ModuleInBoxDataType != ModuleInBoxDataTypeEnum.入箱完成时间).Select(s => new DcParamValue { DataType = ValueTypeEnum.NUMBER, ParamValue = s.DataValue.ToString(), UpMesCode = s.UpMesCode }).ToList();
                dcParams.AddRange(inBoxDatas.Where(w => w.ModuleInBoxDataType == ModuleInBoxDataTypeEnum.模组码).Select(s => new DcParamValue { DataType = ValueTypeEnum.TEXT, ParamValue = s.ModuleCode.ToString(), UpMesCode = s.UpMesCode }).ToList());
                dcParams.AddRange(inBoxDatas.Where(w => w.ModuleInBoxDataType == ModuleInBoxDataTypeEnum.入箱完成时间).Select(s => new DcParamValue { DataType = ValueTypeEnum.TEXT, ParamValue = s.DataValue.ToString(), UpMesCode = s.UpMesCode }).ToList());

                return dcParams.ToOkResult<IList<DcParamValue>, ServiceErrResponse>();
            }
            catch (Exception ex)
            {
                await tras.RollbackAsync();
                var resp = new ServiceErrResponse();
                return resp.ToError(ResponseErrorType.上位机错误, 500, ex.Message).ToErrResult<IList<DcParamValue>, ServiceErrResponse>();
            }
        }

        private bool CheckAvilied(float data, decimal min, decimal max)
        {
            return data >= (float)min && data <= (float)max;
        }

        private async Task<FSharpResult<ServiceErrResponse, ServiceErrResponse>> DealAssembleDataAndUpload(IList<Base_StationTask> stationTasks, ModuleInBoxSingleModuleUploadDto dto)
        {
            try
            {
                var resp = new ServiceErrResponse();

                if (string.IsNullOrWhiteSpace(dto.ModuleCode))
                {
                    return resp.ToError(ResponseErrorType.数据异常, 500, $"没有提供模组条码，请检查后重试").ToErrResult<ServiceErrResponse, ServiceErrResponse>();
                }
                if (dto.ModuleLocation <= 0)
                {
                    return resp.ToError(ResponseErrorType.数据异常, 500, $"上传的模组位置异常: [{dto.ModuleLocation}]，请检查后重试").ToErrResult<ServiceErrResponse, ServiceErrResponse>();
                }

                //获取模组抓取记录
                var moduleGrapRecord = await _dbContext.Proc_ModuleInBox_GrapRecords
                                 .Where(e => e.ModuleCode == dto.ModuleCode)
                                 .FirstOrDefaultAsync();
                if (moduleGrapRecord == null)
                {
                    return resp.ToError(ResponseErrorType.数据异常, 1400, $"模组[{dto.ModuleCode}]未找到抓取记录").ToErrResult<ServiceErrResponse, ServiceErrResponse>();
                }

                var stationTask = stationTasks.FirstOrDefault();
                if (stationTasks.Count == 0 || stationTasks.Count > 1 || stationTask == null)
                {
                    return resp.ToError(ResponseErrorType.上位机错误, 500, $"没有配置模组入箱任务或配置的数量异常：{stationTasks.Count}，请检查后重试").ToErrResult<ServiceErrResponse, ServiceErrResponse>();
                }

                var inBoxDatas = new List<ModuleInBoxDataJsonModel>();
                //获取配方
                var formulas = await _dbContext.Base_StationTask_AutoModuleInBoxs.AsNoTracking()
                    .Where(w => w.StationTaskId == stationTask.Id && !w.IsDeleted)
                    .ToListAsync();

                var formula = formulas.Where(e => e.ModuleInBoxDataType == ModuleInBoxDataTypeEnum.模组码 && e.Location == dto.ModuleLocation).FirstOrDefault();
                if (formula == null)
                {
                    return resp.ToError(ResponseErrorType.上位机错误, 500, $"未找到入箱位置[{dto.ModuleLocation}]的模组码配置，请检查后重试").ToErrResult<ServiceErrResponse, ServiceErrResponse>();
                }
                //校验模组PN
                if (!string.IsNullOrWhiteSpace(formula.ModulePN))
                {
                    if (moduleGrapRecord.ModulePN != formula.ModulePN)
                    {
                        return resp.ToError(ResponseErrorType.数据异常, 1400, $"模组{dto.ModuleCode}的PN{moduleGrapRecord.ModulePN}与配置的PN{formula.ModulePN}不一致").ToErrResult<ServiceErrResponse, ServiceErrResponse>();
                    }
                }

                //校验模组是否已被装配
                var usedModuleInfo = await _dbContext.Proc_ModuleInBox_GrapRecords.AsNoTracking()
                    .Where(e => e.HasUsed == true && e.ModuleCode == dto.ModuleCode && !e.IsDeleted).FirstOrDefaultAsync();
                if(usedModuleInfo != null)
                {
                    return resp.ToError(ResponseErrorType.数据异常, 1400, $"模组[{usedModuleInfo.ModuleCode}]已被扣料，无法重复装配").ToErrResult<ServiceErrResponse, ServiceErrResponse>();
                }

                //校验Pack当前位置是否可入
                var originalModuleInfo = await _dbContext.Proc_ModuleInBox_GrapRecords.AsNoTracking()
                    .Where(e => e.PackCode == dto.PackCode && e.ModuleLocation == dto.ModuleLocation).FirstOrDefaultAsync();
                if (originalModuleInfo != null)
                {
                    if (originalModuleInfo.ModuleCode != moduleGrapRecord.ModuleCode && originalModuleInfo.HasUsed)
                    {
                        return resp.ToError(ResponseErrorType.数据异常, 1400, $"位置[{dto.ModuleLocation}]已装配模组[{originalModuleInfo.ModuleCode}]，无法重复装配").ToErrResult<ServiceErrResponse, ServiceErrResponse>();
                    }
                }
                else
                {
                    //如果没有则更新抓取记录
                    moduleGrapRecord.PackCode = dto.PackCode;
                    moduleGrapRecord.ModuleLocation = dto.ModuleLocation;
                }

                //上传组装数据到CATL
                if (!moduleGrapRecord.HasUsed)
                {
                    var bomDatas = new List<BomData>
                    {
                        new() { TracingType = TracingTypeEnum.扫库存, UseNum = 1, InternalCode = moduleGrapRecord.ModuleCode }
                    };
                    var res = await AssembleToCatlMes(bomDatas, dto.PackCode, dto.StationCode);
                    if (res.IsError)
                    {
                        return res.ErrorValue.ToErrResult<ServiceErrResponse, ServiceErrResponse>();
                    }
                }

                //更新状态为已使用
                moduleGrapRecord.HasUsed = true;
                moduleGrapRecord.UpdateTime = DateTime.Now;

                //保存数据库
                await _dbContext.SaveChangesAsync();

                return resp.ToOkResult<ServiceErrResponse, ServiceErrResponse>();
            }
            catch (Exception ex)
            {
                var resp = new ServiceErrResponse();
                return resp.ToError(ResponseErrorType.上位机错误, 500, ex.Message).ToErrResult<ServiceErrResponse, ServiceErrResponse>();
            }
        }

        private async Task<FSharpResult<CatlMESReponse, ServiceErrResponse>> AssembleToCatlMes(IList<BomData> bomDatas, string sfc, string stationCode)
        {
            try
            {
                var result = await _mesInvoker.MiAssembleAndCollectDataForSfc(bomDatas, sfc, stationCode);
                if (result.code == 0)
                {
                    return result.ToOkResult<CatlMESReponse, ServiceErrResponse>();
                }
                var resp = new ServiceErrResponse();
                return resp.ToError(ResponseErrorType.CatlMes错误, result.code, result.message).ToErrResult<CatlMESReponse, ServiceErrResponse>();
            }
            catch (Exception ex)
            {
                var resp = new ServiceErrResponse();
                return resp.ToError(ResponseErrorType.上位机错误, 500, ex.Message).ToErrResult<CatlMESReponse, ServiceErrResponse>();
            }
        }

        private async Task<FSharpResult<ServiceErrResponse, ServiceErrResponse>> CheckModulePN(IList<Base_StationTask> stationTasks, string moduleCode, string modulePN, int moduleLocation)
        {
            try
            {
                var resp = new ServiceErrResponse();

                if (string.IsNullOrWhiteSpace(moduleCode))
                {
                    return resp.ToError(ResponseErrorType.数据异常, 500, $"没有提供模组条码，请检查后重试").ToErrResult<ServiceErrResponse, ServiceErrResponse>();
                }
                if (moduleLocation <= 0)
                {
                    return resp.ToError(ResponseErrorType.数据异常, 500, $"上传的模组位置异常: [{moduleLocation}]，请检查后重试").ToErrResult<ServiceErrResponse, ServiceErrResponse>();
                }

                var stationTask = stationTasks.FirstOrDefault();
                if (stationTasks.Count == 0 || stationTasks.Count > 1 || stationTask == null)
                {
                    return resp.ToError(ResponseErrorType.上位机错误, 500, $"没有配置模组入箱任务或配置的数量异常：{stationTasks.Count}，请检查后重试").ToErrResult<ServiceErrResponse, ServiceErrResponse>();
                }

                var inBoxDatas = new List<ModuleInBoxDataJsonModel>();
                //获取配方
                var formulas = await _dbContext.Base_StationTask_AutoModuleInBoxs.AsNoTracking()
                    .Where(w => w.StationTaskId == stationTask.Id && !w.IsDeleted)
                    .ToListAsync();

                var formula = formulas.Where(e => e.ModuleInBoxDataType == ModuleInBoxDataTypeEnum.模组码 && e.Location == moduleLocation).FirstOrDefault();
                if (formula == null)
                {
                    return resp.ToError(ResponseErrorType.上位机错误, 500, $"未找到入箱位置[{moduleLocation}]的模组码配置，请检查后重试").ToErrResult<ServiceErrResponse, ServiceErrResponse>();
                }
                //校验模组PN
                if (!string.IsNullOrWhiteSpace(formula.ModulePN))
                {
                    if (modulePN != formula.ModulePN)
                    {
                        return resp.ToError(ResponseErrorType.数据异常, 1400, $"模组{moduleCode}的PN{modulePN}与配置的PN{formula.ModulePN}不一致").ToErrResult<ServiceErrResponse, ServiceErrResponse>();
                    }
                }

                return resp.ToSuccess().ToOkResult<ServiceErrResponse, ServiceErrResponse>();
            }
            catch (Exception ex)
            {
                var resp = new ServiceErrResponse();
                return resp.ToError(ResponseErrorType.上位机错误, 500, ex.Message).ToErrResult<ServiceErrResponse, ServiceErrResponse>();
            }
        }

        private async Task<FSharpResult<CatlMESReponse, ServiceErrResponse>> CheckModule(string packCode, string moduleCode, string stationCode)
        {
            try
            {
                //校验模组
                var checkResult = await _mesInvoker.CheckInventoryAttributes(packCode, moduleCode, stationCode);
                if (checkResult.code == 0)
                {
                    return checkResult.ToOkResult<CatlMESReponse, ServiceErrResponse>();
                }
                var resp = new ServiceErrResponse();
                return resp.ToError(ResponseErrorType.CatlMes错误, checkResult.code, checkResult.message).ToErrResult<CatlMESReponse, ServiceErrResponse>();

            }
            catch (Exception ex)
            {
                var resp = new ServiceErrResponse();
                return resp.ToError(ResponseErrorType.上位机错误, 500, ex.Message).ToErrResult<CatlMESReponse, ServiceErrResponse>();
            }
        }

        public async Task<Response<IList<Proc_ModuleInBox_DataCollect>>> LoadData(ModuleInBoxDataDto dto)
        {
            var resp = new Response<IList<Proc_ModuleInBox_DataCollect>>();
            try
            {
                var datas = await _dbContext.Proc_ModuleInBox_DataCollects.Where(w => !w.IsDeleted).OrderByDescending(o => o.Id).ToListAsync();
                if (!string.IsNullOrEmpty(dto.PackCode))
                {
                    datas = datas.Where(w => w.PackCode == dto.PackCode).ToList();
                }
                if (!string.IsNullOrEmpty(dto.StationCode))
                {
                    datas = datas.Where(w => w.StationCode == dto.StationCode).ToList();
                }
                resp.Count = datas.Count();
                resp.Result = datas.Skip(dto.Limited * (dto.Page - 1)).Take(dto.Limited).ToList();
            }
            catch (Exception ex)
            {
                resp.Code = 500;
                resp.Message = ex.Message;

            }
            return resp;
        }

        public async Task<Response<IList<ModuleInBoxDataJsonModel>>> LoadDataDetail(int dataId)
        {
            var resp = new Response<IList<ModuleInBoxDataJsonModel>>();
            try
            {
                var data = await _dbContext.Proc_ModuleInBox_DataCollects.FirstOrDefaultAsync(w => w.Id == dataId);
                if (data == null)
                {
                    resp.Code = 500;
                    resp.Message = $"记录未找到";
                    return resp;
                }
                var json = JsonConvert.DeserializeObject<IList<ModuleInBoxDataJsonModel>>(data.ModuleInBoxDataJson);
                resp.Result = json;
            }
            catch (Exception ex)
            {
                resp.Code = 500;
                resp.Message = ex.Message;

            }
            return resp;
        }

        public async Task<Response<CatlMESReponse>> UploadDataAgain(string packCode, string stationCode, bool isNeedChangeStatus)
        {
            var resp = new Response<CatlMESReponse>();
            try
            {
                var main = await _dbContext.Proc_StationTask_Mains.FirstOrDefaultAsync(f => !f.IsDeleted && f.PackCode == packCode && f.StationCode == stationCode);
                if (main == null)
                {
                    resp.Code = 500;
                    resp.Message = $"条码-{packCode}在工位-{stationCode}的主记录未找到！";
                    return resp;
                }
                var dcParams = new List<DcParamValue>();
                var record = await _dbContext.Proc_StationTask_Records.Where(w => w.Proc_StationTask_MainId == main.Id && !w.IsDeleted).ToListAsync();
                foreach (var item in record)
                {
                    var timeOut = await _dbContext.Proc_StationTask_CheckTimeouts.FirstOrDefaultAsync(f => f.StationTask_RecordId == item.Id);
                    if (timeOut != null)
                    {
                        dcParams.Add(new DcParamValue
                        {
                            DataType = ValueTypeEnum.NUMBER,
                            ParamValue = timeOut.Time.ToString(),
                            UpMesCode = timeOut.UpMesCode ?? ""
                        });
                        break;
                    }
                }
                var moduleInBoxData = await _dbContext.Proc_ModuleInBox_DataCollects.FirstOrDefaultAsync(f => f.Proc_StationTask_MainId == main.Id);
                if (moduleInBoxData == null)
                {
                    resp.Code = 500;
                    resp.Message = $"没有找到条码-{packCode}在工位-{stationCode}的模组入箱数据！";
                    return resp;
                }
                var data = JsonConvert.DeserializeObject<IList<ModuleInBoxDataJsonModel>>(moduleInBoxData.ModuleInBoxDataJson);
                if (data == null)
                {
                    resp.Code = 500;
                    resp.Message = $"没有找到条码-{packCode}在工位-{stationCode}的模组入箱数据！";
                    return resp;
                }
                dcParams.AddRange(data.Where(w => w.ModuleInBoxDataType != ModuleInBoxDataTypeEnum.模组码 && w.ModuleInBoxDataType != ModuleInBoxDataTypeEnum.入箱完成时间).Select(s => new DcParamValue { DataType = ValueTypeEnum.NUMBER, ParamValue = s.DataValue.ToString(), UpMesCode = s.UpMesCode }));
                dcParams.AddRange(data.Where(w => w.ModuleInBoxDataType == ModuleInBoxDataTypeEnum.模组码).Select(s => new DcParamValue { DataType = ValueTypeEnum.TEXT, ParamValue = s.ModuleCode.ToString(), UpMesCode = s.UpMesCode }));
                dcParams.AddRange(data.Where(w => w.ModuleInBoxDataType == ModuleInBoxDataTypeEnum.入箱完成时间).Select(s => new DcParamValue { DataType = ValueTypeEnum.TEXT, ParamValue = s.DataValue.ToString(), UpMesCode = s.UpMesCode }));
                resp.Result = await _mesInvoker.dataCollect(main.PackCode, dcParams, isNeedChangeStatus, stationCode);
            }
            catch (Exception ex)
            {
                resp.Code = 500;
                resp.Message = ex.Message;
            }
            return resp;
        }

        public async Task<FSharpResult<CatlMESReponse, ServiceErrResponse>> GetModuleCleanTime(string moduleCode, string stationCode)
        {
            try
            {
                ////从字典表获取配置的固定PackCode
                //var dictionary = await _dbContext.Dictionaries.AsNoTracking()
                //    .Where(e => e.IsDeleted == false && e.Code == "ModuleCleanMaxMinute")
                //    .FirstOrDefaultAsync();
                //if (dictionary == null)
                //{
                //    return new ServiceErrResponse().ToError(ResponseErrorType.上位机错误, 404, "未找到字典表[ModuleCleanMaxMinute]配置信息").ToErrResult<CatlMESReponse, ServiceErrResponse>();
                //}
                //var dicDetail = await _dbContext.DictionaryDetails.AsNoTracking()
                //    .Where(e => e.IsDeleted == false && e.DictionaryId == dictionary.Id)
                //    .OrderBy(e => e.Id)
                //    .FirstOrDefaultAsync();
                //if (dicDetail == null)
                //{
                //    return new ServiceErrResponse().ToError(ResponseErrorType.上位机错误, 404, "未找到字典明细表配置信息").ToErrResult<CatlMESReponse, ServiceErrResponse>();
                //}
                //if (string.IsNullOrWhiteSpace(dicDetail.Code))
                //{
                //    return new ServiceErrResponse().ToError(ResponseErrorType.上位机错误, 404, "字典明细表配置Code为空").ToErrResult<CatlMESReponse, ServiceErrResponse>();
                //}
                //var packCode = dicDetail.Code;

                var checkResult = await _mesInvoker.GetParametricValue(moduleCode, "QXSJMES1", stationCode);
                return checkResult.ToOkResult<CatlMESReponse, ServiceErrResponse>();
            }
            catch (Exception ex)
            {
                var resp = new ServiceErrResponse();
                return resp.ToError(ResponseErrorType.上位机错误, 500, ex.Message).ToErrResult<CatlMESReponse, ServiceErrResponse>();
            }
        }
    }
}
