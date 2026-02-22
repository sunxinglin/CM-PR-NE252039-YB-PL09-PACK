using AsZero.Core.Services.Repos;
using AsZero.DbContexts;
using Ctp0600P.Shared.NotificationDTO;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Yee.Common.Library.CommonEnum;
using Yee.Entitys.CommonEntity;
using Yee.Entitys.DBEntity;
using Yee.Entitys.DTOS;
using Yee.Services.AGV;
using Yee.Services.Production;
using Yee.Services.Request;

namespace Yee.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AGVController : ControllerBase
    {
        private readonly ILogger<AGVController> _logger;
        public readonly AsZeroDbContext _dBContext;
        public readonly AGVService _agvService;
        public readonly SaveStationDataService _SaveStationDataService;
        private readonly IMediator _mediator;

        /// <summary>
        /// 构造函数
        /// </summary>
        public AGVController(AsZeroDbContext dBContext, ILogger<AGVController> logger, AGVService agvService, IMediator mediator, SaveStationDataService saveStationDataService)
        {
            _dBContext = dBContext;
            _logger = logger;
            _agvService = agvService;
            _SaveStationDataService = saveStationDataService;
            _mediator = mediator;
        }

        /// <summary>
        /// AGV到达信号
        /// </summary>
        [HttpPost]
        public async Task<Response> AGVArrived(AGVMessage agvMsg)
        {
            var result = new Response();
            try
            {
                _logger.LogInformation($"收到AGV到达信息：{JsonConvert.SerializeObject(agvMsg)}");
                await _agvService.AGVArrived(agvMsg);
                result.Code = 200;

                await _mediator.Publish(new AgvMsgContextNotification() { Action = AgvActionEnum.进站, PackOutCode = agvMsg.HolderBarCode, AgvNo = agvMsg.AgvCode, StationCode = agvMsg.StationName });
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        /// <summary>
        /// AGV离开信号
        /// </summary>
        [HttpPost]
        public async Task<Response> AGVLeaved(AGVMessage agvMsg)
        {
            var result = new Response();
            try
            {
                _logger.LogInformation($"收到AGV离开信息：{JsonConvert.SerializeObject(agvMsg)}");
                _agvService.AGVLeaved(agvMsg);
                result.Code = 200;
                await _mediator.Publish(new AgvMsgContextNotification() { Action = AgvActionEnum.离站, AgvNo = agvMsg.AgvCode, StationCode = agvMsg.StationName });
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 放行AGV
        /// </summary>
        /// <param name="agvCode">AGV车号</param>
        /// <param name="releaseType">类型 放行为1</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<Response> RunAGV(string agvCode, int releaseType)
        {
            _logger.LogInformation($"放行AGV{agvCode}");
            Response response = new Response();
            response = await _agvService.RunAGV(agvCode, releaseType);
            return response;
        }

        [HttpGet]
        public async Task<Response> RunAGVByStepCode(string stepCode, int releaseType)
        {
            var agv = _dBContext.Proc_AGVStatuss.FirstOrDefault(p => p.StationCode == stepCode && !p.IsDeleted);
            _logger.LogInformation($"放行AGV{agv.AGVNo}");
            Response response = new Response();
            var result = await _agvService.RunAGV(agv.AGVNo.ToString(), releaseType);
            //if (!result.Item1)
            //{
            //    response.Code = 500;
            //    response.Message = result.Item2;
            //}
            return response;
        }

        /// <summary>
        /// 添加
        /// </summary>
        [HttpPost]
        public async Task<Response> Add(Proc_AGVStatus obj)
        {
            var result = new Response<Proc_AGVStatus>();
            try
            {
                if (await _agvService.HasCode(obj.AGVNo) == null)
                {
                    var user = Request.Cookies.Where(p => p.Key == "SET_NAME").First().Value.ToString();
                    var newObj = await _agvService.Add(obj, user);
                    result.Result = newObj;
                }
                else
                {
                    result.Code = 500;
                    result.Message = "该编号已存在，请重新输入一个新的编码";
                }

            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }

            return result;

        }

        /// <summary>
        /// 修改
        /// </summary>
        [HttpPost]
        public async Task<Response> Update(Proc_AGVStatus obj)
        {
            var result = new Response<Proc_AGVStatus>();
            try
            {

                var res = await _agvService.Update(obj);
                result.Result = res;
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }

            return result;
        }

        /// <summary>
        /// 删除
        /// </summary>
        [HttpPost]
        public async Task<Response<string>> DeleteEntity(int[] input)
        {
            var user = Request.Cookies.Where(p => p.Key == "SET_NAME").First().Value.ToString();
            var result = new Response<string>();
            try
            {
                foreach (var Id in input)
                {
                    var entity = await _agvService.GetById(Id);
                    if (entity != null)
                    {
                        await _agvService.Delete(entity, user);
                    }
                }
                result.Message = "操作成功";
                return result;
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }

            return result;
        }

        /// <summary>
        /// 获取全部AGV绑定信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<Response<List<Proc_AGVStatus>>> Load(GetByKeyInput keyInput)
        {
            Response<List<Proc_AGVStatus>> response = new Response<List<Proc_AGVStatus>>();
            try
            {
                var list = await _agvService.Load(keyInput);
                response.Data = list.Skip((keyInput.Page - 1) * keyInput.Limit).Take(keyInput.Limit);
                response.Count = list.Count;
            }
            catch (Exception ex)
            {

                response.Code = 500;
                response.Message = $"查询全部AGV绑定信息时出现错误{ex.Message}";
                _logger.LogError(response.Message);
            }
            return response;
        }

        [HttpGet]
        public async Task<Response<Proc_AGVStatus>> LoadStationCurrentAGV(string code)
        {
            var result = new Response<Proc_AGVStatus>();
            try
            {
                var agv = await _agvService.LoadStationCurrentAGV(code);
                result.Result = agv;
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
        public async Task<Response<StationPack_AGV_Task_Record_DTO>> CheckPack_AGV_Task_Record(int productId, int stepId, string stationCode, string agvNo, string packPN)
        {
            var result = new Response<StationPack_AGV_Task_Record_DTO>();
            try
            {
                var agv = await _agvService.CheckPack_AGV_Task_Record(productId, stepId, stationCode, agvNo, packPN);
                result = agv;
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        [HttpGet]
        public async Task<Response<List<StationConfig>>> LoadStationAllPeiFangData(string stationCode)
        {
            var result = new Response<List<StationConfig>>();
            try
            {
                result = await _agvService.LoadStationAllPeiFangData(stationCode);
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        [HttpGet]
        public async Task<Response<StationConfig>> LoadFormula(string stationCode, string ProductPn)
        {
            return await _agvService.LoadFormula(stationCode, ProductPn);
        }

        /// <summary>
        /// 绑定与解绑AGV与产品条码
        /// </summary>
        /// <param name="state">绑定还是解绑</param>
        /// <param name="agvcode">agv号</param>
        /// <param name="packPN">pacj条码</param>
        /// <param name="producttype">pack类型</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<Response<string>> BingAgv(BingAgvDTO dTO)
        {
            Response<string> response = new Response<string>();
            try
            {
                string users = "";
                try
                {
                    var user = Request.Cookies.Where(p => p.Key == "SET_NAME").First().Value.ToString();
                    users = user;
                }
                catch (Exception)
                {

                }

                if (dTO == null)
                {
                    response.Code = 500;
                    response.Message = "绑定数据为空!";
                }
                else
                {
                    var result = await _agvService.BingAgv(dTO, users);
                    if (!result.Item1)
                    {
                        response.Code = 500;
                        response.Message = result.Item2;
                    }
                    await _mediator.Publish(new AgvMsgContextNotification() { Action = AgvActionEnum.绑码, PackOutCode = dTO.HolderBarCode, AgvNo = dTO.AgvCode, StationCode = dTO.StationCode });
                }
            }
            catch (Exception ex)
            {

                response.Code = 500;
                response.Message = $"绑定AGV时出现错误{ex.Message}";
                _logger.LogError(response.Message);
            }
            return response;
        }

        [HttpGet]
        public async Task<Response> PullStepBack(string agvNo, string stationCode, string packBarCode, string outCode)
        {
            Response response = new Response();
            try
            {
                response = await _agvService.PullStepBack(agvNo, stationCode, packBarCode, outCode);
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Message = $"拉站时出现错误{ex.Message}";
                _logger.LogError(response.Message);
            }
            return response;
        }

        [HttpPost]
        public async Task<Response<string>> BingAgv_SBox(BingAgvDTO dTO)
        {
            Response<string> response = new Response<string>();
            try
            {
                if (dTO == null)
                {
                    response.Code = 500;
                    response.Message = "绑定数据为空!";
                }
                else
                {
                    var agv = await _dBContext.Proc_AGVStatuss.Where(a => !a.IsDeleted && a.AGVNo == dTO.AgvCode).FirstOrDefaultAsync();
                    if (agv == null)
                    {
                        agv = new Proc_AGVStatus
                        {
                            AGVNo = dTO.AgvCode,
                            PackCode = dTO.PackPN,
                            StationCode = dTO.StationCode,
                            ProductType = dTO.ProductType,
                            Behavior = 0,
                        };
                        _dBContext.Proc_AGVStatuss.Add(agv);
                    }
                    else
                    {
                        agv.AGVNo = dTO.AgvCode;
                        agv.PackCode = dTO.PackPN;
                        agv.StationCode = dTO.StationCode;
                        agv.ProductType = dTO.ProductType;
                        agv.Behavior = 0;
                        _dBContext.Proc_AGVStatuss.Update(agv);
                    }
                    _dBContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {

                response.Code = 500;
                response.Message = $"绑定AGV时出现错误{ex.Message}";
                _logger.LogError(response.Message);
            }
            return response;
        }

        [HttpGet]
        public async Task<Response<StationPack_AGV_Task_Record_DTO>> CheckPack_AGV_Task_Record_Auto(string stepCode, string stationCode, string agvNo, string packPN, bool askIn)
        {
            try
            {
                var response = await _agvService.CheckPack_AGV_Task_Record_Auto(stepCode, stationCode, agvNo, packPN, askIn);
                return response;
            }
            catch (Exception ex)
            {
                return new Response<StationPack_AGV_Task_Record_DTO> { Code = 500, Message = ex.Message };
            }
        }

    }
}



