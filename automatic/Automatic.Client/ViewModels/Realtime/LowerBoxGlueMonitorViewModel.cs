using Automatic.Protocols.Common;
using Automatic.Protocols.LowerBoxGlue;
using Automatic.Protocols.LowerBoxGlue.Models.Datas;
using Automatic.Protocols.LowerBoxGlue.Models.Wraps;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Automatic.Client.ViewModels.Realtime
{
    public class LowerBoxGlueMonitorViewModel : ReactiveObject
    {
        public LowerBoxGlueMonitorViewModel()
        {
            //this.Station1 = new Station1ViewModel(this);

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
            GlueDataSubject.Select(c => c.GlueDatas).ToPropertyEx(this, x => x.GlueDatas, scheduler: RxApp.MainThreadScheduler);

            ScanContextSubject.Select(c => c.MstMsg.GlueComplete.Flag.HasFlag(MstMsgFlag.Ack)).ToPropertyEx(this, x => x.CompleteGlueAck, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.MstMsg.GlueComplete.Flag.HasFlag(MstMsgFlag.OK)).ToPropertyEx(this, x => x.CompleteGlueOK, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.MstMsg.GlueComplete.Flag.HasFlag(MstMsgFlag.NG)).ToPropertyEx(this, x => x.CompleteGlueNG, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.MstMsg.GlueComplete.ErrorCode).ToPropertyEx(this, x => x.ComplateErrorCode, scheduler: RxApp.MainThreadScheduler);

            //补胶完成
            ScanContextSubject.Select(c => c.DevMsg.ReGlueComplete.Flag.HasFlag(DevMsgFlag.Req)).ToPropertyEx(this, x => x.ReqReGlueComplete, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.DevMsg.ReGlueComplete.VectorCode).ToPropertyEx(this, x => x.ReGlueCompleteVectorCode, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.DevMsg.ReGlueComplete.PackCode.EffectiveContent).ToPropertyEx(this, x => x.ReGlueCompletePackCode, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.DevMsg.ReGlueComplete.ReGlueStartTime).ToPropertyEx(this, x => x.ReGlueCompleteStartTime, scheduler: RxApp.MainThreadScheduler);

            ScanContextSubject.Select(c => c.MstMsg.ReGlueComplete.Flag.HasFlag(MstMsgFlag.Ack)).ToPropertyEx(this, x => x.ReGlueCompleteAck, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.MstMsg.ReGlueComplete.Flag.HasFlag(MstMsgFlag.OK)).ToPropertyEx(this, x => x.ReGlueCompleteOK, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.MstMsg.ReGlueComplete.Flag.HasFlag(MstMsgFlag.NG)).ToPropertyEx(this, x => x.ReGlueCompleteNG, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.MstMsg.ReGlueComplete.ErrorCode).ToPropertyEx(this, x => x.ReGlueCompleteErrorCode, scheduler: RxApp.MainThreadScheduler);


            //开始补胶
            ScanContextSubject.Select(c => c.DevMsg.ReGlueStart.Flag.HasFlag(DevMsgFlag.Req)).ToPropertyEx(this, x => x.ReqReGlueStart, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.DevMsg.ReGlueStart.VectorCode).ToPropertyEx(this, x => x.ReGlueStartVectorCode, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.DevMsg.ReGlueStart.PackCode.EffectiveContent).ToPropertyEx(this, x => x.ReGlueStartPackCode, scheduler: RxApp.MainThreadScheduler);

            ScanContextSubject.Select(c => c.MstMsg.ReGlueStart.Flag.HasFlag(MstMsgFlag.Ack)).ToPropertyEx(this, x => x.ReGlueStartAck, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.MstMsg.ReGlueStart.Flag.HasFlag(MstMsgFlag.OK)).ToPropertyEx(this, x => x.ReGlueStartOK, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.MstMsg.ReGlueStart.Flag.HasFlag(MstMsgFlag.NG)).ToPropertyEx(this, x => x.ReGlueStartNG, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.MstMsg.ReGlueStart.ErrorCode).ToPropertyEx(this, x => x.ReGlueStartErrorCode, scheduler: RxApp.MainThreadScheduler);


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
        public Dictionary<string, object> GlueDatas { get; }

        [ObservableAsProperty]
        public bool ReqReGlueComplete { get; }
        [ObservableAsProperty]
        public ushort ReGlueCompleteVectorCode { get; }
        [ObservableAsProperty]
        public string ReGlueCompletePackCode { get; }
        [ObservableAsProperty]
        public string ReGlueCompleteStartTime { get; }

        [ObservableAsProperty]
        public bool ReqReGlueStart { get; }
        [ObservableAsProperty]
        public ushort ReGlueStartVectorCode { get; }
        [ObservableAsProperty]
        public string ReGlueStartPackCode { get; }

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


        [ObservableAsProperty]
        public bool ReGlueCompleteAck { get; }
        [ObservableAsProperty]
        public bool ReGlueCompleteOK { get; }
        [ObservableAsProperty]
        public bool ReGlueCompleteNG { get; }
        [ObservableAsProperty]
        public ushort ReGlueCompleteErrorCode { get; }

        [ObservableAsProperty]
        public bool ReGlueStartAck { get; }
        [ObservableAsProperty]
        public bool ReGlueStartOK { get; }
        [ObservableAsProperty]
        public bool ReGlueStartNG { get; }
        [ObservableAsProperty]
        public ushort ReGlueStartErrorCode { get; }


        #endregion
    }
}
