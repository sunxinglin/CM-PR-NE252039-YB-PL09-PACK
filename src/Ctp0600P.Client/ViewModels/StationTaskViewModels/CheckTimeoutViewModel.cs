using AsZero.Core.Services.Repos;

using Ctp0600P.Client.Apis;
using Ctp0600P.Client.Command.NoticeHelp.Enum;
using Ctp0600P.Client.Command.NoticeHelp.Handle;
using Ctp0600P.Client.Protocols;
using Ctp0600P.Client.Protocols.ScanCode.Models;

using MediatR;

using System;
using System.Linq;
using System.Text.RegularExpressions;

using Yee.Common.Library.CommonEnum;
using Yee.Entitys.AlarmMgmt;
using Yee.Entitys.DBEntity;
using Yee.Entitys.DTOS;
using Yee.Entitys.Production;

namespace Ctp0600P.Client.ViewModels.StationTaskViewModels
{
    public class CheckTimeoutViewModel : TaskViewModelBase
    {
        public Base_StationTaskCheckTimeOut TaskCheckTime { get; }
        public readonly APIHelper _APIHepler;
        private readonly IMediator mediator;

        public StationTaskDTO _StationTaskDTO { get; }

        public CheckTimeoutViewModel(StationTaskDTO stationTaskDTO, APIHelper apiHepler, IMediator mediator)
        {
            _APIHepler = apiHepler;
            this.mediator = mediator;
            _StationTaskDTO = stationTaskDTO;
            TaskCheckTime = stationTaskDTO.StationTaskCheckTimeout;
            BindHisData();
        }

        #region 【绑定历史信息】

        public void BindHisData()
        {
            //if (App.HisTaskData != null && App.HisTaskData.Proc_StationTask_Main != null)
            //{
            //    foreach (var record in App.HisTaskData.Proc_StationTask_Main.Proc_StationTask_Records)
            //    {
            //        if (record.Base_StationTaskId == _StationTaskDTO.StationTaskId)
            //        {
            //            BindHisData(record);
            //        }
            //    }
            //}

            var histDto = App.HisTaskData2.StationTaskRecords.FirstOrDefault(f => f.StationTaskId == _StationTaskDTO.StationTaskId);
            var checkTimeout = histDto?.CheckTimeouts;
            if (checkTimeout != null)
            {
                _StationTaskDTO.HasFinish = histDto.StationTaskRecord.Status == StationTaskStatusEnum.已完成;
                TaskCheckTime.HasPassed = checkTimeout.Status == StationTaskStatusEnum.已完成;

                StartTime = checkTimeout.StartTime?.ToString("yyyy-MM-dd HH:mm:ss");
                NowTime = checkTimeout.CollectTime?.ToString("yyyy-MM-dd HH:mm:ss");
                Pass = checkTimeout.Pass ? "通过" : "不通过";
                TaskCheckTime.HasFinish = true;
                TaskCheckTime.HasPassed = checkTimeout.Pass;
            }
        }

        /// <summary>
        /// 绑定历史信息
        /// </summary>
        /// <param name="record"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void BindHisData(Proc_StationTask_Record record)
        {
            try
            {
                var gluHisData = record.Proc_StationTask_GluingTime;
                if (gluHisData == null) return;
                StartTime = gluHisData.StartTime?.ToString("yyyy-MM-dd HH:mm:ss");
                NowTime = gluHisData.CollectTime?.ToString("yyyy-MM-dd HH:mm:ss");
                Pass = gluHisData.Pass ? "通过" : "不通过";
                TaskCheckTime.HasFinish = true;
                TaskCheckTime.HasPassed = gluHisData.Pass;
                //AllowTime = Math.Round(TaskCheckTime.MaxDuration, 0).ToString() ;
                RealTime = Math.Round(gluHisData.Time, 0).ToString();
            }
            catch (Exception ex)
            {
                mediator.Publish(new AlarmSYSNotification() { Code = AlarmCode.系统运行错误, Name = AlarmCode.系统运行错误.ToString(), Module = AlarmModule.DESOUTTER_MODULE, Description = $"绑定超时检测历史数据错误:{ex.Message}\r\n{ex.StackTrace}" });
            }
        }

