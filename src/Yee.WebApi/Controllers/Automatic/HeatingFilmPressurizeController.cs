using AsZero.Core.Services.Repos;
using Microsoft.AspNetCore.Mvc;
using Yee.Entitys;
using Yee.Entitys.AutomaticStation;
using Yee.Entitys.DBEntity;
using Yee.Entitys.DTOS;
using Yee.Services.AutomaticStation;

namespace Yee.WebApi.Controllers.AutomaticStation
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HeatingFilmPressurizeController : ControllerBase
    {
        private readonly HeatingFilmPressurizeService _service;

        public HeatingFilmPressurizeController(HeatingFilmPressurizeService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<ServiceErrResponse> SaveAndUploadData(HeatingFilmPressurizeDataUploadDto dto)
        {
            var result = await _service.SaveAndUploadData(dto);
            return result.IsError ? result.ErrorValue : result.ResultValue;
        }

        [HttpGet]
        public async Task<Response<IList<Proc_HeatingFilmPressurizeInfo>>> LoadData([FromQuery] PressureInfosDto dto)
        {
            return await _service.LoadData(dto);
        }

        [HttpGet]
        public async Task<Response<IList<HeatingFilmPressurizeData>>> LoadDataDetail([FromQuery] int dataId)
        {
            return await _service.LoadDataDetail(dataId);
        }

        [HttpGet]
        public async Task<Response<CatlMESReponse>> UploadDataAgain([FromQuery] string packCode, string stationCode, bool isNeedChangeStatus)
        {
            return await _service.UploadDataAgain(packCode, stationCode, isNeedChangeStatus);
        }
    }
}
