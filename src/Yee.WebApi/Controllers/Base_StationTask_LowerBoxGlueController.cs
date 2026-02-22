using AsZero.Core.Services.Repos;
using Microsoft.AspNetCore.Mvc;
using Yee.Entitys.DBEntity.Production;
using Yee.Services.Production;

namespace Yee.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class Base_StationTask_LowerBoxGlueController : Controller
    {
        private readonly Base_StationTask_LowerBoxGlueService _service;

        public Base_StationTask_LowerBoxGlueController(Base_StationTask_LowerBoxGlueService service)
        {
            this._service = service;
        }

        [HttpPost]
        public async Task<Response<Base_StationTask_LowerBoxGlue>> Update(Base_StationTask_LowerBoxGlue entity)
        {
            var response = new Response<Base_StationTask_LowerBoxGlue>();
            try
            {
                var user = Request.Cookies.Where(p => p.Key == "SET_NAME").First().Value.ToString();
                response.Data = await _service.Update(entity, user);
            }
            catch (Exception ex)
            {

                response.Code = 500;
                response.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return response;

        }

        [HttpPost]
        public async Task<Response<Base_StationTask_LowerBoxGlue>> Add(Base_StationTask_LowerBoxGlue entity)
        {
            var response = new Response<Base_StationTask_LowerBoxGlue>();
            try
            {
                var user = Request.Cookies.Where(p => p.Key == "SET_NAME").First().Value.ToString();
                response.Data = await _service.Add(entity, user);
            }
            catch (Exception ex)
            {

                response.Code = 500;
                response.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return response;

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
                    var entity = await _service.GetById(Id);
                    if (entity != null)
                    {
                        var user = Request.Cookies.Where(p => p.Key == "SET_NAME").First().Value.ToString();
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
        public async Task<Response<List<Base_StationTask_LowerBoxGlue>>> GetByTaskId(int taskid)
        {
            var response = new Response<List<Base_StationTask_LowerBoxGlue>>();
            try
            {
                response.Result = await _service.GetByTaskId(taskid);


            }
            catch (Exception ex)
            {

                response.Code = 500;
                response.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return response;
        }
    }
}
