using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yee.Common.Library.CommonEnum
{
    public enum StationTaskTypeEnum
    {
        // 人工站通用
        扫描员工卡 = 1,
        扫码 = 2,
        人工拧螺丝 = 3,
        扫码输入 = 4,
        用户输入 = 5,
        超时检测 = 6,
        时间记录 = 7,
        称重 = 8,
        补拧 = 9,
        放行 = 10,

        //自动站特殊
        涂胶检测 = 31,
        模组入箱 = 32,
        自动涂胶 = 33,
        自动拧紧 = 34,
        自动加压 = 35,

        下箱体涂胶 = 101,
    }
}
