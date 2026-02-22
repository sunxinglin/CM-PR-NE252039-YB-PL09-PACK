using AsZero.Core.Services.Messages;

using Ctp0600P.Client.Apis;
using Ctp0600P.Client.Protocols;

using MediatR;

using NPOI.HSSF.Record;

using Reactive.Bindings;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Yee.Common.Library.CommonEnum;
using Yee.Entitys.AlarmMgmt;
using Yee.Entitys.DBEntity.Production;
using Yee.Entitys.DBEntity.ProductionRecords;
using Yee.Entitys.DTOS;
using Yee.Entitys.DTOS.StationTaskDataDTOS;

namespace Ctp0600P.Client.ViewModels.StationTaskViewModels
{
    public class RecordTimeViewModel : TaskViewModelBase
    {
        private readonly StationTaskDTO _StationTaskDTO;
        private readonly APIHelper _ApiHelper;
        private readonly IMediator _Mediator;

        private Base_StationTask_RecordTime RecordTimeTask => _StationTaskDTO.StationTask_RecordTime;
        public RecordTimeViewModel(StationTaskDTO stationTaskDTO, APIHelper apiHelper, IMediator mediator)
        {
            _StationTaskDTO = stationTaskDTO;
            _ApiHelper = apiHelper;
            this._Mediator = mediator;

            this.TaskName = new ReactiveProperty<string> { Value = RecordTimeTask.RecordTimeTaskName };
            this.RecordTime = new ReactiveProperty<string> { Value = "" };

            BindHistory();

           
        }

        private void BindHistory()
        {
            var histDto = App.HisTaskData2.StationTaskRecords.FirstOrDefault(f => f.StationTaskId == _StationTaskDTO.StationTaskId);
            var timeRecord = histDto?.TimeRecords;
            if (timeRecord != null)
            {
                _StationTaskDTO.HasFinish = histDto.StationTaskRecord.Status == StationTaskStatusEnum.已完成;
                this.RecordTime.Value = timeRecord.TimeValue.ToString();
            }
        }

        public async void CatchRecordTime()
        {
            try
            {
                var dto = new SaveTimeDTO()
                {
                    MainId = App.HisTaskData2.MainId,
                    StationTaskId = _StationTaskDTO.StationTaskId,
                    PackCode = App.PackBarCode,
                    RecordTimeStr = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                    StationCode = string.IsNullOrEmpty(App.CurrentStationCode) ? App._StepStationSetting.StationCode : App.CurrentStationCode,
                    TimeFlag = RecordTimeTask.TimeOutFlag,
                    UploadMesCode = RecordTimeTask.UpMesCode,
                    TaskName = RecordTimeTask.RecordTimeTaskName,
                };
                this.RecordTime.Value = dto.RecordTimeStr;
               var result = await _ApiHelper.SaveTimeRecord(dto);
                if(result.Code != 200)
                {
                    var alarm = new AlarmSYSNotification() { Code = AlarmCode.时间记录错误, Name = AlarmCode.时间记录错误.ToString(), Description = result.Message };
                    await this._Mediator.Publish(alarm);
                    return;
                }

                var newLog = new LogMessage
                {
                    Level = Microsoft.Extensions.Logging.LogLevel.Information,
                    Content = $"时间记录成功。{dto.RecordTimeStr}",
                    Timestamp = DateTime.Now
                };
                await this._Mediator.Publish(new UILogNotification { LogMessage = newLog });
                OnCompleteTask(this._StationTaskDTO);
            }
            catch (Exception ex)
            {
                var alarm = new AlarmSYSNotification() { Code = AlarmCode.时间记录错误, Name = AlarmCode.时间记录错误.ToString(), Description = ex.Message };
                await this._Mediator.Publish(alarm);
            }
        }



        public ReactiveProperty<string> TaskName { get; }

        public ReactiveProperty<string> RecordTime { get; }
    }
}
