using System;
using System.Windows;
using System.Windows.Controls;

using Ctp0600P.Client.Command.NoticeHelp.Enum;
using Ctp0600P.Client.Command.NoticeHelp.Handle;
using Ctp0600P.Client.ViewModels.StationTaskViewModels;

using MediatR;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Ctp0600P.Client.Views.StationTaskPages;

public class StationTaskCommonPage : Page
{
    public IServiceProvider service;
    // ToOptimized:基类定义子类VM是反模式的
    public RepairBoltGun_CommonViewModel _VM;
    private readonly IMediator _mediator;
    public ILogger _logger;

    public StationTaskCommonPage(IServiceProvider service, IMediator mediator, ILogger logger)
    {
        this.service = service;
        _mediator = mediator;
        _logger = logger ?? service.GetService<ILogger<StationTaskCommonPage>>() ?? NullLogger<StationTaskCommonPage>.Instance;
    }

    public StationTaskCommonPage(IServiceProvider service, IMediator mediator)
    {
        this.service = service;
        _mediator = mediator;
        _logger = service.GetService<ILogger<StationTaskCommonPage>>() ?? NullLogger<StationTaskCommonPage>.Instance;
    }

    public StationTaskCommonPage()
    {
        service = ((App)Application.Current)?.RootServiceProvider;
        _mediator = service?.GetService<IMediator>();
        _logger = service?.GetService<ILogger<StationTaskCommonPage>>() ?? NullLogger<StationTaskCommonPage>.Instance;
    }
    
    public virtual async void StationTaskCommonPage_Loaded(object sender, RoutedEventArgs e)
    {
        var fullName = sender.GetType().FullName;

        App.ActivePage = null;
        
        App._ActivityStationTaskCommonPage = (StationTaskCommonPage)sender;

        switch (fullName)
        {
            case "Ctp0600P.Client.Views.StationTaskPages.AnyLoad":
                App.ActivePage = (AnyLoad)sender;
                break;
            case "Ctp0600P.Client.Views.StationTaskPages.ScanCode":
                App.ActivePage = (ScanCode)sender;
                break;
            case "Ctp0600P.Client.Views.StationTaskPages.BoltGun":
                App.ActivePage = (BoltGun)sender;
                //((BoltGun)App.ActivePage)._VM.EnableBoltGuns();
                break;
            case "Ctp0600P.Client.Views.StationTaskPages.LetGoPage":
                App.ActivePage = (LetGoPage)sender;
                break;
            case "Ctp0600P.Client.Views.StationTaskPages.ScanAccountCard":
                App.ActivePage = (ScanAccountCard)sender;
                break;
            case "Ctp0600P.Client.Views.StationTaskPages.CheckTimeout":
                App.ActivePage = (CheckTimeout)sender;
                ((CheckTimeout)App.ActivePage)._VM.CatchTimeMessage();
                break;
            //case "Ctp0600P.Client.Views.StationTaskPages.StewingTimeout":
            //    App._ActivityStewingTimeoutPage = (StewingTimeout)sender;
            //    App._ActivityStewingTimeoutPage._VM.CatchStewingTimeMessage();
            //    break;
            case "Ctp0600P.Client.Views.StationTaskPages.ScanCollect":
                App.ActivePage = (ScanCollect)sender;
                break;
            case "Ctp0600P.Client.Views.StationTaskPages.UserInputCollect":
                App.ActivePage = (UserInputCollect)sender;
                break;
            case "Ctp0600P.Client.Views.StationTaskPages.RepairBoltGunCommon":
                try
                {
                    App.ActivePage = (RepairBoltGunCommon)sender;
                    await ((RepairBoltGunCommon)App.ActivePage)._VM.LoadScrewList();
                    ((RepairBoltGunCommon)App.ActivePage)._VM.FindNextNG();
                    //((RepairBoltGunCommon)App.ActivePage)._VM.EnableBoltGuns();
                }
                catch (Exception ex)
                {
                    var notice = new MessageNotice { messageType = MessageTypeEnum.错误, showType = MessageShowType.右下角弹窗, MessageStr = "打开窗口错误,请联系管理员！" };
                    if (_mediator != null)
                    {
                        await _mediator.Publish(notice);
                    }
                    _logger.LogError("打开绑定窗口出错：{ExMessage}\n{ExStackTrace}", ex.Message, ex.StackTrace);
                }
                break;
            case "Ctp0600P.Client.Views.StationTaskPages.TightenByImage":
                App.ActivePage = (TightenByImage)sender;
                var vm = ((TightenByImage)App.ActivePage)._vm;
                if (App.ReworkLocateTightenByImageStationTaskId.HasValue &&
                    App.ReworkLocateTightenByImageOrderNo.HasValue &&
                    vm != null &&
                    vm._StationTaskDTO?.StationTaskId == App.ReworkLocateTightenByImageStationTaskId.Value)
                {
                    vm.FindNextUndo(App.ReworkLocateTightenByImageOrderNo.Value);
                    App.ReworkLocateTightenByImageStationTaskId = null;
                    App.ReworkLocateTightenByImageOrderNo = null;
                }
                else
                {
                    vm.FindNextUndo();
                }
                break;
            case "Ctp0600P.Client.Views.StationTaskPages.RecordTimeTaskPage":
                App.ActivePage = (RecordTimeTaskPage)sender;
                ((RecordTimeTaskPage)App.ActivePage)._vm.CatchRecordTime();
                break;

        }
    }
}