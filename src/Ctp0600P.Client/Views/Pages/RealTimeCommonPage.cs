using AsZero.Core.Services.Messages;
using Ctp0600P.Client.Apis;
using Ctp0600P.Client.Command.NoticeHelp.Enum;
using Ctp0600P.Client.Command.NoticeHelp.Handle;
using Ctp0600P.Client.Protocols;
using Ctp0600P.Client.ViewModels;
using Ctp0600P.Client.Views.StationTaskPages;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using Yee.Common.Library.LogHelper;
using Yee.Entitys.AlarmMgmt;
using Yee.Entitys.DTOS;
using Yee.Entitys.Production;
using Yee.Services.CatlMesInvoker;

namespace Ctp0600P.Client.Views.Pages
{
    /// <summary>
    /// StationTaskCommonPage.xaml 的交互逻辑
    /// </summary>
    public class RealTimeCommonPage : Page
    {
        public APIHelper _APIHepler;
        public ICatlMesInvoker _catlMesInvoker;
        public RealtimePageViewModel _VM;
        public int WorkingUserId;
        private readonly IMediator mediator;

        private SemaphoreSlim _slim = new SemaphoreSlim(1, 1);

        public RealTimeCommonPage(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task BuildUpMesCollectData()
        {
            if (App.ActivePage != null && App.ActivePage.GetType() == typeof(LetGoPage))
            {
                var data = await _APIHepler.GetUploadCATLData();
                if (data != null && data.Code == 200 && data.Result != null)
                {
                    App.UpCatlMesData = data.Result;
                }
                else if (data != null && data.Code == 500 && data.Result == null)
                {
                    var notice = new MessageNotice() { messageType = MessageTypeEnum.提示, showType = MessageShowType.右下角弹窗, MessageStr = "组织构建上传数据包超时，正在重试中。。。" };
                    await this.mediator.Publish(notice);

                    await BuildUpMesCollectData();
                }
            }
        }

        public async Task<bool> CallCatlMesCollectDataFucAsync()
        {
            if (App._StepStationSetting.IsDebug)
            {
                return true;
            }
            var res = await _APIHepler.GetUploadCATLData(); //加载上传数据
            var uploadData = res.Result;

            if (uploadData == null || uploadData.DCParams.Count() == 0)
            {
                var msg = new UILogNotification { LogMessage = new LogMessage { Content = "没有需要上传的收数数据，不调用收数接口！", Level = LogLevel.Information, Timestamp = DateTime.Now } };
                await this.mediator.Publish(msg);
                return true;
            }
            var stCode = !string.IsNullOrEmpty(App.CurrentStationCode) ? App.CurrentStationCode : App._StepStationSetting.StationCode;
            var cRes = await _catlMesInvoker.dataCollect(uploadData.PackCode, uploadData.DCParams, true, stCode);
            if (cRes.code != 0)
            {
                var cAlarm = new AlarmSYSNotification { Code = AlarmCode.CatlMES错误, Name = AlarmCode.CatlMES错误.ToString(), Description = $"(Code={cRes.code}) {cRes.message}", };
                await this.mediator.Publish(cAlarm);
                return false;
            }

            var cSuccMsg = new UILogNotification { LogMessage = new LogMessage { Content = "数据上传CATL成功！", Level = LogLevel.Information, Timestamp = DateTime.Now } };
            await this.mediator.Publish(cSuccMsg);
            return true;
        }

        /// <summary>
        /// 当前工位任务完成通知
        /// </summary>
        /// <param name="dto"></param>
        public async Task RealtimePage_CompleteTask(StationTaskDTO dto)
        {
            if (await _slim.WaitAsync(30) == false)
            {
                return;
            }
            try
            {
                bool saveStatusOK = false;
                if (dto.StationTask.Type == Yee.Common.Library.CommonEnum.StationTaskTypeEnum.放行)
                {
                    saveStatusOK = await SetStationCurTaskRunAGV(dto);
                    dto.HasFinish = saveStatusOK;
                }
                else
                    saveStatusOK = await SetStationCurTaskFinish(dto, App.HisTaskData2.MainId);
                if (saveStatusOK)
                {
                    bool hasFinish = _VM.CheckStationTaskFinish(dto);
                    if (hasFinish)
                    {
                        if (App._StepStationSetting.StepType == Yee.Common.Library.CommonEnum.StepTypeEnum.线外人工站)
                        {
                            _VM.InitEmpty();
                        }
                        else
                        {
                            this._VM.DealRunAGV();
                            _VM.InitEmpty();
                        }

                    }
                    else
                    {
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            _VM.GoNextUndoTask();
                            _VM.DealTaskStep();
                        });
                    }
                }
                else
                {
                    await mediator.Publish(new AlarmSYSNotification { Code = AlarmCode.系统运行错误, Name = AlarmCode.系统运行错误.ToString(), Module = AlarmModule.DESOUTTER_MODULE, Description = $"[{dto.StationTask.Type}]保存失败，请联系管理员" });
                }
            }
            finally
            {
                _slim.Release();
            }

        }

