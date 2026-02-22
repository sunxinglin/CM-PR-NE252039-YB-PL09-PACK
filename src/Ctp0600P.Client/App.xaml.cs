using Ctp0600P.Client.CommonEntity;
using Ctp0600P.Client.CommonHelper;
using Ctp0600P.Client.IO;
using Ctp0600P.Client.Notifications;
using Ctp0600P.Client.PLC.Context;
using Ctp0600P.Client.Protocols;
using Ctp0600P.Client.Protocols.BoltGun.Models;
using Ctp0600P.Client.Protocols.ScanCode.Models;
using Ctp0600P.Client.UserControls;
using Ctp0600P.Client.UserControls.AGV;
using Ctp0600P.Client.UserControls.DebugTools;
using Ctp0600P.Client.ViewModels;
using Ctp0600P.Client.Views;
using Ctp0600P.Client.Views.Pages;
using Ctp0600P.Client.Views.StationTaskPages;
using Ctp0600P.Client.Views.Windows;
using Ctp0600P.Shared;
using Ctp0600P.SignalRs;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Reactive.Bindings;
using Reactive.Bindings.Schedulers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Yee.Entitys.AlarmMgmt;
using Yee.Entitys.CATL;
using Yee.Entitys.CommonEntity;
using Yee.Entitys.DTOS;

