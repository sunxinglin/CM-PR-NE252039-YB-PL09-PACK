using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

using AsZero.Core.Entities;
using AsZero.Core.Services.Repos;

using Ctp0600P.Client.Protocols;
using Ctp0600P.Client.Services;
using Ctp0600P.Client.Views.StationTaskPages;
using Ctp0600P.Shared;

using Itminus.FSharpExtensions;

using MediatR;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.FSharp.Core;

using Yee.Common.Library.CommonEnum;
using Yee.Entitys;
using Yee.Entitys.AlarmMgmt;
using Yee.Entitys.CATL;
using Yee.Entitys.CommonEntity;
using Yee.Entitys.DBEntity;
using Yee.Entitys.DBEntity.Production;
using Yee.Entitys.DBEntity.ProductionRecords;
using Yee.Entitys.DTOS;
using Yee.Entitys.DTOS.StationTaskDataDTOS;
using Yee.Entitys.Production;

namespace Ctp0600P.Client.Apis;

public class APIHelper : IAPIHelper
{
    private readonly ILogger<APIHelper> _logger;
    private readonly IMediator _mediator;
    private readonly ITraceApi _traceApi;

    protected ApiServerSetting ServerSetting { get; }

    public APIHelper(ILogger<APIHelper> logger, IOptionsMonitor<ApiServerSetting> cloudSetting, IMediator mediator, ITraceApi traceApi)
    {
        this._logger = logger;
        this._mediator = mediator;
        this.ServerSetting = cloudSetting.CurrentValue;
        this._traceApi = traceApi;
    }

    #region [校验员工卡]
    public async Task<Response<User>> Check_CreateAccountCardAsync(string accountCardValue)
    {
        var resp = await _traceApi.Check_CreateAccountCardAsync(accountCardValue);
        return resp.IsSuccessStatusCode ? resp.Content : new Response<User> { Code = Convert.ToInt32(resp.Error.StatusCode), Message = resp.Error.Message };

    }

    public async Task<Response<UploadCATLData>> GetUploadCATLData()
    {
        var resp = await _traceApi.GetUploadCATLData(App.PackBarCode, App._StepStationSetting.Step.Id,
            App._StepStationSetting.Station.Id);
        return resp.IsSuccessStatusCode
            ? resp.Content
            : new Response<UploadCATLData>
                { Code = Convert.ToInt32(resp.Error.StatusCode), Message = resp.Error.Message };
    }

    public async void Record_CheckPower_Log(string password, string moduleName, List<Alarm> alarms)
    {
        var checkDTO = new RecordCheckPowerDTO
        {
            StationCode = App._StepStationSetting.StationCode,
            PackCode = App.PackBarCode,
            Account = password,
            ModuleName = moduleName,
            Alarms = alarms
        };

        _logger.LogDebug("记录校验权限日志请求体：{Serialize}", JsonSerializer.Serialize(checkDTO));

        await _traceApi.Record_CheckPower_Log(checkDTO);
    }

    /// <summary>
    /// 登录
    /// </summary>
    /// <param name="account"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public async Task<Response<User>> LoginWithAPI(string account, string password)
    {
        var resp = await _traceApi.ValidateUser(account, password);
        return resp.IsSuccessStatusCode
            ? resp.Content
            : new Response<User> { Code = Convert.ToInt32(resp.Error.StatusCode), Message = resp.Error.Message };
    }

    /// <summary>
    /// 获取权限
    /// </summary>
    /// <param name="account"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public async Task<Response<ClaimEntity>> CheckClaimWithAPI(string account, string password)
    {
        var resp = await _traceApi.CheckuserClaims(account, password);
        return resp.IsSuccessStatusCode ? resp.Content : new Response<ClaimEntity> { Code = Convert.ToInt32(resp.Error.StatusCode), Message = resp.Error.Message };
    }
    #endregion

    #region 【加载设备配置信息】

    public async Task<List<Base_ProResource>> LoadStationProResourceConfig(ProResourceTypeEnum type)
    {
        var resp = await _traceApi.LoadStationProResourceConfig(App._StepStationSetting.StationCode, (int)type);
        return resp.IsSuccessStatusCode ? resp.Content.Result : null!;
    }

    #endregion

    #region 【保存设备配置信息】

    public async Task<Response<List<Base_ProResource>>> SaveStationEquipmentConifgData(
        List<Base_ProResource> stationScanCodeGunList, ProResourceTypeEnum resourceType)
    {
        var resp = await _traceApi.SaveStationEquipmentConifgData(stationScanCodeGunList, resourceType,
            App._StepStationSetting.StationCode, App.PowerUser);
        return resp.IsSuccessStatusCode
            ? resp.Content
            : new Response<List<Base_ProResource>>
                { Code = Convert.ToInt32(resp.Error.StatusCode), Message = resp.Error.Message };
    }

