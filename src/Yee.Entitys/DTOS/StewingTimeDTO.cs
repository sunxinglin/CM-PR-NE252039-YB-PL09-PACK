using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yee.Entitys.DTOS
{
    public class StewingTimeDTO
    {

        public int Id { get; set; }
        public decimal StewingTime { get; set; }
        public string? StewingTimeName { get; set; }
        public string? PackPN { get; set; }

    }
}
