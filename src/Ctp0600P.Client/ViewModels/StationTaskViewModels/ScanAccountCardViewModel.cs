using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using Ctp0600P.Client.Apis;
using Ctp0600P.Client.Protocols;
using Ctp0600P.Client.Protocols.ScanCode.Models;

using MediatR;

using Yee.Common.Library.CommonEnum;
using Yee.Entitys.AlarmMgmt;
using Yee.Entitys.DBEntity;
using Yee.Entitys.DTOS;
using Yee.Entitys.Production;

namespace Ctp0600P.Client.ViewModels.StationTaskViewModels
{
    public class ScanAccountCardViewModel : TaskViewModelBase
    {
        private readonly APIHelper _apiHelper;
        private readonly IMediator _mediator;
        public Base_StationTaskScanAccountCard TaskScanAccountCard { get; }

        public StationTaskDTO _StationTaskDTO { get; }

        public ScanAccountCardViewModel(StationTaskDTO stationTaskDTO, APIHelper apiHelper, IMediator mediator)
        {
            _apiHelper = apiHelper;
            _mediator = mediator;
            _StationTaskDTO = stationTaskDTO;
            TaskScanAccountCard = _StationTaskDTO.StationTaskScanAccountCard;

            BindHisData();
        }

        public void BindHisData()
        {
            var histDto = App.HisTaskData2.StationTaskRecords.FirstOrDefault(f => f.StationTaskId == _StationTaskDTO.StationTaskId);
            var scanAccountCard = histDto?.ScanAccountCards;
            if (scanAccountCard != null)
            {
                _StationTaskDTO.HasFinish = histDto.StationTaskRecord.Status == StationTaskStatusEnum.已完成;
                TaskScanAccountCard.HasPassed = scanAccountCard.Status == StationTaskStatusEnum.已完成;
                AccountCardValue = scanAccountCard.AccountValue;
            }
        }
        /// <summary>
        /// 绑定历史信息
        /// </summary>
        /// <param name="record"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void BindHisData(Proc_StationTask_Record record)
        {
            var hisRecord = record.Proc_StationTask_ScanAccountCard;
            if (hisRecord == null) return;
            _StationTaskDTO.HasFinish = record.Status == StationTaskStatusEnum.已完成;
            TaskScanAccountCard.HasPassed = hisRecord.Status == StationTaskStatusEnum.已完成;
            AccountCardValue = hisRecord.AccountValue;
        }


        #region 【处理扫码枪扫码消息】

        /// <summary>
        /// 处理扫码枪扫码消息
        /// </summary>
        /// <param name="request"></param>
        public async void CatchScanCodeMessage(ScanCodeGunRequest request)
        {
            var checkResult = await _apiHelper.Check_CreateAccountCardAsync(request.ScanCodeContext);
            if (checkResult.Code != 200)
            {
                await _mediator.Publish(new AlarmSYSNotification { Code = AlarmCode.扫码枪错误, Name = AlarmCode.扫码枪错误.ToString(), Module = AlarmModule.DESOUTTER_MODULE, Description = $"[扫描员工卡]{checkResult.Message}" });
                AccountCardValue = String.Empty;
                return;
            }
            AccountCardValue = request.ScanCodeContext;
            App.Current.Dispatcher.Invoke((Action)(async () =>
            {
                try
                {
                    AccountName = checkResult.Result.Name;
                    App._RealtimePage.WorkingUserId = checkResult.Result.Id;
                    bool saveOK = await SaveAccountCardData();
                    if (!saveOK)
                    {
                        await _mediator.Publish(new AlarmSYSNotification() { Code = AlarmCode.系统运行错误, Name = AlarmCode.系统运行错误.ToString(), Module = AlarmModule.DESOUTTER_MODULE, Description = $"当前任务数据保存失败，请联系管理员！" });
                        return;
                    }
                    _StationTaskDTO.HasFinish = true;
                    OnCompleteTask(_StationTaskDTO);
                    AccountName = String.Empty;
                    AccountCardValue = String.Empty;
                }
                catch (Exception ex)
                {
                    await _mediator.Publish(new AlarmSYSNotification { Code = AlarmCode.系统运行错误, Name = AlarmCode.系统运行错误.ToString(), Module = AlarmModule.DESOUTTER_MODULE, Description = $"[扫描员工卡]{ex.Message}\r\n{ex.StackTrace}" });
                }
            }));
        }

        public ICommand ScanIDCard { get; }
        #endregion


        public async Task<bool> SaveAccountCardData()
        {
            TaskScanAccountCard.CreateUserID = App._RealtimePage.WorkingUserId;
            TaskScanAccountCard.AccountCardValue = AccountCardValue;
            TaskScanAccountCard.CreateTime = DateTime.Now;
            return await App._RealtimePage.SaveAccountCardData(TaskScanAccountCard,_StationTaskDTO.StationTaskId);
        }

        #region 【声明】

        private string _AccountCardValue;

        public string AccountCardValue
        {
            get => _AccountCardValue;
            set
            {
                if (_AccountCardValue != value)
                {
                    _AccountCardValue = value;
                    OnPropertyChanged(nameof(AccountCardValue));
                }
            }
        }

        private string _AccountName;

        public string AccountName
        {
            get => _AccountName;
            set
            {
                if (_AccountName != value)
                {
                    _AccountName = value;
                    OnPropertyChanged(nameof(AccountName));
                }
            }
        }

        #endregion
    }
}
