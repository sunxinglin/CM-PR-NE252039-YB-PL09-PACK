using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Newtonsoft.Json;

using Yee.Entitys.Common;
using Yee.Entitys.Production;

namespace Yee.Entitys.DBEntity.Production;

[Table("Base_StationTask_TightenByImage")]
public class Base_StationTask_TightenByImage : CommonData
{
    public Base_StationTask? StationTask { get; set; }
    public int StationTaskId { get; set; }
    public string TaskName { get; set; } = "";

    public int ScrewNum { get; set; }//螺栓数量

    public int ProgramNo { get; set; } = 1;
    public string UpMesCode { get; set; } = "";

    public string DevicesNos { get; set; } = "1";

    [Column(TypeName = "decimal(10,3)")]
    public decimal? MinTorque { get; set; } = 0;

    [Column(TypeName = "decimal(10,3)")]
    public decimal? MaxTorque { get; set; } = 99;
    [Column(TypeName = "decimal(10,3)")]
    public decimal? MinAngle { get; set; } = 0;
    [Column(TypeName = "decimal(10,3)")]
    public decimal? MaxAngle { get; set; } = 999;

    [Column(TypeName = "decimal(10,3)")]
    public decimal FloatFactor { get; set; } = Convert.ToDecimal(0.4f);

    [MaxLength(30)]
    public string? NcCode { get; set; }
    [MaxLength(10)]
    public string? ReverseGroup { get; set; }
    [MaxLength(10)]
    public string? NgGroup { get; set; }
    public string? ImageUrl { get; set; }

    public string? LayoutJson { get; set; }
    public CanvasLayout? Layout => JsonConvert.DeserializeObject<CanvasLayout>(LayoutJson ?? "");
}


public class CanvasLayout
{
    public float CanvasWidth { get; set; } = 700;
    public float CanvasHeight { get; set; } = 500;

    public IList<PointLayout>? Points { get; set; }
}

public class PointLayout
{
    public int OrderNo { get; set; }
    public float Point_X { get; set; }
    public float Point_Y { get; set; }
}