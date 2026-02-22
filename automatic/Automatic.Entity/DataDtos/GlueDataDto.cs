namespace Automatic.Entity.DataDtos
{
    public class GlueDataDto
    {
        public string PackCode { get; set; } = string.Empty;

        public int VectorCode { get; set; }

        //public string StepCode { get; set; } = string.Empty;

        public string StationCode { get; set; } = string.Empty;

        public string GlueTime { get; set; } = string.Empty;

        public List<UploadGlueData> GlueDatas { get; set; } = null!;
    }

    public class UploadGlueData
    {
        public UploadGlueData(float glueA, float glueB, float glueProportion, float glueTotal)
        {
            GlueA = glueA;
            GlueB = glueB;
            GlueProportion = glueProportion;
            GlueTotal = glueTotal;
        }
        public float GlueA { get; set; }
        public float GlueB { get; set; }
        public float GlueProportion { get; set; }
        public float GlueTotal { get; set; }

    }
}
