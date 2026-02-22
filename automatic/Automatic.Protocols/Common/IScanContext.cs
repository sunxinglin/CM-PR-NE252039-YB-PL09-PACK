namespace Automatic.Protocols.Common
{
    public interface IScanContextWithHeartBeat
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTimeOffset CreatedAt { get; }
    }
}
