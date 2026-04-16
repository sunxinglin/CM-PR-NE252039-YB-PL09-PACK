using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Yee.Entitys.Common;

namespace Yee.Entitys.Production
{
    [Table("Base_Pack")]
    public class Base_Pack : CommonData
    {
        [MaxLength(200)]
        public string? Code { get; set; }
        [MaxLength(200)]
        public string? Name { get; set; }
        [MaxLength(200)]
        public string? PackSN { get; set; }
        public  DateTime OnlineTime { get; set;}
        public bool IsComplete { get; set; }
        public DateTime CompleteTime { get; set; }
        public bool IsNG { get; set; }
        public int CurrentStation { get; set; }
        public bool IsUploadMES { get; set; }
        [MaxLength(200)]
        public string? Description { get; set; }
    }
}
