using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

using Ctp0600P.Client.CommonEntity;
using Ctp0600P.Client.CommonHelper;
using Ctp0600P.Client.Notifications;
using Ctp0600P.Client.PLC.Context;
using Ctp0600P.Client.Protocols;
using Ctp0600P.Client.Protocols.BoltGun.Models;
using Ctp0600P.Client.Protocols.ScanCode.Models;
using Ctp0600P.Client.UserControls;
using Ctp0600P.Client.UserControls.AGV;
using Ctp0600P.Client.UserControls.DebugTools;
using Ctp0600P.Client.ViewModels;
using Ctp0600P.Client.ViewModels.StationTaskViewModels;
using Ctp0600P.Client.Views;
using Ctp0600P.Client.Views.Pages;
using Ctp0600P.Client.Views.StationTaskPages;
using Ctp0600P.Client.Views.Windows;
using Ctp0600P.Shared;
using Ctp0600P.SignalRs;

using FSLib.App.SimpleUpdater;

using MediatR;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using Reactive.Bindings;
using Reactive.Bindings.Schedulers;

using Yee.Common.Library.CommonEnum;
using Yee.Entitys.AlarmMgmt;
using Yee.Entitys.CATL;
using Yee.Entitys.CommonEntity;
using Yee.Entitys.DTOS;
using Yee.Services.CatlMesInvoker;

namespace Ctp0600P.Client;

public partial class App : Application
{
    #region [静态]

    private static IHostEnvironment _env;
    public static DateTime DoingWorkTime { get; set; } = DateTime.Now;
    public static StepStationSetting _StepStationSetting { get; set; }

    private static List<ProductScrewDockData> ProductScrewDockDataList { get; set; }
    private static List<ScrewConfigInfo> ProductTypeList { get; set; }
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
    private static IMediator _mediator;

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
    public static StaionHisDataDTO HisTaskData { get; internal set; } = new(); // 弃用
    public static StationTaskHistoryDTO HisTaskData2 { get; internal set; }
    public static UploadAgain UploadAgainWin { get; set; } = null!;
    public static DebugToolsPageViewModel DebugToolsPageVm { get; internal set; }

    public static int? ReworkLocateTightenByImageStationTaskId { get; set; }
    public static int? ReworkLocateTightenByImageOrderNo { get; set; }

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
        get => _AGVCode;
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
    public static AppConfigInfo _AppConfig { get; set; } = new();

    #endregion

    public IHost _host { get; private set; }
    public IServiceProvider RootServiceProvider { get; internal set; }
    public static bool ServiceOnLineStatus { get; internal set; }

    private CancellationTokenSource cts = new();
    private Mutex _singletonMutex;
    public static string CurrentStationCode { get; set; }

