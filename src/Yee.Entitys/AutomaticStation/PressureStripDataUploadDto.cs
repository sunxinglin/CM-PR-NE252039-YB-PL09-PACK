namespace Yee.Entitys.AutomaticStation
{
    public class PressureDataUploadDto
    {
        public int VectorCode { get; set; }

        public string Pin { get; set; } = string.Empty;

        public string StationCode { get; set; } = string.Empty;

        public string CompleteTime { get; set; } = string.Empty;

        public IList<PressureValue>? PressureDatas { get; set; }
    }
    public class PressureValue
    {
        public int KeepDuration { get; set; }
        public float PressureMax { get; set; }
        public float PressureAverage { get; set; }
        public float ShoudlerHeight { get; set; }
    }
}
