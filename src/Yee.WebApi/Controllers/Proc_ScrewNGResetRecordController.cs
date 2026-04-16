using AsZero.Core.Services.Repos;

using Microsoft.AspNetCore.Mvc;

using Yee.Entitys.DTOS;
using Yee.Services.ProductionRecord;

namespace Yee.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class Proc_ScrewNGResetRecordController : ControllerBase
    {
        private readonly Proc_ScrewNGResetRecordService _service;

        public Proc_ScrewNGResetRecordController(Proc_ScrewNGResetRecordService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<Response> Save(Proc_ScrewNGResetRecordSaveDTO dto)
        {
            return await _service.Save(dto);
        }
    }
}
