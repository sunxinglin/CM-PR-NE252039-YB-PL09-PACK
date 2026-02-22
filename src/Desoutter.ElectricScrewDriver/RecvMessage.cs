using Desoutter.ElectricScrewDriver.MidMessage;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Desoutter.ElectricScrewDriver
{

    public class RecvMessage
    {
        private readonly ILogger<RecvMessage> _logger;
        private readonly Util _util;
        private readonly SendMessage _sendMessage;
        public RecvMessage(ILogger<RecvMessage> logger,  SendMessage sendMessage, Util util)
        {
            _logger = logger;
            _util = util; 
            _sendMessage = sendMessage;
        }

        public bool MID0002_CommunicationStartAcknowledge(byte[] message)
        {
            return true;
        }

        public bool MID0004_ApplicationCommunicationNegativeAcknowledge(byte[] message)
        {

            return true;
        }

        public MID0061_LastTighteningResultData MID0061_Last_Tightening_Result_Data(byte[] message)
        {
            return new MID0061_LastTighteningResultData(message);
        }

        public MID0061_LastTighteningResultData_BS MID0061_Last_Tightening_Result_Data_BS(byte[] message)
        {
            return new MID0061_LastTighteningResultData_BS(message);
        }

        public MID0071_Alarm MID0071_Alarm(byte[] message)
        {
            return new MID0071_Alarm(message);
        }
    }
}
