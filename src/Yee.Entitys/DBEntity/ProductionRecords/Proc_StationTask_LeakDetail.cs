using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yee.Common.Library.CommonEnum;
using Yee.Entitys.Common;
using Yee.Entitys.Production;

namespace Yee.Entitys.DBEntity
{
    [Table("Proc_StationTask_LeakDetail")]
    public class Proc_StationTask_LeakDetail : CommonData
    {
        /// <summary>
        /// 关联工位恩物主表
        /// </summary>
        public Proc_StationTask_Leak? Proc_StationTask_Leak { get; set; }
        public int Proc_StationTask_LeakId { get; set; }

        public int? StationId { get; set; }
        public int? StepId { get; set; }

        /// <summary>
        /// 完成状态
        /// </summary>
        public StationTaskStatusEnum Status { get; set; }

        /// <summary>
        /// 设备号
        /// </summary>
        [MaxLength(200)]
        public int DeviceNo { get; set; }

        /// <summary>
        /// 参数名
        /// </summary>
        public string? ParamName { get; set; }

        /// <summary>
        /// 参数值
        /// </summary>
        public string? ParamValue { get; set; }

        /// <summary>
        /// 数据类型
        /// </summary>
        public ValueTypeEnum ParamType { get; set; }

        /// <summary>
        /// 上传MES代码
        /// </summary>
        public string? UploadCode { get; set; }

        /// <summary>
        /// 上传MES 追溯用
        /// </summary>
        public string? PackPN { get; set; }
    }
}