using SqlSugar;

namespace RogerTech.AuthService.Models
{
    [SugarTable("UserWorkstation")]
    public class UserWorkstation
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        public int UserId { get; set; }
        public int WorkstationId { get; set; }

        [Navigate(NavigateType.OneToOne, nameof(WorkstationId))]
        public Workstation Workstation { get; set; }
    }
} 