using AsZero.Core.Entities;
using AsZero.Core.Services.Repos;
using AsZero.Core.Services.Sys_Logs;
using AsZero.DbContexts;

using Microsoft.EntityFrameworkCore;

using Yee.Common.Library.CommonEnum;
using Yee.Entitys.AlarmMgmt;
using Yee.Entitys.CommonEntity;
using Yee.Entitys.DBEntity;
using Yee.Entitys.DTOS;
using Yee.Entitys.Production;
using Yee.Services.BaseData;
using Yee.Services.Helper;
using Yee.Services.Production;
using Yee.Services.Request;

namespace Yee.Services.AGV
{
    public class AGVService : IAGVResolver
    {
        private readonly ProductService _productService;
        private readonly ILogger<AGVService> _logger;
        public readonly AsZeroDbContext _dbContext;
        private readonly StationService _stationService;
        private readonly FlowService _flowService;
        private readonly StationTaskService _stationTaskService;
        private readonly IConfiguration _configuration;
        public readonly APIHelper _apiHelper;
        private readonly SysLogService _sysLogService;
        private readonly bool _isDebug = false;

        public AGVService(AsZeroDbContext dbContext, StationTaskService stationTaskService, ProductService productService, StationService stationService, FlowService flowService, ILogger<AGVService> logger, IConfiguration configuration, APIHelper aPIHelper, SysLogService sysLogService)
        {

            _dbContext = dbContext;
            _productService = productService;
            _logger = logger;
            _apiHelper = aPIHelper;
            _stationService = stationService;
            _flowService = flowService;
            _stationTaskService = stationTaskService;
            _configuration = configuration;
            _sysLogService = sysLogService;
            _isDebug = _configuration.GetSection("AppOpts").Get<AppOpts>().IsDebug;
        }

        public async Task<bool> CheckAgvBinding(int agvNo, string packPN)
        {
            var agv = await _dbContext.Proc_AGVStatuss.FirstOrDefaultAsync(a => a.AGVNo == agvNo && a.PackPN == packPN);
            if (agv != null)
                return true;
            else
                return false;
        }

        public async Task<List<Proc_AGVStatus>> Load(GetByKeyInput keyInput)
        {
            if (string.IsNullOrEmpty(keyInput.Key))
            {
                return await this._dbContext.Proc_AGVStatuss.Where(o => !o.IsDeleted).OrderBy(o => o.AGVNo).ToListAsync();
            }
            else
            {
                return await this._dbContext.Proc_AGVStatuss.Where(o => !o.IsDeleted && o.AGVNo == Convert.ToInt32(keyInput.Key)).ToListAsync();
            }
        }

        public async Task<Proc_AGVStatus> Add(Proc_AGVStatus entity, string? user)
        {
            var res = await _dbContext.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            await _sysLogService.AddLog(new SysLog() { LogType = Sys_LogType.新增AGV, Message = $"新增AGV:{entity.AGVNo}", Operator = user });
            return res.Entity;
        }


