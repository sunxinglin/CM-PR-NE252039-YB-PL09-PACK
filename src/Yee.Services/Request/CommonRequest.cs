using Yee.Entitys.DBEntity.ProductionRecords;

namespace Yee.Services.Request
{
    public class CommonRequest
    {

    }
    public class GetByKeyInput
    {
        public string? Key { get; set; }
        public int Limit { get; set; }
        public int Page { get; set; }  
    }
    public class StationGetByKeyInput : GetByKeyInput
    {
        public int Flowid { get; set; }
    }
    public class StepGetByKeyInput : GetByKeyInput
    {
        public int Productid { get; set; }
    }
    public class Prodct_StatisticsInput
    {
        public string? Key { get; set; }
        
        public int Limit { get; set; }
        public int Page { get; set; }
        public int? Productid { get; set; }
        public Proc_ProductStates states { get; set; }
        /// <summary>
        /// 查询类型(日/月/年)
        /// </summary>
        public SelectTypeEnmu SelectType { get; set; }
        public DateTime? BegainTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
    public enum SelectTypeEnmu
    {
        日=1,
        月=2,
        年=3
    }
}
