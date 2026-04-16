namespace Yee.Entitys.AutomaticStation
{
    public class CreateMessionRecordDto
    {
        public string Pin { get; set; } = string.Empty;
        public string ProductPn { get; set; } = "";
        public string StationCode { get; set; } = string.Empty;
        public string MessionType { get; set; } = string.Empty;
        public string VectorCode { get; set; } = string.Empty;
    }
}
