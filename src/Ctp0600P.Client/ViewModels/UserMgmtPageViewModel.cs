using System.Collections.ObjectModel;
using System.Security.Claims;
using System.Windows.Input;

using AsZero.Core.Services.Auth;

using Ctp0600P.Client.Command.NoticeHelp.Enum;
using Ctp0600P.Client.Command.NoticeHelp.Handle;

using FutureTech.Mvvm;

using MediatR;

using Microsoft.Extensions.DependencyInjection;

namespace Ctp0600P.Client.ViewModels;

public class UserMgmtPageViewModel: ViewModelBase
{
    private readonly IPrincipalAccessor _principalAccessor;
    private readonly IServiceScopeFactory _ssf;
    private readonly IMediator _mediator;

    public ClaimsPrincipal Principal { get; }

    public UserMgmtPageViewModel(IPrincipalAccessor principalAccessor, IServiceScopeFactory ssf, AppViewModel appvm,IMediator mediator)
    {
        _principalAccessor = principalAccessor;
        _ssf = ssf;
        AppVM = appvm;
        _mediator = mediator;
        var principal = _principalAccessor.GetCurrentPrincipal();
        Principal = principal;

        var account = principal.FindFirst(i => i.Type == ClaimTypes.NameIdentifier)?.Value;
        var name = principal.FindFirst(i => i.Type == ClaimTypes.Name)?.Value;

        // 初始化当前账户名
        ChangeUserPasswordInput = new ChangeUserPasswordPayload
        { 
            Account = account,
        };

        CmdCreateUser = new AsyncRelayCommand<object>(
            async o => {

                if (CreateUserInput == null)
                {
                    var notice = new MessageNotice { messageType = MessageTypeEnum.错误, showType = MessageShowType.右下角弹窗, MessageStr = "请输入用户相关信息" };
                    await _mediator.Publish(notice);
                }

                var account = CreateUserInput.Account;
                var password = CreateUserInput.Password;
                var name = CreateUserInput.Name;
                var WorkId = CreateUserInput.WorkId;
                if (string.IsNullOrEmpty(account))
                {
                    var notice = new MessageNotice { messageType = MessageTypeEnum.错误, showType = MessageShowType.右下角弹窗, MessageStr = "账号名不可为空" };
                    await _mediator.Publish(notice);
                      
                    return;
                }

                if (string.IsNullOrEmpty(password))
                {
                    var notice = new MessageNotice { messageType = MessageTypeEnum.错误, showType = MessageShowType.右下角弹窗, MessageStr = "密码不可为空" };
                    await _mediator.Publish(notice);
                        
                    return;
                }

                if (string.IsNullOrEmpty(name))
                {
                    var notice = new MessageNotice { messageType = MessageTypeEnum.错误, showType = MessageShowType.右下角弹窗, MessageStr = "姓名不可为空" };
                    await _mediator.Publish(notice);
                        
                    return;
                }

                using (var scope = _ssf.CreateScope())
                {
                    var sp = scope.ServiceProvider;
                    var userMgr= sp.GetRequiredService<IUserManager>();
                    var res = await userMgr.CreateUserAsync(account, password, name, WorkId);
                    if (res.Success)
                    {
                        var notice = new MessageNotice { messageType = MessageTypeEnum.错误, showType = MessageShowType.右下角弹窗, MessageStr = $"成功创建用户（账号名={res.Data.Account}）" };
                        await _mediator.Publish(notice);
                         
                        CmdResetCreateUserForm.Execute(null);
                    }
                    else {
                        var notice = new MessageNotice { messageType = MessageTypeEnum.错误, showType = MessageShowType.右下角弹窗, MessageStr = $"无法创建用户（账号名={account}）！原因是={res.Message}" };
                        await _mediator.Publish(notice);
                    }
                }
            },
            o => true
        );

        CmdChangeUserPassword = new AsyncRelayCommand<object>(
            async o => {
                if (ChangeUserPasswordInput == null)
                {
                    var notice = new MessageNotice { messageType = MessageTypeEnum.错误, showType = MessageShowType.右下角弹窗, MessageStr = "请输入用户相关信息" };
                    await _mediator.Publish(notice);
                     
                }

                var account = ChangeUserPasswordInput.Account;
                var password = ChangeUserPasswordInput.Password;
                var oldpass = ChangeUserPasswordInput.OldPassword;

                if (string.IsNullOrEmpty(account))
                {
                    var notice = new MessageNotice { messageType = MessageTypeEnum.错误, showType = MessageShowType.右下角弹窗, MessageStr = "账号名不可为空" };
                    await _mediator.Publish(notice);
                    return;
                }

                if (string.IsNullOrEmpty(oldpass))
                {
                    var notice = new MessageNotice { messageType = MessageTypeEnum.错误, showType = MessageShowType.右下角弹窗, MessageStr = "密码不可为空" };
                    await _mediator.Publish(notice);
                    return;
                }

                if (string.IsNullOrEmpty(password))
                {
                    var notice = new MessageNotice { messageType = MessageTypeEnum.错误, showType = MessageShowType.右下角弹窗, MessageStr = "新密码不可为空" };
                    await _mediator.Publish(notice);
                    return;
                }

                using (var scope = _ssf.CreateScope())
                {
                    var sp = scope.ServiceProvider;
                    var userMgr = sp.GetRequiredService<IUserManager>();

                    var validateRes = await userMgr.ValidateUserAsync(account,oldpass);
                    if (!validateRes.Success)
                    {
                        var notice = new MessageNotice { messageType = MessageTypeEnum.错误, showType = MessageShowType.右下角弹窗, MessageStr = "当前密码错误！请检查！" };
                        await _mediator.Publish(notice);
                        return;
                    }

                    var res = await userMgr.ChangePasswordAsync(account, password);
                    if (res.Success)
                    {
                        var notice = new MessageNotice { messageType = MessageTypeEnum.错误, showType = MessageShowType.右下角弹窗, MessageStr = $"修改用户密码成功！（账号名={res.Data.Account}）" };
                        await _mediator.Publish(notice);
                        CmdResetChangeUserPasswordForm?.Execute(null);
                    }
                    else
                    {
                        var notice = new MessageNotice { messageType = MessageTypeEnum.错误, showType = MessageShowType.右下角弹窗, MessageStr = $"修改用户密码失败（账号名={res.Data.Account}）！原因是={res.Message}" };
                        await _mediator.Publish(notice);
                           
                    }
                }
            },
            o => true
        );

        CmdResetCreateUserForm = new RelayCommand<object>(
            o => {
                if (CreateUserInput != null)
                { 
                    var props = typeof(CreateUserPayload).GetProperties();
                    foreach (var pi in props)
                    {
                        pi.SetValue(CreateUserInput, null);
                    }
                }
            },
            o => true
        );

        CmdResetChangeUserPasswordForm = new RelayCommand<object>(
            o => {
                if (ChangeUserPasswordInput != null)
                {
                    var props = typeof(ChangeUserPasswordPayload).GetProperties();
                    foreach (var pi in props)
                    {
                        // 跳过账户名，其他都重置
                        if (pi.Name != nameof(ChangeUserPasswordInput.Account))
                        { 
                            pi.SetValue(ChangeUserPasswordInput, null);
                        }
                    }
                }
            },
            o => true
        );

        CmdLoadResources = new AsyncRelayCommand<object>(
            async o => {
                //using var scope = _ssf.CreateScope();
                //var sp = scope.ServiceProvider;
                //var resSvc= sp.GetRequiredService<ResourceService>();
                //var claims = await resSvc.LoadAllClaimsAsync();
                //var list = await resSvc.LoadAllResoucesAsync();

                //var items = list
                //    .Where(i => i.Configurable)         // 剔除不可配置的资源
                //    .Select(res => new ResouceItem(_ssf)
                //    {
                //        Id = res.Id,
                //        ClaimsAllowed = new ObservableCollection<ClaimSelection>(
                //            claims.Select(c =>
                //                new ClaimSelection
                //                {
                //                    Claim = c,
                //                    Checked = res.AllowedClaims?.Any(a => a.Type == c.Type && a.Value == c.Value) == true
                //                }
                //            )
                //        ),
                //        ShortName = res.UniqueName
                //    })  ;
                //Resources = items == null ?
                //    new ObservableCollection<ResouceItem>():
                //    new ObservableCollection<ResouceItem>( items);
            },
            o => true
        );


        CmdLoadResources.Execute(null);
    }

