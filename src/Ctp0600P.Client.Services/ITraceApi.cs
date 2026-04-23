using AsZero.Core.Entities;
using AsZero.Core.Services.Repos;

using Refit;

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

namespace Ctp0600P.Client.Services;

public interface ITraceApi
{
    [Get("/api/Account/Check_CreateAccountCard")]
    public Task<IApiResponse<Response<User>>> Check_CreateAccountCardAsync(string card);

    [Get("/api/WPFClientData/GetUploadCATLData")]
    public Task<IApiResponse<Response<UploadCATLData>>> GetUploadCATLData(string packCode, int stepId, int stationId);

    [Post("/api/Account/Record_CheckPower_Log")]
    public Task<IApiResponse> Record_CheckPower_Log(RecordCheckPowerDTO dto);

    [Get("/api/Account/ValidateUser")]
    public Task<IApiResponse<Response<User>>> ValidateUser(string account, string password);

    [Get("/api/Account/CheckuserClaims")]
    public Task<IApiResponse<Response<ClaimEntity>>> CheckuserClaims(string account, string password);

    [Get("/api/WPFClientData/LoadStationProResourceConfig")]
    public Task<IApiResponse<Response<List<Base_ProResource>>>> LoadStationProResourceConfig(string stationCode, int proResourceType);

    [Post("/api/WPFClientData/SaveStationEquipmentConifgData")]
    public Task<IApiResponse<Response<List<Base_ProResource>>>> SaveStationEquipmentConifgData(List<Base_ProResource> resList, ProResourceTypeEnum resourceType, string stationCode, string Operator);

    [Post("/api/WPFClientData/SaveStationTaskCheckTimeOut")]
    public Task<IApiResponse<Response>> SaveStationTaskCheckTimeOut(CheckTimeOutDataDTO proc);

    [Post("/api/WPFClientData/SaveStationTaskStewingTime")]
    public Task<IApiResponse<Response>> SaveStationTaskStewingTime(StewingTimeDataDTO proc);

    [Post("/api/WPFClientData/Save_StationTaskBom")]
    public Task<IApiResponse<Response>> Save_StationTaskBom(BomDataDTO input);

    [Get("/api/WPFClientData/CheckBomUsed")]
    public Task<IApiResponse<Response>> CheckBomUsed(int? stepId, TracingTypeEnum TracingType, string? Code);

    [Post("/api/WPFClientData/Save_StationAnyLoad")]
    public Task<IApiResponse<Response>> Save_StationAnyLoad(AnyLoadDataDTO input);

    [Post("/api/WPFClientData/Save_AccountCardData")]
    public Task<IApiResponse<Response>> Save_AccountCardData(ScanAccountCardDataDTO scanAccountCard);

    [Post("/api/WPFClientData/Save_StationUserScan")]
    public Task<IApiResponse<Response>> Save_StationUserScan(UserInputDataDTO input);

    [Post("/api/WPFClientData/Save_StationScanCollect")]
    public Task<IApiResponse<Response>> Save_StationScanCollect(ScanCollectDataDTO input);

    [Post("/api/WPFClientData/SetStationCurTaskFinish")]
    public Task<IApiResponse<Response>> SetStationCurTaskFinish(StationTaskDTO taskDTO, int mainId);

    [Post("/api/WPFClientData/SetStationCurTaskRunAGV")]
    public Task<IApiResponse<Response>> SetStationCurTaskRunAGV(StationTaskDTO taskDTO, int mainId);

    [Post("/api/WPFClientData/SetReWork")]
    public Task<IApiResponse<Response>> SetReWork(List<ReworkListDTO> reworkLists);

    [Post("/api/WPFClientData/SaveScrewData")]
    public Task<IApiResponse<Response>> SaveScrewData(ScrewDataDTO screw);


    [Post("/api/WPFClientData/SetScrewTaskFinish")]
    public Task<IApiResponse<Response>> SetScrewTaskFinish(Base_StationTaskScrew screw, int mainId);


    [Get("/api/StationTask/LoadStationTaskHistoryData")]
    public Task<IApiResponse<Response<StaionHisDataDTO>>> LoadStationTaskHistoryData(int stepId, string scanCode);


