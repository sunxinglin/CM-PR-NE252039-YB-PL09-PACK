using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Yee.Entitys.Common;

namespace Yee.Entitys.DBEntity.ProductionRecords;

[Table("Proc_StationTask_TightenByImage")]
public class Proc_StationTask_TightenByImage : CommonData
{
    public Proc_StationTask_Record? Proc_StationTask_Record { get; set; }
    public int Proc_StationTask_RecordId { get; set; }

    /// <summary>
    /// 设备号
    /// </summary>
    [MaxLength(200)]
    public int DeviceNo { get; set; }

    public string? TaskName { get; set; }
    /// <summary>
    ///  拧紧结果
    /// </summary>
    public bool ResultIsOK { get; set; }

    /// <summary>
    ///  程序号
    /// </summary>
    public int ProgramNo { get; set; }
  
    public decimal FinalAngle { get; set; }

    public decimal FinalTorque { get; set; }

    /// <summary>
    /// 序号
    /// </summary>
    public int? OrderNo { get; set; }

    /// <summary>
    /// 上传MES 追溯用
    /// </summary>
    public string? SerialCode { get; set; }
    public string? StationCode { get; set; }

    /// <summary>
    /// 上传MES代码
    /// </summary>
    public string? UpMesCodeTor { get; set; }
    public string? UpMesCodeAng { get; set; }
}