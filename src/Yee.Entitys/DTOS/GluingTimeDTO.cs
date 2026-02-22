using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yee.Entitys.DTOS
{
    public class GluingTimeDTO
    {

        public int Id { get; set; }
        public decimal GluingTime { get; set; }
        public string? GluingTimeName { get; set; }
        public string? PackPN { get; set; }
        public DateTime? CreateTime { get; set; }

    }
}
