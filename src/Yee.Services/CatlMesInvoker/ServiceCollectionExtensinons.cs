using AsZero.Core.Services;
using Catl.HostComputer.CommonServices.Actions.Loggings;
using Catl.HostComputer.CommonServices.Alarm.Loggings;
using Catl.HostComputer.CommonServices.Mes;
using Catl.HostComputer.CommonServices.Mes.Loggings;

using Ctp0600P.Shared;

using Microsoft.Extensions.DependencyInjection;
using Yee.Services.Production;
using Yee.Services.ProductionRecord;

namespace Yee.Services.CatlMesInvoker
{
    public static class ServiceCollectionExtensinons
    {
        public static IServiceCollection AddCatlWSInvokerServices(this IServiceCollection services)
        {
            services.AddSingleton<ICatlResourceProvider, DefaultCheckInveotryResourceProvider>();
            services.AddSingleton<CatlMesIniConfigHelper>();
            services.AddSingleton<ICatlMesInvoker, CatlMesInvoker>();
            //services.AddSingleton<TaskUpMESDataService>();
            
            //services.AddSingleton<ISfcInvocationLogger>();
            return services;
        }


        public static IServiceCollection AddClientCatlWSInvokerServices(this IServiceCollection services)
        {
            //services.AddSingleton<>
            return services;
        }

      

        public static void AddCatlLoggingServices(this IServiceCollection services)
        {
            #region CATL Logging
            services.AddSingleton(typeof(ISettingsManager<>), typeof(JsonFileSettingsManager<>));
            services.AddSfcLoggings();
            services.AddAlarmLoggings();
            services.AddActionLoggings();
            #endregion
        }
    }
}
