using Automatic.Protocols.Common;
using Automatic.Protocols.ShoulderGlue;
using Automatic.Protocols.ShoulderGlue.Models.Datas;
using Automatic.Protocols.ShoulderGlue.Models.Wraps;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Automatic.Client.ViewModels.Realtime
{
    public class ShoulderGlueMonitorViewModel : ReactiveObject
    {
        public ShoulderGlueMonitorViewModel()
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

            //进工位
            ScanContextSubject.Select(c => c.DevMsg.EnterStation.Flag.HasFlag(DevMsgFlag.Req)).ToPropertyEx(this, x => x.ReqVectorEnter, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.DevMsg.EnterStation.VectorCode).ToPropertyEx(this, x => x.EnterVectorCode, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.DevMsg.EnterStation.PackCode.EffectiveContent).ToPropertyEx(this, x => x.EnterPackCode, scheduler: RxApp.MainThreadScheduler);

            ScanContextSubject.Select(c => c.MstMsg.EnterStation.Flag.HasFlag(MstMsgFlag.Ack)).ToPropertyEx(this, x => x.VectorEnterAck, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.MstMsg.EnterStation.Flag.HasFlag(MstMsgFlag.OK)).ToPropertyEx(this, x => x.VectorEnterOK, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.MstMsg.EnterStation.Flag.HasFlag(MstMsgFlag.NG)).ToPropertyEx(this, x => x.VectorEnterNG, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.MstMsg.EnterStation.ErrorCode).ToPropertyEx(this, x => x.EnterErrorCode, scheduler: RxApp.MainThreadScheduler);


            //开始涂胶
            ScanContextSubject.Select(c => c.DevMsg.StartGlue.Flag.HasFlag(DevMsgFlag.Req)).ToPropertyEx(this, x => x.ReqStart, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.DevMsg.StartGlue.VectorCode).ToPropertyEx(this, x => x.StartVectorCode, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.DevMsg.StartGlue.PackCode.EffectiveContent).ToPropertyEx(this, x => x.StartPackCode, scheduler: RxApp.MainThreadScheduler);

            ScanContextSubject.Select(c => c.MstMsg.StartGlue.Flag.HasFlag(MstMsgFlag.Ack)).ToPropertyEx(this, x => x.StartGlueAck, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.MstMsg.StartGlue.Flag.HasFlag(MstMsgFlag.OK)).ToPropertyEx(this, x => x.StartGlueOK, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.MstMsg.StartGlue.Flag.HasFlag(MstMsgFlag.NG)).ToPropertyEx(this, x => x.StartGlueNG, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.MstMsg.StartGlue.ErrorCode).ToPropertyEx(this, x => x.StartErrorCode, scheduler: RxApp.MainThreadScheduler);


            //涂胶完成
            ScanContextSubject.Select(c => c.DevMsg.GlueComplete.Flag.HasFlag(DevMsgFlag.Req)).ToPropertyEx(this, x => x.ReqComplete, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.DevMsg.GlueComplete.VectorCode).ToPropertyEx(this, x => x.ComplateVectorCode, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.DevMsg.GlueComplete.PackCode.EffectiveContent).ToPropertyEx(this, x => x.ComplatePackCode, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.DevMsg.GlueComplete.GlueStartTime).ToPropertyEx(this, x => x.GlueStartTime, scheduler: RxApp.MainThreadScheduler);
            GlueDataSubject.Select(c => c.GlueDatas.ToList()).ToPropertyEx(this, x => x.GlueData, scheduler: RxApp.MainThreadScheduler);

            ScanContextSubject.Select(c => c.MstMsg.GlueComplete.Flag.HasFlag(MstMsgFlag.Ack)).ToPropertyEx(this, x => x.CompleteGlueAck, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.MstMsg.GlueComplete.Flag.HasFlag(MstMsgFlag.OK)).ToPropertyEx(this, x => x.CompleteGlueOK, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.MstMsg.GlueComplete.Flag.HasFlag(MstMsgFlag.NG)).ToPropertyEx(this, x => x.CompleteGlueNG, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.MstMsg.GlueComplete.ErrorCode).ToPropertyEx(this, x => x.ComplateErrorCode, scheduler: RxApp.MainThreadScheduler);


            //首件
            ScanContextSubject.Select(c => c.DevMsg.FirstArticle.Flag.HasFlag(DevMsgFlag.Req)).ToPropertyEx(this, x => x.ReqInspect, scheduler: RxApp.MainThreadScheduler);

            ScanContextSubject.Select(c => c.MstMsg.FirstArticle.Flag.HasFlag(MstMsgFlag.Ack)).ToPropertyEx(this, x => x.InspectAck, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.MstMsg.FirstArticle.Flag.HasFlag(MstMsgFlag.OK)).ToPropertyEx(this, x => x.InspectOk, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.MstMsg.FirstArticle.Flag.HasFlag(MstMsgFlag.NG)).ToPropertyEx(this, x => x.InspectNG, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.MstMsg.FirstArticle.ErrorCode).ToPropertyEx(this, x => x.InspectErrorCode, scheduler: RxApp.MainThreadScheduler);

        }

        public Subject<ScanContext> ScanContextSubject { get; } = new Subject<ScanContext>();
        public Subject<DealReqGlueCompleteWraps> GlueDataSubject { get; } = new Subject<DealReqGlueCompleteWraps>();

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
        public string GlueStartTime { get; }
        [ObservableAsProperty]
        public IList<GlueData> GlueData { get; }


        [ObservableAsProperty]
        public bool ReqInspect { get; }


        [ObservableAsProperty]
        public bool VectorEnterAck { get; }
        [ObservableAsProperty]
        public bool VectorEnterOK { get; }
        [ObservableAsProperty]
        public bool VectorEnterNG { get; }
        [ObservableAsProperty]
        public ushort EnterErrorCode { get; }
        [ObservableAsProperty]
        public ushort EnterGlueLevel { get; }

        [ObservableAsProperty]
        public bool StartGlueAck { get; }
        [ObservableAsProperty]
        public bool StartGlueOK { get; }
        [ObservableAsProperty]
        public bool StartGlueNG { get; }
        [ObservableAsProperty]
        public ushort StartErrorCode { get; }
        [ObservableAsProperty]
        public ushort StartGlueLevel { get; }



        [ObservableAsProperty]
        public bool CompleteGlueAck { get; }
        [ObservableAsProperty]
        public bool CompleteGlueOK { get; }
        [ObservableAsProperty]
        public bool CompleteGlueNG { get; }
        [ObservableAsProperty]
        public ushort ComplateErrorCode { get; }

        [ObservableAsProperty]
        public bool InspectAck { get; }
        [ObservableAsProperty]
        public bool InspectOk { get; }
        [ObservableAsProperty]
        public bool InspectNG { get; }
        [ObservableAsProperty]
        public ushort InspectErrorCode { get; }

        #endregion
    }
}