    private AppViewModel _appvm;
    public AppViewModel AppVM {
        get => _appvm; 
        set {
            if (_appvm != value)
            {
                _appvm = value;
                OnPropertyChanged(nameof(AppVM));
            }
        }
    }

    private CreateUserPayload _createUserInput = new CreateUserPayload();

    /// <summary>
    /// 创建新用户的输入
    /// </summary>
    public CreateUserPayload CreateUserInput { 
        get => _createUserInput;
        set
        {
            if (_createUserInput != value)
            { 
                _createUserInput = value;
                OnPropertyChanged(nameof(CreateUserInput));
            }
        }
    }

    private ChangeUserPasswordPayload _changeUserPassowrdInput;

    /// <summary>
    /// 修改用户密码的输入
    /// </summary>
    public ChangeUserPasswordPayload ChangeUserPasswordInput
    {
        get => _changeUserPassowrdInput;
        set {
            if (_changeUserPassowrdInput != value)
            {
                _changeUserPassowrdInput = value;
                OnPropertyChanged(nameof(ChangeUserPasswordInput));
            }
        }
    }


    private ObservableCollection<ResouceItem> _resouces;
    public ObservableCollection<ResouceItem> Resources{ 
        get => _resouces; 
        set { 
            if(_resouces != value)
            {
                _resouces = value;
                OnPropertyChanged(nameof(Resources));
            }
        }
    } 

