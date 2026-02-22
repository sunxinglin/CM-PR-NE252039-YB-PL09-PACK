using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ctp0600P.Client.Protocols.AnyLoad_Wifi
{
    public class AnyLoadSendMessage
    {
        private readonly ILogger<AnyLoadSendMessage> _logger;
        public AnyLoadSendMessage(ILogger<AnyLoadSendMessage> logger)
        {
            _logger = logger;
        }

        public byte[] ReadCurrentWeightData()
        {

            string commond = "01 30 02 47 03 74";
            byte[] bytes = strToToHexByte(commond);
            return bytes;
        }

        public byte[] ClearWeightData()
        {
            string commond = "01 30 02 5A 03 79";
            byte[] bytes = strToToHexByte(commond);
            return bytes;
        }

        /// <summary>
        /// 字符串转16进制字节数组
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public byte[] strToToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

        /// <summary>
        /// 字节数组转16进制字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public string byteToHexStr(byte[] bytes)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    returnStr += bytes[i].ToString("X2")+" ";
                }
            }
            return returnStr.TrimEnd();
        }
    }
}