    [Get("/api/WPFClientData/GetGluingStartTime")]
    public Task<IApiResponse<Response<Proc_GluingInfo>>> GetGluingStartTime(string packPN);


    [Get("/api/WPFClientData/GetModuleInBoxEndTime")]
    public Task<IApiResponse<Response<DateTime>>> GetModuleInBoxEndTime(string packPN, string stepCode);


    [Get("/api/WPFClientData/GetStepTaskEndTime")]
    public Task<IApiResponse<Response<DateTime>>> GetStepTaskEndTime(string packPN, string stepCode);

    [Get("/api/WPFClientData/GetBoltGunEndTime")]
    public Task<IApiResponse<Response<DateTime>>> GetBoltGunEndTime(string packPN, string stepCode);

    [Post("/api/WPFClientData/DealCatlMES_InStation")]
    public Task<IApiResponse<Response>> DealCatlMES_InStation(InStationDataDTO inStationData);

    [Get("/api/WPFClientData/LoadAutoBoltData_Prev")]
    public Task<IApiResponse<Response<List<Proc_AutoBoltInfo_Detail>>>> LoadAutoBoltData_Prev(int stepId, string packCode);

    [Get("/api/WPFClientData/LoadBoltData")]
    public Task<IApiResponse<Response<List<Proc_StationTask_BlotGunDetail>>>> LoadBoltData(int stepId, int stationtaskid, string packCode);

    [Get("/api/AGV/LoadStationCurrentAGV")]
    public Task<IApiResponse<Response<Proc_AGVStatus>>> LoadStationCurrentAGV(string code);

    [Get("/api/AGV/CheckPack_AGV_Task_Record")]
    public Task<IApiResponse<Response<StationPack_AGV_Task_Record_DTO>>> CheckPack_AGV_Task_Record(int productId, int stepId, string stationCode, string agvNo, string packPN);

    [Get("/api/AGV/LoadStationAllPeiFangData")]
    public Task<IApiResponse<Response<List<StationConfig>>>> LoadStationAllPeiFangData(string stationCode);

    [Post("/api/AGV/BingAgv")]
    public Task<IApiResponse<Response<string>>> BingAgv(BindAgvDTO dTO);

    [Get("/api/AGV/PullStepBack")]
    public Task<IApiResponse<Response>> PullStepBack(string agvNo, string stationCode, string packBarCode, string outCode);

    [Post("/api/AGV/BingAgv_SBox")]
    public Task<IApiResponse<Response<string>>> BingAgv_SBox(BindAgvDTO dTO);

    [Get("/api/AGV/RunAGV")]
    public Task<IApiResponse<Response>> RunAGV(string agvCode, int releaseType);

    [Get("/api/AGV/RunAGVByStepCode")]
    public Task<IApiResponse<Response>> RunAGVByStepCode(string stepCode, int releaseType);

    [Post("/api/Alarm/Occur")]
    public Task<IApiResponse<Response<List<Alarm>>>> Occur(AlarmDTO dto);

    [Post("/api/Alarm/ClearALL")]
    public Task<IApiResponse<Response<List<Alarm>>>> ClearALL(List<AlarmDTO> dtos);

    [Get("/api/Account/CheckPower")]
    public Task<IApiResponse<(bool, string)>> CheckPower(string account, string modlename);

    [Post("/api/Proc_Product_NG_Statistics/Add")]
    public Task<IApiResponse<Response<bool>>> OfflineAdd(Proc_Product_Offline entity);

    [Get("/api/WPFClientData/GetOutScanRange")]
    public Task<IApiResponse<Response<Base_StationTaskBom>>> GetOutScanRange(string StationTaskId);

    [Get("/api/WPFClientData/GetScanCollectInfo")]
    public Task<IApiResponse<Response<bool>>> GetScanCollectInfo(int stepid, string packpn, string scanCollectData);

    [Get("/api/WPFClientData/LoadStepPackTaskMainData")]
    public Task<IApiResponse<Response<PackTaskMainDataDTO>>> LoadStepPackTaskMainData(string stationCode, string beginTime, string endTime);

