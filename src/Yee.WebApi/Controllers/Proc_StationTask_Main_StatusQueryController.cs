using AsZero.Core.Services.Repos;
using Microsoft.AspNetCore.Mvc;
using Yee.Entitys.DBEntity;
using Yee.Entitys.DTOS;
using Yee.Entitys.Production;
using Yee.Services.Production;
using Yee.Services.Request;

namespace Yee.WebApi.Controllers.BaseData
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class Proc_StationTask_Main_StatusQueryController : ControllerBase
    {
        private readonly Proc_StationTask_Main_StatusQueryService _stationTask_Main_StatusQuery;
        /// <summary>
        /// 构造函数
        /// </summary>
        public Proc_StationTask_Main_StatusQueryController(Proc_StationTask_Main_StatusQueryService stationTask_Main_StatusQuery)
        {
            _stationTask_Main_StatusQuery = stationTask_Main_StatusQuery;
        }

        /// <summary>
        /// 加载
        /// </summary>
        [HttpGet]
        public async Task<Response> Load([FromQuery] GetByKeyInput input)
        {

            var result = new Response();
            try
            {
                if (input.Key == null)
                {
                    result.Code = 500;
                    result.Message = $"条码不可为空";
                    return result;
                }
                var (res, list) = await _stationTask_Main_StatusQuery.Load(input.Key);
                if (!res || list == null)
                {
                    result.Code = 500;
                    result.Message = $"找不到此条码{input.Key}的数据";
                    return result;
                }
                result.Data = list.Skip((input.Page - 1) * input.Limit).Take(input.Limit);
                result.Count = list.Count;
                result.Code = 200;
                return result;
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
                return result;
            }
        }


        /// <summary>
        /// 强制未完成、完成
        /// </summary>
        /// <param name="status">1：强制完成，0：强制未完成</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<Response> ChangStatusById([FromQuery] int? id, int status)
        {
            var user = Request.Cookies.Where(p => p.Key == "SET_NAME").First().Value.ToString();
            var result = new Response();
            try
            {
                if (id == null)
                {
                    result.Code = 500;
                    result.Message = "id不可为空";
                    return result;
                }
                if (status == 0)
                {
                    var (res, mes) = await _stationTask_Main_StatusQuery.OffStatusById(id, user);
                    if (!res)
                    {
                        result.Code = 500;
                        result.Message = mes;
                        return result;
                    }
                }
                else if (status == 1)
                {
                    var (res, mes) = await _stationTask_Main_StatusQuery.OnStatusById(id, user);
                    if (!res)
                    {
                        result.Code = 500;
                        result.Message = mes;
                        return result;
                    }
                }

                result.Code = 200;
                return result;
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
                return result;
            }
        }


        [HttpGet]
        public async Task<Response> ClearBomDataById([FromQuery] int? id)
        {
            return await _stationTask_Main_StatusQuery.ClearBomById(id);

        }
        [HttpGet]
        public async Task<Response> ClearDataById([FromQuery] int? id)
        {
            return await _stationTask_Main_StatusQuery.ClearDataById(id);

        }

        /// <summary>
        /// 根据Main表Id查询所有精追扫码记录
        /// </summary>
        /// <param name="id">MainId</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<Response> LoadStationTaskDetail([FromQuery] int? id)
        {
            var result = new Response();
            try
            {
                if (id == null)
                {
                    result.Code = 500;
                    result.Message = $"条码不可为空";
                    return result;
                }
                var (res, list) = await _stationTask_Main_StatusQuery.Load_RecordByMainId(id);
                if (!res || list == null)
                {
                    result.Code = 500;
                    result.Message = $"找不到此条码{id}的数据";
                    return result;
                }
                result.Data = list;
                result.Count = list.Proc_StationTask_Record_DTO_List.Count;
                result.Code = 200;
                return result;
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
                return result;
            }
        }

        /// <summary>
        /// 踢料
        /// </summary>
        /// <param name="id">MainId</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<Response> DeleteScan([FromQuery] int? id)
        {
            var user = Request.Cookies.Where(p => p.Key == "SET_NAME").First().Value.ToString();
            var result = new Response();
            try
            {
                if (id == null)
                {
                    result.Code = 500;
                    result.Message = $"条码不可为空";
                    return result;
                }
                var (res, mes) = await _stationTask_Main_StatusQuery.Delete_RecordById(id, user);
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
                result.Message = ex.InnerException?.Message ?? ex.Message;
                return result;
            }
        }

        /// <summary>
        /// 改变上传状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status">0:未上传，1:已上传</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<Response> ChangeHasUp([FromQuery] int? id, bool status)
        {
            var result = new Response();
            try
            {
                if (id == null)
                {
                    result.Code = 500;
                    result.Message = $"条码不可为空";
                    return result;
                }
                var (res, mes) = await _stationTask_Main_StatusQuery.HasUpMes_RecordById(id, status);
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
                result.Message = ex.InnerException?.Message ?? ex.Message;
                return result;
            }
        }

    }
}
