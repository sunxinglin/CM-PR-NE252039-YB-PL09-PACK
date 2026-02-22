using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
//using System.Text;
//using System.Text.Json.Serialization;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Yee.Common.Library.CommonEnum;
using Yee.Entitys.Common;
using Yee.Entitys.Production;

namespace Yee.Entitys.DBEntity
{
    /// <summary>
    /// 涂胶信息表
    /// </summary>
    [Table("Proc_GluingInfo")]
    public class Proc_GluingInfo : CommonData
    {
        public Proc_StationTask_Main? Proc_StationTask_Main { get; set; }
        public int Proc_StationTask_MainId { get; set; }

        [MaxLength(50)]
        public string? PackPN { get; set; } = "";

        [MaxLength(30)]
        public string StationCode { get; set; } = "";

        public string GlueDataJson { get; set; } = "";

        [NotMapped]
        public IList<GlueDataSql> GlueDatas => JsonConvert.DeserializeObject<IList<GlueDataSql>>(GlueDataJson)!;
    }
    public class GlueDataSql
    {
        public string ParamName { get; set; } = "";
        public float Value { get; set; } = 0;

        public string UploadMesCode { get; set; } = "";

        public int GlueLocate { get; set; } = 1;
        public GlueType GlueType { get; set; }
    }
}
