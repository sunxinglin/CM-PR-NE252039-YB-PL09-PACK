using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yee.Entitys.DTOS
{
    public  class AGVMessage
    {
        public int AgvCode { get; set; }

        public string? ProductCode { get; set; }
        public string? HolderBarCode { get; set; }//出货码
        public string? ProductType { get; set; }
        public int? Behavior { get; set; }
        public string? StationName { get; set; }
        public DateTime? Time { get; set; }
    }

    public class BingAgvDTO
    {
        public BingAgvStateEnum State { get; set; }
        public int AgvCode { get; set; }
        public string PackPN { get; set; }
        public string? ProductType { get; set; }
        public string StationCode { get; set; }
        public string? HolderBarCode { get; set; }
    }

    public enum BingAgvStateEnum
    {
        绑定 = 1,
        解绑 = 2
    }
    public class BingagvResponse
    {
        public Boolean Success { get; set; }
        public string Message { get; set; }
        public string Response { get; set; }
        public dynamic AgvIndos { get; set; }
    }
}
