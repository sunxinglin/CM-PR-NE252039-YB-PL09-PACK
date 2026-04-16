using Yee.Entitys.Production;

namespace Yee.Entitys.DTOS.StationTaskDataDTOS
{
    public class ScanCollectDataDTO :CommonDataDto
    {
        public Base_StationTaskScanCollect? ScanCollectData { get; set; }
        public List<StationTaskDTO>? StationTaskList { get; set; }
    }
}
