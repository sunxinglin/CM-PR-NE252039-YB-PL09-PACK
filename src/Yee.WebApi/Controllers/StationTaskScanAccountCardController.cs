using AsZero.Core.Services.Repos;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Yee.Entitys.DBEntity;
using Yee.Entitys.Production;
using Yee.Services.Production;

namespace Yee.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class StationTaskScanAccountCardController : Controller
    {
        private readonly Base_StationTaskScanAccountCardService stationTaskScanAccountService;

        public StationTaskScanAccountCardController(Base_StationTaskScanAccountCardService stationTaskScanAccountService)
        {
            this.stationTaskScanAccountService = stationTaskScanAccountService;
        }
        /// <summary>
        /// 添加
        /// </summary>
        [HttpPost]
        public async Task<Response> Add(TaskScanAccountCardDTO obj)
        {
           
            var result = new Response<Base_StationTaskScanAccountCard>();

            Base_StationTaskScanAccountCard Base_StationTaskScanAccountCard = new Base_StationTaskScanAccountCard()
            {
                ScanAccountCardName = obj.ScanAccountCardName,
                StationTaskId = obj.StationTaskId,
                UpMesCode = obj.UpMesCode
            };
            try
            {
                var cookies = Request.Cookies;
                var user = Request.Cookies.Where(p => p.Key == "SET_NAME").First().Value.ToString();

                var newObj = await stationTaskScanAccountService.Add(Base_StationTaskScanAccountCard, user);
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
        public async Task<Response> Update(Base_StationTaskScanAccountCard obj)
        {
            var result = new Response<Base_StationTaskScanAccountCard>();
            try
            {
                var user = Request.Cookies.Where(p => p.Key == "SET_NAME").First().Value.ToString();

                var res = await this.stationTaskScanAccountService.Update(obj, user);
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
                    var entity = await stationTaskScanAccountService.GetById(Id);
                    if (entity != null)
                    {
                        await stationTaskScanAccountService.Delete(entity,user);
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
        public async Task<Response<List<Base_StationTaskScanAccountCard>>> LoadByTaskId(int taskid)
        {
            var result = new Response<List<Base_StationTaskScanAccountCard>>();
            try
            {
                //var user = Request.Cookies.Where(p => p.Key == "SET_NAME").First().Value.ToString();

                var list = await this.stationTaskScanAccountService.GetStationScanAccountCardTaskByTaskid(taskid);
                result.Data = list;
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }

            return result;
        }


        [HttpGet]
        public async Task<Response<List<Proc_StationTask_UserInput>>> GetUserInputHis(string StationId, string PackPN)
        {
            var result = new Response<List<Proc_StationTask_UserInput>>();
            try
            {
                var list = await this.stationTaskScanAccountService.GetUserInputHis(Convert.ToInt32(StationId), PackPN);
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


    public class TaskScanAccountCardDTO
    {
       // public bool NeedValidate { get; set; }
        public string ScanAccountCardName { get; set; }
        public int StationTaskId { get; set; }

        public string UpMesCode { get; set; }

    }
}
