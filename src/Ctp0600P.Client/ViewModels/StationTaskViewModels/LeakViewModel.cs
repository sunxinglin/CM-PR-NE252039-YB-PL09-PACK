using AsZero.Core.Services.Messages;
using Ctp0600P.Client.Apis;
using Ctp0600P.Client.Command.NoticeHelp.Enum;
using Ctp0600P.Client.Command.NoticeHelp.Handle;
using Ctp0600P.Client.CommonEntity;
using Ctp0600P.Client.PLC.Context;
using Ctp0600P.Client.Protocols;
using Ctp0600P.Client.Protocols.Leak.Models;
using Ctp0600P.Client.Protocols.ScanCode.Models;
using Ctp0600P.Client.UserControls;
using Ctp0600P.Client.UserControls.PLC01;
using Ctp0600P.Client.Views.StationTaskPages;
using Ctp0600P.Client.Views.Windows;
using Ctp0600P.Shared;
using DocumentFormat.OpenXml.Bibliography;
using FutureTech.Mvvm;
using MediatR;
using Microsoft.Extensions.Logging;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Yee.Common.Library;
using Yee.Common.Library.CommonEnum;
using Yee.Entitys.AlarmMgmt;
using Yee.Entitys.DTOS;
using Yee.Entitys.Production;

namespace Ctp0600P.Client.ViewModels.StationTaskViewModels
{
    public class LeakViewModel : TaskViewModelBase
    {
        private readonly APIHelper _APIHelper;
        private readonly IMediator _mediator;
        private readonly StationPLCContext _stationPLCContext;
        private readonly LeakCoreVM _leakCoreVM;
        public List<Base_StationTaskLeak> TaskLeaks { get; }
        public StationTaskDTO _StationTaskDTO { get; }
        public ReactiveProperty<LeakCoreVM> LeakCoreVM { get; }

        public ICommand ReLeak { get; }
        public ICommand CompleteConfirm { get; }

        public LeakViewModel(
            StationTaskDTO stationTaskDTO,
            APIHelper apiHelper,
            IMediator mediator,
            StationPLCContext stationPLCContext,
            LeakCoreVM leakCoreVM)
        {
            _APIHelper = apiHelper;
            _mediator = mediator;
            _StationTaskDTO = stationTaskDTO;
            _stationPLCContext = stationPLCContext;
            _leakCoreVM = leakCoreVM;
            TaskLeaks = _StationTaskDTO.Base_StationTaskLeak;
            LeakCoreVM = new ReactiveProperty<LeakCoreVM>(leakCoreVM);

            // 重新充气命令
            ReLeak = new AsyncRelayCommand<object>(
                async s =>
                {
                    try
                    {
                        await StartLeakAsync();
                    }
                    catch (Exception ex)
                    {
                        _ = _mediator.Publish(new AlarmSYSNotification
                        {
                            Code = AlarmCode.充气NG,
                            Name = AlarmCode.充气NG.ToString(),
                            Module = "充气检测",
                            PackCode = App.PackBarCode,
                            Description = $"充气失败，{ex.Message}"
                        });
                    }
                },
                o => true);

            // 充气完成确认命令 - 仅当请求充气完成=true 且 充气结果OK=true 时可执行
            // 使用 ToReactiveCommand() 确保命令状态自动响应 ReactiveProperty 的变化
            CompleteConfirm = _leakCoreVM.请求充气完成
                .CombineLatest(_leakCoreVM.充气完成确认,_leakCoreVM.充气开始请求, _leakCoreVM.开始充气, (req, ok,ksreq,ksack) => req && ok && !ksreq && !ksack)
                .ToReactiveCommand()
                .WithSubscribe(async () =>
                {
                    try
                    {
                        await CompleteLeakConfirmAsync();
                    }
                    catch (Exception ex)
                    {
                        _ = _mediator.Publish(new AlarmSYSNotification
                        {
                            Code = AlarmCode.充气NG,
                            Name = AlarmCode.充气NG.ToString(),
                            Module = "充气检测",
                            PackCode = App.PackBarCode,
                            Description = $"充气失败，{ex.Message}"
                        });
                    }
                });

            // 订阅充气完成通知
            App.LeakCompleteRequestSubject.Subscribe(CatchLeakCompleteMessage);

            // 绑定历史数据
            BindHisData();
        }

        /// <summary>
        /// 开始充气 - 添加请求到队列
        /// </summary>
        public async Task StartLeakAsync()
        {
            if (_StationTaskDTO?.Base_StationTaskLeak == null || TaskLeaks.Count == 0)
            {
                var notice = new MessageNotice
                {
                    messageType = MessageTypeEnum.提示,
                    showType = MessageShowType.右下角弹窗,
                    MessageStr = "未找到充气任务配置"
                };
                await _mediator.Publish(notice);
                return;
            }

            var taskLeak = TaskLeaks[0];

            // 添加充气请求到上下文
            _stationPLCContext.StationLeakStartReqs.Req = true;
            _stationPLCContext.StationLeakStartReqs.LeakDuration = (ushort)taskLeak.LeakTimes;
            _stationPLCContext.StationLeakStartReqs.PressureDuration = (ushort)taskLeak.KeepTimes;
            _stationPLCContext.StationLeakStartReqs.LeakStress = (ushort)taskLeak.LeakPress;
            _stationPLCContext.StationLeakStartReqs.PressureStress = (float)taskLeak.KeepPress;

        }

