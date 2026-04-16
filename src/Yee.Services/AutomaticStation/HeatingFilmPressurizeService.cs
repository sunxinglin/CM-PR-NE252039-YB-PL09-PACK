using AsZero.Core.Services.Repos;
using AsZero.DbContexts;

using Itminus.FSharpExtensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.FSharp.Core;

using Newtonsoft.Json;

using Yee.Common.Library.CommonEnum;
using Yee.Entitys;
using Yee.Entitys.AutomaticStation;
using Yee.Entitys.CATL;
using Yee.Entitys.DBEntity;
using Yee.Entitys.DTOS;
using Yee.Entitys.Production;
using Yee.Services.CatlMesInvoker;

namespace Yee.Services.AutomaticStation
{
    public class HeatingFilmPressurizeService
    {
        private readonly AsZeroDbContext _dbContext;
        private readonly AutomicCommonService _commonService;
        private readonly ICatlMesInvoker _mesInvoker;

        public HeatingFilmPressurizeService(AsZeroDbContext dbContext, AutomicCommonService commonService, ICatlMesInvoker mesInvoker)
        {
            _dbContext = dbContext;
            _commonService = commonService;
            _mesInvoker = mesInvoker;
        }

        public async Task<FSharpResult<ServiceErrResponse, ServiceErrResponse>> SaveAndUploadData(HeatingFilmPressurizeDataUploadDto dto)
        {
            try
            {
                var q = from main in _commonService.HasMainRecord(dto.Pin, dto.StationCode)
                        from productPn in _commonService.GetProductPnFromCatlMes(dto.Pin, dto.StationCode)
                        from formulas in _commonService.GetBaseStationTasks(productPn.BarCode_GoodsPN, dto.StationCode)
                            //from timeOutValue in this.CheckGlueTimeOut(productPn.BarCode_GoodsPN, dto.StationCode, dto.Pin, main.Id)
                            //from pressureStartTimeDC in this.DealStartTime(formulas.FirstOrDefault(f => f.Type == StationTaskTypeEnum.时间记录), dto, main.Id)
                        from pressureDataDC in this.DealPressureData(formulas.Where(f => f.Type == StationTaskTypeEnum.自动加压).ToList(), dto, main.Id)
                            //let DCparam = pressureStartTimeDC.Concat(pressureDataDC).ToList().Concat(timeOutValue).ToList()
                        from upMes in _commonService.UploadCatlMes(dto.Pin, pressureDataDC, true, dto.StationCode)
                        from mainComplate in _commonService.SetStationComplete(main.Id)
                        select productPn;

                var r = await q;
                if (r.IsError)
                {
                    return r.ErrorValue.ToErrResult<ServiceErrResponse, ServiceErrResponse>();
                }
                var resp = new ServiceErrResponse();
                return resp.ToSuccess().ToOkResult<ServiceErrResponse, ServiceErrResponse>();
            }
            catch (Exception ex)
            {
                var resp = new ServiceErrResponse();
                return resp.ToError(ResponseErrorType.上位机错误, 500, ex.Message).ToErrResult<ServiceErrResponse, ServiceErrResponse>();
            }
        }

        private async Task<FSharpResult<IList<DcParamValue>, ServiceErrResponse>> DealPressureData(IList<Base_StationTask> stationTasks, HeatingFilmPressurizeDataUploadDto dto, int MainId)
        {
            using var tran = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var resp = new ServiceErrResponse();

                if (dto.PressurizeDatas == null)
                {
                    return resp.ToError(ResponseErrorType.数据异常, 500, $"没有提供压力数据，请检查后重试").ToErrResult<IList<DcParamValue>, ServiceErrResponse>();
                }

                //现有的数据设计，自动站只允许有一个任务配置
                var stationTask = stationTasks.Where(a => a.Type == StationTaskTypeEnum.自动加压).FirstOrDefault();
                if (stationTask == null)
                {
                    return resp.ToError(ResponseErrorType.上位机错误, 500, $"任务配置数量【{stationTasks.Count}】异常，请检查后重试").ToErrResult<IList<DcParamValue>, ServiceErrResponse>();
                }

                //获取配方
                var formulas = await _dbContext.Base_AutoStationTask_HeatingFilmPressurizes.Where(w => w.StationTaskId == stationTask.Id && !w.IsDeleted).ToListAsync();

                //整理数据
                var PressureDatas = new List<HeatingFilmPressurizeData>();
                foreach (var formula in formulas)
                {
                    var val = dto.PressurizeDatas.GetValueOrDefault(formula.ParameterName);
                    if (val == null)
                    {
                        return resp.ToError(ResponseErrorType.数据异常, 500, $"参数【{formula.ParameterName}】没有提供数据，请检查后重试").ToErrResult<IList<DcParamValue>, ServiceErrResponse>();
                    }
                    PressureDatas.Add(new HeatingFilmPressurizeData { ParamName = formula.ParameterName, Value = val.ToString(), UploadCode = formula.UpMesCode, ValueType = ValueTypeEnum.NUMBER });
                }

                //保存数据
                var pressureInfo = await _dbContext.Proc_HeatingFilmPressurizeInfos.Where(w => w.Proc_StationTask_MainId == MainId && !w.IsDeleted).FirstOrDefaultAsync();
                if (pressureInfo == null)
                {
                    pressureInfo = new Proc_HeatingFilmPressurizeInfo();
                    pressureInfo.PackPN = dto.Pin;
                    pressureInfo.StationCode = dto.StationCode;
                    pressureInfo.Proc_StationTask_MainId = MainId;
                    pressureInfo.PressurizeDataJson = JsonConvert.SerializeObject(PressureDatas);
                    await _dbContext.AddAsync(pressureInfo);
                }
                pressureInfo.PressurizeDataJson = JsonConvert.SerializeObject(PressureDatas);
                await _dbContext.SaveChangesAsync();
                await tran.CommitAsync();

                var dcParams = PressureDatas.Select(e => new DcParamValue { ParamValue = e.Value, DataType = e.ValueType, UpMesCode = e.UploadCode }).ToList();
                return dcParams.ToOkResult<IList<DcParamValue>, ServiceErrResponse>();
            }
            catch (Exception ex)
            {
                await tran.RollbackAsync();
                var resp = new ServiceErrResponse();
                return resp.ToError(ResponseErrorType.上位机错误, 500, ex.Message).ToErrResult<IList<DcParamValue>, ServiceErrResponse>();
            }
        }