    #endregion

    #region 【保存工位任务数据】

    /// <summary>
    /// 保存涂胶超时数据
    /// </summary>
    /// <param name="proc"></param>
    /// <returns></returns>
    public async Task<Response> SaveCheckTimeOutData(Base_StationTaskCheckTimeOut proc, int taskId)
    {
        proc.AGVCode = App.AGVCode;
        proc.PackCode = App.PackBarCode;
        proc.CreateUserID = App.UserId;
        proc.StationId = App._StepStationSetting.Station.Id;
        proc.StepId = App._StepStationSetting.Step.Id;

        var postData = new CheckTimeOutDataDTO
        {
            MainId = App.HisTaskData2.MainId,
            StationTaskId = taskId,
            CheckTimeOutData = proc,
        };

        var resp = await _traceApi.SaveStationTaskCheckTimeOut(postData);
        return resp.IsSuccessStatusCode
            ? resp.Content
            : new Response { Code = Convert.ToInt32(resp.Error.StatusCode), Message = resp.Error.Message };

    }

    /// <summary>
    /// 静置时差
    /// </summary>
    /// <param name="proc"></param>
    /// <returns></returns>
    public async Task<Response> SaveStewingTimeMessage(Base_StationTaskStewingTime proc)
    {

        proc.AGVCode = App.AGVCode;
        proc.PackCode = App.PackBarCode;
        proc.CreateUserID = App.UserId;

        var postData = new StewingTimeDataDTO
        {
            StewingTimeData = proc,
            //StationTaskList = GetVisiblePeiFang_Data()
        };
        var resp = await _traceApi.SaveStationTaskStewingTime(postData);
        return resp.IsSuccessStatusCode ? resp.Content : new Response { Code = Convert.ToInt32(resp.Error.StatusCode), Message = resp.Error.Message };
    }


    /// <summary>
    /// 保存扫码任务记录到数据库
    /// </summary>
    /// <param name="hasGoods"></param>
    /// <returns></returns>
    public async Task<Response> SaveScanCodeDetailData(Base_StationTaskBom hasGoods, int taskId)
    {
        hasGoods.AGVCode = App.AGVCode;
        hasGoods.PackCode = App.PackBarCode;
        hasGoods.CreateUserID = App.UserId;
        hasGoods.StationID = App._StepStationSetting.Station.Id;
        var postData = new BomDataDTO
        {
            MainId = App.HisTaskData2.MainId,
            StationTaskId = taskId,
            BomData = hasGoods,
        };
        var resp = await _traceApi.Save_StationTaskBom(postData);
        return resp.IsSuccessStatusCode ? resp.Content : new Response { Code = Convert.ToInt32(resp.Error.StatusCode), Message = resp.Error.Message };

    }
    public async Task<Response> CheckBomUsed(Base_StationTaskBom hasGoods)
    {
        var resp = await _traceApi.CheckBomUsed(App._StepStationSetting.Step.Id, hasGoods.TracingType, hasGoods.TracingType == TracingTypeEnum.批追 ? hasGoods.UniBarCode : hasGoods.GoodsOuterCode);
        return resp.IsSuccessStatusCode ? resp.Content : new Response { Code = Convert.ToInt32(resp.Error.StatusCode), Message = resp.Error.Message };

    }

    /// <summary>
    /// 保存电子秤任务信息到数据库
    /// </summary>
    /// <param name="taskAnyLoad"></param>
    /// <returns></returns>
    public async Task<Response> SaveAnyLoadData(Base_StationTaskAnyLoad taskAnyLoad, int taskId)
    {
        taskAnyLoad.AGVCode = App.AGVCode;
        taskAnyLoad.CreateUserID = App.UserId;
        taskAnyLoad.PackCode = App.PackBarCode;
        taskAnyLoad.StationID = App._StepStationSetting.Station.Id;
        var postData = new AnyLoadDataDTO
        {
            MainId = App.HisTaskData2.MainId,
            StationTaskId = taskId,
            AnyLoadData = taskAnyLoad,
        };

        var resp = await _traceApi.Save_StationAnyLoad(postData);
        return resp.IsSuccessStatusCode ? resp.Content : new Response { Code = Convert.ToInt32(resp.Error.StatusCode), Message = resp.Error.Message };
    }

