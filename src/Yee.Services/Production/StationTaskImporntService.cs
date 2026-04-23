using AsZero.Core.Entities;
using AsZero.Core.Services.Sys_Logs;
using AsZero.DbContexts;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

using OfficeOpenXml;

using Yee.Common.Library.CommonEnum;
using Yee.Entitys.DBEntity.Production;
using Yee.Entitys.DTOS;
using Yee.Entitys.Production;
using Yee.Services.BaseData;
using Yee.Tools;

namespace Yee.Services.Production
{
    public class StationTaskImporntService
    {
        private readonly AsZeroDbContext dbContext;
        private readonly ProductService productService;
        private readonly StationService stationService;
        private readonly StationTaskBomService stationBomService;
        private readonly ProResourceService proResourceService;
        private readonly StationTaskService stationTaskService;
        private readonly FlowService flowService;
        private readonly StepService stepService;
        private readonly FlowStepMappingService flowStepMappingService;
        private readonly Base_StationTaskAnyLoadService baseStationTaskAnyLoadService;
        private readonly StationTaskScrewService stationTaskScrewService;
        private readonly StationTaskResourceService stationTaskResourceService;
        private readonly Base_StationTaskAutoService base_StationTaskAutoService;
        private readonly SysLogService sys_LogService;
        private readonly Base_StationTaskGlueService base_StationTaskGlueService;
        private readonly Base_StationTaskGluingTimeService baseStationTaskGluingTimeService;
        private readonly Base_StationTaskUserInputService Base_StationTaskUserInputService;
        private readonly Base_StationTaskScanCollectService base_StationTaskScanCollectService;
        private readonly Base_StationTaskPressureService base_StationTaskPressureService;
        private readonly Base_StationTaskModuleInBoxService base_StationTaskModuleInBoxService;
        private readonly Base_StationTaskStewingTimeService base_StationTaskStewingTimeService;

        public StationTaskImporntService(AsZeroDbContext dbContext,
            ProductService productService,
            StationService stationService,
            StationTaskBomService stationBomService,
            ProResourceService proResourceService,
            StationTaskService stationTaskService,
            FlowService flowService,
            StepService stepService,
            FlowStepMappingService flowStepMappingService,
            Base_StationTaskAnyLoadService baseStationTaskAnyLoadService,
            StationTaskScrewService stationTaskScrewService,
            StationTaskResourceService stationTaskResourceService,
            Base_StationTaskAutoService base_StationTaskAutoService,
            SysLogService sys_LogService,
            Base_StationTaskGlueService base_StationTaskGlueService,
            Base_StationTaskGluingTimeService baseStationTaskGluingTimeService,
            Base_StationTaskUserInputService Base_StationTaskUserInput,
            Base_StationTaskScanCollectService base_StationTaskScanCollect,
            Base_StationTaskPressureService base_StationTaskPressureService,
            Base_StationTaskModuleInBoxService base_StationTaskModuleInBoxService,
            Base_StationTaskStewingTimeService base_StationTaskStewingTimeService
            )
        {
            this.dbContext = dbContext;
            this.productService = productService;
            this.stationService = stationService;
            this.stationBomService = stationBomService;
            this.proResourceService = proResourceService;
            this.stationTaskService = stationTaskService;
            this.flowService = flowService;
            this.stepService = stepService;
            this.flowStepMappingService = flowStepMappingService;
            this.baseStationTaskAnyLoadService = baseStationTaskAnyLoadService;
            this.stationTaskScrewService = stationTaskScrewService;
            this.stationTaskResourceService = stationTaskResourceService;
            this.base_StationTaskAutoService = base_StationTaskAutoService;
            this.sys_LogService = sys_LogService;
            this.base_StationTaskGlueService = base_StationTaskGlueService;
            this.baseStationTaskGluingTimeService = baseStationTaskGluingTimeService;
            this.Base_StationTaskUserInputService = Base_StationTaskUserInput;
            this.base_StationTaskScanCollectService = base_StationTaskScanCollect;
            this.base_StationTaskPressureService = base_StationTaskPressureService;
            this.base_StationTaskModuleInBoxService = base_StationTaskModuleInBoxService;
            this.base_StationTaskStewingTimeService = base_StationTaskStewingTimeService;
        }

