using Ctp0600P.Shared;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Yee.Common.Library.CommonEnum;

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
            var AnyLoadlist = await _apiHelper.LoadStationProResourceConfig(ProResourceTypeEnum.电子秤);

            if (AnyLoadlist != null && AnyLoadlist.Count > 0)
            {
                var item = AnyLoadlist.FirstOrDefault();
                if(item != null)
                    _AnyLoadCtrl = ActivatorUtilities.CreateInstance<AnyLoadCtrl>(_sp, item);
            }
        }
    }
}
