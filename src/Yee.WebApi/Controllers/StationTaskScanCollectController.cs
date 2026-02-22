using AsZero.Core.Services.Repos;
using Microsoft.AspNetCore.Mvc;
using Yee.Entitys.DBEntity;
using Yee.Entitys.Production;
using Yee.Services.Production;

namespace Yee.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class StationTaskScanCollectController : Controller
    {
        private readonly Base_StationTaskScanCollectService stationTaskScanCollectService;

        public StationTaskScanCollectController(Base_StationTaskScanCollectService stationTaskScanCollectService)
        {
            this.stationTaskScanCollectService = stationTaskScanCollectService;
        }
        /// <summary>
        /// 添加
        /// </summary>
        [HttpPost]
        public async Task<Response> Add(TaskScanCollectDTO obj)
        {
           
            var result = new Response<Base_StationTaskScanCollect>();

            Base_StationTaskScanCollect base_StationTaskScanCollect = new Base_StationTaskScanCollect()
            {
                ScanCollectName = obj.ScanCollectName,
                StationTaskId = obj.StationTaskId,
                NeedValidate = obj.NeedValidate,
                UpMesCode = obj.UpMesCode
            };
            try
            {
                var cookies = Request.Cookies;
                var user = Request.Cookies.Where(p => p.Key == "SET_NAME").First().Value.ToString();

                var newObj = await stationTaskScanCollectService.Add(base_StationTaskScanCollect, user);
                result.Result = newObj;
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }

            return result;

        }


        /// <summary>
        /// 修改
        /// </summary>
        [HttpPost]
        public async Task<Response> Update(Base_StationTaskScanCollect obj)
        {
            var result = new Response<Base_StationTaskScanCollect>();
            try
            {
                var user = Request.Cookies.Where(p => p.Key == "SET_NAME").First().Value.ToString();

                var res = await this.stationTaskScanCollectService.Update(obj,user);
                result.Result = res;
                
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }

            return result;
        }

        /// <summary>
        /// 删除
        /// </summary>
        [HttpPost]
        public async Task<Response<string>> DeleteEntity(DeleteByIdsInput input)
        {
            var result = new Response<string>();
            try
            {
                var user = Request.Cookies.Where(p => p.Key == "SET_NAME").First().Value.ToString();
                foreach (var Id in input.Ids)
                {
                    var entity = await stationTaskScanCollectService.GetById(Id);
                    if (entity != null)
                    {
                        await stationTaskScanCollectService.Delete(entity,user);
                    }
                }
                result.Message = "操作成功";
                return result;
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }

            return result;
        }

        [HttpGet]
        public async Task<Response<List<Base_StationTaskScanCollect>>> LoadByTaskId(int taskid)
        {
            var result = new Response<List<Base_StationTaskScanCollect>>();
            try
            {
                //var user = Request.Cookies.Where(p => p.Key == "SET_NAME").First().Value.ToString();

                var list = await this.stationTaskScanCollectService.GetStationScanCollectTaskByTaskid(taskid);
                result.Data = list;
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }

            return result;
        }
  
    }


    public class TaskScanCollectDTO
    {
        public bool NeedValidate { get; set; }
        public string ScanCollectName { get; set; }
        public int StationTaskId { get; set; }

        public string UpMesCode { get; set; }

    }
}
