using System.ComponentModel.DataAnnotations.Schema;

namespace Yee.Entitys.DBEntity
{
    [Table("ModuleTypeTable")]
    public class ModuleTypeTable
    {
        public int Id { get; set; }
        public string ModulePn { get; set; } = "";

        public int ModuleType { get; set; }
    }
}
