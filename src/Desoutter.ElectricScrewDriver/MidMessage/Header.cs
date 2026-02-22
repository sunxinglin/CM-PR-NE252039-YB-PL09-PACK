using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desoutter.ElectricScrewDriver.MidMessage
{
    public class OpenProtocalHeader
    {
        public OpenProtocalHeader(byte[] message)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            length = encoding.GetString(message, 0, 4);
            mid = encoding.GetString(message, 4, 4);
            revision = encoding.GetString(message, 8, 3);
            noAckFlag = encoding.GetString(message, 11, 1);
            stationId = encoding.GetString(message, 12, 2);
            spindleId = encoding.GetString(message, 14, 2);
            spare = encoding.GetString(message, 16, 4);
        }
        public string length { get; set; }
        public string mid { get; set; }
        public string revision { get; set; }
        public string noAckFlag { get; set; }
        public string stationId { get; set; }
        public string spindleId { get; set; }
        public string spare { get; set; }
    }

}
