using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Automatic.Client.ViewModels.Realtime
{
    public class ModuleInBoxMonitorViewModelBase : ReactiveObject
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

        #region 业务字段


        #region PLC信号

        [ObservableAsProperty]
        public bool ReqTakePhotoComplete { get; }
        [ObservableAsProperty]
        public string TakePhotoCompleteCellCode { get; }


        [ObservableAsProperty]
        public bool ReqVectorEnter { get; }




        [ObservableAsProperty]
        public ushort VectorEnterVectorCode { get; }
        [ObservableAsProperty]
        public string VectorEnterPackCode { get; } = "";
        [ObservableAsProperty]
        public ushort VectorNgCode { get; } = 0;

        [ObservableAsProperty]
        public bool ReqStartEnterInBox { get; }
        [ObservableAsProperty]
        public ushort StartInBoxVectorCode { get; }
        [ObservableAsProperty]
        public string StartInBoxPackCode { get; } = "";
        [ObservableAsProperty]
        public ushort StartNgCode { get; }


        [ObservableAsProperty]
        public bool ReqSingleInBoxComplete { get; }
        [ObservableAsProperty]
        public ushort SingleInBoxCompleteVectorCode { get; }
        [ObservableAsProperty]
        public string SingleInBoxCompletePackCode { get; } = string.Empty;
        [ObservableAsProperty]
        public string SingleInBoxCompleteModuleCode { get; } = string.Empty;
        [ObservableAsProperty]
        public ushort SingleInBoxCompleteModuleLocation { get; }



        [ObservableAsProperty]
        public bool ReqStartEnterInBoxComplete { get; }
        [ObservableAsProperty]
        public ushort ComplateInBoxVectorCode { get; }
        [ObservableAsProperty]
        public string ComplateInBoxPackCode { get; } = "";


        [ObservableAsProperty]
        public IList<ModuleInBoxViewData> ModuleDatas { get; }
        #endregion

        #region MES信号
        [ObservableAsProperty]
        public bool AckTakePhotoComplete { get; }
        [ObservableAsProperty]
        public bool TakePhotoCompleteOK { get; }
        [ObservableAsProperty]
        public ushort ModuleType { get; }
        [ObservableAsProperty]
        public string ModuleCode { get; } = "";

        [ObservableAsProperty]
        public bool TakePhotoCompleteNG { get; }
        [ObservableAsProperty]
        public ushort PhotoNgCode { get; }


        [ObservableAsProperty]
        public bool AckStartEnterInBox { get; }
        [ObservableAsProperty]
        public bool StartEnterInBoxOK { get; }

        [ObservableAsProperty]
        public bool StartEnterInBoxNG { get; }
        [ObservableAsProperty]
        public ushort StartEnterInBoxErrorCode { get; }


        [ObservableAsProperty]
        public bool AckSingleInBoxComplete { get; }
        [ObservableAsProperty]
        public bool SingleInBoxCompleteOK { get; }
        [ObservableAsProperty]
        public bool SingleInBoxCompleteNG { get; }
        [ObservableAsProperty]
        public ushort SingleInBoxCompleteErrorCode { get; }


        [ObservableAsProperty]
        public bool AckEnterInBoxComplete { get; }
        [ObservableAsProperty]
        public bool EnterInBoxCompleteOK { get; }
        [ObservableAsProperty]
        public bool EnterInBoxCompleteNG { get; }
        [ObservableAsProperty]
        public ushort EnterInBoxCompleteErrorCode { get; }

        [ObservableAsProperty]
        public bool AckVectorEnter { get; }
        [ObservableAsProperty]
        public bool VectorEnterOk { get; }
        [ObservableAsProperty]
        public bool VectorEnterNg { get; }
        [ObservableAsProperty]
        public uint GlueRemainDuration { get; }
        #endregion

        #endregion
    }

    public class ModuleInBoxViewData
    {
        public string ModuleCode { get; set; } = string.Empty;

        public uint PressurizeDuration { get; set; }

        public float ModuleLenth { get; set; }

        public float DownDistance { get; set; }

        public float DownPressure { get; set; }

        public float LeftPressure { get; set; }

        public float RightPressure { get; set; }

        public string CompleteTime { get; set; } = string.Empty;
    }

}
