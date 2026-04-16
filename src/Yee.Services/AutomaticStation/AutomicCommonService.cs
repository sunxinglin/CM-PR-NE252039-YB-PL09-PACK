using AsZero.DbContexts;

using Catl.WebServices.MIFindCustomAndSfcData;

using Ctp0600P.Shared;

using Itminus.FSharpExtensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.FSharp.Core;

using Yee.Common.Library.CommonEnum;
using Yee.Entitys;
using Yee.Entitys.AutomaticStation;
using Yee.Entitys.CATL;
using Yee.Entitys.DBEntity;
using Yee.Entitys.Production;
using Yee.Services.AGV;
using Yee.Services.CatlMesInvoker;

namespace Yee.Services.AutomaticStation
{
    /// <summary>
    /// 自动站统一接口处理
    /// </summary>
    public class AutomicCommonService
    {
        private readonly AsZeroDbContext _dbContext;
        private readonly ICatlMesInvoker _catlMesInvoker;
        private readonly AGVService _agvService;
        private readonly IOptionsMonitor<CatlMesOpt> _catlMesOptsMonitor;

        public AutomicCommonService(AsZeroDbContext dbContext, ICatlMesInvoker catlMesInvoker, AGVService agvService, IOptionsMonitor<CatlMesOpt> catlMesOptsMonitor)
        {
            _dbContext = dbContext;
            _catlMesInvoker = catlMesInvoker;
            _agvService = agvService;
            _catlMesOptsMonitor = catlMesOptsMonitor;
        }

        public async Task<FSharpResult<ServiceErrResponse, ServiceErrResponse>> CheckVectorBindAndFlowOrder(string packCode, int vectorCode, string stationCode, string VectorStation, string productPn, string? packOutCode)
        {
            try
            {
                var resp = new ServiceErrResponse();

                var paramsOK = !string.IsNullOrEmpty(packCode) && vectorCode > 0 && !string.IsNullOrEmpty(stationCode);
                if (!paramsOK)
                {
                    var ErrorMessage = string.Format("packCode-{0}，小车号-{1}，stationCode-{2}上传参数有误，请验证参数后重试", packCode, vectorCode, stationCode);
                    return resp.ToError(ResponseErrorType.上位机错误, 500, ErrorMessage).ToErrResult<ServiceErrResponse, ServiceErrResponse>();
                }

                var agvInfo = await _dbContext.Proc_AGVStatuss.Where(e => e.AGVNo == vectorCode).FirstOrDefaultAsync();
                if (agvInfo == null)
                {
                    var ErrorMessage = $"载具{vectorCode}不存在";
                    return resp.ToError(ResponseErrorType.载具绑定错误, 500, ErrorMessage).ToErrResult<ServiceErrResponse, ServiceErrResponse>();
                }

                #region 加载流程顺序信息

                var product = await _dbContext.Products.FirstOrDefaultAsync(f => !f.IsDeleted && f.Code == productPn);
                if (product == null)
                {
                    var ErrorMessage = $"产品{productPn}不存在";
                    return resp.ToError(ResponseErrorType.上位机错误, 500, ErrorMessage).ToErrResult<ServiceErrResponse, ServiceErrResponse>();
                }
                var flow = await _dbContext.Base_Flows.FirstOrDefaultAsync(f => !f.IsDeleted && f.ProductId == product.Id);
                if (flow == null)
                {
                    var ErrorMessage = $"产品{productPn}未配置工序流程";
                    return resp.ToError(ResponseErrorType.上位机错误, 500, ErrorMessage).ToErrResult<ServiceErrResponse, ServiceErrResponse>();
                }
                var flowMap = await _dbContext.Base_FlowStepMappings.Where(w => !w.IsDeleted && w.FlowId == flow.Id).OrderBy(o => o.OrderNo).ToListAsync();
                var step = await _dbContext.Base_Stations.Include(i => i.Step).Where(f => !f.IsDeleted && f.Code == stationCode).Select(s => s.Step).FirstOrDefaultAsync();
                if (step == null)
                {
                    var ErrorMessage = $"当前工位没有配置对应工位信息";
                    return resp.ToError(ResponseErrorType.上位机错误, 500, ErrorMessage).ToErrResult<ServiceErrResponse, ServiceErrResponse>();
                }
                var currentMap = flowMap.FirstOrDefault(f => f.StepId == step.Id);
                if (currentMap == null)
                {
                    var ErrorMessage = $"当前工位没有在产品{productPn}的工艺路线中";
                    return resp.ToError(ResponseErrorType.上位机错误, 500, ErrorMessage).ToErrResult<ServiceErrResponse, ServiceErrResponse>();
                }

                var prevStep = flowMap.Where(w => w.OrderNo < currentMap.OrderNo).LastOrDefault();
                if (prevStep == null)
                {
                    return resp.ToSuccess().ToOkResult<ServiceErrResponse, ServiceErrResponse>();
                }
                #endregion

                if (agvInfo.PackPN != packCode)
                {
                    var ErrorMessage = $"载具{vectorCode}绑定的条码[{agvInfo.PackPN}]与当前条码[{packCode}]不匹配";
                    return resp.ToError(ResponseErrorType.载具绑定错误, 500, ErrorMessage).ToErrResult<ServiceErrResponse, ServiceErrResponse>();
                }

                var main = await _dbContext.Proc_StationTask_Mains.FirstOrDefaultAsync(f => !f.IsDeleted && f.StepId == prevStep.StepId && f.Status == StationTaskStatusEnum.已完成 && f.PackCode == packCode);
                if (main == null)
                {
                    var ErrorMessage = $"前置工序未完成";
                    return resp.ToError(ResponseErrorType.上位机错误, 500, ErrorMessage).ToErrResult<ServiceErrResponse, ServiceErrResponse>();
                }

                return resp.ToSuccess().ToOkResult<ServiceErrResponse, ServiceErrResponse>();
            }
            catch (Exception ex)
            {
                var resp = new ServiceErrResponse();
                var ErrorMessage = ex.Message;
                return resp.ToError(ResponseErrorType.上位机错误, 500, ErrorMessage).ToErrResult<ServiceErrResponse, ServiceErrResponse>();
            }
        }

