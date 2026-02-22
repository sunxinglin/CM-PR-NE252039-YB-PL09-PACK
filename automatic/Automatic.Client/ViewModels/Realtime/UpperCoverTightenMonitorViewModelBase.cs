using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using static Automatic.Entity.DataDtos.AutoTightenDataUploadDto;

namespace Automatic.Client.ViewModels.Realtime
{
    public class UpperCoverTightenMonitorViewModelBase : ReactiveObject
    {
        #region 通用状态字
        [ObservableAsProperty]
        public bool DevHeartBeatReq { get; }

        [ObservableAsProperty]
        public bool DevHeartBeatAck { get; }

        [ObservableAsProperty]
        public bool MstHeartBeatReq { get; }

        [ObservableAsProperty]
        public bool MstHeartBeatAck { get; }

        [ObservableAsProperty]
        public bool DevAuto { get; }

        [ObservableAsProperty]
        public bool DevManual { get; }

        [ObservableAsProperty]
        public bool DevStop { get; }

        [ObservableAsProperty]
        public bool DevBusy { get; }

        [ObservableAsProperty]
        public bool DevIdle { get; }
        #endregion

        #region 信号交互
        #region 小车请求进入
        [ObservableAsProperty]
        public bool ReqVectorEnter { get; }
        [ObservableAsProperty]
        public ushort ReqEnterVectorCode { get; }
        [ObservableAsProperty]
        public string ReqEnterPackCode { get; }
        [ObservableAsProperty]
        public bool AckVectorEnter { get; }
        [ObservableAsProperty]
        public bool VectorEnterOK { get; }
        [ObservableAsProperty]
        public bool VectorEnterNG { get; }
        [ObservableAsProperty]
        public ushort VectorEnterErrorCode { get; }
        #endregion

        #region 请求开始拧紧
        [ObservableAsProperty]
        public bool ReqStartTighten { get; }
        [ObservableAsProperty]
        public ushort StartTightenVectorCode { get; }
        [ObservableAsProperty]
        public string StartTightenPackCode { get; }
        [ObservableAsProperty]
        public bool AckStartTighten { get; }
        [ObservableAsProperty]
        public bool StartTightenOK { get; }
        [ObservableAsProperty]
        public bool StartTightenNG { get; }
        [ObservableAsProperty]
        public ushort StartTightenErrorCode { get; }

        #endregion

        #region 请求拧紧完成
        [ObservableAsProperty]
        public bool ReqComplateTighten { get; }
        [ObservableAsProperty]
        public ushort ComplateTightenVectorCode { get; }
        [ObservableAsProperty]
        public string ComplateTightenPackCode { get; }
        [ObservableAsProperty]
        public IList<AutoTightenDataUploadTightenItem> tighteningData { get; }

        [ObservableAsProperty]
        public bool AckComplateTighten { get; }
        [ObservableAsProperty]
        public bool ComplateTightenOK { get; }
        [ObservableAsProperty]
        public bool ComplateTightenNG { get; }
        [ObservableAsProperty]
        public ushort ComplateTightenErrorCode { get; }
        #endregion
        #endregion
    }
}
