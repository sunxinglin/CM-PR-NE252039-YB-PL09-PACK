using AsZero.Core.Services.Repos;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Yee.Common.Library.CommonEnum;
using Yee.Entitys.Production;
using Yee.Services.BaseData;
using Yee.Services.Production;
using Yee.Services.Request;

namespace Yee.WebApi.Controllers
{
    /// <summary>
    /// 生产资源的控制
    /// </summary>
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class ProResourceController : ControllerBase
    {
        private readonly ProResourceService _proResourceService;
        private readonly StepService _stepService;
        private readonly StationService _stationService;
        private  readonly  ILogger<ProResourceController> _logger;

        public ProResourceController(ProResourceService proResourceService, ILogger<ProResourceController> logger,
               StationService stationService,
               StepService stepService)
        {
            this._proResourceService = proResourceService;
            this._stepService = stepService;
            this._stationService = stationService;
            this._logger = logger;

        }

        [Authorize]
        [HttpGet]
        public async Task<Response<IList<Base_ProResource>>> GetProResourceList()
        {

            var result = new Response<IList<Base_ProResource>>();
            try
            {
                result.Result=await this._proResourceService.GetList();
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<Response<List<Base_ProResource>>> Load([FromQuery] GetByKeyInput input)
        {
            var result = new Response<List<Base_ProResource>>();
            try
            {
                var resource = new Base_ProResource();
                var list =new List<Base_ProResource>();

                var listDto = await _proResourceService.GetAll(input.Key);
                foreach (var one in listDto)
                {
                    list.Add(one);
                }
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
        /// 添加
        /// </summary>
        [HttpPost]
        public async Task<Response> Add(Base_ProResource obj)
        {
            var result = new Response<Base_ProResource?>();
            try
            {
                var newObj = await _proResourceService.Add(obj);
                result.Result = newObj;

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
        public async Task<Response> Update(Base_ProResource obj)
        {
            var result = new Response<Base_ProResource?>();
            try
            {
                var res = await _proResourceService.Update(obj);
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
                    var entity = await _proResourceService.GetById(Id);
                    if (entity != null)
                    {
                        await _proResourceService.Delete(entity);
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

        [HttpGet]
        public async Task<Response> GetDesoutterByStepId(int stepId)
        {
            var result = new Response<List<Base_ProResource>>();
            try
            {
                var newObj = await _proResourceService.GetProResourceByStepIdAndType(stepId, ProResourceTypeEnum.拧紧枪);
                result.Result = newObj;
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
