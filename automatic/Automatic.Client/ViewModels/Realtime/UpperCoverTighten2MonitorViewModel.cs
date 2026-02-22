using Automatic.Protocols.Common;
using Automatic.Protocols.UpperCoverTighten2;
using Automatic.Protocols.UpperCoverTighten2.Models.Wraps;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Automatic.Client.ViewModels.Realtime
{
    public class UpperCoverTighten2MonitorViewModel : UpperCoverTightenMonitorViewModelBase
    {
        public UpperCoverTighten2MonitorViewModel()
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

            //进工位
            ScanContextSubject.Select(s => s.DevMsg.EnterStation.Flag.HasFlag(DevMsgFlag.Req)).ToPropertyEx(this, x => x.ReqVectorEnter, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(s => s.DevMsg.EnterStation.VectorCode).ToPropertyEx(this, x => x.ReqEnterVectorCode, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(s => s.DevMsg.EnterStation.PackCode.EffectiveContent).ToPropertyEx(this, x => x.ReqEnterPackCode, scheduler: RxApp.MainThreadScheduler);

            ScanContextSubject.Select(s => s.MstMsg.EnterStation.Flag.HasFlag(MstMsgFlag.Ack)).ToPropertyEx(this, x => x.AckVectorEnter, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(s => s.MstMsg.EnterStation.Flag.HasFlag(MstMsgFlag.OK)).ToPropertyEx(this, x => x.VectorEnterOK, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(s => s.MstMsg.EnterStation.Flag.HasFlag(MstMsgFlag.NG)).ToPropertyEx(this, x => x.VectorEnterNG, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(s => s.MstMsg.EnterStation.ErrorCode).ToPropertyEx(this, x => x.VectorEnterErrorCode, scheduler: RxApp.MainThreadScheduler);


            //开始拧紧
            ScanContextSubject.Select(s => s.DevMsg.StartTighten.Flag.HasFlag(DevMsgFlag.Req)).ToPropertyEx(this, x => x.ReqStartTighten, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(s => s.DevMsg.StartTighten.VectorCode).ToPropertyEx(this, x => x.StartTightenVectorCode, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(s => s.DevMsg.StartTighten.PackCode.EffectiveContent).ToPropertyEx(this, x => x.StartTightenPackCode, scheduler: RxApp.MainThreadScheduler);

            ScanContextSubject.Select(s => s.MstMsg.StartTighten.Flag.HasFlag(MstMsgFlag.Ack)).ToPropertyEx(this, x => x.AckStartTighten, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(s => s.MstMsg.StartTighten.Flag.HasFlag(MstMsgFlag.OK)).ToPropertyEx(this, x => x.StartTightenOK, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(s => s.MstMsg.StartTighten.Flag.HasFlag(MstMsgFlag.NG)).ToPropertyEx(this, x => x.StartTightenNG, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(s => s.MstMsg.StartTighten.ErrorCode).ToPropertyEx(this, x => x.StartTightenErrorCode, scheduler: RxApp.MainThreadScheduler);


            //拧紧完成
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
    }
}
