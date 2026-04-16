using AsZero.Core.Services.Repos;
using AsZero.DbContexts;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using Yee.Services.HistoryData;

namespace Yee.WebApi.Controllers.BaseData
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HistoryDataController : ControllerBase
    {
        private readonly ILogger<HistoryDataController> _logger;
        public readonly AsZeroDbContext _dBContext;
        public readonly HistoryData_APIService _HistoryDataServic;
        private readonly IMediator _mediator;

        /// <summary>
        /// 构造函数
        /// </summary>
        public HistoryDataController(AsZeroDbContext dBContext, ILogger<HistoryDataController> logger, HistoryData_APIService historyDataServic, IMediator mediator)
        {
            _dBContext = dBContext;
            _logger = logger;
            _HistoryDataServic = historyDataServic;
            this._mediator = mediator;
        }

        [HttpGet]
        public async Task<Response> ClearHisData_PeiFang(int id)
        {
            Response response = new Response();
            response = await this._HistoryDataServic.ClearHisData_PeiFang(id);
            return response;
        }
    }
}



