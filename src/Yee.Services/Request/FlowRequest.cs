using Yee.Entitys.Common;

namespace Yee.Services.Request
{
    public class FlowRequest
    {

    }
    public class AddOrUpdateFlowReq: CommonData
    {
        /// <summary>
        /// 编码
        /// </summary>
        public string? Code { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 版本
        /// </summary>
        public string? Version { get; set; }

        /// <summary>
        /// 产品主键
        /// </summary>
        public int ProductId { get; set; }

        public string? Description { get; set; }

        public List<FlowStepRequest> StepList { get; set; }


    }

    public class FlowStepRequest
    {
        public int Id { get; set; }
        public int StepId { get; set; }
        public string? StepName { get; set; }

        /// <summary>
        /// 工序排序号
        /// </summary>
        public int OrderNo { get; set; }

    }

    public class GetFlowStepMappingListReq
    { 
    public int? StepId { get; set; }

        public int? FlowId { get; set; }
    }
}
