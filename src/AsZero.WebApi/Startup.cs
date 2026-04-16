using AsZero.Core.Services.Auth;
using AsZero.Core.Services.HostedServices;
using AsZero.Core.Services.MessageHandlers;
using AsZero.Core.Services.Messages;
using AsZero.DbContexts;

using Ctp0600P.Shared;

using MediatR;

using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

using Newtonsoft.Json;

using Swashbuckle.AspNetCore.SwaggerUI;

using TimedTask;

using Yee.Entitys.CommonEntity;
using Yee.Services;
using Yee.Services.Auth;
using Yee.Services.CatlMesInvoker;
using Yee.WebApi.Controllers.BaseData;
using Yee.WebApi.Hubs;
using Yee.WebApi.MessageHandlers;

namespace AsZero.WebApi
{
    public class Startup
    {
        public IHostEnvironment Environment { get; }
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, IHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddMemoryCache();
            services.AddCors();
            services.AddAuth();
            services.AddYeeAuth();

            #region Interface
            services.AddLogging(logging =>
            {
                logging.AddLog4Net();
            });

            // 根据配置决定 是否 自动创建 已经用不到的数据库表
            var dbContextProfile = Configuration.GetValue<string>("DbContextProfile");
            if (string.Equals(dbContextProfile, "Manual", StringComparison.OrdinalIgnoreCase))
            {
                services.AddDbContext<AsZeroDbContext, AsZeroManualDbContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("AsZeroDbContext")));
            }
            else
            {
                services.AddDbContext<AsZeroDbContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("AsZeroDbContext")));
            }

            services.AddAsZeroHostedServices();
            services.AddMediatR(typeof(LoginRequest).Assembly);
            services.AddMediatR(typeof(LoginWithCardRequest).Assembly);
            #endregion

            #region web
            services
                .AddControllersWithViews(option =>
                {
                })
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    //options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                    options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                })
                .AddApplicationPart(typeof(PackController).Assembly)
                ;

            services.AddSignalR()
                .AddNewtonsoftJsonProtocol(opts =>
                {
                    opts.PayloadSerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                });

            // Forwarded headers
            services.Configure<ForwardedHeadersOptions>(opts =>
            {
                opts.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
                opts.KnownNetworks.Clear();
                opts.KnownProxies.Clear();
            });
            services.AddOptions<CatlMesOpt>().Bind(Configuration.GetSection("CatlMesOpt"));


            services.AddSwaggerGen();
            #endregion

            services.AddHttpClient();
            services.AddApps();

            #region Message Handlers

            services.AddMediatR(
                typeof(AgvMsgNotificationHandler).Assembly
                );

            #endregion

            services.AddClientCatlWSInvokerServices();
            services.AddCatlLoggingServices();
            services.AddCatlWSInvokerServices();
            //services.AddOptions<CatlMesOpt>().Bind(Configuration.GetSection("CatlMesOpt"));

            //services.AddPlcServices(Configuration);
            services.AddHistoryDataClearServices(Configuration);

            services.AddOptions<AppOpts>().Bind(Configuration.GetSection("AppOpts"));

            services.AddCors(option => option.AddPolicy("clientHub",
               policy => policy.AllowAnyHeader().AllowAnyMethod().AllowCredentials()
                   .WithOrigins("http://localhost:7223")));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //可以访问根目录下面的静态文件
            var staticfile = new StaticFileOptions { FileProvider = new PhysicalFileProvider(AppContext.BaseDirectory) };
            app.UseStaticFiles(staticfile);

            //app.UseCors("clientHub");

            app.UseCors(builder => builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());


            //app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseForwardedHeaders();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<AllMessageHub>("clientHub");
            });

            app.UseSwagger();


            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "V1 Docs");
                c.DocExpansion(DocExpansion.None);
            });
        }

    }
}
