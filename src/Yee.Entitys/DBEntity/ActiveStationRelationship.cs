using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Yee.Entitys.DBEntity
{
    [Table("ActiveStationRelationship")]
    public class ActiveStationRelationship
    {
        [Key]
        public int Id { get; set; }
        public string StationFlag { get; set; } = "";
        public string StationCode { get; set; } = "";
        public int StationLevel { get; set; } = 1;
    }
}
