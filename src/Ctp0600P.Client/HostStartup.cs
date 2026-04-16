using System;
using System.IO;
using System.Reflection;

using AsZero.Core.Services;
using AsZero.Core.Services.Auth;
using AsZero.Core.Services.MessageHandlers;
using AsZero.Core.Services.Repos;
using AsZero.DbContexts;

using Catl.HostComputer.CommonServices.Actions.Loggings;
using Catl.HostComputer.CommonServices.Alarm.Loggings;
using Catl.HostComputer.CommonServices.Mes.Loggings;

using Ctp0600P.Client.Apis;
using Ctp0600P.Client.Command;
using Ctp0600P.Client.MessageHandler;
using Ctp0600P.Client.ObservableMonitor;
using Ctp0600P.Client.PLC;
using Ctp0600P.Client.Protocols;
using Ctp0600P.Client.Protocols.AnyLoad_Wifi;
using Ctp0600P.Client.Protocols.AnyLoad;
using Ctp0600P.Client.Services;
using Ctp0600P.Client.UserControls.PLC01;
using Ctp0600P.Client.Views;
using Ctp0600P.Shared;
using Ctp0600P.SignalRs;

using MediatR;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;

using Refit;

using Yee.Services.CatlMesInvoker;

namespace Ctp0600P.Client;

public class HostStartup
{
    public static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
        services.AddClientApis();
        services.AddOptions<StepStationSetting>().Bind(context.Configuration.GetSection("StepSetting"));
        services.AddOptions<CatlMesOpt>().Bind(context.Configuration.GetSection("CatlMesOpt"));
        services.AddOptions<ApiServerSetting>().Bind(context.Configuration.GetSection("ApiServerSettings"));
        services.AddOptions<AGVBindConfig>().Bind(context.Configuration.GetSection("AGVBindConfig"));

        AddDataProtection(context, services);

        services.AddSingleton(typeof(ISettingsManager<>), typeof(JsonFileSettingsManager<>));

        services.AddSfcLoggings();
        services.AddActionLoggings();
        services.AddAlarmLoggings();

        services.AddDbContext<AsZeroDbContext>(options =>
            options.UseSqlServer(context.Configuration.GetConnectionString("AsZeroDbContext")));
        services.AddAsZeroRepositories();
        services.AddAuth();
        services.AddSingleton<IPrincipalAccessor, ClaimsPrincipalAccessor>();
        services.AddViews();
        services.AddHttpClient();
        services.AddRefitClient<ITraceApi>(new RefitSettings
        {
            ContentSerializer = new NewtonsoftJsonContentSerializer(new JsonSerializerSettings()),
        }).ConfigureHttpClient((sp, config) =>
        {
            var opts = sp.GetRequiredService<IOptions<ApiServerSetting>>();
            var settings = opts.Value;
            config.BaseAddress = new Uri(settings.BaseUrl);
        });

        #region MediatR Message Handlers
        services.AddMediatR(
            typeof(LoginMessageHandler).Assembly,
            typeof(LoginWithCardMessageHandler).Assembly,
            typeof(ScanCodeGunMessageHandler).Assembly,
            typeof(MessageBoxNotificationHandler).Assembly,
            typeof(AnyLoadMessageHandler).Assembly
        );
        #endregion

        services.AddCatlWSInvokerServices();

        services.AddSingleton<SignalRProxy>();
        services.AddCommand();

        //扫码枪
        services.AddScanCodeServices();
        //称重仪
        services.AddAnyLoadServices();
        //称重仪（Wifi）
        services.AddAnyLoadWifiServices();

        //人工位PLC
        services.AddManualStationPLCServices(context.Configuration.GetSection("PlcConnections"),
            context.Configuration.GetSection("PlcScanOpts"));
        services.AddSingleton<PLC01MonitorViewModel>();
        services.AddSingleton<PLC01ObservableMonitor>();

        services.AddSingleton<IAPIHelper, APIHelper>();

        //CORS
        services.AddCors(option =>
        {
            option.AddPolicy("clientHub",
                policy => policy.AllowAnyHeader().AllowAnyMethod().AllowCredentials()
                    .WithOrigins("https://localhost:7223"));
        });
    }

    private static void AddDataProtection(HostBuilderContext context, IServiceCollection services)
    {
        var keyIoc = context.Configuration.GetValue<string>("PersistKeyStorageDirectory");
        var path = string.IsNullOrEmpty(keyIoc)
            ? Directory.GetParent(Assembly.GetExecutingAssembly().Location)
            : new DirectoryInfo(keyIoc);
        services.AddDataProtection().PersistKeysToFileSystem(path);
    }
}