using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Ctp0600P.Client.Apis;
using Ctp0600P.Client.Protocols;
using Ctp0600P.Client.Protocols.ScanCode.Models;

using MediatR;

using Microsoft.Extensions.DependencyInjection;

using Reactive.Bindings;

using Yee.Common.Library.CommonEnum;
using Yee.Entitys;
using Yee.Entitys.AlarmMgmt;
using Yee.Entitys.DTOS;

namespace Ctp0600P.Client.ViewModels
{
    public class ReWorkPageViewModel
    {
        private readonly IMediator _mediator;
        private readonly IServiceProvider _serviceProvider;
        private readonly APIHelper _apiHelper;

        public ReWorkPageViewModel(IMediator mediator, IServiceProvider serviceProvider, APIHelper apiHelper)
        {
            this._mediator = mediator;
            this._serviceProvider = serviceProvider;
            this._apiHelper = apiHelper;

            this.ReWorkDatas = new ReactiveProperty<List<ReWorkData>>();
            this.PackCode = new ReactiveProperty<string>() { Value = App.PackBarCode };
            this.StepCode = new ReactiveProperty<string>() { Value = App._StepStationSetting.StationCode };

            this.LoadReworkList = new ReactiveCommand<bool>().WithSubscribe(async w =>
            {
                if (string.IsNullOrEmpty(App.PackBarCode) || string.IsNullOrEmpty(App._StepStationSetting.StationCode))
                {
                    return;
                }
                var dto = new LoadReworkDataDto()
                {
                    PackCode = PackCode.Value,
                    StepCode = StepCode.Value,
                };
                var list = await _apiHelper.LoadReworkList(dto);
                ReWorkDatas.Value = list;
            });

            this.SetRework = new ReactiveCommand<object>().WithSubscribe(async _ =>
            {
                List<WorkRecord> workRecords = new List<WorkRecord>();
                foreach (var item in ReWorkDatas.Value)
                {
                    workRecords.AddRange(item.WorkRecords.Where(w => w.IsChecked).ToList());
                }
                
                
                try
                {
                    var imageTasks = ReWorkDatas.Value
                        .Where(d => d.StationTaskType == StationTaskTypeEnum.图示拧紧)
                        .ToList();

                    var selectedImage = imageTasks
                        .SelectMany(d => d.WorkRecords.Select(w => (data: d, record: w)))
                        .Where(x => x.record.IsChecked && x.record.OrderNo > 0)
                        .ToList();

                    if (selectedImage.Count > 0)
                    {
                        var target = selectedImage.OrderBy(x => x.record.OrderNo).First();
                        App.ReworkLocateTightenByImageStationTaskId = target.data.StationTaskId;
                        App.ReworkLocateTightenByImageOrderNo = target.record.OrderNo;
                    }
                    else
                    {
                        App.ReworkLocateTightenByImageStationTaskId = null;
                        App.ReworkLocateTightenByImageOrderNo = null;
                    }
                }
                catch
                {
                    App.ReworkLocateTightenByImageStationTaskId = null;
                    App.ReworkLocateTightenByImageOrderNo = null;
                }

                
                var result = await _apiHelper.MakeRework(workRecords);
                if (result.Code != 200)
                {
                    await mediator.Publish(new AlarmSYSNotification() { Code = AlarmCode.返工错误, Name = AlarmCode.返工错误.ToString(), Module = AlarmModule.DESOUTTER_MODULE, Description = $"{result.Message}" });
                    return;
                }
                var request = new ScanCodeGunRequest
                {
                    ScanCodeContext = App.PackBarCode.ToString(),
                    ScanCodePortName = "",
                    FromRework = true,
                };

                using var scope = serviceProvider.CreateScope();
                var sp = scope.ServiceProvider;
                var realViewModel = sp.GetRequiredService<RealtimePageViewModel>();
                realViewModel.HasReWork = true;
                realViewModel.InitEmpty_TaskData(false);
                await Task.Delay(100);
                App.ScanCodeGunRequestSubject.OnNext(request);

            });

            this.SetWorking = new ReactiveCommand<ReWorkData>().WithSubscribe(async data =>
            {
                var result = await _apiHelper.SetRecordWorking(data.Id);
                if (result.Code != 200)
                {
                    await mediator.Publish(new AlarmSYSNotification() { Code = AlarmCode.返工错误, Name = AlarmCode.返工错误.ToString(), Module = AlarmModule.DESOUTTER_MODULE, Description = $"{result.Message}" });
                    return;
                }
                var request = new ScanCodeGunRequest
                {
                    ScanCodeContext = App.PackBarCode.ToString(),
                    ScanCodePortName = "",
                    FromRework = true,
                };

                using var scope = serviceProvider.CreateScope();
                var sp = scope.ServiceProvider;
                var realViewModel = sp.GetRequiredService<RealtimePageViewModel>();
                realViewModel.HasReWork = true;
                realViewModel.InitEmpty_TaskData(false);
                await Task.Delay(100);
                App.ScanCodeGunRequestSubject.OnNext(request);
                data.Statue = StationTaskStatusEnum.进行中;
            });

            this.LoadReworkList.Execute(true);
        }

        #region 数据

        public ReactiveProperty<string> PackCode { get; set; }

        public ReactiveProperty<string> StepCode { get; set; }

        public ReactiveProperty<List<ReWorkData>> ReWorkDatas { get; set; }

        public ReactiveCommand<bool> LoadReworkList { get; set; }
        #endregion

        public ReactiveCommand<object> SetRework { get; }

        public ReactiveCommand<ReWorkData> SetWorking { get; }
    }
}
