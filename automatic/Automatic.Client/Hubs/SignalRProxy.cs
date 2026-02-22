using AsZero.Core.Services.Messages;
using Automatic.Protocols.Hubs;
using Automatic.Protocols.Hubs.Models.NotificationDTO;
using Automatic.Shared;
using Automatic.Protocols.LowerBoxGlue;
using Automatic.Protocols.LowerBoxGlue.Middlewares.Common.PublishNotification;
using MediatR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Serialization;
using System.Text;

namespace Automatic.Client.Hubs
{
    public class SignalRProxy
    {
        protected readonly ILogger _logger;
        protected readonly IMediator _mediator;
        protected readonly IOptionsMonitor<ApiServerSetting> _cloudSettingMonitor;
        //private readonly Subject<GluingContextNotification> _gluingContextSubject;
        private readonly DefaultContractResolver _contractResolver;
        private readonly IServiceProvider _service;

        protected HubConnection _hubConnection;
        public SignalRProxy(IServiceProvider service, ILoggerFactory loggerFactory, IMediator mediator, IOptionsMonitor<ApiServerSetting> cloudSetting
            //, Subject<GluingContextNotification> gluingcontextSubject
            )
        {
            _service = service;
            _logger = loggerFactory.CreateLogger(GetType().Name);
            _mediator = mediator;
            _cloudSettingMonitor = cloudSetting;
            //this._gluingContextSubject = gluingcontextSubject;
            _contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };
        }

        protected virtual string CreateAuthToken(ApiServerSetting setting)
        {
            var prefix = setting.ApiKeyPrefix;
            var clientId = setting.ApiKeyIdentifier;
            var pass = setting.ApiKey;
            var auth = $"{clientId}:{pass}";
            var bytes = Encoding.UTF8.GetBytes(auth);
            var token = Convert.ToBase64String(bytes);
            return prefix + token;
        }

        HubConnection CreateHubConnection()
        {
            var setting = _cloudSettingMonitor.CurrentValue;
            var baseuri = new UriBuilder(setting.BaseUrl);
            var auth = CreateAuthToken(setting);
            var huburi = new Uri(baseuri.Uri, $"/clienthub?auth={auth}");
            //var huburi = new Uri(baseuri.Uri, $"/clienthub");

            var conn = new HubConnectionBuilder()
                .WithUrl(huburi)
                .ConfigureLogging(lb =>
                {
                    lb.AddDebug();
                    lb.AddConsole();
                    lb.SetMinimumLevel(LogLevel.Debug);
                })
                .AddNewtonsoftJsonProtocol(opts =>
                {
                    opts.PayloadSerializerSettings.ContractResolver = _contractResolver;
                    opts.PayloadSerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                })
                .Build();

            BindClosedEvents(conn);
            BindHubMsgReceivedEvents(conn);
            return conn;
        }

        public async Task TryStartAsync()
        {
            try
            {
                _hubConnection = CreateHubConnection();
                await _hubConnection.StartAsync();

                var msg = new LogMessage
                {
                    Content = $"已成功连接到服务器Hub",
                    Level = LogLevel.Information,
                    Timestamp = DateTime.Now,
                };
                await _mediator.Publish(new UILogNotification(msg));

                //App.ServiceOnLineStatus = true;

                //using (var scope = this._service.CreateScope())
                //{
                //    var sp = scope.ServiceProvider;
                //    if (App._StepSetting.StepType != StepTypeEnum.自动站)
                //    {
                //        var alarm = new Alarm() { Code = AlarmCode.系统运行错误, Name = AlarmName.与服务器断开连接 };
                //        var _realtimeManualvm = sp.GetRequiredService<RealtimePageViewModel>();
                //        _realtimeManualvm.DeleteAlarm?.Execute(alarm);
                //    }
                //}

                //var alarm = new AlarmSYSNotification() { Code = AlarmCode.系统运行错误, Name = AlarmName.与服务器断开连接, Module = AlarmModule.DESOUTTER_MODULE };
                //var alarmAction = AlarmAction.Clear;
                //alarm.action = alarmAction;
                //await _mediator.Publish(alarm);

                await WhenConnected?.Invoke();
            }
            catch (Exception ex)
            {
                //var alarm = new AlarmSYSNotification() { Code = AlarmCode.系统运行错误, Name = AlarmName.与服务器断开连接, Module = AlarmModule.DESOUTTER_MODULE, Description = $"连接服务器出错:{ex.Message}\r\n{ex.StackTrace}" };
                //var alarmAction = AlarmAction.Occur;
                //alarm.action = alarmAction;
                //await _mediator.Publish(alarm);
                //App.ServiceOnLineStatus = false;

                try
                {
                    if (_hubConnection != null)
                    {
                        await _hubConnection.DisposeAsync();
                        _hubConnection = null;
                    }
                }
                catch
                {
                }
                finally
                {
                    await Task.Delay(5 * 1000);
                    await TryStartAsync();
                }
            }
        }

        public Func<Task> WhenConnected { get; set; }
        public Func<Task> WhenClosed { get; set; }


