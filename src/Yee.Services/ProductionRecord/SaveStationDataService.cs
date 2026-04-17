using System.Globalization;

using AsZero.Core.Entities;
using AsZero.Core.Services.Repos;
using AsZero.Core.Services.Sys_Logs;
using AsZero.DbContexts;

using Itminus.FSharpExtensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.FSharp.Core;

using Newtonsoft.Json;

using Yee.Common.Library.CommonEnum;
using Yee.Entitys;
using Yee.Entitys.AutomaticStation;
using Yee.Entitys.CATL;
using Yee.Entitys.CommonEntity;
using Yee.Entitys.DBEntity;
using Yee.Entitys.DBEntity.Production;
using Yee.Entitys.DBEntity.ProductionRecords;
using Yee.Entitys.DTOS;
using Yee.Entitys.DTOS.StationTaskDataDTOS;
using Yee.Entitys.Production;
using Yee.Services.BaseData;
using Yee.Services.HistoryData;

namespace Yee.Services.Production;

public class SaveStationDataService
{

    private readonly HistoryData_APIService _HistoryData_APIService;
    private readonly ILogger<SaveStationDataService> _logger;
    public readonly AsZeroDbContext _dbContext;
    private readonly StationService _StationService;
    private readonly FlowService flowService;
    private readonly ProductService productService;
    private readonly Proc_Product_OffLineService proc_Product_OffLineService;
    private readonly IConfiguration _configuration;
    private readonly SysLogService sysLogService;
    private readonly Proc_StationTask_PeiFangService _PeiFangService;

    private readonly bool IsDebug = false;
    public SaveStationDataService(AsZeroDbContext dBContext, IConfiguration configuration, StationService stationService, FlowService flowService, ProductService productService, Proc_Product_OffLineService proc_Product_OffLineService,
        ILogger<SaveStationDataService> logger, SysLogService sysLogService, Proc_StationTask_PeiFangService peiFangService, HistoryData_APIService historyData_APIService)
    {
        _dbContext = dBContext;
        this._StationService = stationService;
        this.flowService = flowService;
        this.productService = productService;
        this.proc_Product_OffLineService = proc_Product_OffLineService;
        _logger = logger;
        this.sysLogService = sysLogService;
        _configuration = configuration;
        IsDebug = _configuration.GetSection("AppOpts").Get<AppOpts>().IsDebug;
        _PeiFangService = peiFangService;
        _HistoryData_APIService = historyData_APIService;
    }

    public async Task<Response<Proc_GluingInfo>> GetGluingStartTime(string packPN)
    {
        var result = new Response<Proc_GluingInfo>();
        try
        {
            if (packPN != null)
            {
                //var outGoods = _dbContext.Proc_Join_PackAndOuterCodes.FirstOrDefault(o => !o.IsDeleted && o.PackCode == packPN);
                //if (outGoods == null || string.IsNullOrEmpty(outGoods.OuterGoodsCode))
                //{
                //    result.Code = 500;
                //    result.Message = "必须先扫描下箱体条码";
                //    return result;
                //}
                //假设有多条涂胶数据 默认查询最新的涂胶时间
                //var time = _dbContext.Proc_GluingInfos.OrderByDescending(ps => ps.Id).FirstOrDefault(ps => ps.PackPN == outGoods.OuterGoodsCode && ps.ParameterName == "涂胶开始时间" && ps.IsDeleted == false);
                //if (time != null)
                //{
                //    result.Code = 200;
                //    result.Message = "操作成功";
                //    result.Result = time;
                //}
                //else
                //{
                //    result.Code = 500;
                //    result.Message = "未查询到涂胶开始时间";
                //}
            }
            else
            {
                result.Code = 500;
                result.Message = "Pack码不能为空";
            }
        }
        catch (Exception ex)
        {
            result.Code = 500;
            result.Message = "当前pack未进行涂胶";
        }
        return result;
    }

    public async Task<Response<DateTime>> GetStepTaskEndTime(string packPN, string stepCode)
    {
        var result = new Response<DateTime>();
        try
        {
            if (packPN != null)
            {
                var step = _dbContext.Base_Steps.FirstOrDefault(s => !s.IsDeleted && s.Code == stepCode);
                if (step == null)
                {
                    result.Code = 500;
                    result.Message = "找不到对应的工序";
                    return result;
                }

                //假设有多条涂胶数据 默认查询最新的涂胶时间
                var time = _dbContext.Proc_StationTask_Mains.OrderByDescending(ps => ps.Id).FirstOrDefault(ps => ps.PackCode == packPN && ps.IsDeleted == false && ps.StepId == step.Id && ps.Status == StationTaskStatusEnum.已完成);
                if (time == null)
                {
                    result.Code = 500;
                    result.Message = $"未查询到此Pack码在工序{stepCode}的完工信息";
                    return result;
                }
                else
                {
                    result.Code = 200;
                    result.Message = "操作成功";
                    result.Result = (DateTime)time.UpdateTime;
                }
            }
            else
            {
                result.Code = 500;
                result.Message = "Pack码不能为空";
            }
        }
        catch (Exception ex)
        {
            result.Code = 500;
            result.Message = ex.Message;
        }
        return result;
    }


    public async Task<Response<DateTime>> GetModuleInBoxEndTime(string packPN, string stepCode, string preStepCode = "OP050")
    {
        var result = new Response<DateTime>();
        try
        {
            if (packPN != null)
            {
                //var outGoods = _dbContext.Proc_Join_PackAndOuterCodes.FirstOrDefault(o => !o.IsDeleted && o.PackCode == packPN);
                //if (outGoods == null || string.IsNullOrEmpty(outGoods.OuterGoodsCode))
                //{
                //    result.Code = 500;
                //    result.Message = "必须先扫描下箱体条码";
                //    return result;
                //}
                //var moduleInBoxTaskMain = await _dbContext.Proc_StationTask_Mains.Include(m => m.Step).FirstOrDefaultAsync(ps => ps.IsDeleted == false && ps.Step.Code == stepCode && ps.PackCode == outGoods.OuterGoodsCode && ps.Status == StationTaskStatusEnum.已完成);
                //if (moduleInBoxTaskMain != null)
                //{
                //    result.Code = 200;
                //    result.Message = "操作成功";
                //    result.Result = (DateTime)moduleInBoxTaskMain.UpdateTime;
                //}
                //else
                //{
                //    moduleInBoxTaskMain = await _dbContext.Proc_StationTask_Mains.Include(m => m.Step).FirstOrDefaultAsync(ps => ps.IsDeleted == false && ps.Step.Code == preStepCode && ps.PackCode == outGoods.OuterGoodsCode && ps.Status == StationTaskStatusEnum.已完成);
                //    if (moduleInBoxTaskMain != null)
                //    {
                //        result.Code = 200;
                //        result.Message = "操作成功";
                //        result.Result = (DateTime)moduleInBoxTaskMain.UpdateTime;
                //    }
                //    else
                //    {
                //        result.Code = 500;
                //        result.Message = "未查询到入箱完成时间";
                //    }
                //}
            }
            else
            {
                result.Code = 500;
                result.Message = "Pack码不能为空";
            }
        }
        catch (Exception ex)
        {
            result.Code = 500;
            result.Message = "当前pack未进行涂胶";
        }
        return result;
    }


    public async Task<Response<DateTime>> GetBoltGunEndTime(string packPN, string stepCode)
    {
        var result = new Response<DateTime>();
        try
        {
            if (packPN != null)
            {
                var moduleInBoxTaskMain = await _dbContext.Proc_StationTask_Mains.Include(m => m.Step).FirstOrDefaultAsync(ps => ps.IsDeleted == false && ps.Step.Code == stepCode && ps.PackCode == packPN && ps.Status == StationTaskStatusEnum.已完成);
                if (moduleInBoxTaskMain != null)
                {
                    result.Code = 200;
                    result.Message = "操作成功";
                    result.Result = (DateTime)moduleInBoxTaskMain.CreateTime;
                }
                else
                {
                    result.Code = 500;
                    result.Message = "未查询到模组拧紧完成时间";
                }
            }
            else
            {
                result.Code = 500;
                result.Message = "Pack码不能为空";
            }
        }
        catch (Exception ex)
        {
            result.Code = 500;
            result.Message = "当前pack未进行模组拧紧";
        }
        return result;
    }


    public async Task<Response<Base_StationTaskBom>> GetOutScanRange(string stationTaskId)
    {
        var result = new Response<Base_StationTaskBom>();
        try
        {
            var taskId = await _dbContext.Base_StationTaskBoms.FirstOrDefaultAsync(t => t.StationTaskId == Convert.ToInt16(stationTaskId) && t.IsDeleted == false);

            result.Code = 200;
            result.Result = taskId;
        }
        catch (Exception ex)
        {
            result.Code = 500;
            result.Message = "未配置扫码任务";
        }
        return result;
    }


    public async Task<Response> DealCatlMES_InStation(InStationDataDTO inStationData)
    {
        var result = new Response();
        try
        {
            result.Code = 200;
            result.Message = "保存成功";
            // 1、 查找生产主记录
            var pStationTaskMain = await _dbContext.Proc_StationTask_Mains.FirstOrDefaultAsync(ps => ps.IsDeleted == false && ps.StepId == inStationData.StepId && ps.PackCode == inStationData.PackCode && (ps.Status == StationTaskStatusEnum.未开始 || ps.Status == StationTaskStatusEnum.进行中));
            if (pStationTaskMain == null)
            {
                var peiFID = await _PeiFangService.GetOrCreateNewPeiFangJson_MD5(JsonConvert.SerializeObject(inStationData.StationTaskList));
                DateTimeOffset utcTime = DateTime.Now;

                // 没有主记录 新增主记录
                var newMain = _dbContext.Proc_StationTask_Mains.Add(new Proc_StationTask_Main
                {
                    PeiFang_MD5_ID = peiFID,
                    StepId = inStationData.StepId,
                    StationId = inStationData.StationId,
                    Status = StationTaskStatusEnum.进行中,
                    CurStepNo = 0,
                    UseAGVCode = inStationData.AGVNo,
                    PackCode = inStationData.PackCode,
                    Timespan = utcTime.ToUnixTimeMilliseconds()
                });
                pStationTaskMain = newMain.Entity;
            }

            foreach (var item in inStationData.StationTaskList)
            {
                if (item.StationTask.Type != StationTaskTypeEnum.扫描员工卡)
                {
                    // 2、根据生产主记录查找任务主记录
                    var orgMainRecord = _dbContext.Proc_StationTask_Records.Include(s => s.Base_StationTask).FirstOrDefault(ps => ps.IsDeleted == false &&
                        (ps.Status == StationTaskStatusEnum.未开始 || ps.Status == StationTaskStatusEnum.进行中) && ps.Proc_StationTask_Main == pStationTaskMain);
                    if (orgMainRecord == null)
                    {
                        // 没有任务主记录 新增
                        var newMainRecord = _dbContext.Proc_StationTask_Records.Add(new Proc_StationTask_Record
                        {
                            UseAGVCode = item.AGVCode,
                            Proc_StationTask_Main = pStationTaskMain,
                            Base_StationTaskId = item.StationTaskId,
                            CreateUserID = item.CreateUserID,
                            Status = StationTaskStatusEnum.进行中,
                            TaskName = item.StationTask.Name
                        });
                        orgMainRecord = newMainRecord.Entity;
                    }
                }

            }

            _dbContext.SaveChanges();

        }
        catch (Exception ex)
        {
            result.Code = 500;
            result.Message = "保存失败！";
        }
        return result;
    }

