using Yee.Common.Library.CommonEnum;

namespace Yee.Entitys.DTOS
{
    public  class StationPack_Task_Record_DTO
    {
        public string StationCode { get; set; }
        public string PackCode { get; set; }
        public List<Proc_StationTask_Record_DTO> Proc_StationTask_Record_DTO_List { get; set; }

    }

    public class Proc_StationTask_Record_DTO
    {
        public int Id { get; set; }
        public int? OrderNo { get; set; }
        public TracingTypeEnum? TracingType { get; set; }

        /// <summary>
        /// 任务名
        /// </summary>
        public string TaskName { get; set; }

        /// <summary>
        /// 完成状态
        /// </summary>
        public StationTaskStatusEnum Status { get; set; }

        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 精追码
        /// </summary>
        public string GoodsOuterCode { get; set; }
        /// <summary>
        /// 批次码
        /// </summary>
        public string BatchBarCode { get; set; }
        /// <summary>
        /// 库存码
        /// </summary>
        public string UniBarCode { get; set; }
        /// <summary>
        /// 是否已经上传？？
        /// </summary>
        public bool HasUpMesDone { get; set; } = false;
    }
}