    public ICommand CmdCreateUser { get; }
    public ICommand CmdResetCreateUserForm { get; }

    public ICommand CmdChangeUserPassword { get; }
    public ICommand CmdResetChangeUserPasswordForm { get; }

    public ICommand CmdLoadResources { get; }

}


#region ViewModels Definitions For TabItems
public abstract class PayloadBase : ViewModelBase
{
    private string _account;
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

    private string _pass;
    public string Password
    {
        get => _pass;
        set
        {
            if (_pass != value)
            {
                _pass = value;
                OnPropertyChanged(nameof(Password));
            }
        }
    }
}

public class ChangeUserPasswordPayload : PayloadBase
{
    private string _oldPassword;
    public string OldPassword
    {
        get => _oldPassword;
        set
        {
            if (_oldPassword != value)
            {
                _oldPassword = value;
                OnPropertyChanged(nameof(OldPassword));
            }
        }
    }
}


public class CreateUserPayload : PayloadBase
{
    private string _name;

    private string _WorkId;
    public string Name
    {
        get => _name;
        set
        {
            if (_name != value)
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
    }

    public string WorkId
    {
        get => _WorkId;
        set
        {
            if (_WorkId != value)
            {
                _WorkId = value;
                OnPropertyChanged(nameof(WorkId));
            }
        }
    }
}

public class ResouceItem : ViewModelBase
{
    private IServiceScopeFactory _ssf;
    public ResouceItem(IServiceScopeFactory ssf)
    {
        _ssf = ssf;
        CmdUpdateResource = new AsyncRelayCommand<object>(
            async id => {
                //using var scope = _ssf.CreateScope();
                //var sp = scope.ServiceProvider;
                //var resSvc = sp.GetRequiredService<ResourceService>();
                //var existedRes = await resSvc.LoadResouceAsync(Id);
                //if (!existedRes.Success)
                //{
                //    MessageBox.Show($"加载资源id={id}失败！");
                //    return;
                //}
                //var existed = existedRes.Data;
                //existed.AllowedClaims = ClaimsAllowed
                //    .Where(i => i != null && i.Checked && i.Claim != null)
                //    .Select(i => new Claim(i.Claim.Type, i.Claim.Value))
                //    .ToList();
                //await resSvc.UpdateResouceAsync(existed);
            },
            o => true
        );
    }

    private int _id;
    /// <summary>
    /// 资源id
    /// </summary>
    public int Id 
    { 
        get => _id;
        set {
            if (_id != value)
            { 
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }
    }

    private string _shortName;
    /// <summary>
    /// 资源短名
    /// </summary>
    public string ShortName { 
        get => _shortName; 
        set {
            if (_shortName != value)
            {
                _shortName = value;
                OnPropertyChanged(nameof(ShortName));
            }
        } 
    }



    /// <summary>
    /// 允许的Claim
    /// </summary>
    public ObservableCollection<ClaimSelection> ClaimsAllowed {get;set;}
    public ICommand CmdUpdateResource { get; }
}


public class ClaimSelection : ViewModelBase
{
    private Claim _claim;
    public Claim Claim
    {
        get => _claim;
        set
        {
                
            if (_claim != value)
            {
                _claim = value;
                OnPropertyChanged(nameof(Claim));
            }
        }
    }

    private bool _checked;
    public bool Checked
    {
        get => _checked;
        set
        {
            if (_checked != value)
            {
                _checked = value;
                OnPropertyChanged(nameof(ClaimSelection));
            }
        }
    }
}
#endregion