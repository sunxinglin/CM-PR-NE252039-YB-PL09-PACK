using System;
using System.Reflection;

using Ctp0600P.Client.CatlMes;
using Ctp0600P.Client.IO;
using Ctp0600P.Client.UserControls;
using Ctp0600P.Client.UserControls.AGV;
using Ctp0600P.Client.UserControls.DataCollectForResourceInspect;
using Ctp0600P.Client.UserControls.DebugTools;
using Ctp0600P.Client.ViewModels;
using Ctp0600P.Client.ViewModels.StationTaskViewModels;
using Ctp0600P.Client.Views.Pages;
using Ctp0600P.Client.Views.PopupPages;
using Ctp0600P.Client.Views.StationTaskPages;
using Ctp0600P.Client.Views.Windows;
using Ctp0600P.Shared;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Ctp0600P.Client.Views
{
    public static class ViewsServiceCollectionExtensions
    {
        public static IServiceCollection AddViews(this IServiceCollection services)
        {
            services.AddSingleton(sp =>
            {
                var vm = ActivatorUtilities.CreateInstance<AppViewModel>(sp);
                vm.AppVersion.Value = Assembly.GetExecutingAssembly().GetName().Version?.ToString();
                var appopt = sp.GetRequiredService<IOptions<AppOpt>>().Value;

                vm.AppTitle.Value = appopt.AppTitle;
                vm.EquipId.Value = appopt.EquipmentId;

                vm.MapSourceToPage = url => url switch
                {
                    UrlDefines.URL_Realtime => sp.GetRequiredService<RealtimePage>(),
                    UrlDefines.URL_Realtime_OutLine => sp.GetRequiredService<RealtimePage_OutLine>(),
                    UrlDefines.URL_History => sp.GetRequiredService<HistoryPage>(),
                    UrlDefines.URL_UserMgmt => ActivatorUtilities.CreateInstance<UseMgmtPage>(sp, ActivatorUtilities.CreateInstance<UserMgmtPageViewModel>(sp)),
                    UrlDefines.URL_ParamsMgmt => sp.GetRequiredService<ParamsMgmtPage>(),
                    UrlDefines.URL_DebugTools => sp.GetRequiredService<DebugToolsPage>(),
                    _ => throw new Exception($"未知的URL={url}"),
                };
                return vm;
            });
            services.AddSingleton<LoginPageViewModel>();
            services.AddSingleton<LoginWindow>();

            services.AddSingleton<MainWindow>();

            services.AddSingleton<RealtimePageViewModel>();
            services.AddSingleton<RealtimePage>();
            services.AddSingleton<RealtimePage_OutLine>();
            services.AddSingleton<HistoryPage>();
            services.AddSingleton<HistoryPageViewModel>();

            services.AddSingleton<ParamsMgmtPage>();
            services.AddSingleton<ParamsMgmtPageViewModel>();

            services.AddSingleton<BoltGunConfigPageVM>();
            services.AddSingleton<DebugToolsPage>();
            services.AddSingleton<DebugToolsPageViewModel>();

            services.AddSingleton<ElectricScrewDriverUserControlVM>();

            services.AddSingleton<ScanCodeGunConfigPageVM>();

            services.AddTransient<ManualBolt>();
            services.AddTransient<ManualBoltPageVM>();

            services.AddTransient<AnyLoadConfigPageVM>();

            services.AddSingleton<AGVBingPageViewModel>();
            services.AddTransient<AGVBingPage>();
            services.AddTransient<PullStepBackPage>();
            services.AddSingleton<PullStepBackPageViewModel>();


            services.AddScoped<CheckPowerPage>();
            services.AddScoped<CheckPowerViewModel>();

            services.AddSingleton<MIFindCustomAndSfcDataCtrlVM>();
            services.AddSingleton<DataCollectForMoudleTestCtrlVM>();
            services.AddSingleton<MiCheckSFCStatusExVM>();

            services.AddSingleton<MICheckBOMInventoryCtrlVM>();
            services.AddSingleton<MiAssembleAndCollectDataForSfcCtrlVM>();

            services.AddSingleton<DataCollectForResourceInspect>();
            services.AddSingleton<DataCollectForResourceInspectVM>();

            services.AddTransient<DataCollectForResourceInspectWindow>();
            services.AddTransient<DataCollectForResourceInspectWindowViewModle>();

            services.AddSingleton<DataCollectForResourceInspectListViewModle>();
            services.AddSingleton<DataCollectForResourceInspectList>();

            services.AddSingleton<IOBoxBusinessProcess>();

            services.AddScoped<ReTightenNoConfirmPage>();
            services.AddScoped<ReTightenNoConfirmViewModel>();

            return services;
        }
    }
}
