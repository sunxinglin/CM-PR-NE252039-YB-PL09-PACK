using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Yee.Entitys.Common;

namespace Yee.Entitys.BaseData
{
    [Table("Dictionary")]
    public class Dictionary:CommonData
    {
        [MaxLength(200)]
        public string? Name { get; set; }
        [MaxLength(200)]
        public string? Code { get; set; }
        [MaxLength(200)]
        public string? Description { get; set; }
    }
}
