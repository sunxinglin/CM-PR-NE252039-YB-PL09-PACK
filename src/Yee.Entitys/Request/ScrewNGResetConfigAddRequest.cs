using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yee.Entitys.Request
{
    public class ScrewNGResetConfigAddRequest
    {
        public int StepId { get; set; }
        public int SingleScrewResetNum { get; set; }
        public int[] RoleIdArray { get; set; } = null!;
    }
}