    public async Task<Response> SaveAccountCardData(Base_StationTaskScanAccountCard taskScanAccountCard, int taskId)
    {
        taskScanAccountCard.AGVCode = App.AGVCode;
        taskScanAccountCard.CreateUserID = App.UserId;
        taskScanAccountCard.PackCode = App.PackBarCode;
        taskScanAccountCard.StationID = App._StepStationSetting.Station.Id;
        taskScanAccountCard.StepID = App._StepStationSetting.Step.Id;

        var postData = new ScanAccountCardDataDTO
        {
            MainId = App.HisTaskData2.MainId,
            StationTaskId = taskId,
            ScanAccountCardData = taskScanAccountCard,
        };
        var resp = await _traceApi.Save_AccountCardData(postData);
        return resp.IsSuccessStatusCode ? resp.Content : new Response { Code = Convert.ToInt32(resp.Error.StatusCode), Message = resp.Error.Message };
    }

    public async Task<Response> Save_StationUserScan(Base_StationTaskUserInput taskUserScan, int taskId)
    {
        taskUserScan.AGVCode = App.AGVCode;
        taskUserScan.CreateUserID = App.UserId;
        taskUserScan.PackCode = App.PackBarCode;

        var postData = new UserInputDataDTO
        {
            StationTaskId = taskId,
            MainId = App.HisTaskData2.MainId,
            UserInputData = taskUserScan,
        };
        var resp = await _traceApi.Save_StationUserScan(postData);
        return resp.IsSuccessStatusCode ? resp.Content : new Response { Code = Convert.ToInt32(resp.Error.StatusCode), Message = resp.Error.Message };
    }

    public async Task<Response> Save_StationScanCollect(Base_StationTaskScanCollect taskScanCollect, int taskId)
    {

        taskScanCollect.AGVCode = App.AGVCode;
        taskScanCollect.CreateUserID = App.UserId;
        taskScanCollect.PackCode = App.PackBarCode;

        var postData = new ScanCollectDataDTO
        {
            StationTaskId = taskId,
            MainId = App.HisTaskData2.MainId,
            ScanCollectData = taskScanCollect,
        };
        var resp = await _traceApi.Save_StationScanCollect(postData);
        return resp.IsSuccessStatusCode ? resp.Content : new Response { Code = Convert.ToInt32(resp.Error.StatusCode), Message = resp.Error.Message };
    }


    /// <summary>
    /// 设置当前任务完成
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    public async Task<Response> SetStationCurTaskFinish(StationTaskDTO dto, int mainId)
    {
        dto.AGVCode = App.AGVCode;
        dto.UpdateUserID = App.UserId;
        dto.UpdateTime = DateTime.Now;
        dto.PackCode = App.PackBarCode;

        var resp = await _traceApi.SetStationCurTaskFinish(dto, mainId);
        return resp.IsSuccessStatusCode ? resp.Content : new Response { Code = Convert.ToInt32(resp.Error.StatusCode), Message = resp.Error.Message };
    }


    /// <summary>
    /// 记录放行AGV
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    public async Task<Response> SetStationCurTaskRunAGV(StationTaskDTO dto)
    {
        if (App.ActivePage != null && App.ActivePage.GetType() == typeof(LetGoPage) && !App.IsDealingRunAGV && !string.IsNullOrEmpty(App.PackBarCode) && App._RealtimePage.Vm.StationTaskDataList.Count > 0 && App._RealtimePage.Vm.StationTaskLeftTagDataList.Count > 0)
        {
            dto.AGVCode = App.AGVCode;
            dto.PackCode = App.PackBarCode;
            dto.CreateUserID = App.UserId;
            dto.StationID = App._StepStationSetting.Station.Id;
            dto.StepId = App._StepStationSetting.Step.Id;

            var resp = await _traceApi.SetStationCurTaskRunAGV(dto, App.HisTaskData2.MainId);
            return resp.IsSuccessStatusCode ? resp.Content : new Response { Code = Convert.ToInt32(resp.Error.StatusCode), Message = resp.Error.Message };
        }

        return new Response();
    }

    public async Task<Response> SetReWork(List<ReworkListDTO> reworkLists)
    {

        var resp = await _traceApi.SetReWork(reworkLists);
        return resp.IsSuccessStatusCode ? resp.Content : new Response { Code = Convert.ToInt32(resp.Error.StatusCode), Message = resp.Error.Message };
    }


    /// <summary>
    /// 保存拧紧数据到数据库
    /// </summary>
    /// <param name="screw"></param>
    /// <returns></returns>
    public async Task<Response> SaveScrewData(Base_StationTaskScrew screw, int taskId)
    {
        screw.AGVCode = App.AGVCode;
        screw.PackCode = App.PackBarCode;
        screw.CreateUserID = App.UserId;
        screw.StationID = App._StepStationSetting.Station.Id;
        var postData = new ScrewDataDTO
        {
            MainId = App.HisTaskData2.MainId,
            StationTaskId = taskId,
            ScrewData = screw,
        };

        var resp = await _traceApi.SaveScrewData(postData);
        return resp.IsSuccessStatusCode ? resp.Content : new Response { Code = Convert.ToInt32(resp.Error.StatusCode), Message = resp.Error.Message };
    }


