using System.Reactive.Linq;

using Ctp0600P.Client.ObservableMonitor;
using Ctp0600P.Client.PLC.Common;
using Ctp0600P.Client.PLC.PLC01.Models.Flags;

using Reactive.Bindings;

namespace Ctp0600P.Client.UserControls.PLC01;

public class PLC01MonitorViewModel
{
    public PLC01MonitorViewModel(PLC01ObservableMonitor monitor)
    {

        DEV_HeartBeat_Req = monitor.ContextSource.Select(x => x.DevMsg.CmdFlags.HasFlag(DevMsg_GeneralCmdFlags.HeartbeatReq)).ToReactiveProperty();
        MST_HeartBeat_Answer = monitor.ContextSource.Select(x => x.MstMsg.CmdFlags.HasFlag(MstMsg_GeneralCmdFlags.HeartBeatAnswer)).ToReactiveProperty();

        DEV_AutoMode = monitor.ContextSource.Select(x => x.DevMsg.Status.HasFlag(DevMsg_GeneralStatus.Auto)).ToReactiveProperty();
        DEV_ManualMode = monitor.ContextSource.Select(x => x.DevMsg.Status.HasFlag(DevMsg_GeneralStatus.Manual)).ToReactiveProperty();
        DEV_Fan = monitor.ContextSource.Select(x => x.DevMsg.Status.HasFlag(DevMsg_GeneralStatus.Fan)).ToReactiveProperty();
        DEV_Reset_Req = monitor.ContextSource.Select(x => x.DevMsg.Status.HasFlag(DevMsg_GeneralStatus.Reset)).ToReactiveProperty();
        DEV_LetGo_Req = monitor.ContextSource.Select(x => x.DevMsg.Status.HasFlag(DevMsg_GeneralStatus.LetGo)).ToReactiveProperty();
        MST_Alarm = monitor.ContextSource.Select(x => x.MstMsg.Status.HasFlag(MstMsg_GeneralStatus.Alarm)).ToReactiveProperty();
        MST_LetGo = monitor.ContextSource.Select(x => x.MstMsg.Status.HasFlag(MstMsg_GeneralStatus.LetGo)).ToReactiveProperty();
        MST_OverrunAlarm = monitor.ContextSource.Select(x => x.MstMsg.Status.HasFlag(MstMsg_GeneralStatus.OverrunAlarm)).ToReactiveProperty();
        MST_Reset_Ack = monitor.ContextSource.Select(x => x.MstMsg.Status.HasFlag(MstMsg_GeneralStatus.ResetConfirm)).ToReactiveProperty();
        MST_LetGo_Ack = monitor.ContextSource.Select(x => x.MstMsg.Status.HasFlag(MstMsg_GeneralStatus.LetGoConfirm)).ToReactiveProperty();

        //客户端请求开始拧紧
        DEV_StartTighten_Ack = monitor.ContextSource.Select(x => x.DevMsg.TightenStart.Flag.HasFlag(ResponseFlag.Ack)).ToReactiveProperty();
        DEV_StartTighten_OK = monitor.ContextSource.Select(x => x.DevMsg.TightenStart.Flag.HasFlag(ResponseFlag.OK)).ToReactiveProperty();
        DEV_StartTighten_NG = monitor.ContextSource.Select(x => x.DevMsg.TightenStart.Flag.HasFlag(ResponseFlag.NG)).ToReactiveProperty();
        DEV_StartTighten_EnableFail = monitor.ContextSource.Select(x => x.DevMsg.TightenStart.NGFlag.HasFlag(TightenStartNGFlag.EnableFail)).ToReactiveProperty();

        MST_StartTighten_Req = monitor.ContextSource.Select(x => x.MstMsg.TightenStart.Flag.HasFlag(RequestFlag.Req)).ToReactiveProperty();
        MST_StartTighten_DeviceNo = monitor.ContextSource.Select(x => x.MstMsg.TightenStart.DeviceNo).ToReactiveProperty();
        MST_StartTighten_DeviceBrand = monitor.ContextSource.Select(x => x.MstMsg.TightenStart.DeviceBrand).ToReactiveProperty();
        MST_StartTighten_ProgramNo = monitor.ContextSource.Select(x => x.MstMsg.TightenStart.ProgramNo).ToReactiveProperty();

        //PLC请求拧紧完成
        DEV_TightenComplete_Req = monitor.ContextSource.Select(x => x.DevMsg.TightenComplete.Flag.HasFlag(RequestFlag.Req)).ToReactiveProperty();
        DEV_TightenComplete_DeviceNo = monitor.ContextSource.Select(x => x.DevMsg.TightenComplete.DeviceNo).ToReactiveProperty();
        DEV_TightenComplete_DeviceBrand = monitor.ContextSource.Select(x => x.DevMsg.TightenComplete.DeviceBrand).ToReactiveProperty();
        DEV_TightenComplete_Result_PSet = monitor.ContextSource.Select(x => x.DevMsg.TightenComplete.TightenResult.Pset_selected).ToReactiveProperty();
        DEV_TightenComplete_Result_Torque = monitor.ContextSource.Select(x => x.DevMsg.TightenComplete.TightenResult.Final_torque).ToReactiveProperty();
        DEV_TightenComplete_Result_Angle = monitor.ContextSource.Select(x => x.DevMsg.TightenComplete.TightenResult.Final_angle).ToReactiveProperty();

        MST_TightenComplete_Ack = monitor.ContextSource.Select(x => x.MstMsg.TightenComplete.Flag.HasFlag(ResponseFlag.Ack)).ToReactiveProperty();
        DEV_TightenComplete_Result_OK = monitor.ContextSource.Select(x => x.MstMsg.TightenComplete.Flag.HasFlag(ResponseFlag.OK)).ToReactiveProperty();
        DEV_TightenComplete_Result_NG = monitor.ContextSource.Select(x => x.MstMsg.TightenComplete.Flag.HasFlag(ResponseFlag.NG)).ToReactiveProperty();

        //PLC请求AGV到站
        DEV_AGVArrive_Req = monitor.ContextSource.Select(x => x.DevMsg.AGVArrive.Flag.HasFlag(RequestFlag.Req)).ToReactiveProperty();
        DEV_AGVArrive_AGVNo = monitor.ContextSource.Select(x => x.DevMsg.AGVArrive.AGVNo).ToReactiveProperty();
        DEV_AGVArrive_PackCode = monitor.ContextSource.Select(x => x.DevMsg.AGVArrive.PackCode.EffectiveContent).ToReactiveProperty();
        DEV_AGVArrive_StationCode = monitor.ContextSource.Select(x => x.DevMsg.AGVArrive.StationCode.EffectiveContent).ToReactiveProperty();

        DEV_AGVArrive_Ack = monitor.ContextSource.Select(x => x.MstMsg.AGVArrive.Flag.HasFlag(ResponseFlag.Ack)).ToReactiveProperty();

        //PLC请求AGV离站
        DEV_AGVLeave_Req = monitor.ContextSource.Select(x => x.DevMsg.AGVLeave.Flag.HasFlag(RequestFlag.Req)).ToReactiveProperty();
        DEV_AGVLeave_AGVNo = monitor.ContextSource.Select(x => x.DevMsg.AGVLeave.AGVNo).ToReactiveProperty();
        DEV_AGVLeave_StationCode = monitor.ContextSource.Select(x => x.DevMsg.AGVLeave.StationCode.EffectiveContent).ToReactiveProperty();

        MST_AGVLeave_Ack = monitor.ContextSource.Select(x => x.MstMsg.AGVLeave.Flag.HasFlag(ResponseFlag.Ack)).ToReactiveProperty();

        //客户端请求AGV绑定Pack
        DEV_AGVBindPack_Ack = monitor.ContextSource.Select(x => x.DevMsg.AGVBindPack.Flag.HasFlag(ResponseFlag.Ack)).ToReactiveProperty();
        DEV_AGVBindPack_OK = monitor.ContextSource.Select(x => x.DevMsg.AGVBindPack.Flag.HasFlag(ResponseFlag.OK)).ToReactiveProperty();
        DEV_AGVBindPack_NG = monitor.ContextSource.Select(x => x.DevMsg.AGVBindPack.Flag.HasFlag(ResponseFlag.NG)).ToReactiveProperty();

        MST_AGVBindPack_Req = monitor.ContextSource.Select(x => x.MstMsg.AGVBindPack.Flag.HasFlag(RequestFlag.Req)).ToReactiveProperty();
        MST_AGVBindPack_AGVNo = monitor.ContextSource.Select(x => x.MstMsg.AGVBindPack.AGVNo).ToReactiveProperty();
        MST_AGVBindPack_Behavior = monitor.ContextSource.Select(x => x.MstMsg.AGVBindPack.Behavior).ToReactiveProperty();
        MST_AGVBindPack_PackCode = monitor.ContextSource.Select(x => x.MstMsg.AGVBindPack.PackCode.EffectiveContent).ToReactiveProperty();
        MST_AGVBindPack_HolderBarcode = monitor.ContextSource.Select(x => x.MstMsg.AGVBindPack.HolderBarcode.EffectiveContent).ToReactiveProperty();
        MST_AGVBindPack_StationCode = monitor.ContextSource.Select(x => x.MstMsg.AGVBindPack.StationCode.EffectiveContent).ToReactiveProperty();

        //客户端请求放行AGV
        MST_ReleaseAGV_Req = monitor.ContextSource.Select(x => x.MstMsg.ReleaseAGV.Flag.HasFlag(RequestFlag.Req)).ToReactiveProperty();

        DEV_ReleaseAGV_Ack = monitor.ContextSource.Select(x => x.DevMsg.ReleaseAGV.Flag.HasFlag(ResponseFlag.Ack)).ToReactiveProperty();

        //PLC给的AGV状态
        DEV_AGVStatus_AGVAllowEntry = monitor.ContextSource.Select(x => x.DevMsg.AGVStatus.Flag.HasFlag(AGVStatusFlag.AGVAllowEntry)).ToReactiveProperty();
        DEV_AGVStatus_AGVAllowLeave = monitor.ContextSource.Select(x => x.DevMsg.AGVStatus.Flag.HasFlag(AGVStatusFlag.AGVAllowLeave)).ToReactiveProperty();
        DEV_AGVStatus_AGVInStation = monitor.ContextSource.Select(x => x.DevMsg.AGVStatus.Flag.HasFlag(AGVStatusFlag.AGVInStation)).ToReactiveProperty();
        DEV_AGVStatus_AGVEntrying = monitor.ContextSource.Select(x => x.DevMsg.AGVStatus.Flag.HasFlag(AGVStatusFlag.AGVEntrying)).ToReactiveProperty();
        DEV_AGVStatus_AGVLeaving = monitor.ContextSource.Select(x => x.DevMsg.AGVStatus.Flag.HasFlag(AGVStatusFlag.AGVLeaving)).ToReactiveProperty();
        DEV_AGVStatus_AGVReqWriteBarcode = monitor.ContextSource.Select(x => x.DevMsg.AGVStatus.Flag.HasFlag(AGVStatusFlag.AGVReqWriteBarcode)).ToReactiveProperty();
        DEV_AGVStatus_AGVWriteBarcodeComplete = monitor.ContextSource.Select(x => x.DevMsg.AGVStatus.Flag.HasFlag(AGVStatusFlag.AGVWriteBarcodeComplete)).ToReactiveProperty();

        //PLC给的拧紧枪状态
        DEV_TightenGunStatus_Ready = monitor.ContextSource.Select(x => x.DevMsg.TightenGunStatus.Flag.HasFlag(TightenGunStatusFlag.Ready)).ToReactiveProperty();
        DEV_TightenGunStatus_Enabled = monitor.ContextSource.Select(x => x.DevMsg.TightenGunStatus.Flag.HasFlag(TightenGunStatusFlag.Enabled)).ToReactiveProperty();
        DEV_TightenGunStatus_Tightening = monitor.ContextSource.Select(x => x.DevMsg.TightenGunStatus.Flag.HasFlag(TightenGunStatusFlag.Tightening)).ToReactiveProperty();
        DEV_TightenGunStatus_Alarm = monitor.ContextSource.Select(x => x.DevMsg.TightenGunStatus.Flag.HasFlag(TightenGunStatusFlag.Alarm)).ToReactiveProperty();

    }


