using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Yee.Common.Library.CommonEnum;
using Yee.Entitys.Common;

namespace Yee.Entitys.Production;

[Table("Base_Station")]
public class Base_Station : CommonData
{
    [MaxLength(200)]
    public string? Code { get; set; }
    [MaxLength(200)]
    public string? Name { get; set; }

    [MaxLength(200)]
    public string? IpAddress { get; set; }

    public Base_Step? Step { get; set; }
    public int StepId { get; set; }

    [MaxLength(200)]
    public string? Description { get; set; }

    public StepTypeEnum StepType { get; set; }
}