        /// <summary>
        /// 保存扫描员工卡信息
        /// </summary>
        /// <param name="taskScanAccountCard"></param>
        /// <returns></returns>
        public async Task<bool> SaveAccountCardData(Base_StationTaskScanAccountCard taskScanAccountCard, int taskId)
        {

            var response = await _APIHepler.SaveAccountCardData(taskScanAccountCard, taskId);
            if (response.Code == 200)
            {
                _VM.AddLogMsg.Execute(new LogMessage { Timestamp = DateTime.Now, Content = "保存扫描员工卡数据成功" });
                return true;
            }
            else
            {
                await mediator.Publish(new AlarmSYSNotification { Code = AlarmCode.扫码枪错误, Name = AlarmCode.扫码枪错误.ToString(), Module = AlarmModule.DESOUTTER_MODULE, Description = $"保存扫描员工卡失败{response.Message}" });
                return false;
            }
        }

        /// <summary>
        /// 放行
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        private async Task<bool> SetStationCurTaskRunAGV(StationTaskDTO dto)
        {
            var response = await _APIHepler.SetStationCurTaskRunAGV(dto);
            if (response.Code == 200)
            {
                if (!string.IsNullOrEmpty(App.AGVCode))
                    _VM.AddLogMsg.Execute(new LogMessage { Timestamp = DateTime.Now, Content = $"【{dto.StationTask.Name}】完成，AGV：{App.AGVCode}" });
                return true;
            }
            else
            {
                await mediator.Publish(new AlarmSYSNotification { Code = AlarmCode.AGV错误, Name = AlarmCode.AGV错误.ToString(), Module = AlarmModule.DESOUTTER_MODULE, Description = $"【{dto.StationTask.Name}】失败，AGV：{App.AGVCode},{response.Message}" });

                return false;
            }
        }

        private async Task<bool> SetStationCurTaskFinish(StationTaskDTO dto, int mainId)
        {
            var response = await _APIHepler.SetStationCurTaskFinish(dto, mainId);
            if (response.Code == 200)
            {
                _VM.SetStationCurTaskFinish(dto);
                _VM.AddLogMsg.Execute(new LogMessage { Timestamp = DateTime.Now, Content = $"【{dto.StationTask.Name}】完成" });
                return true;
            }
            else
            {
                await mediator.Publish(new AlarmSYSNotification { Code = AlarmCode.系统运行错误, Name = AlarmCode.系统运行错误.ToString(), Module = AlarmModule.DESOUTTER_MODULE, Description = $"【{dto.StationTask.Name}】失败" });
                return false;
            }
        }

