namespace Automatic.Entity.DataDtos
{
    public class ModuleInBoxDataUploadDto
    {
        public int VectorCode { get; set; }
        public string StepCode { get; set; } = string.Empty;
        public string StationCode { get; set; } = string.Empty;
        public string PackCode { get; set; } = string.Empty;
        public IList<ModouleData>? ModouleDatas { get; set; }
    }

    public class ModouleData
    {
        public string ModuleCode { get; set; } = string.Empty;
        public uint KeepDuation { get; set; }
        public float ModuleLenth { get; set; }
        public float DownDistance { get; set; }
        public float DownPressure { get; set; }
        public float LeftPressure { get; set; }
        public float RightPressure { get; set; }
        public DateTime CompleteTime { get; set; }
    }

    public class ModuleInBoxSingleModuleUploadDto
    {
        public int VectorCode { get; set; }
        public string StepCode { get; set; } = string.Empty;
        public string StationCode { get; set; } = string.Empty;
        public string PackCode { get; set; } = string.Empty;
        public string ModuleCode { get; set; } = string.Empty;
        public int ModuleLocation { get; set; }
    }
}
