using Yee.Entitys.AlarmMgmt;

namespace Yee.Entitys.DTOS
{
    public class RecordCheckPowerDTO
    {
        /// <summary>
        /// 日期
        /// </summary>
        public string? ModuleName { set; get; }

        public string? Account { get; set; }
        public List<Alarm>? Alarms { get; set; }
        public string? PackCode { get; set; }
        public string? StationCode { get; set; }
    }
}
