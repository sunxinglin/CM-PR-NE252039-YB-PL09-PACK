using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using AsZero.Core.Entities;
using AsZero.Core.Services.Auth;
using Ctp0600P.Client.CommonHelper;
using Ctp0600P.Client.Protocols;
using Ctp0600P.Client.Views;
using Ctp0600P.Client.Views.Pages;
using Ctp0600P.Client.Views.Windows;
using Ctp0600P.Shared;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Reactive.Bindings;
using Yee.Entitys.AlarmMgmt;

namespace Ctp0600P.Client.ViewModels
{

    public class AppViewModel
    {
        private readonly IPrincipalAccessor _principalAccessor;
        private readonly IServiceScopeFactory _ssf;
        private readonly IMediator mediator;
        private readonly StepStationSetting _stationSetting;

        public AppViewModel(IPrincipalAccessor principalAccessor, IServiceScopeFactory ssf, IOptionsMonitor<StepStationSetting> stationSetting, IMediator mediator)
        {

            this._stationSetting = stationSetting.CurrentValue;
            this.AppVersion = new ReactiveProperty<string>();
            this.IPAddress = new ReactiveProperty<string>(ClientHelper.GetIpAddress());
            this.AppTitle = new ReactiveProperty<string>(_stationSetting.Project);
            this.StationTitle = new ReactiveProperty<string>(_stationSetting.StationCode);

            this.CurrentStation = new ReactiveProperty<string>(_stationSetting.StationCode);
            this.EquipId = new ReactiveProperty<string>();
            this.UserName = new ReactiveProperty<string>();
            this.UserId = new ReactiveProperty<int>();
            this.ClaimValue = new ReactiveProperty<string>();
            this.CurrentPage = new ReactiveProperty<Page>();
            this.CanAccessUserMgmt = new ReactiveProperty<bool>();
            this.CanAccessParamsSetting = new ReactiveProperty<bool>();
            this.CanAccessUserMgmt_MaintainUser = new ReactiveProperty<bool>();
            this.CanAccessUserMgmt_Privilege = new ReactiveProperty<bool>();
            #region 产线段连接状态
            this.HubConnected = new ReactiveProperty<bool>(false);
            #endregion

            this._principalAccessor = principalAccessor;
            this._ssf = ssf;
            this.mediator = mediator;

        }

        /// <summary>
        /// 软件版本
        /// </summary>
        public ReactiveProperty<string> AppVersion { get; set; }

        public ReactiveProperty<string> IPAddress { get; set; }

        /// <summary>
        /// 应用标题 和 AppName不同，这个可以动态变更
        /// </summary>
        public ReactiveProperty<string> AppTitle { get; set; }

        public ReactiveProperty<string> CurrentStation { get; set; }

        public ReactiveProperty<string> StationTitle { get; set; }

        /// <summary>
        /// 设备ID
        /// </summary>
        public ReactiveProperty<string> EquipId { get; set; }

        /// <summary>
        /// 当前用户名
        /// </summary>
        public ReactiveProperty<string> UserName { get; set; }
        public ReactiveProperty<int> UserId { get; set; }
        public ReactiveProperty<string> ClaimValue { get; set; }


        #region 路由
        /// <summary>
        /// 当前页面
        /// </summary>
        public ReactiveProperty<Page> CurrentPage { get; set; }

        public Func<string, Page> MapSourceToPage { get; set; }

        public void NavigateTo(string source)
        {
            try
            {
                if (this.MapSourceToPage == null)
                {
                    mediator.Publish(new AlarmSYSNotification() { Code = AlarmCode.系统运行错误, Name = AlarmCode.系统运行错误.ToString(), Module = AlarmModule.DESOUTTER_MODULE, Description = $"{nameof(MapSourceToPage)}不可为NULL！你是否忘记设置该属性了？" });
                    throw new Exception($"{nameof(MapSourceToPage)}不可为NULL！你是否忘记设置该属性了？");
                }
                var page = MapSourceToPage(source);

                if (source == UrlDefines.URL_ParamsMgmt)
                {
                    using (var scope = _ssf.CreateScope())
                    {
                        var sp = scope.ServiceProvider;
                        var wincheck = sp.GetRequiredService<CheckPowerPage>();
                        ((CheckPowerViewModel)wincheck.DataContext).Action = delegate
                        {
                            try
                            {
                                wincheck.Close();
                                this.CurrentPage.Value = page;
                            }
                            catch (Exception ex)
                            {
                                //throw;
                            }
                        };
                        ((CheckPowerViewModel)wincheck.DataContext).ModleName = "参数设置";
                        wincheck.Show();
                    }
                }
                else if (source == UrlDefines.URL_DebugTools)
                {
                    using (var scope = _ssf.CreateScope())
                    {
                        var sp = scope.ServiceProvider;
                        var wincheck = sp.GetRequiredService<CheckPowerPage>();
                        ((CheckPowerViewModel)wincheck.DataContext).Action = delegate
                        {
                            try
                            {
                                wincheck.Close();
                                this.CurrentPage.Value = page;
                            }
                            catch (Exception ex)
                            {
                                //throw;
                            }
                        };
                        ((CheckPowerViewModel)wincheck.DataContext).ModleName = "调试工具";
                        wincheck.Show();
                    }
                }
                else
                {
                    this.CurrentPage.Value = page;
                }
                if (page.GetType().FullName == "Ctp0600P.Client.Views.Pages.RealtimePage")
                    App._RealtimePage = (RealtimePage)page;
                if (page.GetType().FullName == "Ctp0600P.Client.Views.Pages.RealtimePage_OutLine")
                    App._RealtimePage = (RealtimePage_OutLine)page;
            }
            catch (Exception ex)
            {

            }
        }
        #endregion


        #region Claim

        public ReactiveProperty<bool> CanAccessUserMgmt { get; set; }
        public ReactiveProperty<bool> CanAccessUserMgmt_MaintainUser { get; set; }
        public ReactiveProperty<bool> CanAccessUserMgmt_Privilege { get; set; }
        public ReactiveProperty<bool> CanAccessParamsSetting { get; set; }


        public async Task RefreshResourcesAsync()
        {
            //using var scope = this._ssf.CreateScope();
            //var resSvc = scope.ServiceProvider.GetRequiredService<ResourceService>();
            //var reslist = await resSvc.LoadAllResoucesAsync();

            //this.CanAccessUserMgmt.Value = CanAccess(reslist.FirstOrDefault(r => r.UniqueName == "用户管理"));
            //this.CanAccessUserMgmt_MaintainUser.Value = CanAccess(reslist.FirstOrDefault(r => r.UniqueName == "用户维护"));
            //this.CanAccessUserMgmt_Privilege.Value = CanAccess(reslist.FirstOrDefault(r => r.UniqueName == "权限管理"));
            //this.CanAccessParamsSetting.Value = CanAccess(reslist.FirstOrDefault(r => r.UniqueName == "参数设置"));

            bool CanAccess(Resource res)
            {
                if (res == null)
                    return false;

                var allows = res.AllowedClaims;
                if (allows == null || allows.Count == 0)
                    return false;

                var principal = this._principalAccessor.GetCurrentPrincipal();
                var allowed = allows.Any(a => principal.FindFirst(c => c.Type == a.Type && c.Value == a.Value) != null);
                return allowed;
            }
        }

        #endregion

        #region 产线段服务器连接状态
        public ReactiveProperty<bool> HubConnected { get; }
        #endregion
    }
}
