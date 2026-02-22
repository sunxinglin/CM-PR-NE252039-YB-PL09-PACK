using MediatR;

namespace Ctp0600P.Client.PLC.PLC01.Models.Notifications
{
    public class StationPLCStatusNotification : INotification
    {
        /// <summary>
        /// 自动
        /// </summary>
        public bool AutoMode { get; set; }
        /// <summary>
        /// 手动
        /// </summary>
        public bool ManualMode { get; set; }
        /// <summary>
        /// 复位
        /// </summary>
        public bool Reset { get; set; }
        /// <summary>
        /// 放行
        /// </summary>
        public bool LetGo { get; set; }
        /// <summary>
        /// 风扇
        /// </summary>
        public bool Fan { get; set; }
    }
}
