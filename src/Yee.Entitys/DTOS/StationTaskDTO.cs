using Yee.Entitys.Common;
using Yee.Entitys.DBEntity.Production;
using Yee.Entitys.Production;

namespace Yee.Entitys.DTOS;

public  class StationTaskDTO : CommonData
{
    public int StepId { get; set; }

    public int StationID { get; set; }

    public Base_StationTask? StationTask { get; set; }
    public int StationTaskId { get; set; }

    public bool HasFinish { get; set; }

    public int Sequence { get; set; }


    public List<Base_StationTaskBom>? StationTaskBom { get; set; }
    public List<Base_StationTaskScrew>? StationTaskScrew { get; set; }

    public Base_StationTaskAnyLoad? StationTaskAnyLoad { get; set; }
    public Base_StationTaskCheckTimeOut? StationTaskCheckTimeout { get; set; }
    public Base_StationTaskStewingTime? StationTaskStewingTime { get; set; }

    public Base_StationTaskUserInput? StationTaskUserInput { get; set; }

    public Base_StationTaskScanCollect? StationTaskScanCollect { get; set; }
    public Base_StationTaskScanAccountCard? StationTaskScanAccountCard { get; set; }
    public Base_StationTask_RecordTime? StationTask_RecordTime { get; set; }
    public Base_StationTask_TightenRework? Base_StationTask_TightenRework { get; set; }
    public Base_StationTask_TightenByImage? Base_StationTask_TightenByImage { get; set; }
}