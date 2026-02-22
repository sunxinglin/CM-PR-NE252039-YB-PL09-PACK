using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Yee.Common.Library.Protocols
{
    /// <summary>
    /// OP030 PLC响应
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class PLCMsg_OP030
    {
        public const int DB_INDEX = 200;
        public const int DB_OFFSET = 2000;

        /// <summary>
        /// 通用控制
        /// </summary>
        public PlcMsg_General General;

        /// <summary>
        /// 涂胶控制
        /// </summary>
        public PlcFlags_TuJiao CmdWord;

        public float A胶压力;

        public float B胶压力;

        public float A胶实际胶量;

        public float B胶实际胶量;

    }
}
