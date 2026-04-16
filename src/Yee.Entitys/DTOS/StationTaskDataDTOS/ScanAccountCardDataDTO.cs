using Yee.Entitys.Production;

namespace Yee.Entitys.DTOS.StationTaskDataDTOS
{
    public class ScanAccountCardDataDTO :CommonDataDto
    {
        public Base_StationTaskScanAccountCard? ScanAccountCardData { get; set; }
        public List<StationTaskDTO>? StationTaskList { get; set; }
    }
}