    public ReactiveProperty<bool> DEV_HeartBeat_Req { get; }
    public ReactiveProperty<bool> MST_HeartBeat_Answer { get; }

    public ReactiveProperty<bool> DEV_AutoMode { get; }
    public ReactiveProperty<bool> DEV_ManualMode { get; }
    public ReactiveProperty<bool> DEV_Fan { get; }
    public ReactiveProperty<bool> DEV_Reset_Req { get; }
    public ReactiveProperty<bool> DEV_LetGo_Req { get; }


    public ReactiveProperty<bool> MST_Alarm { get; }
    public ReactiveProperty<bool> MST_LetGo { get; }
    public ReactiveProperty<bool> MST_OverrunAlarm { get; }
    public ReactiveProperty<bool> MST_Reset_Ack { get; }
    public ReactiveProperty<bool> MST_LetGo_Ack { get; }


    //开始拧紧
    public ReactiveProperty<bool> DEV_StartTighten_Ack { get; }
    public ReactiveProperty<bool> DEV_StartTighten_OK { get; }
    public ReactiveProperty<bool> DEV_StartTighten_NG { get; }
    public ReactiveProperty<bool> DEV_StartTighten_EnableFail { get; }
    public ReactiveProperty<bool> MST_StartTighten_Req { get; }
    public ReactiveProperty<ushort> MST_StartTighten_DeviceNo { get; }
    public ReactiveProperty<ushort> MST_StartTighten_DeviceBrand { get; }
    public ReactiveProperty<ushort> MST_StartTighten_ProgramNo { get; }


