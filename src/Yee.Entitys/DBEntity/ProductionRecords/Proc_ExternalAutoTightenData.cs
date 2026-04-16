using System.ComponentModel.DataAnnotations.Schema;

using Yee.Entitys.Common;
using Yee.Entitys.DBEntity.Production;

namespace Yee.Entitys.DBEntity.ProductionRecords;

[Table("Proc_ExternalAutoTightenData")]
public class Proc_ExternalAutoTightenData : CommonData
{
    public string Sfc { get; set; } = string.Empty;

    public string StationName { get; set; } = string.Empty;

    public TightenReworkType TightenType { get; set; } = TightenReworkType.Lid;

    public string TighteningResultJson { get; set; } = string.Empty;
}

