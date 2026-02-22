using Automatic.Protocols.Common;
using Automatic.Protocols.ModuleInBox;
using Automatic.Protocols.ModuleInBox.Models.Datas;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Automatic.Client.ViewModels.Realtime
{
    public class ModuleInBox1MonitorViewModel : ModuleInBoxMonitorViewModelBase
    {
        public ModuleInBox1MonitorViewModel()
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


            //扫电芯码完成
            ScanContextSubject.Select(c => c.DevMsg.TakePhotoComplete.Flag.HasFlag(DevMsgFlag.Req)).ToPropertyEx(this, x => x.ReqTakePhotoComplete, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.DevMsg.TakePhotoComplete.CellCode.EffectiveContent).ToPropertyEx(this, x => x.TakePhotoCompleteCellCode, scheduler: RxApp.MainThreadScheduler);

            ScanContextSubject.Select(c => c.MstMsg.TakePhotoComplete.Flag.HasFlag(MstMsgFlag.Ack)).ToPropertyEx(this, x => x.AckTakePhotoComplete, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.MstMsg.TakePhotoComplete.Flag.HasFlag(MstMsgFlag.OK)).ToPropertyEx(this, x => x.TakePhotoCompleteOK, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.MstMsg.TakePhotoComplete.Flag.HasFlag(MstMsgFlag.NG)).ToPropertyEx(this, x => x.TakePhotoCompleteNG, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.MstMsg.TakePhotoComplete.ErrorCode).ToPropertyEx(this, x => x.PhotoNgCode, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.MstMsg.TakePhotoComplete.ModuleType).ToPropertyEx(this, x => x.ModuleType, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.MstMsg.TakePhotoComplete.ModuleCode.EffectiveContent).ToPropertyEx(this, x => x.ModuleCode, scheduler: RxApp.MainThreadScheduler);


            //进工位
            ScanContextSubject.Select(c => c.DevMsg.EnterStation.Flag.HasFlag(DevMsgFlag.Req)).ToPropertyEx(this, x => x.ReqVectorEnter, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.DevMsg.EnterStation.VectorCode).ToPropertyEx(this, x => x.VectorEnterVectorCode, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.DevMsg.EnterStation.PackCode.EffectiveContent).ToPropertyEx(this, x => x.VectorEnterPackCode, scheduler: RxApp.MainThreadScheduler);

            ScanContextSubject.Select(c => c.MstMsg.EnterStation.Flag.HasFlag(MstMsgFlag.Ack)).ToPropertyEx(this, x => x.AckVectorEnter, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.MstMsg.EnterStation.Flag.HasFlag(MstMsgFlag.OK)).ToPropertyEx(this, x => x.VectorEnterOk, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.MstMsg.EnterStation.Flag.HasFlag(MstMsgFlag.NG)).ToPropertyEx(this, x => x.VectorEnterNg, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.MstMsg.EnterStation.GlueRemainDuration).ToPropertyEx(this, x => x.GlueRemainDuration, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.MstMsg.EnterStation.ErrorCode).ToPropertyEx(this, x => x.VectorNgCode, scheduler: RxApp.MainThreadScheduler);


            //开始入箱
            ScanContextSubject.Select(c => c.DevMsg.StartInBox.Flag.HasFlag(DevMsgFlag.Req)).ToPropertyEx(this, x => x.ReqStartEnterInBox, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.DevMsg.StartInBox.VectorCode).ToPropertyEx(this, x => x.StartInBoxVectorCode, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.DevMsg.StartInBox.PackCode?.EffectiveContent).ToPropertyEx(this, x => x.StartInBoxPackCode, scheduler: RxApp.MainThreadScheduler);

            ScanContextSubject.Select(c => c.MstMsg.StartInBox.Flag.HasFlag(MstMsgFlag.Ack)).ToPropertyEx(this, x => x.AckStartEnterInBox, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.MstMsg.StartInBox.Flag.HasFlag(MstMsgFlag.OK)).ToPropertyEx(this, x => x.StartEnterInBoxOK, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.MstMsg.StartInBox.Flag.HasFlag(MstMsgFlag.NG)).ToPropertyEx(this, x => x.StartEnterInBoxNG, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.MstMsg.StartInBox.ErrorCode).ToPropertyEx(this, x => x.StartNgCode, scheduler: RxApp.MainThreadScheduler);


            //单个入完成
            ScanContextSubject.Select(c => c.DevMsg.SingleInBoxComplete.Flag.HasFlag(DevMsgFlag.Req)).ToPropertyEx(this, x => x.ReqSingleInBoxComplete, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.DevMsg.SingleInBoxComplete.VectorCode).ToPropertyEx(this, x => x.SingleInBoxCompleteVectorCode, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.DevMsg.SingleInBoxComplete.PackCode?.EffectiveContent).ToPropertyEx(this, x => x.SingleInBoxCompletePackCode, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.DevMsg.SingleInBoxComplete.ModuleCode?.EffectiveContent).ToPropertyEx(this, x => x.SingleInBoxCompleteModuleCode, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.DevMsg.SingleInBoxComplete.ModuleLocation).ToPropertyEx(this, x => x.SingleInBoxCompleteModuleLocation, scheduler: RxApp.MainThreadScheduler);

            ScanContextSubject.Select(c => c.MstMsg.SingleInBoxComplete.Flag.HasFlag(MstMsgFlag.Ack)).ToPropertyEx(this, x => x.AckSingleInBoxComplete, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.MstMsg.SingleInBoxComplete.Flag.HasFlag(MstMsgFlag.OK)).ToPropertyEx(this, x => x.SingleInBoxCompleteOK, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.MstMsg.SingleInBoxComplete.Flag.HasFlag(MstMsgFlag.NG)).ToPropertyEx(this, x => x.SingleInBoxCompleteNG, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.MstMsg.SingleInBoxComplete.ErrorCode).ToPropertyEx(this, x => x.SingleInBoxCompleteErrorCode, scheduler: RxApp.MainThreadScheduler);


            //入箱完成
            ScanContextSubject.Select(c => c.DevMsg.InBoxComplete.Flag.HasFlag(DevMsgFlag.Req)).ToPropertyEx(this, x => x.ReqStartEnterInBoxComplete, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.DevMsg.InBoxComplete.VectorCode).ToPropertyEx(this, x => x.ComplateInBoxVectorCode, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.DevMsg.InBoxComplete.PackCode?.EffectiveContent).ToPropertyEx(this, x => x.ComplateInBoxPackCode, scheduler: RxApp.MainThreadScheduler);

            ScanContextSubject.Select(c => c.MstMsg.InBoxComplete.Flag.HasFlag(MstMsgFlag.Ack)).ToPropertyEx(this, x => x.AckEnterInBoxComplete, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.MstMsg.InBoxComplete.Flag.HasFlag(MstMsgFlag.OK)).ToPropertyEx(this, x => x.EnterInBoxCompleteOK, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.MstMsg.InBoxComplete.Flag.HasFlag(MstMsgFlag.NG)).ToPropertyEx(this, x => x.EnterInBoxCompleteNG, scheduler: RxApp.MainThreadScheduler);
            ScanContextSubject.Select(c => c.MstMsg.InBoxComplete.ErrorCode).ToPropertyEx(this, x => x.EnterInBoxCompleteErrorCode, scheduler: RxApp.MainThreadScheduler);

            ModuleInBoxDataSubject.Select(s => s.Select(x => new ModuleInBoxViewData
            {
                ModuleCode = x.ModuleCode.EffectiveContent,
                PressurizeDuration = x.PressurizeDuration,
                ModuleLenth = x.ModuleLenth,
                DownDistance = x.DownDistance,
                DownPressure = x.DownPressure,
                LeftPressure = x.LeftPressure,
                RightPressure = x.RightPressure,
                CompleteTime = x.CompleteTime,
            }).ToList()).ToPropertyEx(this, x => x.ModuleDatas, scheduler: RxApp.MainThreadScheduler);

        }

        public Subject<ScanContext> ScanContextSubject { get; } = new Subject<ScanContext>();

        public Subject<IList<ModuleInBoxData>> ModuleInBoxDataSubject { get; } = new Subject<IList<ModuleInBoxData>>();


    }
}
