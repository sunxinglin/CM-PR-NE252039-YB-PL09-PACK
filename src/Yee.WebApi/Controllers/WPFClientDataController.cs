using AsZero.Core.Services.Repos;

using Microsoft.AspNetCore.Mvc;

using Yee.Common.Library.CommonEnum;
using Yee.Entitys;
using Yee.Entitys.CATL;
using Yee.Entitys.DBEntity;
using Yee.Entitys.DBEntity.Production;
using Yee.Entitys.DBEntity.ProductionRecords;
using Yee.Entitys.DTOS;
using Yee.Entitys.DTOS.StationTaskDataDTOS;
using Yee.Entitys.Production;
using Yee.Services.AutomaticStation;
using Yee.Services.Production;
using Yee.Services.ProductionRecord;

namespace Yee.WebApi.Controllers.BaseData;

[Route("api/[controller]/[action]")]
[ApiController]
public class WPFClientDataController : ControllerBase
{
    private readonly StationService _StationService;
    private readonly StationTask_BlotGunDetailService _StationTask_BlotGunDetailService;
    private readonly SaveStationDataService _SaveStationDataService;
    private readonly ILogger<WPFClientDataController> _logger;
    private static bool IsDoingWork = false;
    private readonly Base_StationTaskScanCollectService _StationTaskScanCollectService;
    private readonly AutomicCommonService _CommonService;

    public WPFClientDataController(SaveStationDataService saveStationDataService,
        StationTask_BlotGunDetailService stationTask_BlotGunDetailService, StationService stationService,
        ILogger<WPFClientDataController> logger,
        Base_StationTaskScanCollectService base_StationTaskScanCollectService, AutomicCommonService commonService)
    {
        _StationService = stationService;
        _SaveStationDataService = saveStationDataService;
        _logger = logger;
        _StationTask_BlotGunDetailService = stationTask_BlotGunDetailService;
        _StationTaskScanCollectService = base_StationTaskScanCollectService;
        _CommonService = commonService;
    }


    [HttpPost]
    public async Task<Response> DealCatlMES_InStation(InStationDataDTO inStationData)
    {
        var result = new Response();
        try
        {
            result = await _SaveStationDataService.DealCatlMES_InStation(inStationData);
        }
        catch (Exception ex)
        {
            result.Code = 500;
            result.Message = ex.InnerException?.Message ?? ex.Message;
        }

        return result;
    }

    /// <summary>
    /// 保存工位数据
    /// </summary>
    [HttpPost]
    public async Task<Response> Save_StationTaskBom(BomDataDTO input)
    {
        var result = new Response();
        try
        {
            result = await _SaveStationDataService.Save_StationTaskBom(input);
        }
        catch (Exception ex)
        {
            result.Code = 500;
            result.Message = ex.InnerException?.Message ?? ex.Message;
        }

        return result;
    }

    [HttpGet]
    public async Task<Response> CheckBomUsed(int? stepId, TracingTypeEnum TracingType, string? Code)
    {
        var result = new Response();
        try
        {
            result = await _SaveStationDataService.CheckBomUsed(stepId, TracingType, Code);
        }
        catch (Exception ex)
        {
            result.Code = 500;
            result.Message = ex.InnerException?.Message ?? ex.Message;
        }

        return result;
    }

    /// <summary>
    /// 保存工位数据
    /// </summary>
    [HttpPost]
    public async Task<Response> Save_StationAnyLoad(AnyLoadDataDTO input)
    {
        var result = new Response();
        try
        {
            result = await _SaveStationDataService.Save_StationAnyLoad(input);
        }
        catch (Exception ex)
        {
            result.Code = 500;
            result.Message = ex.InnerException?.Message ?? ex.Message;
        }

        return result;
    }

    [HttpPost]
    public async Task<Response> Save_AccountCardData(ScanAccountCardDataDTO scanAccountCard)
    {
        var result = new Response();
        try
        {
            result = await _SaveStationDataService.Save_AccountCardData(scanAccountCard);
        }
        catch (Exception ex)
        {
            result.Code = 500;
            result.Message = ex.InnerException?.Message ?? ex.Message;
        }

        return result;
    }


    /// <summary>
    /// 保存外部输入数据
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<Response> Save_StationUserScan(UserInputDataDTO input)
    {
        var result = new Response();
        try
        {
            result = await _SaveStationDataService.Save_StationUserInput(input);
        }
        catch (Exception ex)
        {
            result.Code = 500;
            result.Message = ex.InnerException?.Message ?? ex.Message;
        }

        return result;
    }

