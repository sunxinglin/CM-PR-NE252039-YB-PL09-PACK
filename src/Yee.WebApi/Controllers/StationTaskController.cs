using AsZero.Core.Services.Repos;

using Microsoft.AspNetCore.Mvc;

using Yee.Entitys.DTOS;
using Yee.Entitys.Production;
using Yee.Services.Production;
using Yee.Services.Request;

namespace Yee.WebApi.Controllers
{
    /// <summary>
    /// 工位任务
    /// </summary>
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class StationTaskController : ControllerBase
    {

        private readonly StationTaskService _stationTaskService;
        private readonly StationTaskBomService stationTaskBomService;
        private readonly Base_StationTaskAnyLoadService baseStationTaskAnyLoadService;
        private readonly StationTaskScrewService stationTaskScrewService;
        private readonly Base_StationTaskGluingTimeService baseStationTaskGluingTimeService;
        private readonly StationService _stationService;
        public StationTaskController(StationService stationService, StationTaskService stationTaskService, StationTaskBomService stationTaskBomService, Base_StationTaskAnyLoadService baseStationTaskAnyLoadService, StationTaskScrewService stationTaskScrewService, Base_StationTaskGluingTimeService baseStationTaskGluingTimeService)
        {
            this._stationTaskService = stationTaskService;
            this.stationTaskBomService = stationTaskBomService;
            this.baseStationTaskAnyLoadService = baseStationTaskAnyLoadService;
            this.stationTaskScrewService = stationTaskScrewService;
            this.baseStationTaskGluingTimeService = baseStationTaskGluingTimeService;
            _stationService = stationService;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<Response<List<Base_StationTask>>> Load([FromQuery] GetByKeyInput input)
        {
            var result = new Response<List<Base_StationTask>>();
            try
            {
                var list = await _stationTaskService.GetAll(input.Key);
                result.Data = list.Skip((input.Page - 1) * input.Limit).Take(input.Limit);
                result.Count = list.Count;
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }
        [HttpGet]
        public async Task<Response<List<Base_StationTask>>> LoadTaskByProductStepId(int stepId, int productId)
        {
            var result = new Response<List<Base_StationTask>>();
            try
            {
                var list = await this._stationTaskService.GetStationTaskByStep(stepId, productId);
                result.Data = list;
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
        public async Task<Response> Update(Base_StationTask obj)
        {
            var result = new Response<Base_StationTask?>();
            try
            {
                var user = Request.Cookies.Where(p => p.Key == "SET_NAME").First().Value.ToString();

                var res = await _stationTaskService.Update(obj, user);
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
        public async Task<Response<string>> DeleteEntity(DeleteByIdsInput input)
        {
            var result = new Response<string>();
            try
            {
                var user = Request.Cookies.Where(p => p.Key == "SET_NAME").First().Value.ToString();
                foreach (var Id in input.Ids)
                {
                    var entity = await _stationTaskService.GetById(Id);

                    if (entity != null)
                    {
                        ///判断是否存在子任务
                        var bom = await this.stationTaskBomService.GetBomListByStationTask(Id);
                        if (bom.Count != 0)
                        {
                            result.Code = 500;
                            result.Message = $"该任务{entity.Name}存在扫码任务,请删除后重试!";
                            return result;
                        }
                        var anyload = await this.baseStationTaskAnyLoadService.GetStationAnyloadTaskByTaskid(Id);
                        if (anyload.Count != 0)
                        {
                            result.Code = 500;
                            result.Message = $"该任务{entity.Name}存在称重,请删除后重试!";
                            return result;
                        }
                        var screw = await this.stationTaskScrewService.GetScrewListByStationTaskID(Id);
                        if (screw.Count != 0)
                        {
                            result.Code = 500;
                            result.Message = $"该任务{entity.Name}存在拧螺丝任务,请删除后重试!";
                            return result;
                        }
                        var gluingtime = await this.baseStationTaskGluingTimeService.GetStationAnyloadTaskByTaskid(Id);
                        if (gluingtime.Count != 0)
                        {
                            result.Code = 500;
                            result.Message = $"该任务{entity.Name}存在超时检测,请删除后重试!";
                            return result;
                        }
                        await _stationTaskService.Delete(entity, user);

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
        /// 添加
        /// </summary>
        [HttpPost]
        public async Task<Response> Add(Base_StationTask obj)
        {
            var result = new Response<Base_StationTask>();
            try
            {
                var user = Request.Cookies.Where(p => p.Key == "SET_NAME").First().Value.ToString();
                var list = await this._stationTaskService.GetStationTaskByStep(obj.StepId, obj.ProductId);
                obj.Sequence = list.Count + 1;
                obj.HasPage = true;
                var newObj = await _stationTaskService.Add(obj, user);
                result.Result = newObj;

            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }

            return result;

        }
        [HttpPost]
        /// <summary>
        /// 更新工位任务顺序
        /// </summary>
        /// <param name="taskorder">需要更新的任务和顺序</param>
        /// <returns></returns>
        public async Task<Response<Boolean>> UpTaskOrder(string[] taskorders)
        {
            var response = new Response<Boolean>();
            var result = await this._stationTaskService.UpTaskOrder(taskorders);
            if (result.Item1)
            {
                response.Result = result.Item1;
            }
            else
            {
                response.Code = 500;
                response.Message = result.Item2;
            }
            return response;
        }

        /// <summary>
        /// 查询工位任务历史数据
        /// </summary>
        [HttpGet]
        public async Task<Response<StaionHisDataDTO>> LoadStationTaskHistoryData(int stepId, string scanCode)
        {
            var result = new Response<StaionHisDataDTO>();
            try
            {
                result = await _stationTaskService.LoadStationTaskHistoryData(stepId, scanCode);
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }
    }
}