        public async Task<FSharpResult<ServiceErrResponse, ServiceErrResponse>> CheckFlowOrder(string packCode, string stationCode, string productPn)
        {
            try
            {
                var resp = new ServiceErrResponse();

                var paramsOK = !string.IsNullOrEmpty(packCode) && !string.IsNullOrEmpty(stationCode);
                if (!paramsOK)
                {
                    var ErrorMessage = string.Format($"上传参数有误，请验证参数后重试。packCode-{packCode}，stationCode-{stationCode}");
                    return resp.ToError(ResponseErrorType.上位机错误, 500, ErrorMessage).ToErrResult<ServiceErrResponse, ServiceErrResponse>();
                }

                var product = await _dbContext.Products.FirstOrDefaultAsync(f => !f.IsDeleted && f.Code == productPn);
                if (product == null)
                {
                    var ErrorMessage = $"产品{productPn}不存在";
                    return resp.ToError(ResponseErrorType.上位机错误, 500, ErrorMessage).ToErrResult<ServiceErrResponse, ServiceErrResponse>();
                }
                var flow = await _dbContext.Base_Flows.FirstOrDefaultAsync(f => !f.IsDeleted && f.ProductId == product.Id);
                if (flow == null)
                {
                    var ErrorMessage = $"产品{productPn}未配置工序流程";
                    return resp.ToError(ResponseErrorType.上位机错误, 500, ErrorMessage).ToErrResult<ServiceErrResponse, ServiceErrResponse>();
                }
                var flowMap = await _dbContext.Base_FlowStepMappings.Where(w => !w.IsDeleted && w.FlowId == flow.Id).OrderBy(o => o.OrderNo).ToListAsync();
                var step = await _dbContext.Base_Stations.Include(i => i.Step).Where(f => !f.IsDeleted && f.Code == stationCode).Select(s => s.Step).FirstOrDefaultAsync();
                if (step == null)
                {
                    var ErrorMessage = $"当前工位没有配置对应工位信息";
                    return resp.ToError(ResponseErrorType.上位机错误, 500, ErrorMessage).ToErrResult<ServiceErrResponse, ServiceErrResponse>();
                }
                var currentMap = flowMap.FirstOrDefault(f => f.StepId == step.Id);
                if (currentMap == null)
                {
                    var ErrorMessage = $"当前工位没有在产品{productPn}的工艺路线中";
                    return resp.ToError(ResponseErrorType.上位机错误, 500, ErrorMessage).ToErrResult<ServiceErrResponse, ServiceErrResponse>();
                }

                var prevStep = flowMap.Where(w => w.OrderNo < currentMap.OrderNo).LastOrDefault();
                if (prevStep == null)
                {
                    return resp.ToSuccess().ToOkResult<ServiceErrResponse, ServiceErrResponse>();
                }

                var main = await _dbContext.Proc_StationTask_Mains.FirstOrDefaultAsync(f => !f.IsDeleted && f.StepId == prevStep.StepId && f.Status == StationTaskStatusEnum.已完成 && f.PackCode == packCode);
                if (main == null)
                {
                    var ErrorMessage = $"前置工序未完成";
                    return resp.ToError(ResponseErrorType.上位机错误, 500, ErrorMessage).ToErrResult<ServiceErrResponse, ServiceErrResponse>();
                }

                return resp.ToSuccess().ToOkResult<ServiceErrResponse, ServiceErrResponse>();
            }
            catch (Exception ex)
            {
                var resp = new ServiceErrResponse();
                var ErrorMessage = ex.Message;
                return resp.ToError(ResponseErrorType.上位机错误, 500, ErrorMessage).ToErrResult<ServiceErrResponse, ServiceErrResponse>();
            }
        }

