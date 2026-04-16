using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Yee.Entitys.Common;

namespace Yee.Entitys.DBEntity.ProductionRecords
{
    /// <summary>
    /// 涂胶信息表
    /// </summary>
    [Table("Proc_LowerBoxGlueInfo")]
    public class Proc_LowerBoxGlueInfo : CommonData
    {
        public Proc_StationTask_Main? Proc_StationTask_Main { get; set; }
        public int Proc_StationTask_MainId { get; set; }

        [MaxLength(50)]
        public string? PackPN { get; set; } = string.Empty;

        [MaxLength(30)]
        public string StationCode { get; set; } = string.Empty;

        public string ParamName { get; set; } = string.Empty;

        public string Value { get; set; } = string.Empty;

        public string UploadMesCode { get; set; } = string.Empty;
        //public string GlueDataJson { get; set; } = string.Empty;
    }
    //public class LowerBoxGlueDataSql
    //{
    //    public string ParamName { get; set; } = string.Empty;
    //    public float Value { get; set; } = 0;
    //    public string UploadMesCode { get; set; } = string.Empty;
    //    public int GlueLocate { get; set; } = 1;
    //    public GlueType GlueType { get; set; }
    //}
}
