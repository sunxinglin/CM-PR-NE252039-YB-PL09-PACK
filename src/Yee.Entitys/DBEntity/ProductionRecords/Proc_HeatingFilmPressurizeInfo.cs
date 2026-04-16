using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Newtonsoft.Json;

using Yee.Common.Library.CommonEnum;
using Yee.Entitys.Common;

namespace Yee.Entitys.DBEntity
{
    [Table("Proc_HeatingFilmPressurizeInfo")]
    public class Proc_HeatingFilmPressurizeInfo : CommonData
    {
        public Proc_StationTask_Main? Proc_StationTask_Main { get; set; }
        public int Proc_StationTask_MainId { get; set; }

        public int AGVNo { get; set; }

        [MaxLength(40)]
        public string PackPN { get; set; } = string.Empty;

        [MaxLength(30)]
        public string StationCode { get; set; } = string.Empty;

        public string PressurizeDataJson { get; set; } = string.Empty;

        [NotMapped]
        public IList<HeatingFilmPressurizeData> PressurizeDatas => JsonConvert.DeserializeObject<IList<HeatingFilmPressurizeData>>(PressurizeDataJson)!;
    }


    public class HeatingFilmPressurizeData
    {
        public string ParamName { get; set; } = string.Empty;
        public string UploadCode { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public ValueTypeEnum ValueType { get; set; }
    }
}
