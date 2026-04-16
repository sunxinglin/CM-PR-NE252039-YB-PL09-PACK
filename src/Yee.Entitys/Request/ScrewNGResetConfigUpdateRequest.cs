namespace Yee.Entitys.Request
{
    public class ScrewNGResetConfigUpdateRequest
    {
        public int Id { get; set; }
        public int StepId { get; set; }
        public int SingleScrewResetNum { get; set; }
        public int[] RoleIdArray { get; set; } = null!;
    }
}
