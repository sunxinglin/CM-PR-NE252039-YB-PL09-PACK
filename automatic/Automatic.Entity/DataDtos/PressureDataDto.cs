using MediatR;

using System.ComponentModel;

namespace Automatic.Entity.DataDtos
{
    public class PressureDataDto
    {
        public string Pin { get; set; } = string.Empty;

        public int VectorCode { get; set; } 

        public string StepCode { get; set; } = string.Empty;

        public string StationCode { get; set; } = string.Empty;

        public string StartTime { get; set; } = string.Empty;

        public Dictionary<string, object> PressureData { get; set; } = null!;
    }

    public class PressureDataUploadDto
    {
        public string Pin { get; set; } = string.Empty;

        public int VectorCode { get; set; }

        public string StepCode { get; set; } = string.Empty;

        public string StationCode { get; set; } = string.Empty;

        public string CompleteTime { get; set; } = string.Empty;

        public IList<PressureValue>? PressureDatas { get; set; }
    }


    public class PressureValue
    {
        public uint KeepDuration { get; set; }
        public float PressureMax { get; set; }
        public float PressureAverage { get; set; }
        public float ShoudlerHeight { get; set; }
    }
}
