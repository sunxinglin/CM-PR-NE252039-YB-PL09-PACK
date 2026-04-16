using Yee.Entitys.Production;

namespace Yee.Entitys.DTOS.StationTaskDataDTOS
{
    public class StewingTimeDataDTO
    {
        public Base_StationTaskStewingTime? StewingTimeData { get; set; }
        public List<StationTaskDTO>? StationTaskList { get; set; }
    }
}