    /// <summary>
    /// 保存扫码输入数据
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<Response> Save_StationScanCollect(ScanCollectDataDTO input)
    {
        var result = new Response();
        try
        {
            result = await _SaveStationDataService.Save_StationScanCollect(input);
        }
        catch (Exception ex)
        {
            result.Code = 500;
            result.Message = ex.InnerException?.Message ?? ex.Message;
        }

        return result;
    }

    [HttpPost]
    public async Task<Response> SaveRecordTime(SaveTimeDTO dto)
    {
        return await _SaveStationDataService.SaveRecordTime(dto);
    }


    /// <summary>
    /// 更新工位任务状态
    /// </summary>
    [HttpPost]
    public async Task<Response> SetStationCurTaskFinish(StationTaskDTO taskDTO, int mainId)
    {
        var result = new Response();
        try
        {
            result = await _SaveStationDataService.SetStationCurTaskFinish(taskDTO, mainId);
        }
        catch (Exception ex)
        {
            result.Code = 500;
            result.Message = ex.InnerException?.Message ?? ex.Message;
        }

        return result;
    }

    /// <summary>
    /// 放行AGV任务
    /// </summary>
    [HttpPost]
    public async Task<Response> SetStationCurTaskRunAGV(StationTaskDTO? taskDTO, int mainId)
    {
        var result = new Response();
        try
        {
            result = await _SaveStationDataService.SetStationCurTaskRunAGV(taskDTO, mainId);
        }
        catch (Exception ex)
        {
            result.Code = 500;
            result.Message = ex.InnerException?.Message ?? ex.Message;
        }

        return result;
    }


    /// <summary>
    /// 保存螺丝拧紧数据
    /// </summary>
    [HttpPost]
    public async Task<Response> SaveScrewData(ScrewDataDTO screw)
    {
        var result = new Response();
        try
        {
            result = await _SaveStationDataService.SaveScrewData(screw);
        }
        catch (Exception ex)
        {
            result.Code = 500;
            result.Message = ex.InnerException?.Message ?? ex.Message;
        }

        return result;
    }
    
    /// <summary>
    /// 保存螺丝拧紧数据
    /// </summary>
    [HttpPost]
    public async Task<Response> SaveTightenByImageData(ScrewDataDTO screw)
    {
        return await _SaveStationDataService.SaveTightenByImageData(screw);
    }
    
    /// <summary>
    /// 保存图示拧紧NG数据
    /// </summary>
    [HttpPost]
    public async Task<Response> SaveNgData(ScrewDataDTO screw)
    {
        return await _SaveStationDataService.SaveNgData(screw);
    }

    /// <summary>
    /// 保存图示拧紧反拧数据
    /// </summary>
    [HttpPost]
    public async Task<Response> SaveReverseData(ScrewDataDTO screw)
    {
        return await _SaveStationDataService.SaveReverseData(screw);
    }

    /// <summary>
    /// 更新螺丝拧紧状态
    /// </summary>
    [HttpPost]
    public async Task<Response> SetScrewTaskFinish(Base_StationTaskScrew screw, int mainId)
    {
        var result = new Response();
        try
        {
            result = await _SaveStationDataService.SetScrewTaskFinish(screw, mainId);
        }
        catch (Exception ex)
        {
            result.Code = 500;
            result.Message = ex.InnerException?.Message ?? ex.Message;
        }

        return result;
    }

    [HttpPost]
    public async Task<Response> SetReWork(List<ReworkListDTO> reworkLists)
    {
        var result = new Response();
        //try
        //{
        //    result = await _SaveStationDataService.ReWork(reworkLists);
        //}
        //catch (Exception ex)
        //{
        //    result.Code = 500;
        //    result.Message = ex.InnerException?.Message ?? ex.Message;
        //}
        return result;
    }

    /// <summary>
    ///  获取涂胶开始时间
    /// </summary>
    /// <param name="packPN"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<Response<Proc_GluingInfo>> GetGluingStartTime(string packPN)
    {
        var result = new Response<Proc_GluingInfo>();
        try
        {
            result = await _SaveStationDataService.GetGluingStartTime(packPN);
        }
        catch (Exception ex)
        {
            result.Code = 500;
            result.Message = ex.InnerException?.Message ?? ex.Message;
        }

        return result;
    }