        public async Task Delete(Proc_AGVStatus entity, string? user)
        {
            entity.IsDeleted = true;
            _dbContext.Update(entity);
            await _sysLogService.AddLog(new SysLog() { LogType = Sys_LogType.删除AGV, Message = $"删除AGV:{entity.AGVNo}", Operator = user });
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Proc_AGVStatus> Update(Proc_AGVStatus entity)
        {
            var res = _dbContext.Update(entity);
            await _dbContext.SaveChangesAsync();

            return res.Entity;
        }

        public async Task<Proc_AGVStatus> HasCode(int agvNo)
        {
            var entity = await _dbContext.Proc_AGVStatuss.Where(d => !d.IsDeleted && d.AGVNo == agvNo).FirstOrDefaultAsync();
            if (entity != null)
                return entity;
            else
                return null;
        }

        /// 通过ID查询
        /// </summary>
        public async Task<Proc_AGVStatus?> GetById(int id)
        {
            var entity = await _dbContext.Proc_AGVStatuss.Where(o => !o.IsDeleted && o.Id == id).FirstOrDefaultAsync();
            return entity;
        }

        public async Task<(bool, string)> BingAgv(BindAgvDTO dto, string? user)
        {
            var hasagv = await this.HasCode(dto.AgvCode);
            if (hasagv != null)
            {
                hasagv.Behavior = (int)dto.State;
                switch (dto.State)
                {
                    case BingAgvStateEnum.绑定:
                        hasagv.PackPN = dto.PackPN;
                        hasagv.ProductType = dto.ProductType;
                        hasagv.HolderBarCode = dto.HolderBarCode;
                        hasagv.StationCode = dto.StationCode;
                        await _sysLogService.AddLog(new SysLog() { LogType = Sys_LogType.绑定AGV, Message = $"绑定AGV:小车号：{hasagv.AGVNo}，Pack码：{hasagv.PackPN},站点{hasagv.StationCode}", Operator = user });
                        break;
                    case BingAgvStateEnum.解绑:
                        hasagv.PackPN = String.Empty;
                        hasagv.ProductType = String.Empty;
                        hasagv.HolderBarCode = String.Empty;
                        hasagv.StationCode = String.Empty;
                        await _sysLogService.AddLog(new SysLog() { LogType = Sys_LogType.解绑AGV, Message = $"解绑AGV:，Pack码：{dto.PackPN},站点{dto.StationCode}", Operator = user });
                        break;
                    default:
                        break;
                }

                this._dbContext.Update(hasagv);
                this._dbContext.SaveChanges();

                return (true, "绑定完成");
            }
            else
            {
                return (false, "需要绑定的Agv不存在或者Pack条码不合法!");
            }
        }

        public async Task<Response> RunAGV(string agvCode, int releaseType)
        {
            Response response = new Response();
            try
            {
                // var result = await _apiHelper.RunAgv(agvCode, releaseType);
                response = await _apiHelper.RunAgv_JT(agvCode, releaseType);
                return response;
                //if (!result.Success)
                //{
                //    response.Code = 500;
                //    response.Message = result.Message;
                //}
            }
            catch (Exception e)
            {
                response.Code = 500;
                response.Message = e.InnerException?.Message ?? e.Message;
                return response;
            }

        }

        public async Task<Proc_AGVStatus> LoadStationCurrentAGV(string code)
        {
            var agv = await _dbContext.Proc_AGVStatuss.OrderByDescending(a => a.UpdateTime).Where(a => a.StationCode.Contains(code) && !a.IsDeleted).FirstOrDefaultAsync();
            return agv;
        }

        public async Task AGVArrived(AGVMessage agvMsg)
        {
            //清空站内其他小车的工位号
            if (!string.IsNullOrWhiteSpace(agvMsg.StationName))
            {
                var otherAGVsInStation = await _dbContext.Proc_AGVStatuss.Where(e => e.StationCode == agvMsg.StationName).ToListAsync();
                foreach (var otherAGV in otherAGVsInStation)
                {
                    otherAGV.StationCode = string.Empty;
                }
            }

            //更新进站小车的工位号
            var agv = _dbContext.Proc_AGVStatuss.FirstOrDefault(a => a.AGVNo == agvMsg.AgvCode && !a.IsDeleted);
            if (agv == null)
            {
                agv = new Proc_AGVStatus()
                {
                    AGVNo = agvMsg.AgvCode,
                    StationCode = agvMsg.StationName,
                    PackPN = agvMsg.ProductCode
                };
                await _dbContext.AddAsync(agv);
            }
            agv.StationCode = agvMsg.StationName;
            agv.PackPN = agvMsg.ProductCode;
            agv.UpdateTime = DateTime.Now;

            await _dbContext.SaveChangesAsync();
        }

        public void AGVLeaved(AGVMessage agvMsg)
        {
            var agv = _dbContext.Proc_AGVStatuss.FirstOrDefault(a => a.AGVNo == agvMsg.AgvCode && !a.IsDeleted);
            if (agv != null)
            {
                agv.StationCode = string.Empty;
                agv.UpdateTime = DateTime.Now;
                _dbContext.SaveChanges();
            }
        }

        /// <summary>
        /// 读取当前工位 在此Pack下对应的配方数据，同时校验此Pack的生产记录是否合法
        /// </summary>
        /// <param name="stepCode"></param>
        /// <param name="agvNo"></param>
        /// <param name="packPN"></param>
        /// <returns></returns>
        public async Task<Response<StationPack_AGV_Task_Record_DTO>> CheckPack_AGV_Task_Record_Auto(string stepCode, string agvNo, string packPN)
        {
            var result = new Response<StationPack_AGV_Task_Record_DTO>();
            result.Result = new StationPack_AGV_Task_Record_DTO();

            // 读取AGV信息
            Proc_AGVStatus agv = _dbContext.Proc_AGVStatuss.OrderByDescending(a => a.UpdateTime).FirstOrDefault(a => a.AGVNo.ToString() == agvNo && !a.IsDeleted);
            if (agv == null)
            {
                result.Result.Proc_AGVStatus = null;
                result.Code = 10001;
                return result;
            }
            if (string.IsNullOrEmpty(agv.PackPN) || agv.PackPN != packPN)
            {
                result.Result.Proc_AGVStatus = null;
                result.Code = 10002;
                return result;
            }

            if (string.IsNullOrEmpty(agv.StationCode))
            {
                result.Result.Proc_AGVStatus = null;
                result.Code = 10015;
                return result;
            }

            // 找到pack条码对应的Product
            var product = _productService.GetByPackcode(packPN).Result;
            if (product == null)
            {
                result.Code = 10003;
                result.Message = $"未找到Pack码{packPN}对应的配方数据";
                return result;
            }
            // 根据Product 找到对应得工艺流程
            var entityFlow = _dbContext.Base_Flows.FirstOrDefault(f => f.ProductId == product.Id && !f.IsDeleted);
            if (entityFlow == null)
            {
                result.Code = 10004;
                result.Message = $"未找到Pack码{packPN}对应的工艺流程";
                return result;
            }
            // 根据Pack条码 工位编码 查找对应的工位
            var flowStepMapping = await _dbContext.Base_FlowStepMappings.Where(f => !f.IsDeleted && f.FlowId == entityFlow.Id).OrderByDescending(o => o.OrderNo).ToListAsync();
            if (flowStepMapping == null)
            {
                result.Code = 10005;
                result.Message = $"未找到Pack码{packPN}对应的工序";
                return result;
            }

            var step = await _dbContext.Base_Steps.FirstOrDefaultAsync(s => s.Code == stepCode && s.IsDeleted == false && flowStepMapping.Select(o => o.StepId).Contains(s.Id));
            if (step == null)
            {
                result.Code = 10005;
                result.Message = $"未找到Pack码{packPN}对应的工序";
                return result;
            }

            var station = await _dbContext.Base_Stations.FirstOrDefaultAsync(s => s.Code == agv.StationCode && s.IsDeleted == false && s.StepId == step.Id);
            if (station == null)
            {
                result.Code = 10005;
                result.Message = $"未找到Pack码{packPN}对应的工序";
                return result;
            }
            var stationCode = agv.StationCode;

            if (!_isDebug)
            {
                // 检查上一工位是否已完成
                (bool check, string checkMsg) = await _stationService.CheckPrevStationWorkStatus(step, entityFlow, packPN);
                if (!check)
                {
                    result.Code = 10005;
                    result.Message = checkMsg;
                    return result;
                }
            }

            // 查询配方数据
            var mapping = await _stationTaskService.GetStationTaskDTOByStation(step, stationCode, product.Id);
            if (mapping == null)
            {
                result.Code = 10005;
                result.Message = $"未找到Pack码{packPN}对应的工位配方数据";
                return result;
            }

            int? programNo = 0;

            // 读取自动站程序号数据
            if (step.StepType == StepTypeEnum.自动站)
            {
                if (step.Code == "OP120" || step.Code == "OP180")
                {
                    var screw = _dbContext.Base_AutoStationTaskTightens.FirstOrDefault(t => !t.IsDeleted && t.StationTaskId == mapping[0].Id);
                    if (screw != null)
                        programNo = screw.ProgramNo;
                }
            }

            result.Result.StationConfig = new StationConfig
            {
                Product = product,
                StationTaskList = mapping,
            };

            result.Result.Proc_AGVStatus = agv;
            result.Code = 200;
            return result;
        }

        /// <summary>
        /// 读取当前工位 在此Pack下对应的配方数据，同时校验此Pack的生产记录是否合法
        /// </summary>
        /// <param name="code"></param>
        /// <param name="agvNo"></param>
        /// <param name="packPN"></param>
        /// <returns></returns>
        public async Task<Response<StationPack_AGV_Task_Record_DTO>> CheckPack_AGV_Task_Record_Auto(string stepCode, string stationCode, string agvNo, string packPN, bool askIn = false)
        {
            var result = new Response<StationPack_AGV_Task_Record_DTO>();
            result.Result = new StationPack_AGV_Task_Record_DTO();

            // 读取AGV信息
            Proc_AGVStatus agv = _dbContext.Proc_AGVStatuss.OrderByDescending(a => a.UpdateTime).FirstOrDefault(a => a.AGVNo.ToString() == agvNo && a.PackPN == packPN && !a.IsDeleted);
            if (agv == null)
            {
                result.Result.Proc_AGVStatus = null;
                result.Code = 10001;
                result.Message = "AGV绑定关系校验失败";
                return result;
            }

            if (!askIn && string.IsNullOrEmpty(agv.StationCode))
            {
                result.Result.Proc_AGVStatus = null;
                result.Code = 10015;
                return result;
            }

            var agvStation = _dbContext.Proc_AGVStatuss.OrderByDescending(a => a.UpdateTime).FirstOrDefault(a => a.AGVNo.ToString() == agvNo && a.StationCode == stationCode && a.PackPN == packPN && !a.IsDeleted);
            if (agvStation == null && !askIn)
            {
                result.Result.Proc_AGVStatus = null;
                result.Code = 10002;
                result.Message = "AGV工位绑定关系校验失败";

                return result;
            }


            // 找到pack条码对应的Product
            var product = _productService.GetByPackcode(packPN).Result;
            if (product == null)
            {
                result.Code = 10003;
                result.Message = $"未找到Pack码{packPN}对应的配方数据";
                return result;
            }
            // 根据Product 找到对应得工艺流程
            var entityFlow = _dbContext.Base_Flows.FirstOrDefault(f => f.ProductId == product.Id && !f.IsDeleted);
            if (entityFlow == null)
            {
                result.Code = 10004;
                result.Message = $"未找到Pack码{packPN}对应的工艺流程";
                return result;
            }
            // 根据Pack条码 工位编码 查找对应的工位
            var flowStepMapping = await _dbContext.Base_FlowStepMappings.Where(f => !f.IsDeleted && f.FlowId == entityFlow.Id).OrderByDescending(o => o.OrderNo).ToListAsync();
            if (flowStepMapping == null)
            {
                result.Code = 10005;
                result.Message = $"未找到Pack码{packPN}对应的工序";
                return result;
            }

            var step = await _dbContext.Base_Steps.FirstOrDefaultAsync(s => s.Code == stepCode && s.IsDeleted == false && flowStepMapping.Select(o => o.StepId).Contains(s.Id));
            if (step == null)
            {
                result.Code = 10005;
                result.Message = $"未找到Pack码{packPN}对应的工序";
                return result;
            }

            var station = await _dbContext.Base_Stations.FirstOrDefaultAsync(s => s.Code == stationCode && s.IsDeleted == false && s.StepId == step.Id);
            if (station == null)
            {
                result.Code = 10005;
                result.Message = $"未找到Pack码{packPN}对应的工序";
                return result;
            }

            if (!_isDebug)
            {
                // 检查上一工位是否已完成
                (bool check, string checkMsg) = await _stationService.CheckPrevStationWorkStatus(step, entityFlow, packPN);
                if (!check)
                {
                    result.Code = 10005;
                    result.Message = checkMsg;
                    return result;
                }
            }

            // 查询配方数据
            var mapping = await _stationTaskService.GetStationTaskDTOByStation(step, stationCode, product.Id);
            if (mapping == null)
            {
                result.Code = 10005;
                result.Message = $"未找到Pack码{packPN}对应的工位配方数据";
                return result;
            }

            int? programNo = 0;

            // 读取自动站程序号数据
            if (step.StepType == StepTypeEnum.自动站)
            {
                if (step.Code == "OP120" || step.Code == "OP180")
                {
                    var screw = _dbContext.Base_AutoStationTaskTightens.FirstOrDefault(t => !t.IsDeleted && t.StationTaskId == mapping[0].Id);
                    if (screw != null)
                        programNo = screw.ProgramNo;
                }
                //if (step.Code == "OP030")
                //{
                //    var taskGlue = await _stationTaskService.GetStationTaskGlue(step.Id);
                //    if (taskGlue != null)
                //        programNo = taskGlue?.ProgramNo;
                //}
            }

            result.Result.StationConfig = new StationConfig
            {
                Product = product,
                StationTaskList = mapping,
            };

            result.Result.Proc_AGVStatus = agvStation;
            result.Code = 200;
            return result;
        }

        public async Task<Response<StationPack_AGV_Task_Record_DTO>> CheckPack_AGV_Task_Record(int productId, int stepId, string stationCode, string agvNo, string packPN)
        {
            var result = new Response<StationPack_AGV_Task_Record_DTO>();
            result.Result = new StationPack_AGV_Task_Record_DTO();


            var entity = _dbContext.Products.FirstOrDefault(f => f.Id == productId);
            if (entity == null)
            {
                result.Code = 500;
                result.Message = $"未找到Pack码{packPN}对应的配方数据";
                return result;
            }
            // 根据Product 找到对应得工艺流程
            var entityFlow = _dbContext.Base_Flows.FirstOrDefault(f => f.ProductId == entity.Id && !f.IsDeleted);
            if (entityFlow == null)
            {
                result.Code = 500;
                result.Message = $"未找到Pack码{packPN}对应的工艺流程";
                return result;
            }

            var step = await _dbContext.Base_Steps.FirstOrDefaultAsync(s => s.Id == stepId);
            if (step == null)
            {
                result.Code = 500;
                result.Message = $"未找到Pack码{packPN}对应的工位";
                return result;
            }


            // 检查上一工位是否已完成
            (bool check, string checkMsg) = await _stationService.CheckPrevStationWorkStatus(step, entityFlow, packPN);
            if (!check)
            {
                result.Code = 10005;
                result.Message = checkMsg;
                return result;
            }

            // 读取AGV信息
            var agv = await _dbContext.Proc_AGVStatuss.OrderByDescending(a => a.UpdateTime).FirstOrDefaultAsync(a => a.PackPN == packPN && a.StationCode == stationCode && a.AGVNo.ToString() == agvNo && !a.IsDeleted);
            if (agv == null)//扫描Pack校验小车Pack码
            {
                result.Result.Proc_AGVStatus = null;
                result.Code = 500;
                result.Message = "AGV绑定关系校验失败！";
                return result;
            }
            result.Result.Proc_AGVStatus = agv;

            var hisData = await _stationTaskService.LoadStationTaskHistoryData(step.Id, packPN);
            result.Result.Pack_His_Data = hisData.Result;
            result.Code = 200;
            return result;
        }

        public async Task<Response<List<StationConfig>>> LoadStationAllPeiFangData(string stationCode)
        {
            var result = new Response<List<StationConfig>>();
            result.Result = new List<StationConfig>();

            var entityList = _dbContext.Products.Include(p => p.Type).Where(f => !string.IsNullOrEmpty(f.PackPNRule) && !f.IsDeleted).ToList();

            foreach (var entity in entityList)
            {
                // 根据Product 找到对应得工艺流程
                var entityFlow = _dbContext.Base_Flows.FirstOrDefault(f => f.ProductId == entity.Id && !f.IsDeleted);
                if (entityFlow != null)
                {
                    // 根据Pack条码 工位编码 查找对应的工位
                    var steps = _flowService.GetStepsByProduct(entity).Result!.Select(s => s.StepId).ToList();
                    if (steps != null && steps.Count > 0)
                    {
                        var stationCodeList = stationCode.Split(",").ToList();
                        var stationList = _dbContext.Base_Stations.Include(s => s.Step).Where(s => stationCodeList.Contains(s.Code!) && steps.Contains(s.StepId) && s.IsDeleted == false).ToList();
                        foreach (var station in stationList)
                        {
                            // 查询配方数据
                            var mapping = await _stationTaskService.GetStationTaskDTOByStation(station, entity.Id);
                            if (mapping != null)
                            {
                                var config = new StationConfig
                                {
                                    Product = entity,
                                    StationTaskList = mapping
                                };
                                result.Result.Add(config);
                            }
                        }
                    }
                }
            }
            return result;
        }

        public async Task<Response<StationConfig>> LoadFormula(string stationCode, string ProductPn)
        {
            var response = new Response<StationConfig>();

            try
            {
                var config = new StationConfig();
                var product = await _dbContext.Products.FirstOrDefaultAsync(f => f.Code == ProductPn && !f.IsDeleted);
                if (product == null)
                {
                    response.Code = 500;
                    response.Message = $"未找到{ProductPn}的产品信息";
                    return response;
                }

                config.Product = product;

                var station = await _dbContext.Base_Stations.FirstOrDefaultAsync(f => f.Code == stationCode && !f.IsDeleted);
                if (station == null)
                {
                    response.Code = 500;
                    response.Message = $"未找到{stationCode}的工位信息";
                    return response;
                }
                var step = await _dbContext.Base_Steps.FirstOrDefaultAsync(f => f.Id == station.StepId && !f.IsDeleted);
                if (step == null)
                {
                    response.Code = 500;
                    response.Message = $"未找到{stationCode}的工序信息";
                    return response;
                }
                config.Station = station;
                config.Step = step;

                var tasks = await _dbContext.Base_StationTasks.Where(w => !w.IsDeleted && w.ProductId == product.Id && w.StepId == station.StepId).OrderBy(o => o.Sequence).ToListAsync();

                var stationTaskDto = new List<StationTaskDTO>();
                foreach (var task in tasks)
                {
                    var dto = new StationTaskDTO()
                    {
                        StepId = station.StepId,
                        StationID = station.Id,
                        StationTask = task,
                        StationTaskId = task.Id,
                        Sequence = task.Sequence,
                        HasFinish = false,
                    };
                    switch (task.Type)
                    {
                        case StationTaskTypeEnum.扫描员工卡:
                            dto.StationTaskScanAccountCard = await _dbContext.Base_StationTaskScanAccountCards.FirstOrDefaultAsync(f => f.StationTaskId == task.Id && !f.IsDeleted);
                            break;
                        case StationTaskTypeEnum.扫码:
                            dto.StationTaskBom = await _dbContext.Base_StationTaskBoms.Where(f => f.StationTaskId == task.Id && !f.IsDeleted).ToListAsync();
                            break;
                        case StationTaskTypeEnum.人工拧螺丝:
                            dto.StationTaskScrew = await _dbContext.Base_StationTaskScrews.Where(f => f.StationTaskId == task.Id && !f.IsDeleted).ToListAsync();
                            break;
                        case StationTaskTypeEnum.扫码输入:
                            dto.StationTaskScanCollect = await _dbContext.Base_StationTaskScanCollects.FirstOrDefaultAsync(f => f.StationTaskId == task.Id && !f.IsDeleted);
                            break;
                        case StationTaskTypeEnum.用户输入:
                            dto.StationTaskUserInput = await _dbContext.Base_StationTaskUserInputs.FirstOrDefaultAsync(f => f.StationTaskId == task.Id && !f.IsDeleted);
                            break;
                        case StationTaskTypeEnum.超时检测:
                            dto.StationTaskCheckTimeout = await _dbContext.Base_StationTaskCheckTimeOuts.FirstOrDefaultAsync(f => f.StationTaskId == task.Id && !f.IsDeleted);
                            break;
                        case StationTaskTypeEnum.时间记录:
                            dto.StationTask_RecordTime = await _dbContext.Base_StationTask_RecordTimes.FirstOrDefaultAsync(f => f.StationTaskId == task.Id && !f.IsDeleted);
                            break;
                        case StationTaskTypeEnum.称重:
                            dto.StationTaskAnyLoad = await _dbContext.Base_StationTaskAnyLoads.FirstOrDefaultAsync(f => f.StationTaskId == task.Id && !f.IsDeleted);
                            break;
                        case StationTaskTypeEnum.补拧:
                            dto.Base_StationTask_TightenRework = await _dbContext.Base_StationTask_TightenReworks.FirstOrDefaultAsync(f => f.StationTaskId == task.Id && !f.IsDeleted);
                            break;
                        case StationTaskTypeEnum.图示拧紧:
                            dto.Base_StationTask_TightenByImage = await _dbContext.Base_StationTask_TightenByImages.FirstOrDefaultAsync(f => f.StationTaskId == task.Id && !f.IsDeleted);
                            break;
                    }
                    stationTaskDto.Add(dto);
                }

                config.StationTaskList = stationTaskDto;
                response.Result = config;
                return response;
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Message = ex.Message;
                return response;
            }
        }

        public async Task<Response> PullStepBack(string agvNo, string stationCode, string packBarCode, string outCode)
        {
            var result = new Response();
            try
            {
                // 找到pack条码对应的Product
                var entityList = _dbContext.Products.Include(p => p.Type).Where(f => !string.IsNullOrEmpty(f.PackPNRule) && !f.IsDeleted).ToList();
                var entity = entityList.FirstOrDefault(f => f.PackPNRule.Length == packBarCode.Length && packBarCode.StartsWith(f.PackPNRule.TrimEnd('*')));
                if (entity == null)
                {
                    result.Code = 500;
                    result.Message = $"未找到Pack码{packBarCode}对应的产品";
                    return result;
                }
                // 读取AGV信息
                var agv = await _dbContext.Proc_AGVStatuss.FirstOrDefaultAsync(a => a.AGVNo.ToString() == agvNo && !a.IsDeleted);
                if (agv == null)
                {
                    result.Code = 500;
                    result.Message = $"未找到AGV小车{agvNo}";
                    return result;
                }
                agv.StationCode = stationCode;
                agv.PackPN = packBarCode;
                agv.HolderBarCode = outCode;
                _dbContext.SaveChanges();
                result.Code = 200;
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<Response<Alarm>> SelectBoltAlarm(string packCode, string stationCode)
        {
            try
            {
                var alarm = await _dbContext.Alarms.Where(a => a.StationCode == stationCode && a.IsFinish == true && a.PackCode == packCode).FirstOrDefaultAsync();
                if (alarm == null)
                {
                    return new Response<Alarm> { Code = 500, Result = alarm };
                }
                return new Response<Alarm> { Code = 200, Result = alarm };
            }
            catch (Exception ex)
            {
                return new Response<Alarm> { Code = 500, Message = ex.Message };
            }
        }

        public async Task<Response<Base_AutoStationTaskTighten>> SelectBoltUseNum(string stationCode, string packPN)
        {
            var result = new Response<Base_AutoStationTaskTighten>();

            try
            {

                //首先找到对应的step
                var station = await _stationService.GetStationByPackCode(stationCode, packPN);
                if (station == null)
                {
                    result.Code = 10003;
                    result.Message = $"未找到Pack码{packPN}对应的工位配方数据";
                    return result;
                }

                //然后找到对应的产品
                var entityList = _dbContext.Products.Include(p => p.Type).Where(f => !string.IsNullOrEmpty(f.PackPNRule) && !f.IsDeleted).ToList();
                var entity = entityList.FirstOrDefault(f => f.PackPNRule.Length == packPN.Length && packPN.StartsWith(f.PackPNRule.TrimEnd('*')));
                if (entity == null)
                {
                    result.Code = 500;
                    result.Message = $"未找到Pack码{packPN}对应的产品";
                    return result;
                }

                //然后找到对应的stationtask
                var stationtask = _dbContext.Base_StationTasks.FirstOrDefault(a => a.StepId == station.Id && a.ProductId == entity.Id && !a.IsDeleted);
                if (stationtask == null)
                {
                    result.Code = 500;
                    result.Message = $"未找到Pack码{packPN}对应的StationTask";
                    return result;
                }

                var stationTaskAuto = _dbContext.Base_AutoStationTaskTightens.FirstOrDefault(a => a.StationTaskId == stationtask.Id && !a.IsDeleted);
                if (stationTaskAuto == null)
                {
                    result.Code = 500;
                    result.Message = $"未找到Pack码{packPN}对应的自动拧紧螺丝数量信息";
                    return result;
                }
                result.Code = 200;
                result.Result = stationTaskAuto;
                return result;

            }
            catch (Exception ex)
            {

                result.Code = 500;
                result.Message = ex.Message;

            }
            return result;

        }
    }
}
