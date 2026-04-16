namespace Yee.Entitys.Response
{
    public class IndexStatisticsResponse
    {
        /// <summary>
        /// 今日入箱完成数
        /// </summary>
        public int DayModuleInBoxCompletePackCount { get; set; }
        /// <summary>
        /// 今天Pack完成数
        /// </summary>
        public int DayCompletePackCount { get; set; }
        /// <summary>
        /// 今天入箱超时数
        /// </summary>
        public int DayModuleInBoxTimeOutPackCount { get; set; }
        /// <summary>
        /// 今日拧紧超时数
        /// </summary>
        public int DayScrewTimeOutPackCount { get; set; }
        /// <summary>
        /// 本周Pack完成数
        /// </summary>
        public int WeekCompletePackCount { get; set; }
        /// <summary>
        /// 本周入箱超时数
        /// </summary>
        public int WeekModuleInBoxTimeOutPackCount { get; set; }
        /// <summary>
        /// 本周拧紧超时数
        /// </summary>
        public int WeekScrewTimeOutPackCount { get; set; }
        /// <summary>
        /// 本月Pack完成数
        /// </summary>
        public int MonthCompletePackCount { get; set; }
        /// <summary>
        /// 本月入箱超时数
        /// </summary>
        public int MonthModuleInBoxTimeOutPackCount { get; set; }
        /// <summary>
        /// 本月拧紧超时数
        /// </summary>
        public int MonthScrewTimeOutPackCount { get; set; }
    }
}
