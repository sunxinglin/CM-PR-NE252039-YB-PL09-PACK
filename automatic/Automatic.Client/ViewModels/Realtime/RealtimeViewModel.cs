using Microsoft.Extensions.Options;
using ReactiveUI;
using Splat;
using StdUnit.Sharp7.Options;

namespace Automatic.Client.ViewModels.Realtime
{
    public class RealtimeViewModel : ReactiveObject, IRoutableViewModel
    {
        public RealtimeViewModel(
            IScreen screen,
            IOptionsMonitor<S7ScanOpt> scanOptsMonitor,

            UILogsViewModel uilogsViewModel,
            LowerBoxGlueMonitorViewModel lowerBoxGlueMonitorViewModel,
            PressureStripPressurizeMonitorViewModel pressureStripMonitorViewModel,
            ShoulderGlueMonitorViewModel shoulderGlueMonitorViewModel,
            ModuleInBox1MonitorViewModel moduleInBoxMonitorViewModel,
            ModuleTightenMonitorViewModel moduleTightenMonitorViewModel,
            TerminalReshapeMonitorViewModel terminalReshapeMonitorViewModel,
            UpperCoverTighten1MonitorViewModel upperCoverTighten1MonitorViewModel,
            UpperCoverTighten2MonitorViewModel upperCoverTighten2MonitorViewModel,
            HeatingFilmPressurizeMonitorViewModel heatingFilmPressurizeMonitorViewModel
            )
        {
            HostScreen = screen ?? Locator.Current.GetService<IScreen>();
            _scanOptsMonitor = scanOptsMonitor;

            UIlogsViewModel = uilogsViewModel;
            LowerBoxGlueMonitorViewModel = lowerBoxGlueMonitorViewModel;
            UpperCoverTighten1MonitorViewModel = upperCoverTighten1MonitorViewModel;
            UpperCoverTighten2MonitorViewModel = upperCoverTighten2MonitorViewModel;
            PressureStripMonitorViewModel = pressureStripMonitorViewModel;
            ShoulderGlueMonitorViewModel = shoulderGlueMonitorViewModel;
            ModuleTightenMonitorViewModel = moduleTightenMonitorViewModel;
            ModuleInBoxMonitorViewModel = moduleInBoxMonitorViewModel;
            TerminalReshapeMonitorViewModel = terminalReshapeMonitorViewModel;
            HeatingFilmPressurizeMonitorViewModel = heatingFilmPressurizeMonitorViewModel;
        }

        public UILogsViewModel UIlogsViewModel { get; }
        public LowerBoxGlueMonitorViewModel LowerBoxGlueMonitorViewModel { get; }
        public UpperCoverTighten1MonitorViewModel UpperCoverTighten1MonitorViewModel { get; }
        public UpperCoverTighten2MonitorViewModel UpperCoverTighten2MonitorViewModel { get; }
        public PressureStripPressurizeMonitorViewModel PressureStripMonitorViewModel { get; }
        public HeatingFilmPressurizeMonitorViewModel HeatingFilmPressurizeMonitorViewModel { get; }
        public ShoulderGlueMonitorViewModel ShoulderGlueMonitorViewModel { get; }
        public ModuleTightenMonitorViewModel ModuleTightenMonitorViewModel { get; }
        public ModuleInBox1MonitorViewModel ModuleInBoxMonitorViewModel { get; }
        public TerminalReshapeMonitorViewModel TerminalReshapeMonitorViewModel { get; }

        #region Routing
        public string UrlPathSegment => UrlDefines.URL_Realtime;

        public IScreen HostScreen { get; }

        public readonly IOptionsMonitor<S7ScanOpt> _scanOptsMonitor;
        #endregion

    }
}
