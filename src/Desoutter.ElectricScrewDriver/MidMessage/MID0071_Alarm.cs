using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desoutter.ElectricScrewDriver.MidMessage
{
    public class DataField_MID0071
    {
        public DataField_MID0071(byte[] message)
        {
            encoding = new ASCIIEncoding();
            
            this.Create(message);

        }
        public string _01_ErrorCode { get; set; } = string.Empty;
        public string _02_ControllerReadyStatus { get; set; } = string.Empty;
        public string _03_ToolReadyStatus { get; set; } = string.Empty;
        public string _04_Time { get; set; } = string.Empty;
        public ASCIIEncoding encoding { get; set; }
        private void Create(byte[] message)
        {
            this.Get01_ErrorCode(message);
            this.Get02_ControllerReadyStatus(message); 
            this.Get03_ToolReadyStatus(message);
            this.Get04_Time(message);
        }

        public void Get01_ErrorCode(byte[] message)
        {
            string seq = encoding.GetString(message, 20, 2);
            string data = encoding.GetString(message, 22, 4);
            if (seq == "01")
            {
                _01_ErrorCode = data;
            }
        }
        public void Get02_ControllerReadyStatus(byte[] message)
        {
            string seq = encoding.GetString(message, 26, 2);
            string data = encoding.GetString(message, 28, 1);
            if (seq == "02")
            {
                _02_ControllerReadyStatus = data;
            }
        }
        public void Get03_ToolReadyStatus(byte[] message)
        {
            string seq = encoding.GetString(message, 29, 2);
            string data = encoding.GetString(message, 31, 1);
            if (seq == "03")
            {
                _03_ToolReadyStatus = data;
            }
        }
        public void Get04_Time(byte[] message)
        {
            string seq = encoding.GetString(message, 32, 2);
            string data = encoding.GetString(message, 34, 19);
            if (seq == "04")
            {
                _04_Time = data;
            }
        }
    }

    public class MID0071_Alarm
    {
        public MID0071_Alarm(byte[] message)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            header = new OpenProtocalHeader(message);
            dataField = new DataField_MID0071(message);
        }
        public OpenProtocalHeader header { get; set; }
        public DataField_MID0071 dataField { get; set; }

    }
}
