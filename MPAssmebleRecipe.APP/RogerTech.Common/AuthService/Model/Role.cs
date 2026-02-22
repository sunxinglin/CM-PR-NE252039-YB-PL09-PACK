using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogerTech.AuthService.Models
{
    [SugarTable("Role")]
    public class Role
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        [SugarColumn(Length = 50, IsNullable = false)]
        public string RoleName { get; set; }

        [SugarColumn(Length = 200, IsNullable =true)]
        public string Description { get; set; }

        [SugarColumn(DefaultValue = "CURRENT_TIMESTAMP")]
        public DateTime CreatedTime { get; set; }

        // 导航属性（角色菜单权限）
        [Navigate(NavigateType.OneToMany, nameof(RoleMenu.RoleId))]
        public List<RoleMenu> RoleMenus { get; set; }

        // 导航属性（用户角色）
        [Navigate(NavigateType.OneToMany, nameof(UserRole.RoleId))]
        public List<UserRole> UserRoles { get; set; }
    }
}
