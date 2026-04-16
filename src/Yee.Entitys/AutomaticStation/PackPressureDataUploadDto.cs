namespace Yee.Entitys.AutomaticStation
{
    public class PackPressureDataUploadDto
    {
        public int VectorCode { get; set; }

        public string Pin { get; set; } = string.Empty;

        public string StepCode { get; set; } = string.Empty;

        public string StationCode { get; set; } = string.Empty;

        public Dictionary<string, object> PressureDatas { get; set; } = null!;
    }
}
