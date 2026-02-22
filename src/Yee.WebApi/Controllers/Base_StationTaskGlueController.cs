using AsZero.Core.Services.Repos;
using Microsoft.AspNetCore.Mvc;
using Yee.Entitys.Production;
using Yee.Services.Production;
using Yee.Services.Request;

namespace Yee.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class Base_StationTaskGlueController : Controller
    {
        private readonly Base_StationTaskGlueService Base_AutoStationTaskGlueService;

        public Base_StationTaskGlueController(Base_StationTaskGlueService Base_AutoStationTaskGlueService)
        {
            this.Base_AutoStationTaskGlueService = Base_AutoStationTaskGlueService;
        }
        [HttpPost]
        public async Task<Response<Base_AutoStationTaskGlue>> Update(Base_AutoStationTaskGlue entity)
        {
            var response = new Response<Base_AutoStationTaskGlue>();
            try
            {
                var user = Request.Cookies.Where(p => p.Key == "SET_NAME").First().Value.ToString();
                response.Data = await Base_AutoStationTaskGlueService.Update(entity,user);
            }
            catch (Exception ex)
            {

                response.Code = 500;
                response.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return response;

        }
        [HttpPost]
        public async Task<Response<Base_AutoStationTaskGlue>> Add(Base_AutoStationTaskGlue entity)
        {
            var response = new Response<Base_AutoStationTaskGlue>();
            try
            {
                var user = Request.Cookies.Where(p => p.Key == "SET_NAME").First().Value.ToString();
                response.Data = await Base_AutoStationTaskGlueService.Add(entity,user);
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
                    var entity = await Base_AutoStationTaskGlueService.GetById(Id);
                    if (entity != null)
                    {
                        var user = Request.Cookies.Where(p => p.Key == "SET_NAME").First().Value.ToString();
                        await Base_AutoStationTaskGlueService.Delete(entity,user);
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
        public async Task<Response<List<Base_AutoStationTaskGlue?>>> GetByTaskId(int taskid)
        {
            var response = new Response<List< Base_AutoStationTaskGlue?>>();
            try
            {
                response.Result = await Base_AutoStationTaskGlueService.GetByTaskId(taskid);
               
             
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
