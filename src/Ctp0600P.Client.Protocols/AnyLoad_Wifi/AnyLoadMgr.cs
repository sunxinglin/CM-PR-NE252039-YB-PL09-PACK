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

namespace Ctp0600P.Client.Protocols.AnyLoad_Wifi
{
    public class AnyLoadMgr
    {
        private readonly StepStationSetting _stationSetting;
        private readonly IServiceProvider _sp;
        private readonly IAPIHelper _ApiHelper;
        public AnyLoadCtrl? _AnyLoadCtrl { get; set; }
        public AnyLoadMgr(IServiceProvider sp, IAPIHelper ApiHelper, IOptionsMonitor<StepStationSetting> stationSetting)
        {
            _ApiHelper = ApiHelper;
            _stationSetting = stationSetting.CurrentValue;
            _sp = sp;
        }

        public async Task LoadProtocols()
        {
            var AnyLoadlist = await _ApiHelper.LoadStationProResourceConfig(Yee.Common.Library.CommonEnum.ProResourceTypeEnum.无线电子秤);

            if (AnyLoadlist != null && AnyLoadlist.Count > 0)
            {
                var item = AnyLoadlist.FirstOrDefault();
                if (item != null)
                    _AnyLoadCtrl = ActivatorUtilities.CreateInstance<AnyLoadCtrl>(_sp, item);
            }
        }
    }
}
