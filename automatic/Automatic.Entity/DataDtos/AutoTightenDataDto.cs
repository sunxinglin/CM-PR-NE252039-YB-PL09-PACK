namespace Automatic.Entity.DataDtos
{
    public class AutoTightenDataDto
    {
        public string Pin { get; set; } = string.Empty;

        public int VectorCode { get; set; }

        public string StepCode { get; set; } = string.Empty;

        public string StationCode { get; set; } = string.Empty;

        public Dictionary<int, float> PressureData { get; set; } = null!;

    }

    public class AutoTightenDataUploadDto
    {
        public string Pin { get; set; } = string.Empty;

        public int VectorCode { get; set; }

        public string StepCode { get; set; } = string.Empty;

        public string StationCode { get; set; } = string.Empty;
        public string BoltType { get; set; } = string.Empty;

        public IList<AutoTightenDataUploadTightenItem> TightenDatas { get; set; } = null!;

        public class AutoTightenDataUploadTightenItem
        {
            public int OrderNo { get; set; }
            public bool ResultIsOK { get; set; }
            public ushort ProgramNo { get; set; }

            public float FinalTorque { get; set; }

            public ushort Constant1 { get; set; }

            public ushort Torque_trend { get; set; }

            public float FinalAngle { get; set; }

            public ushort Constant2 { get; set; }

            public ushort Angle_trend { get; set; }

            public float TorqueRate_Min { get; set; }

            public float TargetTorqueRate { get; set; }

            public float TorqueRate_Max { get; set; }

            public float Angle_Min { get; set; }

            public float TargetAngle { get; set; }

            public float Angle_Max { get; set; }
        }
    }
}