    /// <summary>
    /// 设置当前拧紧任务完成
    /// </summary>
    /// <param name="screw"></param>
    /// <returns></returns>
    public async Task<Response> SetScrewTaskFinish(Base_StationTaskScrew screw)
    {
        screw.AGVCode = App.AGVCode;
        screw.PackCode = App.PackBarCode;
        screw.UpdateUserID = App.UserId;
        screw.UpdateTime = DateTime.Now;
        screw.StationID = App._StepStationSetting.Station.Id;

        var resp = await _traceApi.SetScrewTaskFinish(screw, App.HisTaskData2.MainId);
        return resp.IsSuccessStatusCode ? resp.Content : new Response { Code = Convert.ToInt32(resp.Error.StatusCode), Message = resp.Error.Message };
    }
    #endregion

    #region 【加载工位任务信息】

    /// <summary>
    /// 加载工位未完成的历史记录
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<Response<StaionHisDataDTO>> LoadStationTaskHistoryData(int id, string scanCode)
    {
        var resp = await _traceApi.LoadStationTaskHistoryData(id, scanCode);
        return resp.IsSuccessStatusCode ? resp.Content : new Response<StaionHisDataDTO> { Code = Convert.ToInt32(resp.Error.StatusCode), Message = resp.Error.Message };
    }
    #endregion

    #region 【获取涂胶开始时间】
    public async Task<Response<Proc_GluingInfo>> GetGluingStratTime(string packPN)
    {
        var resp = await _traceApi.GetGluingStartTime(packPN);
        return resp.IsSuccessStatusCode ? resp.Content : new Response<Proc_GluingInfo> { Code = Convert.ToInt32(resp.Error.StatusCode), Message = resp.Error.Message };
    }


    public async Task<Response<DateTime>> GetModuleInBoxEndTime(string packPN, string stepCode)
    {
        var resp = await _traceApi.GetModuleInBoxEndTime(packPN, stepCode);
        return resp.IsSuccessStatusCode ? resp.Content : new Response<DateTime> { Code = Convert.ToInt32(resp.Error.StatusCode), Message = resp.Error.Message };
    }


    #endregion

    #region 【获取工位任务完成时间】

    public async Task<Response<DateTime>> GetStepTaskEndTime(string packPN, string stepCode)
    {
        var resp = await _traceApi.GetStepTaskEndTime(packPN, stepCode);
        return resp.IsSuccessStatusCode ? resp.Content : new Response<DateTime> { Code = Convert.ToInt32(resp.Error.StatusCode), Message = resp.Error.Message };
    }

    #endregion

    #region 获取拧紧完成时间
    public async Task<Response<DateTime>> GetBoltGunEndTime(string packPN, string stepCode)
    {
        var resp = await _traceApi.GetBoltGunEndTime(packPN, stepCode);
        return resp.IsSuccessStatusCode ? resp.Content : new Response<DateTime> { Code = Convert.ToInt32(resp.Error.StatusCode), Message = resp.Error.Message };
    }
    #endregion

    /// <summary>
    /// 进站
    /// </summary>
    /// <param name="packPN"></param>
    /// <param name="stationTaskList"></param>
    /// <returns></returns>
    public async Task<Response> DealCatlMES_InStation(string packPN, List<StationTaskDTO> stationTaskList)
    {
        var dto = new InStationDataDTO
        {
            StepId = App._StepStationSetting.Step.Id,
            StationId = App._StepStationSetting.Station.Id,
            PackCode = packPN,
            AGVNo = App.AGVCode,
            StationTaskList = stationTaskList
        };
        var resp = await _traceApi.DealCatlMES_InStation(dto);
        return resp.IsSuccessStatusCode ? resp.Content : new Response { Code = Convert.ToInt32(resp.Error.StatusCode), Message = resp.Error.Message };
    }

    #region 【加载自动拧紧螺丝信息】

    public async Task<List<Proc_AutoBoltInfo_Detail>> LoadAutoBoltData()
    {
        var resp = await _traceApi.LoadAutoBoltData_Prev(App._StepStationSetting.Step.Id, App.PackBarCode);
        return resp.IsSuccessStatusCode ? resp.Content.Result : null!;
    }

    #endregion
    #region 【加载人工拧紧螺丝信息】

