using AsZero.Core.Services.Repos;

using Microsoft.AspNetCore.Mvc;

using Yee.Entitys.DBEntity.ProductionRecords;
using Yee.Services.Production;

namespace Yee.WebApi.Controllers.BaseData
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class Proc_GluingTimeController : ControllerBase
    {
        private readonly Proc_GluingtimeService _gluingtimeService;
        /// <summary>
        /// 构造函数
        /// </summary>
        public Proc_GluingTimeController(Proc_GluingtimeService gluingtimeService)
        {
            _gluingtimeService = gluingtimeService;
        }

        /// <summary>
        /// 通过Pack获取涂胶超时信息
        /// </summary>
        /// <param name="pack">pack码</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<Response> GetByPack([FromQuery] string? pack)
        {
            Response result = new();
            try
            {
                if (pack == null)
                {
                    result.Code = 500;
                    result.Message = "条码不可为空";
                    return result;
                }
                var gluingTime = await this._gluingtimeService.GetByPack(pack);
                result.Data = gluingTime;
                result.Code = 200;
                return result;
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.Message;
                return result;
            }
        }

        /// <summary>
        /// 修改涂胶超时时间
        /// </summary>
        /// <param name="gluingTime"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<Response> Updata([FromQuery] int id, decimal time)
        {
            Response result = new();
            try
            {
                if (_gluingtimeService == null)
                {
                    result.Code = 500;
                    result.Message = "待修改实体不可为空";
                    return result;
                }
                var (res, mes) = await this._gluingtimeService.Updata(id, time);
                if (!res)
                {
                    result.Code = 500;
                    result.Message = mes;
                    return result;
                }
                result.Code = 200;
                return result;
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.Message;
                return result;
            }
        }

        [HttpGet]
        public async Task<Response> GetRecordTime([FromQuery] string? pack)
        {
            
            var gluingTime = await this._gluingtimeService.GetRecordTime(pack);
            return gluingTime;
        }

        [HttpPost]

        public async Task<Response> UpdateTime(Proc_StationTask_TimeRecord record)
        {
            var gluingTime = await this._gluingtimeService.UpdateTime(record);
            return gluingTime;
        }
    }


}
