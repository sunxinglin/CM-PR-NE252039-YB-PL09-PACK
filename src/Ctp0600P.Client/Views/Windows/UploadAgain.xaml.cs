using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;

using Ctp0600P.Client.Apis;
using Ctp0600P.Client.Protocols;

using MediatR;

using Microsoft.Extensions.DependencyInjection;

using Reactive.Bindings;

using Yee.Entitys.AlarmMgmt;
using Yee.Services.CatlMesInvoker;

namespace Ctp0600P.Client.Views.Windows;

/// <summary>
/// UploadAgain.xaml 的交互逻辑
/// </summary>
public partial class UploadAgain : Window
{
    private readonly UploadAgainViewModel _uploadAgainViewModel;

    public UploadAgain(UploadAgainViewModel uploadAgainViewModel)
    {
        InitializeComponent();
        _uploadAgainViewModel = uploadAgainViewModel;
        DataContext = _uploadAgainViewModel;
        _uploadAgainViewModel.IsNeedCloseWindow.ToEvent().OnNext += s =>
        {
            if (s)
            {
                Close();
            }
        };
    }

    private void Window_Closed(object sender, EventArgs e)
    {
        App.UploadAgainWin = null;
    }
}

public class UploadAgainViewModel
{
    private readonly ICatlMesInvoker _mesInvoker;
    private readonly APIHelper _apiHelper;
    private readonly IMediator _mediator;

    public UploadAgainViewModel(IServiceProvider service)
    {
        _mesInvoker = service.GetRequiredService<ICatlMesInvoker>();
        _apiHelper = service.GetRequiredService<APIHelper>();
        _mediator = service.GetRequiredService<IMediator>();

        PackCode = App.ScanCodeGunRequestSubject.Select(s => s.ScanCodeContext).ToReactiveProperty();
        StationCode = new ReactiveProperty<string> { Value = App._StepStationSetting.StationCode };
        IsNeedChangeStatus = new ReactiveProperty<bool> { Value = false };
        BtnOk = new ReactiveCommand().WithSubscribe(async () =>
        {
            var resp = await _apiHelper.GetUploadDataAgain(PackCode.Value, StationCode.Value);
            if (resp.Code != 200)
            {
                await _mediator.Publish(new AlarmSYSNotification
                    { Code = AlarmCode.系统运行错误, Name = nameof(AlarmCode.系统运行错误), Description = resp.Message });
                return;
            }

            if (resp.Result == null)
            {
                IsNeedCloseWindow.OnNext(true);
                return;
            }

            var result = await _mesInvoker.dataCollect(PackCode.Value, resp.Result.DCParams, IsNeedChangeStatus.Value, StationCode.Value);
            if (result.code != 0)
            {
                await _mediator.Publish(new AlarmSYSNotification
                {
                    Code = AlarmCode.CatlMES错误, Name = AlarmCode.CatlMES错误.ToString(),
                    Description = $"(Code={result.code}) {result.message}"
                });
                return;
            }

            IsNeedCloseWindow.OnNext(true);
        });
        BtnCancel = new ReactiveCommand().WithSubscribe(() => { IsNeedCloseWindow.OnNext(true); });
    }

    public Subject<bool> IsNeedCloseWindow { get; } = new();
    public ReactiveProperty<string> PackCode { get; }
    public ReactiveProperty<string> StationCode { get; }
    public ReactiveProperty<bool> IsNeedChangeStatus { get; }
    public ReactiveCommand BtnOk { get; }
    public ReactiveCommand BtnCancel { get; }
}