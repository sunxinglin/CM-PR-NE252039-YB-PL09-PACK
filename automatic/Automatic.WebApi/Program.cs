using AutomaticStation.WebApi;

using Serilog;


var builder = WebApplication.CreateBuilder(args);

var startUp = new StartUp(builder.Configuration, builder.Environment);

builder.Host
    .ConfigureAppConfiguration((context, builder) =>
    {
        builder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
    })
    .UseSerilog((hostingContext, services, loggerConfiguration) =>
                    loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration)
                                       .ReadFrom.Services(services)

                )
    .ConfigureServices((context, services) =>
    {
        startUp.ConfigureServices(context, services);
    });

var app = builder.Build();
startUp.Configure(app, app.Environment);

try
{
    app.Run();
}
catch (Exception ex)
{
    app.Logger.LogError("宿主运行异常：{err}", ex.Message);
}