        protected virtual void BindClosedEvents(HubConnection conn)
        {
            conn.Closed += async (error) =>
            {

                var log = new LogMessage
                {
                    Content = $"和服务器连接已断开，5s后开始重连...",
                    Level = LogLevel.Warning,
                    Timestamp = DateTime.Now,
                };
                await _mediator.Publish(new UILogNotification(log));

                await WhenClosed?.Invoke();

                await Task.Delay(5 * 1000);
                await TryStartAsync();
            };
        }


        protected void BindHubMsgReceivedEvents(HubConnection conn)
        {
            conn.On<LowerBoxGlueNotificationDTO>(nameof(IMessageHubClient.ReceiveLowerBoxGlue), async dto =>
            {
                var context = new ScanContext(null, dto.DevMsg, dto.MstMsg, dto.CreatedAt);
                var notification = new ScanContextNotification(context);
                await _mediator.Publish(notification);
            });


            //conn.On<AlarmAutoNotification>(nameof(IAllMessageHubClient.ReceiveAutoAlarm), async dto =>
            //{
            //    if (App._StepStationSetting.StepType == Yee.Common.Library.CommonEnum.StepTypeEnum.自动站)
            //        await this._mediator.Publish(dto);
            //});

            //conn.On<AlarmMessage>(nameof(IAllMessageHubClient.ReceiveUILogAlarmNotification), async msg =>
            //{
            //    var notification = new UILogNotification(msg);
            //    await this._mediator.Publish(notification);
            //});

            //conn.On<LogMessage>(nameof(IAllMessageHubClient.ReceiveUILogMessageNotification), async msg =>
            //{
            //    var notification = new UILogNotification(msg);
            //    await this._mediator.Publish(notification);
            //});

            //conn.On<GluingContextNotificationDto>(nameof(IAllMessageHubClient.ReceiveGluing), async dto =>
            //{
            //    if (App._StepStationSetting.StepType == Yee.Common.Library.CommonEnum.StepTypeEnum.自动站 || App._StepStationSetting.StepType == Yee.Common.Library.CommonEnum.StepTypeEnum.模组数量监控站)
            //    {
            //        var context = new GluingContext(null, dto.devmsg, dto.MstMsg, dto.CreatedAt);
            //        var notification = new GluingContextNotification(context);
            //        await this._mediator.Publish(notification);
            //    }
            //});

            //conn.On<AutoBolt_MZContextNotificationDto>(nameof(IAllMessageHubClient.ReceiveBolt080), async dto =>
            //{
            //    var context = new AutoBolt_MZContext(null, dto.devmsg, dto.MstMsg, dto.CreatedAt);
            //    var notification = new AutoBolt_MZContextNotification(context);

            //    //if(notification.DevInfo.Core.AutoBoltToComplete)

            //    await this._mediator.Publish(notification);
            //});

            //conn.On<AutoBolt_SGContextNotificationDto>(nameof(IAllMessageHubClient.ReceiveBolt160), async dto =>
            //{
            //    if (App._StepStationSetting.StepType == Yee.Common.Library.CommonEnum.StepTypeEnum.自动站)
            //    {
            //        var context = new AutoBolt_SGContext(null, dto.devmsg, dto.MstMsg, dto.CreatedAt);
            //        var notification = new AutoBolt_SGContextNotification(context);
            //        await this._mediator.Publish(notification);
            //    }
            //});

            //conn.On<AgvMsgContextNotification>(nameof(IAllMessageHubClient.AgvActionMsg), async msg =>
            //{
            //    var notification = new AgvMsgNotification(msg);
            //    await this._mediator.Publish(notification);
            //});

            //conn.On<AutoModuleInBox1NotificationDto>(nameof(IAllMessageHubClient.ReceiveModuleInBox1), async dto =>
            //{
            //    if (App._StepStationSetting.StepType == Yee.Common.Library.CommonEnum.StepTypeEnum.自动站)
            //    {
            //        var context = new Ctp0600P.Protocols.ModuleInBox1.ModuleInBoxContext(null, dto.Devmsg, dto.MstMsg, dto.CreatedAt);
            //        var notification = new Ctp0600P.Protocols.ModuleInBox1.Middlewares.ModuleInBoxContextNotification(context);
            //        await this._mediator.Publish(notification);
            //    }
            //});

            //conn.On<AutoModuleInBox2NotificationDto>(nameof(IAllMessageHubClient.ReceiveModuleInBox2), async dto =>
            //{
            //    if (App._StepStationSetting.StepType == Yee.Common.Library.CommonEnum.StepTypeEnum.自动站)
            //    {
            //        var context = new Ctp0600P.Protocols.ModuleInBox2.ModuleInBoxContext(null, dto.Devmsg, dto.MstMsg, dto.CreatedAt);
            //        var notification = new Ctp0600P.Protocols.ModuleInBox2.Middlewares.ModuleInBoxContextNotification(context);
            //        await this._mediator.Publish(notification);
            //    }
            //});

        }
    }
}
