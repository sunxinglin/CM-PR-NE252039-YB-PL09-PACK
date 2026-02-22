using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yee.Entitys.DTOS
{
    public class ReworkListDTO
    {
        public int TaskId { get; set; }
        public Boolean IsCleanTask { get; set; }
        public int StepID { get; set; }
        public string PackCode { get; set; }
        public int ReScrewNum { get; set; }
    }

    public class LoadReworkDataDto
    {
        public string PackCode { get; set; } = "";
        public string StepCode { get; set; } = "";
    }
}