        /// <summary>
        /// 上传工位信息
        /// </summary>
        /// <param name="stationTaskExcels"></param>
        /// <returns></returns>
        public async Task<(Boolean, string)> ImporntStationTask(List<StationTaskExcel> datalist, string op)
        {
            try
            {
                using (IDbContextTransaction dbContextTransaction = dbContext.Database.BeginTransaction())
                {
                    var productCount = datalist.GroupBy(g => g.ProductCode).Count();
                    if (productCount != 1)
                    {
                        return (false, "产品PN不唯一，请查验后重试");
                    }
                    var product = await dbContext.Products.FirstOrDefaultAsync(f => !f.IsDeleted && f.Code == datalist[0].ProductCode);
                    if (product == null)
                    {
                        return (false, $"产品{datalist[0].ProductCode}不存在，请查验后重试");
                    }

                    var stepTasks = datalist.GroupBy(g => g.StepCode);
                    foreach (var stepTask in stepTasks)
                    {
                        var step = await dbContext.Base_Steps.FirstOrDefaultAsync(f => !f.IsDeleted && f.Code == stepTask.Key);
                        if (step == null) continue;

                        //先删除配方
                        var hisTasks = await dbContext.Base_StationTasks.Where(w => w.StepId == step.Id && w.ProductId == product.Id && !w.IsDeleted).ToListAsync();
                        foreach (var hisTask in hisTasks)
                        {
                            hisTask.IsDeleted = true;
                        }
                        dbContext.UpdateRange(hisTasks);
                        var stTasks = stepTask.OrderBy(o => o.Sequence).GroupBy(o => o.TaskName).ToList();
                        foreach (var items in stTasks)
                        {
                            var baseTask = new Base_StationTask()
                            {
                                Name = items.First().TaskName,
                                Sequence = stTasks.IndexOf(items) + 1,
                                Type = items.First().Type,
                                Clock = items.First().Clock,
                                StepId = step.Id,
                                ProductId = product.Id,
                            };
                            await dbContext.AddAsync(baseTask);
                            await dbContext.SaveChangesAsync();

                            foreach (var task in items)
                            {
                                switch (task.Type)
                                {
                                    case StationTaskTypeEnum.扫描员工卡:
                                        var scanAccount = new Base_StationTaskScanAccountCard
                                        {
                                            ScanAccountCardName = task.TaskDetailName,
                                            StationTaskId = baseTask.Id,
                                            UpMesCode = task.UpMesCode ?? "",
                                        };
                                        await dbContext.AddAsync(scanAccount);
                                        break;
                                    case StationTaskTypeEnum.扫码:
                                        var scanCode = new Base_StationTaskBom
                                        {
                                            GoodsName = task.TaskDetailName,
                                            UseNum = task.UseNum ?? 1,
                                            StationTaskId = baseTask.Id,
                                            TracingType = task.TracingType,
                                            NeedCollectMESData = task.NeedCollectMESData,
                                            GoodsPN = task.TaskBom_GoodsPN,
                                            GoodsPNRex = task.TaskBom_GoodsPNRex,
                                            OuterGoodsPNRex = task.TaskBom_OuterGoodsPNRex
                                        };
                                        await dbContext.AddAsync(scanCode);
                                        break;
                                    case StationTaskTypeEnum.人工拧螺丝:
                                        var screw = new Base_StationTaskScrew
                                        {
                                            ScrewSpecs = task.TaskDetailName,
                                            DeviceNos = task.TaskScrew_DeviceNos,
                                            UpMesCodePN = task.UpMesCode,
                                            UseNum = task.UseNum ?? 1,
                                            StationTaskId = baseTask.Id,
                                            ProgramNo = task.TaskScrew_ProgramNo,
                                            TorqueMinLimit = task.TaskScrew_TorqueMinLimit,
                                            TorqueMaxLimit = task.TaskScrew_TorqueMaxLimit,
                                            AngleMinLimit = task.TaskScrew_AngleMinLimit,
                                            AngleMaxLimit = task.TaskScrew_AngleMaxLimit,
                                            TaoTongNo = task.TaskScrew_TaoTongNo,
                                            UpMESCodeStartNo = task.TaskScrew_UpMESCodeStartNo ?? 1,
                                            ReworkLimitTimes = task.ReworkLimitTimes ?? task.UseNum,
                                        };
                                        await dbContext.AddAsync(screw);
                                        break;
                                    case StationTaskTypeEnum.扫码输入:
                                        var scanCollect = new Base_StationTaskScanCollect
                                        {
                                            StationTaskId = baseTask.Id,
                                            ScanCollectName = task.TaskDetailName,
                                            UpMesCode = task.UpMesCode ?? "",
                                            CodeRule = task.SacnCollectRule ?? "",
                                        };
                                        await dbContext.AddAsync(scanCollect);
                                        break;
                                    case StationTaskTypeEnum.用户输入:
                                        var userInput = new Base_StationTaskUserInput
                                        {
                                            StationTaskId = baseTask.Id,
                                            UserScanName = task.TaskDetailName,
                                            MinRange = task.TaskUserInput_MinRange,
                                            MaxRange = task.TaskUserInput_MaxRange,
                                            UpMesCode = task.UpMesCode ?? ""
                                        };
                                        await dbContext.AddAsync(userInput);
                                        break;
                                    case StationTaskTypeEnum.人工充气:
                                        Base_StationTaskLeak base_StationTaskLeak = new Base_StationTaskLeak()
                                        {
                                            ParameterName = task.TaskDetailName ?? "",
                                            UpMesCodePN = task.UpMesCode,
                                            KeepPress = task.KeepPress,
                                            KeepTimes = task.KeepTimes,
                                            LeakTimes = task.LeakTimes,
                                            LeakPress = task.LeakTimes,
                                            StationTaskId = baseTask.Id,
                                        };
                                        this.dbContext.Add(base_StationTaskLeak);
                                        await this.dbContext.SaveChangesAsync();
                                        break;
                                    case StationTaskTypeEnum.超时检测:
                                        var checkTimeOut = new Base_StationTaskCheckTimeOut
                                        {
                                            StationTaskId = baseTask.Id,
                                            TimeOutTaskName = task.TaskDetailName ?? "",
                                            MinDuration = task.TimeoutMin ?? 0,
                                            MaxDuration = task.TimeoutMax ?? 999,
                                            UpMesCode = task.UpMesCode,
                                            TimeOutFlag = task.TimeoutFlag

                                        };
                                        await dbContext.AddAsync(checkTimeOut);
                                        break;
                                    case StationTaskTypeEnum.时间记录:
                                        var timeRecord = new Base_StationTask_RecordTime
                                        {
                                            StationTaskId = baseTask.Id,
                                            RecordTimeTaskName = task.TaskDetailName ?? "",
                                            TimeOutFlag = task.TimeFlag,
                                            UpMesCode = task.UpMesCode ?? ""
                                        };
                                        await dbContext.AddAsync(timeRecord);
                                        break;
                                    case StationTaskTypeEnum.称重:
                                        var anyload = new Base_StationTaskAnyLoad
                                        {
                                            StationTaskId = baseTask.Id,
                                            AnyLoadName = task.TaskDetailName ?? "",
                                            MinWeight = task.TaskAnyLoad_MinWeight,
                                            MaxWeight = task.TaskAnyLoad_MaxWeight,
                                            UpMesCode = task.UpMesCode ?? "",
                                        };
                                        await dbContext.AddAsync(anyload);
                                        break;
                                    case StationTaskTypeEnum.补拧:
                                        var screwRe = new Base_StationTask_TightenRework
                                        {
                                            TaskName = task.TaskDetailName ?? "",
                                            DevicesNos = task.TaskRepair_DeviceNos ?? "1",
                                            UpMesCode = task.UpMesCode ?? "",
                                            ScrewNum = task.UseNum ?? 1,
                                            StationTaskId = baseTask.Id,
                                            ProgramNo = task.TaskRepair_ProgramNo ?? 1,
                                            MinTorque = task.TaskRepair_TorqueMinLimit ?? 0,
                                            MaxTorque = task.TaskRepair_TorqueMaxLimit ?? 999,
                                            MinAngle = task.TaskRepair_AngleMinLimit ?? 0,
                                            MaxAngle = task.TaskRepair_AngleMaxLimit ?? 999,
                                            ReworkType = task.TightenReworkType ?? TightenReworkType.Module,
                                        };
                                        await dbContext.AddAsync(screwRe);
                                        break;
                                    case StationTaskTypeEnum.图示拧紧:
                                        // 网页端配方导入导出
                                        var tightenByImage = new Base_StationTask_TightenByImage
                                        {
                                            StationTaskId = baseTask.Id,
                                            TaskName = task.TaskDetailName ?? baseTask.Name ?? "",
                                            ScrewNum = task.UseNum ?? 1,
                                            ProgramNo = task.TaskScrew_ProgramNo ?? 1,
                                            UpMesCode = task.UpMesCode ?? "",
                                            DevicesNos = task.TaskScrew_DeviceNos ?? "1",
                                            MinTorque = task.TaskScrew_TorqueMinLimit ?? 0,
                                            MaxTorque = task.TaskScrew_TorqueMaxLimit ?? 99,
                                            MinAngle = task.TaskScrew_AngleMinLimit ?? 0,
                                            MaxAngle = task.TaskScrew_AngleMaxLimit ?? 999,
                                            FloatFactor = Convert.ToDecimal(0.4f),
                                        };
                                        await dbContext.AddAsync(tightenByImage);
                                        break;
                                    case StationTaskTypeEnum.放行:
                                        break;
                                    case StationTaskTypeEnum.涂胶检测:
                                        break;
                                    case StationTaskTypeEnum.自动拧紧:
                                        var screwA = new Base_AutoStationTaskTighten
                                        {
                                            ParamName = task.TaskDetailName ?? "",
                                            UpMesCode = task.UpMesCode ?? "",
                                            UseNum = task.UseNum ?? 1,
                                            StationTaskId = baseTask.Id,
                                            ProgramNo = task.TaskScrew_ProgramNo ?? 1,
                                            TorqueMin = task.TaskScrew_TorqueMinLimit ?? 0,
                                            TorqueMax = task.TaskScrew_TorqueMaxLimit ?? 999,
                                            AngleMin = task.TaskScrew_AngleMinLimit ?? 0,
                                            AngleMax = task.TaskScrew_AngleMaxLimit ?? 999,
                                        };
                                        await dbContext.AddAsync(screwA);
                                        break;
                                    case StationTaskTypeEnum.自动涂胶:
                                        var glue = new Base_AutoStationTaskGlue
                                        {
                                            StationTaskId = baseTask.Id,
                                            ParameterName = task.TaskDetailName ?? "",
                                            GlueType = task.GlueType ?? GlueType.A胶,
                                            GlueLocate = task.GlueLocate ?? 1,
                                            MinValue = task.GlueMin ?? 0,
                                            MaxValue = task.GlueMax ?? 999,
                                            UpMesCode = task.UpMesCode ?? "",
                                        };
                                        await dbContext.AddRangeAsync(glue);
                                        break;
                                    case StationTaskTypeEnum.自动加压:
                                        var pressure = new Base_AutoStationTaskPressure
                                        {
                                            StationTaskId = baseTask.Id,
                                            ParameterName = task.TaskDetailName ?? "",
                                            PressureLocate = task.PressureLocate ?? 1,
                                            PressurizeDataType = task.PressurizeDataType ?? PressurizeDataType.肩部高度,
                                            MinValue = task.PressureMin ?? 0,
                                            MaxValue = task.PressureMax ?? 999,
                                            UpMesCode = task.UpMesCode ?? "",
                                        };
                                        await dbContext.AddRangeAsync(pressure);
                                        break;
                                    case StationTaskTypeEnum.模组入箱:
                                        var incase = new Base_StationTask_AutoModuleInBox
                                        {
                                            StationTaskId = baseTask.Id,
                                            ParameterName = task.TaskDetailName ?? "",
                                            Location = task.ModuleLocation ?? 1,
                                            ModuleInBoxDataType = task.ModuleInBoxDataType ?? ModuleInBoxDataTypeEnum.模组码,
                                            ModulePN = task.ModulePn ?? "",
                                            MinValue = task.BlockMinValue ?? 0,
                                            MaxValue = task.BlockMaxValue ?? 999,
                                            UpMesCode = task.UpMesCode ?? "",
                                        };
                                        await dbContext.AddRangeAsync(incase);
                                        break;
                                    case StationTaskTypeEnum.下箱体涂胶:
                                        var lowerBoxGlue = new Base_StationTask_LowerBoxGlue
                                        {
                                            StationTaskId = baseTask.Id,
                                            ParameterName = task.TaskDetailName ?? "",
                                            UpMesCode = task.UpMesCode ?? "",
                                            MinValue = task.GlueMin ?? 0,
                                            MaxValue = task.GlueMax ?? 999,
                                        };
                                        await dbContext.AddRangeAsync(lowerBoxGlue);
                                        break;
                                }
                                await dbContext.SaveChangesAsync();
                            }
                        }
                    }
                    await sys_LogService.AddLog(new SysLog { LogType = Sys_LogType.导入配方, Message = $"导入配方，时间{DateTime.Now:yyyy-MM-dd HH:mm:ss}", Operator = op });
                    await dbContextTransaction.CommitAsync();
                }

                return (true, "");
            }
            catch (Exception ex)
            {

                return (false, ex.Message);
            }
        }

