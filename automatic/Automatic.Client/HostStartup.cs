using Automatic.Client.Hubs;
using Automatic.Client.Views;
using Automatic.Protocols;
using Automatic.Protocols.Services;
using Automatic.Shared;
using MediatR;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ReactiveUI;
using Refit;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Reflection;

namespace Automatic.Client
{
    internal static class HostStartup
    {
        public static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            services.UseMicrosoftDependencyResolver();
            var resolver = Locator.CurrentMutable;
            resolver.InitializeSplat();
            resolver.InitializeReactiveUI();

            services.AddMediatR(typeof(App).Assembly);
            //services.AddMediatR(typeof(SaveImageMessageNotification).Assembly);
            //services.AddSingleton<IPrincipalAccessor, ClaimsPrincipalAccessor>();
            services.AddViews(context.Configuration);

            services.AddSingleton<SignalRProxy>();

            services.AddHttpClient();
            services.AddOptions<ApiServerSetting>()
                 .Bind(context.Configuration.GetSection("ApiServerSettings"));
            //services.AddOptions<object>()
            //    .Bind(context.Configuration.GetSection("AppSettings"));

            AddDataProtection(context, services);

            //services.AddCatlMesServices();

            services
               .AddRefitClient<IAutomaticTraceApi>(new RefitSettings
               {
                   ContentSerializer = new NewtonsoftJsonContentSerializer(new JsonSerializerSettings())
               })
               .ConfigureHttpClient((sp, c) =>
               {
                   var opts = sp.GetRequiredService<IOptions<ApiServerSetting>>();
                   var settings = opts.Value;
                   c.BaseAddress = new Uri(settings.BaseUrl);
               });

            services.AddDealPlcReqServices();

            // 与plc交互
            services.AddPlcServices(
                context.Configuration.GetSection("PlcConnections"),
                context.Configuration.GetSection("PlcScanOpts")
            );

            //services.AddOptions<object>("工位1").Bind(context.Configuration.GetSection("OffsetRangeValidation:工位1"));

            services.AddCors(option => option.AddPolicy("clientHub", policy => policy.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins("https://localhost:7223")));
        }

        private static void AddDataProtection(HostBuilderContext context, IServiceCollection services)
        {
            var keyloc = context.Configuration.GetValue<string>("PersistKeyStorageDirectory");
            var path = string.IsNullOrEmpty(keyloc) ? Directory.GetParent(Assembly.GetExecutingAssembly().Location) : new DirectoryInfo(keyloc);
            services.AddDataProtection().PersistKeysToFileSystem(path);
        }
    }
}
