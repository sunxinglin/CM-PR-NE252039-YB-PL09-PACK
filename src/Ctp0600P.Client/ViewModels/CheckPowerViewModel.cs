using AsZero.Core.Services.Messages;
using Ctp0600P.Client.Apis;
using Ctp0600P.Client.Protocols;
using Ctp0600P.Client.Views.Windows;
using FutureTech.Mvvm;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Yee.Entitys.AlarmMgmt;

namespace Ctp0600P.Client.ViewModels
{
    public class CheckPowerViewModel : ViewModelBase
    {
        private readonly APIHelper _apiHelper;
        private readonly RealtimePageViewModel _realtimePageViewModel;
        private readonly IServiceScopeFactory _scf;
        private readonly IMediator _mediator;

        public CheckPowerViewModel(APIHelper apiHelper, RealtimePageViewModel realtimePageViewModel, IServiceScopeFactory scf, IMediator mediator)
        {
            this._apiHelper = apiHelper;
            this._realtimePageViewModel = realtimePageViewModel;
            this._scf = scf;
            this._mediator = mediator;

            this.Check = new AsyncRelayCommand<PasswordBox>(async o =>
            {
                try
                {

                    if (string.IsNullOrWhiteSpace(o.Password))
                    {
                        realtimePageViewModel.AddLogMsg.Execute(new LogMessage { Timestamp = DateTime.Now, Level = LogLevel.Warning, Content = $"请刷卡" });
                        return;
                    }
                    using var scope = this._scf.CreateScope();
                    var sp = scope.ServiceProvider;
                    var mediator = sp.GetRequiredService<IMediator>();
                    var checkwin = sp.GetRequiredService<CheckPowerPage>();
                    var loginResp = await apiHelper.CheckPowe(o.Password, ModleName);
                    if (loginResp.Item1)
                    {
                        apiHelper.Record_CheckPower_Log(o.Password, ModleName, realtimePageViewModel.Alarms.ToList());

                        App.PowerUser = o.Password;
                        checkwin.Visibility = Visibility.Hidden;
                        ShowWin();
                    }
                    else
                    {
                        realtimePageViewModel.AddLogMsg.Execute(new LogMessage { Timestamp = DateTime.Now, Level = LogLevel.Warning, Content = $"{loginResp.Item2}" });
                        ErrorMessage = loginResp.Item2;
                    }
                    o.Password = string.Empty;
                }
                catch (Exception ex)
                {
                    await this._mediator.Publish(new AlarmSYSNotification() { Code = AlarmCode.系统运行错误, Name = AlarmCode.系统运行错误.ToString(), Module = AlarmModule.DESOUTTER_MODULE, Description = $"验证权限时发生错误{ex.Message}" });
                    throw;
                }
            }, o => true);

        }

        private void ShowWin()
        {
            if (this.Action != null)
            {
                Action();
            }
            else
            {
                _realtimePageViewModel.AddLogMsg.Execute(new LogMessage { Timestamp = DateTime.Now, Level = LogLevel.Warning, Content = $"待执行事件为空!" });

            }
        }

        public Action Action { get; set; }

        public string ModleName { get; set; }

        public ICommand Check { get; }


        private string _ErrorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _ErrorMessage;
            set { if (this._ErrorMessage != value) { _ErrorMessage = value; this.OnPropertyChanged(nameof(ErrorMessage)); } }
        }

    }
}
