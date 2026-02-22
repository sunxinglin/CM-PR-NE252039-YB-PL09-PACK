using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yee.Common.Library.CommonEnum;
using Yee.Entitys.Common;
using Yee.Entitys.DBEntity.ProductionRecords;
using Yee.Entitys.Production;

namespace Yee.Entitys.DBEntity
{
    [Table("Proc_StationTask_Record")]
    public class Proc_StationTask_Record : CommonData
    {
        /// <summary>
        /// 关联工位任务主表
        /// </summary>
        public Proc_StationTask_Main? Proc_StationTask_Main { get; set; }
        public int Proc_StationTask_MainId { get; set; }

        public Base_StationTask? Base_StationTask { get; set; }
        public int? Base_StationTaskId { get; set; }

        /// <summary>
        /// 任务名
        /// </summary>
        public string TaskName { get; set; }
        public string? UseAGVCode { get; set; }


        /// <summary>
        /// 完成状态
        /// </summary>
        public StationTaskStatusEnum Status { get; set; }
        /// <summary>
        /// 扫码任务
        /// </summary>
        [NotMapped]
        public List<Proc_StationTask_Bom>? Proc_StationTask_Boms { get; set; }
        /// <summary>
        /// 称重任务
        /// </summary>
        [NotMapped]
        public Proc_StationTask_AnyLoad? Proc_StationTask_AnyLoad { get; set; }
        /// <summary>
        /// 扫码员工卡
        /// </summary>
        [NotMapped]
        public Proc_StationTask_ScanAccountCard? Proc_StationTask_ScanAccountCard { get; set; }
        /// <summary>
        /// 用户输入
        /// </summary>
        [NotMapped]
        public Proc_StationTask_UserInput? Proc_StationTask_UserInput { get; set; }
        /// <summary>
        /// 扫码输入
        /// </summary>
        [NotMapped]
        public Proc_StationTask_ScanCollect? Proc_StationTask_ScanCollect { get; set; }
        /// <summary>
        /// 人工拧紧任务
        /// </summary>
        [NotMapped]
        public List<Proc_StationTask_BlotGun>? Proc_StationTask_BlotGuns { get; set; }

        /// <summary>
        /// 涂胶超时时间
        /// </summary>
        [NotMapped]
        public Proc_StationTask_CheckTimeout? Proc_StationTask_GluingTime { get; set; }

        /// <summary>
        /// 静置时长
        /// </summary>
        [NotMapped]
        public Proc_StationTask_TimeRecord? Proc_StationTask_StewingTime { get; set; }
    }
}
