using AsZero.DbContexts;
using AsZero.WebApi;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MinRequestBodyDataRate = null;
    serverOptions.Limits.MinResponseDataRate = null;

    serverOptions.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(10);
    serverOptions.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(10);
});

var startup = new Startup(builder.Configuration, builder.Environment);
startup.ConfigureServices(builder.Services);
var host = builder.Build();

// ensure db exists
using (var scope = host.Services.CreateScope())
{
    var sp = scope.ServiceProvider;
    var db = sp.GetRequiredService<AsZeroDbContext>();
    db.Database.EnsureCreated();
}

// run
startup.Configure(host, host.Environment);
try
{
    host.Run();
}
catch (Exception ex)
{
    
}
