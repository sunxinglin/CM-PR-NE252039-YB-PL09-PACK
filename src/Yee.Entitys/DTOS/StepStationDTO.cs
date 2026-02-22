using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yee.Entitys.Production;

namespace Yee.Entitys.DTOS
{
    public  class StepStationDTO
    {
        public Base_Step Step { get; set; }
        public Base_Station Station { get; set; }
    }
}
