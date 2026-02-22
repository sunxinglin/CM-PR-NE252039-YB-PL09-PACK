using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogerTech.AuthService.Models
{
    [SugarTable("User")]
    public class User
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        [SugarColumn(Length = 50, IsNullable = false)]
        public string UserName { get; set; }

        [SugarColumn(Length = 20, IsNullable = false)]
        public string EmployeeId { get; set; }

        [SugarColumn(Length = 256, IsNullable = false)]
        public string PasswordHash { get; set; }

        [SugarColumn(Length = 128, IsNullable = false)]
        public string Salt { get; set; } = "RogerTech";

        [SugarColumn(Length = 128, IsNullable = false)]
        public string RoleName { get; set; } 

        public bool IsActive { get; set; } = true;

        [SugarColumn(DefaultValue = "CURRENT_TIMESTAMP")]
        public DateTime CreatedTime { get; set; }

        // 导航属性（用户角色）
        [Navigate(NavigateType.OneToMany, nameof(UserRole.UserId))]
        public List<UserRole> UserRoles { get; set; }

        // 导航属性（用户工位）
        [Navigate(NavigateType.OneToMany, nameof(UserWorkstation.UserId))]
        public List<UserWorkstation> UserWorkstations { get; set; }
    }
}
