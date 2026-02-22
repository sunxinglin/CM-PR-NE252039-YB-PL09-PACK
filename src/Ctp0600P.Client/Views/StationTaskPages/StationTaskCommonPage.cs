using AsZero.Core.Services.Messages;
using Ctp0600P.Client.Command.NoticeHelp.Enum;
using Ctp0600P.Client.Command.NoticeHelp.Handle;
using Ctp0600P.Client.ViewModels.StationTaskViewModels;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Ctp0600P.Client.Views.StationTaskPages
{
    public class StationTaskCommonPage : Page
    {
        public IServiceProvider service;
        public RepairBoltGun_CommonViewModel _VM;
        public IMediator _mediator;
        public ILogger _logger;
        // public ZlanIOApi _zlanIOApi;
        public StationTaskCommonPage(IServiceProvider service, IMediator mediator, ILogger logger)
        {
            this.service = service;
            this._mediator = mediator;
            _logger = logger;
            // _zlanIOApi = zlanIOApi;
        }

        public StationTaskCommonPage(IServiceProvider service, IMediator mediator)
        {
            this.service = service;
            this._mediator = mediator;
        }
        public StationTaskCommonPage()
        {
        }
        public virtual async void StationTaskCommonPage_Loaded(object sender, RoutedEventArgs e)
        {
            var fullName = sender.GetType().FullName;

            App.ActivePage = null;

            //App._ActivityGluingTimeoutPage = null;

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
                        ((RepairBoltGunCommon)App.ActivePage)._VM.FindNextNG();
                        //((RepairBoltGunCommon)App.ActivePage)._VM.EnableBoltGuns();
                    }
                    catch (Exception ex)
                    {
                        var notice = new MessageNotice() { messageType = MessageTypeEnum.错误, showType = MessageShowType.右下角弹窗, MessageStr = $"打开窗口错误,请联系管理员!！" };
                        await this._mediator.Publish(notice);
                        this._logger.LogError($"打开绑定窗口出错：{ex.Message}\n{ex.StackTrace}");
                    }
                    break;
                case "Ctp0600P.Client.Views.StationTaskPages.RecordTimeTaskPage":
                    App.ActivePage = (RecordTimeTaskPage)sender;
                    ((RecordTimeTaskPage)App.ActivePage)._vm.CatchRecordTime();
                    break;

            }
        }
    }
}