    /// <summary>
    /// 保存扫码任务详情
    /// </summary>
    /// <param name="bom"></param>
    /// <returns></returns>
    public async Task<Response> Save_StationTaskBom(BomDataDTO dto)
    {
        var result = new Response<Proc_StationTask_BomDetail>();
        using var trans = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var bom = dto.BomData;
            if (bom == null)
            {
                result.Code = 500;
                result.Message = "未提供物料扫码信息";
                return result;
            }
            // 1、 查找生产主记录
            var pStationTaskMain = await GetMainData(dto.MainId);
            if (pStationTaskMain == null)
            {
                result.Code = 500;
                result.Message = $"主记录未查询到";
                return result;
            }

            // 2、根据生产主记录查找任务主记录
            var orgMainRecord = await CreateOrGetRecordData(dto.MainId, dto.StationTaskId, bom.GoodsName ?? "", bom.CreateUserID);
            if (orgMainRecord.Status == StationTaskStatusEnum.已完成)
            {
                result.Code = 500;
                result.Message = $"此任务已完成";
                return result;
            }

            // 3、根据任务主记录查找任务BOM
            var orgTaskBom = await _dbContext.Proc_StationTask_Boms.FirstOrDefaultAsync(ps => ps.IsDeleted == false && ps.StationTask_RecordId == orgMainRecord.Id);
            if (orgTaskBom == null)
            {
                var newBom = new Proc_StationTask_Bom
                {
                    StationTask_RecordId = orgMainRecord.Id,
                    CreateUserID = bom.CreateUserID,
                    Status = StationTaskStatusEnum.已完成,
                    HasOuterParam = bom.HasOuterParam,
                    GoodsName = bom.GoodsName,
                    GoodsPN = bom.GoodsPN,
                    TracingType = bom.TracingType,
                    PackPN = bom.PackCode,
                    Base_ProResourceId = bom.UseResourceId,
                    UseNum = bom.UseNum,
                    CurCompleteNum = bom.UseNum,
                };
                await _dbContext.AddAsync(newBom);
                await _dbContext.SaveChangesAsync();
                orgTaskBom = newBom;
            }
            var bomdeatil = new Proc_StationTask_BomDetail
            {
                Proc_StationTask_BomId = orgTaskBom.Id,
                StationId = pStationTaskMain.StationId,
                StepId = pStationTaskMain.StepId,
                UseNum = orgTaskBom.UseNum,
                CreateUserID = bom.CreateUserID,
                PackPN = bom.PackCode,
                BatchBarCode = bom.BatchBarCode,
                GoodsOuterCode = bom.GoodsOuterCode,
                HasOuterParam = bom.HasOuterParam,
                GoodsPN = bom.GoodsPN,
                GoodsName = bom.GoodsName,
                TracingType = bom.TracingType,
                UniBarCode = bom.UniBarCode,
            };
            await _dbContext.AddAsync(bomdeatil);
            await _dbContext.SaveChangesAsync();
            await trans.CommitAsync();
            result.Result = bomdeatil;
        }
        catch (Exception ex)
        {
            result.Code = 500;
            result.Message = "保存失败！";
        }
        return result;
    }

    public async Task<Response> CheckBomUsed(int? stepId, TracingTypeEnum TracingType, string? Code)
    {
        Response result = new();
        if (TracingType == TracingTypeEnum.扫库存)
        {
            var exiest = await _dbContext.Proc_StationTask_BomDetails.Where(w => !w.IsDeleted && w.StepId == stepId && w.UniBarCode == Code).AnyAsync();
            if (exiest)
            {
                result.Code = 500;
                result.Message = "扫描的库存码重复";
                return result;
            }
        }

        if (TracingType == TracingTypeEnum.精追)
        {
            var exiest = await _dbContext.Proc_StationTask_BomDetails.Where(w => !w.IsDeleted && w.StepId == stepId && w.GoodsOuterCode == Code).AnyAsync();
            if (exiest)
            {
                result.Code = 500;
                result.Message = "扫描的库存码重复";
                return result;
            }
        }
        return result;
    }

    public async Task<Response<PackTaskMainDataDTO>> LoadStepPackTaskMainData(string stationCode, DateTime beginTime, DateTime endTime)
    {
        var result = new Response<PackTaskMainDataDTO>();
        try
        {
            result.Result = new PackTaskMainDataDTO
            {
                TotalCount = 0,
                Items = new List<PackTaskRecordDataDTO>()
            };
            result.Code = 200;
            var station = await _dbContext.Base_Stations.FirstOrDefaultAsync(s => s.Code == stationCode && s.IsDeleted == false);
            if (station == null)
            {
                result.Code = 500;
                result.Message = $"未找到工位代码{stationCode}对应的数据";
                return result;
            }
            result.Result.TotalCount = _dbContext.Proc_StationTask_Mains.Count(t => t.StepId == station.StepId && t.Status == StationTaskStatusEnum.已完成 && t.CreateTime > beginTime && t.CreateTime < endTime && t.IsDeleted == false);
            var mainDatas = _dbContext.Proc_StationTask_Mains.Where(t => t.StepId == station.StepId && t.CreateTime > beginTime && t.CreateTime < endTime && t.IsDeleted == false).ToList();
            foreach (var record in mainDatas)
            {
                result.Result.Items.Add(new PackTaskRecordDataDTO
                {
                    CreateTime = record.CreateTime?.ToString("yyyy-MM-dd HH:mm:ss")!,
                    PackCode = record.PackCode,
                    Status = record.Status.ToString(),
                    StationCode = record.StationCode
                });
            }
        }
        catch (Exception ex)
        {
            result.Code = 500;
            result.Message = ex.Message;
        }
        return result;
    }

    public async Task<UploadCATLData> GetUploadCATLData(string packCode, int stepId, int stationId)
    {
        var result = new UploadCATLData
        {
            PackCode = packCode,
            DCParams = new List<DcParamValue>(),
            ScanCodeData = new List<BomData>()
        };

        var dcParams = new List<DcParamValue>();
        var main = await _dbContext.Proc_StationTask_Mains.FirstOrDefaultAsync(f =>
            !f.IsDeleted && f.StepId == stepId && f.PackCode == packCode);
        if (main == null)
        {
            return result;
        }

        var records = await _dbContext.Proc_StationTask_Records.Include(i => i.Base_StationTask).Where(w =>
            w.Base_StationTask != null && !w.Base_StationTask.IsDeleted && !w.IsDeleted &&
            w.Proc_StationTask_MainId == main.Id).ToListAsync();

        foreach (var record in records)
        {
            switch (record.Base_StationTask!.Type)
            {
                case StationTaskTypeEnum.扫描员工卡:
                    var scanAccount = await LoadScanAccountData(record.Id);
                    if (scanAccount.IsError)
                    {
                        return new UploadCATLData();
                    }
                    dcParams.AddRange(scanAccount.ResultValue);
                    break;
                case StationTaskTypeEnum.人工拧螺丝:
                    var tightenData = await LoadTightenData(record.Id);
                    if (tightenData.IsError)
                    {
                        return new UploadCATLData();
                    }
                    dcParams.AddRange(tightenData.ResultValue);
                    break;
                case StationTaskTypeEnum.称重:
                    var weightData = await LoadWeightData(record.Id);
                    if (weightData.IsError)
                    {
                        return new UploadCATLData();
                    }
                    dcParams.AddRange(weightData.ResultValue);
                    break;
                case StationTaskTypeEnum.用户输入:
                    var userInputData = await LoadUserInputData(record.Id);
                    if (userInputData.IsError)
                    {
                        return new UploadCATLData();
                    }
                    dcParams.AddRange(userInputData.ResultValue);
                    break;
                case StationTaskTypeEnum.扫码输入:
                    var scanCollectData = await LoadScanCollectData(record.Id);
                    if (scanCollectData.IsError)
                    {
                        return new UploadCATLData();
                    }
                    dcParams.AddRange(scanCollectData.ResultValue);
                    break;
                case StationTaskTypeEnum.时间记录:
                    var timeRecord = await LoadTimeRecordData(record.Id);
                    if (timeRecord.IsError)
                    {
                        return new UploadCATLData();
                    }
                    dcParams.AddRange(timeRecord.ResultValue);
                    break;
                case StationTaskTypeEnum.超时检测:
                    var timeOut = await LoadTimeCheckOutData(record.Id);
                    if (timeOut.IsError)
                    {
                        return new UploadCATLData();
                    }
                    dcParams.AddRange(timeOut.ResultValue);
                    break;
                case StationTaskTypeEnum.补拧:
                    var tightenRework = await LoadTightenReworkData(record, main.PackCode);
                    if (tightenRework.IsError)
                    {
                        return new UploadCATLData();
                    }
                    dcParams.AddRange(tightenRework.ResultValue);
                    break;
                case StationTaskTypeEnum.图示拧紧:
                    var tightenImage = await LoadTightenImageData(record.Id);
                    if (tightenImage.IsError)
                    {
                        return new UploadCATLData();
                    }
                    dcParams.AddRange(tightenImage.ResultValue);
                    break;
            }
        }

        result.DCParams = dcParams;
        return result;
    }

    private async Task<FSharpResult<IList<DcParamValue>, string>> LoadTightenImageData(int recordId)
    {
        try
        {
            // 图示拧紧同一序号可能存在多次记录（重复保存/NG/反拧/返工后再次拧紧）。
            // 上传MES时按“序号”粒度，仅取每个序号最新的 OK 结果，避免重复/脏数据上传。
            var details = await _dbContext.Proc_StationTask_TightenByImages
                .Where(w => !w.IsDeleted && w.Proc_StationTask_RecordId == recordId && w.OrderNo.HasValue && w.ResultIsOK)
                .OrderByDescending(o => o.CreateTime)
                .ThenByDescending(o => o.Id)
                .ToListAsync();

            var latestOkByOrder = details
                .Where(w => (w.OrderNo ?? 0) > 0)
                .GroupBy(g => g.OrderNo!.Value)
                .Select(g => g.First())
                .ToList();

            var torqueData = latestOkByOrder
                .Where(s => !string.IsNullOrEmpty(s.UpMesCodeTor))
                .Select(s => new DcParamValue { DataType = ValueTypeEnum.NUMBER, ParamValue = s.FinalTorque.ToString(), UpMesCode = s.UpMesCodeTor! });

            var angleData = latestOkByOrder
                .Where(s => !string.IsNullOrEmpty(s.UpMesCodeAng))
                .Select(s => new DcParamValue { DataType = ValueTypeEnum.NUMBER, ParamValue = s.FinalAngle.ToString(), UpMesCode = s.UpMesCodeAng! });

            return torqueData.Concat(angleData).ToList().ToOkResult<IList<DcParamValue>, string>();
        }
        catch (Exception ex)
        {
            return ex.Message.ToErrResult<IList<DcParamValue>, string>();
        }
    }

    private async Task<FSharpResult<IList<DcParamValue>, string>> LoadTightenData(int recordId)
    {
        try
        {
            var q = from blotGunDetail in _dbContext.Proc_StationTask_BlotGunDetails
                    join blotGun in _dbContext.Proc_StationTask_BlotGuns on blotGunDetail.Proc_StationTask_BlotGunId equals blotGun.Id
                    where blotGun.StationTask_RecordId == recordId && blotGunDetail.ResultIsOK && !blotGun.IsDeleted && !blotGunDetail.IsDeleted
                    select blotGunDetail;
            var boltGunData = await q.ToListAsync();

            var torqueData = boltGunData.Select(s => new DcParamValue
            {
                DataType = ValueTypeEnum.NUMBER,
                ParamValue = s.FinalTorque.ToString(CultureInfo.InvariantCulture),
                UpMesCode = s.UploadCode ?? ""
            });
            var angleData = boltGunData.Select(s => new DcParamValue
            {
                DataType = ValueTypeEnum.NUMBER,
                ParamValue = s.FinalAngle.ToString(CultureInfo.InvariantCulture),
                UpMesCode = s.UploadCode_JD ?? ""
            });

            return torqueData.Concat(angleData).ToList().ToOkResult<IList<DcParamValue>, string>();
        }
        catch (Exception ex)
        {
            return ex.Message.ToErrResult<IList<DcParamValue>, string>();
        }
    }

    private async Task<FSharpResult<IList<DcParamValue>, string>> LoadScanAccountData(int recordId)
    {
        try
        {
            var scanAccountData = await _dbContext.Proc_StationTask_ScanAccountCards
                .Where(w => !w.IsDeleted && w.StationTask_RecordId == recordId).Select(s => new DcParamValue
                {
                    DataType = ValueTypeEnum.TEXT,
                    ParamValue = s.AccountValue!.ToString(),
                    UpMesCode = s.UpMesCode!
                }).ToListAsync();

            return scanAccountData.Where(w => !string.IsNullOrEmpty(w.UpMesCode)).ToList()
                .ToOkResult<IList<DcParamValue>, string>();
        }
        catch (Exception ex)
        {
            return ex.Message.ToErrResult<IList<DcParamValue>, string>();
        }
    }

    private async Task<FSharpResult<IList<DcParamValue>, string>> LoadWeightData(int recordId)
    {
        try
        {
            var weightData = await _dbContext.Proc_StationTask_AnyLoads
                .Where(w => !w.IsDeleted && w.StationTask_RecordId == recordId).Select(s => new DcParamValue
                {
                    DataType = ValueTypeEnum.NUMBER,
                    ParamValue = s.WeightData.ToString(CultureInfo.InvariantCulture),
                    UpMesCode = s.UpMesCode ?? ""
                }).ToListAsync();
            return weightData.ToOkResult<IList<DcParamValue>, string>();
        }
        catch (Exception ex)
        {
            return ex.Message.ToErrResult<IList<DcParamValue>, string>();
        }
    }

    private async Task<FSharpResult<IList<DcParamValue>, string>> LoadUserInputData(int recordId)
    {
        try
        {
            var userInputData = await _dbContext.Proc_StationTask_UserInputs
                .Where(w => !w.IsDeleted && w.StationTask_RecordId == recordId).Select(s => new DcParamValue
                { DataType = ValueTypeEnum.NUMBER, ParamValue = s.UserInputData, UpMesCode = s.UpMesCode })
                .ToListAsync();
            return userInputData.ToOkResult<IList<DcParamValue>, string>();
        }
        catch (Exception ex)
        {
            return ex.Message.ToErrResult<IList<DcParamValue>, string>();
        }
    }

    private async Task<FSharpResult<IList<DcParamValue>, string>> LoadScanCollectData(int recordId)
    {
        try
        {
            var userInputData = await _dbContext.Proc_StationTask_ScanCollects
                .Where(w => !w.IsDeleted && w.StationTask_RecordId == recordId).Select(s => new DcParamValue
                { DataType = ValueTypeEnum.NUMBER, ParamValue = s.ScanCollectData, UpMesCode = s.UpMesCode })
                .ToListAsync();
            return userInputData.ToOkResult<IList<DcParamValue>, string>();
        }
        catch (Exception ex)
        {
            return ex.Message.ToErrResult<IList<DcParamValue>, string>();
        }
    }

    private async Task<FSharpResult<IList<DcParamValue>, string>> LoadTimeRecordData(int recordId)
    {
        try
        {
            var timeRecord = await _dbContext.Proc_StationTask_TimeRecords
                .Where(w => !w.IsDeleted && w.Proc_StationTask_RecordId == recordId).Select(s => new DcParamValue
                { DataType = ValueTypeEnum.TEXT, ParamValue = s.TimeValue, UpMesCode = s.UploadMesCode })
                .ToListAsync();
            return timeRecord.ToOkResult<IList<DcParamValue>, string>();
        }
        catch (Exception ex)
        {
            return ex.Message.ToErrResult<IList<DcParamValue>, string>();
        }
    }

    private async Task<FSharpResult<IList<DcParamValue>, string>> LoadTimeCheckOutData(int recordId)
    {
        try
        {
            var timeRecord = await _dbContext.Proc_StationTask_CheckTimeouts
                .Where(w => !w.IsDeleted && w.StationTask_RecordId == recordId).Select(s => new DcParamValue
                { DataType = ValueTypeEnum.NUMBER, ParamValue = s.Time.ToString(CultureInfo.InvariantCulture), UpMesCode = s.UpMesCode! })
                .ToListAsync();
            return timeRecord.ToOkResult<IList<DcParamValue>, string>();
        }
        catch (Exception ex)
        {
            return ex.Message.ToErrResult<IList<DcParamValue>, string>();
        }
    }

    /// <summary>
    /// 加载/处理补拧数据以上传至CATL MES
    /// </summary>
    /// <param name="record"></param>
    /// <param name="packCode"></param>
    /// <returns></returns>
    private async Task<FSharpResult<IList<DcParamValue>, string>> LoadTightenReworkData(Proc_StationTask_Record record, string packCode)
    {
        try
        {
            #region 配方判空

            var tightenFormula =
                await _dbContext.Base_StationTask_TightenReworks.FirstOrDefaultAsync(f =>
                    f.StationTaskId == record.Base_StationTaskId && !f.IsDeleted);
            if (tightenFormula == null)
            {
                return "当前补拧站没有配置补拧任务配方".ToErrResult<IList<DcParamValue>, string>();
            }

            #endregion

            # region 处理人工补拧数据（扭矩、角度）
            var tightenRework = await _dbContext.Proc_StationTask_TightenReworks
                .Where(w => !w.IsDeleted && w.StationTaskRecordId == record.Id && w.ResultOk).ToListAsync();
            var dcParamsTorqueR = tightenRework.Select(s => new DcParamValue
            {
                DataType = ValueTypeEnum.NUMBER,
                ParamValue = s.TorqueValue.ToString(CultureInfo.InvariantCulture),
                UpMesCode = s.UpMesCode
            }).ToList();
            var dcParamsAngleR = tightenRework.Select(s => new DcParamValue
            {
                DataType = ValueTypeEnum.NUMBER,
                ParamValue = s.AngleValue.ToString(CultureInfo.InvariantCulture),
                UpMesCode = s.UpMesCodeJD
            }).ToList();
            #endregion

            #region 处理自动拧紧数据（扭矩、角度）

            var tightenData = new List<DcParamValue>();
            // 优先处理新自动站的拧紧数据
            var external = await _dbContext.Proc_ExternalAutoTightenDatas
                .Where(w => !w.IsDeleted && w.Sfc == packCode && w.TightenType == tightenFormula.ReworkType)
                .FirstOrDefaultAsync();
            if (external != null)
            {
                var tighteningResults =
                    JsonConvert.DeserializeObject<List<TighteningResult>>(external.TighteningResultJson) ??
                    new List<TighteningResult>();

                foreach (var item in tighteningResults.Where(w => w.IsOk).OrderBy(o => o.OrderNo))
                {
                    if (!TryParseDecimal(item.TorqueResult?.TagValue, out var torque))
                    {
                        return $"Torque TagValue无法解析, OrderNo={item.OrderNo}".ToErrResult<IList<DcParamValue>, string>();
                    }

                    if (!TryParseDecimal(item.AngleResult?.TagValue, out var angle))
                    {
                        return $"Angle TagValue无法解析, OrderNo={item.OrderNo}".ToErrResult<IList<DcParamValue>, string>();
                    }
                    // 上传代码还是使用人工补拧的配方，若有需要可换成自动拧紧config中写的上传代码
                    tightenData.Add(new DcParamValue
                    {
                        DataType = ValueTypeEnum.NUMBER,
                        ParamValue = torque.ToString(CultureInfo.InvariantCulture),
                        UpMesCode = tightenFormula.UpMesCode + item.OrderNo
                        // UpMesCode = item.TorqueResult.MesName + item.OrderNo
                    });
                    tightenData.Add(new DcParamValue
                    {
                        DataType = ValueTypeEnum.NUMBER,
                        ParamValue = angle.ToString(CultureInfo.InvariantCulture),
                        UpMesCode = tightenFormula.UpMesCode + "JD" + item.OrderNo
                        // UpMesCode = item.AngleResult.MesName + item.OrderNo
                    });
                }
            }
            else
            {
                // 兼容老的自动站程序：如果新的表里没有数据则回退至旧表查询
                var autoTightenData = await _dbContext.Proc_AutoBoltInfo_Details
                    .Where(w => !w.IsDeleted && w.PackPN == packCode && w.BoltType == tightenFormula.ReworkType.ToString())
                    .ToListAsync();

                foreach (var item in autoTightenData)
                {
                    if (item.AutoBlotInfoArray == null)
                    {
                        return "当前补拧没有对应的自动拧紧数据".ToErrResult<IList<DcParamValue>, string>();
                    }

                    var torqueData = item.AutoBlotInfoArray.Where(w => w.ResultIsOK).Select(s => new DcParamValue
                    {
                        DataType = ValueTypeEnum.NUMBER,
                        ParamValue = s.FinalTorque.ToString(CultureInfo.InvariantCulture),
                        UpMesCode = item.UploadCode + s.OrderNo
                    }).ToList();
                    var angleData = item.AutoBlotInfoArray.Where(w => w.ResultIsOK).Select(s => new DcParamValue
                    {
                        DataType = ValueTypeEnum.NUMBER,
                        ParamValue = s.FinalAngle.ToString(CultureInfo.InvariantCulture),
                        UpMesCode = item.UploadCode_JD + s.OrderNo
                    }).ToList();
                    tightenData.AddRange(torqueData);
                    tightenData.AddRange(angleData);
                }
            }

            #endregion

            tightenData.AddRange(dcParamsTorqueR);
            tightenData.AddRange(dcParamsAngleR);
            return tightenData.ToOkResult<IList<DcParamValue>, string>();
        }
        catch (Exception ex)
        {
            return ex.Message.ToErrResult<IList<DcParamValue>, string>();
        }
    }

    /// <summary>
    /// 保存称重任务详情
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    public async Task<Response> Save_StationAnyLoad(AnyLoadDataDTO dto)
    {
        var result = new Response();
        using var trans = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            var anyLoad = dto.AnyLoadData;
            var main = await GetMainData(dto.MainId);
            if (main == null)
            {
                result.Code = 500;
                result.Message = $"主记录未查询到";
                return result;
            }
            // 2、根据生产主记录查找任务主记录
            var orgMainRecord = await CreateOrGetRecordData(main.Id, dto.StationTaskId, dto.AnyLoadData?.AnyLoadName ?? "", dto.AnyLoadData?.CreateUserID);
            if (orgMainRecord.Status == StationTaskStatusEnum.已完成)
            {
                await trans.RollbackAsync();
                result.Code = 500;
                result.Message = "此任务已完成！";
                return result;
            }

            // 3、根据任务主记录查找任务
            var orgTaskBom = _dbContext.Proc_StationTask_AnyLoads.FirstOrDefault(ps => ps.IsDeleted == false && ps.StationTask_RecordId == orgMainRecord.Id);
            if (orgTaskBom == null)
            {
                var newTaskBom = _dbContext.Proc_StationTask_AnyLoads.Add(new Proc_StationTask_AnyLoad
                {
                    StationTask_RecordId = orgMainRecord.Id,
                    StationId = main.StationId,
                    StepId = main.StepId,
                    PackPN = anyLoad!.PackCode,
                    CreateUserID = anyLoad.CreateUserID,
                    Status = StationTaskStatusEnum.已完成,
                    AnyLoadName = anyLoad.AnyLoadName,
                    WeightData = anyLoad.WeightData,
                    UpMesCode = anyLoad.UpMesCode,
                });
                orgTaskBom = newTaskBom.Entity;
            }

            await _dbContext.SaveChangesAsync();
            await trans.CommitAsync();

        }
        catch (Exception ex)
        {
            await trans.RollbackAsync();
            result.Code = 500;
            result.Message = "保存失败！";
            _logger.LogError("工位任务称重数据保存失败，参数为：{SerializeObject}，异常消息为：{O}",
                JsonConvert.SerializeObject(dto.AnyLoadData), JsonConvert.SerializeObject(ex));
        }
        return result;
    }

    /// <summary>
    /// 扫描员工卡任务保存
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    public async Task<Response> Save_AccountCardData(ScanAccountCardDataDTO dto)
    {
        using var trans = await _dbContext.Database.BeginTransactionAsync();
        var result = new Response();
        try
        {
            var scanAccountCard = dto.ScanAccountCardData;
            var main = await GetMainData(dto.MainId);
            if (main == null)
            {
                result.Code = 500;
                result.Message = $"主记录未查询到";
                return result;
            }

            var orgMainRecord = await CreateOrGetRecordData(main.Id, dto.StationTaskId,
                dto.ScanAccountCardData?.ScanAccountCardName ?? "", dto.ScanAccountCardData?.CreateUserID);
            if (orgMainRecord.Status == StationTaskStatusEnum.已完成)
            {
                orgMainRecord.Status = StationTaskStatusEnum.进行中;
                _dbContext.Update(orgMainRecord);
                await _dbContext.SaveChangesAsync();
            }

            var orgTaskAccount = _dbContext.Proc_StationTask_ScanAccountCards
                .Where(ps => ps.IsDeleted == false && ps.StationTask_RecordId == orgMainRecord.Id).ToList();
            if (orgTaskAccount != null)
            {
                foreach (var org in orgTaskAccount)
                {
                    org.IsDeleted = true;
                }
            }
            _dbContext.Proc_StationTask_ScanAccountCards.Add(new Proc_StationTask_ScanAccountCard
            {
                StationTask_RecordId = orgMainRecord.Id,
                StationId = main.StationId,
                StepId = main.StepId,
                PackPN = scanAccountCard!.PackCode,
                CreateUserID = scanAccountCard.CreateUserID,
                Status = StationTaskStatusEnum.已完成,
                ScanAccountCardName = scanAccountCard.ScanAccountCardName,
                AccountValue = scanAccountCard.AccountCardValue,
                UpMesCode = scanAccountCard.UpMesCode,
            });

            await _dbContext.SaveChangesAsync();
            await trans.CommitAsync();
        }
        catch (Exception ex)
        {
            result.Code = 500;
            result.Message = "保存失败！";
            _logger.LogError($"扫描员工卡保存失败，参数为：{JsonConvert.SerializeObject(dto.ScanAccountCardData)}，异常消息为：{JsonConvert.SerializeObject(ex)}");
        }
        return result;
    }

    /// <summary>
    /// 保存用户输入任务详情
    /// </summary>
    /// <param name="bom"></param>
    /// <returns></returns>
    public async Task<Response> Save_StationUserInput(UserInputDataDTO dto)
    {
        var result = new Response();
        using var trans = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var userInput = dto.UserInputData;
            // 1、 查找生产主记录
            var main = await GetMainData(dto.MainId);
            if (main == null)
            {
                result.Code = 500;
                result.Message = $"主记录未查询到";
                return result;
            }
            // 2、根据生产主记录查找任务主记录
            var orgMainRecord = await CreateOrGetRecordData(main.Id, dto.StationTaskId, dto.UserInputData?.UserScanName ?? "", dto.UserInputData?.CreateUserID);
            if (orgMainRecord.Status == StationTaskStatusEnum.已完成)
            {
                await trans.RollbackAsync();
                result.Code = 500;
                result.Message = "此任务已完成！";
                return result;
            }
            // 3、根据任务主记录查找任务
            var orgTaskBom = _dbContext.Proc_StationTask_UserInputs.FirstOrDefault(ps => ps.IsDeleted == false && ps.StationTask_RecordId == orgMainRecord.Id);
            if (orgTaskBom == null)
            {
                // 没有任务新增
                var newTaskBom = _dbContext.Proc_StationTask_UserInputs.Add(new Proc_StationTask_UserInput
                {
                    StationTask_RecordId = orgMainRecord.Id,
                    StationId = main.StationId,
                    StepId = main.StepId,
                    CreateUserID = userInput?.CreateUserID,
                    Status = StationTaskStatusEnum.已完成,
                    UserInputName = userInput?.UserScanName,
                    UserInputData = userInput!.ScanData!,
                    UpMesCode = userInput!.UpMesCode,
                    PackPN = userInput.PackPN,
                });
            }
            await _dbContext.SaveChangesAsync();
            await trans.CommitAsync();
        }
        catch (Exception ex)
        {
            await trans.RollbackAsync();
            result.Code = 500;
            result.Message = "保存失败！";
            _logger.LogError($"用户输入数据保存失败，参数为：{JsonConvert.SerializeObject(dto.UserInputData)}，异常消息为：{JsonConvert.SerializeObject(ex)}");
        }
        return result;
    }

    public async Task<Response> Save_StationScanCollect(ScanCollectDataDTO dto)
    {
        var result = new Response();
        using var trans = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var scanCollect = dto.ScanCollectData;
            var main = await GetMainData(dto.MainId);
            if (main == null)
            {
                result.Code = 500;
                result.Message = $"主记录未查询到";
                return result;
            }
            // 2、根据生产主记录查找任务主记录
            var orgMainRecord = await CreateOrGetRecordData(main.Id, dto.StationTaskId, dto.ScanCollectData?.ScanCollectName ?? "", dto.ScanCollectData?.CreateUserID);
            if (orgMainRecord.Status == StationTaskStatusEnum.已完成)
            {
                await trans.RollbackAsync();
                result.Code = 500;
                result.Message = "此任务已完成！";
                return result;
            }

            // 3、根据任务主记录查找任务
            var orgTaskBom = _dbContext.Proc_StationTask_ScanCollects.FirstOrDefault(ps => ps.IsDeleted == false && ps.StationTask_RecordId == orgMainRecord.Id);
            if (orgTaskBom == null)
            {
                // 没有任务新增
                _dbContext.Proc_StationTask_ScanCollects.Add(new Proc_StationTask_ScanCollect
                {
                    StationTask_RecordId = orgMainRecord.Id,
                    StationId = main.StationId,
                    StepId = main.StepId,
                    CreateTime = DateTime.Now,
                    CreateUserID = scanCollect!.CreateUserID,
                    Status = StationTaskStatusEnum.已完成,
                    ScanCollectData = scanCollect!.ScanInputData!,
                    ScanCollectName = scanCollect.ScanCollectName,
                    UpMesCode = scanCollect.UpMesCode,
                    PackPN = scanCollect.PackPN
                });
            }
            await _dbContext.SaveChangesAsync();
            await trans.CommitAsync();
        }
        catch (Exception ex)
        {
            result.Code = 500;
            result.Message = "保存失败！";
            _logger.LogError($"扫码收数数据保存失败，参数为：{JsonConvert.SerializeObject(dto.ScanCollectData)}，异常消息为：{JsonConvert.SerializeObject(ex)}");
        }
        return result;
    }


    public async Task<Response> SaveRecordTime(SaveTimeDTO dto)
    {
        var response = new Response();
        using var trans = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var main = await GetMainData(dto.MainId);
            if (main == null)
            {
                response.Code = 500;
                response.Message = $"主记录未查询到";
                return response;
            }
            // 2、根据生产主记录查找任务主记录
            var orgMainRecord = await CreateOrGetRecordData(main.Id, dto.StationTaskId, dto.TaskName ?? "", main.CreateUserID);
            if (orgMainRecord.Status == StationTaskStatusEnum.已完成)
            {
                await trans.RollbackAsync();
                response.Code = 500;
                response.Message = "此任务已完成！";
                return response;
            }

            //删除历史
            var his = await _dbContext.Proc_StationTask_TimeRecords.Where(w => !w.IsDeleted && w.Proc_StationTask_RecordId == orgMainRecord.Id).ToListAsync();
            his.ForEach(f => f.IsDeleted = true);
            _dbContext.UpdateRange(his);
            var newData = new Proc_StationTask_TimeRecord
            {
                SerialCode = dto.PackCode,
                StationCode = dto.StationCode,
                Proc_StationTask_RecordId = orgMainRecord.Id,
                TimeFlag = dto.TimeFlag,
                TimeValue = dto.RecordTimeStr,
                UploadMesCode = dto.UploadMesCode
            };

            await _dbContext.AddAsync(newData);
            await _dbContext.SaveChangesAsync();
            await trans.CommitAsync();
        }
        catch (Exception ex)
        {
            await trans.RollbackAsync();
            response.Code = 500;
            response.Message = ex.Message;
        }
        return response;
    }

    /// <summary>
    /// 保存单个螺丝拧紧任务详情
    /// </summary>
    /// <param name="screw"></param>
    /// <returns></returns>
    public async Task<Response> SaveScrewData(ScrewDataDTO dto)
    {
        var result = new Response();
        using var trans = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var screw = dto.ScrewData;
            if (screw == null)
            {
                result.Code = 500;
                result.Message = $"未提供拧紧数据";
                return result;
            }
            var main = await GetMainData(dto.MainId);
            if (main == null)
            {
                result.Code = 500;
                result.Message = $"主记录未查询到";
                return result;
            }
            // 2、根据生产主记录查找任务主记录
            var orgMainRecord = await CreateOrGetRecordData(main.Id, dto.StationTaskId, dto.ScrewData?.ScrewSpecs ?? "", dto.ScrewData?.CreateUserID);
            if (orgMainRecord.Status == StationTaskStatusEnum.已完成)
            {
                await trans.RollbackAsync();
                result.Code = 500;
                result.Message = "此任务已完成！";
                return result;
            }

            // 3、根据任务主记录查找拧紧任务主记录
            var orgTaskBlotGun = _dbContext.Proc_StationTask_BlotGuns.FirstOrDefault(ps => ps.IsDeleted == false && ps.StationTask_RecordId == orgMainRecord.Id && ps.Base_ScrewTaskId == screw.Id);
            if (orgTaskBlotGun == null)
            {
                // 没有拧紧任务主记录 新增
                var blotGun = new Proc_StationTask_BlotGun
                {
                    Base_ScrewTaskId = screw.Id,
                    StationTask_RecordId = orgMainRecord.Id,
                    CreateTime = DateTime.Now,
                    CreateUserID = screw.CreateUserID,
                    Status = StationTaskStatusEnum.进行中,
                    ScrewName = screw.ScrewSpecs,
                    UseNum = screw.UseNum,
                    ProgramNo = screw.ProgramNo,
                };

                await _dbContext.AddAsync(blotGun);
                await _dbContext.SaveChangesAsync();
                orgTaskBlotGun = blotGun;
            }

            // 拧紧任务进度+1
            if (screw.ResultIsOK)
                orgTaskBlotGun.CurCompleteNum += 1;

            if (orgTaskBlotGun.CurCompleteNum == orgTaskBlotGun.UseNum)
            {
                orgTaskBlotGun.Status = StationTaskStatusEnum.已完成;
            }

            var baseScrew = _dbContext.Base_StationTaskScrews.FirstOrDefault(s => s.IsDeleted == false && s.StationTaskId == dto.StationTaskId);
            if (baseScrew == null)
            {
                result.Code = 500;
                result.Message = "配方未查询到";
                return result;
            }
            if (baseScrew.UseNum > 1)
            {
                // 4、根据拧紧任务主记录 新增拧紧任务详情
                _dbContext.Proc_StationTask_BlotGunDetails.Add(new Proc_StationTask_BlotGunDetail
                {
                    Base_ProResourceId = screw.UseResourceId,
                    StationId = main.StationId,
                    StepId = main.StepId,
                    Proc_StationTask_BlotGun = orgTaskBlotGun,
                    CreateUserID = screw.CreateUserID,
                    Status = StationTaskStatusEnum.已完成,
                    ScrewName = screw.ScrewSpecs,
                    FinalTorque = screw.CurFinalTorque,
                    ResultIsOK = screw.ResultIsOK,
                    AngleStatus = screw.CurAngleStatus,
                    Angle_Max = screw.CurAngle_Max,
                    Angle_Min = screw.CurAngle_Min,
                    DeviceNo = screw.DeviceNo,
                    FinalAngle = screw.CurFinalAngle,
                    ProgramNo = (int)screw.ProgramNo,
                    TargetAngle = screw.CurTargetAngle,
                    TargetTorqueRate = screw.CurTargetTorqueRate,
                    TorqueRate_Max = screw.CurTorqueRate_Max,
                    TorqueRate_Min = screw.CurTorqueRate_Min,
                    TorqueStatus = screw.CurTorqueStatus,
                    OrderNo = screw.CurNo,
                    PackPN = screw.PackCode,
                    UploadCode = screw.UpMesCodePN + (screw.UpMESCodeStartNo + screw.CurNo),
                    UploadCode_JD = screw.UpMesCodePN + "JD" + (screw.UpMESCodeStartNo + screw.CurNo),
                });
            }
            else
            {
                //只有1颗螺丝，上传代码不带数字
                // 4、根据拧紧任务主记录 新增拧紧任务详情
                _dbContext.Proc_StationTask_BlotGunDetails.Add(new Proc_StationTask_BlotGunDetail
                {
                    Base_ProResourceId = screw.UseResourceId,
                    StationId = main.StationId,
                    StepId = main.StepId,
                    Proc_StationTask_BlotGun = orgTaskBlotGun,
                    CreateTime = DateTime.Now,
                    CreateUserID = screw.CreateUserID,
                    Status = StationTaskStatusEnum.已完成,
                    ScrewName = screw.ScrewSpecs,
                    FinalTorque = screw.CurFinalTorque,
                    ResultIsOK = screw.ResultIsOK,
                    AngleStatus = screw.CurAngleStatus,
                    Angle_Max = screw.CurAngle_Max,
                    Angle_Min = screw.CurAngle_Min,
                    DeviceNo = screw.DeviceNo,
                    FinalAngle = screw.CurFinalAngle,
                    ProgramNo = (int)screw.ProgramNo,
                    TargetAngle = screw.CurTargetAngle,
                    TargetTorqueRate = screw.CurTargetTorqueRate,
                    TorqueRate_Max = screw.CurTorqueRate_Max,
                    TorqueRate_Min = screw.CurTorqueRate_Min,
                    TorqueStatus = screw.CurTorqueStatus,
                    OrderNo = screw.CurNo,
                    PackPN = screw.PackCode,
                    UploadCode = screw.UpMesCodePN,
                    UploadCode_JD = screw.UpMesCodePN + "JD",
                });
            }

            await _dbContext.SaveChangesAsync();
            await trans.CommitAsync();

        }
        catch (Exception ex)
        {
            await trans.RollbackAsync();
            result.Code = 500;
            result.Message = "保存失败！";
            _logger.LogError("工位任务螺丝详情保存失败，参数为：{SerializeObject}，异常消息为：{O}", JsonConvert.SerializeObject(dto.ScrewData), JsonConvert.SerializeObject(ex));
        }
        return result;
    }

    public async Task<Response> SaveTightenByImageData(ScrewDataDTO dto)
    {
        var result = new Response();
        using var trans = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var main = await GetMainData(dto.MainId);
            if (main == null)
            {
                result.Code = 500;
                result.Message = $"主记录未查询到";
                return result;
            }
            // 2、根据生产主记录查找任务主记录
            var orgMainRecord = await CreateOrGetRecordData(main.Id, dto.StationTaskId, dto.TaskName ?? "", main.CreateUserID);
            if (orgMainRecord.Status == StationTaskStatusEnum.已完成)
            {
                await trans.RollbackAsync();
                result.Code = 500;
                result.Message = "此任务已完成！";
                return result;
            }

            _dbContext.Proc_StationTask_TightenByImages.Add(new Proc_StationTask_TightenByImage
            {
                Proc_StationTask_RecordId = orgMainRecord.Id,
                CreateTime = DateTime.Now,
                FinalTorque = dto.FinalTorque,
                FinalAngle = dto.FinalAngle,
                SerialCode = main.PackCode,
                StationCode = dto.StationCode ?? main.StationCode,
                OrderNo = dto.OrderNo,
                ResultIsOK = true,
                UpMesCodeTor = $"{dto.UploadMesCode}{dto.OrderNo}",
                UpMesCodeAng = $"{dto.UploadMesCode}JD{dto.OrderNo}",
            });

            await _dbContext.SaveChangesAsync();
            await trans.CommitAsync();
        }
        catch (Exception ex)
        {
            await trans.RollbackAsync();
            result.Code = 500;
            result.Message = "保存失败！";
            _logger.LogError($"工位任务螺丝详情保存失败，参数为：{JsonConvert.SerializeObject(dto.ScrewData)}，异常消息为：{JsonConvert.SerializeObject(ex)}");
        }
        return result;
    }


    /// <summary>
    /// 保存图示拧紧NG数据
    /// </summary>
    public async Task<Response> SaveNgData(ScrewDataDTO dto)
    {
        var result = new Response();
        using var trans = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var main = await GetMainData(dto.MainId);
            if (main == null)
            {
                result.Code = 500;
                result.Message = $"主记录未查询到";
                return result;
            }
            var orgMainRecord = await CreateOrGetRecordData(main.Id, dto.StationTaskId, dto.TaskName ?? "", main.CreateUserID);
            if (orgMainRecord.Status == StationTaskStatusEnum.已完成)
            {
                await trans.RollbackAsync();
                result.Code = 500;
                result.Message = "此任务已完成！";
                return result;
            }

            _dbContext.Proc_StationTask_TightenByImages.Add(new Proc_StationTask_TightenByImage
            {
                Proc_StationTask_RecordId = orgMainRecord.Id,
                CreateTime = DateTime.Now,
                FinalTorque = dto.FinalTorque,
                FinalAngle = dto.FinalAngle,
                SerialCode = main.PackCode,
                StationCode = dto.StationCode ?? main.StationCode,
                OrderNo = dto.OrderNo,
                ResultIsOK = false,
                UpMesCodeTor = $"{dto.UploadMesCode}{dto.OrderNo}",
                UpMesCodeAng = $"{dto.UploadMesCode}JD{dto.OrderNo}",
            });

            await _dbContext.SaveChangesAsync();
            await trans.CommitAsync();
        }
        catch (Exception ex)
        {
            await trans.RollbackAsync();
            result.Code = 500;
            result.Message = "保存失败！";
            _logger.LogError($"图示拧紧NG数据保存失败，参数为：{JsonConvert.SerializeObject(dto)}，异常：{ex.Message}");
        }
        return result;
    }

    /// <summary>
    /// 保存图示拧紧反拧数据
    /// </summary>
    public async Task<Response> SaveReverseData(ScrewDataDTO dto)
    {
        var result = new Response();
        using var trans = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var main = await GetMainData(dto.MainId);
            if (main == null)
            {
                result.Code = 500;
                result.Message = $"主记录未查询到";
                return result;
            }
            var orgMainRecord = await CreateOrGetRecordData(main.Id, dto.StationTaskId, dto.TaskName ?? "", main.CreateUserID);
            if (orgMainRecord.Status == StationTaskStatusEnum.已完成)
            {
                await trans.RollbackAsync();
                result.Code = 500;
                result.Message = "此任务已完成！";
                return result;
            }

            _dbContext.Proc_StationTask_TightenByImages.Add(new Proc_StationTask_TightenByImage
            {
                Proc_StationTask_RecordId = orgMainRecord.Id,
                CreateTime = DateTime.Now,
                FinalTorque = dto.FinalTorque,
                FinalAngle = dto.FinalAngle,
                SerialCode = main.PackCode,
                StationCode = dto.StationCode ?? main.StationCode,
                OrderNo = dto.OrderNo,
                ResultIsOK = true,
                UpMesCodeTor = $"{dto.UploadMesCode}{dto.OrderNo}",
                UpMesCodeAng = $"{dto.UploadMesCode}JD{dto.OrderNo}",
            });

            await _dbContext.SaveChangesAsync();
            await trans.CommitAsync();
        }
        catch (Exception ex)
        {
            await trans.RollbackAsync();
            result.Code = 500;
            result.Message = "保存失败！";
            _logger.LogError($"图示拧紧反拧数据保存失败，参数为：{JsonConvert.SerializeObject(dto)}，异常：{ex.Message}");
        }
        return result;
    }

    public async Task<Proc_StationTask_Main?> GetMainData(int mainId)
    {
        return await _dbContext.Proc_StationTask_Mains.FirstOrDefaultAsync(f => !f.IsDeleted && f.Id == mainId);
    }

    public async Task<Proc_StationTask_Record> CreateOrGetRecordData(int mainId, int TsakId, string TaskName, int? UserId)
    {
        var record = await _dbContext.Proc_StationTask_Records.Include(s => s.Base_StationTask).FirstOrDefaultAsync(ps => ps.IsDeleted == false && ps.Proc_StationTask_MainId == mainId && ps.Base_StationTaskId == TsakId);
        if (record != null)
        {
            return record;
        }

        var newRecord = new Proc_StationTask_Record
        {
            Proc_StationTask_MainId = mainId,
            Base_StationTaskId = TsakId,
            CreateUserID = UserId,
            Status = StationTaskStatusEnum.进行中,
            TaskName = TaskName
        };
        // 没有任务主记录 新增
        await _dbContext.AddAsync(newRecord);
        await _dbContext.SaveChangesAsync();
        return newRecord;
    }

    /// <summary>
    /// 设置当前拧紧任务完成
    /// </summary>
    /// <param name="screw"></param>
    /// <returns></returns>
    public async Task<Response> SetScrewTaskFinish(Base_StationTaskScrew screw, int mainId)
    {
        var result = new Response();
        using var trans = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var pStationTaskMain = await GetMainData(mainId);
            if (pStationTaskMain == null)
            {
                result.Code = 500;
                result.Message = $"主记录未查询到";
                return result;
            }

            var orgMainRecord = await _dbContext.Proc_StationTask_Records.FirstOrDefaultAsync(f => f.Proc_StationTask_MainId == mainId && f.Base_StationTaskId == screw.StationTaskId && !f.IsDeleted);
            if (orgMainRecord == null)
            {
                result.Code = 500;
                result.Message = $"任务记录未查询到";
                return result;
            }
            if (orgMainRecord.Status == StationTaskStatusEnum.已完成)
            {
                result.Code = 500;
                result.Message = $"当前任务{orgMainRecord.TaskName}已完成";
                return result;
            }

            var orgTaskBolt = _dbContext.Proc_StationTask_BlotGuns.FirstOrDefault(ps => ps.IsDeleted == false && ps.StationTask_RecordId == orgMainRecord.Id && ps.Base_ScrewTaskId == screw.Id);
            if (orgTaskBolt == null)
            {
                result.Code = 500;
                result.Message = $"当前拧紧任务记录不存在无法设置拧紧完成";
                return result;
            }
            orgTaskBolt.Status = StationTaskStatusEnum.已完成;
            orgTaskBolt.UpdateTime = DateTime.Now;
            _dbContext.Update(orgTaskBolt);
            await _dbContext.SaveChangesAsync();
            await trans.CommitAsync();
        }
        catch (Exception ex)
        {
            await trans.RollbackAsync();
            result.Code = 500;
            result.Message = "保存失败！";
            _logger.LogError($"工位任务螺丝详情保存失败，参数为：{JsonConvert.SerializeObject(screw)}，异常消息为：{JsonConvert.SerializeObject(ex)}");
        }
        return result;
    }

    /// <summary>
    /// 设置当前工位任务状态为完成
    /// </summary>
    /// <param name="taskDTO"></param>
    /// <param name="mainId"></param>
    /// <returns></returns>
    public async Task<Response> SetStationCurTaskFinish(StationTaskDTO taskDTO, int mainId)
    {
        var result = new Response();
        using var dbTras = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var main = await GetMainData(mainId);
            if (main == null)
            {
                result.Code = 500;
                result.Message = "设置任务完成时，未查询到工位任务主记录";
                return result;
            }
            var taskRecords = await _dbContext.Proc_StationTask_Records.Where(t => t.Proc_StationTask_MainId == main.Id && !t.IsDeleted).ToListAsync();
            var taskRecord = taskRecords.FirstOrDefault(t => t.Base_StationTaskId == taskDTO.StationTaskId);
            if (taskRecord == null)
            {
                taskRecord = await CreateOrGetRecordData(main.Id, taskDTO.StationTask!.Id, taskDTO.StationTask.Name!, main.CreateUserID);
            }
            if (taskRecord.Status == StationTaskStatusEnum.已完成)
            {
                result.Code = 200;
                result.Message = $"当前任务【{taskRecord.TaskName}】已完成，请继续作业";
                return result;
            }
            taskRecord.Status = StationTaskStatusEnum.已完成;
            taskRecord.UpdateTime = DateTime.Now;
            _dbContext.Update(taskRecord);
            main.CurStepNo = taskRecords.Count + 1;
            main.UpdateTime = DateTime.Now;
            _dbContext.Update(main);
            await _dbContext.SaveChangesAsync();
            await dbTras.CommitAsync();
        }
        catch (Exception ex)
        {
            await dbTras.RollbackAsync();
            result.Code = 500;
            result.Message = "保存失败！";
            _logger.LogError($"工位任务状态保存失败，参数为：{JsonConvert.SerializeObject(taskDTO)}，异常消息为：{JsonConvert.SerializeObject(ex)}");
        }
        return result;
    }

    /// <summary>
    /// 放行AGV任务
    /// </summary>
    /// <param name="taskDTO"></param>
    /// <returns></returns>
    public async Task<Response> SetStationCurTaskRunAGV(StationTaskDTO taskDTO, int mainId)
    {
        var result = new Response();
        using var trans = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var main = await GetMainData(mainId);
            if (main == null)
            {
                result.Code = 500;
                result.Message = "设置任务完成时，未查询到工位任务主记录";
                return result;
            }
            var taskRecords = await _dbContext.Proc_StationTask_Records.Where(t => t.Proc_StationTask_MainId == main.Id && !t.IsDeleted).ToListAsync();
            var taskRecord = taskRecords.FirstOrDefault(t => t.Base_StationTaskId == taskDTO.StationTaskId);
            if (taskRecord == null)
            {
                var newRecord = new Proc_StationTask_Record
                {
                    Proc_StationTask_MainId = main.Id,
                    TaskName = "放行",
                    Base_StationTaskId = taskDTO.StationTaskId,
                    Status = StationTaskStatusEnum.进行中,
                    CreateUserID = taskDTO.CreateUserID,
                    CreateTime = DateTime.Now,
                };
                await _dbContext.AddAsync(newRecord);
                await _dbContext.SaveChangesAsync();
                taskRecord = newRecord;
            }
            taskRecord.Status = StationTaskStatusEnum.已完成;
            taskRecord.UpdateTime = DateTime.Now;
            taskRecord.UpdateUserID = taskDTO.CreateUserID;
            _dbContext.Update(taskRecord);
            main.Status = StationTaskStatusEnum.已完成;
            main.Timespan = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            _dbContext.Update(main);
            await _dbContext.SaveChangesAsync();

            await SaveToOffline(main);//保存下线记录

            await trans.CommitAsync();
        }
        catch (Exception ex)
        {
            result.Code = 500;
            result.Message = "保存失败！";
            _logger.LogError($"工位任务状态保存失败，参数为：{JsonConvert.SerializeObject(taskDTO)}，异常消息为：{JsonConvert.SerializeObject(ex)}");
        }
        return result;
    }

    private async Task SaveToOffline(Proc_StationTask_Main main)
    {
        var flow = await _dbContext.Base_Flows.FirstOrDefaultAsync(f => !f.IsDeleted && f.ProductId == main.ProductId);
        if (flow == null) return;

        var lastStep = await _dbContext.Base_FlowStepMappings.Where(w => !w.IsDeleted && w.FlowId == flow.Id).OrderBy(o => o.OrderNo).LastOrDefaultAsync();
        if (lastStep == null) return;

        if (main.StepId != lastStep.StepId)
        {
            return;
        }
        var newOffline = new Proc_Product_Offline
        {
            ProductCode = main.PackCode,
            InstorageTime = DateTime.Now,
            StationId = main.StationId,
            State = Proc_ProductStates.正常下线,
            Productid = main.ProductId,
            StepId = lastStep.StepId,
            CreateUserID = main.CreateUserID,
        };
        await _dbContext.AddAsync(newOffline);
        await _dbContext.SaveChangesAsync();
    }


    /// <summary>
    /// 加载工位生产资源配置
    /// </summary>
    /// <param name="stationCode">工位号</param>
    /// <param name="proResourceType">生产资源类型</param>
    /// <returns></returns>
    public async Task<Response<List<Base_ProResource>>> LoadStationProResourceConfig(string stationCode, int proResourceType)
    {
        var result = new Response<List<Base_ProResource>>();

        var resourceList = await _dbContext.Base_ProResources
            .Where(r => r.StationCode == stationCode && (int)r.ProResourceType == proResourceType && r.IsDeleted == false)
            .ToListAsync();
        result.Code = 200;
        result.Message = "读取成功";
        result.Result = resourceList;
        return result;
    }

    /// <summary>
    /// 保存电子秤配置
    /// </summary>
    /// <param name="listRes"></param>
    /// <returns></returns>
    public async Task<Response<List<Base_ProResource>>> SaveStationEquipmentConifgData(List<Base_ProResource> listRes, ProResourceTypeEnum resourceType, string stationCode, string Operator)
    {
        var result = new Response<List<Base_ProResource>>();

        var user = _dbContext.Users.FirstOrDefault(u => u.Account == Operator && !u.IsDeleted);
        try
        {
            var Resource = _dbContext.Base_ProResources.Where(p => p.StationCode == stationCode && p.ProResourceType == resourceType && !p.IsDeleted);
            if (listRes.Count == 0)
            {

                foreach (var r in Resource)
                {
                    r.IsDeleted = true;
                    await sysLogService.AddLog(new SysLog { LogType = Sys_LogType.删除生产资源, Message = $"删除生产资源: {r.Name}", Operator = user?.Name });
                }
                _dbContext.Base_ProResources.UpdateRange(Resource);
            }
            else
            {
                bool add = true;
                foreach (var res in listRes)
                {
                    add = true;
                    foreach (var exiestRes in Resource)
                    {
                        if (exiestRes.Name == res.Name)
                        {
                            add = false;

                            if (exiestRes.IpAddress != res.IpAddress)
                            {
                                exiestRes.IpAddress = res.IpAddress;
                                await sysLogService.AddLog(new SysLog { LogType = Sys_LogType.修改生产资源, Message = $"修改生产资源: {res.Name}的IP修改为{res.IpAddress}", Operator = user?.Name });
                            }

                            if (exiestRes.Port != res.Port)
                            {
                                exiestRes.Port = res.Port;
                                await sysLogService.AddLog(new SysLog { LogType = Sys_LogType.修改生产资源, Message = $"修改生产资源: {res.Name}的端口修改为{res.Port}", Operator = user?.Name });
                            }

                            if (exiestRes.Baud != res.Baud)
                            {
                                exiestRes.Baud = res.Baud;
                                await sysLogService.AddLog(new SysLog { LogType = Sys_LogType.修改生产资源, Message = $"修改生产资源: {res.Name}的波特率修改为{res.Baud}", Operator = user?.Name });
                            }

                            if (exiestRes.IsEnable != res.IsEnable)
                            {
                                exiestRes.IsEnable = res.IsEnable;
                                await sysLogService.AddLog(new SysLog { LogType = Sys_LogType.修改生产资源, Message = $"修改生产资源: {res.Name}{(res.IsEnable ? "启用" : "禁用")}", Operator = user?.Name });

                            }

                            _dbContext.Base_ProResources.Update(exiestRes);

                            break;
                        }
                    }
                    if (!add)
                    {
                        continue;
                    }
                    else
                    {
                        _dbContext.Base_ProResources.Add(res);
                        await sysLogService.AddLog(new SysLog { LogType = Sys_LogType.新增生产资源, Message = $"新增生产资源: {res.Name}", Operator = user?.Name });
                    }
                }
                bool b = false;
                foreach (var exiestRes in Resource)
                {
                    b = false;

                    foreach (var res in listRes)
                    {
                        if (exiestRes.Name == res.Name)
                        {
                            b = true;
                            break;
                        }
                    }
                    if (b)
                    {
                        continue;
                    }
                    else
                    {
                        exiestRes.IsDeleted = true;
                        _dbContext.Base_ProResources.Update(exiestRes);
                        await sysLogService.AddLog(new SysLog { LogType = Sys_LogType.删除生产资源, Message = $"删除生产资源: {exiestRes.Name}", Operator = user?.Name });
                    }
                }
            }
            _dbContext.SaveChanges();

            var resourceList = _dbContext.Base_ProResources.Where(r => r.StationCode == stationCode && r.ProResourceType == resourceType && r.IsDeleted == false).ToList();

            result.Code = 200;
            result.Message = "读取成功";
            result.Result = resourceList;
        }
        catch (Exception ex)
        {
            result.Code = 500;
            result.Message = "保存失败！";
            _logger.LogError($"工位{resourceType}配置失败，异常消息为：{JsonConvert.SerializeObject(listRes)}");
        }

        return result;
    }

    public async Task<Response> SaveStationTaskCheckTimeOut(CheckTimeOutDataDTO dto)
    {
        var result = new Response();
        using var trans = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var proc = dto.CheckTimeOutData;
            var main = await GetMainData(dto.MainId);
            if (main == null)
            {
                result.Code = 500;
                result.Message = $"主记录未查询到";
                return result;
            }
            // 2、根据生产主记录查找任务主记录
            var orgMainRecord = await CreateOrGetRecordData(main.Id, dto.StationTaskId, dto.CheckTimeOutData?.TimeOutTaskName ?? "", dto.CheckTimeOutData?.CreateUserID);
            if (orgMainRecord.Status == StationTaskStatusEnum.已完成)
            {
                await trans.RollbackAsync();
                result.Code = 500;
                result.Message = "此任务已完成！";
                return result;
            }

            // 3、根据任务主记录查找任务
            var orgTaskBom = _dbContext.Proc_StationTask_CheckTimeouts.FirstOrDefault(ps => ps.IsDeleted == false && (ps.Status == StationTaskStatusEnum.未开始 || ps.Status == StationTaskStatusEnum.进行中) && ps.StationTask_Record == orgMainRecord);
            if (orgTaskBom == null)
            {
                // 没有任务新增
                await _dbContext.Proc_StationTask_CheckTimeouts.AddAsync(new Proc_StationTask_CheckTimeout
                {
                    StationTaskId = dto.StationTaskId,
                    StepId = main.StepId,
                    StationTask_RecordId = orgMainRecord.Id,
                    CreateTime = DateTime.Now,
                    CreateUserID = dto.CheckTimeOutData?.CreateUserID,
                    Status = StationTaskStatusEnum.已完成,
                    TimeName = proc.TimeOutTaskName,
                    Pass = proc.HasPassed,
                    Time = proc.RealDuration,
                    StartTime = proc.StartTime,
                    CollectTime = proc.CheckTime,
                    UpMesCode = proc.UpMesCode,
                    PackPN = proc.PackCode,
                    StationId = main.StationId,
                });
            }
            await _dbContext.SaveChangesAsync();
            await trans.CommitAsync();
        }
        catch (Exception ex)
        {
            result.Code = 500;
            result.Message = ex.Message;
        }
        return result;
    }

    public async Task<Response> SetStationTaskStewingTime(StewingTimeDataDTO procDTO)
    {
        var result = new Response();

        try
        {
            var proc = procDTO.StewingTimeData;
            var stepId = proc.StepId;
            var stationId = proc.StationId;

            // 1、 查找生产主记录
            var pStationTaskMain = _dbContext.Proc_StationTask_Mains.FirstOrDefault(ps => ps.IsDeleted == false && ps.StepId == stepId && ps.PackCode == proc.PackCode && (ps.Status == StationTaskStatusEnum.未开始 || ps.Status == StationTaskStatusEnum.进行中));
            if (pStationTaskMain == null)
            {
                DateTimeOffset utcTime = DateTime.Now;
                var peiFangID = await _PeiFangService.GetOrCreateNewPeiFangJson_MD5(JsonConvert.SerializeObject(procDTO.StationTaskList));

                // 没有主记录 新增主记录
                var newMain = _dbContext.Proc_StationTask_Mains.Add(new Proc_StationTask_Main
                {
                    PeiFang_MD5_ID = peiFangID,
                    StepId = (int)stepId,
                    StationId = (int)stationId,
                    Status = StationTaskStatusEnum.进行中,
                    CurStepNo = proc.StationTask.Sequence,
                    UseAGVCode = proc.AGVCode,
                    CreateUserID = proc.CreateUserID,
                    CreateTime = DateTime.Now,
                    UpdateTime = DateTime.Now,
                    PackCode = proc.PackCode,
                    Timespan = utcTime.ToUnixTimeMilliseconds()
                });
                pStationTaskMain = newMain.Entity;
            }

            // 2、根据生产主记录查找任务主记录
            //var orgMainRecord = _dbContext.Proc_StationTask_Records.Include(s => s.Base_StationTask).FirstOrDefault(ps => ps.IsDeleted == false && (ps.Status == StationTaskStatusEnum.未开始 || ps.Status == StationTaskStatusEnum.进行中) && ps.Proc_StationTask_Main == pStationTaskMain && ps.Base_StationTask.Type == StationTaskTypeEnum.工序时间差);
            //if (orgMainRecord == null)
            //{
            //    // 没有任务主记录 新增
            //    var newMainRecord = _dbContext.Proc_StationTask_Records.Add(new Proc_StationTask_Record
            //    {
            //        Proc_StationTask_Main = pStationTaskMain,
            //        Base_StationTaskId = proc.StationTaskId,
            //        CreateTime = DateTime.Now,
            //        CreateUserID = proc.CreateUserID,
            //        Status = StationTaskStatusEnum.进行中,
            //        TaskName = proc.StationTask.Name
            //    });
            //    orgMainRecord = newMainRecord.Entity;
            //}

            // 3、根据任务主记录查找任务
            //var orgTaskBom = _dbContext.Proc_StationTask_StewingTimes.FirstOrDefault(ps => ps.IsDeleted == false && (ps.Status == StationTaskStatusEnum.未开始 || ps.Status == StationTaskStatusEnum.进行中) && ps.StationTask_Record == orgMainRecord);
            //if (orgTaskBom == null)
            //{
            //    // 没有任务新增
            //    var newTaskBom = _dbContext.Proc_StationTask_StewingTimes.Add(new Proc_StationTask_StewingTime
            //    {
            //        StationTaskId = proc.StationTaskId,
            //        StepId = stepId,
            //        StationTask_Record = orgMainRecord,
            //        CreateTime = DateTime.Now,
            //        CreateUserID = proc.CreateUserID,
            //        Status = StationTaskStatusEnum.已完成,
            //        StewingTimeName = proc.TaskStewingTimedName,
            //        Pass = proc.HasPassed,
            //        StewingTime = proc.StewingTime,
            //        StewingStartTime = proc.StewingStartTime,
            //        StewingCollectTime = proc.StewingCollectTime,
            //        UpMesCode = proc.UpMesCode,
            //        PackPN = proc.PackCode,
            //        StationId = stationId,
            //    });
            //    orgTaskBom = newTaskBom.Entity;
            //}
            _dbContext.SaveChanges();
        }
        catch (Exception ex)
        {
            result.Code = 500;
            result.Message = ex.Message;
        }
        return result;
    }


    public async Task<Response<Proc_StationTask_TimeRecord>> GetRecordTime(string packCode, string TimeFlag)
    {
        var response = new Response<Proc_StationTask_TimeRecord>();
        try
        {
            var result = await _dbContext.Proc_StationTask_TimeRecords.Where(w => !w.IsDeleted && w.TimeFlag == TimeFlag && w.SerialCode == packCode).FirstOrDefaultAsync(); //、理论上一个Pack可以有多个时间标志，但一个时间标志只能代表一个时间，如果有多个时间，则默认第一个
            if (result == null)
            {
                response.Code = 500;
                response.Message = $"未找到PACK-{packCode}的 {TimeFlag}的记录";
                return response;
            }
            response.Result = result;
        }
        catch (Exception ex)
        {
            response.Code = 500;
            response.Message = ex.Message;
        }
        return response;

    }

    /// <summary>
    /// 创建或加载人工位的工作记录
    /// </summary>
    /// <param name="packCode">Pack码</param>
    /// <param name="stationCode">工位编码</param>
    /// <param name="productCode">产品码</param>
    /// <returns></returns>
    public async Task<Response<StationTaskHistoryDTO>> CreateOrLoadTraceInfo(string packCode, string stationCode, string productCode)
    {
        var response = new Response<StationTaskHistoryDTO>();
        try
        {
            var dto = new StationTaskHistoryDTO();
            var product = await _dbContext.Products.FirstOrDefaultAsync(f => !f.IsDeleted && f.Code == productCode);
            if (product == null)
            {
                response.Code = 500;
                response.Message = $"产品{productCode}不存在";
                return response;
            }

            var station = await _dbContext.Base_Stations.FirstOrDefaultAsync(f => f.Code == stationCode && !f.IsDeleted);
            if (station == null)
            {
                response.Code = 500;
                response.Message = $"工位{stationCode}不存在";
                return response;
            }

            var main = await _dbContext.Proc_StationTask_Mains.FirstOrDefaultAsync(f => !f.IsDeleted && f.PackCode == packCode && f.StepId == station.StepId);
            // 首次作业，创建任务主记录
            if (main == null)
            {
                main = new Proc_StationTask_Main
                {
                    StationCode = stationCode,
                    StationId = station.Id,
                    StepId = station.StepId,
                    PackCode = packCode,
                    Status = StationTaskStatusEnum.进行中,
                    ProductId = product.Id,
                };
                await _dbContext.AddAsync(main);
                await _dbContext.SaveChangesAsync();
                dto.StationTaskMain = main;
                dto.IsNew = true;
                response.Result = dto;
                return response;
            }

            // 非首次作业，加载任务记录(详情)
            dto.StationTaskMain = main;
            dto.IsNew = false;

            var Records = await _dbContext.Proc_StationTask_Records.Where(w => !w.IsDeleted && w.Proc_StationTask_MainId == main.Id).Include(i => i.Base_StationTask).ToListAsync();
            var recordList = new List<RecordHistoryDTO>();
            foreach (var item in Records)
            {
                var record = new RecordHistoryDTO
                {
                    StationTaskRecord = item
                };
                var stationTask = item.Base_StationTask;
                if (stationTask == null) continue;
                switch (stationTask.Type)
                {
                    case StationTaskTypeEnum.扫描员工卡:
                        record.ScanAccountCards = await _dbContext.Proc_StationTask_ScanAccountCards.FirstOrDefaultAsync(f => f.StationTask_RecordId == item.Id && !f.IsDeleted);
                        break;
                    case StationTaskTypeEnum.扫码:
                        var bomDtos = new List<BomHistoryDTO>();
                        var boms = await _dbContext.Proc_StationTask_Boms.Where(w => !w.IsDeleted && w.StationTask_RecordId == item.Id).ToListAsync();
                        foreach (var bom in boms)
                        {
                            bomDtos.Add(new BomHistoryDTO
                            {
                                StationTaskBom = bom,
                                BomDetails = await _dbContext.Proc_StationTask_BomDetails.Where(w => !w.IsDeleted && w.Proc_StationTask_BomId == bom.Id).ToListAsync(),
                            });
                        }
                        record.Boms = bomDtos;
                        break;
                    case StationTaskTypeEnum.人工拧螺丝:
                        var screwDtos = new List<ScrewHistoryDTO>();
                        var screws = await _dbContext.Proc_StationTask_BlotGuns.Where(w => !w.IsDeleted && w.StationTask_RecordId == item.Id).ToListAsync();
                        foreach (var screw in screws)
                        {
                            screwDtos.Add(new ScrewHistoryDTO
                            {
                                StationTaskScrew = screw,
                                ScrewDetails = await _dbContext.Proc_StationTask_BlotGunDetails.Where(w => !w.IsDeleted && w.Proc_StationTask_BlotGunId == screw.Id).ToListAsync(),
                            });
                        }
                        record.Screws = screwDtos;
                        break;
                    case StationTaskTypeEnum.扫码输入:
                        record.ScanCollects = await _dbContext.Proc_StationTask_ScanCollects.FirstOrDefaultAsync(f => f.StationTask_RecordId == item.Id && !f.IsDeleted);
                        break;
                    case StationTaskTypeEnum.用户输入:
                        record.UserInputs = await _dbContext.Proc_StationTask_UserInputs.FirstOrDefaultAsync(f => f.StationTask_RecordId == item.Id && !f.IsDeleted);
                        break;
                    case StationTaskTypeEnum.超时检测:
                        record.CheckTimeouts = await _dbContext.Proc_StationTask_CheckTimeouts.FirstOrDefaultAsync(f => f.StationTask_RecordId == item.Id && !f.IsDeleted);
                        break;
                    case StationTaskTypeEnum.时间记录:
                        record.TimeRecords = await _dbContext.Proc_StationTask_TimeRecords.FirstOrDefaultAsync(f => f.Proc_StationTask_RecordId == item.Id && !f.IsDeleted);
                        break;
                    case StationTaskTypeEnum.称重:
                        record.AnyLoads = await _dbContext.Proc_StationTask_AnyLoads.FirstOrDefaultAsync(f => f.StationTask_RecordId == item.Id && !f.IsDeleted);
                        break;
                    case StationTaskTypeEnum.补拧:
                        record.TightenReworks = await _dbContext.Proc_StationTask_TightenReworks.Where(f => f.StationTaskRecordId == item.Id && !f.IsDeleted).ToListAsync();
                        break;
                    case StationTaskTypeEnum.图示拧紧:
                        record.TightenByImages = await _dbContext.Proc_StationTask_TightenByImages
                            .Where(f => f.Proc_StationTask_RecordId == item.Id && !f.IsDeleted && f.OrderNo.HasValue)
                            .OrderByDescending(o => o.CreateTime)
                            .ThenByDescending(o => o.Id)
                            .ToListAsync();
                        break;
                }
                recordList.Add(record);
            }

            dto.StationTaskRecords = recordList;
            response.Result = dto;
            return response;
        }
        catch (Exception ex)
        {
            response.Code = 500;
            response.Message = ex.Message;
            return response;
        }
    }


    /// <summary>
    /// 加载自动拧紧站的数据
    /// </summary>
    /// <param name="packCode"></param>
    /// <param name="autoTightenType"></param>
    /// <param name="screwNum"></param>
    /// <returns></returns>
    public async Task<Response<IList<AutoBlotInfo>>> LoadAutoTightenData(string packCode, TightenReworkType autoTightenType, int screwNum)
    {
        var result = new Response<IList<AutoBlotInfo>>();

        try
        {
            var boltDataList = new List<AutoBlotInfo>();

            var external = await _dbContext.Proc_ExternalAutoTightenDatas
                .Where(w => !w.IsDeleted && w.Sfc == packCode && w.TightenType == autoTightenType)
                .OrderByDescending(w => w.UpdateTime ?? w.CreateTime)
                .FirstOrDefaultAsync();

            if (external != null)
            {
                var tighteningResults =
                    JsonConvert.DeserializeObject<List<TighteningResult>>(external.TighteningResultJson) ??
                    new List<TighteningResult>();

                foreach (var item in tighteningResults.OrderBy(o => o.OrderNo))
                {
                    if (!TryParseDecimal(item.TorqueResult?.TagValue, out var torque))
                    {
                        result.Code = 500;
                        result.Message = $"Torque TagValue无法解析, OrderNo={item.OrderNo}";
                        return result;
                    }

                    if (!TryParseDecimal(item.AngleResult?.TagValue, out var angle))
                    {
                        result.Code = 500;
                        result.Message = $"Angle TagValue无法解析, OrderNo={item.OrderNo}";
                        return result;
                    }

                    boltDataList.Add(new AutoBlotInfo
                    {
                        ResultIsOK = item.ResultOK == 1,
                        ProgramNo = item.ProgramNo,
                        FinalTorque = torque,
                        FinalAngle = angle,
                        OrderNo = item.OrderNo
                    });
                }
            }
            else
            {
                // 兼容老的自动站程序：如果新的表里没有数据则回退至旧表查询
                var oldDataList = await _dbContext.Proc_AutoBoltInfo_Details
                    .Where(w => !w.IsDeleted && w.PackPN == packCode && w.BoltType == autoTightenType.ToString())
                    .ToListAsync();

                foreach (var item in oldDataList)
                {
                    boltDataList.AddRange(item.AutoBlotInfoArray ?? new List<AutoBlotInfo>());
                }
            }

            if (boltDataList.Count == 0)
            {
                result.Code = 500;
                result.Message = $"没有找到螺栓数据，请检查Pack码{packCode}是否正确或自动拧紧数据已经上传！";
            }

            if (boltDataList.Count != screwNum)
            {
                result.Code = 500;
                result.Message = $"螺栓数量不匹配，需求数量:{screwNum},实际数量:{boltDataList.Count},请校验自动工位提供的数量是否正确，或补拧配方是否正确！";
            }
            result.Result = boltDataList;
        }
        catch (Exception ex)
        {
            result.Code = 500;
            result.Message = ex.Message;
        }
        return result;
    }

    private static bool TryParseDecimal(string? raw, out decimal value)
    {
        value = 0m;
        if (string.IsNullOrWhiteSpace(raw))
        {
            return false;
        }
        var trimmed = raw.Trim();
        if (decimal.TryParse(trimmed, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
        {
            return true;
        }
        return decimal.TryParse(trimmed, NumberStyles.Any, CultureInfo.CurrentCulture, out value);
    }

    public async Task<Response> SaveRepairData(TightenReworkDataDto dto)
    {
        var response = new Response();
        await using var trans = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var main = await GetMainData(dto.MainId);
            if (main == null)
            {
                response.Code = 500;
                response.Message = $"生产主记录不存在";
                return response;
            }

            var record = await CreateOrGetRecordData(dto.MainId, dto.StationTaskId, dto.TaskName, main.CreateUserID);
            if (record.Status == StationTaskStatusEnum.已完成)
            {
                await trans.RollbackAsync();
                response.Code = 500;
                response.Message = $"此任务已完成";
                return response;
            }

            var data = new Proc_StationTask_TightenRework
            {
                StationTaskRecordId = record.Id,
                OrderNo = dto.OrderNo,
                ResultOk = dto.ResultOk,
                ProgramNo = dto.ProgramNo,
                TorqueValue = dto.TorqueValue,
                AngleValue = dto.AngleValue,
                TorqueMin = dto.TorqueMin,
                TorqueMax = dto.TorqueMax,
                AngleMin = dto.AngleMin,
                AngleMax = dto.AngleMax,
                UpMesCode = dto.UpMesCode,
                UpMesCodeJD = dto.UpMesCodeJD,
                CreateUserID = dto.Operator,
                PackPn = main.PackCode
            };
            await _dbContext.AddAsync(data);
            await _dbContext.SaveChangesAsync();
            await trans.CommitAsync();
        }
        catch (Exception ex)
        {
            await trans.RollbackAsync();
            response.Code = 500;
            response.Message = $"{ex.Message}";
        }

        return response;
    }

    public async Task<Response> MakeTaskComplateForce(StationTaskDTO dto, int mainId)
    {
        var result = new Response();
        try
        {
            var main = await GetMainData(mainId);
            if (main == null)
            {
                result.Code = 500;
                result.Message = $"生产主记录不存在";
                return result;
            }
            var record = await CreateOrGetRecordData(mainId, dto.StationTaskId, dto.StationTask?.Name ?? "", main.CreateUserID);
            record.Status = StationTaskStatusEnum.已完成;
            _dbContext.Update(record);
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            result.Code = 500;
            result.Message = ex.Message;
        }
        return result;
    }


    public async Task<Response> MakeTasksComplateForce(IList<(int, string)> Tasks, int mainId)
    {
        var result = new Response();
        try
        {
            var main = await GetMainData(mainId);
            if (main == null)
            {
                result.Code = 500;
                result.Message = $"生产主记录不存在";
                return result;
            }
            foreach (var item in Tasks)
            {
                var record = await CreateOrGetRecordData(mainId, item.Item1, item.Item2 ?? "", main.CreateUserID);
                record.Status = StationTaskStatusEnum.已完成;
                _dbContext.Update(record);
            }

            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            result.Code = 500;
            result.Message = ex.Message;
        }
        return result;
    }

    public async Task<Response> MakeRework(List<WorkRecord> works)
    {
        var result = new Response();
        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        int MainId = 0;
        try
        {
            foreach (var item in works)
            {
                switch (item.StationTaskType)
                {
                    case StationTaskTypeEnum.扫码:
                        var bom = await _dbContext.Proc_StationTask_Boms.Include(i => i.StationTask_Record).Where(w => !w.IsDeleted && w.Id == item.Id).FirstOrDefaultAsync();
                        if (bom == null) continue;

                        bom.Status = StationTaskStatusEnum.进行中;
                        if (item.IsNeedRemoveAll)
                        {
                            var details = await _dbContext.Proc_StationTask_BomDetails.Where(w => w.Proc_StationTask_BomId == bom.Id).ToListAsync();
                            foreach (var detail in details)
                            {
                                detail.IsDeleted = true;
                            }
                            _dbContext.UpdateRange(details);
                        }
                        _dbContext.Update(bom);
                        MainId = await SetRecordWorking(bom.StationTask_RecordId);
                        break;
                    case StationTaskTypeEnum.人工拧螺丝:
                        var screw = await _dbContext.Proc_StationTask_BlotGuns.Include(i => i.StationTask_Record).Where(w => !w.IsDeleted && w.Id == item.Id).FirstOrDefaultAsync();
                        if (screw == null) continue;
                        screw.Status = StationTaskStatusEnum.进行中;
                        screw.CurCompleteNum = screw.CurCompleteNum >= item.ReworkNum ? screw.CurCompleteNum - item.ReworkNum : 0;
                        var screwDetails = await _dbContext.Proc_StationTask_BlotGunDetails.Where(w => !w.IsDeleted && w.Proc_StationTask_BlotGunId == screw.Id && w.ResultIsOK).OrderByDescending(o => o.Id).Take(item.ReworkNum).ToListAsync();
                        foreach (var screwDetail in screwDetails)
                        {
                            screwDetail.IsDeleted = true;
                        }
                        _dbContext.UpdateRange(screwDetails);
                        _dbContext.Update(screw);
                        MainId = await SetRecordWorking(screw.StationTask_RecordId);
                        break;
                    case StationTaskTypeEnum.图示拧紧:
                        var screwImage = await _dbContext.Proc_StationTask_TightenByImages
                            .Include(i => i.Proc_StationTask_Record)
                            .Where(w => !w.IsDeleted && w.Id == item.Id)
                            .FirstOrDefaultAsync();

                        if (screwImage == null) continue;
                        if (item.OrderNo > 0)
                        {
                            var sameOrder = await _dbContext.Proc_StationTask_TightenByImages
                                .Where(w => !w.IsDeleted && w.Proc_StationTask_RecordId == screwImage.Proc_StationTask_RecordId && w.OrderNo == item.OrderNo)
                                .ToListAsync();
                            foreach (var d in sameOrder)
                            {
                                d.IsDeleted = true;
                            }
                            _dbContext.UpdateRange(sameOrder);
                        }
                        else
                        {
                            screwImage.IsDeleted = true;
                            _dbContext.Update(screwImage);
                        }

                        MainId = await SetRecordWorking(screwImage.Proc_StationTask_RecordId);
                        break;
                    case StationTaskTypeEnum.称重:
                        var anyload = await _dbContext.Proc_StationTask_AnyLoads.Where(w => !w.IsDeleted && w.Id == item.Id).FirstOrDefaultAsync();
                        if (anyload == null) continue;
                        anyload.IsDeleted = true;
                        _dbContext.Update(anyload);
                        MainId = await SetRecordWorking(anyload.StationTask_RecordId);

                        break;
                    case StationTaskTypeEnum.超时检测:
                        var glueTime = await _dbContext.Proc_StationTask_CheckTimeouts.Where(w => !w.IsDeleted && w.Id == item.Id).FirstOrDefaultAsync();
                        if (glueTime == null) continue;
                        glueTime.IsDeleted = true;
                        _dbContext.Update(glueTime);
                        MainId = await SetRecordWorking(glueTime.StationTask_RecordId);
                        break;
                    case StationTaskTypeEnum.用户输入:
                        var userInput = await _dbContext.Proc_StationTask_UserInputs.Where(w => !w.IsDeleted && w.Id == item.Id).FirstOrDefaultAsync();
                        if (userInput == null) continue;
                        userInput.IsDeleted = true;
                        MainId = await SetRecordWorking(userInput.StationTask_RecordId);
                        _dbContext.Update(userInput);
                        break;
                    case StationTaskTypeEnum.补拧: //特殊逻辑特殊处理
                        var repairScrews = await _dbContext.Proc_StationTask_TightenReworks.Where(w => !w.IsDeleted && w.StationTaskRecordId == item.Id).OrderByDescending(o => o.OrderNo).ToListAsync();
                        if (repairScrews == null || repairScrews.Count == 0) continue;
                        foreach (var repairScrew in repairScrews)
                        {
                            repairScrew.IsDeleted = true;
                        }
                        _dbContext.UpdateRange(repairScrews);
                        MainId = await SetRecordWorking(item.Id);
                        break;
                    case StationTaskTypeEnum.扫码输入:
                        var scanCollect = await _dbContext.Proc_StationTask_ScanCollects.Where(w => !w.IsDeleted && w.Id == item.Id).FirstOrDefaultAsync();
                        if (scanCollect == null)
                            continue;
                        scanCollect.IsDeleted = true;
                        _dbContext.Update(scanCollect);
                        MainId = await SetRecordWorking(scanCollect.StationTask_RecordId);
                        break;
                    case StationTaskTypeEnum.时间记录:
                        var timeRecord = await _dbContext.Proc_StationTask_TimeRecords.Where(w => !w.IsDeleted && w.Id == item.Id).FirstOrDefaultAsync();
                        if (timeRecord == null)
                            continue;
                        timeRecord.IsDeleted = true;
                        _dbContext.Update(timeRecord);
                        MainId = await SetRecordWorking(timeRecord.Proc_StationTask_RecordId);
                        break;
                }
            }

            var records = await _dbContext.Proc_StationTask_Records.Where(w => w.Proc_StationTask_MainId == MainId).Include(i => i.Base_StationTask).ToListAsync();
            var letGoRecord = records.Where(w => w.Base_StationTask.Type == StationTaskTypeEnum.放行).ToList();
            var account = records.Where(w => w.Base_StationTask.Type == StationTaskTypeEnum.扫描员工卡).ToList();
            /*// 网页上一键返工自动将工位设置为未完成
            var main = await _dbContext.Proc_StationTask_Mains.FirstOrDefaultAsync(f => !f.IsDeleted && f.Id == MainId);
            if (main != null)
            {
                main.Status = StationTaskStatusEnum.进行中;
                _dbContext.Update(main);
            }*/
            _dbContext.RemoveRange(letGoRecord);
            _dbContext.RemoveRange(account);
            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            result.Code = 500;
            result.Message = ex.Message;
        }
        return result;
    }


    public async Task<int> SetRecordWorking(int RecordId)
    {
        var record = await _dbContext.Proc_StationTask_Records.FirstOrDefaultAsync(f => f.Id == RecordId);
        if (record == null)
        {
            return 0;
        }
        record.Status = StationTaskStatusEnum.进行中;
        _dbContext.Update(record);
        await _dbContext.SaveChangesAsync();
        return record.Proc_StationTask_MainId;
    }
    public async Task<Response<List<ReWorkData>>> LoadReworkList(LoadReworkDataDto dto)
    {
        var result = new Response<List<ReWorkData>>();
        try
        {
            var Station = await _dbContext.Base_Stations.FirstOrDefaultAsync(f => !f.IsDeleted && f.Code == dto.StepCode);
            if (Station == null)
            {
                result.Code = 500;
                result.Message = $"工位{dto.StepCode}不存在";
                return result;
            }
            var mains = await _dbContext.Proc_StationTask_Mains.Where(f => f.StepId == Station.StepId && !f.IsDeleted).ToListAsync();
            var main = mains.FirstOrDefault(f => f.PackCode == dto.PackCode);
            if (main == null)
            {
                result.Code = 500;
                result.Message = "没有可以返工的项";
                return result;
            }
            var product = await _dbContext.Products.FirstOrDefaultAsync(f => !f.IsDeleted && f.Id == main.ProductId);
            if (product == null)
            {
                result.Code = 500;
                result.Message = "未找到返工条码对应的产品信息";
                return result;
            }

            var tasks = await _dbContext.Base_StationTasks.Where(w => w.ProductId == product.Id && !w.IsDeleted && w.StepId == Station.StepId).OrderBy(o => o.Sequence).ToListAsync();
            if (tasks.Count() == 0)
            {
                result.Code = 500;
                result.Message = "没有可以返工的项";
                return result;
            }

            var records = await _dbContext.Proc_StationTask_Records.Where(w => !w.IsDeleted && w.Proc_StationTask_MainId == main.Id).ToListAsync();
            result.Result = await PacketReworkData(tasks, records);
        }
        catch (Exception ex)
        {
            result.Code = 500;
            result.Message = $"{ex.Message}";
            return result;
        }
        return result;
    }

    private async Task<List<ReWorkData>> PacketReworkData(List<Base_StationTask> StationTasks, List<Proc_StationTask_Record> Records)
    {
        var result = new List<ReWorkData>();

        foreach (var task in StationTasks)
        {
            var record = Records.FirstOrDefault(f => f.Base_StationTaskId == task.Id);
            if (record == null)
            {
                continue;
            }
            var reworkData = new ReWorkData()
            {
                Id = record.Id,
                StationTaskId = task.Id,
                WorkName = task.Name == null ? "" : task.Name,
                Sequence = task.Sequence,
                StationTaskType = task.Type,
                Statue = record == null ? 0 : record.Status,
                WorkRecords = record == null ? new List<WorkRecord>() : await PacketWorkRecord(record.Id, task.Type)
            };
            result.Add(reworkData);
        }
        return result;
    }

    private async Task<List<WorkRecord>> PacketWorkRecord(int recordId, StationTaskTypeEnum type)
    {
        var result = new List<WorkRecord>();
        switch (type)
        {
            case StationTaskTypeEnum.扫码:
                var boms = await _dbContext.Proc_StationTask_Boms.Where(w => !w.IsDeleted && w.StationTask_RecordId == recordId).ToListAsync();
                foreach (var bom in boms)
                {
                    result.Add(new WorkRecord()
                    {
                        Id = bom.Id,
                        WorkName = bom.GoodsName ?? "",
                        CurNo = bom.CurCompleteNum,
                        ReworkNum = bom.CurCompleteNum,
                        Statue = bom.Status,
                        StationTaskType = StationTaskTypeEnum.扫码
                    });
                }
                break;
            case StationTaskTypeEnum.人工拧螺丝:
                var screws = await _dbContext.Proc_StationTask_BlotGuns.Where(w => !w.IsDeleted && w.StationTask_RecordId == recordId).ToListAsync();
                foreach (var screw in screws)
                {
                    result.Add(new WorkRecord()
                    {
                        Id = screw.Id,
                        WorkName = screw.ScrewName ?? "",
                        CurNo = screw.CurCompleteNum,
                        ReworkNum = screw.CurCompleteNum,
                        Statue = screw.Status,
                        ReworkNums = CreateReworkNum(screw.CurCompleteNum),
                        ScrewDatas = await _dbContext.Proc_StationTask_BlotGunDetails.Where(w => !w.IsDeleted && w.Proc_StationTask_BlotGunId == screw.Id).Select(s => new ScrewDatas { Id = s.Id, OrderNo = (int)s.OrderNo!, ResultOk = s.ResultIsOK, Torque = s.FinalTorque + "N", Angle = s.FinalAngle + "°" }).ToListAsync(),
                        StationTaskType = StationTaskTypeEnum.人工拧螺丝
                    });
                }
                break;
            case StationTaskTypeEnum.图示拧紧:
                var screwImage = await _dbContext.Proc_StationTask_TightenByImages
                    .Where(w => !w.IsDeleted && w.Proc_StationTask_RecordId == recordId && w.OrderNo.HasValue)
                    .OrderBy(o => o.OrderNo)
                    .ThenByDescending(o => o.CreateTime)
                    .ThenByDescending(o => o.Id)
                    .ToListAsync();

                // 返工列表按序号去重（取最新一条），避免同一序号出现多条可选项
                var latestByOrder = screwImage
                    .Where(w => (w.OrderNo ?? 0) > 0)
                    .GroupBy(g => g.OrderNo!.Value)
                    .Select(g => g.First())
                    .OrderBy(o => o.OrderNo)
                    .ToList();

                foreach (var item in latestByOrder)
                {
                    result.Add(new WorkRecord()
                    {
                        Id = item.Id,
                        WorkName = (item.TaskName ?? "") + ",序号：" + item.OrderNo,
                        OrderNo = item.OrderNo ?? 0,
                        StationTaskType = StationTaskTypeEnum.图示拧紧
                    });
                }
                break;
            case StationTaskTypeEnum.称重:

                var anyloads = await _dbContext.Proc_StationTask_AnyLoads.Where(w => !w.IsDeleted && w.StationTask_RecordId == recordId).ToListAsync();
                foreach (var anyload in anyloads)
                {
                    result.Add(new WorkRecord()
                    {
                        Id = anyload.Id,
                        WorkName = anyload.AnyLoadName ?? "",
                        CurNo = 1,
                        ReworkNum = 1,
                        Statue = anyload.Status,
                        StationTaskType = StationTaskTypeEnum.称重
                    });
                }
                break;
            case StationTaskTypeEnum.超时检测:
                var chekTimeOuts = await _dbContext.Proc_StationTask_CheckTimeouts.Where(w => !w.IsDeleted && w.StationTask_RecordId == recordId).ToListAsync();
                foreach (var chekTimeOut in chekTimeOuts)
                {
                    result.Add(new WorkRecord()
                    {
                        Id = chekTimeOut.Id,
                        WorkName = chekTimeOut.TimeName ?? "",
                        CurNo = 1,
                        ReworkNum = 1,
                        Statue = chekTimeOut.Status ?? StationTaskStatusEnum.进行中,
                        StationTaskType = StationTaskTypeEnum.超时检测
                    });
                }
                break;
            case StationTaskTypeEnum.用户输入:
                var userInputs = await _dbContext.Proc_StationTask_UserInputs.Where(w => !w.IsDeleted && w.StationTask_RecordId == recordId).ToListAsync();
                foreach (var userInput in userInputs)
                {
                    result.Add(new WorkRecord()
                    {
                        Id = userInput.Id,
                        WorkName = userInput.UserInputName ?? "",
                        CurNo = 1,
                        ReworkNum = 1,
                        Statue = userInput.Status,
                        StationTaskType = StationTaskTypeEnum.用户输入
                    });
                }
                break;
            case StationTaskTypeEnum.补拧:
                var repairScrews = await _dbContext.Proc_StationTask_TightenReworks.Where(w => !w.IsDeleted && w.StationTaskRecordId == recordId).ToListAsync();
                result.Add(new WorkRecord()
                {
                    Id = recordId,
                    WorkName = "补拧",
                    CurNo = repairScrews.Count,
                    ReworkNum = repairScrews.Count,
                    Statue = StationTaskStatusEnum.已完成,
                    StationTaskType = StationTaskTypeEnum.补拧
                });
                break;
            case StationTaskTypeEnum.扫码输入:
                var scanCollects = await _dbContext.Proc_StationTask_ScanCollects.Where(w => !w.IsDeleted && w.StationTask_RecordId == recordId).ToListAsync();
                foreach (var scanCollect in scanCollects)
                {
                    result.Add(new WorkRecord()
                    {
                        Id = scanCollect.Id,
                        WorkName = scanCollect.ScanCollectName ?? "",
                        CurNo = 1,//TODO增加数量
                        ReworkNum = 1,
                        Statue = scanCollect.Status,
                        ReworkNums = CreateReworkNum(1),
                        StationTaskType = StationTaskTypeEnum.扫码输入
                    });
                }
                break;
            case StationTaskTypeEnum.时间记录:
                var timeRecords = await _dbContext.Proc_StationTask_TimeRecords.Where(w => !w.IsDeleted && w.Proc_StationTask_RecordId == recordId).ToListAsync();
                foreach (var timeRecord in timeRecords)
                {
                    result.Add(new WorkRecord()
                    {
                        Id = timeRecord.Id,
                        WorkName = "时间记录",
                        CurNo = 1,//TODO增加数量
                        ReworkNum = 1,
                        Statue = StationTaskStatusEnum.已完成,
                        StationTaskType = StationTaskTypeEnum.时间记录
                    });
                }
                break;
        }

        return result;

    }

    private List<int> CreateReworkNum(int TotalNum)
    {
        List<int> result = new List<int>();
        for (int i = 1; i <= TotalNum; i++)
        {
            result.Add(i);
        }

        return result;
    }


    public async Task<UploadCATLData> GetUploadCATLDataByCode(string packCode, string stationCode)
    {
        var result = new UploadCATLData();
        result.PackCode = packCode;
        result.DCParams = new List<DcParamValue>();
        result.ScanCodeData = new List<BomData>();

        var station = await _dbContext.Base_Stations.AsNoTracking().Where(e => e.Code == stationCode).FirstOrDefaultAsync() ?? throw new Exception($"未找到工位信息，[{stationCode}]");
        var dcParams = new List<DcParamValue>();
        var main = await _dbContext.Proc_StationTask_Mains.FirstOrDefaultAsync(f => !f.IsDeleted && f.StepId == station.StepId && f.PackCode == packCode);
        if (main == null)
        {
            return result;
        }

        //var records = await _dbContext.Proc_StationTask_Records.Include(i => i.Base_StationTask).Where(w => w.Base_StationTask != null && !w.Base_StationTask.IsDeleted && !w.IsDeleted && w.Proc_StationTask_MainId == main.Id).ToListAsync();
        var records = await _dbContext.Proc_StationTask_Records.Include(i => i.Base_StationTask).Where(w => !w.IsDeleted && w.Proc_StationTask_MainId == main.Id).ToListAsync();

        foreach (var record in records)
        {
            switch (record.Base_StationTask!.Type)
            {
                case StationTaskTypeEnum.扫描员工卡:
                    var scanAccount = await LoadScanAccountData(record.Id);
                    if (scanAccount.IsError)
                    {
                        return new UploadCATLData();
                    }
                    dcParams.AddRange(scanAccount.ResultValue);
                    break;
                case StationTaskTypeEnum.人工拧螺丝:
                    var tightenData = await LoadTightenData(record.Id);
                    if (tightenData.IsError)
                    {
                        return new UploadCATLData();
                    }
                    dcParams.AddRange(tightenData.ResultValue);
                    break;
                case StationTaskTypeEnum.称重:
                    var weightData = await LoadWeightData(record.Id);
                    if (weightData.IsError)
                    {
                        return new UploadCATLData();
                    }
                    dcParams.AddRange(weightData.ResultValue);
                    break;
                case StationTaskTypeEnum.用户输入:
                    var userInputData = await LoadUserInputData(record.Id);
                    if (userInputData.IsError)
                    {
                        return new UploadCATLData();
                    }
                    dcParams.AddRange(userInputData.ResultValue);
                    break;
                case StationTaskTypeEnum.扫码输入:
                    var scanCollectData = await LoadScanCollectData(record.Id);
                    if (scanCollectData.IsError)
                    {
                        return new UploadCATLData();
                    }
                    dcParams.AddRange(scanCollectData.ResultValue);
                    break;
                case StationTaskTypeEnum.时间记录:
                    var timeRecord = await LoadTimeRecordData(record.Id);
                    if (timeRecord.IsError)
                    {
                        return new UploadCATLData();
                    }
                    dcParams.AddRange(timeRecord.ResultValue);
                    break;
                case StationTaskTypeEnum.超时检测:
                    var timeOut = await LoadTimeCheckOutData(record.Id);
                    if (timeOut.IsError)
                    {
                        return new UploadCATLData();
                    }
                    dcParams.AddRange(timeOut.ResultValue);
                    break;
                case StationTaskTypeEnum.补拧:
                    var tightenRework = await LoadTightenReworkData(record, main.PackCode);
                    if (tightenRework.IsError)
                    {
                        return new UploadCATLData();
                    }
                    dcParams.AddRange(tightenRework.ResultValue);
                    break;
            }
        }
        result.DCParams = dcParams;
        return result;
    }

    public async Task<Response<string>> GetProductPnFormDb(string packCode)
    {
        var resp = new Response<string>();
        try
        {
            var productList = await _dbContext.Products.Where(w => !w.IsDeleted).ToListAsync();
            var p = productList.FirstOrDefault(s => CompareCodeRuleAsync(s.PackPNRule, packCode));
            if (p != null)
            {
                resp.Result = p.Code;
                return resp;
            }
            resp.Code = 500;

            resp.Message = "产品未找到";
        }
        catch (Exception ex)
        {
            resp.Code = 500;
            resp.Message = ex.Message;
        }
        return resp;
    }
    public bool CompareCodeRuleAsync(string rule, string Code)
    {
        if (string.IsNullOrEmpty(rule))
        {
            return true;
        }
        if (rule.Length != Code.Length)
        {
            return false;
        }
        var z = rule.Zip(Code);
        var tuple = z.Where(w => w.First != '*').ToList();
        var checkError = tuple.Any(f => f.First != f.Second);
        if (checkError)
        {
            return false;
        }
        return true;
    }
}
