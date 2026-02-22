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
    public class StationTaskUserScanController : Controller
    {
        private readonly Base_StationTaskUserInputService stationTaskUserScanService;

        public StationTaskUserScanController(Base_StationTaskUserInputService stationTaskUserScanService)
        {
            this.stationTaskUserScanService = stationTaskUserScanService;
        }
        /// <summary>
        /// 添加
        /// </summary>
        [HttpPost]
        public async Task<Response> Add(TaskUserScanDTO obj)
        {
           
            var result = new Response<Base_StationTaskUserInput>();

            Base_StationTaskUserInput Base_StationTaskUserInput = new Base_StationTaskUserInput()
            {
                UserScanName = obj.UserScanName,
                StationTaskId = obj.StationTaskId,
                MaxRange = obj.MaxRange,
                MinRange = obj.MinRange,
                UpMesCode = obj.UpMesCode
            };
            try
            {
                var cookies = Request.Cookies;
                var user = Request.Cookies.Where(p => p.Key == "SET_NAME").First().Value.ToString();

                var newObj = await stationTaskUserScanService.Add(Base_StationTaskUserInput, user);
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
        public async Task<Response> Update(Base_StationTaskUserInput obj)
        {
            var result = new Response<Base_StationTaskUserInput>();
            //Base_StationTaskUserInput Base_StationTaskUserInput = new Base_StationTaskUserInput()
            //{
            //    UserScanName = obj.UserScanName,
            //    StationTaskId = obj.StationTaskId,
            //    MaxRange = obj.MaxRange,
            //    MinRange = obj.MinRange,
            //    NeedValidate = obj.NeedValidate,
            //    UpMesCode = obj.UpMesCode
            //};
            try
            {
                var user = Request.Cookies.Where(p => p.Key == "SET_NAME").First().Value.ToString();

                var res = await this.stationTaskUserScanService.Update(obj, user);
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
                    var entity = await stationTaskUserScanService.GetById(Id);
                    if (entity != null)
                    {
                        await stationTaskUserScanService.Delete(entity,user);
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
        public async Task<Response<List<Base_StationTaskUserInput>>> LoadByTaskId(int taskid)
        {
            var result = new Response<List<Base_StationTaskUserInput>>();
            try
            {
                //var user = Request.Cookies.Where(p => p.Key == "SET_NAME").First().Value.ToString();

                var list = await this.stationTaskUserScanService.GetStationUserScanTaskByTaskid(taskid);
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
                var list = await this.stationTaskUserScanService.GetUserInputHis(Convert.ToInt32(StationId), PackPN);
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


    public class TaskUserScanDTO
    {

        public Decimal MaxRange { get; set; }
        public Decimal MinRange { get; set; }
        public bool NeedValidate { get; set; }
        public string UserScanName { get; set; }
        public int StationTaskId { get; set; }

        public string UpMesCode { get; set; }

    }
}
