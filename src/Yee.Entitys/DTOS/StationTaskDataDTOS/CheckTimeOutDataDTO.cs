using Yee.Entitys.Production;

namespace Yee.Entitys.DTOS.StationTaskDataDTOS
{
    public class CheckTimeOutDataDTO : CommonDataDto
    {
        public Base_StationTaskCheckTimeOut? CheckTimeOutData { get; set; }
        public List<StationTaskDTO>? StationTaskList { get; set; }
    }
}
