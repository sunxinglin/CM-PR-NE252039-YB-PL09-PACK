using Ctp0600P.Client.Apis;
using Ctp0600P.Client.Command.NoticeHelp.Enum;
using Ctp0600P.Client.Command.NoticeHelp.Handle;
using Ctp0600P.Client.CommonEntity;
using Ctp0600P.Client.Protocols;
using Ctp0600P.Client.Protocols.AnyLoad;

using Ctp0600P.Client.Protocols.ScanCode.Models;
using FutureTech.Mvvm;
using MediatR;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Yee.Common.Library.CommonEnum;
using Yee.Common.Library.LogHelper;
using Yee.Entitys.AlarmMgmt;
using Yee.Entitys.DBEntity;
using Yee.Entitys.DTOS;
using Yee.Entitys.Production;

namespace Ctp0600P.Client.ViewModels.StationTaskViewModels
{
    public class UserInputCollectViewModel : TaskViewModelBase
    {

        private string _UserInput = "";
        public string UserInput
        {
            get => _UserInput;
            set
            {
                if (this._UserInput != value)
                {
                    _UserInput = value;
                    this.OnPropertyChanged(nameof(UserInput));
                }
            }
        }
        public ICommand NumerClick { get; }

        private Visibility _NgTips;
        public Visibility NgTips
        {
            get { return _NgTips; }
            set
            {
                _NgTips = value;
                OnPropertyChanged(nameof(NgTips));
            }
        }

        private void DoSomething()
        {
            if (string.IsNullOrEmpty(UserInput)
             || UserInput.StartsWith("00")
             || UserInput.StartsWith(".")
             || UserInput.EndsWith(".")
             || UserInput.Length > 10
             || UserInput.Split(".").Length > 2)
            {
                UserInput = string.Empty;
                MessageBox.Show("数值不合法，请重新输入");
                NgTips = Visibility.Visible;
            }
            else
            {
                if (string.IsNullOrEmpty(MinData)) MinData = "0";
                if(Convert.ToDouble(UserInput) <= Convert.ToDouble(MaxData) && Convert.ToDouble(UserInput) >= Convert.ToDouble(MinData))
                {
                    DealScanData();
                }
                else
                {
                     NgTips = Visibility.Visible;

                    var error = new AlarmSYSNotification { Code = AlarmCode.用户输入错误, Name = AlarmCode.用户输入错误.ToString(), Module = AlarmModule.DESOUTTER_MODULE, Description = $"当前输入的值{UserInput}不在范围{MinData}--{MaxData}中" };
                    mediator.Publish(error);

                   ///
                }
            }
        }


        private string _StationTaskName;
        public string StationTaskName
        {
            get { return _StationTaskName; }
            set
            {
                _StationTaskName = value;
                OnPropertyChanged(nameof(StationTaskName));
            }
        }

