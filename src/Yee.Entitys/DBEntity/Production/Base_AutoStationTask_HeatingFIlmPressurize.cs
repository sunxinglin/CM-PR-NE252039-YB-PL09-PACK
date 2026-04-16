using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Yee.Entitys.Common;

namespace Yee.Entitys.Production
{
    [Table("Base_AutoStationTask_HeatingFilmPressurize")]
    public class Base_AutoStationTask_HeatingFilmPressurize : CommonData
    {
        public Base_StationTask? StationTask { get; set; }
        public int? StationTaskId { get; set; }

        [MaxLength(100)]
        public string ParameterName { get; set; } = string.Empty;

        public string UpMesCode { get; set; } = string.Empty;

    }
}