        public async Task<FSharpResult<Proc_StationTask_Main, ServiceErrResponse>> CreateMessionRecord(CreateMessionRecordDto dto)
        {
            try
            {
                var resp = new ServiceErrResponse();
                var productPn = dto.ProductPn;
                var product = await _dbContext.Products.FirstOrDefaultAsync(f => f.Code == productPn && !f.IsDeleted);
                if (product == null)
                {
                    return resp.ToError(ResponseErrorType.上位机错误, 500, $"产品{productPn}不存在！").ToErrResult<Proc_StationTask_Main, ServiceErrResponse>();
                }
                var station = await _dbContext.Base_Stations.Include(i => i.Step).FirstOrDefaultAsync(f => !f.IsDeleted && f.Code == dto.StationCode);
                if (station == null)
                {
                    return resp.ToError(ResponseErrorType.上位机错误, 500, $"工位不存在！").ToErrResult<Proc_StationTask_Main, ServiceErrResponse>();
                }
                var main = await _dbContext.Proc_StationTask_Mains.FirstOrDefaultAsync(f => !f.IsDeleted && f.PackCode == dto.Pin && f.StepId == station.StepId);
                if (main == null)
                {
                    main = new Proc_StationTask_Main()
                    {
                        PackCode = dto.Pin,
                        StationId = station.Id,
                        StepId = station.StepId,
                        StationCode = dto.StationCode,
                        Status = StationTaskStatusEnum.进行中,
                        ProductId = product.Id,
                    };
                    await _dbContext.AddAsync(main);
                    await _dbContext.SaveChangesAsync();
                    return main.ToOkResult<Proc_StationTask_Main, ServiceErrResponse>();
                }
                main.Status = StationTaskStatusEnum.进行中;
                _dbContext.Update(main);
                await _dbContext.SaveChangesAsync();
                return main.ToOkResult<Proc_StationTask_Main, ServiceErrResponse>();
            }
            catch (Exception ex)
            {
                var resp = new ServiceErrResponse();
                return resp.ToError(ResponseErrorType.上位机错误, 500, ex.Message).ToErrResult<Proc_StationTask_Main, ServiceErrResponse>();
            }
        }