        public StationTaskDTO StationTaskDTO { get; }
        public Base_StationTaskUserInput TaskUserInput { get; }
        private readonly APIHelper _ApiHelper;
        public UserInputCollectViewModel(StationTaskDTO stationTaskDTO, IMediator mediator, APIHelper ApiHelper)
        {
            StationTaskDTO = stationTaskDTO;
            TaskUserInput = StationTaskDTO.StationTaskUserInput;
            this.mediator = mediator;
            this._ApiHelper = ApiHelper;
            NgTips = Visibility.Hidden;
            this.NumerClick = new AsyncRelayCommand<Button>(
                    async number =>
                    {
                        NgTips = Visibility.Hidden;

                        if (number.Content.ToString() == "清除")
                            UserInput = String.Empty;
                        else if (number.Content.ToString() == "退格")
                        {
                            if (!String.IsNullOrEmpty(UserInput))
                                UserInput = UserInput.Substring(0, UserInput.Length - 1);
                        }
                        else if (number.Content.ToString() == "确定")
                        {
                            DoSomething();
                        }
                        else
                        {
                            UserInput += number.Content.ToString();
                        }

                    },
                    o => true
                );


            MaxData = TaskUserInput?.MaxRange?.ToString("#0.######");
            MinData = TaskUserInput?.MinRange?.ToString("#0.######");

            //var Scan = _ApiHelper.GetScanSetInfo(TaskUserScan.StationTaskId);
            //if(Scan!=null)
            //{
            //    MaxData = TaskUserScan?.MaxRange.ToString().TrimEnd(new char[] { '0','.'});//Scan.Result.MaxRange.ToString().TrimEnd(new char[] { "0"});
            //    MinData = TaskUserScan?.MinRange.ToString().TrimEnd(new char[] { '0','.' });//Scan.Result.MinRange.ToString().TrimEnd();
            //}
            //else
            //{
            //    var alarm = new AlarmSYSNotification(){ Code = AlarmCode.系统运行错误, Name = AlarmCode.系统运行错误.ToString(), Module = AlarmModule.DESOUTTER_MODULE, Description = $"没有查询到用户录入的任务详情，请检查配方设置" };
            //    mediator.Publish(alarm);
            //}
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
            var userInput = histDto?.UserInputs;
            if (userInput != null)
            {
                StationTaskDTO.HasFinish = histDto.StationTaskRecord.Status == StationTaskStatusEnum.已完成;
                TaskUserInput.HasPassed = userInput.Status == StationTaskStatusEnum.已完成;
                TaskUserInput.Status = userInput.Status.ToString();
                UserInput = userInput.UserInputData; ;
            }
        }
        /// <summary>
        /// 绑定历史信息
        /// </summary>
        /// <param name="record"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void BindHisData(Proc_StationTask_Record record)
        {
            var hisRecord = record.Proc_StationTask_UserInput;
            if (hisRecord == null) return;
            StationTaskDTO.HasFinish = record.Status == StationTaskStatusEnum.已完成;
            TaskUserInput.HasPassed = hisRecord.Status == StationTaskStatusEnum.已完成;
            TaskUserInput.Status = hisRecord.Status == StationTaskStatusEnum.已完成 ? "已完成" : "未完成";
            UserInput = hisRecord.UserInputData;
        }
        #endregion

        private string _MinData;
        public string MinData
        {
            get { return _MinData; }
            set
            {
                _MinData = value;
                OnPropertyChanged(nameof(MinData));
            }
        }

        private string _MaxData;
        public string MaxData
        {
            get { return _MaxData; }
            set
            {
                _MaxData = value;
                OnPropertyChanged(nameof(MaxData));
            }
        }


        /// <summary>
        /// 当前任务完成
        /// </summary>
        /// <param name="hasGoods"></param>
        private async void CompleteTask()
        {
            Status = "已完成";
            TaskUserInput.HasPassed = true;
            bool saveOK = await SaveUserScanData();
            if (!saveOK)
            {
                await mediator.Publish(new AlarmSYSNotification() { Code = AlarmCode.系统运行错误, Name = AlarmCode.系统运行错误.ToString(), Module = AlarmModule.DESOUTTER_MODULE, Description = $"当前任务数据保存失败，请联系管理员！" });
                return;
            }
           

            StationTaskDTO.HasFinish = true;
            OnCompleteTask(StationTaskDTO);
        }


       // private string _ScanData;
        private readonly IMediator mediator;

        //public string ScanData
        //{
        //    get => _ScanData;
        //    set
        //    {
        //        if (_ScanData != value)
        //        {
        //            _ScanData = value;
        //            OnPropertyChanged(nameof(ScanData));
        //        }
        //    }
        //}
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
            TaskUserInput.CreateUserID = App._RealtimePage.WorkingUserId;
            TaskUserInput.CreateTime = DateTime.Now;
            return await App._RealtimePage.SaveUserScanData(TaskUserInput,StationTaskDTO.StationTaskId);
        }


        public async void DealScanData()
        {
            try
            {
                TaskUserInput.ScanData = UserInput;
                TaskUserInput.PackPN = App.PackBarCode;
                CompleteTask();    
            }
            catch
            {
                  await mediator.Publish(new AlarmSYSNotification() { Code = AlarmCode.系统运行错误, Name = AlarmCode.系统运行错误.ToString(), Module = AlarmModule.DESOUTTER_MODULE, Description = $"完成任务出错" });
            }
        }

    }
}
