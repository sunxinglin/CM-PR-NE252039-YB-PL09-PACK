using AsZero.Core.Services.Messages;
using Ctp0600P.Client.Apis;
using Ctp0600P.Client.PLC.Context;
using Ctp0600P.Client.Protocols;
using Ctp0600P.Client.ViewModels.StationTaskViewModels;
using MediatR;
using Microsoft.Extensions.Logging;
using Reactive.Bindings;
using System;
using System.Threading.Tasks;
using System.Windows;
using Yee.Common.Library.CommonEnum;
using Yee.Entitys.AlarmMgmt;
using Yee.Entitys.DTOS;

namespace Ctp0600P.Client.Views.StationTaskPages
{
    /// <summary>
    /// ScanCode.xaml 的交互逻辑
    /// </summary>
    public partial class LetGoPage : StationTaskCommonPage
    {
        public StationTaskDTO StationTaskDTO { get; }

        public TaskViewModelBase _VM;
        private readonly APIHelper _apiHepler;
        private readonly IMediator _mediator;
        private readonly StationPLCContext _stationPLCContext;


        public ReactiveProperty<bool> HasUpMesOK { get; }
        public ReactiveProperty<System.Windows.Media.Brush> TipColor { get; }
        public ReactiveCommand ReUploadMes { get; }
        public ReactiveCommand SetComplate { get; }
        public ReactiveProperty<string> Tips { get; }

        public LetGoPage(StationTaskDTO mapping, APIHelper apiHepler, IMediator mediator, StationPLCContext stationPLCContext)
        {
            InitializeComponent();
            _VM = new TaskViewModelBase();
            this._apiHepler = apiHepler;
            this._mediator = mediator;
            this._stationPLCContext = stationPLCContext;

            this.StationTaskDTO = mapping;
            HasUpMesOK = new ReactiveProperty<bool> { Value = App._StepStationSetting.IsDebug };
            this.DataContext = this;
            this.Tips = new ReactiveProperty<string> { Value = "数据上传完成，请放行小车！" };
            this.TipColor = new ReactiveProperty<System.Windows.Media.Brush> { Value = System.Windows.Media.Brushes.DarkGreen };
            ReUploadMes = new ReactiveCommand().WithSubscribe(async () =>
            {
                if (!HasUpMesOK.Value && !App._StepStationSetting.IsDebug)
                {
                    await BuildAndUpMesData();
                }
            });
            SetComplate = new ReactiveCommand().WithSubscribe(async () =>
            {
                if (!HasUpMesOK.Value)
                {
                    await mediator.Publish(new UILogNotification(new LogMessage()
                    {
                        Level = LogLevel.Warning,
                        Content = $"[放行任务]手动设置任务完成，AGV={StationTaskDTO.AGVCode}，PackCode={StationTaskDTO.PackCode}",
                        Timestamp = DateTime.Now
                    }));
                }
                HasUpMesOK.Value = true;
            });
        }

        /// <summary>
        /// 处理IO 消息
        /// </summary>
        /// <param name="request"></param>
        public async void CatchIOMessage(IOEnum io)
        {
            try
            {
                //先上传Mes
                if (!HasUpMesOK.Value && !App._StepStationSetting.IsDebug)
                {
                    await BuildAndUpMesData();
                }
                if (!HasUpMesOK.Value)
                {
                    return;
                }
                // 在第一次放行任务保存成功，但是放行AGV失败时，任务状态为已完成
                // 这时，消除报警后 再次按下放行按钮，则直接执行放行AGV，不用再次保存数据
                if (StationTaskDTO.HasFinish && App._StepStationSetting.StepType == StepTypeEnum.线内人工站)
                {
                    this._logger.LogError($"按放行尝试调用放行AGV LetGoPage");
                    App._RealtimePage._VM.DealRunAGV();
                }
                else
                {
                    if (!string.IsNullOrEmpty(App.PackBarCode))
                        _VM.OnCompleteTask(StationTaskDTO);
                }
            }
            catch (Exception ex)
            {
                await _mediator.Publish(new AlarmSYSNotification { Code = AlarmCode.系统运行错误, Name = AlarmCode.系统运行错误.ToString(), Module = AlarmModule.DESOUTTER_MODULE, Description = $"放行AGV时发生系统错误:{ex.Message}\r\n{ex.StackTrace}！" });
            }
        }

        private void Btn_RunAGV(object sender, RoutedEventArgs e)
        {
            _VM.OnCompleteTask(StationTaskDTO);
        }

        public override async void StationTaskCommonPage_Loaded(object sender, RoutedEventArgs e)
        {
            base.StationTaskCommonPage_Loaded(sender, e);
            await Task.Run(async () =>
            {
                if (!HasUpMesOK.Value && !App._StepStationSetting.IsDebug)
                {
                    await BuildAndUpMesData();
                }
            });
        }

        public async Task BuildAndUpMesData()
        {
            await App._RealtimePage.BuildUpMesCollectData();
            await this.UpMes();
        }

        public async Task UpMes()
        {
            this.Tips.Value = "当前数据上传中，请等待数据上传完成！";
            this.TipColor.Value = System.Windows.Media.Brushes.OrangeRed;
            var catlAPIResult = await App._RealtimePage.CallCatlMesCollectDataFucAsync();

            if (catlAPIResult)
            {
                this.HasUpMesOK.Value = true;
                this.Tips.Value = "数据上传完成，请放行小车！";
                this.TipColor.Value = System.Windows.Media.Brushes.DarkGreen;
                _stationPLCContext.LetGo = true;
                return;
            }

            this.HasUpMesOK.Value = false;
            this.Tips.Value = "数据上传出错，请重试！";
            this.TipColor.Value = System.Windows.Media.Brushes.Red;
        }

    }
}