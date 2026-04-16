using System;
using System.Reactive.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

using AsZero.Core.Services.Messages;

using Ctp0600P.Client.PLC.Context;
using Ctp0600P.Client.Protocols;
using Ctp0600P.Client.Protocols.ScanCode.Models;
using Ctp0600P.Shared;

using FutureTech.Mvvm;

using MediatR;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Yee.Entitys.AlarmMgmt;

namespace Ctp0600P.Client.ViewModels;

public class AGVBingPageViewModel : ViewModelBase
{
    private readonly IMediator _mediator;
    private readonly StationPLCContext _stationPLCContext;
    private readonly IOptionsMonitor<AGVBindConfig> _agvBindConfig;

    public AGVBingPageViewModel(IMediator mediator, StationPLCContext stationPLCContext, IOptionsMonitor<AGVBindConfig> agvBindConfig)
    {
        _mediator = mediator;
        _stationPLCContext = stationPLCContext;
        _agvBindConfig = agvBindConfig;

        App.ScanCodeGunRequestSubject.ToEvent().OnNext -= CatchScanCodeMessage;
        App.ScanCodeGunRequestSubject.ToEvent().OnNext += CatchScanCodeMessage;
    }

    private string _PackBarCode;
    public string PackBarCode
    {
        get => _PackBarCode;
        set
        {
            if (_PackBarCode != value)
            {
                _PackBarCode = value;
                OnPropertyChanged(nameof(PackBarCode));
            }
        }
    }

    private string _OutCode;
    public string OutCode
    {
        get => _OutCode;
        set
        {
            if (_OutCode != value)
            {
                _OutCode = value;
                OnPropertyChanged(nameof(OutCode));
            }
        }
    }


    public delegate void CloseWindow();
    public event CloseWindow CloseWindowEvent;

    public async void CatchScanCodeMessage(ScanCodeGunRequest request)
    {
        // 仅当实时页与“AGV绑定弹窗”都处于激活状态时，才接收扫码枪消息进行绑定流程
        if (App._RealtimePage == null || App._ActivityAGVBingPage == null)
        {
            return;
        }

        Application.Current.Dispatcher.Invoke(() =>
        {
            // 不需要绑定出货码：扫一个 Pack 条码即可触发绑定
            if (!_agvBindConfig.CurrentValue.NeedHolderBarcode)
            {
                PackBarCode = request.ScanCodeContext;
            }
            else
            {
                // 需要绑定出货码：需要扫描Pack码和出货码
                // 规则：长度为 12 的条码优先认为是出货码（仅在 OutCode 为空时写入）
                if (request.ScanCodeContext.Length == 12 && string.IsNullOrWhiteSpace(OutCode))
                {
                    OutCode = request.ScanCodeContext;
                }
                else
                {
                    // 其余情况认为是Pack码（仅在 PackBarCode 为空时写入，避免后续误覆盖）
                    if (string.IsNullOrWhiteSpace(PackBarCode))
                    {
                        PackBarCode = request.ScanCodeContext;
                    }
                }
            }
        });

        // 满足绑定所需信息后触发绑定：
        // - 永远要求 Pack码 不为空
        // - 若需要出货码，则 OutCode 也必须不为空
        if (!string.IsNullOrEmpty(PackBarCode) &&
            (!string.IsNullOrEmpty(OutCode) || !_agvBindConfig.CurrentValue.NeedHolderBarcode))
        {
            var result = await BindingAgv();
            if (result)
            {
                // 绑定成功：关闭弹窗并清空本次输入，避免下次弹窗沿用旧数据
                CloseWindowEvent();
                this.PackBarCode = null;
                this.OutCode = null;
            }
        }
    }

    public async Task<bool> BindingAgv()
    {
        try
        {
            if (_stationPLCContext.AGVBindPackReq.Req)
            {
                await _mediator.Publish(new UILogNotification
                {
                    LogMessage = new LogMessage
                    {
                        Timestamp = DateTime.Now,
                        Content = $"【AGV绑定】当前存在处理中的绑定请求，{JsonSerializer.Serialize(_stationPLCContext.AGVBindPackReq)}",
                        Level = LogLevel.Information
                    }
                });
                return false;
            }

            _stationPLCContext.AGVBindPackReq.Req = true;
            _stationPLCContext.AGVBindPackReq.AGVNo = Convert.ToUInt16(App.AGVCode);
            _stationPLCContext.AGVBindPackReq.Behavior = 1;
            _stationPLCContext.AGVBindPackReq.PackCode = this.PackBarCode;
            _stationPLCContext.AGVBindPackReq.StationCode = App._StepStationSetting.StationCode;
            _stationPLCContext.AGVBindPackReq.HolderBarcode = this.OutCode;

            await _mediator.Publish(new UILogNotification
            {
                LogMessage = new LogMessage
                {
                    Timestamp = DateTime.Now, 
                    Content = $"【AGV绑定】发起请求：{App.AGVCode}号车[绑定条码]{this.PackBarCode}",
                    Level = LogLevel.Information
                }
            });


            /*
            //用WebApi直接绑定AGV，现在要走PLC，故注释
            var hash = this.GetHashCode();
            var dto = new BindAgvDTO()
            {
                AgvCode = Convert.ToInt32(App.AGVCode),
                State = BingAgvStateEnum.绑定,
                PackPN = this.PackBarCode,
                StationCode = App._StepStationSetting.StationCode,
                HolderBarCode = this.OutCode,
            };
            var request = await _apiHelper.BingAGVPack(dto);
            if (request.Code == 200)
            {
                await _mediator.Publish(new UILogNotification() { LogMessage = new LogMessage { Timestamp = DateTime.Now, Content = $"【AGV】{App.AGVCode}号车[绑定条码]{this.PackBarCode}成功", Level = LogLevel.Information } });
            }
            else
            {
                await _mediator.Publish(new AlarmSYSNotification() { Code = AlarmCode.系统运行错误, Name = AlarmCode.系统运行错误.ToString(), Module = AlarmModule.DESOUTTER_MODULE, Description = $"【AGV】{App.AGVCode}号车[绑定条码]{this.PackBarCode}失败" });
                return false;
            }*/

        }
        catch (Exception e)
        {
            await _mediator.Publish(new AlarmSYSNotification
            {
                Code = AlarmCode.系统运行错误, Name = nameof(AlarmCode.系统运行错误), Module = AlarmModule.AGV_MODULE,
                Description = $"绑定AGV错误：{e.Message}\n{e.StackTrace}"
            });
            return false;
        }

        return true;
    }
}