    public async Task<List<Proc_StationTask_BlotGunDetail>> LoadBoltData(int stationtaskid)
    {
        var resp = await _traceApi.LoadBoltData(App._StepStationSetting.Step.Id, stationtaskid, App.PackBarCode);
        return resp.IsSuccessStatusCode ? resp.Content.Result : null!;
    }

    #endregion

    #region 【AGV相关信息】

    /// <summary>
    /// 获取当前工位停靠的AGV信息
    /// </summary>
    /// <returns></returns>
    public async Task<Response<Proc_AGVStatus>> LoadStationCurrentAGV()
    {
        var resp = await _traceApi.LoadStationCurrentAGV(App._StepStationSetting.StationCode);
        return resp.IsSuccessStatusCode ? resp.Content : new Response<Proc_AGVStatus> { Code = Convert.ToInt32(resp.Error.StatusCode), Message = resp.Error.Message };
    }

    /// <summary>
    /// 处理扫描Pack码后的数据逻辑
    /// 1、Pack信息校验
    /// 2、AGV信息校验
    /// 3、工位配方数据加载
    /// 4、此Pack工位记录校验
    /// </summary>
    /// <param name="scanCodeContext"></param>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<Response<StationPack_AGV_Task_Record_DTO>> CheckPack_AGV_Task_Record(string scanCodeContext, int productId, int stepId)
    {
        var resp = await _traceApi.CheckPack_AGV_Task_Record(productId, stepId, App._StepStationSetting.Station.Code, App.AGVCode, scanCodeContext);
        return resp.IsSuccessStatusCode ? resp.Content : new Response<StationPack_AGV_Task_Record_DTO> { Code = Convert.ToInt32(resp.Error.StatusCode), Message = resp.Error.Message };
    }

    //public async Task<Response<List<StationConfig>>> LoadStationAllPeiFangData()
    //{
    //    var resp = await traceApi.LoadStationAllPeiFangData(App._StepStationSetting.StationCode);
    //    return resp.IsSuccessStatusCode ? resp.Content : new Response<List<StationConfig>> { Code = Convert.ToInt32(resp.Error.StatusCode), Message = resp.Error.Message };
    //}

    /// <summary>
    /// 绑定或解绑agv与pack
    /// </summary>
    /// <param name="dTO"></param>
    /// <returns></returns>
    public async Task<Response<string>> BingAGVPack(BindAgvDTO dTO)
    {
        var resp = await _traceApi.BingAgv(dTO);
        return resp.IsSuccessStatusCode ? resp.Content : new Response<string> { Code = Convert.ToInt32(resp.Error.StatusCode), Message = resp.Error.Message };
    }

    public async Task<Response> PullStepBack(string agvNo, string packBarCode, string outCode)
    {
        var resp = await _traceApi.PullStepBack(agvNo, App._StepStationSetting.Step.Code, packBarCode, outCode);
        return resp.IsSuccessStatusCode ? resp.Content : new Response { Code = Convert.ToInt32(resp.Error.StatusCode), Message = resp.Error.Message };
    }


    public async Task<Response<string>> BingAGVPAck_SBox(BindAgvDTO dto)
    {
        var resp = await _traceApi.BingAgv_SBox(dto);
        return resp.IsSuccessStatusCode ? resp.Content : new Response<string> { Code = Convert.ToInt32(resp.Error.StatusCode), Message = resp.Error.Message };
    }

    /// <summary>
    /// 放行AGV
    /// </summary>
    /// <param name="agvCode"></param>
    /// <param name="releaseType">1 放行 2 急停 3 解除急停</param>
    /// <returns></returns>
    public async Task<Response> RunAGV(string agvCode, int releaseType)
    {
        var resp = await _traceApi.RunAGV(agvCode, releaseType);
        return resp.IsSuccessStatusCode ? resp.Content : new Response { Code = Convert.ToInt32(resp.Error.StatusCode), Message = resp.Error.Message };
    }

    public async Task<Response> RunAGVByStepCode(string stepCode, int releaseType)
    {
        var resp = await _traceApi.RunAGVByStepCode(stepCode, releaseType);
        return resp.IsSuccessStatusCode ? resp.Content : new Response { Code = Convert.ToInt32(resp.Error.StatusCode), Message = resp.Error.Message };
    }

    #endregion


    #region 错误日志

    /// <summary>
    /// 更新和添加错误日志
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    public async Task<List<Alarm>> OccurAlarm(AlarmDTO dto)
    {
        dto.StepCode = App._StepStationSetting.Step?.Code;
        var resp = await _traceApi.Occur(dto);
        return resp.IsSuccessStatusCode ? resp.Content.Result : null;
    }

