using AsZero.Core.Services.Messages;
using MediatR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Serialization;
using Ctp0600P.Shared;
using System;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Ctp0600P.Shared.NotificationDTO;
using Ctp0600P.Client.Notifications;
using Ctp0600P.Client.Protocols;
using Yee.Entitys.AlarmMgmt;
using Yee.Entitys.DTOS;
using Ctp0600P.Client;
using Yee.Common.Library.CommonEnum;
using Ctp0600P.Client.ViewModels;
using System.Linq;

namespace Ctp0600P.SignalRs
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
            this._service = service;
            this._logger = loggerFactory.CreateLogger(this.GetType().Name);
            this._mediator = mediator;
            this._cloudSettingMonitor = cloudSetting;
            //this._gluingContextSubject = gluingcontextSubject;
            this._contractResolver = new DefaultContractResolver
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
            var setting = this._cloudSettingMonitor.CurrentValue;
            var baseuri = new UriBuilder(setting.BaseUrl);
            var auth = this.CreateAuthToken(setting);
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
                    opts.PayloadSerializerSettings.ContractResolver = this._contractResolver;
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
                this._hubConnection = this.CreateHubConnection();
                await this._hubConnection.StartAsync();

                var msg = new LogMessage
                {
                    Content = $"已成功连接到服务器Hub",
                    Level = LogLevel.Information,
                    Timestamp = DateTime.Now,
                };
                await this._mediator.Publish(new UILogNotification(msg));

                App.ServiceOnLineStatus = true;

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

                var alarm = new AlarmSYSNotification() { Code = AlarmCode.系统运行错误, Name = AlarmName.与服务器断开连接, Module = AlarmModule.SERVER_MODULE };
                var alarmAction = AlarmAction.Clear;
                alarm.action = alarmAction;
                await _mediator.Publish(alarm);

                await this.WhenConnected?.Invoke();
            }
            catch (Exception ex)
            {
                var alarm = new AlarmSYSNotification() { Code = AlarmCode.系统运行错误, Name = AlarmName.与服务器断开连接, Module = AlarmModule.SERVER_MODULE, Description = $"连接服务器出错:{ex.Message}\r\n{ex.StackTrace}" };
                var alarmAction = AlarmAction.Occur;
                alarm.action = alarmAction;
                await _mediator.Publish(alarm);
                App.ServiceOnLineStatus = false;

                try
                {
                    if (this._hubConnection != null)
                    {
                        await this._hubConnection.DisposeAsync();
                        this._hubConnection = null;
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
                await this._mediator.Publish(new UILogNotification(log));

                await this.WhenClosed?.Invoke();

                await Task.Delay(5 * 1000);
                await this.TryStartAsync();
            };
        }


        protected void BindHubMsgReceivedEvents(HubConnection conn)
        {
            conn.On<AlarmMessage>(nameof(IAllMessageHubClient.ReceiveUILogAlarmNotification), async msg =>
            {
                var notification = new UILogNotification(msg);
                await this._mediator.Publish(notification);
            });

            conn.On<LogMessage>(nameof(IAllMessageHubClient.ReceiveUILogMessageNotification), async msg =>
            {
                var notification = new UILogNotification(msg);
                await this._mediator.Publish(notification);
            });

            conn.On<AgvMsgContextNotification>(nameof(IAllMessageHubClient.AgvActionMsg), async msg =>
            {
                var notification = new AgvMsgNotification(msg);
                await this._mediator.Publish(notification);
            });
        }
    }
}
