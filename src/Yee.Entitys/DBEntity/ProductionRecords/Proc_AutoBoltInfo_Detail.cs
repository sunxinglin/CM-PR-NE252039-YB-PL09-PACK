using System.ComponentModel.DataAnnotations.Schema;

using Newtonsoft.Json;

using Yee.Entitys.Common;

namespace Yee.Entitys.DBEntity;

[Table("Proc_AutoBoltInfo_Detail")]
public class Proc_AutoBoltInfo_Detail : CommonData
{
    public Proc_StationTask_Main? Proc_StationTask_Main { get; set; }
    public int Proc_StationTask_MainId { get; set; }

    public string BoltDataArray { get; set; } = "";

    public string UploadCode { get; set; } = "";
    public string UploadCode_JD { get; set; } = "";

    public string PackPN { get; set; } = "";

    public string BoltType { get; set; } = "";

    [NotMapped]
    public IList<AutoBlotInfo>? AutoBlotInfoArray => JsonConvert.DeserializeObject<IList<AutoBlotInfo>>(BoltDataArray);
}

public class AutoBlotInfo
{
    /// <summary>
    ///  拧紧结果
    /// </summary>
    public bool ResultIsOK { get; set; }

    /// <summary>
    ///  程序号
    /// </summary>
    public int ProgramNo { get; set; }

    /// <summary>
    /// 扭矩值
    /// </summary>
    public decimal FinalTorque { get; set; }

    /// <summary>
    /// 常数1反馈
    /// </summary>
    public int Constant1 { get; set; }

    /// <summary>
    /// 扭矩结果：0 偏小； 1 正好； 2偏大；
    /// </summary>
    public int TorqueStatus { get; set; }

    /// <summary>
    /// 角度值
    /// </summary>
    public decimal FinalAngle { get; set; }

    /// <summary>
    /// 常数1反馈
    /// </summary>
    public int Constant2 { get; set; }

    /// <summary>
    /// 角度结果：0 偏小； 1 正好； 2偏大；
    /// </summary>
    public int AngleStatus { get; set; }

    /// <summary>
    /// 最小扭矩
    /// </summary>
    public decimal TorqueRate_Min { get; set; }

    /// <summary>
    /// 目标扭矩
    /// </summary>
    public decimal TargetTorqueRate { get; set; }

    /// <summary>
    /// 最大扭矩
    /// </summary>
    public decimal TorqueRate_Max { get; set; }

    /// <summary>
    /// 最小角度
    /// </summary>
    public decimal Angle_Min { get; set; }

    /// <summary>
    /// 目标角度
    /// </summary>
    public decimal TargetAngle { get; set; }

    /// <summary>
    /// 最大角度
    /// </summary>
    public decimal Angle_Max { get; set; }

    /// <summary>
    /// 螺丝编号
    /// </summary>
    public int OrderNo { get; set; }
}
