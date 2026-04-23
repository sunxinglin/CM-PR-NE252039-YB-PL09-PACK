using Ctp0600P.Client.ObservableMonitor;
using Ctp0600P.Client.PLC.Common;
using Ctp0600P.Client.PLC.PLC01.Models.Datas;
using Ctp0600P.Client.PLC.PLC01.Models.Flags;
using Reactive.Bindings;
using System.Linq;
using System.Reactive.Linq;

namespace Ctp0600P.Client.UserControls.PLC01
{
    public class LeakCoreVM
    {
        public LeakCoreVM(PLC01ObservableMonitor monitor)
        {
            // ========== DevMsg 设备消息层标志 ==========
            this.开始充气 = monitor.ContextSource.Select(x => x.DevMsg.LeakStart.Flag.HasFlag(ResponseFlag.Ack)).ToReactiveProperty();
            this.请求充气完成 = monitor.ContextSource.Select(x => x.DevMsg.LeakComplete.Flag.HasFlag(RequestFlag.Req)).ToReactiveProperty();

            // ========== LeakComplete 设备信息层数据 ==========
            this.充气开始时间 = monitor.ContextSource.Select(s => s.DevMsg.LeakComplete.LeakStartTime).ToReactiveProperty();
            this.充气结束时间 = monitor.ContextSource.Select(s => s.DevMsg.LeakComplete.LeakCompleteTime).ToReactiveProperty();
            this.充气时长 = monitor.ContextSource.Select(s => (int)s.DevMsg.LeakComplete.LeakTime).ToReactiveProperty();
            this.实时充气时长 = monitor.ContextSource.Select(s => (int)s.DevMsg.LeakComplete.LeakRealTime).ToReactiveProperty();
            this.保压时长 = monitor.ContextSource.Select(s => (int)s.DevMsg.LeakComplete.LeakKeepTime).ToReactiveProperty();
            this.充气压力 = monitor.ContextSource.Select(s => s.DevMsg.LeakComplete.LeakPressPower).ToReactiveProperty();
            this.保压压力 = monitor.ContextSource.Select(s => s.DevMsg.LeakComplete.LeakKeepPressPower).ToReactiveProperty();
            this.比例阀当前压力 = monitor.ContextSource.Select(s => s.DevMsg.LeakComplete.LeakProportionalPressure).ToReactiveProperty();
            this.亚德客当前压力 = monitor.ContextSource.Select(s => s.DevMsg.LeakComplete.LeakAdkerPressure).ToReactiveProperty();
            this.总气压力 = monitor.ContextSource.Select(s => s.DevMsg.LeakComplete.LeakTotalPower).ToReactiveProperty();

            // ========== LeakStart 主站消息层标志 ==========
            this.充气开始请求 = monitor.ContextSource.Select(s => s.MstMsg.LeakStart.Flag.HasFlag(RequestFlag.Req)).ToReactiveProperty();
            this.充气完成确认 = monitor.ContextSource.Select(s => s.MstMsg.LeakComplete.Flag.HasFlag(ResponseFlag.Ack)).ToReactiveProperty();

            // ========== LeakStart MES参数数据 ==========
            this.充气时长_MES = monitor.ContextSource.Select(s => s.MstMsg.LeakStart.LeakDuration).ToReactiveProperty();
            this.保压时长_MES = monitor.ContextSource.Select(s => s.MstMsg.LeakStart.PressureDuration).ToReactiveProperty();
            this.充气压力_MES = monitor.ContextSource.Select(s => s.MstMsg.LeakStart.LeakStress).ToReactiveProperty();
            this.保压压力_MES = monitor.ContextSource.Select(s => s.MstMsg.LeakStart.PressureStress).ToReactiveProperty();
        }


        public ReactiveProperty<bool> 开始充气 { get; }
        public ReactiveProperty<bool> 请求充气完成 { get; }
        public ReactiveProperty<bool> 充气结果OK { get; }
        public ReactiveProperty<bool> 充气结果NG { get; }
        public ReactiveProperty<string> 充气开始时间 { get; }
        public ReactiveProperty<string> 充气结束时间 { get; }
        public ReactiveProperty<int> 充气时长 { get; }
        public ReactiveProperty<int> 实时充气时长 { get; }
        public ReactiveProperty<int> 保压时长 { get; }
        public ReactiveProperty<float> 充气压力 { get; }
        public ReactiveProperty<float> 保压压力 { get; }
        public ReactiveProperty<float> 总气压力 { get; }
        public ReactiveProperty<float> 比例阀当前压力 { get; }
        public ReactiveProperty<float> 亚德客当前压力 { get; }

        public ReactiveProperty<bool> 充气开始请求 { get; }
        public ReactiveProperty<bool> 充气完成确认 { get; }
        public ReactiveProperty<ushort> 充气时长_MES { get; }
        public ReactiveProperty<ushort> 保压时长_MES { get; }
        public ReactiveProperty<float> 充气压力_MES { get; }
        public ReactiveProperty<float> 保压压力_MES { get; }
    }
}