using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yee.Common.Library.CommonEnum;
using Yee.Entitys.Common;

namespace Yee.Entitys.Production
{
    [Table("Base_ProResource")]
    public class Base_ProResource : CommonData
    {
        /// <summary>
        /// 资源号
        /// </summary>
        [MaxLength(200)]
        public string? Code { get; set; }

        /// <summary>
        /// 资源名
        /// </summary>
        [MaxLength(200)]
        public string? Name { get; set; }
        public ProResourceTypeEnum ProResourceType { get; set; }
        public ProtocolTypeEnum ProtocolType { get; set; }

        /// <summary>
        /// 波特率 ProtocolType =RS232/RS485时使用
        /// </summary>
        public int Baud { get; set; }

        /// <summary>
        /// IP地址 ProtocolType =RS232、RS485、ModbusTCP、TCP/IP时使用
        /// </summary>
        [MaxLength(200)]
        public string? IpAddress { get; set; }

        /// <summary>
        /// 端口 ProtocolType =TCP/IP、ModbusTCP时使用时使用
        /// </summary>
        [MaxLength(200)]
        public string? Port { get; set; }

        [MaxLength(200)]
        public string? Description { get; set; }

        public string? StationCode { get; set; }

        public DeviceBrand DeviceBrand { get; set; } = DeviceBrand.博世;
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnable { get; set; }

        /// <summary>
        /// 设备号
        /// </summary>
        public string DeviceNo { get; set; } = "";

        /// <summary>
        /// 是否勾选
        /// </summary>
        [NotMapped]
        public bool IsSelected { get; set; }

    }
    public  enum DeviceBrand
    {
        霍尼韦尔 =1,
        马头 = 11,
        博世 = 12,
        Anyload = 21,
    }
}
