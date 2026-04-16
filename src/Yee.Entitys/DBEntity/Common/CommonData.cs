using System.ComponentModel.DataAnnotations.Schema;

using Yee.Entitys.DBEntity.Common;

namespace Yee.Entitys.Common;

public class CommonData : BaseDataModel
{
    [NotMapped]
    public string? AGVCode { get; set; }
    [NotMapped]
    public virtual string? PackCode { get; set; }
        
}