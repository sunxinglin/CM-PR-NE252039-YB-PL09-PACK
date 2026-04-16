using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

using AsZero.Core.Entities;
using AsZero.Core.Services.Auth;
using AsZero.Core.Services.Repos;

using Ctp0600P.Client.Command.NoticeHelp.Enum;
using Ctp0600P.Client.Command.NoticeHelp.Handle;
using Ctp0600P.Client.Protocols;
using Ctp0600P.Client.Views;

using FutureTech.Mvvm;

using MediatR;

using Microsoft.Extensions.DependencyInjection;

using Yee.Entitys.AlarmMgmt;

namespace Ctp0600P.Client.ViewModels;

public class LoginPageViewModel : ViewModelBase
{
    private readonly IPrincipalAccessor _principalAccessor;
    private readonly IMediator _mediator;
    private readonly IServiceScopeFactory _scf;
    private readonly AppViewModel _appVM;
    private readonly IAPIHelper _iAPIHelper;

    public LoginPageViewModel(IPrincipalAccessor principalAccessor, IMediator mediator, IServiceScopeFactory scf,
        AppViewModel appVM, IAPIHelper iApiHelper)
    {
        _principalAccessor = principalAccessor;
        _mediator = mediator;
        _scf = scf;
        _appVM = appVM;
        _iAPIHelper = iApiHelper;

        this.CmdValidateUser = new AsyncRelayCommand<PasswordBox>(async pwbox =>
        {
            var loginResp = await this.TrySwitchUser(this.Account, pwbox.Password);
            pwbox.Clear();
            if (loginResp.Code != 200)
            {
                var notice = new MessageNotice()
                {
                    messageType = MessageTypeEnum.警告, showType = MessageShowType.右下角弹窗,
                    MessageStr = loginResp.Message
                };
                await this._mediator.Publish(notice);
            }
            else
            {
                using (var scope = _scf.CreateScope())
                {
                    var sp = scope.ServiceProvider;

                    var loginWin = sp.GetRequiredService<LoginWindow>();
                    var mainWin = sp.GetRequiredService<MainWindow>();
                    if (loginResp.Code == 200)
                    {
                        loginWin.Hide();
                        mainWin.Show();

                        _appVM.NavigateTo(App._StepStationSetting.StepType.ToString());
                    }
                    else
                    {
                        var notice = new MessageNotice()
                        {
                            messageType = MessageTypeEnum.警告, showType = MessageShowType.右下角弹窗,
                            MessageStr = "登录失败：请检查账户或者密码是否正确"
                        };
                        await this._mediator.Publish(notice);
                        await mediator.Publish(new AlarmSYSNotification()
                        {
                            Code = AlarmCode.系统运行错误, Name = nameof(AlarmCode.系统运行错误),
                            Module = AlarmModule.DESOUTTER_MODULE, Description = $"登录失败：请检查账户或者密码是否正确"
                        });
                    }
                }
            }
        }, o => true);

        this.LoginCmd = new AsyncRelayCommand<PasswordBox>(async cardNo =>
        {
            if (string.IsNullOrWhiteSpace(cardNo.Password))
            {
                var notice = new MessageNotice()
                {
                    messageType = MessageTypeEnum.警告, showType = MessageShowType.右下角弹窗, MessageStr = "未识别到卡号，请稍后重试"
                };
                await _mediator.Publish(notice);
                return;
            }

            var loginResp = await this.TrySwitchUser(cardNo.Password, cardNo.Password);
            cardNo.Clear();
            if (loginResp.Code != 200)
            {
                var notice = new MessageNotice()
                {
                    messageType = MessageTypeEnum.警告, showType = MessageShowType.右下角弹窗,
                    MessageStr = loginResp.Message
                };
                await _mediator.Publish(notice);
                return;
            }

            using var scope = this._scf.CreateScope();
            var sp = scope.ServiceProvider;

            var loginWin = sp.GetRequiredService<LoginWindow>();
            var mainWin = sp.GetRequiredService<MainWindow>();
            loginWin.Hide();
            mainWin.Show();
            _appVM.NavigateTo(App._StepStationSetting.StepType.ToString());
            LoadAGVInfo();
        }, o => true);
    }

    private async void LoadAGVInfo()
    {
        var agv = await _iAPIHelper.LoadStationCurrentAGV();
        if (agv.Code == 200)
        {
            if (agv.Result != null)
            {
                App.AGVCode = agv.Result.AGVNo.ToString();
                App.PackOutCode = agv.Result.HolderBarCode;
            }
        }
    }

    //private async Task<LoginResponse> TrySwithUserAsync(string account)
    //{
    //    using (var scope = this._scf.CreateScope())
    //    {
    //        var sp = scope.ServiceProvider;
    //        var mediator = sp.GetRequiredService<IMediator>();
    //        var loginResp = await mediator.Send(new LoginWithCardRequest
    //        {
    //            Account = account
    //        });
    //        if (!loginResp.Status) return loginResp;
    //        await BindPrincipalAsync(loginResp.Principal);
    //        return loginResp;
    //    }
    //}

    private async Task<Response<User>> TrySwitchUser(string account, string password)
    {
        //await _iAPIHelper.LoginWithAPI(account, password);
        var loginResp = await _iAPIHelper.LoginWithAPI(account, password);
        if (loginResp == null) loginResp = new Response<User> { Code = 500, Message = "登录失败" };
        if (loginResp.Code != 200) return loginResp;
        BindPrincipalAsync(loginResp.Result);
        return loginResp;

        //return new Response<User> { };
    }

    public async void BindPrincipalAsync(User user)
    {
        _appVM.UserName.Value = user.Name;
        _appVM.UserId.Value = user.Id;
        var claim = await _iAPIHelper.CheckClaimWithAPI(user.Account, user.Password);
        if (claim.Code == 200)
        {
            _appVM.ClaimValue.Value = claim.Result.ClaimValue;
        }

        App.UserId = _appVM.UserId.Value;
        if (_appVM.UserName?.Value != null)
        {
            await _appVM.RefreshResourcesAsync();
        }
    }

    private string _account = "operator";

    public string Account
    {
        get => _account;
        set
        {
            if (_account != value)
            {
                _account = value;
                OnPropertyChanged(nameof(Account));
            }
        }
    }


    private string _tips;

    public string Tips
    {
        get => _tips;
        set
        {
            if (_tips != value)
            {
                _tips = value;
                OnPropertyChanged(nameof(Tips));
            }
        }
    }

    private bool _hasSignedIn = false;

    public bool HasSignedIn
    {
        get => _hasSignedIn;
        set
        {
            if (_hasSignedIn != value)
            {
                _hasSignedIn = value;
                OnPropertyChanged(nameof(HasSignedIn));
            }
        }
    }

    public ICommand CmdValidateUser { get; }
    public ICommand LoginCmd { get; }
}