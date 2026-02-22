using AsZero.Core.Services.Messages;
using AsZero.DbContexts;
using Ctp0600P.Client.Apis;
using Ctp0600P.Client.Command.NoticeHelp.Enum;
using Ctp0600P.Client.Command.NoticeHelp.Handle;
using Ctp0600P.Client.Protocols;
using Ctp0600P.Client.Protocols.ScanCode.Models;
using Ctp0600P.Client.ViewModels;
using FutureTech.Mvvm;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Yee.Entitys.AlarmMgmt;
using Yee.Entitys.DTOS;

namespace Ctp0600P.Client.UserControls
{
    /// <summary>
    /// AGVBingViewControl.xaml 的交互逻辑
    /// </summary>
    public partial class PullStepBackPage : Window
    {
        public PullStepBackPageViewModel _VM;
        public PullStepBackPage(PullStepBackPageViewModel vm)
        {
            _VM = vm;
            this.DataContext = vm;
            InitializeComponent();
            Topmost = true;
            WindowStyle = WindowStyle.ToolWindow;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        protected override void OnClosed(EventArgs e)
        {
            App._ActivityPullStepBackPage = null;
            base.OnClosed(e);
        }

        private void Button_Click_PullStepBack(object sender, RoutedEventArgs e)
        {
            //this._VM.Enable = false;

            //var result = this._VM.PullStepBack();

            //this._VM.Enable = true;
            //if (result)
            //{
            //    this.Close();
            //    App._RealtimePage._VM.CatchScanCodeMessage();
            //}

        }
    }
    public class PullStepBackPageViewModel : ViewModelBase
    {
        public ICommand NumerClick { get; }

        public PullStepBackPageViewModel(ILogger<PullStepBackPageViewModel> logger, IMediator mediator, APIHelper aPIHelper, IServiceProvider service)
        {
            _mediator = mediator;
            App.ScanCodeGunRequestSubject.ToEvent().OnNext += CatchScanCodeMessage;
            this.NumerClick = new AsyncRelayCommand<Button>(
                async number =>
                {
                    if (number.Content.ToString() == "退格")
                    {
                        if (!String.IsNullOrEmpty(UserInput))
                            UserInput = UserInput.Substring(0, UserInput.Length - 1);
                    }
                    else
                    {
                        UserInput += number.Content.ToString();
                    }

                },
                o => true
            );
            this.apihelp = aPIHelper;
        }

        public async Task<bool> PullStepBack()
        {
            UserInput = UserInput.TrimStart('0');
            var result = await apihelp.PullStepBack(UserInput, PackBarCode, OutCode);
            if (result.Code != 200)
            {
                var notice = new MessageNotice() { messageType = MessageTypeEnum.警告, showType = MessageShowType.右下角弹窗, MessageStr = result.Message };
                await this._mediator.Publish(notice);
            }
            else
            {
                App.AGVCode = UserInput;
                App.PackOutCode = OutCode;
                App.PackBarCode = PackBarCode;
            }
            return result.Code == 200;
        }

        public ICommand scan_1 { get; }
        public ICommand scan_2 { get; }


        private string _UserInput = "";
        public string UserInput
        {
            get => _UserInput;
            set
            {
                if (this._UserInput != value)
                {
                    _UserInput = value;
                    this.OnPropertyChanged(nameof(UserInput));
                }
            }
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

        private Boolean _enable = true;
        public Boolean Enable
        {
            get => _enable;
            set
            {
                if (_enable != value)
                {
                    _enable = value;
                    OnPropertyChanged(nameof(Enable));
                }
            }
        }
        private readonly APIHelper apihelp;

        private readonly IMediator _mediator;

        public void CatchScanCodeMessage(ScanCodeGunRequest request)
        {

            if(App._RealtimePage == null || App._ActivityPullStepBackPage == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(OutCode))
            {
                if (request.ScanCodeContext.Length != 12)
                {
                    var notice = new MessageNotice() { messageType = MessageTypeEnum.警告, showType = MessageShowType.右下角弹窗, MessageStr = "请先扫描12位出货码！" };
                    this._mediator.Publish(notice);
                    return;
                }
                OutCode = request.ScanCodeContext;
            }
            else if (string.IsNullOrEmpty(PackBarCode))
            {
                if (request.ScanCodeContext.Length != 24)
                {
                    var notice = new MessageNotice() { messageType = MessageTypeEnum.警告, showType = MessageShowType.右下角弹窗, MessageStr = "请扫描24位Pack码！" };
                    this._mediator.Publish(notice);
                    return;
                }
                PackBarCode = request.ScanCodeContext;
            }
        }
    }
}
