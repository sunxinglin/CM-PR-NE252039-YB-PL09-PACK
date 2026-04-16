using AsZero.Core.Services.Repos;

using Microsoft.AspNetCore.Mvc;

using Yee.Entitys.DBEntity;
using Yee.Entitys.Request;
using Yee.Services.ProductionRecord;

namespace Yee.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ProcCheckPowerRecordController : ControllerBase
    {
        private readonly ProcRecordCheckPowerLogService _serivce;

        public ProcCheckPowerRecordController(ProcRecordCheckPowerLogService serivce)
        {
            _serivce = serivce;
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<Response<List<Proc_CheckPowerRecord>>> GetPageList([FromQuery] ProcCheckPowerRecordGetPageListRequest request)
        {
            return await _serivce.GetPageList(request);
        }


    }
}
