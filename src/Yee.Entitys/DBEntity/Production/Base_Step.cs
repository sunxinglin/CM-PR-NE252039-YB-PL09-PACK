using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yee.Common.Library.CommonEnum;
using Yee.Entitys.Common;

namespace Yee.Entitys.Production
{
    [Table("Base_Step")]
    public class Base_Step : CommonData
    {
        [MaxLength(200)]
        public string? Code { get; set; }
        [MaxLength(200)]
        public string? Name { get; set; }
        public StepTypeEnum Steptype { get; set; }
        [MaxLength(200)]
        public string? Description { get; set; }
    }
}
