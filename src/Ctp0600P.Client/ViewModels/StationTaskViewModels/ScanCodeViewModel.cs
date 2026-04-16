using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AsZero.Core.Services.Messages;

using Ctp0600P.Client.Apis;
using Ctp0600P.Client.Protocols;
using Ctp0600P.Client.Protocols.ScanCode.Models;

using MediatR;

using Reactive.Bindings;

using Yee.Common.Library.CommonEnum;
using Yee.Entitys.AlarmMgmt;
using Yee.Entitys.CATL;
using Yee.Entitys.DBEntity;
using Yee.Entitys.DTOS;
using Yee.Entitys.Production;
using Yee.Services.CatlMesInvoker;

namespace Ctp0600P.Client.ViewModels.StationTaskViewModels;

public class ScanCodeViewModel : TaskViewModelBase
{
    private static bool IsDoingScanJob = false;
    private readonly APIHelper _apiHelper;
    public ScanCodeViewModel(StationTaskDTO stationTaskDTO, Base_StationTaskBom taskBom, IMediator mediator, ICatlMesInvoker catlMesInvoker, APIHelper _apiHelper)
    {
        StationTaskDTO = stationTaskDTO;
        this._mediator = mediator;
        this._apiHelper = _apiHelper;
        _catlMesInvoker = catlMesInvoker;
        StationTaskBom = taskBom;
        StationTaskBomList = new ReactiveCollection<Base_StationTaskBom> { taskBom };
        BindHisData();
    }

    public ReactiveProperty<string> UniBarCode { get; }
    #region 【绑定历史信息】

    public void BindHisData()
    {
        //if (App.HisTaskData != null && App.HisTaskData.Proc_StationTask_Main != null)
        //{
        //    foreach (var record in App.HisTaskData.Proc_StationTask_Main.Proc_StationTask_Records)
        //    {
        //        if (record.Base_StationTaskId == StationTaskDTO.StationTaskId)
        //        {
        //            BindHisData(record);
        //        }
        //    }
        //}

        var histDto = App.HisTaskData2.StationTaskRecords.FirstOrDefault(f => f.StationTaskId == StationTaskDTO.StationTaskId);
        var boms = histDto?.Boms;
        if (boms is { Count: > 0 })
        {
            StationTaskDTO.HasFinish = histDto.StationTaskRecord.Status == StationTaskStatusEnum.已完成;
            var bom = boms.FirstOrDefault();
            if (bom == null) return;
            StationTaskBom.Status = bom.StationTaskBom.Status.ToString();
            StationTaskBom.ScanBarCodeList = new ObservableCollection<ScanBarCode>(bom.BomDetails.Select(s => new ScanBarCode
            {
                BatchBarCode = s.BatchBarCode,
                GoodsOuterCode = s.GoodsOuterCode,
                UniBarCode = s.UniBarCode,
                OuterParam1 = s.OuterParam1,
                OuterParam2 = s.OuterParam2,
                OuterParam3 = s.OuterParam3,
                OuterParam1Result = s.OuterParam1Result,
                OuterParam2Result = s.OuterParam2Result,
                OuterParam3Result = s.OuterParam3Result,
            }).ToList());
        }
    }

    /// <summary>
    /// 绑定历史信息
    /// </summary>
    /// <param name="record"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void BindHisData(Proc_StationTask_Record record)
    {
        try
        {
            var hisRecord = record.Proc_StationTask_Boms.FirstOrDefault(h => h.GoodsPN == StationTaskBom.GoodsPN && h.GoodsName == StationTaskBom.GoodsName);
            if (hisRecord == null) return;
            StationTaskBom.Status = hisRecord.Status.ToString();
            StationTaskBom.ScanBarCodeList = new ObservableCollection<ScanBarCode>();
            foreach (var detail in hisRecord.Proc_StationTask_BomDetails)
            {
                StationTaskBom.ScanBarCodeList.Add(new ScanBarCode()
                {
                    BatchBarCode = detail.BatchBarCode,
                    GoodsOuterCode = detail.GoodsOuterCode,
                    OuterParam1 = detail.OuterParam1,
                    OuterParam2 = detail.OuterParam2,
                    OuterParam3 = detail.OuterParam3,
                    OuterParam1Result = detail.OuterParam1Result,
                    OuterParam2Result = detail.OuterParam2Result,
                    OuterParam3Result = detail.OuterParam3Result,
                    UniBarCode = detail.UniBarCode,
                    ViewBarCode = string.IsNullOrEmpty(detail.UniBarCode) ? String.IsNullOrEmpty(detail.GoodsOuterCode) ? detail.BatchBarCode : detail.GoodsOuterCode : detail.UniBarCode
                });
            }
        }
        catch (Exception ex)
        {
            _mediator.Publish(new AlarmSYSNotification() { Code = AlarmCode.系统运行错误, Name = AlarmCode.系统运行错误.ToString(), Module = AlarmModule.DESOUTTER_MODULE, Description = $"扫码绑定历史数据错误:{ex.Message}\r\n{ex.StackTrace}" });
        }

    }