        /// <summary>
        /// 保存电子秤数据
        /// </summary>
        /// <param name="taskAnyLoad"></param>
        /// <returns></returns>
        public async Task<bool> SaveAnyLoadData(Base_StationTaskAnyLoad taskAnyLoad, int taskId)
        {
            var response = await _APIHepler.SaveAnyLoadData(taskAnyLoad, taskId);
            if (response.Code == 200)
            {
                _VM.AddLogMsg.Execute(new LogMessage { Timestamp = DateTime.Now, Content = "保存称重数据成功" });
                return true;
            }
            else
            {
                await mediator.Publish(new AlarmSYSNotification { Code = AlarmCode.称重错误, Name = AlarmCode.称重错误.ToString(), Module = AlarmModule.DESOUTTER_MODULE, Description = $"保存称重数据失败" });
                return false;
            }
        }

        public async Task<bool> SaveUserScanData(Base_StationTaskUserInput taskUserScan, int taskId)
        {
            var response = await _APIHepler.Save_StationUserScan(taskUserScan, taskId);
            if (response.Code == 200)
            {
                _VM.AddLogMsg.Execute(new LogMessage { Timestamp = DateTime.Now, Content = "保存用户输入数据数据成功" });
                return true;
            }
            else
            {
                await mediator.Publish(new AlarmSYSNotification { Code = AlarmCode.用户输入错误, Name = AlarmCode.用户输入错误.ToString(), Module = AlarmModule.DESOUTTER_MODULE, Description = $"保存用户输入数据失败" });
                return false;
            }
        }

        public async Task<bool> SaveScanCollectData(Base_StationTaskScanCollect taskUserScan, int taskId)
        {
            var response = await _APIHepler.Save_StationScanCollect(taskUserScan, taskId);
            if (response.Code == 200)
            {
                _VM.AddLogMsg.Execute(new LogMessage { Timestamp = DateTime.Now, Content = "保存扫码输入数据数据成功" });
                return true;
            }
            else
            {
                await mediator.Publish(new AlarmSYSNotification { Code = AlarmCode.用户输入错误, Name = AlarmCode.用户输入错误.ToString(), Module = AlarmModule.DESOUTTER_MODULE, Description = $"保存用户输入数据失败" });
                return false;
            }
        }

        /// <summary>
        /// 保存螺丝拧紧数据
        /// </summary>
        /// <param name="screw"></param>
        /// <returns></returns>
        public async Task<bool> SaveScrewData(Base_StationTaskScrew screw, int taskId)
        {
            var response = await _APIHepler.SaveScrewData(screw, taskId);
            if (response != null && response.Code == 200)
            {
                _VM.AddLogMsg.Execute(new LogMessage { Timestamp = DateTime.Now, Content = "保存拧紧数据成功" });
                return true;
            }
            else
            {
                await mediator.Publish(new AlarmSYSNotification { Code = AlarmCode.拧紧枪错误, Name = AlarmCode.拧紧枪错误.ToString(), Module = AlarmModule.DESOUTTER_MODULE, Description = $"保存拧紧数据失败" });
                return false;
            }
        }

        /// <summary>
        /// 螺丝拧紧完成
        /// </summary>
        /// <param name="screw"></param>
        /// <returns></returns>
        public async Task<bool> SetScrewTaskFinish(Base_StationTaskScrew screw)
        {
            var response = await _APIHepler.SetScrewTaskFinish(screw);
            if (response.Code == 200)
            {
                _VM.AddLogMsg.Execute(new LogMessage { Timestamp = DateTime.Now, Content = $"【{screw.ScrewSpecs}】拧紧任务完成" });
                return true;
            }
            else
            {
                await mediator.Publish(new AlarmSYSNotification { Code = AlarmCode.拧紧枪错误, Name = AlarmCode.拧紧枪错误.ToString(), Module = AlarmModule.DESOUTTER_MODULE, Description = $"【{screw.ScrewSpecs}】拧紧任务失败" });
                return false;
            }
        }
    }
}
