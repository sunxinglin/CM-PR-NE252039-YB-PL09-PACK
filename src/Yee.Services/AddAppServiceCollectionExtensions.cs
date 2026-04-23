using TimedTask.ClearHisData;

using Yee.Services.AGV;
using Yee.Services.AlarmMgmt;
using Yee.Services.AutomaticStation;
using Yee.Services.BaseData;
using Yee.Services.Helper;
using Yee.Services.HistoryData;
using Yee.Services.Production;
using Yee.Services.ProductionRecord;

namespace Yee.Services;
public static class AddAppServiceCollectionExtensions
{
    public static IServiceCollection AddApps(this IServiceCollection services)
    {
        services.AddScoped<CategoryTypeService>();
        services.AddScoped<DictionaryService>();
        services.AddScoped<DictionaryDetailService>();
        services.AddScoped<ProductService>();
        services.AddScoped<FlowService>();
        services.AddScoped<StepService>();
        services.AddScoped<FlowStepMappingService>();
        services.AddScoped<StationService>();
        services.AddScoped<StationTaskBomService>();
        services.AddScoped<SaveStationDataService>();

        services.AddScoped<ProResourceService>();
        services.AddScoped<PackService>();
        services.AddScoped<StationTaskService>();
        services.AddScoped<StationTaskScrewService>();
        services.AddScoped<StationTaskResourceService>();
        services.AddScoped<Base_StationTaskAnyLoadService>();
        services.AddScoped<Base_StationTaskModuleInBoxService>();
        services.AddScoped<Base_StationTaskStewingTimeService>();
        services.AddScoped<Base_StationTaskPressureService>();
        services.AddScoped<Base_StationTaskRecordTimeService>();
        services.AddScoped<Base_StationTaskUserInputService>();
        services.AddScoped<Base_StationTaskScanCollectService>();
        services.AddScoped<Base_StationTaskGluingTimeService>();
        services.AddScoped<Base_StationTaskLeakService>();
        services.AddScoped<StationTaskImporntService>();
        services.AddScoped<Proc_StationTask_Main_StatusQueryService>();
        services.AddScoped<AGVService>();
        services.AddScoped<IAGVResolver, AGVService>();
        services.AddScoped<StationTaskTightenByImageService>();
        services.AddScoped<Proc_outerCodeCheckRecordService>();

        services.AddScoped<HistoryData_APIService>();
        services.AddSingleton<IHistoryDataResolver, HistoryDataService>();
        services.AddScoped<AlarmService>();
        services.AddScoped<AGVService>();
        services.AddScoped<APIHelper>();
        services.AddScoped<Proc_Product_OffLineService>();
        services.AddScoped<ProductTraceBackService>();
        services.AddScoped<StationTask_BlotGunDetailService>();
        services.AddScoped<StationTask_GlueDetailService>();
        services.AddScoped<StationTask_BomDetailService>();
        services.AddScoped<Base_StationTaskAutoService>();
        services.AddScoped<Base_StationTaskGlueService>();
        services.AddScoped<Base_StationTaskScanAccountCardService>();
        services.AddScoped<Proc_GluingtimeService>();
        services.AddScoped<Proc_Product_NG_StatisticsService>();

        services.AddScoped<StationTaskMainService>();
        services.AddScoped<ProcRecordCheckPowerLogService>();
        services.AddScoped<Proc_StationTask_PeiFangService>();
        services.AddScoped<Proc_PressureInfosService>();
        //services.AddScoped<Proc_PressureStripInfosService>();
        services.AddScoped<Proc_ModuleInBoxInfoService>();
        services.AddScoped<Proc_ModuleInBoxRecordService>();
        services.AddScoped<Base_StationTaskTightenReworkService>();
        services.AddScoped<Proc_StationTask_ScanCollectService>();
        services.AddScoped<Proc_StationTask_LeakDetailService>();

        //自动站
        services.AddScoped<AutomicCommonService>();
        services.AddScoped<AutoTightenService>();
        services.AddScoped<ExternalAutoTightenDataService>();
        services.AddScoped<AutoGlueService>();
        services.AddScoped<ModuleInBoxService>();
        services.AddScoped<AutoPressureService>();

        services.AddScoped<LowerBoxGlueService>();
        services.AddScoped<Base_StationTask_LowerBoxGlueService>();

        services.AddScoped<Proc_ScrewNGResetRecordService>();
        services.AddScoped<Base_ScrewNGResetConfigService>();

        return services;
    }
}
