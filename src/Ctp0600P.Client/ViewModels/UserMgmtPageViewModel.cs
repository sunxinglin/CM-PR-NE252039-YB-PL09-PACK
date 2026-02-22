using AsZero.Core.Services.Auth;
using Ctp0600P.Client.Command.NoticeHelp.Enum;
using Ctp0600P.Client.Command.NoticeHelp.Handle;
using Ctp0600P.Client.ViewModels;
using FutureTech.Mvvm;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Ctp0600P.Client.ViewModels
{
    public class UserMgmtPageViewModel: ViewModelBase
    {
        private readonly IPrincipalAccessor _principalAccessor;
        private readonly IServiceScopeFactory _ssf;
        private readonly IMediator mediator;

        public ClaimsPrincipal Principal { get; }

        public UserMgmtPageViewModel(IPrincipalAccessor principalAccessor, IServiceScopeFactory ssf, AppViewModel appvm,IMediator mediator)
        {
            this._principalAccessor = principalAccessor;
            this._ssf = ssf;
            AppVM = appvm;
            this.mediator = mediator;
            var principal = this._principalAccessor.GetCurrentPrincipal();
            this.Principal = principal;

            var account = principal.FindFirst(i => i.Type == ClaimTypes.NameIdentifier)?.Value;
            var name = principal.FindFirst(i => i.Type == ClaimTypes.Name)?.Value;

            // 初始化当前账户名
            this.ChangeUserPasswordInput = new ChangeUserPasswordPayload() 
            { 
                Account = account,
            };

            this.CmdCreateUser = new AsyncRelayCommand<object>(
                async o => {

                    if (this.CreateUserInput == null)
                    {
                        var notice = new MessageNotice() { messageType = MessageTypeEnum.错误, showType = MessageShowType.右下角弹窗, MessageStr = $"请输入用户相关信息" };
                        await this.mediator.Publish(notice);
                    }

                    var account = this.CreateUserInput.Account;
                    var password = this.CreateUserInput.Password;
                    var name = this.CreateUserInput.Name;
                    var WorkId = this.CreateUserInput.WorkId;
                    if (string.IsNullOrEmpty(account))
                    {
                        var notice = new MessageNotice() { messageType = MessageTypeEnum.错误, showType = MessageShowType.右下角弹窗, MessageStr = $"账号名不可为空" };
                        await this.mediator.Publish(notice);
                      
                        return;
                    }

                    if (string.IsNullOrEmpty(password))
                    {
                        var notice = new MessageNotice() { messageType = MessageTypeEnum.错误, showType = MessageShowType.右下角弹窗, MessageStr = $"密码不可为空" };
                        await this.mediator.Publish(notice);
                        
                        return;
                    }

                    if (string.IsNullOrEmpty(name))
                    {
                        var notice = new MessageNotice() { messageType = MessageTypeEnum.错误, showType = MessageShowType.右下角弹窗, MessageStr = $"姓名不可为空" };
                        await this.mediator.Publish(notice);
                        
                        return;
                    }

                    using (var scope = this._ssf.CreateScope())
                    {
                        var sp = scope.ServiceProvider;
                        var userMgr= sp.GetRequiredService<IUserManager>();
                        var res = await userMgr.CreateUserAsync(account, password, name, WorkId);
                        if (res.Success)
                        {
                            var notice = new MessageNotice() { messageType = MessageTypeEnum.错误, showType = MessageShowType.右下角弹窗, MessageStr = $"成功创建用户（账号名={res.Data.Account}）" };
                            await this.mediator.Publish(notice);
                         
                            this.CmdResetCreateUserForm.Execute(null);
                        }
                        else {
                            var notice = new MessageNotice() { messageType = MessageTypeEnum.错误, showType = MessageShowType.右下角弹窗, MessageStr = $"无法创建用户（账号名={account}）！原因是={res.Message}" };
                            await this.mediator.Publish(notice);
                        }
                    }
                },
                o => true
            );

            this.CmdChangeUserPassword = new AsyncRelayCommand<object>(
                async o => {
                    if (this.ChangeUserPasswordInput == null)
                    {
                        var notice = new MessageNotice() { messageType = MessageTypeEnum.错误, showType = MessageShowType.右下角弹窗, MessageStr = $"请输入用户相关信息" };
                        await this.mediator.Publish(notice);
                     
                    }

                    var account = this.ChangeUserPasswordInput.Account;
                    var password = this.ChangeUserPasswordInput.Password;
                    var oldpass = this.ChangeUserPasswordInput.OldPassword;

                    if (string.IsNullOrEmpty(account))
                    {
                        var notice = new MessageNotice() { messageType = MessageTypeEnum.错误, showType = MessageShowType.右下角弹窗, MessageStr = $"账号名不可为空" };
                        await this.mediator.Publish(notice);
                        return;
                    }

                    if (string.IsNullOrEmpty(oldpass))
                    {
                        var notice = new MessageNotice() { messageType = MessageTypeEnum.错误, showType = MessageShowType.右下角弹窗, MessageStr = $"密码不可为空" };
                        await this.mediator.Publish(notice);
                        return;
                    }

                    if (string.IsNullOrEmpty(password))
                    {
                        var notice = new MessageNotice() { messageType = MessageTypeEnum.错误, showType = MessageShowType.右下角弹窗, MessageStr = $"新密码不可为空" };
                        await this.mediator.Publish(notice);
                        return;
                    }

                    using (var scope = this._ssf.CreateScope())
                    {
                        var sp = scope.ServiceProvider;
                        var userMgr = sp.GetRequiredService<IUserManager>();

                        var validateRes = await userMgr.ValidateUserAsync(account,oldpass);
                        if (!validateRes.Success)
                        {
                            var notice = new MessageNotice() { messageType = MessageTypeEnum.错误, showType = MessageShowType.右下角弹窗, MessageStr = $"当前密码错误！请检查！" };
                            await this.mediator.Publish(notice);
                            return;
                        }

                        var res = await userMgr.ChangePasswordAsync(account, password);
                        if (res.Success)
                        {
                            var notice = new MessageNotice() { messageType = MessageTypeEnum.错误, showType = MessageShowType.右下角弹窗, MessageStr = $"修改用户密码成功！（账号名={res.Data.Account}）" };
                            await this.mediator.Publish(notice);
                            this.CmdResetChangeUserPasswordForm?.Execute(null);
                        }
                        else
                        {
                            var notice = new MessageNotice() { messageType = MessageTypeEnum.错误, showType = MessageShowType.右下角弹窗, MessageStr = $"修改用户密码失败（账号名={res.Data.Account}）！原因是={res.Message}" };
                            await this.mediator.Publish(notice);
                           
                        }
                    }
                },
                o => true
            );

            this.CmdResetCreateUserForm = new RelayCommand<object>(
                o => {
                    if (this.CreateUserInput != null)
                    { 
                        var props = typeof(CreateUserPayload).GetProperties();
                        foreach (var pi in props)
                        {
                            pi.SetValue(this.CreateUserInput, null);
                        }
                    }
                },
                o => true
            );

            this.CmdResetChangeUserPasswordForm = new RelayCommand<object>(
                o => {
                    if (this.ChangeUserPasswordInput != null)
                    {
                        var props = typeof(ChangeUserPasswordPayload).GetProperties();
                        foreach (var pi in props)
                        {
                            // 跳过账户名，其他都重置
                            if (pi.Name != nameof(ChangeUserPasswordInput.Account))
                            { 
                                pi.SetValue(this.ChangeUserPasswordInput, null);
                            }
                        }
                    }
                },
                o => true
            );

            this.CmdLoadResouces = new AsyncRelayCommand<object>(
                async o => {
                    //using var scope = this._ssf.CreateScope();
                    //var sp = scope.ServiceProvider;
                    //var resSvc= sp.GetRequiredService<ResourceService>();
                    //var claims = await resSvc.LoadAllClaimsAsync();
                    //var list = await resSvc.LoadAllResoucesAsync();

                    //var items = list
                    //    .Where(i => i.Configurable)         // 剔除不可配置的资源
                    //    .Select(res => new ResouceItem(this._ssf)
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
                    //this.Resources = items == null ?
                    //    new ObservableCollection<ResouceItem>():
                    //    new ObservableCollection<ResouceItem>( items);
                },
                o => true
            );


            this.CmdLoadResouces.Execute(null);
        }

        private AppViewModel _appvm;
        public AppViewModel AppVM {
            get => _appvm; 
            set {
                if (this._appvm != value)
                {
                    this._appvm = value;
                    this.OnPropertyChanged(nameof(AppVM));
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
                if (this._createUserInput != value)
                { 
                    this._createUserInput = value;
                    this.OnPropertyChanged(nameof(CreateUserInput));
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
                if (this._changeUserPassowrdInput != value)
                {
                    this._changeUserPassowrdInput = value;
                    this.OnPropertyChanged(nameof(ChangeUserPasswordInput));
                }
            }
        }


        private ObservableCollection<ResouceItem> _resouces;
        public ObservableCollection<ResouceItem> Resources{ 
            get => _resouces; 
            set { 
                if(this._resouces != value)
                {
                    this._resouces = value;
                    this.OnPropertyChanged(nameof(Resources));
                }
            }
        } 

        public ICommand CmdCreateUser { get; }
        public ICommand CmdResetCreateUserForm { get; }

        public ICommand CmdChangeUserPassword { get; }
        public ICommand CmdResetChangeUserPasswordForm { get; }

        public ICommand CmdLoadResouces { get; }

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
                if (this._account != value)
                {
                    this._account = value;
                    this.OnPropertyChanged(nameof(Account));
                }
            }
        }

        private string _pass;
        public string Password
        {
            get => _pass;
            set
            {
                if (this._pass != value)
                {
                    this._pass = value;
                    this.OnPropertyChanged(nameof(Password));
                }
            }
        }
    }

    public class ChangeUserPasswordPayload : PayloadBase
    {
        private string _oldpass;
        public string OldPassword
        {
            get => _oldpass;
            set
            {
                if (this._oldpass != value)
                {
                    this._oldpass = value;
                    this.OnPropertyChanged(nameof(OldPassword));
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
                if (this._name != value)
                {
                    this._name = value;
                    this.OnPropertyChanged(nameof(Name));
                }
            }
        }

        public string WorkId
        {
            get => _WorkId;
            set
            {
                if (this._WorkId != value)
                {
                    this._WorkId = value;
                    this.OnPropertyChanged(nameof(WorkId));
                }
            }
        }
    }

    public class ResouceItem : ViewModelBase
    {
        private IServiceScopeFactory _ssf;
        public ResouceItem(IServiceScopeFactory ssf)
        {
            this._ssf = ssf;
            this.CmdUpdateResource = new AsyncRelayCommand<object>(
                async id => {
                    //using var scope = this._ssf.CreateScope();
                    //var sp = scope.ServiceProvider;
                    //var resSvc = sp.GetRequiredService<ResourceService>();
                    //var existedRes = await resSvc.LoadResouceAsync(this.Id);
                    //if (!existedRes.Success)
                    //{
                    //    MessageBox.Show($"加载资源id={id}失败！");
                    //    return;
                    //}
                    //var existed = existedRes.Data;
                    //existed.AllowedClaims = this.ClaimsAllowed
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
                if (this._id != value)
                { 
                    this._id = value;
                    this.OnPropertyChanged(nameof(Id));
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
                if (this._shortName != value)
                {
                    this._shortName = value;
                    this.OnPropertyChanged(nameof(ShortName));
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
                
                if (this._claim != value)
                {
                    this._claim = value;
                    this.OnPropertyChanged(nameof(Claim));
                }
            }
        }

        private bool _checked;
        public bool Checked
        {
            get => _checked;
            set
            {
                if (this._checked != value)
                {
                    this._checked = value;
                    this.OnPropertyChanged(nameof(ClaimSelection));
                }
            }
        }
    }
    #endregion


}
