using Yee.Common.Library.CommonEnum;

namespace Yee.Entitys.CATL;

public class UploadCATLData
{
    public string PackCode { get; set; } = "";
    public IList<BomData>? ScanCodeData { get; set; }
    public IList<DcParamValue>? DCParams { get; set; }
}

public class DcParamValue
{
    public string ParamValue { get; set; } = "";
    public string UpMesCode { get; set; } = "";
    public ValueTypeEnum DataType { get; set; }
}

public class BomData
{
    public string InternalCode { get; set; } = "";
    public string ExternalCode { get; set; } = "";
    public TracingTypeEnum TracingType { get; set; } = TracingTypeEnum.精追;
    public int UseNum { get; set; }
}