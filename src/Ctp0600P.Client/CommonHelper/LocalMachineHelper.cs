using Ctp0600P.Client.CommonEntity;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ctp0600P.Client.CommonHelper
{
    public class LocalMachineHelper
    {
        public static List<ComData> LoadSerialPortList()
        {
            var comList = new List<ComData>();

            foreach (string vPortName in SerialPort.GetPortNames())
            {
                comList.Add(new ComData { PortName = vPortName });
            }
            return comList;
        }
    }
}
