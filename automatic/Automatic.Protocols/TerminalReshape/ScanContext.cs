using Automatic.Protocols.Common;
using Automatic.Protocols.TerminalReshape.Models;
using Itminus.Middlewares;

using Newtonsoft.Json;

using StdUnit.Sharp7.Options;

namespace Automatic.Protocols.TerminalReshape
{
    /// <summary>
    /// 上下文
    /// </summary>
    public class ScanContext : IWorkContext, IScanContext<DevMsg, MstMsg>, IScanContextWithHeartBeat
    {
        public ScanContext(IServiceProvider sp, DevMsg devmsg, MstMsg mstmsg, DateTimeOffset createdAt)
        {
            ServiceProvider = sp;
            DevMsg = devmsg;
            MstMsg = mstmsg;
            CreatedAt = createdAt;
        }

        /// <summary>
        /// 只读属性
        /// </summary>
        public DevMsg DevMsg { get; }

        /// <summary>
        /// 只读属性
        /// </summary>
        public MstMsg MstMsg { get; }

        [JsonIgnore]
        public IServiceProvider ServiceProvider { get; }

        public DateTimeOffset CreatedAt { get; }
    }

}
