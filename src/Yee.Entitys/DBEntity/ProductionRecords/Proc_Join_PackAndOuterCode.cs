using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yee.Common.Library;
using Yee.Common.Library.CommonEnum;
using Yee.Entitys.Common;
using Yee.Entitys.Production;

namespace Yee.Entitys.DBEntity
{
    [Table("Proc_Join_PackAndOuterCode")]
    public class Proc_Join_PackAndOuterCode : CommonData
    {
        /// <summary>
        /// 外部条码
        /// </summary>
        [MaxLength(200)]
        public string? OuterGoodsCode { get; set; }

        /// <summary>
        /// pack条码
        /// </summary>
        [MaxLength(200)]
        public string? PackCode { get; set; }

        /// <summary>
        /// 工序ID
        /// </summary>
        public int StepId { get; set; }

        /// <summary>
        /// 扫描外部条码时，所在工位ID
        /// </summary>
        public int StationId { get; set; }

    }


}
