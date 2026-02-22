using Ctp0600P.Client.Apis;
using Ctp0600P.Client.CommonEntity;
using Ctp0600P.Client.Protocols;
using Ctp0600P.Client.Protocols.ScanCode.Models;
using Ctp0600P.Client.ViewModels;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NPOI.SS.Formula.Functions;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Yee.Common.Library.CommonEnum;
using Yee.Entitys.AlarmMgmt;
using Yee.Entitys.AutomaticStation;
using Yee.Entitys.Production;

namespace Ctp0600P.Client.Views.Windows
{
    /// <summary>
    /// SkipStepWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SkipStepWindow : Window
    {
        private readonly SkipStepViewModel _vm;
        
        public SkipStepWindow(SkipStepViewModel vm)
        {
            InitializeComponent();
            _vm = vm;
            this.DataContext = _vm;
            this._vm.IsNeedCloseWindow.ToEvent().OnNext += s => {
                if (s)
                {
                    this.Close();
                }
            };
        }
    }

    public class SkipStepViewModel
    {
        private readonly APIHelper _apiHelper;
        private readonly RealtimePageViewModel _realV;
        private readonly IMediator _mediator;
        public SkipStepViewModel(IList<Base_StationTask> DataList, APIHelper apiHelper, RealtimePageViewModel realV, IMediator mediator)
        {
            _DataList = new ReactiveProperty<IList<TaskDto>> { Value = DataList.Where(w => w.Type != StationTaskTypeEnum.扫描员工卡 && w.Type != StationTaskTypeEnum.放行).Select(s => new TaskDto { TaskId = s.Id, TaskName = s.Name }).ToList() };
            DealFinish();///处理跳步列表
            this.AcceptSkip = new ReactiveCommand().WithSubscribe(async () =>
            {
                var datas = _DataList.Value.Where(w => w.IsChecked).Select(s => (s.TaskId, s.TaskName)).ToList();
                var result = await _apiHelper.MakeTaskListComplateForce(datas, App.HisTaskData2.MainId);
                if (result.Code != 200)
                {
                    await _mediator.Publish(new AlarmSYSNotification { Code = AlarmCode.系统运行错误, Name = AlarmCode.系统运行错误.ToString(), Description = result.Message });
                    return;
                }

                var request = new ScanCodeGunRequest
                {
                    ScanCodeContext = App.PackBarCode.ToString(),
                    ScanCodePortName = ""
                };

                _realV.InitEmpty_TaskData(false);
                await Task.Delay(100);
                App.ScanCodeGunRequestSubject.OnNext(request);
                IsNeedCloseWindow.OnNext(true);
            });
            this.Cancel = new ReactiveCommand().WithSubscribe(async () =>
            {
                IsNeedCloseWindow.OnNext(true);
            });
            _apiHelper = apiHelper;
            _realV = realV;
            _mediator = mediator;
        }
        public ReactiveProperty<IList<TaskDto>> _DataList { get; }

        public ReactiveCommand AcceptSkip { get; }
        public ReactiveCommand Cancel { get; }
        public Subject<bool> IsNeedCloseWindow { get; } = new Subject<bool>();
        public void DealFinish()
        {
            foreach(var item in _DataList.Value)
            {
                
                var record = App.HisTaskData2.StationTaskRecords.FirstOrDefault(f => f.StationTaskId == item.TaskId);
                if(record != null)
                {
                    item.Status = record.StationTaskRecord.Status;
                    item.IsEnable = record.StationTaskRecord.Status != StationTaskStatusEnum.已完成;
                }
            }
        }
        public class TaskDto
        {
            public bool IsChecked { get; set; } = false;
            public bool IsEnable { get; set; } = true;
        
            public int TaskId { get; set; }
            public string TaskName { get; set; } = "";

            public StationTaskStatusEnum Status { get; set; } = StationTaskStatusEnum.未开始; 

          
        }
    }
} 
