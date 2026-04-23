namespace Ctp0600P.Client.Protocols.Leak.Models
{
    /// <summary>
    /// 充气完成请求数据
    /// </summary>
    public class LeakCompleteRequest
    {
        /// <summary>
        /// 充气结果是否OK
        /// </summary>
        public bool QualityOK { get; set; }

        /// <summary>
        /// 充气结果是否NG
        /// </summary>
        public bool QualityNG { get; set; }

        /// <summary>
        /// 充气开始时间
        /// </summary>
        public string LeakStartTime { get; set; }

        /// <summary>
        /// 充气结束时间
        /// </summary>
        public string LeakCompleteTime { get; set; }

        /// <summary>
        /// 充气时长(秒)
        /// </summary>
        public ushort LeakTime { get; set; }

        /// <summary>
        /// 保压时长(分钟)
        /// </summary>
        public ushort LeakKeepTime { get; set; }

        /// <summary>
        /// 充气压力
        /// </summary>
        public float LeakPressPower { get; set; }

        /// <summary>
        /// 保压压力
        /// </summary>
        public float LeakKeepPressPower { get; set; }

        /// <summary>
        /// 总气压力
        /// </summary>
        public float LeakTotalPower { get; set; }

        /// <summary>
        /// 比例阀当前压力
        /// </summary>
        public float LeakProportionalPressure { get; set; }

        /// <summary>
        /// 亚德客当前压力
        /// </summary>
        public float LeakAdkerPressure { get; set; }
    }
}