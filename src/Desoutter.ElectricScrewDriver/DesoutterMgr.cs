using Ctp0600P.Client.Protocols;
using Ctp0600P.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Desoutter.ElectricScrewDriver
{
    public class DesoutterMgr
    {
        private readonly IAPIHelper _ApiHelper;
        private readonly StepStationSetting _stationSetting;
        private readonly IServiceProvider _sp;
        public Dictionary<string, DesoutterCtrl> DesoutterCtrls { get; set; } = new Dictionary<string, DesoutterCtrl>();
        public DesoutterMgr(IServiceProvider sp, IAPIHelper ApiHelper, IOptionsMonitor<StepStationSetting> stationSetting)
        {
            _ApiHelper = ApiHelper;
            _stationSetting = stationSetting.CurrentValue;
            _sp = sp;
        }
        public async Task LoadProtocols()
        {
            var desoutterlist = await _ApiHelper.LoadStationProResourceConfig(Yee.Common.Library.CommonEnum.ProResourceTypeEnum.拧紧枪);
            if (desoutterlist != null && desoutterlist.Count > 0)
            {
                foreach (var item in desoutterlist)
                {
                    var ctrl = ActivatorUtilities.CreateInstance<DesoutterCtrl>(_sp, item);
                    DesoutterCtrls.Add(item.DeviceNo, ctrl);
                }
            }
        }

    }
}
