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

namespace Ctp0600P.Client.Protocols.ScanCode
{
    public class ScanCodeMgr
    {
        private readonly StepStationSetting _stationSetting;
        private readonly IServiceProvider _sp;
        private readonly IAPIHelper _apiHelper;

        public Dictionary<string, ScanCodeCtrl> ScanCodeCtrls { get; set; } = new Dictionary<string, ScanCodeCtrl>();
        public ScanCodeMgr(IServiceProvider sp, IAPIHelper _ApiHelper, IOptionsMonitor<StepStationSetting> stationSetting)
        {
            _apiHelper = _ApiHelper;
            _stationSetting = stationSetting.CurrentValue;
            _sp = sp;

        }

        public async Task LoadProtocols()
        {
            var scanCodelist = await _apiHelper.LoadStationProResourceConfig(Yee.Common.Library.CommonEnum.ProResourceTypeEnum.扫码枪);

            if (scanCodelist != null && scanCodelist.Count > 0)
            {
                foreach (var item in scanCodelist)
                {
                    var ctrl = ActivatorUtilities.CreateInstance<ScanCodeCtrl>(_sp, item);
                    ScanCodeCtrls.Add(item.Name ?? "", ctrl);
                }
            }
        }
    }
}
