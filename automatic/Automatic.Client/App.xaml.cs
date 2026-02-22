using Automatic.Client.Hubs;
using Automatic.Client.ViewModels;
using Automatic.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ReactiveUI;
using Serilog;
using Splat.Microsoft.Extensions.DependencyInjection;
using System.Text;
using System.Windows;
using System.Windows.Media;
using Log = Serilog.Log;

namespace Automatic.Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly Mutex _singletonMutex;
        public IHost? _host { get; private set; }
        public IServiceProvider? RootServiceProvider { get; internal set; }
        private CancellationTokenSource cts = new CancellationTokenSource();

        public App()
        {
            var appname = typeof(App).AssemblyQualifiedName;
            this._singletonMutex = new Mutex(true, appname, out var createdNew);
            if (!createdNew)
            {
                MessageBox.Show($"软件已经启动！不可重复启动！");
                Environment.Exit(0);
                return;
            }

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            InitHost();
            Environment.SetEnvironmentVariable("IsLevelControl", "Enable");
        }

        private void InitHost()
        {
            this._host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((context, builder) =>
                {
                    builder
                        .SetBasePath(context.HostingEnvironment.ContentRootPath)
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                    builder.AddEnvironmentVariables();
                })
                .UseSerilog((hostingContext, services, loggerConfiguration) =>
                    loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration)
                )
                .ConfigureServices(HostStartup.ConfigureServices)
                .Build();
            this.RootServiceProvider = this._host.Services;

            RootServiceProvider.UseMicrosoftDependencyResolver();

            Log.Logger = new LoggerConfiguration()
           .MinimumLevel.Information()
           .WriteTo.Console()
           .WriteTo.File("Logs//log.log", rollingInterval: RollingInterval.Hour)
           .CreateLogger();
        }

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            RenderOptions.ProcessRenderMode = System.Windows.Interop.RenderMode.SoftwareOnly;
            try
            {
                //var loginWindow = Locator.Current.GetService<IViewFor<LoginViewModel>>() as Window;
                //loginWindow.Show();

                // 按照CATL WM-S8-008：直接以默认Opeator启动（无需启动登录窗口）
                await SigninOperatorAsync(this.RootServiceProvider);

                var thread = new Thread(async () =>
                {
                    try
                    {
                        var appvm = _host.Services.GetRequiredService<AppViewModel>();
                        // do sth before running

                        var proxy = _host.Services.GetRequiredService<SignalRProxy>();
                        proxy.WhenConnected = () =>
                        {
                            //appvm.HubConected.Value = true;
                            //iobusinessProcess.MakesReset(false);
                            return Task.CompletedTask;
                        };
                        proxy.WhenClosed = () =>
                        {
                            //appvm.HubConected.Value = false;
                            return Task.CompletedTask;
                        };
                        _ = proxy.TryStartAsync();

                        await _host.RunAsync(cts.Token);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        this.Shutdown();
                    }
                });
                thread.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                using (var scope = this.RootServiceProvider.CreateScope())
                {
                    var sp = scope.ServiceProvider;
                    var logger = sp.GetRequiredService<ILogger<App>>();
                    logger.LogError($"{ex.Message}\r\n{ex.StackTrace}");
                }
                this.Shutdown();
            }
        }

        private static async Task SigninOperatorAsync(IServiceProvider sp)
        {
            //var loginvm = sp.GetRequiredService<LoginViewModel>();
            var appvm = sp.GetRequiredService<AppViewModel>();
            var apiOptsMonitor = sp.GetRequiredService<IOptionsMonitor<ApiServerSetting>>();
            var account = apiOptsMonitor.CurrentValue.DefaultOperatorAccount;
            var password = apiOptsMonitor.CurrentValue.DefaultOperatorPassword;
            //var loginResp = await loginvm.TrySwithUserAsync(account, password);

            //if (loginResp.Status)
            //{
            //    await appvm.CmdRefreshResources.Execute().ToTask();
            //}
            //else
            //{
            //    MessageBox.Show($"登录失败,请检查服务器是否启动或操作员凭据");
            //}
            //var loginWin = sp.GetRequiredService<IViewFor<LoginViewModel>>() as Window;
            var mainWin = sp.GetRequiredService<IViewFor<MainViewModel>>() as Window;
            //loginWin.Hide();
            mainWin.Show();
            appvm.NavigateTo(UrlDefines.URL_Realtime);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            try
            {
                this._singletonMutex.ReleaseMutex();
            }
            finally
            {
                using (_host)
                {
                    var lieftime = _host.Services.GetRequiredService<IHostApplicationLifetime>();
                    lieftime.StopApplication();
                }
                base.OnExit(e);
            }
        }

    }

}
