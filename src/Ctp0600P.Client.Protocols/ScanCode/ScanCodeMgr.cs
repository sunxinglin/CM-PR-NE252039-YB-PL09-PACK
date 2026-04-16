using Ctp0600P.Shared;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Yee.Common.Library.CommonEnum;

namespace Ctp0600P.Client.Protocols.ScanCode;

public class ScanCodeMgr
{
    private readonly StepStationSetting _stationSetting;
    private readonly IServiceProvider _sp;
    private readonly IAPIHelper _apiHelper;

    public Dictionary<string, ScanCodeCtrl> ScanCodeCtrls { get; set; } = new Dictionary<string, ScanCodeCtrl>();
    public ScanCodeMgr(IServiceProvider sp, IAPIHelper apiHelper, IOptionsMonitor<StepStationSetting> stationSetting)
    {
        _apiHelper = apiHelper;
        _stationSetting = stationSetting.CurrentValue;
        _sp = sp;

    }

    public async Task LoadProtocols()
    {
        var scanCodelist = await _apiHelper.LoadStationProResourceConfig(ProResourceTypeEnum.扫码枪);

        if (scanCodelist.Count > 0)
        {
            foreach (var item in scanCodelist)
            {
                var ctrl = ActivatorUtilities.CreateInstance<ScanCodeCtrl>(_sp, item);
                ScanCodeCtrls.Add(item.Name ?? "", ctrl);
            }
        }
    }
}