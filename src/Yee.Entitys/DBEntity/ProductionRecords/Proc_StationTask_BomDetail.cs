using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Yee.Common.Library;
using Yee.Common.Library.CommonEnum;
using Yee.Entitys.Common;

namespace Yee.Entitys.DBEntity
{
    [Table("Proc_StationTask_BomDetail")]
    public class Proc_StationTask_BomDetail : CommonData
    {
        /// <summary>
        /// 关联工位BOM主表
        /// </summary>
        public Proc_StationTask_Bom? Proc_StationTask_Bom { get; set; }
        public int Proc_StationTask_BomId { get; set; }
        public int? StationId { get; set; }
        public int? StepId { get; set; }
        
        public int? UseNum { get; set; }
        
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
        /// 是否有外部输入
        /// </summary>
        public bool HasOuterParam { get; set; }

        /// <summary>
        /// 追溯类型
        /// </summary>
        public TracingTypeEnum TracingType { get; set; }
       
        /// <summary>
        /// 上传MES 追溯用
        /// </summary>
        [MaxLength(200)]
        public string? PackPN { get; set; }

        /// <summary>
        /// 上传MES代码
        /// </summary>
        [MaxLength(200)]
        public string? UploadCode { get; set; }

        public string? UniBarCode { get; set; }
        [MaxLength(200)]
        public string? GoodsOuterCode { get; set; }
        [MaxLength(200)]
        public string? BatchBarCode { get; set; }

        public OuterParamTypeEnum OuterParam1 { get; set; }

        public OuterParamTypeEnum OuterParam2 { get; set; }
        public OuterParamTypeEnum OuterParam3 { get; set; }
        [MaxLength(200)]
        public string? OuterParam1Result { get; set; }
        [MaxLength(200)]
        public string? OuterParam2Result { get; set; }
        [MaxLength(200)]
        public string? OuterParam3Result { get; set; }

        //外部输入上传代码
        [MaxLength(200)]
        public string? UpValue_1 { get; set; }
        [MaxLength(200)]
        public string? UpValue_2 { get; set; }
        [MaxLength(200)]
        public String? UpValue_3 { get; set; }
        public bool HasUpMesDone { get; set; } = false;
    }
}
