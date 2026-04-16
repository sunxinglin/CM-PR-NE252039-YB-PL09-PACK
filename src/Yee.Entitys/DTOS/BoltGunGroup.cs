using Yee.Entitys.Production;

namespace Yee.Entitys.DTOS
{
    public class BoltGunGroup
    {
        public string ProgramNo { get; set; }
        public List<Base_ProResource> BoltGuns { get; set; }
    }
}
