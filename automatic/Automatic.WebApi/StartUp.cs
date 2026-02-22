using AutomaticStation.Protocols;
using AutomaticStation.Protocols.Hubs;
using AutomaticStation.Protocols.Services;
using AutomaticStation.Shared;
using AutomaticStation.WebApi.Handlers;
using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Refit;

namespace AutomaticStation.WebApi
{
    public class StartUp
    {
        public IHostEnvironment Environment { get; }
        public IConfiguration Configuration { get; }

        public StartUp(IConfiguration configuration, IHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IServiceCollection ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            //services.AddSwaggerGen();
            services.AddMediatR(typeof(StartUp).Assembly,
                typeof(UILogNotificationHandler).Assembly,
                typeof(LowerBoxGlueNotificationHandler).Assembly
                );
            services.AddOptions<ApiServerSetting>().Bind(context.Configuration.GetSection("ApiServerSettings"));

            //services.AddCatlMesServices();
            //services.AddLogging();
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
            //.AddHttpMessageHandler<AuthHeaderHandler>();

            // 与plc交互
            services.AddDealPlcReqServices();
            services.AddPlcServices(
                context.Configuration.GetSection("PlcConnections"),
                context.Configuration.GetSection("PlcScanOpts")
            );


            services.AddSignalR().AddNewtonsoftJsonProtocol(opts => { opts.PayloadSerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore; });
            services.AddCors(option => option.AddPolicy("clientHub", policy => policy.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins("http://localhost:7223")));


            return services;
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
                endpoints.MapHub<MessageHub>("clientHub");
            });

            //if(env.IsDevelopment())
            //{
            //    app.UseSwagger();
            //    app.UseSwaggerUI();
            //}
        }
    }
}
