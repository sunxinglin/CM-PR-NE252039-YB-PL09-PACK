using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Yee.Entitys.BaseData
{
    [Table("CategoryType")]
    public class CategoryType 
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(200)]
        public string? Name { get; set; }

        public bool IsDeleted { get; set; }=false;

    }
}
