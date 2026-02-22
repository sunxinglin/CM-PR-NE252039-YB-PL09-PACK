using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogerTech.AuthService.Models
{
    [SugarTable("Workstation")]
    public class Workstation
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        [SugarColumn(Length = 50, IsNullable = false)]
        public string Name { get; set; }

        [SugarColumn(Length = 20, IsNullable = false)]
        public string Code { get; set; }

        [SugarColumn(Length = 200)]
        public string Description { get; set; }

        [SugarColumn(DefaultValue = "CURRENT_TIMESTAMP")]
        public DateTime CreatedTime { get; set; }

        // 导航属性（用户工位）
        [Navigate(NavigateType.OneToMany, nameof(UserWorkstation.WorkstationId))]
        public List<UserWorkstation> UserWorkstations { get; set; }
    }
}