    /// <summary>
    /// 获取pack在对应工位的任务完成时间
    /// </summary>
    /// <param name="packPN"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<Response<DateTime>> GetStepTaskEndTime(string packPN, string stepCode)
    {
        var result = new Response<DateTime>();
        try
        {
            result = await _SaveStationDataService.GetStepTaskEndTime(packPN, stepCode);
        }
        catch (Exception ex)
        {
            result.Code = 500;
            result.Message = ex.InnerException?.Message ?? ex.Message;
        }

        return result;
    }


    [HttpGet]
    public async Task<Response<DateTime>> GetModuleInBoxEndTime(string packPN, string stepCode)
    {
        var result = new Response<DateTime>();
        try
        {
            result = await _SaveStationDataService.GetModuleInBoxEndTime(packPN, stepCode);
        }
        catch (Exception ex)
        {
            result.Code = 500;
            result.Message = ex.InnerException?.Message ?? ex.Message;
        }

        return result;
    }

    /// <summary>
    /// 获取模组拧紧完成时间
    /// </summary>
    /// <param name="packPN"></param>
    /// <param name="stepCode"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<Response<DateTime>> GetBoltGunEndTime(string packPN, string stepCode)
    {
        var result = new Response<DateTime>();
        try
        {
            result = await _SaveStationDataService.GetBoltGunEndTime(packPN, stepCode);
        }
        catch (Exception ex)
        {
            result.Code = 500;
            result.Message = ex.InnerException?.Message ?? ex.Message;
        }

        return result;
    }

    /// <summary>
    /// 获取外部输入范围
    /// </summary>
    /// <param name="StationId"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<Response<Base_StationTaskBom>> GetOutScanRange(string StationTaskId)
    {
        var result = new Response<Base_StationTaskBom>();
        try
        {
            result = await _SaveStationDataService.GetOutScanRange(StationTaskId);
        }
        catch (Exception ex)
        {
            result.Code = 500;
            result.Message = ex.InnerException?.Message ?? ex.Message;
        }

        return result;
    }

    /// <summary>
    /// 保存超时信息
    /// </summary>
    /// <param name="stepCode"></param>
    /// <param name="proResourceType"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<Response> SaveStationTaskCheckTimeOut(CheckTimeOutDataDTO proc)
    {
        var result = new Response();

        try
        {
            result = await _SaveStationDataService.SaveStationTaskCheckTimeOut(proc);
            result.Code = 200;
            result.Message = "保存成功";
        }
        catch (Exception ex)
        {
            result.Code = 500;
            result.Message = ex.InnerException?.Message ?? ex.Message;
        }

        return result;
    }

    /// <summary>
    /// 保存工序时间差信息
    /// </summary>
    /// <param name="stepCode"></param>
    /// <param name="proResourceType"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<Response> SaveStationTaskStewingTime(StewingTimeDataDTO proc)
    {
        var result = new Response();

        try
        {
            result = await _SaveStationDataService.SetStationTaskStewingTime(proc);
            result.Code = 200;
            result.Message = "保存成功";
        }
        catch (Exception ex)
        {
            result.Code = 500;
            result.Message = ex.InnerException?.Message ?? ex.Message;
        }

        return result;
    }

    [HttpGet]
    public async Task<Response<List<Base_ProResource>>> LoadStationProResourceConfig(string stationCode,
        int proResourceType)
    {
        var result = new Response<List<Base_ProResource>>();
        try
        {
            result = await _SaveStationDataService.LoadStationProResourceConfig(stationCode, proResourceType);
        }
        catch (Exception ex)
        {
            result.Code = 500;
            result.Message = ex.InnerException?.Message ?? ex.Message;
        }

        return result;
    }


    /// <summary>
    /// 保存工位设备配置
    /// </summary>
    [HttpPost]
    public async Task<Response<List<Base_ProResource>>> SaveStationEquipmentConifgData(
        List<Base_ProResource> resList, ProResourceTypeEnum resourceType, string stationCode, string Operator)
    {
        var result = new Response<List<Base_ProResource>>();
        try
        {
            result = await _SaveStationDataService.SaveStationEquipmentConifgData(resList, resourceType,
                stationCode, Operator);
        }
        catch (Exception ex)
        {
            result.Code = 500;
            result.Message = ex.InnerException?.Message ?? ex.Message;
        }

        return result;
    }

