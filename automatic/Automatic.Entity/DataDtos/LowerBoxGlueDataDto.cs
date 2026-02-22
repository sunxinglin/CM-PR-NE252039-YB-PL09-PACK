namespace Automatic.Entity.DataDtos
{
    public class LowerBoxGlueDataDto
    {
        public string PackCode { get; set; } = string.Empty;

        public int VectorCode { get; set; }

        public string StationCode { get; set; } = string.Empty;

        public string GlueStartTime { get; set; } = string.Empty;

        public Dictionary<string, object> GlueDatas { get; set; } = null!;
    }
}
