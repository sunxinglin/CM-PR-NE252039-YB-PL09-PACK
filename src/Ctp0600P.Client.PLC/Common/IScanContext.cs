namespace Ctp0600P.Client.PLC.Common
{
    public interface IScanContextWithHeartBeat
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTimeOffset CreatedAt { get; }
    }
}
