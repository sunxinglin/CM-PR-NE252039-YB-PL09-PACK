using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yee.Entitys.DBEntity.Production;

namespace Yee.Entitys.Response
{
    public class ScrewNGResetConfigPageListResponse : Base_ScrewNGResetConfig
    {
        public string[]? RoleNameArray { get; set; }
    }
}
