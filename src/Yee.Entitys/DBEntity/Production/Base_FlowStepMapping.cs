using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yee.Entitys.Common;
using Yee.Entitys.Production;

namespace Yee.Entitys.Production
{
    [Table("Base_FlowStepMapping")]
    public class Base_FlowStepMapping : CommonData
    {
        public Base_Flow? Flow { get; set; }
        public int FlowId { get; set; }
        public Base_Step? Step { get; set; }
        public int StepId { get; set; }
        
        /// <summary>
        /// 工序排序号
        /// </summary>
        public int OrderNo { get; set; }
    }
}