    /// <summary>
    /// 结束此工位的全部错误日志
    /// </summary>
    /// <param name="dtos"></param>
    /// <returns></returns>
    public async Task<List<Alarm>> ClearALLAlarm(List<AlarmDTO> dtos)
    {
        dtos.First().StepCode = App._StepStationSetting.Step?.Code;
        var resp = await _traceApi.ClearALL(dtos);
        return resp.IsSuccessStatusCode ? resp.Content.Result : null;
    }
    #endregion

    /// <summary>
    /// 校验权限
    /// </summary>
    /// <param name="account"></param>
    /// <param name="modlename"></param>
    /// <returns></returns>
    public async Task<(Boolean, string)> CheckPowe(string account, string modlename)
    {
        var resp = await _traceApi.CheckPower(account, modlename);
        return resp.IsSuccessStatusCode ? resp.Content : (false, resp.Error.Message);
    }
    /// <summary>
    /// NG下线
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    public async Task<Response<bool>> ProductOutLine(Proc_Product_Offline dto)
    {
        var resp = await _traceApi.OfflineAdd(dto);
        return resp.IsSuccessStatusCode ? resp.Content : new Response<bool> { Code = Convert.ToInt32(resp.Error.StatusCode), Message = resp.Error.Message };
    }

    public async Task<Response<Base_StationTaskBom>> GetRange(int? StationTaskId)
    {
        var resp = await _traceApi.GetOutScanRange(StationTaskId.ToString());
        return resp.IsSuccessStatusCode ? resp.Content : new Response<Base_StationTaskBom> { Code = Convert.ToInt32(resp.Error.StatusCode), Message = resp.Error.Message };
    }

    /// <summary>
    /// 校验二维码输入
    /// </summary>
    /// <param name="ScanData"></param>
    /// <returns></returns>
    public async Task<Response<bool>> GetScanCollectInfo(string ScanData)
    {
        var resp = await _traceApi.GetScanCollectInfo(App._StepStationSetting.Step.Id, App.PackBarCode, ScanData);
        return resp.IsSuccessStatusCode ? resp.Content : new Response<bool> { Code = Convert.ToInt32(resp.Error.StatusCode), Message = resp.Error.Message };
    }

    public async Task<PackTaskMainDataDTO> LoadStepPackTaskMainData(DateTime searchBeginTime, DateTime searchEndTime, string stationCode)
    {
        var resp = await _traceApi.LoadStepPackTaskMainData(stationCode, searchBeginTime.ToString("yyyy-MM-dd HH:mm:ss"), searchEndTime.ToString("yyyy-MM-dd HH:mm:ss"));
        return resp.IsSuccessStatusCode ? resp.Content.Result : null!;
    }

    public async Task<StepStationDTO> LoadStepStationData(string packCode, int productId)
    {
        var resp = await _traceApi.LoadStepStationData(packCode, productId.ToString(), App._StepStationSetting.StationCode);
        return resp.IsSuccessStatusCode ? resp.Content.Result : null!;
    }

    //获取时间记录
    public async Task<Response<Proc_StationTask_TimeRecord>> GetRecordTime(string packCode, string TimeFlag)
    {
        var resp = await _traceApi.GetRecordTime(packCode, TimeFlag);
        return resp.IsSuccessStatusCode ? resp.Content : new Response<Proc_StationTask_TimeRecord>() { Code = 500, Message = resp.Error.Message };
    }

    public async Task<Response> SaveTimeRecord(SaveTimeDTO dto)
    {
        var resp = await _traceApi.SaveRecordTime(dto);
        return resp.IsSuccessStatusCode ? resp.Content : new Response() { Code = 500, Message = resp.Error.Message };

    }

    public async Task<FSharpResult<(string, int), string>> GetCurrentStationAndLevel(string stationFlag, string packCode)
    {

        var resp = await _traceApi.GetCurrentStationAndLevel(stationFlag, packCode);

        if (resp.IsSuccessStatusCode)
        {
            if (resp.Content.Code != 200)
            {
                return resp.Content.Message.ToErrResult<(string, int), string>();
            }
            App._StepStationSetting.StationCode = resp.Content.Result.Item1;
            //App._StepStationSetting.CurrentLevel = resp.Content.Result.Item2;

            return resp.Content.Result.ToOkResult<(string, int), string>();
        }

        return resp.Error.Message.ToErrResult<(string, int), string>();
    }

