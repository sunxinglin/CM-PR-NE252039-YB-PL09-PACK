using Automatic.Entity.DataDtos;
using Automatic.Protocols.Common;
using Automatic.Protocols.TerminalReshape;
using Automatic.Protocols.TerminalReshape.Models.WorkRequireFlags;
using Automatic.Protocols.TerminalReshape.Models.Wraps;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Automatic.Client.ViewModels.Realtime
{
    public class TerminalReshapeMonitorViewModel : ReactiveObject
    {
        public TerminalReshapeMonitorViewModel()
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

            //业务
            ScanContextSubject.Select(c => c.DevMsg.DevMsgReqVectorEnterFlag.HasFlag(DevMsgReqVectorEnterFlag.ReqVectorEnter)).ToPropertyEx(this, x => x.ReqVectorEnter, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.DevMsg.EnterVectorCode).ToPropertyEx(this, x => x.EnterVectorCode, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.DevMsg.EnterPackCode.EffectiveContent).ToPropertyEx(this, x => x.EnterPackCode, scheduler: RxApp.MainThreadScheduler);

            ScanContextSubject.Select(c => c.DevMsg.DevMsgReqStartReshapeFlag.HasFlag(DevMsgReqStartReshapeFlag.ReqStartReshape)).ToPropertyEx(this, x => x.ReqStart, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.DevMsg.StartVectorCode).ToPropertyEx(this, x => x.StartVectorCode, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.DevMsg.StartPackCode.EffectiveContent).ToPropertyEx(this, x => x.StartPackCode, scheduler: RxApp.MainThreadScheduler);

            ScanContextSubject.Select(c => c.DevMsg.DevMsgReqComplateReshapeFlag.HasFlag(DevMsgReqComplateReshapeFlag.ReqCompleteReshape)).ToPropertyEx(this, x => x.ReqComplete, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.DevMsg.ComplateVectorCode).ToPropertyEx(this, x => x.ComplateVectorCode, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.DevMsg.ComplatePackCode.EffectiveContent).ToPropertyEx(this, x => x.ComplatePackCode, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.DevMsg.ReshapeStartTime).ToPropertyEx(this, x => x.ReshapeStartTime, scheduler: RxApp.MainThreadScheduler);


            ScanContextSubject.Select(c => c.MstMsg.MstMsgAckVectorEnterFlag.HasFlag(MstMsgAckVectorEnterFlag.AckVectorEnter)).ToPropertyEx(this, x => x.VectorEnterAck, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.MstMsg.MstMsgAckVectorEnterFlag.HasFlag(MstMsgAckVectorEnterFlag.VectorEnterOK)).ToPropertyEx(this, x => x.VectorEnterOK, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.MstMsg.MstMsgAckVectorEnterFlag.HasFlag(MstMsgAckVectorEnterFlag.VectorEnterNG)).ToPropertyEx(this, x => x.VectorEnterNG, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.MstMsg.EnterErrorCode).ToPropertyEx(this, x => x.EnterErrorCode, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.MstMsg.EnterReshapeLevel).ToPropertyEx(this, x => x.EnterReshapeLevel, scheduler: RxApp.MainThreadScheduler);

            ScanContextSubject.Select(c => c.MstMsg.MstMsgAckStartReshapeFlag.HasFlag(MstMsgAckStartReshapeFlag.AckStartReshape)).ToPropertyEx(this, x => x.StartReshapeAck, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.MstMsg.MstMsgAckStartReshapeFlag.HasFlag(MstMsgAckStartReshapeFlag.StartReshapeOK)).ToPropertyEx(this, x => x.StartReshapeOK, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.MstMsg.MstMsgAckStartReshapeFlag.HasFlag(MstMsgAckStartReshapeFlag.StartReshapeNG)).ToPropertyEx(this, x => x.StartReshapeNG, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.MstMsg.StartErrorCode).ToPropertyEx(this, x => x.StartErrorCode, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.MstMsg.StartReshapeLevel).ToPropertyEx(this, x => x.StartReshapeLevel, scheduler: RxApp.MainThreadScheduler);

            ScanContextSubject.Select(c => c.MstMsg.MstMsgAckComplateReshapeFlag.HasFlag(MstMsgAckComplateReshapeFlag.AckCompleteReshape)).ToPropertyEx(this, x => x.CompleteReshapeAck, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.MstMsg.MstMsgAckComplateReshapeFlag.HasFlag(MstMsgAckComplateReshapeFlag.CompleteReshapeOK)).ToPropertyEx(this, x => x.CompleteReshapeOK, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.MstMsg.MstMsgAckComplateReshapeFlag.HasFlag(MstMsgAckComplateReshapeFlag.CompleteReshapeNG)).ToPropertyEx(this, x => x.CompleteReshapeNG, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.MstMsg.ComplateErrorCode).ToPropertyEx(this, x => x.ComplateErrorCode, scheduler: RxApp.MainThreadScheduler);

            ReshapeWrapSubject.Select(c => c.PressureValue).ToPropertyEx(this, x => x.PressureValues, scheduler: RxApp.MainThreadScheduler);
        }
        public Subject<ScanContext> ScanContextSubject { get; set; } = new Subject<ScanContext>();
        public Subject<DealReqCompleteReshapeWraps> ReshapeWrapSubject { get; set; } = new Subject<DealReqCompleteReshapeWraps>();
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

        [ObservableAsProperty]
        public bool ReqVectorEnter { get; }
        [ObservableAsProperty]
        public ushort EnterVectorCode { get; }
        [ObservableAsProperty]
        public string EnterPackCode { get; }

        [ObservableAsProperty]
        public bool ReqStart { get; }
        [ObservableAsProperty]
        public ushort StartVectorCode { get; }
        [ObservableAsProperty]
        public string StartPackCode { get; }


        [ObservableAsProperty]
        public bool ReqComplete { get; }
        [ObservableAsProperty]
        public ushort ComplateVectorCode { get; }
        [ObservableAsProperty]
        public string ComplatePackCode { get; }

        [ObservableAsProperty]
        public string ReshapeStartTime { get; }

        [ObservableAsProperty]
        public bool VectorEnterAck { get; }
        [ObservableAsProperty]
        public bool VectorEnterOK { get; }
        [ObservableAsProperty]
        public bool VectorEnterNG { get; }
        [ObservableAsProperty]
        public ushort EnterErrorCode { get; }
        [ObservableAsProperty]
        public ushort EnterReshapeLevel { get; }

        [ObservableAsProperty]
        public bool StartReshapeAck { get; }
        [ObservableAsProperty]
        public bool StartReshapeOK { get; }
        [ObservableAsProperty]
        public bool StartReshapeNG { get; }
        [ObservableAsProperty]
        public ushort StartErrorCode { get; }
        [ObservableAsProperty]
        public ushort StartReshapeLevel { get; }

        [ObservableAsProperty]
        public bool CompleteReshapeAck { get; }
        [ObservableAsProperty]
        public bool CompleteReshapeOK { get; }
        [ObservableAsProperty]
        public bool CompleteReshapeNG { get; }
        [ObservableAsProperty]
        public ushort ComplateErrorCode { get; }

        [ObservableAsProperty]
        public IList<PressureValue> PressureValues { get; }

        #endregion
    }
}