    //拧紧完成
    public ReactiveProperty<bool> DEV_TightenComplete_Req { get; }
    public ReactiveProperty<ushort> DEV_TightenComplete_DeviceNo { get; }
    public ReactiveProperty<ushort> DEV_TightenComplete_DeviceBrand { get; }
    public ReactiveProperty<bool> DEV_TightenComplete_Result_OK { get; }
    public ReactiveProperty<bool> DEV_TightenComplete_Result_NG { get; }
    public ReactiveProperty<ushort> DEV_TightenComplete_Result_PSet { get; }
    public ReactiveProperty<float> DEV_TightenComplete_Result_Torque { get; }
    public ReactiveProperty<float> DEV_TightenComplete_Result_Angle { get; }
    public ReactiveProperty<bool> MST_TightenComplete_Ack { get; }

    //AGV到站
    public ReactiveProperty<bool> DEV_AGVArrive_Req { get; }
    public ReactiveProperty<ushort> DEV_AGVArrive_AGVNo { get; }
    public ReactiveProperty<string> DEV_AGVArrive_PackCode { get; }
    public ReactiveProperty<string> DEV_AGVArrive_StationCode { get; }
    public ReactiveProperty<bool> DEV_AGVArrive_Ack { get; }

    //AGV离站
    public ReactiveProperty<bool> DEV_AGVLeave_Req { get; }
    public ReactiveProperty<ushort> DEV_AGVLeave_AGVNo { get; }
    public ReactiveProperty<string> DEV_AGVLeave_StationCode { get; }