    #endregion

    #region 【校验并获取任务】
    public async Task CheckBomAsync(ScanCodeGunRequest request)
    {
        try
        {
            // 1. 本地正则校验（长度、前缀）
            var checkResult = await this.IsCheckedOk(request.ScanCodeContext);
            if (!checkResult)
            {
                return;
            }
            // 2. 根据追溯类型分别处理
            switch (StationTaskBom.TracingType)
            {
                case TracingTypeEnum.扫库存:
                    // 如果已经扫码过了，直接返回
                    if (!string.IsNullOrEmpty(StationTaskBom.UniBarCode))
                    {
                        return;
                    }
                    StationTaskBom.UniBarCode = request.ScanCodeContext;
                    // 3. 调用MES接口校验库存
                    if (!await CheckCodeFromCatl(request.ScanCodeContext))
                    {
                        StationTaskBom.UniBarCode = "";
                        return;
                    }
                    ////校验重复使用，先屏蔽
                    //var checkUsed = await _apiHelper.CheckBomUsed(StationTaskBom);
                    //if (checkUsed.Code != 200)
                    //{
                    //    await mediator.Publish(new AlarmSYSNotification() { Code = AlarmCode.系统运行错误, Name = AlarmCode.系统运行错误.ToString(), Module = AlarmModule.DESOUTTER_MODULE, Description = $"{checkUsed.Message}" });
                    //    StationTaskBom.UniBarCode = "";

                    //    return;
                    //}
                    break;
                case TracingTypeEnum.批追:
                    if (!string.IsNullOrEmpty(StationTaskBom.BatchBarCode))
                    {
                        return;
                    }
                    StationTaskBom.BatchBarCode = request.ScanCodeContext;
                    if (!await CheckCodeFromCatl(request.ScanCodeContext))
                    {
                        StationTaskBom.BatchBarCode = "";
                        return;
                    }

                    break;
                case TracingTypeEnum.精追:
                    if (!string.IsNullOrEmpty(StationTaskBom.BatchBarCode) && !string.IsNullOrEmpty(StationTaskBom.GoodsOuterCode))
                    {
                        return;
                    }
                    if (string.IsNullOrEmpty(StationTaskBom.BatchBarCode))
                    {
                        StationTaskBom.BatchBarCode = request.ScanCodeContext;
                        if (!await CheckCodeFromCatl(request.ScanCodeContext))
                        {
                            StationTaskBom.BatchBarCode = "";
                            StationTaskBom.GoodsOuterCode = "";
                        }
                        return;
                    }
                    StationTaskBom.GoodsOuterCode = request.ScanCodeContext;
                    ////校验重复使用，先屏蔽
                    //var checkOuterUsed = await _apiHelper.CheckBomUsed(StationTaskBom);
                    //if (checkOuterUsed.Code != 200)
                    //{
                    //    await mediator.Publish(new AlarmSYSNotification() { Code = AlarmCode.系统运行错误, Name = AlarmCode.系统运行错误.ToString(), Module = AlarmModule.DESOUTTER_MODULE, Description = $"{checkOuterUsed.Message}" });
                    //    StationTaskBom.GoodsOuterCode = "";
                    //    return;
                    //}
                    break;
            }
            
            await DealUploadAndSave();
        }
        catch (Exception ex)
        {
            await _mediator.Publish(new AlarmSYSNotification() { Code = AlarmCode.系统运行错误, Name = nameof(AlarmCode.系统运行错误), Module = AlarmModule.SERVER_MODULE, Description = $"校验并获取任务错误:{ex.Message}\r\n{ex.StackTrace}" });
        }
    }
    private async Task DealUploadAndSave()
    {
        if (StationTaskBom.TracingType == TracingTypeEnum.精追 && string.IsNullOrEmpty(StationTaskBom.GoodsOuterCode))
        {
            return;
        }

        // 先上传CATL MES，成功则保存到数据库
        var catlRes = await UploadScanData();
        if (!catlRes)
        {
            StationTaskBom.BatchBarCode = "";
            StationTaskBom.GoodsOuterCode = "";
            StationTaskBom.UniBarCode = "";
            return;
        }
        StationTaskBom.CurNo = StationTaskBom.UseNum;
        if (!await SaveScanCodeDetailData())
        {
            StationTaskBom.BatchBarCode = "";
            StationTaskBom.GoodsOuterCode = "";
            StationTaskBom.UniBarCode = "";
            return;
        }
        CompleteTask();
    }
    #endregion

