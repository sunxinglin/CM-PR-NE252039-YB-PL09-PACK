namespace Yee.Entitys.AutomaticStation
{
    public class HeatingFilmPressurizeDataUploadDto
    {
        public int VectorCode { get; set; }
        public string Pin { get; set; } = string.Empty;
        public string StationCode { get; set; } = string.Empty;
        public string PressureTime { get; set; } = string.Empty;
        public Dictionary<string, object>? PressurizeDatas { get; set; }
    }
}