    public ReactiveProperty<bool> MST_AGVLeave_Ack { get; }

    //AGV绑定条码
    public ReactiveProperty<bool> DEV_AGVBindPack_Ack { get; }
    public ReactiveProperty<bool> DEV_AGVBindPack_OK { get; }
    public ReactiveProperty<bool> DEV_AGVBindPack_NG { get; }


    public ReactiveProperty<bool> MST_AGVBindPack_Req { get; }
    public ReactiveProperty<ushort> MST_AGVBindPack_AGVNo { get; }
    public ReactiveProperty<ushort> MST_AGVBindPack_Behavior { get; }
    public ReactiveProperty<string> MST_AGVBindPack_PackCode { get; }
    public ReactiveProperty<string> MST_AGVBindPack_HolderBarcode { get; }
    public ReactiveProperty<string> MST_AGVBindPack_StationCode { get; }

    //放行AGV
    public ReactiveProperty<bool> DEV_ReleaseAGV_Ack { get; }
    public ReactiveProperty<bool> DEV_ReleaseAGV_OK { get; }
    public ReactiveProperty<bool> DEV_ReleaseAGV_NG { get; }

    //AGV状态
    public ReactiveProperty<bool> DEV_AGVStatus_AGVAllowEntry { get; }
    public ReactiveProperty<bool> DEV_AGVStatus_AGVAllowLeave { get; }
    public ReactiveProperty<bool> DEV_AGVStatus_AGVInStation { get; }
    public ReactiveProperty<bool> DEV_AGVStatus_AGVEntrying { get; }
    public ReactiveProperty<bool> DEV_AGVStatus_AGVLeaving { get; }
    public ReactiveProperty<bool> DEV_AGVStatus_AGVReqWriteBarcode { get; }
    public ReactiveProperty<bool> DEV_AGVStatus_AGVWriteBarcodeComplete { get; }

    //拧紧枪状态
    public ReactiveProperty<bool> DEV_TightenGunStatus_Ready { get; }
    public ReactiveProperty<bool> DEV_TightenGunStatus_Enabled { get; }
    public ReactiveProperty<bool> DEV_TightenGunStatus_Tightening { get; }
    public ReactiveProperty<bool> DEV_TightenGunStatus_Alarm { get; }

    //MES请求离开
    public ReactiveProperty<bool> MST_ReleaseAGV_Req { get; }
}