using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Yee.Entitys.Common;
using Yee.Entitys.Production;

namespace Yee.Entitys.DBEntity.Production;

[Table("Base_StationTask_RecordTime")]
public class Base_StationTask_RecordTime : CommonData
{
    [MaxLength(100)]
    public string RecordTimeTaskName { get; set; } = "";

    [MaxLength(30)]
    public string TimeOutFlag { get; set; } = "";

    [MaxLength(30)]
    public string UpMesCode { get; set; } = "";

    public Base_StationTask? Base_StationTask {  get; set; }
        
    public int? StationTaskId { get; set; }

}