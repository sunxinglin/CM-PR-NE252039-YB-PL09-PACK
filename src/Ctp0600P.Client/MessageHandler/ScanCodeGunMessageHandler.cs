using System;
using System.Threading;
using System.Threading.Tasks;

using AsZero.Core.Services.Messages;

using Ctp0600P.Client.Command.NoticeHelp.Enum;
using Ctp0600P.Client.Command.NoticeHelp.Handle;
using Ctp0600P.Client.Protocols.ScanCode.Models;

using MediatR;

using Microsoft.Extensions.Logging;

using Yee.Common.Library.CommonEnum;

namespace Ctp0600P.Client.MessageHandler;

public class ScanCodeGunMessageHandler : INotificationHandler<ScanCodeGunRequest>
{
    private readonly IMediator _mediator;

    public ScanCodeGunMessageHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Handle(ScanCodeGunRequest request, CancellationToken cancellationToken)
    {
        // 日志记录扫码结果
        var msg = new LogMessage
        {
            Content = $"当前扫码结果：{request.ScanCodeContext}",
            Level = LogLevel.Information,
            Timestamp = DateTime.Now,
        };
        await _mediator.Publish(new UILogNotification(msg), cancellationToken);

        if (!App._StepStationSetting.IsDebug && App.StationTaskNGPause)
        {
            // 日志记录扫码结果
            var msg1 = new LogMessage
            {
                Content = "当前存在未处理的报警，扫码结果无效",
                Level = LogLevel.Error,
                Timestamp = DateTime.Now,
            };
            await _mediator.Publish(new UILogNotification(msg1), cancellationToken);
            return;
        }

        // 小车未进站
        if (string.IsNullOrEmpty(App.AGVCode) &&
            App._StepStationSetting.StepType == StepTypeEnum.线内人工站 &&
            App.UploadAgainWin == null)
        {
            var notice = new MessageNotice
                { messageType = MessageTypeEnum.警告, showType = MessageShowType.右下角弹窗, MessageStr = "AGV未进站，请先进站AGV。" };
            await _mediator.Publish(notice, cancellationToken);
            return;
        }

        App.ScanCodeGunRequestSubject.OnNext(request);
    }
}