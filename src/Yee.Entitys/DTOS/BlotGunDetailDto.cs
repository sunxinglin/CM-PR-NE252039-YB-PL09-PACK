namespace Yee.Entitys.DTOS
{
    public class BlotGunDetailDto
    {
        /// <summary>
        ///  程序号
        /// </summary>
        public int? ProgramNo { get; set; }
        /// <summary>
        /// 套筒号
        /// </summary>
        public int? DeviceNo { get; set; }
        /// <summary>
        ///  拧紧结果 1:NG ,2:OK,3:全部
        /// </summary>
        public int? ResultIsOK { get; set; }
        public int Limit { get; set; }
        public int Page { get; set; }
        public string? PackPN { get; set; }

        /// <summary>
        /// 工位类型 1：自动 2：人工 3：默认人工
        /// </summary>
        //public int StationType { get; set; }
        public string? StationCode { get; set; }
        public DateTime? BeginTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
