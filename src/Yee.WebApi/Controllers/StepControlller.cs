using AsZero.Core.Services.Repos;
using AsZero.DbContexts;
using Microsoft.AspNetCore.Mvc;
using Yee.Entitys.Production;
using Yee.Services.BaseData;
using Yee.Services.Production;
using Yee.Services.Request;

namespace Yee.WebApi.Controllers.BaseData
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class StepController : ControllerBase
    {
        private readonly ProResourceService _proResourceService;
        private readonly StepService _stepService;
        private readonly AsZeroDbContext _dbContext;
        /// <summary>
        /// 构造函数
        /// </summary>
        public StepController(StepService stepService, AsZeroDbContext dbContext,
            ProResourceService proResourceService)
        {
            _stepService = stepService;
            this._proResourceService = proResourceService;
            this._dbContext = dbContext;
        }

        /// <summary>
        /// 加载
        /// </summary>
        [HttpGet]
        public async Task<Response<List<Base_Step>>> Load([FromQuery] GetByKeyInput input)
        {
            var result = new Response<List<Base_Step>>();
            try
            {
                var list = await _stepService.GetAll(input.Key);
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
        public async Task<Response<List<Base_Step>>> GetList([FromQuery] GetByKeyInput input)
        {
            var result = new Response<List<Base_Step>>();
            try
            {
                result.Data = await _stepService.GetAll(input.Key);
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
        public async Task<Response> Add(Base_Step obj)
        {
            var result = new Response<Base_Step>();
            try
            {
                if (!await _stepService.HasCode(obj.Code))
                {
                    var user = Request.Cookies.Where(p => p.Key == "SET_NAME").First().Value.ToString();
                    var newObj = await _stepService.Add(obj, user);

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
        public async Task<Response> Update(Base_Step obj)
        {
            var result = new Response<Base_Step>();
            try
            {
                var user = Request.Cookies.Where(p => p.Key == "SET_NAME").First().Value.ToString();
                var res = await _stepService.Update(obj, user);
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
                foreach (var Id in input.Ids)
                {
                    var user = Request.Cookies.Where(p => p.Key == "SET_NAME").First().Value.ToString();
                    var step = await _stepService.GetById(Id);
                    if (step != null)
                    {
                        var stepTask = this._dbContext.Base_StationTasks.FirstOrDefault(s => s.StepId == step.Id && !s.IsDeleted);
                        if (stepTask != null)
                        {
                            result.Code = 500;
                            result.Message = $"当前工序{step.Code},存在配方数据,请先删除配方!";
                            return result;
                        }

                        var station = await this._stepService.GetStationByStep(step.Id);
                        if (station != null)
                        {
                            result.Code = 500;
                            result.Message = $"当前工序{step.Code},存在关联工位,请先删除工位!";
                            return result;
                        }
                        //else
                        //{
                        //    var resource = await this._proResourceService.GetProResourceByStepId(Id);
                        //    if (resource.Count > 0)
                        //    {
                        //        result.Code = 500;
                        //        result.Message = $"当前工序{step.Code},存在设备资源,请先删除资源!";
                        //        return result;
                        //    }
                        //}

                        await _stepService.Delete(step, user);
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
        /// 根据IP查询工位信息
        /// </summary>
        [HttpGet]
        public async Task<Response<Base_Step>> LoadStepInfo(string code)
        {
            var result = new Response<Base_Step>();
            try
            {
                var step = await _stepService.LoadStepInfo(code);
                if (step != null)
                {
                    result.Result = step;
                    result.Code = 200;
                }
                else
                {
                    result.Result = null;
                    result.Code = 500;
                    result.Message = $"找不到Code{code}对应的工序！";
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
        /// 根据产品id获取对于工序
        /// </summary>
        /// <param name="productid"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<Response<List<Base_Step>>> GetStepsByProductId([FromQuery] StepGetByKeyInput input)
        {
            var result = new Response<List<Base_Step>>();
            try
            {
                var list = await _stepService.GetStepsByProductId(input.Productid);
                if (list.Code != 200)
                {
                    result.Result = null;
                    result.Message = list.Message;
                }
                else
                {
                    result.Result = list.Result;
                }
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
