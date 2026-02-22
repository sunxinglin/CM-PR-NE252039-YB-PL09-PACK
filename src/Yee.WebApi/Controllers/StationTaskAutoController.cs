using AsZero.Core.Services.Repos;

using Microsoft.AspNetCore.Mvc;

using System.Threading.Tasks;

using Yee.Entitys.Production;
using Yee.Services.Production;

namespace Yee.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class StationTaskAutoController : Controller
    {
        private readonly Base_StationTaskAutoService base_StationTaskAuto;

        public StationTaskAutoController(Base_StationTaskAutoService base_StationTaskAuto)
        {
            this.base_StationTaskAuto = base_StationTaskAuto;
        }

        [HttpPost]
        public async Task<Response> Add(Base_AutoStationTaskTighten entity)
        {
            var user = Request.Cookies.Where(p => p.Key == "SET_NAME").First().Value.ToString();
            return await base_StationTaskAuto.Add(entity, user);
        }

        [HttpPost]
        public async Task<Response> Delete(Base_AutoStationTaskTighten entity)
        {
            var user = Request.Cookies.Where(p => p.Key == "SET_NAME").First().Value.ToString();
            return await base_StationTaskAuto.Delete(entity, user);
        }

        [HttpPost]
        public async Task<Response<Base_AutoStationTaskTighten>> Update(Base_AutoStationTaskTighten entity)
        {
            var response = new Response<Base_AutoStationTaskTighten>();
            try
            {
                var user = Request.Cookies.Where(p => p.Key == "SET_NAME").First().Value.ToString();
                response.Data = await base_StationTaskAuto.Update(entity, user);
            }
            catch (Exception ex)
            {

                response.Code = 500;
                response.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return response;

        }

        [HttpGet]
        public async Task<Response<IList<Base_AutoStationTaskTighten>>> GetByTaskId(int taskid)
        {
            return await base_StationTaskAuto.GetByTaskId(taskid);
        }
    }
}
