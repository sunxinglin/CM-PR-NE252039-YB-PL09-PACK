using Ctp0600P.Client.Protocols;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Yee.Entitys.AlarmMgmt;
using Yee.Entitys.DTOS;

namespace HJZK.IOController
{
    public class IOBox01_BackgroundService : BackgroundService
    {
        private readonly IIOBoxApi _ioBoxApi;
        private readonly IOptionsMonitor<IOBoxConfig> _ioBoxOptions;
        private readonly ILogger<IOBox01_BackgroundService> _logger;
        private readonly IMediator _mediator;
        private readonly IServiceProvider _serviceProvider;

        private IOBoxConfig _ioBoxConfig;
        private IOMessageNotification _ioNotifaction;

        private bool _ioConAlarm;

        private bool _disConnect = false;
        public IOBox01_BackgroundService(IIOBoxApi ioBoxApi, IOMessageNotification iOMessageNotification, IOptionsMonitor<IOBoxConfig> ioBoxOptions, ILogger<IOBox01_BackgroundService> logger, IMediator mediator, IServiceProvider serviceProvider)
        {
            _ioBoxApi = ioBoxApi;
            _ioNotifaction = iOMessageNotification;
            this._ioBoxOptions = ioBoxOptions;
            _logger = logger;
            _mediator = mediator;
            _serviceProvider = serviceProvider;
            //_zlanConfig = zlanConfig;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var IO1 = _ioBoxOptions.Get("IO1");
            if (IO1.Enable)
            {
                _ioBoxConfig = IO1;
                var thread = new Thread(() =>
                {
                    _ = RunAsync(stoppingToken);
                });
                thread.IsBackground = true;
                thread.Start();
            }

            return Task.CompletedTask;
        }
        public async Task RunAsync(CancellationToken st)
        {
            while (!st.IsCancellationRequested)
            {
                try
                {
                    var Di = await _ioBoxApi.GetDIAsync(_ioBoxConfig);//读数据
                    var Do = await _ioBoxApi.GetDoAsync(_ioBoxConfig);//读数据
              
                    _ioNotifaction.Di = Di;
                    _ioNotifaction.Do = Do;
                    _ioNotifaction.IOBoxConfig = _ioBoxConfig;

                    var dic = new List<DIStatus>();
                    foreach(var item in _ioBoxConfig.DiItems)
                    {
                        dic.Add(new DIStatus { ControlName = item.ControlName, Status = Di[item.Id - 1] });
                    }
                    _ioNotifaction.DIStatus = dic;

                    await this._mediator.Publish(_ioNotifaction);
                    await _ioBoxApi.PutDoAsync(_ioBoxConfig);

                    if (_disConnect == true)
                    {
                        _disConnect = false;
                        _logger.Log(LogLevel.Information, $"Connection to IO box {_ioBoxConfig.Name} has been restored");
                    }

                    Task.Delay(300).Wait();//20ms一帧
                }
                catch (Exception ex)
                {
                    var errorMsg = ex.ToString();
                    _logger.Log(LogLevel.Error, errorMsg);

                    if (_disConnect == false && (errorMsg.Contains("连接") 
                        || errorMsg.Contains("Object") 
                        || errorMsg.Contains("调用中断") 
                        || errorMsg.Contains("Response was not of expected transaction ID")
                        || errorMsg.Contains("restored")))
                    {
                        _disConnect = true;
                    }

                    Task.Delay(500).Wait();//等待500ms重试
                }
            }

        }
    }
}
