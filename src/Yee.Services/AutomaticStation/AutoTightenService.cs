using AsZero.Core.Services.Repos;
using AsZero.DbContexts;

using Itminus.FSharpExtensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.FSharp.Core;

using Newtonsoft.Json;

using Yee.Entitys.AutomaticStation;
using Yee.Entitys.DBEntity;
using Yee.Entitys.DTOS;
using Yee.Entitys.Production;

namespace Yee.Services.AutomaticStation;

public class AutoTightenService
{
    private readonly AsZeroDbContext _dbContext;
    private readonly AutomicCommonService _commonService;

    public AutoTightenService(AsZeroDbContext dbContext, AutomicCommonService automicStationCommonService)
    {
        _dbContext = dbContext;
        _commonService = automicStationCommonService;
    }

    public async Task<FSharpResult<ServiceErrResponse, ServiceErrResponse>> UploadData(AutoTightenDataUploadDto dto)
    {
        try
        {
            var resp = new ServiceErrResponse();
            var q = from mainData in _commonService.HasMainRecord(dto.Pin, dto.StationCode) //判断有无开始记录或者是否完工
                from prodctPN in _commonService.GetProductPnFromCatlMes(dto.Pin, dto.StationCode)//获取产品PN
                from formula in this.LoadAutoTightenFormula(prodctPN.BarCode_GoodsPN, dto.StationCode)//加载拧紧配方
                from saveData in this.SaveTightenData(prodctPN.BarCode_GoodsPN, mainData.Id, formula, dto)//保存拧紧数据
                from setMainComp in _commonService.SetStationComplete(mainData.Id)//设置main完工
                select setMainComp;

            var r = await q;

            if(r.IsError)
            {
                return r.ErrorValue.ToErrResult<ServiceErrResponse, ServiceErrResponse>();
            }

            return resp.ToSuccess().ToOkResult<ServiceErrResponse, ServiceErrResponse>();
        }
        catch (Exception ex)
        {
            var resp = new ServiceErrResponse();
            return resp.ToError(ResponseErrorType.上位机错误,500, ex.Message).ToErrResult<ServiceErrResponse, ServiceErrResponse>();
        }
    }

    private async Task<FSharpResult<Base_AutoStationTaskTighten, ServiceErrResponse>> LoadAutoTightenFormula(string productPN, string stationCode)
    {
        var resp = new ServiceErrResponse();
        var product = await _dbContext.Products.FirstOrDefaultAsync(f => !f.IsDeleted && f.Code == productPN);
        var station = await _dbContext.Base_Stations.FirstOrDefaultAsync(f => !f.IsDeleted && f.Code == stationCode);
        if (product == null)
        {
            resp.ErrorMessage = $"产品{productPN}不存在";
            resp.ErrorType = ResponseErrorType.上位机错误;
            return resp.ToErrResult<Base_AutoStationTaskTighten, ServiceErrResponse>();
        }
        if (station == null)
        {
            resp.ErrorMessage = $"工位{stationCode}不存在";
            resp.ErrorType = ResponseErrorType.上位机错误;
            return resp.ToErrResult<Base_AutoStationTaskTighten, ServiceErrResponse>();
        }
        var stationTask = await _dbContext.Base_StationTasks.Where(w => !w.IsDeleted && w.ProductId == product.Id && w.StepId == station.StepId).FirstOrDefaultAsync();
        if (stationTask == null)
        {
            resp.ErrorMessage = $"工位任务不存在";
            resp.ErrorType = ResponseErrorType.上位机错误;
            return resp.ToErrResult<Base_AutoStationTaskTighten, ServiceErrResponse>();
        }

        var formula = await _dbContext.Base_AutoStationTaskTightens.FirstOrDefaultAsync(f => !f.IsDeleted && f.StationTaskId == stationTask.Id);
        if (formula == null)
        {
            resp.ErrorMessage = $"工位任务不存在";
            resp.ErrorType = ResponseErrorType.上位机错误;
            return resp.ToErrResult<Base_AutoStationTaskTighten, ServiceErrResponse>();
        }
        return formula.ToOkResult<Base_AutoStationTaskTighten, ServiceErrResponse>();
    }