    public async Task<FSharpResult<StationConfig, string>> LoadFormula(string stationCode, string productPn)
    {
        var resp = await _traceApi.LoadFormula(stationCode, productPn);

        if (resp.IsSuccessStatusCode)
        {
            if (resp.Content.Code != 200)
            {
                return resp.Content.Message.ToErrResult<StationConfig, string>();
            }

            App._StepStationSetting.Station = resp.Content.Result.Station;
            App._StepStationSetting.Step = resp.Content.Result.Step;
            App._StationConfig = resp.Content.Result;
            return resp.Content.Result.ToOkResult<StationConfig, string>();
        }

        return resp.Error.Message.ToErrResult<StationConfig, string>();
    }

    public async Task<FSharpResult<StationTaskHistoryDTO, string>> CreateOrLoadTraceInfo(string packCode, string stationCode, string productCode)
    {

        var resp = await _traceApi.CreateOrLoadTraceInfo(packCode, stationCode, productCode);
        if (resp.IsSuccessStatusCode)
        {
            if (resp.Content.Code != 200)
            {
                return resp.Content.Message.ToErrResult<StationTaskHistoryDTO, string>();
            }
            return resp.Content.Result.ToOkResult<StationTaskHistoryDTO, string>();
        }

        return resp.Error.Message.ToErrResult<StationTaskHistoryDTO, string>();
    }

    public async Task<FSharpResult<bool, string>> CheckVectorBindAndFlowOrder(string packCode, int vectorCode, string stationCode, string VectorStation, string productPn, string packOutCode)
    {
        if (App._StepStationSetting.IsDebug)
        {
            return true.ToOkResult<bool, string>();
        }

        var resp = await _traceApi.CheckVectorBindAndFlowOrder(packCode, vectorCode, stationCode, VectorStation, productPn, packOutCode);
        if (resp.IsSuccessStatusCode)
        {
            if (resp.Content.Code != 200)
            {
                return resp.Content.Message.ToErrResult<bool, string>();
            }
            return true.ToOkResult<bool, string>();
        }

        return resp.Error.Message.ToErrResult<bool, string>();
    }

    public async Task<FSharpResult<bool, string>> CheckFlowOrder(string packCode, string stationCode, string productPn)
    {
        if (App._StepStationSetting.IsDebug)
        {
            return true.ToOkResult<bool, string>();
        }

        var resp = await _traceApi.CheckFlowOrder(packCode, stationCode, productPn);
        if (resp.IsSuccessStatusCode)
        {
            if (resp.Content.Code != 200)
            {
                return resp.Content.Message.ToErrResult<bool, string>();
            }
            return true.ToOkResult<bool, string>();
        }

        return resp.Error.Message.ToErrResult<bool, string>();
    }


    public async Task<Response<IList<AutoBlotInfo>>> LoadAutoTightenData(string packCode, TightenReworkType AutoTightenType, int ScrewNum)
    {
        var resp = await _traceApi.LoadAutoTightenData(packCode, AutoTightenType, ScrewNum);
        return resp.IsSuccessStatusCode ? resp.Content : new Response<IList<AutoBlotInfo>>() { Code = 500, Message = resp.Error.Message };
    }

    public async Task<Response> SaveRepairData(TightenReworkDataDto dto)
    {
        var resp = await _traceApi.SaveRepairData(dto);
        return resp.IsSuccessStatusCode ? resp.Content : new Response() { Code = 500, Message = resp.Error.Message };
    }

    public async Task<Response> MakeTaskComplateForce(StationTaskDTO dto, int mainId)
    {
        var resp = await _traceApi.MakeTaskComplateForce(dto, mainId);
        return resp.IsSuccessStatusCode ? resp.Content : new Response() { Code = 500, Message = resp.Error.Message };
    }

    public async Task<Response> MakeTaskListComplateForce(IList<(int, string)> dto, int mainId)
    {
        var resp = await _traceApi.MakeTaskListComplateForce(dto, mainId);
        return resp.IsSuccessStatusCode ? resp.Content : new Response() { Code = 500, Message = resp.Error.Message };
    }


    public async Task<List<ReWorkData>> LoadReworkList(LoadReworkDataDto dto)
    {
        try
        {
            var result = await _traceApi.LoadReworkList(dto);
            if (result.Code != 200)
            {
                await _mediator.Publish(new AlarmSYSNotification() { Code = AlarmCode.返工错误, Name = AlarmCode.返工错误.ToString(), Module = AlarmModule.DESOUTTER_MODULE, Description = $"{result.Message}" });
                return null;
            }

            return result.Result;
        }
        catch (Exception ex)
        {
            await _mediator.Publish(new AlarmSYSNotification() { Code = AlarmCode.系统运行错误, Name = AlarmCode.系统运行错误.ToString(), Module = AlarmModule.DESOUTTER_MODULE, Description = $"接口调用异常：{ex.Message}" });
            return null;
        }
    }

