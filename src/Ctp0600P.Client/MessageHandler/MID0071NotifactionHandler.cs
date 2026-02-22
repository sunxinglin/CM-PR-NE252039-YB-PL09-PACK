//using AsZero.Core.Services.Messages;
//using Ctp0600P.Client.ViewModels;
//using MediatR;
//using Microsoft.Extensions.Logging;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Windows;

//namespace Ctp0600P.Client.MessageHandler
//{
//    public class MID0071NotifactionHandler : INotificationHandler<MID0071_AlarmNotifaction>
//    {
//        private readonly ILogger<MID0071NotifactionHandler> _logger;
//        public MID0071NotifactionHandler(ILogger<MID0071NotifactionHandler> logger)
//        {
//            this._logger = logger;
//        }

//        public Task Handle(MID0071_AlarmNotifaction notification, CancellationToken cancellationToken)
//        {
//            return Task.CompletedTask;
//        }
//    }
//}