    private async Task<FSharpResult<Proc_AutoBoltInfo_Detail, ServiceErrResponse>> SaveTightenData(string productPN, int mainId, Base_AutoStationTaskTighten formula, AutoTightenDataUploadDto dto)
    {
        try
        {
            var boltExit = await _dbContext.Proc_AutoBoltInfo_Details.FirstOrDefaultAsync(f => !f.IsDeleted && f.Proc_StationTask_MainId == mainId);
            if (boltExit == null)
            {
                boltExit = new Proc_AutoBoltInfo_Detail
                {
                    PackPN = dto.Pin,
                    Proc_StationTask_MainId = mainId,
                    BoltDataArray = JsonConvert.SerializeObject(dto.TightenDatas),
                    BoltType = dto.BoltType,
                    UploadCode = formula.UpMesCode,
                    UploadCode_JD = formula.UpMesCode + "JD",
                    CreateTime = DateTime.Now,
                };
                await _dbContext.AddAsync(boltExit);
                await _dbContext.SaveChangesAsync();
                return boltExit.ToOkResult<Proc_AutoBoltInfo_Detail, ServiceErrResponse>();
            }

            boltExit.BoltDataArray = JsonConvert.SerializeObject(dto.TightenDatas);
            boltExit.UploadCode = formula.UpMesCode;
            boltExit.UploadCode_JD = formula.UpMesCode + "JD";
            boltExit.UpdateTime = DateTime.Now;
            _dbContext.Update(boltExit);
            await _dbContext.SaveChangesAsync();
            return boltExit.ToOkResult<Proc_AutoBoltInfo_Detail, ServiceErrResponse>();
        }
        catch (Exception ex)
        {
            var resp = new ServiceErrResponse();
            resp.ErrorMessage = ex.Message;
            resp.ErrorType = ResponseErrorType.上位机错误;
            resp.ErrorCode = 500;
            return resp.ToErrResult<Proc_AutoBoltInfo_Detail, ServiceErrResponse>();
        }
    }

    public async Task<Response<IList<Proc_AutoBoltInfo_Detail>>> LoadAutoTightenData(AutoTightenInfoDto dto)
    {
        var resp = new Response<IList<Proc_AutoBoltInfo_Detail>>();
        try
        {
            var datas = _dbContext.Proc_AutoBoltInfo_Details.Where(w => !w.IsDeleted && w.CreateTime != null);
            if (!string.IsNullOrEmpty(dto.PackCode))
            {
                datas = datas.Where(w => w.PackPN == dto.PackCode);
            }
            //if (!string.IsNullOrEmpty(dto.AutoBoltType))
            //{
            //    datas = datas.Where(w => w.BoltType == dto.AutoBoltType).ToList();
            //}
            if (dto.BeginTime != null)
            {
                datas = datas.Where(o => o.CreateTime!.Value.Date >= dto.BeginTime.Value.Date);
            }

            if (dto.EndTime != null)
            {
                datas = datas.Where(o => o.CreateTime!.Value.Date <= dto.EndTime.Value.Date);
            }

            resp.Count = datas.Count();
            resp.Result = await datas.OrderByDescending(o => o.Id).Skip(dto.Limit * (dto.Page - 1)).Take(dto.Limit).ToListAsync();
        }
        catch (Exception ex)
        {
            resp.Code = 500;
            resp.Message = ex.Message;

        }
        return resp;
    }

    public async Task<Response<IList<AutoBlotInfo>>> LoadAutoTightenDataDetail(int dataId)
    {
        var resp = new Response<IList<AutoBlotInfo>>();
        try
        {
            var data = await _dbContext.Proc_AutoBoltInfo_Details.FirstOrDefaultAsync(w => w.Id == dataId);
            if (data == null)
            {
                resp.Code = 500;
                resp.Message = $"记录未找到";
                return resp;
            }
            var json = JsonConvert.DeserializeObject<IList<AutoBlotInfo>>(data.BoltDataArray);
            resp.Result = json?.OrderBy(o => o.OrderNo).ToList();
        }
        catch (Exception ex)
        {
            resp.Code = 500;
            resp.Message = ex.Message;

        }
        return resp;
    }

}