    [Get("/api/Station/LoadStepStationData")]
    public Task<IApiResponse<Response<StepStationDTO>>> LoadStepStationData(string packCode, string productId, string stationCode);

    [Get("/api/WPFClientData/GetRecordTime")]
    public Task<IApiResponse<Response<Proc_StationTask_TimeRecord>>> GetRecordTime(string packCode, string TimeFlag);

    [Post("/api/WPFClientData/SaveRecordTime")]
    public Task<IApiResponse<Response>> SaveRecordTime(SaveTimeDTO dto);

    [Get("/api/WPFClientData/GetCurrentStationAndLevel")]
    public Task<IApiResponse<Response<(string, int)>>> GetCurrentStationAndLevel(string stationFlag, string packCode);

    [Get("/api/AGV/LoadFormula")]
    public Task<IApiResponse<Response<StationConfig>>> LoadFormula(string stationCode, string ProductPn);


    [Post("/api/WPFClientData/CreateOrLoadTraceInfo")]
    public Task<IApiResponse<Response<StationTaskHistoryDTO>>> CreateOrLoadTraceInfo(string packCode, string stationCode, string productCode);

    [Get("/api/WPFClientData/CheckVectorBindAndFlowOrder")]
    public Task<IApiResponse<Response>> CheckVectorBindAndFlowOrder(string packCode, int vectorCode, string stationCode, string VectorStation, string productPn, string packOutCode);

    [Get("/api/WPFClientData/CheckFlowOrder")]
    public Task<IApiResponse<Response>> CheckFlowOrder(string packCode, string stationCode, string productPn);

    [Get("/api/WPFClientData/LoadAutoTightenData")]
    public Task<IApiResponse<Response<IList<AutoBlotInfo>>>> LoadAutoTightenData(string packCode, TightenReworkType AutoTightenType, int ScrewNum);

    [Post("/api/WPFClientData/SaveRepairData")]
    public Task<IApiResponse<Response>> SaveRepairData(TightenReworkDataDto dto);

    [Post("/api/WPFClientData/MakeTaskComplateForce")]
    public Task<IApiResponse<Response>> MakeTaskComplateForce(StationTaskDTO dto, int mainId);

    [Post("/api/WPFClientData/MakeTaskListComplateForce")]
    public Task<IApiResponse<Response>> MakeTaskListComplateForce(IList<(int, string)> dto, int mainId);

    [Post("/api/WPFClientData/LoadReworkList")]
    public Task<Response<List<ReWorkData>>> LoadReworkList(LoadReworkDataDto dto);

    [Post("/api/WPFClientData/MakeRework")]
    public Task<Response> MakeRework(List<WorkRecord> works);

    [Post("/api/WPFClientData/SetRecordWorking")]
    public Task<Response> SetRecordWorking(int recordId);

    [Get("/api/WPFClientData/UploadDataAgain")]
    public Task<Response<UploadCATLData>> UploadDataAgain(string packCode, string stationCode);

    [Get("/api/WPFClientData/GetProductPnFormDb")]
    public Task<Response<string>> GetProductPnFormDb(string packCode);

    [Post("/api/AGV/AGVArrived")]
    public Task<Response> AGVArrived(AGVMessage dto);

    [Post("/api/AGV/AGVLeaved")]
    public Task<Response> AGVLeaved(AGVMessage dto);

    [Post("/api/Proc_ScrewNGResetRecord/Save")]
    public Task<IApiResponse<Response>> SaveScrewNGResetRecord(Proc_ScrewNGResetRecordSaveDTO dto);
    
    [Post("/api/WPFClientData/SaveTightenByImageData")]
    public Task<Response> SaveTightenByImageData(ScrewDataDTO dto);
    [Post("/api/WPFClientData/SaveNgData")]
    public Task<Response> SaveNgData(ScrewDataDTO dto);
    [Post("/api/WPFClientData/SaveReverseData")]
    public Task<Response> SaveReverseData(ScrewDataDTO dto);


    [Post("/api/WPFClientData/Save_StationLeak")]
    public Task<IApiResponse<Response>> Save_StationLeak(LeakDataDTO input);
}