using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Yee.Entitys.DBEntity.Common;

namespace Yee.Entitys.DBEntity.ProductionRecords
{
    /// <summary>
    /// 模组抓取记录
    /// </summary>
    [Table("Proc_ModuleInBox_GrapRecord")]
    public class Proc_ModuleInBox_GrapRecord : BaseDataModel
    {

        [MaxLength(40)]
        public string CellCode { get; set; } = "";

        [MaxLength(40)]
        public string ModuleCode { get; set; } = "";

        [MaxLength(40)]
        public string ModulePN { get; set; } = string.Empty;

        public bool HasUsed { get; set; } = false;

        public DateTime GrabTime { get; set; } = DateTime.UtcNow;//抓取时间

        [MaxLength(40)]
        public string StationCode { get; set; } = "";

        [MaxLength(40)]
        public string? PackCode { get; set; }

        /// <summary>
        /// 模组放置在Pack中的位置
        /// </summary>
        public int? ModuleLocation { get; set; }
    }
}
