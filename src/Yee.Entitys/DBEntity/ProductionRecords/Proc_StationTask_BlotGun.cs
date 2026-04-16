using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Yee.Common.Library.CommonEnum;
using Yee.Entitys.Common;

namespace Yee.Entitys.DBEntity
{
    [Table("Proc_StationTask_BlotGun")]
    public class Proc_StationTask_BlotGun : CommonData
    {
        /// <summary>
        /// 关联工位恩物主表
        /// </summary>
        public Proc_StationTask_Record? StationTask_Record { get; set; }
        public int StationTask_RecordId { get; set; }
        public int Base_ScrewTaskId { get; set; }
        
        public StationTaskStatusEnum Status { get; set; }

        /// <summary>
        /// 程序号
        /// </summary>
        public int? ProgramNo { get; set; }

        /// <summary>
        /// 螺丝名
        /// </summary>
        [MaxLength(200)]
        public string? ScrewName { get; set; }
        public int UseNum { get; set; }
        /// <summary>
        /// 当前已完成数
        /// </summary>
        public int CurCompleteNum { get; set; } = 0;

        [NotMapped]
        public List<Proc_StationTask_BlotGunDetail> Proc_StationTask_BlotGunDetails { get; set; }
    }
}
