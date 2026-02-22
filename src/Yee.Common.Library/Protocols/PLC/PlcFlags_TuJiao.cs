using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yee.Common.Library.Protocols
{
    public enum PlcFlags_TuJiao : ushort
    {
        None = 0,

        AGV允许进OP030光栅 = 1 << 0,
      
        AGV已到达OP030工位确认 = 1 << 1,

        AGV允许离开OP030光栅 = 1 << 2,

        AGV已离开OP030光栅确认 = 1 << 3,

        涂胶完成 = 1 << 4,

        涂胶完成_成功 = 1 << 5,

        涂胶完成_失败 = 1 << 6,

        人工涂胶完成 = 1 << 7,

        人工涂胶完成_成功 = 1 << 8,

        人工涂胶完成_失败 = 1 << 9,
    }
}
