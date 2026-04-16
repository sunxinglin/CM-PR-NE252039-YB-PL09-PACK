using Yee.Entitys.Production;

namespace Yee.Entitys.DTOS.StationTaskDataDTOS;

public class ScrewDataDTO :CommonDataDto
{
    public Base_StationTaskScrew? ScrewData { get; set; }
    public List<StationTaskDTO>? StationTaskList { get; set; }
    public int? NgTimes { get; set; } = 1;
    public string? StationCode { get; set; }
    public decimal FinalTorque { get; set; }
    public decimal FinalAngle { get; set; }
    public string UploadMesCode { get; set; } = "";
    public int OrderNo { get; set; }

    public string TaskName { get; set; } = "";
}