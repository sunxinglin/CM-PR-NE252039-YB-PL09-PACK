namespace TimedTask.ClearHisData
{
    public class TimedTaskConifg
    {
        /// <summary>
        /// 要清理多少天之前的数据
        /// </summary>
        public int ClearHisData_Days { get; set; } = 90;

        /// <summary>
        /// 每隔多少分钟 循环一次
        /// </summary>
        public int TimedInterval { get; set; } = 5;

        /// <summary>
        /// 执行定时任务的具体时分：03:00
        /// </summary>
        public string TaskRunAt_Time { get;  set; }
    }
}