    #region 【处理扫码枪扫码消息】
    private readonly SemaphoreSlim Slim = new(1, 1);

    /// <summary>
    /// 处理扫码枪扫码消息
    /// </summary>
    /// <param name="request"></param>
    public async void CatchScanCodeMessage(ScanCodeGunRequest request)
    {
        // 等待2.5S精追码等待MES校验完成
        if (!await Slim.WaitAsync(2500))
        {
            return;
        }
        try
        {
            await CheckBomAsync(request);
        }
        finally
        {
            Slim.Release();
        }
    }
    #endregion

    #region 【完成任务】

    /// <summary>
    /// 当前任务完成
    /// </summary>
    private void CompleteTask()
    {
        UIHelper.RunInUIThread(pl =>
        {
            try
            {
                // 检查是否完成所有使用数量
                if (StationTaskBom.UseNum == StationTaskBom.CurNo)
                {
                    StationTaskBom.Status = "已完成";
                    StationTaskDTO.HasFinish = true;
                    // 触发事件，通知外层ViewModel切到下一步
                    OnCompleteTask(StationTaskDTO);
                }
            }
            catch (Exception ex)
            {
                _mediator.Publish(new AlarmSYSNotification { Code = AlarmCode.系统运行错误, Name = nameof(AlarmCode.系统运行错误), Module = AlarmModule.SERVER_MODULE, Description = $"设置扫码枪任务完成时错误:{ex.Message}\r\n{ex.StackTrace}" });
            }
        });
    }

    public async Task<bool> SaveScanCodeDetailData()
    {
        StationTaskBom.PackCode = App.PackBarCode;
        StationTaskBom.CreateUserID = App._RealtimePage.WorkingUserId;
        StationTaskBom.StationID = App._StepStationSetting.Station.Id;
        try
        {
            var resp = await _apiHelper.SaveScanCodeDetailData(StationTaskBom, StationTaskDTO.StationTaskId);
            if (resp.Code != 200)
            {
                await _mediator.Publish(new AlarmSYSNotification { Code = AlarmCode.扫码枪错误, Name = nameof(AlarmCode.系统运行错误), Module = AlarmModule.SERVER_MODULE, Description = $"{resp.Message}" });
                return false;
            }
            return true;
        }
        catch (Exception ex)
        {
            await _mediator.Publish(new AlarmSYSNotification { Code = AlarmCode.系统运行错误, Name = nameof(AlarmCode.系统运行错误), Module = AlarmModule.SERVER_MODULE, Description = $"接口调用异常：{ex.Message}" });
            return false;
        }
    }
    #endregion

    #region 【声明】
    private string _ScanCodeText;

    public string ScanCodeText
    {
        get => _ScanCodeText;
        set
        {
            if (_ScanCodeText != value)
            {
                _ScanCodeText = value;
                OnPropertyChanged(nameof(ScanCodeText));
            }
        }
    }
    public StationTaskDTO StationTaskDTO { get; }

