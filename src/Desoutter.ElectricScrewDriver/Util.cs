using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Desoutter.ElectricScrewDriver
{

    public enum DesoutterSendOrRecv
    {
        Send = 1,
        Recv = 2
    }

    public class Util
    {
        private readonly ILogger<Util> _logger;
        public Util(ILogger<Util> logger)
        {
            _logger = logger;
        }

        public async Task LogMessage(DesoutterSendOrRecv sr, string messageType,Byte[] bytes)
        {
            string log = "【" + messageType + "】";
            for (int i = 0; i < bytes.Length; i++)
            {
                log += bytes[i].ToString();
            }
            string SendOrRecv = string.Empty;
            if (sr == DesoutterSendOrRecv.Send){
                SendOrRecv = "【Send】";
            }else if(sr == DesoutterSendOrRecv.Recv){
                SendOrRecv = "【Recv】";
            }
            else{
                SendOrRecv = string.Empty;
            }
            log += "----";
            var msg = SendOrRecv + log + ASCIIToString(bytes);
            _logger.LogInformation(msg);
            
        }

        public async Task SendMessage(TcpClient tcpClient, string messageType ,byte[] bytes)
        {
            NetworkStream stream = tcpClient.GetStream();
            if(stream.CanWrite)
            {
                await stream.WriteAsync(bytes,0,bytes.Length);
                this.LogMessage(DesoutterSendOrRecv.Send,messageType, bytes);
            }
            return;
        }

        public byte[] StringToASCII(string str)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            char[] chars = str.ToArray();
            byte[] bytes = new byte[chars.Length];
            bytes = encoding.GetBytes(chars);
            return bytes;
        }

        public string ASCIIToString(byte[] bytes)
        {
            string str = string.Empty;
            ASCIIEncoding encoding = new ASCIIEncoding();
            for(int i =0; i < bytes.Length; i++)
            {
                char[] buf = encoding.GetChars(bytes);
                str += buf[i];
            }
            return str;
        }


        public byte[] FormatMessage(DesoutterResult header, DesoutterResult dataField)
        {
            End end = new End();

            if (header != null && dataField != null)
            {
                byte[] bytes = new byte[1024];
                var tmp = header.Data.Concat(dataField.Data).ToArray();

                bytes = tmp.Concat(end.MessageEnd).ToArray();

                return bytes;
            }
            return null;

        }

        public DesoutterResult DataFieldToASC(DataField dataField)
        {
            DesoutterResult result = new DesoutterResult();
            string strData = string.Empty;
            if (dataField != null)
            {
                int pos = 1;
                foreach(var field in dataField.Parameter)
                {
                    //strData += pos.ToString().PadLeft(2,'0');
                    strData += field;
                }

                result.Data = StringToASCII(strData);
                result.IsSucc = true;
            }
            else
            {
                result.IsSucc = false;
                result.Message = "[DataFieldToASC]:dataField is null";
            }

            return result;
        }

        public static ArraySegment<byte> SetArraySegment(byte[] src, ArraySegment<byte> dest)
        {
            if(src.Length == dest.Count)
            {
                for(int i=0; i<src.Length; i++)
                {
                    dest[i] = src[i];
                }
            }
            return dest;
        }


        public DesoutterResult HeaderToASC(Header header)
        {
            DesoutterResult result = new DesoutterResult();

            if (header == null)
            {
                result.IsSucc = false;
                result.Message = "HeaderToASC input header is null";
                return result;
            }

            byte[] data = new byte[20];
            var array = new ArraySegment<byte>(data);

            ArraySegment<byte> length = new ArraySegment<byte>(data, 0, 4);
            ArraySegment<byte> mid = new ArraySegment<byte>(data, 4, 4);
            ArraySegment<byte> revision = new ArraySegment<byte>(data, 8, 3);
            ArraySegment<byte> noAckFlag = new ArraySegment<byte>(data, 11, 1);
            ArraySegment<byte> stationId = new ArraySegment<byte>(data, 12, 2);
            ArraySegment<byte> spindleID = new ArraySegment<byte>(data, 14, 2);
            ArraySegment<byte> sequenceNumber = new ArraySegment<byte>(data, 16, 2);
            ArraySegment<byte> numberOfMessageParts = new ArraySegment<byte>(data, 18, 1);
            ArraySegment<byte> messagePartNumber = new ArraySegment<byte>(data,19,1);

            length = SetArraySegment(header.Length, length);
            mid = SetArraySegment(header.MID, mid);
            revision = SetArraySegment(header.Revision, revision);
            noAckFlag = SetArraySegment(header.NoAckFlag, noAckFlag);
            stationId = SetArraySegment(header.StationId, stationId);
            spindleID = SetArraySegment(header.SpindleID, spindleID);
            sequenceNumber = SetArraySegment(header.SequenceNumber, sequenceNumber);
            numberOfMessageParts = SetArraySegment(header.NumberOfMessageParts, numberOfMessageParts);
            messagePartNumber = SetArraySegment(header.MessagePartNumber, messagePartNumber);

            result.Data = data;
            result.IsSucc = true;
            return result; 
        }
    }


}
