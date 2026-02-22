using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJZK.IOController
{
    public enum IOBoxStatusEnum
    {
        //DI
        风扇反馈 = 1000,
        _24V空开 = 1001,
        手动 = 1002,
        自动 = 1003,
        放行 = 1004,
        复位 = 1005,
        照明电源空开 = 1006,
        _24V电源 = 1007,

        //DO
        StartSignl = 0,
        ResetSignl = 1,
        RedLed = 2,
        YellowLed = 3,
        GreenLed = 4,
        Beep = 5,
        预留1 = 6,
        预留2 = 7,

        套筒1 = 8,
        套筒2 = 9,
        套筒3 = 10,
        预留6 = 11,
        预留7 = 12,
        预留8 = 13,
        预留9 = 14,
        预留10 = 15,

        StartSpecialSignl = 40,
        RedLedSpecial = 41,
    }
}