        /// <summary>
        /// 充气完成确认 - 验证数据并设置确认标志
        /// </summary>
        public async Task CompleteLeakConfirmAsync()
        {
            // 验证数据
            if (!await ValidateLeakDataAsync())
            {
                return;
            }

            // 保存数据
            if (!await SaveLeakDataAsync())
            {
                var notice = new MessageNotice
                {
                    messageType = MessageTypeEnum.提示,
                    showType = MessageShowType.右下角弹窗,
                    MessageStr = "保存充气数据失败"
                };
                await _mediator.Publish(notice);
                return;
            }

            // 设置确认点击标志
            //_stationPLCContext.LeakConfirmClicked = true;

            // 标记任务完成
            _StationTaskDTO.HasFinish = true;
            OnCompleteTask(_StationTaskDTO);

        }

        /// <summary>
        /// 接收PLC充气完成通知
        /// </summary>
        private void CatchLeakCompleteMessage(LeakCompleteRequest request)
        {
            if (request.QualityNG)
            {
                // 充气NG，发送报警通知
                _ = _mediator.Publish(new AlarmSYSNotification
                {
                    Code = AlarmCode.充气NG,
                    Name = AlarmCode.充气NG.ToString(),
                    Module = "充气检测",
                    PackCode = App.PackBarCode,
                    Description = $"充气结果NG，请检查设备后重新充气\r\n充气时长:{request.LeakTime}秒, 保压时长:{request.LeakKeepTime}分钟\r\n充气压力:{request.LeakPressPower}, 保压压力:{request.LeakKeepPressPower}"
                });
            }
        }

        /// <summary>
        /// 验证充气数据
        /// </summary>
        private async Task<bool> ValidateLeakDataAsync()
        {
            if (!_leakCoreVM.请求充气完成.Value)
            {
                await ShowMessageAsync("请等待充气完成");
                return false;
            }

            if (string.IsNullOrEmpty(_leakCoreVM.充气开始时间.Value))
            {
                await ShowMessageAsync("充气开始时间不正确");
                return false;
            }

            if (string.IsNullOrEmpty(_leakCoreVM.充气结束时间.Value))
            {
                await ShowMessageAsync("充气结束时间不正确");
                return false;
            }

            if (_leakCoreVM.充气时长.Value <= 0)
            {
                await ShowMessageAsync("充气时长不正确");
                return false;
            }

            if (_leakCoreVM.保压时长.Value <= 0)
            {
                await ShowMessageAsync("保压时长不正确");
                return false;
            }

            if (_leakCoreVM.充气压力.Value <= 0)
            {
                await ShowMessageAsync("充气压力不正确");
                return false;
            }

            if (_leakCoreVM.保压压力.Value <= 0)
            {
                await ShowMessageAsync("保压压力不正确");
                return false;
            }

            return true;
        }

        /// <summary>
        /// 保存充气数据
        /// </summary>
        private async Task<bool> SaveLeakDataAsync()
        {
            if (_StationTaskDTO?.Base_StationTaskLeak == null)
            {
                return false;
            }

            foreach (var info in TaskLeaks)
            {
                switch (info.ParameterName)
                {
                    case "水冷板充气时间": info.ParamValue = LeakCoreVM.Value.充气时长.Value.ToString(); break;
                    case "保压时长": info.ParamValue = LeakCoreVM.Value.保压时长.Value.ToString(); break;
                    case "水冷板充气压力": info.ParamValue = LeakCoreVM.Value.充气压力.Value.ToString(); break;
                    case "保压压力": info.ParamValue = LeakCoreVM.Value.保压压力.Value.ToString(); break;
                    case "总气压力": info.ParamValue = LeakCoreVM.Value.总气压力.Value.ToString(); break;
                    case "亚德客当前压力": info.ParamValue = LeakCoreVM.Value.亚德客当前压力.Value.ToString(); break;
                    case "比例阀当前压力": info.ParamValue = LeakCoreVM.Value.比例阀当前压力.Value.ToString(); break;
                }
                info.CreateUserID = App._RealtimePage.WorkingUserId;
                info.CreateTime = DateTime.Now;
                info.PackCode = App.PackBarCode;
                info.CreateUserID = App._RealtimePage.WorkingUserId;
                info.CreateTime = DateTime.Now;
                info.StationID = App._StepStationSetting.Station.Id;
                info.StepID = App._StepStationSetting.Step.Id;

            }

            return await App._RealtimePage.SaveLeakData(TaskLeaks, _StationTaskDTO.StationTaskId);
        }

        /// <summary>
        /// 显示消息提示
        /// </summary>
        private async Task ShowMessageAsync(string message)
        {
            var notice = new MessageNotice
            {
                messageType = MessageTypeEnum.提示,
                showType = MessageShowType.右下角弹窗,
                MessageStr = message
            };
            await _mediator.Publish(notice);
        }

        /// <summary>
        /// 绑定历史数据
        /// </summary>
        public void BindHisData()
        {
            if (App.HisTaskData2?.StationTaskRecords != null)
            {
                var histDto = App.HisTaskData2.StationTaskRecords.FirstOrDefault(f => f.StationTaskId == _StationTaskDTO.StationTaskId);
                if (histDto != null)
                {
                    _StationTaskDTO.HasFinish = histDto.StationTaskRecord.Status == StationTaskStatusEnum.已完成;
                }
            }
        }
    }
}