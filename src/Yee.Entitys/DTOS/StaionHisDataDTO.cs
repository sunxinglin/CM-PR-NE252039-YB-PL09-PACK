using Yee.Entitys.DBEntity;
using Yee.Entitys.Production;

namespace Yee.Entitys.DTOS;

public class StaionHisDataDTO
{
    public Proc_StationTask_Main Proc_StationTask_Main { get; set; }
    public Base_Station LastStation { get; set; }
    public List<StationTaskDTO> StationTaskList { get; set; }

}