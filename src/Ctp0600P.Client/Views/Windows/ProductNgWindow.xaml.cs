using AsZero.Core.Services.Messages;
using Ctp0600P.Client.Apis;
using Ctp0600P.Client.Command.NoticeHelp.Enum;
using Ctp0600P.Client.Command.NoticeHelp.Handle;
using Ctp0600P.Client.Protocols;
using Ctp0600P.Client.ViewModels;
using FutureTech.Mvvm;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
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
using Yee.Entitys.DBEntity.ProductionRecords;
using static Ctp0600P.Client.ViewModels.RealtimePageViewModel;

namespace Ctp0600P.Client.Views.Windows
{
    /// <summary>
    /// ProductNgWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ProductNgWindow : Window
    {
        public ProductNgWindow(ProductNGViewModel vm)
        {
            InitializeComponent();
            this.DataContext = vm;
        }

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
    public class ProductNGViewModel : ViewModelBase
    {
        public ProductNGViewModel(APIHelper aPIHelper, NGBack ngback , RealtimePageViewModel realtimePageViewModel ,IMediator mediator)
        {
            this.aPIHelper = aPIHelper;
            this.nGBack = ngback;
            this.realtimePageViewModel = realtimePageViewModel;
            this.mediator = mediator;
            this.Sumbit = new RelayCommand<Window>(async o =>
            {
                try
                {
                    if (string.IsNullOrEmpty( this.Code))
                    {
                        mediator.Publish(new UILogNotification(new AlarmMessage { Timestamp = DateTime.Now, Level = LogLevel.Error, Content = "当前PACK条码为空,不可下线" }));
                       
                        return;
                    }

                    var dto = new Proc_Product_Offline();
                    dto.ProductCode = this.Code;
                    dto.AGVCode = App.AGVCode;
                    dto.StationId = App._StepStationSetting.Station.Id;
                    dto.StepId = App._StepStationSetting.Step.Id;
                    dto.InstorageTime=DateTime.Now;
                    dto.State = Proc_ProductStates.NG下线;
                    var result = await aPIHelper.ProductOutLine(dto);
                    if(result.Code != 200 || !result.Result )
                    {
                        await mediator.Publish(new AlarmSYSNotification { Code = AlarmCode.系统运行错误, Name = AlarmCode.系统运行错误.ToString(), Module = AlarmModule.DESOUTTER_MODULE, Description = $"下线失败" });
                        if (nGBack != null)
                        {
                            this.nGBack(this.Code, false);
                        }
                        return;
                    }
                    if (result.Result)
                    {
                        await mediator.Publish(new UILogNotification(new LogMessage { Timestamp = DateTime.Now, Level = LogLevel.None, Content = $"NG下线成功!，AGV：{App.AGVCode}" }));
                        if (nGBack != null)
                        {
                            this.nGBack(this.Code, true);
                        }
                        o.Close();
                        return;
                    }
                }
                catch (Exception ex)
                {

                    var notice = new MessageNotice() { messageType = MessageTypeEnum.错误, showType = MessageShowType.右下角弹窗, MessageStr = $"{ex.Message}！" };
                    this.mediator.Publish(notice);
                }
            },
            o=>true);
        }

        private string _code;
        private readonly APIHelper aPIHelper;
        private readonly NGBack nGBack;
        private readonly RealtimePageViewModel realtimePageViewModel;
        private readonly IMediator mediator;

        public string Code
        {
            get => _code;
            set
            {
                if (this._code != value)
                {
                    _code = value;
                    this.OnPropertyChanged(nameof(Code));
                }
            }
        }
        public ICommand Sumbit { get; }
    }
}
