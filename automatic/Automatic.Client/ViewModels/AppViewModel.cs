using AsZero.Core.Entities;
using Automatic.Client.Opts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace Automatic.Client.ViewModels
{

    /// <summary>
    /// App ViewModel： 保存了当前用户信息和一些最基本的状态
    /// </summary>
    public class AppViewModel: ReactiveObject, IScreen
    {


        public AppViewModel(IOptions<AppSetting> appOpt)
        {
            //// CATL技术标准: 默认以操作员登录，所以每个人都有访问 《用户管理/用户登录》 的权限
            //this.CanAccessUserMgmt = true;
            //this.CmdRefreshResources = ReactiveCommand.CreateFromTask(this.RefreshResourcesImpl);

            var appsetting = appOpt.Value;

            AppTitle = appsetting.AppTitle;
            EquipId = appsetting.EquipId;
        }

        public RoutingState Router { get; } = new RoutingState();
        public Unit NavigateTo(string url)
        {
            var vm = UrlDefines.GetRoutableViewModel(url);
            Router.Navigate.Execute(vm);
            return Unit.Default;
        }

        /// <summary>
        /// 软件版本
        /// </summary>
        [Reactive]
        public string AppVersion { get; set; }

        /// <summary>
        /// 应用标题—— 和 AppName不同，这个可以动态变更
        /// </summary>
        [Reactive]
        public string AppTitle { get; set; } = "";



        /// <summary>
        /// 设备ID
        /// </summary>
        [Reactive]
        public string EquipId { get; set; } = "";

        /// <summary>
        /// 当前用户名
        /// </summary>
        [Reactive]
        public string UserName { get; set; }


        #region Claim
        [Reactive]
        public bool CanAccessUserMgmt { get; set;}
        [Reactive]
        public bool CanAccessUserMgmt_MaintainUser { get; set; }
        [Reactive]
        public bool CanAccessUserMgmt_Privilege { get; set; }
        [Reactive]
        public bool CanAccessParamsSetting { get; set; }


        //private async Task RefreshResourcesImpl()
        //{
        //    try
        //    {
        //        var reslist = await this._generalApi.LoadAllResourcesAsync();

        //        this.CanAccessUserMgmt = CanAccess(reslist.FirstOrDefault(r => r.UniqueName == "用户管理"));
        //        this.CanAccessUserMgmt_MaintainUser = CanAccess(reslist.FirstOrDefault(r => r.UniqueName == "用户维护"));
        //        this.CanAccessUserMgmt_Privilege = CanAccess(reslist.FirstOrDefault(r => r.UniqueName == "权限管理"));
        //        this.CanAccessParamsSetting = CanAccess(reslist.FirstOrDefault(r => r.UniqueName == "参数设置"));

        //        bool CanAccess(FuncResource res)
        //        {
        //            var principal = this._principalAccessor.GetCurrentPrincipal();

        //            if (res == null)
        //                return false;

        //            var allows = res.AllowedClaims;
        //            if (allows == null || allows.Count == 0)
        //                return false;

        //            var allowed = allows.Any(a => principal.FindFirst(c => c.Type == a.Type && c.Value == a.Value) != null);
        //            return allowed;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"{ex.Message}");
        //    }
        //}

        public ReactiveCommand<Unit, Unit> CmdRefreshResources { get; }

        #endregion

        #region 服务器连接状态
        [Reactive]
        public bool HubConected { get; set; }

        #endregion


    }
}
