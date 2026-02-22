using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automatic.Entity
{
    public class CatlMesResponse
    {
        public int code { get; set; }
        public string message { get; set; } = "";
        public string BarCode_GoodsPN { get; set; } = "";
        public string BarCode { get; set; } = "";
    }
}
