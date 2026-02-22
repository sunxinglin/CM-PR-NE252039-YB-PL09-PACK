using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