        public async Task<FSharpResult<CatlMESReponse, ServiceErrResponse>> GetProductPnFromCatlMes(string packCode, string stationCode)
        {
            //判断是否启用CATL
            if (!_catlMesOptsMonitor.CurrentValue.Enabled)
            {
                return new CatlMESReponse { code = 0, BarCode_GoodsPN = "00000" }.ToOkResult<CatlMESReponse, ServiceErrResponse>();
            }

            var catlResp = await _catlMesInvoker.MiFindCustomAndSfcData(packCode, modeProcessSFC.MODE_NONE, stationCode);
            if (catlResp.code != 0)
            {
                return new ServiceErrResponse() { ErrorCode = catlResp.code, ErrorMessage = catlResp.message, ErrorType = ResponseErrorType.CatlMes错误, IsError = true }.ToErrResult<CatlMESReponse, ServiceErrResponse>();
            }
            return catlResp.ToOkResult<CatlMESReponse, ServiceErrResponse>();
        }

        public async Task<FSharpResult<CatlMESReponse, ServiceErrResponse>> MakePackStart(string packCode, string stationCode)
        {
            //var IsNeedStart = await _catlMesInvoker.CheckSfcStatu(packCode, stationCode);
            var catlResp = await _catlMesInvoker.MiFindCustomAndSfcData(packCode, null, stationCode);//modeProcessSFC为null时，接口按照工位配置来
            if (catlResp.code != 0)
            {
                return new ServiceErrResponse() { ErrorCode = catlResp.code, ErrorMessage = catlResp.message, ErrorType = ResponseErrorType.CatlMes错误, IsError = true }.ToErrResult<CatlMESReponse, ServiceErrResponse>();
            }
            return catlResp.ToOkResult<CatlMESReponse, ServiceErrResponse>();
        }

        public async Task<FSharpResult<Proc_StationTask_Main, ServiceErrResponse>> HasMainRecord(string packCode, string stationCode)
        {
            var resp = new ServiceErrResponse();
            var station = await _dbContext.Base_Stations.Include(e => e.Step).FirstOrDefaultAsync(f => f.Code == stationCode);
            if (station == null)
            {
                var ErrorMessage = $"工位{stationCode}不存在";
                return resp.ToError(ResponseErrorType.上位机错误, 500, ErrorMessage).ToErrResult<Proc_StationTask_Main, ServiceErrResponse>();
            }
            var main = await _dbContext.Proc_StationTask_Mains.FirstOrDefaultAsync(w => !w.IsDeleted && w.PackCode == packCode && w.StepId == station.StepId);
            if (main == null)
            {
                var ErrorMessage = $"PACK码{packCode}没有在工位{stationCode}的开始记录";
                return resp.ToError(ResponseErrorType.上位机错误, 500, ErrorMessage).ToErrResult<Proc_StationTask_Main, ServiceErrResponse>();
            }
            if (main.Status == StationTaskStatusEnum.已完成)
            {
                var ErrorMessage = $"PACK码{packCode}在工位{stationCode}已完工，请勿重复发起请求";
                return resp.ToError(ResponseErrorType.上位机错误, 500, ErrorMessage).ToErrResult<Proc_StationTask_Main, ServiceErrResponse>();
            }
            return main.ToOkResult<Proc_StationTask_Main, ServiceErrResponse>();
        }

        public async Task<FSharpResult<Proc_StationTask_Main, ServiceErrResponse>> SetStationComplete(int mainId)
        {
            try
            {
                var main = await _dbContext.Proc_StationTask_Mains.FirstOrDefaultAsync(x => x.Id == mainId);
                if (main == null)
                {
                    var resp = new ServiceErrResponse();
                    return resp.ToError(ResponseErrorType.上位机错误, 500, "未查询到主记录").ToErrResult<Proc_StationTask_Main, ServiceErrResponse>();
                }
                main.Status = StationTaskStatusEnum.已完成;
                _dbContext.Update(main);
                await _dbContext.SaveChangesAsync();
                return main.ToOkResult<Proc_StationTask_Main, ServiceErrResponse>();
            }
            catch (Exception ex)
            {
                var resp = new ServiceErrResponse();
                return resp.ToError(ResponseErrorType.上位机错误, 500, ex.Message).ToErrResult<Proc_StationTask_Main, ServiceErrResponse>();
            }
        }