    public Base_StationTaskBom StationTaskBom
    {
        get => _StationTaskBom;
        set
        {
            if (_StationTaskBom != value)
            {
                _StationTaskBom = value;
                OnPropertyChanged(nameof(StationTaskBom));
            }
        }
    }

    private Base_StationTaskBom _StationTaskBom = new();

    private readonly IMediator _mediator;
    private readonly ICatlMesInvoker _catlMesInvoker;

    public ReactiveCollection<Base_StationTaskBom> StationTaskBomList { get; set; }
    #endregion

    private async Task PublishError(AlarmCode code, string content)
    {
        await _mediator.Publish(new AlarmSYSNotification { Code = code, Name = code.ToString(), Description = content });
    }

    /// <summary>
    /// 校验条码规则
    /// </summary>
    /// <param name="content">条码</param>
    /// <returns></returns>
    private async Task<bool> IsCheckedOk(string content)
    {
        if (!string.IsNullOrEmpty(this.StationTaskBom.BatchBarCode))
        {
            if (!await App.CompareCodeRuleAsync(StationTaskBom.OuterGoodsPNRex, content))
            {
                await PublishError(AlarmCode.扫码枪错误,
                    $"条码规则校验失败,规则:{StationTaskBom.OuterGoodsPNRex}-长度{StationTaskBom.OuterGoodsPNRex.Length},实际条码{content}-长度{content.Length}");
                return false;
            }
            return true;
        }
        if (!await App.CompareCodeRuleAsync(StationTaskBom.GoodsPNRex, content))
        {
            await PublishError(AlarmCode.扫码枪错误,
                $"条码规则校验失败,规则:{StationTaskBom.GoodsPNRex}-长度{StationTaskBom.GoodsPNRex.Length},实际条码{content}-长度{content.Length}");
            return false;
        }
        return true;

    }

    private async Task<bool> CheckCodeFromCatl(string content)
    {
        if (App._StepStationSetting.IsDebug)
        {
            return true;
        }
        await _mediator.Publish(new UILogNotification(new LogMessage { Content = "正在调用CATL MICheckBOMInventory接口" }));
        var stCode = !string.IsNullOrEmpty(App.CurrentStationCode) ? App.CurrentStationCode : App._StepStationSetting.StationCode;
        var response = await _catlMesInvoker.MICheckBOMInventory(App.PackBarCode, StationTaskBom.GoodsPN, content, StationTaskBom.UseNum, stCode);
        if (response.code != 0)
        {
            await _mediator.Publish(new AlarmSYSNotification { Code = AlarmCode.CatlMES错误, Name = nameof(AlarmCode.CatlMES错误), Module = AlarmModule.SERVER_MODULE, Description = $"批追: (Code={response.code}) {response.message}" });
            return false;
        }

        return true;
    }

    private async Task<bool> UploadScanData()
    {
        if (App._StepStationSetting.IsDebug)
        {
            return true;
        }

        var bomData = new BomData
        {
            InternalCode = this.StationTaskBom.TracingType == TracingTypeEnum.扫库存 ? this.StationTaskBom.UniBarCode : this.StationTaskBom.BatchBarCode ?? "",
            ExternalCode = this.StationTaskBom.GoodsOuterCode ?? "",
            TracingType = this.StationTaskBom.TracingType,
            UseNum = this.StationTaskBom.UseNum
        };

        await _mediator.Publish(new UILogNotification(new LogMessage { Content = "正在调用CATL MiAssembleAndCollectDataForSfc接口" }));
        var stCode = !string.IsNullOrEmpty(App.CurrentStationCode) ? App.CurrentStationCode : App._StepStationSetting.StationCode;
        var catlResult = await _catlMesInvoker.MiAssembleAndCollectDataForSfc(new List<BomData> { bomData }, App.PackBarCode, stCode);

        if (catlResult.code != 0)
        {
            await _mediator.Publish(new AlarmSYSNotification { Code = AlarmCode.CatlMES错误, Name = nameof(AlarmCode.CatlMES错误), Module = AlarmModule.SERVER_MODULE, Description = $"批追: (Code={catlResult.code}) {catlResult.message}" });
            return false;
        }
        return true;
    }
}