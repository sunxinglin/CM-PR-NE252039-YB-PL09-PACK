using System.ComponentModel.DataAnnotations.Schema;

using Yee.Entitys.Common;

namespace Yee.Entitys.Production;

[Table("Base_AutoStationTaskLidTighten")]
public class Base_AutoStationTaskTighten : CommonData
{
    public string ParamName { get; set; } = "";
    public int UseNum { get; set; }
    public Base_StationTask? StationTask { get; set; }
    public int? StationTaskId { get; set; }

    public int ProgramNo { get; set; } = 1;
    public string UpMesCode { get; set; } = "";

    [Column(TypeName = "decimal(10,3)")]
    public decimal TorqueMin { get; set; } = 0;

    [Column(TypeName = "decimal(10,3)")]
    public decimal TorqueMax { get; set; } = 0;

    [Column(TypeName = "decimal(10,3)")]
    public decimal AngleMin { get; set; } = 0;

    [Column(TypeName = "decimal(10,3)")]
    public decimal AngleMax { get; set; } = 0;
}