        public async Task<(string, int)> GetCurrentStation(string stationFlag, string packCode)
        {
            try
            {
                var stations = await _dbContext.ActiveStationRelationships.Where(w => w.StationFlag == stationFlag).OrderBy(o => o.StationLevel).ToListAsync();
                if (stations.Count == 1)
                {
                    var st = stations.First();
                    return (st.StationCode, st.StationLevel);
                }
                var baseStations = await _dbContext.Base_Stations.Where(w => !w.IsDeleted).ToListAsync();
                var mains = await _dbContext.Proc_StationTask_Mains.Where(w => !w.IsDeleted && w.PackCode == packCode).ToListAsync();
                foreach (var item in stations)
                {
                    var station = baseStations.FirstOrDefault(f => f.Code == item.StationCode);
                    if (station == null)
                    {
                        continue;
                    }
                    var main = mains.FirstOrDefault(f => f.StationId == station.Id);
                    if (main == null || main.Status != StationTaskStatusEnum.已完成)
                    {
                        return (item.StationCode ?? "", item.StationLevel);
                    }
                }
                return ("", 0);
            }
            catch
            {
                return ("", 0);
            }
        }

        public async Task<FSharpResult<IList<Base_StationTask>, ServiceErrResponse>> GetBaseStationTasks(string productPn, string stationCode)
        {
            try
            {
                var resp = new ServiceErrResponse();
                var product = await _dbContext.Products.FirstOrDefaultAsync(f => f.Code == productPn && !f.IsDeleted);
                var station = await _dbContext.Base_Stations.FirstOrDefaultAsync(f => f.Code == stationCode && !f.IsDeleted);
                if (product == null)
                {
                    return resp.ToError(ResponseErrorType.上位机错误, 500, $"产品{productPn}不存在").ToErrResult<IList<Base_StationTask>, ServiceErrResponse>();
                }
                if (station == null)
                {
                    return resp.ToError(ResponseErrorType.上位机错误, 500, $"工位{stationCode}不存在").ToErrResult<IList<Base_StationTask>, ServiceErrResponse>();
                }
                var stepId = station.StepId;
                var stationTasks = await _dbContext.Base_StationTasks.Where(w => !w.IsDeleted && w.ProductId == product.Id && w.StepId == stepId).ToListAsync();
                return stationTasks.ToOkResult<IList<Base_StationTask>, ServiceErrResponse>();
            }
            catch (Exception ex)
            {
                var resp = new ServiceErrResponse();
                return resp.ToError(ResponseErrorType.上位机错误, 500, ex.Message).ToErrResult<IList<Base_StationTask>, ServiceErrResponse>();
            }
        }

        public async Task<FSharpResult<CatlMESReponse, ServiceErrResponse>> UploadCatlMes(string sfc, IList<DcParamValue> dcParams, bool needChangeStatus, string stationCode = "")
        {
            try
            {
                var response = await _catlMesInvoker.dataCollect(sfc, dcParams, needChangeStatus, stationCode);
                if (response.code != 0)
                {
                    var resp = new ServiceErrResponse();
                    return resp.ToError(ResponseErrorType.CatlMes错误, response.code, response.message).ToErrResult<CatlMESReponse, ServiceErrResponse>();
                }
                return response.ToOkResult<CatlMESReponse, ServiceErrResponse>();
            }
            catch (Exception ex)
            {
                var resp = new ServiceErrResponse();
                return resp.ToError(ResponseErrorType.上位机错误, 500, ex.Message).ToErrResult<CatlMESReponse, ServiceErrResponse>();
            }
        }

