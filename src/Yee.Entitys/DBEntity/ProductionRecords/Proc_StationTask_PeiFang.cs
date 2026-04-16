using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Yee.Entitys.DBEntity
{
    /// <summary>
    /// 工位配方 JSON 和 MD5哈希值 mapping表
    /// </summary>
    [Table("Proc_StationTask_PeiFang")]
    public class Proc_StationTask_PeiFang
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public string PeiFang_Json { get; set; }

        [MaxLength(40)]
        public string PeiFang_MD5 { get; set; }
    }
}
