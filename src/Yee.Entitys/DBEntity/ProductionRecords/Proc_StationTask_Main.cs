using AsZero.Core.Entities;
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
    /// <summary>
    /// 工位任务表
    /// </summary>
    [Table("Proc_StationTask_Main")]
    public class Proc_StationTask_Main
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public long Timespan { get; set; }

        public bool IsDeleted { get; set; } = false;
        public bool IsModuleInBoxOFFAuto { get; set; } = false;

        public int ProductId { get; set; }

        [MaxLength(200)]
        public string? Remark { get; set; }

        public int PeiFang_MD5_ID { get; set; }

        public DateTime? CreateTime { get; set; } = DateTime.Now;
        public int? CreateUserID { get; set; }
        public User? CreateUser { get; set; }

        public DateTime? UpdateTime { get; set; }
        public int? UpdateUserID { get; set; }
        public User? UpdateUser { get; set; }
        public DateTime? DeleteTime { get; set; }
        public int? DeleteUserID { get; set; }
        public User? DeleteUser { get; set; }
        [NotMapped]
        public string? AGVCode { get; set; }
        /// <summary>
        /// 关联工位
        /// </summary>
        public Base_Station? Station { get; set; }
        public int StationId { get; set; }

        public Base_Step? Step { get; set; }
        public int? StepId { get; set; }
        /// <summary>
        /// Pack条码号
        /// </summary>
        [MaxLength(50)]
        public string PackCode { get; set; } = "";
        /// <summary>
        /// 完成状态
        /// </summary>
        public StationTaskStatusEnum Status { get; set; }

        [MaxLength(30)]
        public string StationCode { get; set; } = "";

        /// <summary>
        /// AGV车号
        /// </summary>
        [MaxLength(100)]
        public string? UseAGVCode { get; set; }

        /// <summary>
        /// 当前步骤号
        /// </summary>
        public int CurStepNo { get; set; }

        [NotMapped]
        public List<Proc_StationTask_Record> Proc_StationTask_Records { get; set; }

     

        [NotMapped]
        public List<Proc_AutoBoltInfo_Detail>? AutoBoltList { get; set; }


        /// <summary>
        /// 模组与Pack绑定关系
        /// </summary>
        [NotMapped]
        public List<Proc_ModuleInBox_ModuleRecord>? Proc_ModuleInBox_ModuleRecord { get; set; }
        /// <summary>
        /// 等离子清洗
        /// </summary>
   
        /// <summary>
        /// 模组入箱
        /// </summary>
        [NotMapped]
        public List<Proc_ModuleInBox_DataCollect>? Proc_ModuleInBox_DataCollect { get; set; }
        /// <summary>
        /// Pack加压
        /// </summary>
        [NotMapped]
        public List<Proc_PressureInfo>? PressureInfo { get; set; }

    }
}