        public async Task<FSharpResult<ServiceErrResponse, ServiceErrResponse>> CheckLastBoxLevel(string StationFlag, string PackCode, int CurrentLevel)
        {
            var resp = new ServiceErrResponse();
            try
            {
                var LevelFlag = await _dbContext.ActiveLastBoxTypes.FirstOrDefaultAsync(f => f.StationFlag == StationFlag);
                if (LevelFlag == null)
                {
                    LevelFlag = new ActiveLastBoxType
                    {
                        StationFlag = StationFlag,
                        BoxLevel = 0,
                    };
                    await _dbContext.AddAsync(LevelFlag);
                    await _dbContext.SaveChangesAsync();
                    return resp.ToSuccess().ToOkResult<ServiceErrResponse, ServiceErrResponse>();
                }

                if (LevelFlag.PackCode == PackCode)
                {
                    return resp.ToSuccess().ToOkResult<ServiceErrResponse, ServiceErrResponse>();
                }

                if (LevelFlag.BoxLevel == CurrentLevel)
                {
                    return resp.ToError(ResponseErrorType.流程顺序错误, 500, $"当前Pack不满足上下层顺序").ToOkResult<ServiceErrResponse, ServiceErrResponse>();
                }

                return resp.ToSuccess().ToOkResult<ServiceErrResponse, ServiceErrResponse>();

            }
            catch (Exception ex)
            {
                return resp.ToError(ResponseErrorType.上位机错误, 500, $"{ex.Message}").ToOkResult<ServiceErrResponse, ServiceErrResponse>();
            }
        }

        public async Task<FSharpResult<ServiceErrResponse, ServiceErrResponse>> ChangeLastBoxLevel(string StationFlag, string PackCode, int CurrentLevel)
        {
            var resp = new ServiceErrResponse();
            try
            {
                var LevelFlag = await _dbContext.ActiveLastBoxTypes.FirstOrDefaultAsync(f => f.StationFlag == StationFlag);
                if (LevelFlag == null)
                {
                    LevelFlag = new ActiveLastBoxType
                    {
                        StationFlag = StationFlag,
                        PackCode = PackCode,
                        BoxLevel = CurrentLevel
                    };
                    await _dbContext.AddAsync(LevelFlag);
                    await _dbContext.SaveChangesAsync();
                    return resp.ToSuccess().ToOkResult<ServiceErrResponse, ServiceErrResponse>();
                }

                LevelFlag.PackCode = PackCode;
                LevelFlag.BoxLevel = CurrentLevel;

                _dbContext.Update(LevelFlag);
                await _dbContext.SaveChangesAsync();

                return resp.ToSuccess().ToOkResult<ServiceErrResponse, ServiceErrResponse>();

            }
            catch (Exception ex)
            {
                return resp.ToError(ResponseErrorType.上位机错误, 500, $"{ex.Message}").ToOkResult<ServiceErrResponse, ServiceErrResponse>();
            }
        }

        public async Task<FSharpResult<ServiceErrResponse, ServiceErrResponse>> CheckBomInventory(string packCode, string materialPN, string materialCode, int useNum, string stationCode)
        {
            var resp = new ServiceErrResponse();
            try
            {
                var result = await _catlMesInvoker.MICheckBOMInventory(packCode, materialPN, materialCode, useNum, stationCode);
                if (result.code != 0)
                {
                    return resp.ToError(ResponseErrorType.CatlMes错误, result.code, $"{result.message}").ToOkResult<ServiceErrResponse, ServiceErrResponse>();
                }
                return resp.ToSuccess().ToOkResult<ServiceErrResponse, ServiceErrResponse>();
            }
            catch (Exception ex)
            {
                return resp.ToError(ResponseErrorType.上位机错误, 500, $"{ex.Message}").ToOkResult<ServiceErrResponse, ServiceErrResponse>();
            }
        }

        public async Task<int> GetPackMAT(string productPn)
        {
            try
            {
                var product = await _dbContext.Products.Include(i => i.Type).FirstOrDefaultAsync(f => f.Code == productPn && !f.IsDeleted);
                return product?.Type?.Value ?? 0;
            }
            catch
            {
                return 0;
            }
        }

        public async Task<CatlMESReponse> CheckSfcStatus(string packCode, string stationCode)
        {
            return await _catlMesInvoker.CheckSfcStatu(packCode, stationCode);
        }
    }
}