    public App()
    {
        var appName = typeof(App).AssemblyQualifiedName;
        _singletonMutex = new Mutex(true, appName, out var createdNew);
        // 所有的响应式属性默认都在 UI 线程更新
        ReactivePropertyScheduler.SetDefault(new ReactivePropertyWpfScheduler(Dispatcher));

#if !DEBUG_MULTI_INSTANCE
        if (!createdNew)
        {
            MessageBox.Show($"站控软件已经启动！不可重复启动！");
            Environment.Exit(0);
            return;
        }
#endif

        ProductScrewDockDataList = new List<ProductScrewDockData>();
        // 注册编码提供程序，解决中文乱码问题
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        _host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((context, builder) =>
            {
                builder
                    .SetBasePath(context.HostingEnvironment.ContentRootPath)
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true,
                        reloadOnChange: true);
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

        // 保存根服务提供者，作为全局“服务定位器”，用于在无法依赖注入的上下文（如事件处理、后台线程）中手动获取服务。
        RootServiceProvider = this._host.Services;
        var scope = RootServiceProvider.CreateScope();
        var sp = scope.ServiceProvider;
        _env = sp.GetRequiredService<IHostEnvironment>();
        var config = sp.GetRequiredService<IConfiguration>();

        _mediator = sp.GetService<IMediator>();

        // 检查客户端是否需要更新
        var updateUrl = config.GetSection("updateUrl").Get<Update>();
        if (updateUrl is { IsDebug: false })
        {
            var dd = Updater.CheckUpdateSimple("http://" + updateUrl.ServerAddress, "update_c.xml");
        }

        // 订阅设备消息流（扫码、拧紧、称重）
        ScanCodeGunRequestSubject.ToEvent().OnNext += DealScanCodeToWork;
        BoltGunRequestSubject.ToEvent().OnNext += DealBoltGunToWork;
        AnyLoadRequestSubject.ToEvent().OnNext += DealAnyLoadToWork;

        // 程序启动时，初始化PLC
        var plcContext = _host.Services.GetRequiredService<StationPLCContext>();
        if (plcContext != null)
        {
            plcContext.Initial = true;
        }
    }

    private async void Application_Startup(object sender, StartupEventArgs e)
    {
        // 设置渲染模式为软件渲染，避免 GPU 渲染问题
        RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
        try
        {
            using (var scope = RootServiceProvider.CreateScope())
            {
                var sp = scope.ServiceProvider;
                var env = sp.GetRequiredService<IHostEnvironment>();
                var config = sp.GetRequiredService<IConfiguration>();
                var logger = sp.GetRequiredService<ILogger<App>>();
                _StepStationSetting = config.GetSection("StepSetting").Get<StepStationSetting>();
                if (_StepStationSetting.ScrewLayoutEnable)
                {
                    ProductTypeList = config.GetSection("ProductTypeList").Get<List<ScrewConfigInfo>>();
                    LoadProductScrewDockData();
                }
            }

            try
            {
                var loginWindow = RootServiceProvider.GetRequiredService<LoginWindow>();
                loginWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.Shutdown();
                return;
            }

            var thread = new Thread(() =>
            {
                try
                {
                    var appVm = _host.Services.GetRequiredService<AppViewModel>();
                    var proxy = _host.Services.GetRequiredService<SignalRProxy>();
                    proxy.WhenConnected = () =>
                    {
                        appVm.HubConnected.Value = true;
                        return Task.CompletedTask;
                    };
                    proxy.WhenClosed = () =>
                    {
                        appVm.HubConnected.Value = false;
                        return Task.CompletedTask;
                    };
                    _ = proxy.TryStartAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    Shutdown();
                }
            });
            thread.IsBackground = true;
            thread.Start();
            await _host.RunAsync(cts.Token);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
            Shutdown();
        }
    }

    /// <summary>
    /// 加载补拧页面 螺丝点位布局文件
    /// </summary>
    private static void LoadProductScrewDockData()
    {
        if (ProductTypeList == null || ProductTypeList.Count == 0) return;
        ProductScrewDockDataList ??= new List<ProductScrewDockData>();
        foreach (var config in ProductTypeList)
        {
            var str = JsonHelper.ReadJson(config.JsonUrl);
            var pro = JsonConvert.DeserializeObject<ProductScrewDockData>(str);
            foreach (var mar in pro.ScrewLocationList)
            {
                mar.MarginSelf = new Thickness(mar.Margin.Left, mar.Margin.Top, mar.Margin.Right, mar.Margin.Bottom);
            }

            ProductScrewDockDataList.Add(pro);
        }
    }

    /// <summary>
    /// 从所有点位布局文件中 查找 对应产品 工位的螺丝布局数据
    /// </summary>
    /// <returns></returns>
    public static ProductScrewDockData GetProductScrewDockData()
    {
        return ProductScrewDockDataList.FirstOrDefault(p =>
            p.ProductType == _StationConfig.Product.Code && p.StepCode == _StepStationSetting.Step?.Code);
    }


    /// <summary>
    /// 程序退出
    /// </summary>
    /// <param name="e"></param>
    protected override void OnExit(ExitEventArgs e)
    {
        try
        {
            _singletonMutex.ReleaseMutex();
        }
        finally
        {
            // 停止后台服务主机
            using (_host)
            {
                var lifeTime = _host.Services.GetRequiredService<IHostApplicationLifetime>();
                lifeTime.StopApplication();
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
            if (_RealtimePage != null)
            {
                // 转交给当前的主界面ViewModel
                _RealtimePage.Vm.CatchAgvMessage(notification);
            }
        }
    }

    /// <summary>
    /// 扫码时校验条码规则
    /// </summary>
    /// <param name="rule">条码规则</param>
    /// <param name="code">实际条码</param>
    /// <returns></returns>
    public static async Task<bool> CompareCodeRuleAsync(string rule, string code)
    {
        if (string.IsNullOrEmpty(rule))
        {
            return true;
        }

        if (rule.Length != code.Length)
        {
            await PublishCompareError($"条码规则比对错误：长度不匹配，规则长度-{rule.Length},条码长度-{code.Length}");
            return false;
        }

        var z = rule.Zip(code);
        var tuple = z.Where(w => w.First != '*').ToList();
        var checkError = tuple.Any(f => f.First != f.Second);
        if (checkError)
        {
            await PublishCompareError($"条码规则比对错误：规则-{rule}，条码-{code}");
            return false;
        }

        return true;
    }

    private static async Task PublishCompareError(string msg)
    {
        await _mediator.Publish(new AlarmSYSNotification
        {
            Code = AlarmCode.条码规则比对错误,
            Name = nameof(AlarmCode.条码规则比对错误),
            Module = AlarmModule.SERVER_MODULE,
            Description = $"{msg}"
        });
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        this.DispatcherUnhandledException += App_UnHandledException;
    }

    /// <summary>
    /// 捕获主 UI 线程上抛出且未被 try-catch 块捕获的异常
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void App_UnHandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        var alarm = new AlarmSYSNotification()
        {
            Code = AlarmCode.系统运行错误,
            Name = nameof(AlarmCode.系统运行错误),
            Module = AlarmModule.SERVER_MODULE,
            Description = e.Exception.Message + Environment.NewLine + e.Exception.StackTrace,
        };
        var mediator = RootServiceProvider.GetRequiredService<IMediator>();
        await mediator.Publish(alarm);
        e.Handled = true;
    }

    private void DealScanCodeToWork(ScanCodeGunRequest request)
    {
        if (ActivePage == null || _RealtimePage == null)
        {
            return;
        }

        if (ActivePage.GetType() == typeof(ScanCode))
        {
            ((ScanCode)ActivePage)._VM.CatchScanCodeMessage(request);
            return;
        }

        if (ActivePage.GetType() == typeof(ScanCollect))
        {
            ((ScanCollect)ActivePage)._VM.CatchUserScanMessage(request);
            return;
        }

        if (ActivePage.GetType() == typeof(ScanAccountCard))
        {
            ((ScanAccountCard)ActivePage)._VM.CatchScanCodeMessage(request);
            return;
        }
    }

    private void DealBoltGunToWork(BoltGunRequest request)
    {
        if (ActivePage == null || _RealtimePage == null)
        {
            return;
        }

        if (ActivePage.GetType() == typeof(BoltGun))
        {
            ((BoltGun)ActivePage)._VM.CatchBoltGunMessage(request);
            return;
        }

        if (ActivePage.GetType() == typeof(RepairBoltGunCommon))
        {
            ((RepairBoltGunCommon)ActivePage)._VM.CatchBoltGunMessage(request);
            return;
        }

        if (ActivePage is TightenByImage tightenByImagePage)
        {
            tightenByImagePage._vm.CatchBoltGunMessage(request);
            return;
        }
    }

    private void DealAnyLoadToWork(AnyLoadRequest request)
    {
        if (ActivePage == null)
        {
            return;
        }

        if (ActivePage.GetType() == typeof(AnyLoad))
        {
            ((AnyLoad)ActivePage)._VM.CatchAnyLoadMessage(request);
            return;
        }
    }
    public static async Task<bool> CompareAndSavelastTighteningIDFromJsonFile(int _DevicesNo, long _TighteningID, string deviceNo)
        {
            try
            {
                var root = _env?.ContentRootPath;
                if (string.IsNullOrWhiteSpace(root))
                {
                    root = AppDomain.CurrentDomain.BaseDirectory;
                }
                var path = Path.Combine(root, $"TighteningID_{deviceNo}.json");
                if (!File.Exists(path))
                {
                    var TighteningID = new List<BoltGunViewModel.LastTighteningID>
                    {
                        new BoltGunViewModel.LastTighteningID
                        {
                            devicesNo = _DevicesNo,
                            TighteningID = _TighteningID,
                        }
                    };
                    await File.WriteAllTextAsync(path, JsonConvert.SerializeObject(TighteningID), Encoding.UTF8);
                    return true;
                }

                var str = await File.ReadAllTextAsync(path);
                if (string.IsNullOrEmpty(str))
                {
                    var TighteningID = new List<BoltGunViewModel.LastTighteningID>
                    {
                        new BoltGunViewModel.LastTighteningID
                        {
                            devicesNo = _DevicesNo,
                            TighteningID = _TighteningID,
                        }
                    };
                    await File.WriteAllTextAsync(path, JsonConvert.SerializeObject(TighteningID), Encoding.UTF8);

                    return true;
                }
                var LastTighteningIDs = JsonConvert.DeserializeObject<IList<BoltGunViewModel.LastTighteningID>>(str);

                var nowDevice = LastTighteningIDs.FirstOrDefault(f => f.devicesNo == _DevicesNo);
                if (nowDevice == null)
                {
                    LastTighteningIDs.Add(new BoltGunViewModel.LastTighteningID
                    {
                        devicesNo = _DevicesNo,
                        TighteningID = _TighteningID,
                    });

                    await File.WriteAllTextAsync(path, JsonConvert.SerializeObject(LastTighteningIDs), Encoding.UTF8);
                    return true;
                }

                if (nowDevice.TighteningID == _TighteningID)
                {
                    return false;
                }

                nowDevice.TighteningID = _TighteningID;
                await File.WriteAllTextAsync(path, JsonConvert.SerializeObject(LastTighteningIDs), Encoding.UTF8);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static Task UploadReverseSingleAsync(string dcGroup, string upMesCode, int order, decimal finalTorque, decimal finalAngle)
        {
            var app = (App)Current;
            return app.UploadReverseSingle(dcGroup, upMesCode, order, finalTorque, finalAngle);
        }

        public async Task UploadReverseSingle(string dcGroup, string upMesCode, int order, decimal finalTorque, decimal finalAngle)
        {
            using var scope = RootServiceProvider.CreateScope();
            var sp = scope.ServiceProvider;
            var mesInvoker = sp.GetRequiredService<ICatlMesInvoker>();
            var value = new List<DcParamValue>
            {
                new DcParamValue { DataType = ValueTypeEnum.NUMBER, ParamValue = finalTorque.ToString(), UpMesCode = $"FZ{upMesCode}{order}" },
                new DcParamValue { DataType = ValueTypeEnum.NUMBER, ParamValue = finalAngle.ToString(), UpMesCode = $"FZ{upMesCode}JD{order}" },
                new DcParamValue { DataType = ValueTypeEnum.TEXT, ParamValue = _RealtimePage.WorkingUserCard, UpMesCode = $"FZYGH"}
            };
            await mesInvoker.dataCollect(PackBarCode, value, dcGroup, false, CurrentStationCode);
        }

        public static Task UploadNgSingleAsync(string dcGroup, string upMesCode, int order, decimal finalTorque, decimal finalAngle)
        {
            var app = (App)Current;
            return app.UploadNgSingle(dcGroup, upMesCode, order, finalTorque, finalAngle);
        }

        public async Task UploadNgSingle(string dcGroup, string upMesCode, int order, decimal finalTorque, decimal finalAngle)
        {
            using var scope = RootServiceProvider.CreateScope();
            var sp = scope.ServiceProvider;
            var mesInvoker = sp.GetRequiredService<ICatlMesInvoker>();
            var value = new List<DcParamValue>
            {
                new DcParamValue { DataType = ValueTypeEnum.NUMBER, ParamValue = finalTorque.ToString(), UpMesCode = $"NG{upMesCode}{order}" },
                new DcParamValue { DataType = ValueTypeEnum.NUMBER, ParamValue = finalAngle.ToString(), UpMesCode = $"NG{upMesCode}JD{order}" },
            };
            await mesInvoker.dataCollect(PackBarCode, value, dcGroup, false, CurrentStationCode);
        }
        private class NGTime
        {
            public NGTime(int deviceNo, int ngTimes)
            {
                DeviceNo = deviceNo;
                NgTimes = ngTimes;
            }
            public int DeviceNo { get; set; }

            public int NgTimes { get; set; }
        }

        private static List<NGTime> NGTimes = new List<NGTime>();
        public static void AddNgTimes(int devicesNo)
        {
            var device = NGTimes.FirstOrDefault(f => f.DeviceNo == devicesNo);
            if (device == null)
            {
                device = new NGTime(devicesNo, 0);
                NGTimes.Add(device);
            }
            device.NgTimes++;
            return;
        }
        public static int GetNgTimes(int devicesNo)
        {
            var device = NGTimes.FirstOrDefault(f => f.DeviceNo == devicesNo);
            if (device == null)
            {
                device = new NGTime(devicesNo, 0);
                NGTimes.Add(device);
            }

            return device.NgTimes;
        }
        public static void ClearNgTimes(int devicesNo)
        {
            var device = NGTimes.FirstOrDefault(f => f.DeviceNo == devicesNo);
            if (device == null)
            {
                device = new NGTime(devicesNo, 0);
                NGTimes.Add(device);
            }
            device.NgTimes = 0;
            return;
        }
        public static bool CheckOverflow(int devicesNo)
        {
            var device = NGTimes.FirstOrDefault(f => f.DeviceNo == devicesNo);
            if (device == null)
            {
                device = new NGTime(devicesNo, 0);
                NGTimes.Add(device);
            }

            return device.NgTimes >= _StepStationSetting.NGUpTimes && _StepStationSetting.IsNeedAutoNC;
        }
}
