using Automatic.Entity.DataDtos;
using Automatic.Protocols.Common;
using Automatic.Protocols.HeatingFilmPressurize;
using Automatic.Protocols.HeatingFilmPressurize.Models.Wraps;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Automatic.Client.ViewModels.Realtime
{
    public class HeatingFilmPressurizeMonitorViewModel : ReactiveObject
    {
        public HeatingFilmPressurizeMonitorViewModel()
        {
            #region 通用状态

            //dev
            ScanContextSubject.Select(c => c.DevMsg.CmdFlags.HasFlag(DevMsg_GeneralCmdFlags.Heartbeat_Req)).ToPropertyEx(this, x => x.DevHeartBeatReq, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.DevMsg.CmdFlags.HasFlag(DevMsg_GeneralCmdFlags.Heartbeat_Answer)).ToPropertyEx(this, x => x.DevHeartBeatAck, scheduler: RxApp.MainThreadScheduler);

            ScanContextSubject.Select(c => c.DevMsg.Status.HasFlag(DevMsg_GeneralStatus.Auto)).ToPropertyEx(this, x => x.DevAuto, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.DevMsg.Status.HasFlag(DevMsg_GeneralStatus.Manual)).ToPropertyEx(this, x => x.DevManual, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.DevMsg.Status.HasFlag(DevMsg_GeneralStatus.EmgencyStop)).ToPropertyEx(this, x => x.DevStop, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.DevMsg.Status.HasFlag(DevMsg_GeneralStatus.Busy)).ToPropertyEx(this, x => x.DevBusy, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.DevMsg.Status.HasFlag(DevMsg_GeneralStatus.Idle)).ToPropertyEx(this, x => x.DevIdle, scheduler: RxApp.MainThreadScheduler);


            //mst
            ScanContextSubject.Select(c => c.MstMsg.CmdFlags.HasFlag(MstMsg_GeneralCmdFlags.HeartBeatReq)).ToPropertyEx(this, x => x.MstHeartBeatReq, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.MstMsg.CmdFlags.HasFlag(MstMsg_GeneralCmdFlags.Heartbeat_Answer)).ToPropertyEx(this, x => x.MstHeartBeatAck, scheduler: RxApp.MainThreadScheduler);

            #endregion

            #region 业务

            //进工位
            ScanContextSubject.Select(c => c.DevMsg.EnterStation.Flag.HasFlag(DevMsgFlag.Req)).ToPropertyEx(this, x => x.ReqVectorEnter, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.DevMsg.EnterStation.VectorCode).ToPropertyEx(this, x => x.EnterVectorCode, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.DevMsg.EnterStation.PackCode.EffectiveContent).ToPropertyEx(this, x => x.EnterPackCode, scheduler: RxApp.MainThreadScheduler);

            ScanContextSubject.Select(c => c.MstMsg.EnterStation.Flag.HasFlag(MstMsgFlag.Ack)).ToPropertyEx(this, x => x.VectorEnterAck, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.MstMsg.EnterStation.Flag.HasFlag(MstMsgFlag.OK)).ToPropertyEx(this, x => x.VectorEnterOK, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.MstMsg.EnterStation.Flag.HasFlag(MstMsgFlag.NG)).ToPropertyEx(this, x => x.VectorEnterNG, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.MstMsg.EnterStation.ErrorCode).ToPropertyEx(this, x => x.EnterErrorCode, scheduler: RxApp.MainThreadScheduler);


            //开始加压
            ScanContextSubject.Select(c => c.DevMsg.StartPressurize.Flag.HasFlag(DevMsgFlag.Req)).ToPropertyEx(this, x => x.ReqStart, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.DevMsg.StartPressurize.VectorCode).ToPropertyEx(this, x => x.StartVectorCode, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.DevMsg.StartPressurize.PackCode.EffectiveContent).ToPropertyEx(this, x => x.StartPackCode, scheduler: RxApp.MainThreadScheduler);

            ScanContextSubject.Select(c => c.MstMsg.StartPressurize.Flag.HasFlag(MstMsgFlag.Ack)).ToPropertyEx(this, x => x.StartAck, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.MstMsg.StartPressurize.Flag.HasFlag(MstMsgFlag.OK)).ToPropertyEx(this, x => x.StartOK, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.MstMsg.StartPressurize.Flag.HasFlag(MstMsgFlag.NG)).ToPropertyEx(this, x => x.StartNG, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.MstMsg.StartPressurize.ErrorCode).ToPropertyEx(this, x => x.StartErrorCode, scheduler: RxApp.MainThreadScheduler);


            //加压完成
            ScanContextSubject.Select(c => c.DevMsg.PressurizeComplete.Flag.HasFlag(DevMsgFlag.Req)).ToPropertyEx(this, x => x.ReqComplete, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.DevMsg.PressurizeComplete.VectorCode).ToPropertyEx(this, x => x.ComplateVectorCode, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.DevMsg.PressurizeComplete.PackCode.EffectiveContent).ToPropertyEx(this, x => x.ComplatePackCode, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.DevMsg.PressurizeComplete.StartTime).ToPropertyEx(this, x => x.StartTime, scheduler: RxApp.MainThreadScheduler);

            ScanContextSubject.Select(c => c.MstMsg.PressurizeComplete.Flag.HasFlag(MstMsgFlag.Ack)).ToPropertyEx(this, x => x.CompleteAck, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.MstMsg.PressurizeComplete.Flag.HasFlag(MstMsgFlag.OK)).ToPropertyEx(this, x => x.CompleteOK, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.MstMsg.PressurizeComplete.Flag.HasFlag(MstMsgFlag.NG)).ToPropertyEx(this, x => x.CompleteNG, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.MstMsg.PressurizeComplete.ErrorCode).ToPropertyEx(this, x => x.CompleteErrorCode, scheduler: RxApp.MainThreadScheduler);

            ReshapeWrapSubject.Select(s => s.PressurizeDatas).ToPropertyEx(this, x => x.PressurizeDatas, scheduler: RxApp.MainThreadScheduler);

            #endregion
        }

        public Subject<ScanContext> ScanContextSubject { get; } = new Subject<ScanContext>();

        public Subject<DealReqPressurizeCompleteWraps> ReshapeWrapSubject { get; set; } = new Subject<DealReqPressurizeCompleteWraps>();


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

        #region PLC
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
        public string StartTime { get; }
        #endregion

        #region MES
        [ObservableAsProperty]
        public bool VectorEnterAck { get; }
        [ObservableAsProperty]
        public bool VectorEnterOK { get; }
        [ObservableAsProperty]
        public bool VectorEnterNG { get; }
        [ObservableAsProperty]
        public ushort EnterErrorCode { get; }
        [ObservableAsProperty]
        public ushort EnterPressureLevel { get; }
        [ObservableAsProperty]
        public bool StartAck { get; }
        [ObservableAsProperty]
        public bool StartOK { get; }
        [ObservableAsProperty]
        public bool StartNG { get; }
        [ObservableAsProperty]
        public ushort StartErrorCode { get; }
        [ObservableAsProperty]
        public ushort StartPressureLevel { get; }
        [ObservableAsProperty]
        public bool CompleteAck { get; }
        [ObservableAsProperty]
        public bool CompleteOK { get; }
        [ObservableAsProperty]
        public bool CompleteNG { get; }
        [ObservableAsProperty]
        public ushort CompleteErrorCode { get; }

        [ObservableAsProperty]
        public Dictionary<string, object> PressurizeDatas { get; }
        #endregion
        #endregion
    }
}