        #endregion

        public async void CatchTimeMessage()
        {
            try
            {
                var minTime = TaskCheckTime.MinDuration;
                var maxTime = TaskCheckTime.MaxDuration;
                var timeResult = await _APIHepler.GetRecordTime(App.PackBarCode, TaskCheckTime.TimeOutFlag);
                if (timeResult.Code != 200)
                {
                    await mediator.Publish(new AlarmSYSNotification { Code = AlarmCode.超时检测报警, Name = AlarmCode.超时检测报警.ToString(), Module = AlarmModule.DESOUTTER_MODULE, Description = $"[超时检测]{timeResult.Message}" });
                    return;
                }
                var now = DateTime.Now;
                this.NowTime = now.ToString("yyyy/MM/dd HH:mm:ss");
                this.StartTime = timeResult.Result.TimeValue;

                var duration = (now - DateTime.Parse(timeResult.Result.TimeValue)).TotalMinutes;
                RealTime = ((int)duration).ToString();
                var checkResult = duration > Convert.ToDouble(minTime) && duration < Convert.ToDouble(maxTime);
                if (!checkResult)
                {
                    Pass = "不通过";
                    await mediator.Publish(new AlarmSYSNotification { Code = AlarmCode.超时检测报警, Name = AlarmCode.超时检测报警.ToString(), Module = AlarmModule.DESOUTTER_MODULE, Description = $"[超时检测] 时间间隔超出范围，范围：[{minTime}, {maxTime}]，实际时长：{duration}" });
                    return;
                }

                Pass = "通过";
                //记录超时检测时间结果
                this.TaskCheckTime.StartTime = DateTime.Parse(this.StartTime);
                this.TaskCheckTime.CheckTime = now;
                this.TaskCheckTime.HasPassed = checkResult;
                this.TaskCheckTime.RealDuration = Convert.ToDecimal(duration);

                var response = await _APIHepler.SaveCheckTimeOutData(this.TaskCheckTime, _StationTaskDTO.StationTaskId);
                if (response.Code != 200)
                {
                    await mediator.Publish(new AlarmSYSNotification { Code = AlarmCode.超时检测报警, Name = AlarmCode.超时检测报警.ToString(), Module = AlarmModule.DESOUTTER_MODULE, Description = $"[超时检测]{response.Message}" });
                    return;
                }
                OnCompleteTask(_StationTaskDTO);

            }
            catch (Exception ex)
            {
                await mediator.Publish(new AlarmSYSNotification { Code = AlarmCode.超时检测报警, Name = AlarmCode.超时检测报警.ToString(), Module = AlarmModule.DESOUTTER_MODULE, Description = $"[超时检测]{ex.Message}\r\n{ex.StackTrace}" });
            }

        }

        #region 【声明】
        private string _StartTime;
        public string StartTime
        {
            get { return _StartTime; }
            set
            {
                _StartTime = value;
                OnPropertyChanged(nameof(StartTime));
            }
        }

        private string _TaskName;
        public string TaskName
        {
            get { return _TaskName; }
            set
            {
                _TaskName = value;
                OnPropertyChanged(nameof(TaskName));
            }
        }

        private string _NowTime;
        public string NowTime
        {
            get { return _NowTime; }
            set
            {
                _NowTime = value;
                OnPropertyChanged(nameof(NowTime));
            }
        }

        private string _Pass;
        public string Pass
        {
            get { return _Pass; }
            set
            {
                _Pass = value;
                OnPropertyChanged(nameof(Pass));
            }
        }

        private string _RealTime = "0";
        public string RealTime
        {
            get { return _RealTime; }
            set
            {
                _RealTime = value + "分钟";
                OnPropertyChanged(nameof(RealTime));
            }
        }
        #endregion
    }
}