    /// <summary>
    /// 加载OPO160工位的自动拧紧数据
    /// </summary>
    [HttpGet]
    public async Task<Response<List<Proc_AutoBoltInfo_Detail>>> LoadAutoBoltData(int stepId, string packCode)
    {
        var result = new Response<List<Proc_AutoBoltInfo_Detail>>();
        try
        {
            //    List<Proc_AutoBoltInfo_Detail> list = _AutoBoltService.LoadAutoBoltData(stepId, packCode);
            //    result.Result = list;
            result.Code = 200;
        }
        catch (Exception ex)
        {
            result.Code = 500;
            result.Message = ex.InnerException?.Message ?? ex.Message;
        }

        return result;
    }


    /// <summary>
    /// 加载OPO160工位的自动拧紧数据
    /// </summary>
    [HttpGet]
    public async Task<Response<List<Proc_AutoBoltInfo_Detail>>> LoadAutoBoltData_Prev(int stepId, string packCode)
    {
        var result = new Response<List<Proc_AutoBoltInfo_Detail>>();
        try
        {
            //List<Proc_AutoBoltInfo_Detail> list = await _AutoBoltService.LoadAutoBoltData_Prev(stepId, packCode);
            //result.Result = list;
            result.Code = 200;
        }
        catch (Exception ex)
        {
            result.Code = 500;
            result.Message = ex.InnerException?.Message ?? ex.Message;
        }

        return result;
    }


    /// <summary>
    /// 加载OPO160工位的自动拧紧数据
    /// </summary>
    [HttpGet]
    public async Task<Response<List<Proc_StationTask_BlotGunDetail>>> LoadBoltData(int stepId, int stationtaskid,
        string packCode)
    {
        var result = new Response<List<Proc_StationTask_BlotGunDetail>>();
        try
        {
            List<Proc_StationTask_BlotGunDetail> list =
                await _StationTask_BlotGunDetailService.LoadBoltData(stepId, stationtaskid, packCode);
            result.Result = list;
            result.Code = 200;
        }
        catch (Exception ex)
        {
            result.Code = 500;
            result.Message = ex.InnerException?.Message ?? ex.Message;
        }

        return result;
    }

    /// <summary>
    /// 校验扫码输入
    /// </summary>
    /// <param name="stepid"></param>
    /// <param name="packpn"></param>
    /// <param name="scanCollectData"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<Response<bool>> GetScanCollectInfo(int stepid, string packpn, string scanCollectData)
    {
        var result = new Response<bool>();
        try
        {
            var getScanCollectInfoResult =
                await _StationTaskScanCollectService.GetScanCollectInfo(stepid, packpn, scanCollectData);
            result.Result = getScanCollectInfoResult;
            result.Code = 200;
        }
        catch (Exception ex)
        {
            result.Code = 500;
            result.Message = ex.InnerException?.Message ?? ex.Message;
        }

        return result;
    }

    [HttpGet]
    public async Task<Response<UploadCATLData>> GetUploadCATLData(string packCode, int stepId, int stationId)
    {
        var result = new Response<UploadCATLData>();

        try
        {
            var upData = await _SaveStationDataService.GetUploadCATLData(packCode, stepId, stationId);
            result.Result = upData;
            result.Code = 200;
        }
        catch (Exception ex)
        {
            result.Code = 500;
            result.Message = ex.InnerException?.Message ?? ex.Message;
        }

        return result;
    }

    [HttpGet]
    public async Task<Response<PackTaskMainDataDTO>> LoadStepPackTaskMainData(string stationCode, string beginTime,
        string endTime)
    {
        var result = new Response<PackTaskMainDataDTO>();

        try
        {
            DateTime begin, end;
            if (DateTime.TryParse(beginTime, out begin) && DateTime.TryParse(endTime, out end))
            {
                result = await _SaveStationDataService.LoadStepPackTaskMainData(stationCode, begin, end);
            }
            else
            {
                result.Code = 500;
                result.Message = "查询时间范围不合法";
            }
        }
        catch (Exception ex)
        {
            result.Code = 500;
            result.Message = ex.InnerException?.Message ?? ex.Message;
        }

        return result;
    }

    [HttpGet]
    public async Task<Response<Proc_StationTask_TimeRecord>> GetRecordTime(string packCode, string TimeFlag)
    {
        return await _SaveStationDataService.GetRecordTime(packCode, TimeFlag);
    }

