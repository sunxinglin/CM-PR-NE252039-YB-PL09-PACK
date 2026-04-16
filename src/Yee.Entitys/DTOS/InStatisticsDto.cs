using Yee.Entitys.DBEntity.ProductionRecords;

namespace Yee.Entitys.DTOS
{
    public class InStatisticsDto
    {
       public DateTime BeginTime { get; set; }
       public DateTime EndTime { get; set; }
       public Proc_ProductStates states { get; set; }
       public int Productid { get; set; }
        public string? Key { get; set; }
        public int? Limit { get; set; }
        public int? Page { get; set; }
    }
}
