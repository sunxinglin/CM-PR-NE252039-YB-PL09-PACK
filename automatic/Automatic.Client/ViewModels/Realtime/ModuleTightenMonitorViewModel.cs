using Automatic.Protocols.Common;
using Automatic.Protocols.ModuleTighten;
using Automatic.Protocols.ModuleTighten.Models.Wraps;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using static Automatic.Entity.DataDtos.AutoTightenDataUploadDto;

namespace Automatic.Client.ViewModels.Realtime
{
    public class ModuleTightenMonitorViewModel : ReactiveObject
    {
        public ModuleTightenMonitorViewModel()
        {
            #region 通用状态

            //dev
            ScanContextSubject.Select(c => c.DevMsg.CmdFlags.HasFlag(DevMsg_GeneralCmdFlags.Heartbeat_Req))
                .ToPropertyEx(this, x => x.DevHeartBeatReq, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.DevMsg.CmdFlags.HasFlag(DevMsg_GeneralCmdFlags.Heartbeat_Answer))
                .ToPropertyEx(this, x => x.DevHeartBeatAck, scheduler: RxApp.MainThreadScheduler);

            ScanContextSubject.Select(c => c.DevMsg.Status.HasFlag(DevMsg_GeneralStatus.Auto))
                .ToPropertyEx(this, x => x.DevAuto, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.DevMsg.Status.HasFlag(DevMsg_GeneralStatus.Manual))
                .ToPropertyEx(this, x => x.DevManual, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.DevMsg.Status.HasFlag(DevMsg_GeneralStatus.EmgencyStop))
                .ToPropertyEx(this, x => x.DevStop, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.DevMsg.Status.HasFlag(DevMsg_GeneralStatus.Busy))
                .ToPropertyEx(this, x => x.DevBusy, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.DevMsg.Status.HasFlag(DevMsg_GeneralStatus.Idle))
                .ToPropertyEx(this, x => x.DevIdle, scheduler: RxApp.MainThreadScheduler);
            //mst
            ScanContextSubject.Select(c => c.MstMsg.CmdFlags.HasFlag(MstMsg_GeneralCmdFlags.HeartBeatReq))
                .ToPropertyEx(this, x => x.MstHeartBeatReq, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.MstMsg.CmdFlags.HasFlag(MstMsg_GeneralCmdFlags.Heartbeat_Answer))
                .ToPropertyEx(this, x => x.MstHeartBeatAck, scheduler: RxApp.MainThreadScheduler);

            #endregion

            #region 信号交互
            ScanContextSubject.Select(s => s.DevMsg.EnterStation.Flag.HasFlag(DevMsgFlag.Req)).ToPropertyEx(this, x => x.ReqVectorEnter, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(s => s.DevMsg.EnterStation.VectorCode).ToPropertyEx(this, x => x.ReqEnterVectorCode, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(s => s.DevMsg.EnterStation.PackCode.EffectiveContent).ToPropertyEx(this, x => x.ReqEnterPackCode, scheduler: RxApp.MainThreadScheduler);

            ScanContextSubject.Select(s => s.MstMsg.EnterStation.Flag.HasFlag(MstMsgFlag.Ack)).ToPropertyEx(this, x => x.AckVectorEnter, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(s => s.MstMsg.EnterStation.Flag.HasFlag(MstMsgFlag.OK)).ToPropertyEx(this, x => x.VectorEnterOK, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(s => s.MstMsg.EnterStation.Flag.HasFlag(MstMsgFlag.NG)).ToPropertyEx(this, x => x.VectorEnterNG, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(s => s.MstMsg.EnterStation.ErrorCode).ToPropertyEx(this, x => x.VectorEnterErrorCode, scheduler: RxApp.MainThreadScheduler);



            ScanContextSubject.Select(s => s.DevMsg.StartTighten.Flag.HasFlag(DevMsgFlag.Req)).ToPropertyEx(this, x => x.ReqStartTighten, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(s => s.DevMsg.StartTighten.VectorCode).ToPropertyEx(this, x => x.StartTightenVectorCode, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(s => s.DevMsg.StartTighten.PackCode.EffectiveContent).ToPropertyEx(this, x => x.StartTightenPackCode, scheduler: RxApp.MainThreadScheduler);

            ScanContextSubject.Select(s => s.MstMsg.StartTighten.Flag.HasFlag(MstMsgFlag.Ack)).ToPropertyEx(this, x => x.AckStartTighten, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(s => s.MstMsg.StartTighten.Flag.HasFlag(MstMsgFlag.OK)).ToPropertyEx(this, x => x.StartTightenOK, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(s => s.MstMsg.StartTighten.Flag.HasFlag(MstMsgFlag.NG)).ToPropertyEx(this, x => x.StartTightenNG, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(s => s.MstMsg.StartTighten.ErrorCode).ToPropertyEx(this, x => x.StartTightenErrorCode, scheduler: RxApp.MainThreadScheduler);


            ScanContextSubject.Select(s => s.DevMsg.TightenComplete.Flag.HasFlag(DevMsgFlag.Req)).ToPropertyEx(this, x => x.ReqComplateTighten, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(s => s.DevMsg.TightenComplete.VectorCode).ToPropertyEx(this, x => x.ComplateTightenVectorCode, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(s => s.DevMsg.TightenComplete.PackCode.EffectiveContent).ToPropertyEx(this, x => x.ComplateTightenPackCode, scheduler: RxApp.MainThreadScheduler);
            TightenDataSubject.Select(s => s.TightenDatas.ToList()).ToPropertyEx(this, x => x.tighteningData, scheduler: RxApp.MainThreadScheduler);


            ScanContextSubject.Select(s => s.MstMsg.TightenComplete.Flag.HasFlag(MstMsgFlag.Ack)).ToPropertyEx(this, x => x.AckComplateTighten, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(s => s.MstMsg.TightenComplete.Flag.HasFlag(MstMsgFlag.OK)).ToPropertyEx(this, x => x.ComplateTightenOK, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(s => s.MstMsg.TightenComplete.Flag.HasFlag(MstMsgFlag.NG)).ToPropertyEx(this, x => x.ComplateTightenNG, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(s => s.MstMsg.TightenComplete.ErrorCode).ToPropertyEx(this, x => x.ComplateTightenErrorCode, scheduler: RxApp.MainThreadScheduler);

            #endregion

        }
        public Subject<ScanContext> ScanContextSubject { get; set; } = new Subject<ScanContext>();
        public Subject<DealReqTightenCompleteWraps> TightenDataSubject { get; set; } = new Subject<DealReqTightenCompleteWraps>();

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
        [ObservableAsProperty]
        public ushort VectorEnterTightenLevel { get; }
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
        [ObservableAsProperty]
        public ushort StartTightenLevel { get; }

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