        /// <summary>
        /// 导出指定工位的任务详情
        /// </summary>
        /// <param name="stepIds">工位id</param>
        /// <returns></returns>
        public async Task<(Boolean, string)> ExportStationTask(int[] stepIds, int productId)
        {
            string filepath = Directory.GetCurrentDirectory() + @"\工位配方导入导出.xls";
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }
            var stream = new FileStream(filepath, FileMode.OpenOrCreate);
            try
            {

                var product = await dbContext.Products.FirstOrDefaultAsync(f => !f.IsDeleted && f.Id == productId);
                if (product == null)
                {
                    return (false, $"需要导出的产品信息不存在,请验证后重试!");
                }

                var flow = await dbContext.Base_Flows.FirstOrDefaultAsync(f => !f.IsDeleted && f.ProductId == product.Id);
                if (flow == null)
                {
                    stream.Close();
                    return (false, $"需要导出的流程信息不存在，请验证后重试!");
                }

                var steps = await dbContext.Base_Steps.Where(w => !w.IsDeleted && stepIds.Contains(w.Id)).ToListAsync();
                // 只导出存在的工序，忽略不存在的工序
                var totalExcelList = new List<StationTaskExcel>();
                foreach (var step in steps)
                {
                    var baseStationTasks = await dbContext.Base_StationTasks.Where(w => !w.IsDeleted && w.ProductId == product.Id && w.StepId == step.Id).OrderBy(o => o.Sequence).ToListAsync();
                    var excelList = new List<StationTaskExcel>();
                    foreach (var stationTask in baseStationTasks)
                    {
                        switch (stationTask.Type)
                        {
                            case StationTaskTypeEnum.扫描员工卡:
                                var scanAccount = await dbContext.Base_StationTaskScanAccountCards.Where(f => !f.IsDeleted && f.StationTaskId == stationTask.Id).Select(s => new StationTaskExcel
                                {
                                    FlowCode = flow.Code,
                                    ProductCode = product.Code ?? "",
                                    StepCode = step.Code ?? "",
                                    Sequence = stationTask.Sequence,
                                    Type = stationTask.Type,
                                    TaskName = stationTask.Name ?? "",
                                    TaskDetailName = s.ScanAccountCardName,
                                    UpMesCode = s.UpMesCode,
                                }).FirstOrDefaultAsync();
                                if (scanAccount != null)
                                    excelList.Add(scanAccount);
                                break;
                            case StationTaskTypeEnum.扫码:
                                var scanBom = await dbContext.Base_StationTaskBoms.Where(f => !f.IsDeleted && f.StationTaskId == stationTask.Id).Select(s => new StationTaskExcel
                                {
                                    FlowCode = flow.Code,
                                    ProductCode = product.Code ?? "",
                                    StepCode = step.Code ?? "",
                                    UseNum = s.UseNum,
                                    Sequence = stationTask.Sequence,
                                    TaskName = stationTask.Name ?? "",
                                    Type = stationTask.Type,
                                    TaskDetailName = s.GoodsName,
                                    TaskBom_GoodsPN = s.GoodsPN,
                                    TracingType = s.TracingType,
                                    TaskBom_GoodsPNRex = s.GoodsPNRex,
                                    TaskBom_OuterGoodsPNRex = s.OuterGoodsPNRex
                                }).ToListAsync();
                                excelList.AddRange(scanBom);
                                break;
                            case StationTaskTypeEnum.人工拧螺丝:
                                var screw = await dbContext.Base_StationTaskScrews.Where(f => !f.IsDeleted && f.StationTaskId == stationTask.Id).Select(s => new StationTaskExcel
                                {
                                    FlowCode = flow.Code,
                                    ProductCode = product.Code ?? "",
                                    StepCode = step.Code ?? "",
                                    Sequence = stationTask.Sequence,
                                    TaskName = stationTask.Name ?? "",
                                    Type = stationTask.Type,
                                    UpMesCode = s.UpMesCodePN,
                                    UseNum = s.UseNum,
                                    TaskDetailName = s.ScrewSpecs,
                                    TaskScrew_ProgramNo = s.ProgramNo,
                                    TaskScrew_DeviceNos = s.DeviceNos,
                                    TaskScrew_TaoTongNo = s.TaoTongNo,
                                    TaskScrew_TorqueMaxLimit = s.TorqueMaxLimit,
                                    TaskScrew_TorqueMinLimit = s.TorqueMinLimit,
                                    TaskScrew_AngleMaxLimit = s.AngleMaxLimit,
                                    TaskScrew_AngleMinLimit = s.AngleMinLimit,
                                    TaskScrew_UpMESCodeStartNo = s.UpMESCodeStartNo,
                                    ReworkLimitTimes = s.ReworkLimitTimes,
                                }).ToListAsync();

                                excelList.AddRange(screw);
                                break;
                            case StationTaskTypeEnum.扫码输入:
                                var scanCollect = await dbContext.Base_StationTaskScanCollects.Where(f => !f.IsDeleted && f.StationTaskId == stationTask.Id).Select(s => new StationTaskExcel
                                {
                                    FlowCode = flow.Code,
                                    ProductCode = product.Code ?? "",
                                    StepCode = step.Code ?? "",
                                    Sequence = stationTask.Sequence,
                                    Type = stationTask.Type,
                                    TaskName = stationTask.Name ?? "",
                                    TaskDetailName = s.ScanCollectName,
                                    SacnCollectRule = s.CodeRule,
                                    UpMesCode = s.UpMesCode,
                                }).FirstOrDefaultAsync();
                                if (scanCollect != null)
                                    excelList.Add(scanCollect);
                                break;
                            case StationTaskTypeEnum.用户输入:
                                var userInput = await dbContext.Base_StationTaskUserInputs.Where(f => !f.IsDeleted && f.StationTaskId == stationTask.Id).Select(s => new StationTaskExcel
                                {
                                    FlowCode = flow.Code,
                                    ProductCode = product.Code ?? "",
                                    StepCode = step.Code ?? "",
                                    Sequence = stationTask.Sequence,
                                    Type = stationTask.Type,
                                    TaskName = stationTask.Name ?? "",
                                    TaskDetailName = s.UserScanName,
                                    TaskUserInput_MinRange = s.MinRange,
                                    TaskUserInput_MaxRange = s.MaxRange,
                                    UpMesCode = s.UpMesCode,
                                }).FirstOrDefaultAsync();
                                if (userInput != null)
                                    excelList.Add(userInput);
                                break;
                            case StationTaskTypeEnum.超时检测:
                                var timeOut = await dbContext.Base_StationTaskCheckTimeOuts.Where(f => !f.IsDeleted && f.StationTaskId == stationTask.Id).Select(s => new StationTaskExcel
                                {
                                    FlowCode = flow.Code,
                                    ProductCode = product.Code ?? "",
                                    StepCode = step.Code ?? "",
                                    Sequence = stationTask.Sequence,
                                    Type = stationTask.Type,
                                    TaskName = stationTask.Name ?? "",
                                    TaskDetailName = s.TimeOutTaskName,
                                    TimeoutMin = s.MinDuration,
                                    TimeoutMax = s.MaxDuration,
                                    TimeoutFlag = s.TimeOutFlag,
                                    UpMesCode = s.UpMesCode,
                                }).FirstOrDefaultAsync();
                                if (timeOut != null)
                                    excelList.Add(timeOut);
                                break;
                            case StationTaskTypeEnum.时间记录:
                                var timeRecord = await dbContext.Base_StationTask_RecordTimes.Where(f => !f.IsDeleted && f.StationTaskId == stationTask.Id).Select(s => new StationTaskExcel
                                {
                                    FlowCode = flow.Code,
                                    ProductCode = product.Code ?? "",
                                    StepCode = step.Code ?? "",
                                    Sequence = stationTask.Sequence,
                                    Type = stationTask.Type,
                                    TaskName = stationTask.Name ?? "",
                                    TaskDetailName = s.RecordTimeTaskName,
                                    TimeFlag = s.TimeOutFlag,
                                    UpMesCode = s.UpMesCode,
                                }).FirstOrDefaultAsync();
                                if (timeRecord != null)
                                    excelList.Add(timeRecord);
                                break;
                            case StationTaskTypeEnum.称重:
                                var anyload = await dbContext.Base_StationTaskAnyLoads.Where(f => !f.IsDeleted && f.StationTaskId == stationTask.Id).Select(s => new StationTaskExcel
                                {
                                    FlowCode = flow.Code,
                                    ProductCode = product.Code ?? "",
                                    StepCode = step.Code ?? "",
                                    Sequence = stationTask.Sequence,
                                    Type = stationTask.Type,
                                    TaskName = stationTask.Name ?? "",
                                    TaskDetailName = s.AnyLoadName,
                                    TaskAnyLoad_MinWeight = s.MinWeight,
                                    TaskAnyLoad_MaxWeight = s.MaxWeight,
                                    UpMesCode = s.UpMesCode,
                                }).FirstOrDefaultAsync();
                                if (anyload != null)
                                    excelList.Add(anyload);
                                break;
                            case StationTaskTypeEnum.补拧:
                                var screwR = await dbContext.Base_StationTask_TightenReworks.Where(f => !f.IsDeleted && f.StationTaskId == stationTask.Id).Select(s => new StationTaskExcel
                                {
                                    FlowCode = flow.Code,
                                    ProductCode = product.Code ?? "",
                                    StepCode = step.Code ?? "",
                                    Sequence = stationTask.Sequence,
                                    Type = stationTask.Type,
                                    TaskName = stationTask.Name ?? "",
                                    UpMesCode = s.UpMesCode,
                                    UseNum = s.ScrewNum,
                                    TaskDetailName = s.TaskName,
                                    TaskRepair_ProgramNo = s.ProgramNo,
                                    TaskRepair_DeviceNos = s.DevicesNos,
                                    TaskRepair_TorqueMaxLimit = s.MinTorque,
                                    TaskRepair_TorqueMinLimit = s.MaxTorque,
                                    TaskRepair_AngleMaxLimit = s.MinAngle,
                                    TaskRepair_AngleMinLimit = s.MaxAngle,
                                    TightenReworkType = s.ReworkType,
                                }).FirstOrDefaultAsync();
                                if (screwR != null)
                                    excelList.Add(screwR);
                                break;
                            case StationTaskTypeEnum.图示拧紧:
                                var tightenByImage = await dbContext.Base_StationTask_TightenByImages
                                    .Where(f => !f.IsDeleted && f.StationTaskId == stationTask.Id)
                                    .Select(s => new StationTaskExcel
                                    {
                                        FlowCode = flow.Code,
                                        ProductCode = product.Code ?? "",
                                        StepCode = step.Code ?? "",
                                        Sequence = stationTask.Sequence,
                                        Type = stationTask.Type,
                                        TaskName = stationTask.Name ?? "",
                                        TaskDetailName = s.TaskName,
                                        UseNum = s.ScrewNum,
                                        UpMesCode = s.UpMesCode,
                                        TaskScrew_ProgramNo = s.ProgramNo,
                                        TaskScrew_DeviceNos = s.DevicesNos,
                                        TaskScrew_TorqueMinLimit = s.MinTorque,
                                        TaskScrew_TorqueMaxLimit = s.MaxTorque,
                                        TaskScrew_AngleMinLimit = s.MinAngle,
                                        TaskScrew_AngleMaxLimit = s.MaxAngle,
                                    })
                                    .FirstOrDefaultAsync();
                                if (tightenByImage != null)
                                    excelList.Add(tightenByImage);
                                break;
                            case StationTaskTypeEnum.人工充气:
                                var leaks = await this.dbContext.Base_StationTaskLeaks.Where(o => !o.IsDeleted && o.StationTaskId == stationTask.Id).ToListAsync();
                                foreach (var leak in leaks)
                                {
                                    StationTaskExcel taskExcelAutoStation = new StationTaskExcel()
                                    {
                                        FlowCode = flow.Code,
                                        ProductCode = product.Code ?? "",
                                        StepCode = step.Code ?? "",
                                        Sequence = stationTask.Sequence,
                                        Type = stationTask.Type,
                                        TaskName = stationTask.Name ?? "",
                                        TaskDetailName = leak.ParameterName,
                                        UpMesCode = leak.UpMesCodePN,
                                        KeepPress = leak.KeepPress,
                                        KeepTimes = leak.KeepTimes,
                                        LeakTimes = leak.LeakTimes,
                                        LeakPress = leak.LeakPress,
                                    };
                                    excelList.Add(taskExcelAutoStation);
                                }
                                break;
                            case StationTaskTypeEnum.放行:
                                var letgo = new StationTaskExcel
                                {
                                    FlowCode = flow.Code,
                                    ProductCode = product.Code ?? "",
                                    StepCode = step.Code ?? "",
                                    Sequence = stationTask.Sequence,
                                    Type = stationTask.Type,
                                    TaskName = stationTask.Name ?? "",
                                };
                                excelList.Add(letgo);
                                break;

                            case StationTaskTypeEnum.涂胶检测:
                                break;
                            case StationTaskTypeEnum.自动拧紧:
                                var screwA = await dbContext.Base_AutoStationTaskTightens.Where(f => !f.IsDeleted && f.StationTaskId == stationTask.Id).Select(s => new StationTaskExcel
                                {
                                    FlowCode = flow.Code,
                                    ProductCode = product.Code ?? "",
                                    StepCode = step.Code ?? "",
                                    Sequence = stationTask.Sequence,
                                    Type = stationTask.Type,
                                    TaskName = stationTask.Name ?? "",
                                    TaskDetailName = s.ParamName,
                                    UseNum = s.UseNum,
                                    UpMesCode = s.UpMesCode,
                                    TaskScrew_ProgramNo = s.ProgramNo,
                                    TaskScrew_TorqueMaxLimit = s.TorqueMax,
                                    TaskScrew_TorqueMinLimit = s.TorqueMin,
                                    TaskScrew_AngleMaxLimit = s.AngleMax,
                                    TaskScrew_AngleMinLimit = s.AngleMin,
                                }).FirstOrDefaultAsync();
                                if (screwA != null)
                                    excelList.Add(screwA);
                                break;
                            case StationTaskTypeEnum.自动涂胶:
                                var glue = await dbContext.Base_AutoStationTaskGlues.Where(f => !f.IsDeleted && f.StationTaskId == stationTask.Id).Select(s => new StationTaskExcel
                                {
                                    FlowCode = flow.Code,
                                    ProductCode = product.Code ?? "",
                                    StepCode = step.Code ?? "",
                                    Sequence = stationTask.Sequence,
                                    Type = stationTask.Type,
                                    TaskName = stationTask.Name ?? "",
                                    TaskDetailName = s.ParameterName,
                                    UpMesCode = s.UpMesCode,
                                    GlueLocate = s.GlueLocate,
                                    GlueMin = s.MinValue,
                                    GlueMax = s.MaxValue,
                                    GlueType = s.GlueType
                                }).ToListAsync();
                                excelList.AddRange(glue);
                                break;
                            case StationTaskTypeEnum.自动加压:
                                var pressure = await dbContext.Base_AutoStationTaskPressures.Where(f => !f.IsDeleted && f.StationTaskId == stationTask.Id).Select(s => new StationTaskExcel
                                {
                                    FlowCode = flow.Code,
                                    ProductCode = product.Code ?? "",
                                    StepCode = step.Code ?? "",
                                    Sequence = stationTask.Sequence,
                                    Type = stationTask.Type,
                                    TaskName = stationTask.Name ?? "",
                                    UpMesCode = s.UpMesCode,
                                    TaskDetailName = s.ParameterName,
                                    PressureMin = s.MinValue,
                                    PressureMax = s.MaxValue,
                                    PressureLocate = s.PressureLocate,
                                    PressurizeDataType = s.PressurizeDataType,
                                }).ToListAsync();
                                excelList.AddRange(pressure);
                                break;

                            case StationTaskTypeEnum.模组入箱:
                                var incase = await dbContext.Base_StationTask_AutoModuleInBoxs.Where(f => !f.IsDeleted && f.StationTaskId == stationTask.Id).Select(s => new StationTaskExcel
                                {
                                    FlowCode = flow.Code,
                                    ProductCode = product.Code ?? "",
                                    StepCode = step.Code ?? "",
                                    Sequence = stationTask.Sequence,
                                    Type = stationTask.Type,
                                    TaskName = stationTask.Name ?? "",
                                    UpMesCode = s.UpMesCode,
                                    TaskDetailName = s.ParameterName,
                                    BlockMinValue = s.MinValue,
                                    BlockMaxValue = s.MaxValue,
                                    ModuleLocation = s.Location,
                                    ModulePn = s.ModulePN,
                                    ModuleInBoxDataType = s.ModuleInBoxDataType
                                }).ToListAsync();
                                excelList.AddRange(incase);
                                break;
                            case StationTaskTypeEnum.下箱体涂胶:
                                var lowerBoxGlue = await dbContext.Base_StationTask_LowerBoxGlues.Where(f => !f.IsDeleted && f.StationTaskId == stationTask.Id).Select(s => new StationTaskExcel
                                {
                                    FlowCode = flow.Code,
                                    ProductCode = product.Code ?? "",
                                    StepCode = step.Code ?? "",
                                    Sequence = stationTask.Sequence,
                                    Type = stationTask.Type,
                                    TaskName = stationTask.Name ?? "",
                                    TaskDetailName = s.ParameterName,
                                    UpMesCode = s.UpMesCode,
                                    GlueMin = s.MinValue,
                                    GlueMax = s.MaxValue,
                                }).ToListAsync();
                                excelList.AddRange(lowerBoxGlue);
                                break;
                        }
                    }
                    totalExcelList.AddRange(excelList);
                }

                ExcelPackage excelPackage = new ExcelPackage();
                excelPackage = ExcelToEntity.ListToExcek<StationTaskExcel>(excelPackage, "工位配方导出", 2, totalExcelList);
                excelPackage.SaveAs(stream);
                stream.Close();
                return (true, filepath);
            }
            catch (Exception ex)
            {
                stream.Close();
                return (false, $"导出出现错误{ex.Message}");
            }
        }

        private (Boolean, string) SetTask(List<StationTaskExcel> exceldatalist, Base_StationTask stationTask, Base_Step step, Base_FlowStepMapping mapdata)
        {
            try
            {
                foreach (var exceldata in exceldatalist)
                {
                    exceldata.FlowCode = mapdata.Flow.Code;
                    exceldata.ProductCode = mapdata.Flow.Product.Code;
                    exceldata.TaskName = stationTask.Name;
                    exceldata.Type = stationTask.Type;
                    exceldata.Sequence = stationTask.Sequence;
                    exceldata.StepCode = step.Code;
                    //exceldata.HasPage = stationTask.HasPage;
                    exceldata.Clock = stationTask.Clock;
                }

                return (true, "");
            }
            catch (Exception ex)
            {

                return (true, $"加入公共字段出错{ex.Message}");
            }

        }
    }
}
