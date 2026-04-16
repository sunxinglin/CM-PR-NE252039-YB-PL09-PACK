using AsZero.Core.Services.Auth;
using AsZero.DbContexts;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

using Xunit.Abstractions;

namespace AsZero.UnitTest;

public class AccountServiceTest
{
    private const string CONNECTION_STRING = "Server=localhost;Database=UnitTest;User ID=sa;Password=Aa123456;Trust Server Certificate=true";
    private const string SALT = "ec0d0a12-70ff-4d93-b3e6-307403c99978";
    
    private readonly ITestOutputHelper _output;
    
    public AccountServiceTest(ITestOutputHelper output)
    {
        _output = output;
    }
    
    
    [Fact]
    public void ComputeHashTest()
    {
        var hasher = new DefaultPasswordHasher();
        var actual = hasher.ComputeHash("0", SALT);
        _output.WriteLine("{0}", actual);
    }

    [Fact]
    public async Task CreateUserTest()
    {
        await using var db = await CreateDbAsync();
        var userManager = CreateUserManager(db);

        // var account = "0";
        var account = "l";
        var create = await userManager.CreateUserAsync(account, account, "测试账号0", "60000000", 0, SALT);
        // Assert.True(create.Success);
        // Assert.NotNull(create.Data);
        
        _output.WriteLine("{0}", create.Data.Account);
        _output.WriteLine("{0}", create.Data.Password);
        
        // var validate = await userManager.ValidateUserAsync(account, "0");
        // Assert.True(validate.Success);
        // Assert.NotNull(validate.Data);
        // Assert.Equal(account, validate.Data!.Account);
    }

    
    private static DefaultUserManager CreateUserManager(AsZeroDbContext db)
    {
        var hasher = new DefaultPasswordHasher();
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AppOpts:AccountCardRule"] = "6*******"
            })
            .Build();

        var provider = DataProtectionProvider.Create(new DirectoryInfo(Path.Combine(Path.GetTempPath(), "AsZero.UnitTest.DataProtection")));
        return new DefaultUserManager(db, hasher, config, provider);
    }

    private static async Task<AsZeroDbContext> CreateDbAsync()
    {
        var options = new DbContextOptionsBuilder<AsZeroDbContext>()
            .UseSqlServer(CONNECTION_STRING)
            .Options;

        var db = new AsZeroDbContext(options, new TestHostEnvironment());
        await db.Database.EnsureCreatedAsync();
        return db;
    }

    private sealed class TestHostEnvironment : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = Environments.Development;
        public string ApplicationName { get; set; } = "AsZero.UnitTest";
        public string ContentRootPath { get; set; } = AppContext.BaseDirectory;
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}
