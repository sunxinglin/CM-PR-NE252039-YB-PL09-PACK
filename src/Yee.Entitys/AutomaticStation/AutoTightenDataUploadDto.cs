using Newtonsoft.Json;
using Yee.Common.Library.CommonEnum;
using Yee.Entitys.DBEntity;

namespace Yee.Entitys.AutomaticStation;

public class AutoTightenDataUploadDto
{
    /// <summary>
    /// Pack码
    /// </summary>
    public string Pin { get; set; } = string.Empty;

    /// <summary>
    /// AGV车号
    /// </summary>
    public int VectorCode { get; set; }

    public string StepCode { get; set; } = string.Empty;

    public string StationCode { get; set; } = string.Empty;

    public string BoltType {  get; set; } = string.Empty;

    public IList<AutoBlotInfo> TightenDatas { get; set; } = null!;
}

public enum TighteningResultOkFlag : short
{
    Ok = 1,
    Ng = 2
}

public class AutoTighteningDataDto
{
    public string SFC { get; set; } = string.Empty;
    public string StationName { get; set; } = string.Empty;
    public List<TighteningResult> TighteningResultList { get; set; } = null!;
}

public sealed class TighteningResult
{
    public short Index { get; set; }
    /// <summary>
    /// 拧紧结果：1代表true；0代表false
    /// </summary>
    public short ResultOK { get; set; }
    public short OrderNo { get; set; }
    public short ProgramNo { get; set; }
    public MesMeasuredValue TorqueResult { get; set; }
    public MesMeasuredValue AngleResult { get; set; }
    [JsonIgnore]
    public bool IsOk => ResultOK == (short)TighteningResultOkFlag.Ok;
}

public sealed class MesMeasuredValue
{
    /// <summary>
    /// PLC DB块中的标签名和对应的标签值
    /// </summary>
    public string TagName { get; set; }
    public string TagValue { get; set; }

    /// <summary>
    /// 工厂MES上传代码
    /// </summary>
    public string MesName { get; set; }

    /// <summary>
    /// 工厂MES上传数据类型
    /// </summary>
    public ValueTypeEnum MesDataType { get; set; }
}
