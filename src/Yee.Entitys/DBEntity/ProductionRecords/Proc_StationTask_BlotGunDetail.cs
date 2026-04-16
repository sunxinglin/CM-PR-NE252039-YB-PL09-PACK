using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Yee.Common.Library.CommonEnum;
using Yee.Entitys.Common;
using Yee.Entitys.Production;

namespace Yee.Entitys.DBEntity;

[Table("Proc_StationTask_BlotGunDetail")]
public class Proc_StationTask_BlotGunDetail : CommonData
{
    /// <summary>
    /// 关联工位任务主表
    /// </summary>
    public Proc_StationTask_BlotGun? Proc_StationTask_BlotGun { get; set; }
    public int Proc_StationTask_BlotGunId { get; set; }

    public int? StationId { get; set; }
    public int? StepId { get; set; }
        
    /// <summary>
    /// 完成状态
    /// </summary>
    public StationTaskStatusEnum Status { get; set; }

    /// <summary>
    /// 设备号
    /// </summary>
    [MaxLength(200)]
    public int DeviceNo { get; set; }

    /// <summary>
    /// 螺丝名
    /// </summary>
    public string? ScrewName { get; set; }

    public Base_ProResource? Base_ProResource { get; set; }
    public int? Base_ProResourceId { get; set; }

    /// <summary>
    ///  拧紧结果
    /// </summary>
    public bool ResultIsOK { get; set; }

    /// <summary>
    ///  程序号
    /// </summary>
    public int ProgramNo { get; set; }

    /// <summary>
    /// 角度值
    /// </summary>
    public decimal FinalAngle { get; set; }

    /// <summary>
    /// 角度结果：0 偏小； 1 正好； 2偏大；
    /// </summary>
    public int AngleStatus { get; set; }

    /// <summary>
    /// 最小角度
    /// </summary>
    public decimal? Angle_Min { get; set; }
    /// <summary>
    /// 最大角度
    /// </summary>
    public decimal? Angle_Max { get; set; }

    /// <summary>
    /// 目标角度
    /// </summary>
    public decimal? TargetAngle { get; set; }


    /// <summary>
    /// 最小扭矩
    /// </summary>
    public decimal? TorqueRate_Min { get; set; }

    /// <summary>
    /// 目标扭矩
    /// </summary>
    public decimal? TargetTorqueRate { get; set; }

    /// <summary>
    /// 最大扭矩
    /// </summary>
    public decimal? TorqueRate_Max { get; set; }

    /// <summary>
    /// 扭矩值
    /// </summary>
    public decimal FinalTorque { get; set; }

    /// <summary>
    /// 扭矩结果：0 偏小； 1 正好； 2偏大；
    /// </summary>
    public int TorqueStatus { get; set; }

    /// <summary>
    /// 序号
    /// </summary>
    public int? OrderNo { get; set; }

    /// <summary>
    /// 上传MES 追溯用
    /// </summary>
    public string? PackPN { get; set; }

    /// <summary>
    /// 上传MES代码
    /// </summary>
    public string? UploadCode { get; set; }
    public string? UploadCode_JD { get; set; }
}