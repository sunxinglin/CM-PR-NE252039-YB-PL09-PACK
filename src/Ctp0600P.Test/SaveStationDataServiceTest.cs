using System.Text.Json;

using AsZero.DbContexts;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

using Xunit.Abstractions;

using Yee.Services.Production;

namespace Ctp0600P.Test;

public class SaveStationDataServiceTest
{
    private readonly ITestOutputHelper _output;

    public SaveStationDataServiceTest(ITestOutputHelper output)
    {
        _output = output;
    }

    [Theory]
    [InlineData("00123", 40, 40)]
    public async Task GetUploadCATLData_Execute(string packCode, int stepId, int stationId)
    {
        var (service, dbContext) = await TestBootstrap.CreateSaveStationDataServiceAsync();
        await using (dbContext)
        {
            var result = await service.GetUploadCATLData(packCode, stepId, stationId);
            TryWriteJson(result);
        }
    }

    private void TryWriteJson<T>(T value)
    {
        try
        {
            _output.WriteLine(JsonSerializer.Serialize(value, new JsonSerializerOptions { WriteIndented = true }));
        }
        catch (Exception ex)
        {
            _output.WriteLine($"Json serialize failed: {ex.Message}");
            _output.WriteLine(value?.ToString() ?? "<null>");
        }
    }

    private static class TestBootstrap
    {
        public static async Task<(SaveStationDataService Service, AsZeroDbContext DbContext)> CreateSaveStationDataServiceAsync()
        {
            var appSettingsPath = TryFindWebApiAppSettingsPath();
            var configuration = BuildConfiguration(appSettingsPath);
            var connectionString = ResolveConnectionString(configuration);
            var options = new DbContextOptionsBuilder<AsZeroDbContext>()
                .UseSqlServer(connectionString)
                .Options;

            var env = new TestHostEnvironment();
            var dbContext = new AsZeroDbContext(options, env);

            var canConnect = await dbContext.Database.CanConnectAsync();
            if (!canConnect)
            {
                throw new InvalidOperationException("数据库连接不可用，请检查 SQL Server 是否已启动，以及 ConnectionStrings:AsZeroDbContext 是否正确。");
            }

            var service = new SaveStationDataService(
                dbContext,
                configuration,
                stationService: null!,
                flowService: null!,
                productService: null!,
                proc_Product_OffLineService: null!,
                logger: null!,
                sysLogService: null!,
                peiFangService: null!,
                historyData_APIService: null!);

            return (service, dbContext);
        }

        private static string ResolveConnectionString(IConfiguration configuration)
        {
            var connectionString =
                Environment.GetEnvironmentVariable("ConnectionStrings__AsZeroDbContext") ??
                Environment.GetEnvironmentVariable("ASZERO_CONNECTIONSTRING") ??
                configuration.GetSection("ConnectionStrings").GetValue<string>("AsZeroDbContext");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("未找到数据库连接字符串。可设置环境变量 ConnectionStrings__AsZeroDbContext / ASZERO_CONNECTIONSTRING，或确保 AsZero.WebApi/appsettings.json 内存在 ConnectionStrings:AsZeroDbContext。");
            }

            return connectionString;
        }

        private static string? TryFindWebApiAppSettingsPath()
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir != null)
            {
                var candidate = Path.Combine(dir.FullName, "AsZero.WebApi", "appsettings.json");
                if (File.Exists(candidate))
                {
                    return candidate;
                }

                candidate = Path.Combine(dir.FullName, "src", "AsZero.WebApi", "appsettings.json");
                if (File.Exists(candidate))
                {
                    return candidate;
                }

                dir = dir.Parent;
            }

            return null;
        }

        private static IConfiguration BuildConfiguration(string? webApiAppSettingsPath)
        {
            var builder = new ConfigurationBuilder();
            if (!string.IsNullOrWhiteSpace(webApiAppSettingsPath) && File.Exists(webApiAppSettingsPath))
            {
                builder.AddJsonFile(webApiAppSettingsPath, optional: true, reloadOnChange: false);
            }

            return builder.Build();
        }

        private sealed class TestHostEnvironment : IHostEnvironment
        {
            public string EnvironmentName { get; set; } = Environments.Development;
            public string ApplicationName { get; set; } = nameof(Ctp0600P);
            public string ContentRootPath { get; set; } = AppContext.BaseDirectory;
            public IFileProvider ContentRootFileProvider { get; set; } = null!;
        }
    }
}
