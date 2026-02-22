using Ctp0600P.Client.Apis;
using Ctp0600P.Client.Command.NoticeHelp.Enum;
using Ctp0600P.Client.Command.NoticeHelp.Handle;
using Ctp0600P.Client.CommonEntity;
using Ctp0600P.Client.Protocols;
using Ctp0600P.Client.Protocols.AnyLoad;

using Ctp0600P.Client.Protocols.ScanCode.Models;
using Ctp0600P.Client.Views.StationTaskPages;

using FutureTech.Mvvm;
using MediatR;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Yee.Common.Library.CommonEnum;
using Yee.Entitys.AlarmMgmt;
using Yee.Entitys.DBEntity;
using Yee.Entitys.DTOS;
using Yee.Entitys.Production;

namespace Ctp0600P.Client.ViewModels.StationTaskViewModels
{
    public class ScanCollectViewModel : TaskViewModelBase
    {
        public StationTaskDTO StationTaskDTO { get; }
        public Base_StationTaskScanCollect TaskScanCollect { get; }
        private readonly APIHelper _ApiHelper;
        public ScanCollectViewModel(StationTaskDTO stationTaskDTO, IMediator mediator, APIHelper ApiHelper)
        {
            StationTaskDTO = stationTaskDTO;
            TaskScanCollect = StationTaskDTO.StationTaskScanCollect;
            this.mediator = mediator;
            this._ApiHelper = ApiHelper;
            BindHisData();
        }


        #region 绑定历史记录
        public void BindHisData()
        {
            //if (App.HisTaskData != null && App.HisTaskData.Proc_StationTask_Main != null)
            //{
            //    foreach (var record in App.HisTaskData.Proc_StationTask_Main.Proc_StationTask_Records)
            //    {
            //        if (record.Base_StationTaskId == StationTaskDTO.StationTaskId)
            //        {
            //            BindHisData(record);
            //        }
            //    }
            //}

            var histDto = App.HisTaskData2.StationTaskRecords.FirstOrDefault(f => f.StationTaskId == StationTaskDTO.StationTaskId);
            var sacnCollect = histDto?.ScanCollects;
            if (sacnCollect != null)
            {
                StationTaskDTO.HasFinish = histDto.StationTaskRecord.Status == StationTaskStatusEnum.已完成;
                TaskScanCollect.HasPassed = sacnCollect.Status == StationTaskStatusEnum.已完成;
                TaskScanCollect.Status = sacnCollect.Status.ToString();
                ScanData = sacnCollect.ScanCollectData;
            }
        }
        /// <summary>
        /// 绑定历史信息
        /// </summary>
        /// <param name="record"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void BindHisData(Proc_StationTask_Record record)
        {
            var hisRecord = record.Proc_StationTask_ScanCollect;
            if (hisRecord == null) return;
            StationTaskDTO.HasFinish = record.Status == StationTaskStatusEnum.已完成;
            TaskScanCollect.HasPassed = hisRecord.Status == StationTaskStatusEnum.已完成;
            TaskScanCollect.Status = hisRecord.Status == StationTaskStatusEnum.已完成 ? "已完成" : "未完成";
            ScanData = hisRecord.ScanCollectData;
        }
        #endregion


        public async void CatchUserScanMessage(ScanCodeGunRequest request)
        {
            try
            {
                var result = await App.CompareCodeRuleAsync(this.TaskScanCollect.CodeRule, request.ScanCodeContext);
                if (!result)
                {
                    return;
                }
                ScanData = request.ScanCodeContext;
                DealScanData();
            }
            catch (Exception ex)
            {
                await mediator.Publish(new AlarmSYSNotification() { Code = AlarmCode.系统运行错误, Name = AlarmCode.系统运行错误.ToString(), Module = AlarmModule.DESOUTTER_MODULE, Description = $"{ex.Message}" });
            }
        }


        /// <summary>
        /// 当前任务完成
        /// </summary>
        /// <param name="hasGoods"></param>
        private async void CompleteTask()
        {

            TaskScanCollect.HasPassed = true;
            bool saveOK = await SaveUserScanData();
            if (saveOK)
            {
                StationTaskDTO.HasFinish = true;
                OnCompleteTask(StationTaskDTO);
                TaskScanCollect.Status = "已完成";
            }
            else
            {
                var notice = new MessageNotice() { messageType = MessageTypeEnum.错误, showType = MessageShowType.右下角弹窗, MessageStr = $"保存失败，请联系管理员！" };
                await this.mediator.Publish(notice);
                await mediator.Publish(new AlarmSYSNotification() { Code = AlarmCode.系统运行错误, Name = AlarmCode.系统运行错误.ToString(), Module = AlarmModule.DESOUTTER_MODULE, Description = $"当前任务数据保存失败，请联系管理员！" });
            }
        }

        private readonly IMediator mediator;
        private string _ScanData;
        public string ScanData
        {
            get => _ScanData;
            set
            {
                if (_ScanData != value)
                {
                    _ScanData = value;
                    OnPropertyChanged(nameof(ScanData));
                }
            }
        }
        private string _Status;
        public string Status
        {
            get => _Status;
            set
            {
                if (this._Status != null)
                {
                    _Status = value;
                    OnPropertyChanged(nameof(Status));
                }
            }
        }

        public async Task<bool> SaveUserScanData()
        {
            TaskScanCollect.CreateUserID = App._RealtimePage.WorkingUserId;
            TaskScanCollect.CreateTime = DateTime.Now;
            return await App._RealtimePage.SaveScanCollectData(TaskScanCollect, StationTaskDTO.StationTaskId);
        }


        public async void DealScanData()
        {
            //扫码校验
            var Scan = await _ApiHelper.GetScanCollectInfo(ScanData);
            if (Scan.Result)
            {
                TaskScanCollect.ScanInputData = ScanData;
                TaskScanCollect.PackPN = App.PackBarCode;
                CompleteTask();
            }
            else
            {
                //报警并清除扫码内容
                ScanData = string.Empty;
                await mediator.Publish(new AlarmSYSNotification() { Code = AlarmCode.系统运行错误, Name = AlarmCode.系统运行错误.ToString(), Module = AlarmModule.DESOUTTER_MODULE, Description = $"当前扫码的二维码在当前工站重复，请更换二维码后重试" });
            }

        }

    }
}
