using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using Ctp0600P.Client.Command.NoticeHelp.Enum;
using Ctp0600P.Client.Command.NoticeHelp.Handle;
using Ctp0600P.Client.ViewModels;

using MediatR;

using Microsoft.Extensions.DependencyInjection;

namespace Ctp0600P.Client.Views.Pages
{
    /// <summary>
    /// Interaction logic for UseMgmtPage.xaml
    /// </summary>
    public partial class UseMgmtPage : Page
    {
        private readonly UserMgmtPageViewModel _vm;
        private readonly IServiceProvider _sp;
        private readonly AppViewModel _appvm;
        private readonly IMediator mediator;

        public UseMgmtPage(UserMgmtPageViewModel vm, IServiceProvider sp, AppViewModel appvm,IMediator mediator)
        {
            InitializeComponent();
            this.DataContext = vm;
            this._vm = vm;
            this._sp = sp;
            this._appvm = appvm;
            this.mediator = mediator;
        }

        private void BtnClick_SwitchUser(object sender, RoutedEventArgs e)
        {
            var logWin = this._sp.GetRequiredService<LoginWindow>();
            logWin.Show();
            logWin.Focus();
        }

        private void Button_SwitchUserToOperator(object sender, RoutedEventArgs e)
        {
            var mediator= this._sp.GetRequiredService<IMediator>();
            var loginvm = this._sp.GetRequiredService<LoginPageViewModel>();

            Task.Run(async () => { 
                //var user = await mediator.Send(new LoadPrincipalRequest() { Account = Defines.UserAccount_DefaultOperator });
                //UIHelper.RunInUIThread(async _ => { 
                //    await loginvm.BindPrincipalAsync(principal);
                //    this._appvm.NavigateTo(UrlDefines.URL_Realtime);
                //});
            });
        }

        private void BtnClick_ChangeUserPassword(object sender, RoutedEventArgs e)
        {
            if (this.Confirm.Password != this.NewPassword.Password)
            {
                var notice = new MessageNotice() { messageType = MessageTypeEnum.错误, showType = MessageShowType.右下角弹窗, MessageStr = $"两次输入的新密码不一致！" };
                this.mediator.Publish(notice);
               
                return;
            }

            this._vm.ChangeUserPasswordInput.OldPassword = this.OldPassword.Password;
            this._vm.ChangeUserPasswordInput.Password = this.NewPassword.Password;
            this._vm.CmdChangeUserPassword.Execute(null);
            // 重置表单
            this.OldPassword.Clear();
            this.NewPassword.Clear();
            this.Confirm.Clear();
            this._vm.CmdResetChangeUserPasswordForm.Execute(null);
        }

        private void BtnClick_CreateUser(object sender, RoutedEventArgs e)
        {
            if (this.CreatePassword.Password != this.CreatePassword2.Password)
            {
                var notice = new MessageNotice() { messageType = MessageTypeEnum.错误, showType = MessageShowType.右下角弹窗, MessageStr = $"两次输入的密码不一致！" };
                this.mediator.Publish(notice);
                return;
            }

            this._vm.CreateUserInput.Password= this.CreatePassword.Password;
            this._vm.CmdCreateUser.Execute(null);
            // 重置表单
            this.CreatePassword.Clear();
            this.CreatePassword2.Clear();
            this._vm.CmdResetCreateUserForm.Execute(null);
        }
    }
}
