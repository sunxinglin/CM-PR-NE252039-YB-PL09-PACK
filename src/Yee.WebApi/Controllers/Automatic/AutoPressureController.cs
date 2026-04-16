using AsZero.Core.Services.Repos;

using Microsoft.AspNetCore.Mvc;

using Yee.Entitys;
using Yee.Entitys.AutomaticStation;
using Yee.Entitys.DBEntity;
using Yee.Entitys.DTOS;
using Yee.Services.AutomaticStation;

namespace Yee.WebApi.Controllers.Automatic
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AutoPressureController : ControllerBase
    {
        private readonly AutoPressureService _pressureService;

        public AutoPressureController(AutoPressureService pressureService)
        {
            _pressureService = pressureService;
        }

        [HttpPost]
        public async Task<ServiceErrResponse> SaveAndUploadPressureData(PressureDataUploadDto dto)
        {
            var result = await _pressureService.SaveAndUploadPressureData(dto);
            return result.IsError ? result.ErrorValue : result.ResultValue;
        }

        [HttpGet]
        public async Task<Response<IList<Proc_PressureInfo>>> LoadPressureData([FromQuery] PressureInfosDto dto)
        {
            return await _pressureService.LoadPressureData(dto);
        }

        [HttpGet]
        public async Task<Response<IList<PressureData>>> LoadPressureDataDetail([FromQuery] int dataId)
        {
            return await _pressureService.LoadPressureDataDetail(dataId);
        }

        [HttpGet]
        public async Task<Response<CatlMESReponse>> UploadPressureDataAgain([FromQuery] string packCode, string stationCode, bool isNeedChangeStatus)
        {
            return await _pressureService.UploadPressureDataAgain(packCode, stationCode, isNeedChangeStatus);
        }
    }
}
