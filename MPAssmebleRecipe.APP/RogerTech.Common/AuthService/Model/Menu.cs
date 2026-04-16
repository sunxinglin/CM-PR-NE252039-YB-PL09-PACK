using System.Collections.Generic;
using SqlSugar;

namespace RogerTech.AuthService.Models
{
    [SugarTable("Menu")]
    public class Menu
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }


        [SugarColumn(Length = 50, IsNullable = false)]
        public string Page { get; set; }

        [SugarColumn(Length = 100, IsNullable = true)]
        public string SubPage { get; set; }

        [SugarColumn(Length = 50,IsNullable =true)]
        public string ElementName { get; set; }

        // 导航属性（角色菜单权限）
        [Navigate(NavigateType.OneToMany, nameof(RoleMenu.MenuId))]
        public List<RoleMenu> RoleMenus { get; set; }
    }
}