    public async Task<Response> MakeRework(List<WorkRecord> workRecords)
    {
        var result = new Response();
        try
        {
            result = await _traceApi.MakeRework(workRecords);
        }
        catch (Exception ex)
        {
            await _mediator.Publish(new AlarmSYSNotification() { Code = AlarmCode.系统运行错误, Name = AlarmCode.系统运行错误.ToString(), Module = AlarmModule.DESOUTTER_MODULE, Description = $"接口调用异常：{ex.Message}" });
            result.Code = 400;
            result.Message = $" 接口调用异常：{ex.Message}";
        }
        return result;
    }

    public async Task<Response> SetRecordWorking(int recordId)
    {
        var result = new Response();
        try
        {
            result = await _traceApi.SetRecordWorking(recordId);
        }
        catch (Exception ex)
        {
            await _mediator.Publish(new AlarmSYSNotification() { Code = AlarmCode.系统运行错误, Name = nameof(AlarmCode.系统运行错误), Module = AlarmModule.SERVER_MODULE, Description = $"接口调用异常：{ex.Message}" });
            result.Code = 400;
            result.Message = $" 接口调用异常：{ex.Message}";
        }
        return result;
    }
    public async Task<Response<UploadCATLData>> GetUploadDataAgain(string packCode, string stationCode)
    {
        var result = new Response<UploadCATLData>();
        try
        {
            result = await _traceApi.UploadDataAgain(packCode, stationCode);
        }
        catch (Exception ex)
        {
            await _mediator.Publish(new AlarmSYSNotification() { Code = AlarmCode.系统运行错误, Name = nameof(AlarmCode.系统运行错误), Module = AlarmModule.SERVER_MODULE, Description = $"接口调用异常：{ex.Message}" });
            result.Code = 400;
            result.Message = $" 接口调用异常：{ex.Message}";
        }
        return result;
    }
    public async Task<Response<string>> GetProductPnFormDb(string packCode)
    {
        var result = new Response<string>();
        try
        {
            result = await _traceApi.GetProductPnFormDb(packCode);
        }
        catch (Exception ex)
        {
            await _mediator.Publish(new AlarmSYSNotification { Code = AlarmCode.系统运行错误, Name = nameof(AlarmCode.系统运行错误), Module = AlarmModule.SERVER_MODULE, Description = $"接口调用异常：{ex.Message}" });
            result.Code = 400;
            result.Message = $" 接口调用异常：{ex.Message}";
        }
        return result;
    }

    public async Task<Response> SaveScrewNGResetRecord(Proc_ScrewNGResetRecordSaveDTO dto)
    {
        var resp = await _traceApi.SaveScrewNGResetRecord(dto);
        return resp.IsSuccessStatusCode ? resp.Content : new Response { Code = 500, Message = resp.Error.Message };
    }
    
    public async Task<Response> SaveTightenByImageData(ScrewDataDTO dto)
    {
        var result = new Response();
        try
        {
            result = await _traceApi.SaveTightenByImageData(dto);
        }
        catch (Exception ex)
        {
            await _mediator.Publish(new AlarmSYSNotification() { Code = AlarmCode.系统运行错误, Name = AlarmCode.系统运行错误.ToString(), Module = AlarmModule.DESOUTTER_MODULE, Description = $"接口调用异常：{ex.Message}" });
            result.Code = 400;
            result.Message = $" 接口调用异常：{ex.Message}";
        }
        return result;
    }
    public async Task<Response> SaveNgData(ScrewDataDTO dto)
    {
        var result = new Response();
        try
        {
            result = await _traceApi.SaveNgData(dto);
        }
        catch (Exception ex)
        {
            await _mediator.Publish(new AlarmSYSNotification() { Code = AlarmCode.系统运行错误, Name = AlarmCode.系统运行错误.ToString(), Module = AlarmModule.DESOUTTER_MODULE, Description = $"接口调用异常：{ex.Message}" });
            result.Code = 400;
            result.Message = $" 接口调用异常：{ex.Message}";
        }
        return result;
    }
    public async Task<Response> SaveReverseData(ScrewDataDTO dto)
    {
        var result = new Response();
        try
        {
            result = await _traceApi.SaveReverseData(dto);
        }
        catch (Exception ex)
        {
            await _mediator.Publish(new AlarmSYSNotification() { Code = AlarmCode.系统运行错误, Name = AlarmCode.系统运行错误.ToString(), Module = AlarmModule.DESOUTTER_MODULE, Description = $"接口调用异常：{ex.Message}" });
            result.Code = 400;
            result.Message = $" 接口调用异常：{ex.Message}";
        }
        return result;
    }
}