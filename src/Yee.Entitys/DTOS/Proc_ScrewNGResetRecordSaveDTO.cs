namespace Yee.Entitys.DTOS
{
    public class Proc_ScrewNGResetRecordSaveDTO
    {
        public string Account { get; set; } = null!;
        public string StationCode { get; set; } = null!;
        public string PackCode { get; set; } = null!;
        public int ScrewSerialNo { get; set; }
    }
}
