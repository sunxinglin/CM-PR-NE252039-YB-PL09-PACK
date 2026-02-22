using AsZero.Core.Services.Repos;
using AsZero.DbContexts;
using FutureTech.OpResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.Formula.Functions;
using Yee.Common.Library.CommonEnum;
using Yee.Entitys.CATL;
using Yee.Entitys.CommonEntity;
using Yee.Entitys.DTOS;
using Yee.Entitys.Production;
using Yee.Services.BaseData;
using Yee.Services.Production;
using Yee.Services.Request;
using Yee.Tools;

namespace Yee.WebApi.Controllers.BaseData
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class StationController : ControllerBase
    {
        private readonly ProductService _productService;
        private readonly StationService _stationService;
        private readonly StationTaskService _stationTaskService;
        private readonly StationTaskBomService _stationBomService;
        private readonly ProResourceService _proResourceService;
        private readonly StationTaskScrewService _stationTaskScrewService;
        private readonly StationTaskResourceService _stationTaskResourceService;
        private readonly Base_StationTaskAnyLoadService base_Station_WeightService;
        private readonly StationTaskImporntService stationTaskImporntService;
        private readonly StepService stepService;
        private readonly FlowStepMappingService flowStepMappingService;
        private readonly FlowService _flowService;
        private readonly Base_StationTaskGluingTimeService base_StationTaskGluingTimeService;
        /// <summary>
        /// 构造函数
        /// </summary>
        public StationController(StationService stationService,
            StationTaskBomService stationBomService,
            ProductService productService,
            ProResourceService proResourceService,
            StationTaskService stationTaskService,
            FlowService flowService,
            StationTaskScrewService stationTaskScrewService,
            StationTaskResourceService stationTaskResourceService,
            StationTaskImporntService stationTaskImporntService,
            StepService stepService, FlowStepMappingService flowStepMappingService,
            Base_StationTaskAnyLoadService base_Station_WeightService,
            Base_StationTaskGluingTimeService base_StationTaskGluingTimeService
            )
        {
            _productService = productService;
            _stationService = stationService;
            _stationTaskService = stationTaskService;
            _flowService = flowService;
            _stationBomService = stationBomService;
            _stationTaskResourceService = stationTaskResourceService;
            this.base_Station_WeightService = base_Station_WeightService;
            this.stationTaskImporntService = stationTaskImporntService;
            this.stepService = stepService;
            this.flowStepMappingService = flowStepMappingService;
            _stationTaskScrewService = stationTaskScrewService;
            this._proResourceService = proResourceService;
            this.base_StationTaskGluingTimeService = base_StationTaskGluingTimeService;
        }


        /// <summary>
        /// 加载
        /// </summary>
        [HttpGet]
        public async Task<Response<List<Base_Station>>> Load([FromQuery] GetByKeyInput input)
        {
            var result = new Response<List<Base_Station>>();
            try
            {
                var list = await _stationService.GetAll(input.Key);
                result.Data = list.Skip((input.Page - 1) * input.Limit).Take(input.Limit);
                result.Count = list.Count;
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }
        /// <summary>
        /// 加载
        /// </summary>
        [HttpGet]
        public async Task<Response<List<Base_Station>>> GetAllStation()
        {
            var result = new Response<List<Base_Station>>();
            try
            {
                var list = await _stationService.GetAll(null);
                result.Result = list;
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }


        /// <summary>
        /// 加载
        /// </summary>
        [HttpGet]
        public async Task<Response<List<Base_Station>>> GetStationsByStepId(int stepId)
        {
            var result = new Response<List<Base_Station>>();
            try
            {
                var list = await _stationService.GetStationsByStepId(stepId);

                result.Data = list;
                result.Count = list.Count;
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 加载
        /// </summary>
        [HttpGet]
        public async Task<Response<List<Base_Station>>> GetStationsByFlowId([FromQuery] StationGetByKeyInput input)
        {
            var result = new Response<List<Base_Station>>();
            try
            {
                var user = HttpContext.User;
                var list = new List<Base_Station>();
                ///获取工艺路线对应工序关系
                var flowmaps = await flowStepMappingService.GetList(new GetFlowStepMappingListReq() { FlowId = input.Flowid });
                foreach (var flowmap in flowmaps)
                {
                    var station = await _stationService.GetStationByStepid(flowmap.StepId);
                    if (station != null)
                    {
                        list.Add(station);
                    }

                }
                if (!string.IsNullOrEmpty(input.Key) && !string.IsNullOrWhiteSpace(input.Key))
                {
                    list = list.Where(o => o.Code == input.Key || o.Name == input.Key).ToList();
                }

                result.Data = list.Skip((input.Page - 1) * input.Limit).Take(input.Limit);
                result.Count = list.Count;
                //var list = await _stationService.GetAll(null);
                //result.Result = list;
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }
        /// <summary>
        /// 关键字查询
        /// </summary>
        [HttpGet]
        public async Task<Response<List<Base_Station>>> GetList([FromQuery] GetByKeyInput input)
        {
            var result = new Response<List<Base_Station>>();
            try
            {
                result.Data = await _stationService.GetAll(input.Key);
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 根据IP查询工位信息、工位任务信息
        /// </summary>
        [HttpGet]
        public async Task<Response<StationConfig>> LoadStationConfig(string stepCode, string stationCode, string packPN)
        {
            var result = new Response<StationConfig>();
            try
            {
                var pruduct = _productService.GetProductByPackRule(packPN);
                if (pruduct != null)
                {
                    var station = await _stationService.GetStationByCode(stationCode);
                    if (station != null)
                    {
                        var mapping = await _stationTaskService.GetStepTaskDTOByStep(pruduct.Id, station.StepId, stationCode);
                        result.Result = new StationConfig
                        {
                            StationTaskList = mapping
                        };
                    }
                    else
                    {
                        result.Result = null;
                        result.Code = 500;
                        result.Message = $"找不到{stationCode}对应的工位！";
                    }
                }
                else
                {
                    result.Result = null;
                    result.Code = 500;
                    result.Message = $"找不到{packPN}对应的产品！";
                }
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 根据IP查询工位信息
        /// </summary>
        [HttpGet]
        public async Task<Response<Base_Station>> LoadStationInfo(string code)
        {
            var result = new Response<Base_Station>();
            try
            {
                var station = await _stationService.GetStationByCode(code);
                if (station != null)
                {
                    result.Result = station;
                    result.Code = 200;
                }
                else
                {
                    result.Result = null;
                    result.Code = 500;
                    result.Message = $"找不到Code{code}对应的工位！";
                }
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 根据IP查询工位信息
        /// </summary>
        [HttpGet]
        public async Task<Response<StepStationDTO>> LoadStepStationData2(string stepCode, string stationCode)
        {
            var result = new Response<StepStationDTO>() { Result = new StepStationDTO() };
            try
            {
                var station = await _stationService.GetStationByCode(stationCode);
                if (station != null)
                {
                    result.Result.Station = station;
                }
                var step = await stepService.LoadStepInfo(stepCode);
                if (step != null)
                {
                    result.Result.Step = step;
                }

            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        [HttpGet]
        public async Task<Response<StepStationDTO>> LoadStepStationData(string packCode, string productId, string stationCode)
        {
            var result = new Response<StepStationDTO>();
            try
            {
                result.Result = await _stationService.LoadStepStationData(packCode, productId, stationCode);
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }


        /// <summary>
        /// 添加
        /// </summary>
        [HttpPost]
        public async Task<Response> Add(Base_Station obj)
        {
            var result = new Response<Base_Station>();
            try
            {
                if (!await _stationService.HasCode(obj.Code, obj.StepId))
                {
                    var step = await this.stepService.GetById(obj.StepId);
                    if (step != null)
                    {
                        var user = Request.Cookies.Where(p => p.Key == "SET_NAME").First().Value.ToString();
                        //obj.Code = step.Code;
                        var newObj = await _stationService.Add(obj, user);
                        result.Result = newObj;
                    }
                    else
                    {
                        result.Code = 500;
                        result.Message = "关联工序不存在,请确认后重试!";
                    }

                }
                else
                {
                    result.Code = 500;
                    result.Message = "该编号已存在，请重新输入一个新的编码";
                }

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
        public async Task<Response> Update(Base_Station obj)
        {
            var result = new Response<Base_Station>();
            try
            {
                var user = Request.Cookies.Where(p => p.Key == "SET_NAME").First().Value.ToString();
                obj.Step = null;
                var res = await _stationService.Update(obj, user);
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
                //var user = Request.Cookies.Where(p => p.Key == "SET_NAME").First().Value.ToString();
                //foreach (var Id in input.Ids)
                //{
                //    var entity = await _stationService.GetById(Id);
                //    if (entity != null)
                //    {
                //        var task = await this._stationTaskService.GetStationTaskByStep(entity.Id);
                //        if (task.Count != 0)
                //        {
                //            result.Code = 500;
                //            result.Message = $"当前工位{entity.Name},存在工位任务,请先删除工位任务!";
                //            return result;
                //        }
                //        await _stationService.Delete(entity, user);
                //    }
                //}

            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }

            return result;
        }
        /// <summary>
        /// 导入工位Excel数据
        /// </summary>
        /// <param name="file">导入的数据</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<Response<Boolean>> ImporntStationFile(IFormFile file)
        {
            var response = new Response<Boolean>();
            try
            {
                if (file == null)
                {
                    response.Code = 500;
                    response.Message = "导入数据不可为空!";
                    return response;
                }

                var datalist = ExcelToEntity.WorksheetToDataRow<StationTaskExcel>(file.OpenReadStream(), 1, 3, 0, 0, 0);
                if (datalist.Count > 0)
                {
                    var user = Request.Cookies.Where(p => p.Key == "SET_NAME").First().Value.ToString();
                    var (state, mesage) = await stationTaskImporntService.ImporntStationTask(datalist, user);
                    if (!state)
                    {
                        response.Code = 500;
                        response.Message = mesage;
                    }

                }



            }
            catch (Exception ex)
            {

                response.Code = 500;
                response.Message = ex.Message;
            }
            return response;
        }
        /// <summary>
        /// 导出指定的工位任务详情
        /// </summary>
        /// <param name="stationids"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ModelExpornt(StationTaskExporn dto)
        {
            try
            {
                var memory = new MemoryStream();
                if (dto.StepIds.Length <= 0)
                {
                    return this.StatusCode(200, new { data = 208, message = "未选择导出的工位ID" });
                    //this.ModelState.AddModelError("Message", "");
                    //return  new FileStreamResult(memory,"");
                }

                var result = await this.stationTaskImporntService.ExportStationTask(dto.StepIds, dto.ProductId);
                if (!result.Item1)
                {
                    return this.StatusCode(200, new { data = 208, message = result.Item2 });
                }


                using (var filestream = new FileStream(result.Item2, FileMode.Open))
                {
                    filestream.CopyTo(memory);
                }

                memory.Position = 0;
                this.Request.ContentType = "blob";
                return File(memory, "application/vnd.ms-excel", $"{DateTime.Now.Date.ToShortDateString()}工位配方导出.xls");

            }
            catch (Exception)
            {

                throw;
            }

        }

    }

    public class StationTaskExporn
    {
        public int[] StepIds { get; set; }
        public int ProductId { get; set; }

    }


    public class StationTaskWithResource
    {
        public List<StdStationResource> StationTasks { get; set; }
        public List<Base_ProResource> StationBoltGuns { get; set; }
        public List<BoltGunGroup> StationBoltGunsGroup { get; set; }

    }



    public class StdStationResource
    {
        public Base_StationTask stdtask { get; set; }

        public List<Base_StationTaskBom> stationTaskBoms { get; set; }

        public List<Base_StationTaskScrew> stationTaskScrews { get; set; }

        public List<Base_ProResource> resources { get; set; }

        public List<Base_StationTaskAnyLoad> stationTaskAnyLoads { get; set; }
        public List<Base_StationTaskCheckTimeOut> stationTaskGluingTimes { get; set; }

    }
}
