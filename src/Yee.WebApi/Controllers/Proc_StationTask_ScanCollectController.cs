using AsZero.Core.Services.Repos;

using Microsoft.AspNetCore.Mvc;

using Yee.Entitys.DBEntity;
using Yee.Services.Production;

namespace Yee.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class Proc_StationTask_ScanCollectController : ControllerBase
    {
        private Proc_StationTask_ScanCollectService _service;

        public Proc_StationTask_ScanCollectController(Proc_StationTask_ScanCollectService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<Response<List<Proc_StationTask_ScanCollect>>> GetScanCollects([FromQuery] Proc_StationTask_ScanCollectService.GetScanCollectServiceRequest request)
        {
            var result = new Response<List<Proc_StationTask_ScanCollect>>();
            try
            {
                var data = await _service.GetScanCollectsAsync(request);
                result.Data = data.Item1;
                result.Count = data.Item2;
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }

            return result;
        }

        [HttpPost]
        public async Task<Response<List<Proc_StationTask_ScanCollect>>> ChangeScanCollectState(string scanData, string packPN, bool isDeleted)
        {
            var result = new Response<List<Proc_StationTask_ScanCollect>>();
            try
            {
                var data = await _service.ChangeDatasState(scanData, packPN, isDeleted);
                result.Data = data;
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
