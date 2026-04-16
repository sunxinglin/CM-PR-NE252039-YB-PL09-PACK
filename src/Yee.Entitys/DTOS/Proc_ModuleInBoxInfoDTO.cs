namespace Yee.Entitys.DTOS
{
    public class Proc_ModuleInBoxInfoDTO
    {
        public string StationCode { get; set; }

        public int AGVNo { get; set; }

        public string? PackPN { get; set; }

        public bool Stauts { get; set; }

        public int GlueType { get; set; }
        public DateTime GlueStartTime { get; set; }

        public Dictionary<string, float> GlueDataArray { get; set; }
    }
}
