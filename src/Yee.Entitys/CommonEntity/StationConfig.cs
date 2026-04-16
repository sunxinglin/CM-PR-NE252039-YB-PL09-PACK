using Yee.Entitys.DTOS;
using Yee.Entitys.Production;

namespace Yee.Entitys.CommonEntity;

public class StationConfig
{
    public Base_Product Product { get; set; }
    public Base_Station Station { get; set; }
    public Base_Step Step { get; set; }
    public List<StationTaskDTO> StationTaskList { get; set; }
}