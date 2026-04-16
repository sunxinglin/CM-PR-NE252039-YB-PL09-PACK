using System.ComponentModel.DataAnnotations.Schema;

using Yee.Entitys.Common;

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
