using Ctp0600P.Shared;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Yee.Common.Library.CommonEnum;

namespace Ctp0600P.Client.Protocols.AnyLoad_Wifi;

public class AnyLoadMgr
{
    private readonly StepStationSetting _stationSetting;
    private readonly IServiceProvider _sp;
    private readonly IAPIHelper _apiHelper;
    public AnyLoadCtrl? _AnyLoadCtrl { get; set; }
    public AnyLoadMgr(IServiceProvider sp, IAPIHelper apiHelper, IOptionsMonitor<StepStationSetting> stationSetting)
    {
        _apiHelper = apiHelper;
        _stationSetting = stationSetting.CurrentValue;
        _sp = sp;
    }

    public async Task LoadProtocols()
    {
        var AnyLoadList = await _apiHelper.LoadStationProResourceConfig(ProResourceTypeEnum.无线电子秤);

        if (AnyLoadList is { Count: > 0 })
        {
            var item = AnyLoadList.FirstOrDefault();
            if (item != null)
            {
                _AnyLoadCtrl = ActivatorUtilities.CreateInstance<AnyLoadCtrl>(_sp, item);
            }
        }
    }
}