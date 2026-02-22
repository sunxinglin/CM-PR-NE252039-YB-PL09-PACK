using SqlSugar;

namespace RogerTech.AuthService.Models
{
    [SugarTable("RoleMenu")]
    public class RoleMenu
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public int RoleId { get; set; }
        public int MenuId { get; set; }

    }
} 