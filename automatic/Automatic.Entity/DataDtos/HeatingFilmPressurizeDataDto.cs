using MediatR;

using System.ComponentModel;

namespace Automatic.Entity.DataDtos
{
    public class HeatingFilmPressurizeDataDto
    {
        public string Pin { get; set; } = string.Empty;

        public int VectorCode { get; set; } 

        public string StepCode { get; set; } = string.Empty;

        public string StationCode { get; set; } = string.Empty;

        public string StartTime { get; set; } = string.Empty;

        public Dictionary<string, object> PressureDatas { get; set; } = null!;
    }
}
