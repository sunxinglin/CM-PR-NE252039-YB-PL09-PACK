using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Yee.Common.Library.CommonEnum;
using Yee.Entitys.Common;
using Yee.Entitys.Production;

namespace Yee.Entitys.DBEntity;

[Table("Proc_StationTask_Bom")]
public class Proc_StationTask_Bom : CommonData
{
    /// <summary>
    /// 关联工位任务主表
    /// </summary>
    public Proc_StationTask_Record? StationTask_Record { get; set; }
    public int StationTask_RecordId { get; set; }

    /// <summary>
    /// 完成状态
    /// </summary>
    public StationTaskStatusEnum Status { get; set; }

    /// <summary>
    /// 物料号
    /// </summary>
    [MaxLength(200)]
    public string? GoodsPN { get; set; }

    /// <summary>
    /// 物料名
    /// </summary>
    [MaxLength(200)]
    public string? GoodsName { get; set; }

    /// <summary>
    /// 用量
    /// </summary>
    public int UseNum { get; set; }

    /// <summary>
    /// 当前已完成数
    /// </summary>
    public int CurCompleteNum { get; set; }

    /// <summary>
    /// 是否有外部输入
    /// </summary>
    public bool HasOuterParam { get; set; }

    public Base_ProResource? Base_ProResource { get; set; }
    public int? Base_ProResourceId { get; set; }

    /// <summary>
    /// 追溯类型
    /// </summary>
    public TracingTypeEnum TracingType { get; set; }
       
    /// <summary>
    /// 上传MES 追溯用
    /// </summary>
    public string? PackPN { get; set; }

    /// <summary>
    /// 上传MES代码
    /// </summary>
    public string? UploadCode { get; set; }

    [NotMapped]

    public List<Proc_StationTask_BomDetail> Proc_StationTask_BomDetails { get; set; }

}