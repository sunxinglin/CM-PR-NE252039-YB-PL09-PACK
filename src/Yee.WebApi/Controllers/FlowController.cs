using AsZero.Core.Services.Repos;

using Microsoft.AspNetCore.Mvc;

using Yee.Entitys.Production;
using Yee.Services.BaseData;
using Yee.Services.Request;

namespace Yee.WebApi.Controllers.BaseData
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class FlowController : ControllerBase
    {
        private readonly FlowService _flowService;
        private readonly FlowStepMappingService _flowStepMappingService;
        /// <summary>
        /// 构造函数
        /// </summary>
        public FlowController(FlowService flowService, FlowStepMappingService flowStepMappingService)
        {
            _flowService = flowService;
            _flowStepMappingService = flowStepMappingService;
        }

        /// <summary>
        /// 加载
        /// </summary>
        [HttpGet]
        public async Task<Response<List<Base_Flow>>> Load([FromQuery] GetByKeyInput input)
        {
            var result = new Response<List<Base_Flow>>();
            try
            {
                var list = await _flowService.GetAll(input.Key);
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
        /// <summary>
        /// 关键字查询
        /// </summary>
        [HttpGet]
        public async Task<Response<List<Base_Flow>>> GetList([FromQuery] GetByKeyInput input)
        {
            var result = new Response<List<Base_Flow>>();
            try
            {
                result.Data = await _flowService.GetAll(input.Key);
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        [HttpGet]
        public async Task<Response<List<Base_FlowStepMapping>>> GetFlowStepList([FromQuery] GetFlowStepMappingListReq input)
        {
            var result = new Response<List<Base_FlowStepMapping>>();
            try
            {
                result.Data = await _flowStepMappingService.GetList(input);
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }
        [HttpGet]
        public async Task<Response<Boolean>> GetMapByStepId([FromQuery] int stepid,int flowId)
        {
            Response<Boolean> response = new Response<bool>();
            try
            {
                var entity = await this._flowStepMappingService.GetMapByStepId(stepid,flowId);
                if (entity != null)
                {
                    response.Result = true;
                }
                else
                {
                    response.Result = false;
                }
            }
            catch (Exception ex)
            {

                response.Code = 500;
                response.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return response;
        }
        /// <summary>
        /// 添加
        /// </summary>
        [HttpPost]
        public async Task<Response> Add(AddOrUpdateFlowReq request)
        {
            var result = new Response<Base_Flow>();
            try
            {
                if (!await _flowService.HasCode(request.Code))
                {
                    var user = Request.Cookies.Where(p => p.Key == "SET_NAME").First().Value.ToString();
                    Base_Flow flowEntity = new Base_Flow();
                    flowEntity.Code = request.Code;
                    flowEntity.Name = request.Name;
                    flowEntity.Description = request.Description;
                    flowEntity.ProductId = request.ProductId;
                    var newObj = await _flowService.Add(flowEntity,user);

                    foreach (var item in request.StepList)
                    {
                        Base_FlowStepMapping flowStepEnity = new Base_FlowStepMapping();
                        flowStepEnity.StepId = item.StepId;
                        flowStepEnity.FlowId = newObj.Id;
                        flowStepEnity.OrderNo = item.OrderNo;

                        await _flowStepMappingService.Add(flowStepEnity);
                    }
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
        public async Task<Response> Update(AddOrUpdateFlowReq request)
        {
            var result = new Response<Base_Flow>();
            try
            {
                var user = Request.Cookies.Where(p => p.Key == "SET_NAME").First().Value.ToString();
                var entity = await _flowService.GetById(request.Id);
                entity.Name = request.Name;
                entity.Description = request.Description;
                entity.Product = null;
                entity.ProductId = request.ProductId;
                await _flowService.Update(entity,user);
                result.Result = entity;
                //先获取原来已有的子表信息
                GetFlowStepMappingListReq getFlowStepMappingListReq = new GetFlowStepMappingListReq();
                getFlowStepMappingListReq.FlowId = request.Id;
                var fsList = await _flowStepMappingService.GetList(getFlowStepMappingListReq);
                if (fsList != null && fsList.Count > 0)
                {
                    var oldIds = fsList.Select(s => s.Id);
                    var newIds = request.StepList.Where(s => s.Id != null && s.Id != 0).ToList().Select(s => s.Id);
                    //找差集
                    var chaIds = oldIds.Except(newIds);
                    var chaOutStockDetailList = fsList.Where(s => chaIds.Contains(s.Id)).ToList();
                    //删除差集
                    foreach (var item in chaOutStockDetailList)
                    {
                        await _flowStepMappingService.Delete(item);
                    }
                }


                //var chaOutStockDetailList = await __flowStepMappingService.ListAsync(new Specification<FlowStep>(s => s.IsDeleted == false && chaIds.Contains(s.Id)));
                ////删除差集
                //foreach (var item in chaOutStockDetailList)
                //{
                //    await __flowStepMappingService.Delete(item);
                //}
                foreach (var item in request.StepList)
                {
                    if (item.Id == null || item.Id == 0)//id为空，为新增的
                    {
                        Base_FlowStepMapping flowStepEnity = new Base_FlowStepMapping();
                        flowStepEnity.StepId = item.StepId;
                        flowStepEnity.FlowId = entity.Id;
                        flowStepEnity.OrderNo = item.OrderNo;
                        await _flowStepMappingService.Add(flowStepEnity);

                    }
                    else
                    {
                        var flowStepEnity = fsList.Where(s => s.Id == item.Id).FirstOrDefault();
                        flowStepEnity.StepId = item.StepId;
                        flowStepEnity.FlowId = entity.Id;
                        flowStepEnity.OrderNo = item.OrderNo;

                        await _flowStepMappingService.Update(flowStepEnity);
                    }
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
        /// 批量删除
        /// </summary>
        [HttpPost]
        public async Task<Response<string>> DeleteFlow(DeleteByIdsInput input)
        {
            var result = new Response<string>();


            try
            {
                var user = Request.Cookies.Where(p => p.Key == "SET_NAME").First().Value.ToString();
                foreach (var Id in input.Ids)
                {
                    var query = await _flowService.GetById(Id);
                    await _flowService.Delete(query,user);
                    //删除工艺工序关系表

                    GetFlowStepMappingListReq getFlowStepMappingListReq = new GetFlowStepMappingListReq();
                    getFlowStepMappingListReq.FlowId = Id;
                    var fsList = await _flowStepMappingService.GetList(getFlowStepMappingListReq);
                    foreach (var item in fsList)
                    {
                        await _flowStepMappingService.Delete(item);
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
                    var entity = await _flowService.GetById(Id);
                    if (entity != null)
                    {
                        await _flowService.Delete(entity,user);
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

    }
}