    [HttpGet]
    public async Task<Response<(string, int)>> GetCurrentStationAndLevel(string stationFlag, string packCode)
    {
        var resp = new Response<(string, int)>();
        var result = await _CommonService.GetCurrentStation(stationFlag, packCode);
        if (string.IsNullOrEmpty(result.Item1))
        {
            resp.Code = 500;
            resp.Message = $"{packCode}在{stationFlag}的所有层级任务均已完工";
            return resp;
        }

        resp.Result = result;
        return resp;
    }

    [HttpGet]
    public async Task<Response> CheckVectorBindAndFlowOrder(string packCode, int vectorCode, string stationCode, 
        string VectorStation, string productPn, string? packOutCode)
    {
        var resp = new Response();
        var result = await _CommonService.CheckVectorBindAndFlowOrder(packCode, vectorCode, stationCode,
            VectorStation, productPn, packOutCode);
        if (result.IsError)
        {
            resp.Code = result.ErrorValue.ErrorCode;
            resp.Message = result.ErrorValue.ErrorMessage;
        }

        return resp;
    }

    [HttpGet]
    public async Task<Response> CheckFlowOrder(string packCode, string stationCode, string productPn)
    {
        var resp = new Response();
        var result = await _CommonService.CheckFlowOrder(packCode, stationCode, productPn);
        if (result.IsError)
        {
            resp.Code = result.ErrorValue.ErrorCode;
            resp.Message = result.ErrorValue.ErrorMessage;
        }

        return resp;
    }


    [HttpPost]
    public async Task<Response<StationTaskHistoryDTO>> CreateOrLoadTraceInfo(string packCode, string stationCode, string productCode)
    {
        return await _SaveStationDataService.CreateOrLoadTraceInfo(packCode, stationCode, productCode);
    }

    [HttpGet]
    public async Task<Response<IList<AutoBlotInfo>>> LoadAutoTightenData(string packCode, TightenReworkType AutoTightenType, int ScrewNum)
    {
        return await _SaveStationDataService.LoadAutoTightenData(packCode, AutoTightenType, ScrewNum);
    }

    [HttpPost]
    public async Task<Response> SaveRepairData(TightenReworkDataDto dto)
    {
        return await _SaveStationDataService.SaveRepairData(dto);
    }

    [HttpPost]
    public async Task<Response> MakeTaskComplateForce(StationTaskDTO dto, int mainId)
    {
        return await _SaveStationDataService.MakeTaskComplateForce(dto, mainId);
    }


    [HttpPost]
    public async Task<Response> MakeTaskListComplateForce(IList<(int, string)> dto, int mainId)
    {
        return await _SaveStationDataService.MakeTasksComplateForce(dto, mainId);
    }


    [HttpPost]
    public async Task<Response<List<ReWorkData>>> LoadReworkList(LoadReworkDataDto dto)
    {
        return await _SaveStationDataService.LoadReworkList(dto);
    }


    [HttpPost]
    public async Task<Response> MakeRework(List<WorkRecord> works)
    {
        return await _SaveStationDataService.MakeRework(works);
    }

    [HttpPost]
    public async Task<Response> SetRecordWorking(int recordId)
    {
        var response = new Response();

        try
        {
            var result = await _SaveStationDataService.SetRecordWorking(recordId);
            if (result == 0)
            {
                response.Code = 500;
                response.Message = "任务记录不存在";
            }
        }
        catch (Exception ex)
        {
            response.Code = 500;
            response.Message = $"{ex.Message}";
        }

        return response;
    }

    /// <summary>
    /// 数据重传
    /// </summary>
    /// <param name="packCode"></param>
    /// <param name="stationCode"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<Response<UploadCATLData>> UploadDataAgain(string packCode, string stationCode)
    {
        var response = new Response<UploadCATLData>();
        var result = await _SaveStationDataService.GetUploadCATLDataByCode(packCode, stationCode);
        response.Result = result;
        return response;
    }

    [HttpGet]
    public async Task<Response<string>> GetProductPnFormDb(string packCode)
    {
        var result = await _SaveStationDataService.GetProductPnFormDb(packCode);
        return result;
    }

    /// <summary>
    /// 保存充气数据
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<Response> Save_StationLeak(LeakDataDTO input)
    {
        var result = new Response();
        try
        {
            result = await _SaveStationDataService.Save_StationLeak(input);
        }
        catch (Exception ex)
        {
            result.Code = 500;
            result.Message = ex.InnerException?.Message ?? ex.Message;
        }
        return result;
    }
}