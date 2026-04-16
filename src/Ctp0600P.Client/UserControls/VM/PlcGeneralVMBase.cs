using System;

using Reactive.Bindings;

namespace Ctp0600P.Client.UserControls.VM;

public abstract class GeneralVMBase
{

    public virtual ReadOnlyReactiveProperty<bool> MstHeartbeat_Answer { get; protected set; }
    public virtual ReadOnlyReactiveProperty<bool> MstHeartBeatReq { get; protected set; }
    public virtual ReadOnlyReactiveProperty<bool> MstEmgencyStop_Req { get; protected set; }
    public virtual ReadOnlyReactiveProperty<bool> MstEmgencyStop_CompleteConfirm { get; protected set; }
    public virtual ReadOnlyReactiveProperty<bool> MstRecovery_Req { get; protected set; }
    public virtual ReadOnlyReactiveProperty<bool> MstRecovery_CompleteConfirm { get; protected set; }
    public virtual ReadOnlyReactiveProperty<bool> MstInitDataAckComplete { get; protected set; }

    public virtual ReadOnlyReactiveProperty<bool> Heartbeat_Answer { get; protected set; }
    public virtual ReadOnlyReactiveProperty<bool> Heartbeat_Req { get; protected set; }
    public virtual ReadOnlyReactiveProperty<bool> EmgencyStop_Confirmed { get; protected set; }
    public virtual ReadOnlyReactiveProperty<bool> EmgencyStop_Finish { get; protected set; }
    public virtual ReadOnlyReactiveProperty<bool> Recovery_Confirm { get; protected set; }
    public virtual ReadOnlyReactiveProperty<bool> Recovery_Complete { get; protected set; }
    public virtual ReadOnlyReactiveProperty<bool> Req_Init_Data { get; protected set; }



    //public virtual ReadOnlyReactiveProperty<bool> EmgencyStopAck { get; protected set; }
    //public virtual ReadOnlyReactiveProperty<bool> EmgencyStopFin { get; protected set; }
    //public virtual ReadOnlyReactiveProperty<bool> ResumeAck { get; protected set; }
    //public virtual ReadOnlyReactiveProperty<bool> ResumeFin { get; protected set; }




    public virtual ReadOnlyReactiveProperty<bool> AutoMode { get; protected set; }
    public virtual ReadOnlyReactiveProperty<bool> ManualMode { get; protected set; }
    public virtual ReadOnlyReactiveProperty<bool> EmgencyStop { get; protected set; }

    public virtual ReadOnlyReactiveProperty<bool> NotYetHoming { get; protected set; }
    public virtual ReadOnlyReactiveProperty<bool> Homing { get; protected set; }
    public virtual ReadOnlyReactiveProperty<bool> HomeCompleted { get; protected set; }
    public virtual ReadOnlyReactiveProperty<bool> Busy { get; protected set; }
    public virtual ReadOnlyReactiveProperty<bool> Idle { get; protected set; }

    public virtual ReadOnlyReactiveProperty<DateTime> LastBeatedAt { get; protected set; }
}