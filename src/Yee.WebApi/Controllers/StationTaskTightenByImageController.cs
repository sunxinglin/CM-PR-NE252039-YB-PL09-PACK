using AsZero.Core.Services.Repos;

using Microsoft.AspNetCore.Mvc;

using Yee.Entitys.DBEntity.Production;
using Yee.Entitys.DTOS.StationTaskDataDTOS;
using Yee.Services.Production;

namespace Yee.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class StationTaskTightenByImageController : ControllerBase
    {
        private readonly StationTaskTightenByImageService _service;

        public StationTaskTightenByImageController(StationTaskTightenByImageService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<Response> Add(Base_StationTask_TightenByImage obj)
        {
            var result = new Response<Base_StationTask_TightenByImage>();
            try
            {
                var user = Request.Cookies.First(p => p.Key == "SET_NAME").Value.ToString();
                var newObj = await _service.Add(obj, user);
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
        public async Task<Response> Update(Base_StationTask_TightenByImage obj)
        {
            var result = new Response<Base_StationTask_TightenByImage>();
            try
            {
                var user = Request.Cookies.First(p => p.Key == "SET_NAME").Value.ToString();
                var res = await this._service.Update(obj, user);
                result.Result = res;
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }

            return result;
        }

        [HttpPost]
        public async Task<Response> DeleteEntity(DeleteByIdsInput input)
        {
            var result = new Response();
            try
            {
                var user = Request.Cookies.First(p => p.Key == "SET_NAME").Value.ToString();
                foreach (var Id in input.Ids)
                {
                    var entity = await _service.GetById(Id);
                    if (entity != null)
                    {
                        await _service.Delete(entity, user);
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
        public async Task<Response<List<Base_StationTask_TightenByImage>>> LoadByTaskId(int taskId)
        {
            var result = new Response<List<Base_StationTask_TightenByImage>>();
            try
            {
                var list = await this._service.GetScrewListByStationTaskID(taskId);
                result.Result = list;
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }

            return result;
        }

        [HttpPost]
        public async Task<Response> UploadImage([FromForm] UploadImageDTO dto)
        {
            return await this._service.UploadImage(dto);
        }

        [HttpGet]
        public async Task<Response> GetImageUrl([FromQuery] int taskId)
        {
            return await _service.GetImageUrl(taskId);
        }

        [HttpGet]
        public async Task<Response<IList<TightenByImageDto>>> LoadTaskList([FromQuery] string? productPn, string? stepCode)
        {
            return await this._service.LoadTaskList(productPn, stepCode);
        }

        [HttpGet]
        public async Task<Response<LayoutInfoDto>> LoadLayout(int taskId)
        {
            return await this._service.LoadLayout(taskId);
        }

        [HttpPost]
        public async Task<Response> SaveLayout(LayoutInfoDto dto)
        {
            return await this._service.SaveLayout(dto);
        }
    }
}
