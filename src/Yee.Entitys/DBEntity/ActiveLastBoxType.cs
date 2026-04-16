using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Yee.Entitys.DBEntity
{
    [Table("ActiveLastBoxType")]
    public class ActiveLastBoxType
    {
        public int Id { get; set; }

        [MaxLength(30)]
        public string StationFlag { get; set; } = "";
        [MaxLength(30)]
        public string PackCode { get; set; } = "";

        public int BoxLevel { get; set; } = 1;
    }
}
