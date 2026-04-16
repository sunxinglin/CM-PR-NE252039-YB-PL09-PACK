using System.Collections.Generic;
using RogerTech.Tool;

namespace RogerTech.Common.Models
{
    public class TighteningDataDto
    {
        public string SFC { get; set; }
        public List<TighteningResult> TighteningResultList { get; set; }
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
        public ParameterDataType MesDataType { get; set; }
    }
}