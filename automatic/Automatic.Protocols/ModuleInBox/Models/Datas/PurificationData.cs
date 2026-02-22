using FutureTech.Protocols;
using System.Runtime.InteropServices;

namespace Automatic.Protocols.ModuleInBox.Models.Datas
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class PurificationData
    {
        /// <summary>
        /// 功率
        /// </summary>
        [Endian(Endianness.BigEndian)]
        public float PurificationPower;

        /// <summary>
        /// 转速
        /// </summary>
        [Endian(Endianness.BigEndian)]
        public float PurificationRevolution;

        /// <summary>
        /// 高度
        /// </summary>
        [Endian(Endianness.BigEndian)]
        public float PurificationHeight;

        /// <summary>
        /// 流量
        /// </summary>
        [Endian(Endianness.BigEndian)]
        public float PurificationVolume;

        /// <summary>
        /// 电流
        /// </summary>
        [Endian(Endianness.BigEndian)]
        public float PurificationCurrent;

        /// <summary>
        /// 模组长度
        /// </summary>
        [Endian(Endianness.BigEndian)]
        public float ModuleALenth;

        /// <summary>
        /// 
        /// </summary>
        [Endian(Endianness.BigEndian)]
        public float ModuleBLenth;

        /// <summary>
        /// 速度
        /// </summary>
        [Endian(Endianness.BigEndian)]
        public float PurificationSpeed;

    }
}
