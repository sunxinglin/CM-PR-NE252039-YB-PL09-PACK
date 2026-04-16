namespace Yee.Entitys.Request
{
    public class ScrewNGResetConfigAddRequest
    {
        public int StepId { get; set; }
        public int SingleScrewResetNum { get; set; }
        public int[] RoleIdArray { get; set; } = null!;
    }
}
