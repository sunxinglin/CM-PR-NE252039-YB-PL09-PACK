namespace Ctp0600P.Client.PLC.Context
{
    public class StationPLCContext
    {
        /// <summary>
        /// 报警状态
        /// </summary>
        public bool Alarm { get; set; } = false;

        /// <summary>
        /// 可放行状态
        /// </summary>
        public bool LetGo { get; set; } = false;

        /// <summary>
        /// 超时报警状态
        /// </summary>
        public bool OverrunAlarm { get; set; } = false;

        /// <summary>
        /// 初始化状态
        /// </summary>
        public bool Initial { get; set; } = false;

        /// <summary>
        /// 拧紧请求数组
        /// </summary>
        public List<StationTightenStartReq> StationTightenStartReqs { get; set; } = new();

        /// <summary>
        /// AGV绑定Pack请求数组
        /// </summary>
        public AGVBindPackReqData AGVBindPackReq { get; set; } = new();

        /// <summary>
        /// 放行AGV请求数组
        /// </summary>
        public List<ReleaseAGVReq> ReleaseAGVReqs { get; set; } = new();


        public class StationTightenStartReq
        {
            /// <summary>
            /// 设备编号
            /// </summary>
            public ushort DeviceNo { get; set; } = 0;
            /// <summary>
            /// 设备品牌。1.马头 2.博世
            /// </summary>
            public ushort DeviceBrand { get; set; } = 0;
            /// <summary>
            /// 程序号
            /// </summary>
            public ushort ProgramNo { get; set; } = 0;
            /// <summary>
            /// 拧紧序号
            /// </summary>
            public ushort SortNo { get; set; } = 0;
            /// <summary>
            /// 当前任务之前的拧紧数量
            /// </summary>
            public int ScrewCountBeforeCurrentTask { get; set; } = 0;
            /// <summary>
            /// 来源
            /// </summary>
            public string Resource { get; set; } = string.Empty;
        }

        public class AGVBindPackReqData
        {
            public bool Req { get; set; } = false;
            /// <summary>
            /// 操作行为。绑定1，解绑2
            /// </summary>
            public ushort Behavior { get; set; } = 0;
            /// <summary>
            /// 小车号
            /// </summary>
            public ushort AGVNo { get; set; } = 0;
            /// <summary>
            /// pack码
            /// </summary>
            public string PackCode { get; set; } = string.Empty;
            /// <summary>
            /// 台车码
            /// </summary>
            public string HolderBarcode { get; set; } = string.Empty;
            /// <summary>
            /// 工站编号
            /// </summary>
            public string StationCode { get; set; } = string.Empty;
        }

        public class ReleaseAGVReq { }

    }
}
