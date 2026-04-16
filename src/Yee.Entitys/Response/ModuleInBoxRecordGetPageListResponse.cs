namespace Yee.Entitys.Response
{
    public class ModuleInBoxRecordGetPageListResponse
    {
        /// <summary>
        /// 记录主键Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Pack码
        /// </summary>
        public string PackCode { get; set; }
        /// <summary>
        /// 下箱体码
        /// </summary>
        public string OuterGoodsCode { get; set; }
        /// <summary>
        /// 模组码
        /// </summary>
        public string ModuleCode { get; set; }
        /// <summary>
        /// 入箱时间
        /// </summary>
        public DateTime? InTime { get; set; }
        /// <summary>
        /// 等离子枪1
        /// </summary>
        public string? dlz1 { get; set; }
        /// <summary>
        /// 等离子枪2
        /// </summary>
        public string? dlz2 { get; set; }

    }
}
