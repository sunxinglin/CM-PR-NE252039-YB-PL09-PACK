using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using AsZero.Core.Services.Messages;
using Ctp0600P.Client.Protocols;
using Ctp0600P.Shared;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Yee.Entitys.Production;

namespace Ctp0600P.Client.Protocols.AnyLoad
{
    public class AnyLoadMgr
    {
        private readonly StepStationSetting _stationSetting;
        private readonly IAPIHelper _apiHelper;
        private readonly IServiceProvider _sp;

        public AnyLoadCtrl? _AnyLoadCtrl { get; set; }
        public AnyLoadMgr(IServiceProvider sp, IAPIHelper _ApiHelper, IOptionsMonitor<StepStationSetting> stationSetting)
        {
            _apiHelper = _ApiHelper;
            _stationSetting = stationSetting.CurrentValue;
            _sp = sp;
        }

        public async Task LoadProtocols()
        {
            var AnyLoadlist = await _apiHelper.LoadStationProResourceConfig(Yee.Common.Library.CommonEnum.ProResourceTypeEnum.电子秤);

            if (AnyLoadlist != null && AnyLoadlist.Count > 0)
            {
                var item = AnyLoadlist.FirstOrDefault();
                if(item != null)
                    _AnyLoadCtrl = ActivatorUtilities.CreateInstance<AnyLoadCtrl>(_sp, item);
            }
        }
    }
}
