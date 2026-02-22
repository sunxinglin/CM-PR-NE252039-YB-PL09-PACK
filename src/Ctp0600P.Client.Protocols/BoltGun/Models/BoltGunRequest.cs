using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ctp0600P.Client.Protocols.BoltGun.Models
{
    public class BoltGunRequest
    {
        public string IpAddress { get; set; }
        public string DeviceNo { get; set; }
        public int DeviceBrand { get; set; }
        public string BoltGunValue { get; set; }
        public string BoltGunState { get; set; }

        /// <summary>
        ///  拧紧结果
        /// </summary>
        public bool ResultIsOK;

        /// <summary>
        ///  程序号
        /// </summary>
        public int ProgramNo;

        /// <summary>
        /// 角度值
        /// </summary>
        public decimal FinalAngle;

        /// <summary>
        /// 角度结果：0 偏小； 1 正好； 2偏大；
        /// </summary>
        public int AngleStatus;

        /// <summary>
        /// 最小角度
        /// </summary>
        public decimal Angle_Min;
        /// <summary>
        /// 最大角度
        /// </summary>
        public decimal Angle_Max;

        /// <summary>
        /// 目标角度
        /// </summary>
        public decimal TargetAngle;


        /// <summary>
        /// 最小扭矩
        /// </summary>
        public decimal TorqueRate_Min;

        /// <summary>
        /// 目标扭矩
        /// </summary>
        public decimal TargetTorqueRate;

        /// <summary>
        /// 最大扭矩
        /// </summary>
        public decimal TorqueRate_Max;

        /// <summary>
        /// 扭矩值
        /// </summary>
        public decimal FinalTorque;

        /// <summary>
        /// 扭矩结果：0 偏小； 1 正好； 2偏大；
        /// </summary>
        public int TorqueStatus;

        public int TightenID;
    }
}
