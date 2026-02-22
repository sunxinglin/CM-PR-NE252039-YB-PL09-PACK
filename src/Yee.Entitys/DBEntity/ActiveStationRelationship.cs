using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
