using AsZero.Core.Services.Repos;
using Microsoft.AspNetCore.Mvc;
using Yee.Entitys.Request;
using Yee.Entitys.Response;
using Yee.Services.ProductionRecord;

namespace Yee.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class Base_ScrewNGResetConfigController : ControllerBase
    {
        private readonly Base_ScrewNGResetConfigService _service;

        public Base_ScrewNGResetConfigController(Base_ScrewNGResetConfigService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<Response<List<ScrewNGResetConfigPageListResponse>>> PageList(int page = 1, int limit = 20)
        {
            return await _service.GetPageList(page, limit);
        }

        [HttpPost]
        public async Task<Response> Add(ScrewNGResetConfigAddRequest request)
        {
            return await _service.Add(request);
        }

        [HttpPost]
        public async Task<Response> Update(ScrewNGResetConfigUpdateRequest request)
        {
            return await _service.Update(request);
        }

        [HttpPost]
        public async Task<Response> Delete(DeleteByIdsInput request)
        {
            return await _service.Delete(request);
        }

    }
}
