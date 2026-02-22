using SqlSugar;

namespace RogerTech.AuthService.Models
{
    [SugarTable("UserRole")]
    public class UserRole
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        public int UserId { get; set; }
        public int RoleId { get; set; }

        [Navigate(NavigateType.OneToOne, nameof(RoleId))]
        public Role Role { get; set; }
    }
} 