using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Newtonsoft.Json;

using Yee.Entitys.Common;

namespace Yee.Entitys.DBEntity
{
    /// <summary>
    /// 涂胶信息表
    /// </summary>
    [Table("Proc_PressureInfo")]
    public class Proc_PressureInfo : CommonData
    {
        public Proc_StationTask_Main? Proc_StationTask_Main { get; set; }
        public int Proc_StationTask_MainId { get; set; }

        public int AGVNo { get; set; }

        [MaxLength(40)]
        public string PackPN { get; set; } = string.Empty;

        [MaxLength(30)]
        public string StationCode { get; set; } = string.Empty;

        public string PressureDataJson { get; set; } = string.Empty;

        [NotMapped]
        public IList<PressureData> PressureDatas => JsonConvert.DeserializeObject<IList<PressureData>>(PressureDataJson)!;
    }


    public class PressureData
    {
        public string ParamName { get; set; } = "";

        public int Locate { get; set; } = 1;

        public string UpMesCode { get; set; } = "";

        public float Value { get; set; }

    }
}
