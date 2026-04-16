using System;
using System.Text;
using System.Threading.Tasks;

using AsZero.Core.Services.Messages;

using Ctp0600P.Client;
using Ctp0600P.Client.Notifications;
using Ctp0600P.Client.Protocols;
using Ctp0600P.Shared;
using Ctp0600P.Shared.NotificationDTO;

using MediatR;

using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Newtonsoft.Json.Serialization;

using Yee.Entitys.AlarmMgmt;
using Yee.Entitys.DTOS;

namespace Ctp0600P.SignalRs;

public class SignalRProxy
{
    private readonly ILogger _logger;
    private readonly IMediator _mediator;
    private readonly IOptionsMonitor<ApiServerSetting> _cloudSettingMonitor;
    private readonly DefaultContractResolver _contractResolver;
    private readonly IServiceProvider _service;

    private HubConnection _hubConnection;

    public SignalRProxy(IServiceProvider service, ILoggerFactory loggerFactory, IMediator mediator,
        IOptionsMonitor<ApiServerSetting> cloudSetting
    )
    {
        this._service = service;
        this._logger = loggerFactory.CreateLogger(this.GetType().Name);
        this._mediator = mediator;
        this._cloudSettingMonitor = cloudSetting;
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

    private HubConnection CreateHubConnection()
    {
        var setting = _cloudSettingMonitor.CurrentValue;
        var baseuri = new UriBuilder(setting.BaseUrl);
        var auth = this.CreateAuthToken(setting);
        var huburi = new Uri(baseuri.Uri, $"/clienthub?auth={auth}");

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

    /// <summary>
    /// 尝试连接服务器
    /// </summary>
    /// <returns></returns>
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

            App.ServiceOnLineStatus = true;

            var alarm = new AlarmSYSNotification
            {
                Code = AlarmCode.系统运行错误, 
                Name = AlarmName.与服务器断开连接, 
                Module = AlarmModule.SERVER_MODULE,
                action = AlarmAction.Clear
            };
            await _mediator.Publish(alarm);

            await WhenConnected?.Invoke();
        }
        catch (Exception ex)
        {
            var alarm = new AlarmSYSNotification
            {
                Code = AlarmCode.系统运行错误, 
                Name = AlarmName.与服务器断开连接, 
                Module = AlarmModule.SERVER_MODULE,
                action = AlarmAction.Occur,
                Description = $"连接服务器出错:{ex.Message}\r\n{ex.StackTrace}"
            };
            await _mediator.Publish(alarm);
            App.ServiceOnLineStatus = false;

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

    /// <summary>
    /// 绑定连接关闭事件
    ///     1. 监听连接关闭事件
    ///     2. 发布连接关闭通知
    ///     3. 等待5s后重新连接
    /// </summary>
    /// <param name="conn"></param>
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

    /// <summary>
    /// 绑定接收服务器消息事件
    ///     1. 监听服务端发来的事件
    ///     2. 将原始消息封装成对应的MediatR通知对象
    ///     3. 发布到MediatR总线
    /// </summary>
    /// <param name="conn"></param>
    protected void BindHubMsgReceivedEvents(HubConnection conn)
    {
        conn.On<AlarmMessage>(nameof(IAllMessageHubClient.ReceiveUILogAlarmNotification), async msg =>
        {
            var notification = new UILogNotification(msg);
            await _mediator.Publish(notification);
        });

        conn.On<LogMessage>(nameof(IAllMessageHubClient.ReceiveUILogMessageNotification), async msg =>
        {
            var notification = new UILogNotification(msg);
            await _mediator.Publish(notification);
        });

        conn.On<AgvMsgContextNotification>(nameof(IAllMessageHubClient.AgvActionMsg), async msg =>
        {
            var notification = new AgvMsgNotification(msg);
            await _mediator.Publish(notification);
        });
    }
}