namespace Ctp0600P.Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        #region [静态]
        public static DateTime DoingWorkTime { get; set; } = DateTime.Now;
        public static StepStationSetting _StepStationSetting { get; set; }

        public static List<ProductScrewDockData> ProductScrewDockDatalist { get; set; }
        public static List<ScrewConfigInfo> ProductTypeList { get; set; }
        public static UploadCATLData UpCatlMesData { get; set; }
        public static StationConfig _StationConfig { get; set; }
        public static StationConfig _StationConfigInit { get; set; }
        public static List<StationConfig> StationConfig_List { get; set; }
        public static List<StationConfig> StationConfig_ListInit { get; set; }

        public static Page ActivePage { get; set; }

        public static PullStepBackPage _ActivityPullStepBackPage { get; set; }
        public static AGVBingPage _ActivityAGVBingPage { get; set; }

        public static StationTaskCommonPage _ActivityStationTaskCommonPage { get; set; }
        public static RealTimeCommonPage _RealtimePage { get; set; }

        public static bool BingAgvWinOpen = false;
        public static IMediator mediator;

        //扫码数据
        public static Subject<ScanCodeGunRequest> ScanCodeGunRequestSubject { get; } = new();

        //拧紧数据
        public static Subject<BoltGunRequest> BoltGunRequestSubject { get; } = new();

        //称重数据
        public static Subject<AnyLoadRequest> AnyLoadRequestSubject { get; } = new();

        public static bool StationTaskNGPause { get; set; } = false;

        public static int UserId { get; set; }
        public static string PowerUser { get; set; }
        public static bool IsDealingRunAGV { get; set; }
        public static StaionHisDataDTO HisTaskData { get; internal set; } = new StaionHisDataDTO(); //弃用
        public static StationTaskHistoryDTO HisTaskData2 { get; internal set; }
        public static UploadAgain UploadAgainWin { get; set; } = null!;
        public static DebugToolsPageViewModel DebugToolsPageVm { get; internal set; }

        public static string LastScanCode;
        public static DateTime LastScanTime = DateTime.Now;

        /// <summary>
        /// 静态属性通知
        /// </summary>
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;

        private static string _AGVCode = "";
        /// <summary>
        /// 当前AGV车号
        /// </summary>
        public static string AGVCode
        {
            get { return _AGVCode; }
            set
            {
                _AGVCode = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(AGVCode)));
            }
        }

        private static string _PackBarCode;
        public static string PackBarCode
        {
            get => _PackBarCode;
            set
            {
                if (_PackBarCode != value)
                {
                    _PackBarCode = value;
                    StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(PackBarCode)));
                }
            }
        }
        private static string _PackOutCode = "";
        public static string PackOutCode
        {
            get => _PackOutCode;
            set
            {
                if (_PackOutCode != value)
                {
                    _PackOutCode = value;
                    StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(PackOutCode)));
                }
            }
        }
        private static string _CurTaskName;
        public static string CurTaskName
        {
            get => _CurTaskName;
            set
            {
                if (_CurTaskName != value)
                {
                    _CurTaskName = value;
                    StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(CurTaskName)));
                }
            }
        }

        public static ElectricScrewDriverUserControlVM DebugTool_Screw_VM { get; internal set; }
        public static AppConfigInfo _AppConfig { get; set; } = new AppConfigInfo();

        #endregion

        public IHost _host { get; private set; }
        public IServiceProvider RootServiceProvider { get; internal set; }
        public static bool ServiceOnLineStatus { get; internal set; }

        private CancellationTokenSource cts = new();
        private Mutex _singletonMutex;
        public static string CurrentStationCode { get; set; }
        public App()
        {
            var appname = typeof(App).AssemblyQualifiedName;
            this._singletonMutex = new Mutex(true, appname, out var createdNew);
            ReactivePropertyScheduler.SetDefault(new ReactivePropertyWpfScheduler(Dispatcher));
            if (!createdNew)
            {
                MessageBox.Show($"站控软件已经启动！不可重复启动！");
                Environment.Exit(0);
                return;
            }

            ProductScrewDockDatalist = new List<ProductScrewDockData>();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            this._host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((context, builder) =>
                {
                    builder
                        .SetBasePath(context.HostingEnvironment.ContentRootPath)
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true);
                        // 如果需要外部配置，可以开启这行作为最终覆盖
                        // .AddJsonFile("D:/AppRoot/appsettings.json", optional: true, reloadOnChange: true); 
                    builder.AddEnvironmentVariables();
                })
               .ConfigureLogging((ctx, logging) =>
               {
                   if (ctx.HostingEnvironment.IsDevelopment())
                   {
                       logging.AddDebug();
                   }
                   logging.AddLog4Net();
               })
               .ConfigureServices(HostStartup.ConfigureServices)

               .Build();
            this.RootServiceProvider = this._host.Services;
            var scope = this.RootServiceProvider.CreateScope();
            var sp = scope.ServiceProvider;
            var config = sp.GetRequiredService<IConfiguration>();

            mediator = sp.GetService<IMediator>();
            var updateUrl = config.GetSection("updateUrl").Get<Update>();
            if (updateUrl != null && !updateUrl.IsDebug)
            {
                var dd = FSLib.App.SimpleUpdater.Updater.CheckUpdateSimple("http://" + updateUrl.ServerAddress, "update_c.xml");
            }

            ScanCodeGunRequestSubject.ToEvent().OnNext += DealScanCodeToWork;
            BoltGunRequestSubject.ToEvent().OnNext += DealBlotGunToWork;
            AnyLoadRequestSubject.ToEvent().OnNext += DealAnyLoadToWork;

            //程序启动时，初始化PLC。
            var plcContext = _host.Services.GetRequiredService<StationPLCContext>();
            if (plcContext != null)
            {
                plcContext.Initial = true;
            }

        }

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            RenderOptions.ProcessRenderMode = System.Windows.Interop.RenderMode.SoftwareOnly;
            try
            {
                using (var scope = this.RootServiceProvider.CreateScope())
                {
                    var sp = scope.ServiceProvider;
                    var env = sp.GetRequiredService<IHostEnvironment>();
                    var config = sp.GetRequiredService<IConfiguration>();
                    var logger = sp.GetRequiredService<ILogger<App>>();
                    App._StepStationSetting = config.GetSection("StepSetting").Get<StepStationSetting>();
                    if (App._StepStationSetting.ScrewLayoutEnable)
                    {
                        App.ProductTypeList = config.GetSection("ProductTypeList").Get<List<ScrewConfigInfo>>();
                        LoadProductScrewDockData();
                    }
                }
                try
                {
                    var loginWindow = this.RootServiceProvider.GetRequiredService<LoginWindow>();
                    loginWindow.Show();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    this.Shutdown();
                    return;
                }

                var thread = new Thread(async () =>
                {
                    try
                    {
                        var appvm = _host.Services.GetRequiredService<AppViewModel>();

                        var proxy = _host.Services.GetRequiredService<SignalRProxy>();
                        var iobusinessProcess = _host.Services.GetService<IOBoxBusinessProcess>();
                        proxy.WhenConnected = () =>
                        {
                            appvm.HubConnected.Value = true;
                            return Task.CompletedTask;
                        };
                        proxy.WhenClosed = () =>
                        {
                            appvm.HubConnected.Value = false;
                            return Task.CompletedTask;
                        };
                        _ = proxy.TryStartAsync();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        this.Shutdown();
                    }
                });
                thread.IsBackground = true;
                thread.Start();
                await _host.RunAsync(cts.Token);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.Shutdown();
            }
        }

        /// <summary>
        /// 加载补拧页面 螺丝点位布局文件
        /// </summary>
        private void LoadProductScrewDockData()
        {
            if (ProductTypeList == null || ProductTypeList.Count == 0) return;
            if (ProductScrewDockDatalist == null) ProductScrewDockDatalist = new List<ProductScrewDockData>();
            foreach (var config in ProductTypeList)
            {
                var str = JsonHelper.ReadJosn(config.JsonUrl);
                var pro = JsonConvert.DeserializeObject<ProductScrewDockData>(str);
                foreach (var mar in pro.ScrewLocationList)
                {
                    mar.MarginSelf = new Thickness(mar.Margin.Left, mar.Margin.Top, mar.Margin.Right, mar.Margin.Bottom);
                }
                ProductScrewDockDatalist.Add(pro);
            }
        }


        /// <summary>
        /// 从所有点位布局文件中 查找 对应产品 工位的螺丝布局数据
        /// </summary>
        /// <param name="productType"></param>
        /// <returns></returns>
        public static ProductScrewDockData GetProductScrewDockData()
        {
            return ProductScrewDockDatalist.FirstOrDefault(p => p.ProductType == App._StationConfig.Product.Code && p.StepCode == App._StepStationSetting.Step?.Code);
        }


        /// <summary>
        /// 程序退出时间
        /// </summary>
        /// <param name="e"></param>
        protected override async void OnExit(ExitEventArgs e)
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
                    //var desoutters = _host.Services.GetService<DesoutterMgr>();
                    //foreach (var desoutter in desoutters.DesoutterCtrls)
                    //{
                    //    var ctral = desoutter.Value;
                    //    await ctral.Disable();
                    //}
                }
                base.OnExit(e);
                Environment.Exit(0);
            }
        }


        /// <summary>
        /// 收到AGV消息
        /// </summary>
        /// <param name="notification"></param>
        public static void CatchAgvMessage(AgvMsgNotification notification)
        {
            if (notification.StationCode == _StepStationSetting.StationCode)
            {
                if (App._RealtimePage != null)
                    App._RealtimePage._VM.CatchAgvMessage(notification);
            }
        }

        public static async Task<bool> CompareCodeRuleAsync(string rule, string Code)
        {
            if (string.IsNullOrEmpty(rule))
            {
                return true;
            }
            if (rule.Length != Code.Length)
            {
                await PublishCompareError($"条码规则比对错误：长度不匹配，规则长度-{rule.Length},条码长度-{Code.Length}");
                return false;
            }
            var z = rule.Zip(Code);
            var tuple = z.Where(w => w.First != '*').ToList();
            var checkError = tuple.Any(f => f.First != f.Second);
            if (checkError)
            {
                await PublishCompareError($"条码规则比对错误：规则-{rule}，条码-{Code}");
                return false;
            }
            return true;
        }

        private static async Task PublishCompareError(string msg)
        {
            await mediator.Publish(new AlarmSYSNotification
            {
                Code = AlarmCode.条码规则比对错误,
                Name = AlarmCode.条码规则比对错误.ToString(),
                Module = AlarmModule.DESOUTTER_MODULE,
                Description = $"{msg}"
            });
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            this.DispatcherUnhandledException += App_UnHandledException;
        }
        private async void App_UnHandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var alarm = new AlarmSYSNotification()
            {
                Code = AlarmCode.系统运行错误,
                Name = AlarmCode.系统运行错误.ToString(),
                Description = e.Exception.Message + "/r/n" + e.Exception.StackTrace,
            };
            var mediator = this.RootServiceProvider.GetRequiredService<IMediator>();
            await mediator.Publish(alarm);
            e.Handled = true;
        }

        private void DealScanCodeToWork(ScanCodeGunRequest request)
        {
            if (App.ActivePage == null || App._RealtimePage == null)
            {
                return;
            }

            if (App.ActivePage.GetType() == typeof(ScanCode))
            {
                ((ScanCode)App.ActivePage)._VM.CatchScanCodeMessage(request);
                return;
            }

            if (App.ActivePage.GetType() == typeof(ScanCollect))
            {
                ((ScanCollect)App.ActivePage)._VM.CatchUserScanMessage(request);
                return;
            }

            if (App.ActivePage.GetType() == typeof(ScanAccountCard))
            {
                ((ScanAccountCard)App.ActivePage)._VM.CatchScanCodeMessage(request);
                return;
            }

        }

        private void DealBlotGunToWork(BoltGunRequest request)
        {
            if (App.ActivePage == null || App._RealtimePage == null)
            {
                return;
            }

            if (App.ActivePage.GetType() == typeof(BoltGun))
            {
                ((BoltGun)App.ActivePage)._VM.CatchBoltGunMessage(request);
                return;
            }

            if (App.ActivePage.GetType() == typeof(RepairBoltGunCommon))
            {
                ((RepairBoltGunCommon)App.ActivePage)._VM.CatchBoltGunMessage(request);
                return;
            }
        }

        private void DealAnyLoadToWork(AnyLoadRequest request)
        {
            if (App.ActivePage == null)
            {
                return;
            }

            if (App.ActivePage.GetType() == typeof(AnyLoad))
            {
                ((AnyLoad)App.ActivePage)._VM.CatchAnyLoadMessage(request);
                return;
            }
        }
    }
}
