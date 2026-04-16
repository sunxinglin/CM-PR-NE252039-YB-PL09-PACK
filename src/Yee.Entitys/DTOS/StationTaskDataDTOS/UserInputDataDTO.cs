using Yee.Entitys.Production;

namespace Yee.Entitys.DTOS.StationTaskDataDTOS
{
    public class UserInputDataDTO : CommonDataDto
    {
        public Base_StationTaskUserInput? UserInputData { get; set; }
        public List<StationTaskDTO>? StationTaskList { get; set; }
    }
}
