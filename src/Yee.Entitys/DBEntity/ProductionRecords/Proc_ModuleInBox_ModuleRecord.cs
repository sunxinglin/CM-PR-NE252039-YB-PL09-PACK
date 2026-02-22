using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Yee.Entitys.DBEntity
{
    /// <summary>
    /// 模组入箱组装数据
    /// </summary>
    [Table("Proc_ModuleInBox_ModuleRecord")]
    public class Proc_ModuleInBox_ModuleRecord
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public bool IsDeleted { get; set; } = false;

        [MaxLength(200)]
        public string? Remark { get; set; }

        public DateTime? DeleteTime { get; set; }

        [MaxLength(100)]
        public string? ModuleCode { get; set; }

        [MaxLength(100)]
        public string? PackCode { get; set; }

        public ModuleStatusEnum BlockStatus { get; set; } = ModuleStatusEnum.None;

        public DateTime? CreateTime { get; set; }

        public DateTime? UpdateTime { get; set; }

    }

    public enum ModuleStatusEnum
    {
        None,
        /// <summary>
        /// 未使用
        /// </summary>
        NotUsed,
        /// <summary>
        /// 已使用
        /// </summary>
        Used
    }
}