        public async Task<Response<IList<Proc_HeatingFilmPressurizeInfo>>> LoadData(PressureInfosDto dto)
        {
            var resp = new Response<IList<Proc_HeatingFilmPressurizeInfo>>();
            try
            {
                var datas = await _dbContext.Proc_HeatingFilmPressurizeInfos.Where(w => !w.IsDeleted).OrderByDescending(o => o.Id).ToListAsync();
                if (!string.IsNullOrWhiteSpace(dto.PackCode))
                {
                    datas = datas.Where(w => w.PackPN == dto.PackCode).ToList();
                }
                if (!string.IsNullOrWhiteSpace(dto.StationCode))
                {
                    datas = datas.Where(w => w.StationCode == dto.StationCode).ToList();
                }
                resp.Count = datas.Count();
                resp.Result = datas.Skip(dto.Limit * (dto.Page - 1)).Take(dto.Limit).ToList();
            }
            catch (Exception ex)
            {
                resp.Code = 500;
                resp.Message = ex.Message;
            }
            return resp;
        }

        public async Task<Response<IList<HeatingFilmPressurizeData>>> LoadDataDetail(int dataId)
        {
            var resp = new Response<IList<HeatingFilmPressurizeData>>();
            try
            {
                var data = await _dbContext.Proc_HeatingFilmPressurizeInfos.FirstOrDefaultAsync(w => w.Id == dataId);
                if (data == null)
                {
                    resp.Code = 500;
                    resp.Message = $"记录未找到";
                    return resp;
                }
                var list = data.PressurizeDatas;
                resp.Result = list;
            }
            catch (Exception ex)
            {
                resp.Code = 500;
                resp.Message = ex.Message;

            }
            return resp;
        }

        public async Task<Response<CatlMESReponse>> UploadDataAgain(string packCode, string stationCode, bool isNeedChangeStatus)
        {
            var resp = new Response<CatlMESReponse>();
            try
            {
                var main = await _dbContext.Proc_StationTask_Mains.FirstOrDefaultAsync(f => f.PackCode == packCode && f.StationCode == stationCode && !f.IsDeleted);
                if (main == null)
                {
                    resp.Code = 500;
                    resp.Message = $"主记录记录未找到";
                    return resp;
                }
                var data = await _dbContext.Proc_HeatingFilmPressurizeInfos.FirstOrDefaultAsync(w => w.Proc_StationTask_MainId == main.Id && !w.IsDeleted);
                if (data == null)
                {
                    resp.Code = 500;
                    resp.Message = $"记录未找到";
                    return resp;
                }
                var json = JsonConvert.DeserializeObject<IList<HeatingFilmPressurizeData>>(data.PressurizeDataJson);

                var dcparams = json.Select(s => new DcParamValue { DataType = ValueTypeEnum.NUMBER, ParamValue = s.Value.ToString(), UpMesCode = s.UploadCode }).ToList();
                //var records = await _dbContext.Proc_StationTask_Records.Where(w => w.Proc_StationTask_MainId == main.Id).ToListAsync();
                //foreach (var item in records)
                //{
                //    var time = await _dbContext.Proc_StationTask_TimeRecords.Where(w => w.Proc_StationTask_RecordId == item.Id && !w.IsDeleted).ToListAsync();
                //    dcparams = dcparams.Concat(time.Select(s => new DcParamValue { DataType = ValueTypeEnum.TEXT, ParamValue = s.TimeValue.ToString(), UpMesCode = s.UploadMesCode })).ToList();
                //    break;
                //}

                //foreach (var item in records)
                //{
                //    var time = await _dbContext.Proc_StationTask_CheckTimeouts.Where(w => w.StationTask_RecordId == item.Id && !w.IsDeleted).ToListAsync();
                //    dcparams = dcparams.Concat(time.Select(s => new DcParamValue { DataType = ValueTypeEnum.TEXT, ParamValue = s.Time.ToString(), UpMesCode = s.UpMesCode })).ToList();
                //    break;
                //}
                resp.Result = await _mesInvoker.dataCollect(main.PackCode, dcparams, isNeedChangeStatus, stationCode);
            }
            catch (Exception ex)
            {
                resp.Code = 500;
                resp.Message = ex.Message;
            }
            return resp;
        }
    }
}
