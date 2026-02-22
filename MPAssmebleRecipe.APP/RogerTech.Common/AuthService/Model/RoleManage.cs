using System;
using RogerTech.AuthService.Models;
using SqlSugar;

namespace RogerTech.Common.AuthService.Model
{
    [SugarTable("RoleManage")]
    public class RoleManage
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        [SugarColumn(IsNullable = false)]
        public int ManagerRoleId { get; set; }

        [SugarColumn(IsNullable = false)]
        public int ManagedRoleId { get; set; }

        [SugarColumn(DefaultValue = "CURRENT_TIMESTAMP")]
        public DateTime CreatedTime { get; set; }

        // 导航属性（管理者角色）
        [Navigate(NavigateType.OneToOne, nameof(ManagerRoleId))]
        public Role ManagerRole { get; set; }

        // 导航属性（被管理角色）
        [Navigate(NavigateType.OneToOne, nameof(ManagedRoleId))]
        public Role ManagedRole { get; set; }
    }
}