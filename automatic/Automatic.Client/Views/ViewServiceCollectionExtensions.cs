using Automatic.Client.ViewModels;
using Automatic.Client.ViewModels.Realtime;
using Automatic.Client.Views.Realtime;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace Automatic.Client.Views
{
    static class ViewServiceCollectionExtensions
    {
        public static IServiceCollection AddViews(this IServiceCollection services, IConfiguration config)
        {
            services.AddSingleton<AppViewModel>();
            services.AddSingleton<IScreen, AppViewModel>(sp => sp.GetRequiredService<AppViewModel>());

            services.AddSingleton<MainViewModel>();
            services.AddSingleton<IViewFor<MainViewModel>, MainWindow>();




            // 实时界面
            services.AddSingleton<UILogsViewModel>();
            services.AddSingleton<RealtimeViewModel>();
            services.AddSingleton<IViewFor<RealtimeViewModel>, RealtimeView>();
            services.AddSingleton<IViewFor<UILogsViewModel>, UILogsView>();


            services.AddSingleton<HeatingFilmPressurizeMonitorViewModel>();
            services.AddSingleton<LowerBoxGlueMonitorViewModel>();
            services.AddSingleton<ModuleInBox1MonitorViewModel>();
            services.AddSingleton<ShoulderGlueMonitorViewModel>();
            services.AddSingleton<PressureStripPressurizeMonitorViewModel>();
            services.AddSingleton<UpperCoverTighten1MonitorViewModel>();
            services.AddSingleton<UpperCoverTighten2MonitorViewModel>();

            services.AddSingleton<ModuleTightenMonitorViewModel>();
            services.AddSingleton<TerminalReshapeMonitorViewModel>();
            return services;
        }


    }
}
