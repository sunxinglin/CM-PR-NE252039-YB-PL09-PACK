using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desoutter.ElectricScrewDriver
{
    public class SendMessage
    {
        private readonly ILogger<SendMessage> _logger;
        private readonly Util _util;
        public SendMessage(ILogger<SendMessage> logger, Util util)
        {
            _logger = logger;
            _util = util;
        }

        //创建TCP连接
        public byte[] MID0001_CommunicationStart()
        {

            Header header = new Header();
            header.MID[0] = 48;
            header.MID[1] = 48;
            header.MID[2] = 48;
            header.MID[3] = 49;

            header.Revision[0] = 48;
            header.Revision[1] = 48;
            header.Revision[2] = 48;

            DataField dataField = new DataField();
            DesoutterResult dataFeildResult = _util.DataFieldToASC(dataField);

            var msgLength = 20 + dataFeildResult.Data.Length;
            header.Length = _util.StringToASCII(msgLength.ToString().PadLeft(4, '0'));

            DesoutterResult headerResult = _util.HeaderToASC(header);

            byte[] bytes = _util.FormatMessage(headerResult, dataFeildResult);
            return bytes;
        }

        //断开TCP连接
        public byte[] MID0003_CommunicationStop()
        {
            Header header = new Header();
            //header.Length = "0020";
            //header.MID = "0003";
            //header.Revision = "001";

            DataField dataField = new DataField();
            DesoutterResult dataFeildResult = _util.DataFieldToASC(dataField);

            var msgLength = 20 + dataFeildResult.Data.Length;
            header.Length = _util.StringToASCII(msgLength.ToString().PadLeft(4, '0'));

            DesoutterResult headerResult = _util.HeaderToASC(header);

            byte[] bytes = _util.FormatMessage(headerResult, dataFeildResult);
            return bytes;
        }

        //选择工具
        public byte[] MID0014_Parameter_Set_Selected_Subscribe()
        {
            Header header = new Header();
            header.MID[0] = 48;
            header.MID[1] = 48;
            header.MID[2] = 49;
            header.MID[3] = 52;

            header.Revision[0] = 48;
            header.Revision[1] = 48;
            header.Revision[2] = 48;

            DataField dataField = new DataField();
            DesoutterResult dataFeildResult = _util.DataFieldToASC(dataField);

            var msgLength = 20 + dataFeildResult.Data.Length;
            header.Length = _util.StringToASCII(msgLength.ToString().PadLeft(4, '0'));

            DesoutterResult headerResult = _util.HeaderToASC(header);

            byte[] bytes = _util.FormatMessage(headerResult, dataFeildResult);
            return bytes;
        }

        //选择工具ACK
        public byte[] MID0016_Parameter_Set_Selected_Acknowledge()
        {
            Header header = new Header();
            header.MID[0] = 48;
            header.MID[1] = 48;
            header.MID[2] = 49;
            header.MID[3] = 54;

            header.Revision[0] = 48;
            header.Revision[1] = 48;
            header.Revision[2] = 48;

            DataField dataField = new DataField();
            DesoutterResult dataFeildResult = _util.DataFieldToASC(dataField);

            var msgLength = 20 + dataFeildResult.Data.Length;
            header.Length = _util.StringToASCII(msgLength.ToString().PadLeft(4, '0'));

            DesoutterResult headerResult = _util.HeaderToASC(header);

            byte[] bytes = _util.FormatMessage(headerResult, dataFeildResult);
            return bytes;
        }

        //设置PSet
        public byte[] MID0018_Select_Parameter_Set(int pSet)
        {

            if(pSet > 1000 || pSet <= 0)
            {
                return null;
            }

            Header header = new Header();
            header.MID[0] = 48;
            header.MID[1] = 48;
            header.MID[2] = 49;
            header.MID[3] = 56;

            header.Revision[0] = 48;
            header.Revision[1] = 48;
            header.Revision[2] = 49;

            DataField dataField = new DataField();
            List<string> param = new List<string>();
            param.Add(pSet.ToString().PadLeft(3,'0'));
            dataField.Parameter = param;
            DesoutterResult dataFeildResult = _util.DataFieldToASC(dataField);

            var msgLength = 20 + dataFeildResult.Data.Length;
            header.Length = _util.StringToASCII(msgLength.ToString().PadLeft(4, '0'));

            DesoutterResult headerResult = _util.HeaderToASC(header);

            byte[] bytes = _util.FormatMessage(headerResult, dataFeildResult);
            return bytes;
        }

        //去使能工具
        public byte[] MID0042_Disable_Tool()
        {
            Header header = new Header();
            header.MID[0] = 48;
            header.MID[1] = 48;
            header.MID[2] = 52;
            header.MID[3] = 50;

            header.Revision[0] = 48;
            header.Revision[1] = 48;
            header.Revision[2] = 48;

            DataField dataField = new DataField();

            DesoutterResult dataFeildResult = _util.DataFieldToASC(dataField);

            var msgLength = 20 + dataFeildResult.Data.Length;
            header.Length = _util.StringToASCII(msgLength.ToString().PadLeft(4, '0'));

            DesoutterResult headerResult = _util.HeaderToASC(header);

            byte[] bytes = _util.FormatMessage(headerResult, dataFeildResult);
            return bytes;
        }

        //使能工具
        public byte[] MID0043_Enable_Tool()
        {
            Header header = new Header();
            header.MID[0] = 48;
            header.MID[1] = 48;
            header.MID[2] = 52;
            header.MID[3] = 51;

            header.Revision[0] = 48;
            header.Revision[1] = 48;
            header.Revision[2] = 48;

            DataField dataField = new DataField();

            DesoutterResult dataFeildResult = _util.DataFieldToASC(dataField);

            var msgLength = 20 + dataFeildResult.Data.Length;
            header.Length = _util.StringToASCII(msgLength.ToString().PadLeft(4, '0'));

            DesoutterResult headerResult = _util.HeaderToASC(header);

            byte[] bytes = _util.FormatMessage(headerResult, dataFeildResult);
            return bytes;
        }

        //获取数据
        public byte[] MID0060_Last_Tightening_Result_Data_Subscribe()
        {
            Header header = new Header();
            header.MID[0] = 48;
            header.MID[1] = 48;
            header.MID[2] = 54;
            header.MID[3] = 48;

            header.Revision[0] = 48;
            header.Revision[1] = 48;
            header.Revision[2] = 55;

            DataField dataField = new DataField();
            DesoutterResult dataFeildResult = _util.DataFieldToASC(dataField);

            var msgLength = 20 + dataFeildResult.Data.Length;
            header.Length = _util.StringToASCII(msgLength.ToString().PadLeft(4, '0'));

            DesoutterResult headerResult = _util.HeaderToASC(header);

            byte[] bytes = _util.FormatMessage(headerResult, dataFeildResult);
            return bytes;
        }

        public byte[] MID0060_Last_Tightening_Result_Data_Subscribe_BS()
        {
            Header header = new Header();
            header.MID[0] = 48;
            header.MID[1] = 48;
            header.MID[2] = 54;
            header.MID[3] = 48;

            header.Revision[0] = 48;
            header.Revision[1] = 48;
            header.Revision[2] = 49;
            header.NoAckFlag[0] = 48;

            DataField dataField = new DataField();
            DesoutterResult dataFeildResult = _util.DataFieldToASC(dataField);

            var msgLength = 20 + dataFeildResult.Data.Length;
            header.Length = _util.StringToASCII(msgLength.ToString().PadLeft(4, '0'));

            DesoutterResult headerResult = _util.HeaderToASC(header);

            byte[] bytes = _util.FormatMessage(headerResult, dataFeildResult);
            return bytes;
        }

        //获取数据Ack
        public byte[] MID0062_Last_Tightening_Result_Data_Acknowledge()
        {
            Header header = new Header();
            header.MID[0] = 48;
            header.MID[1] = 48;
            header.MID[2] = 54;
            header.MID[3] = 50;

            header.Revision[0] = 48;
            header.Revision[1] = 48;
            header.Revision[2] = 48;

            DataField dataField = new DataField();
            DesoutterResult dataFeildResult = _util.DataFieldToASC(dataField);

            var msgLength = 20 + dataFeildResult.Data.Length;
            header.Length = _util.StringToASCII(msgLength.ToString().PadLeft(4, '0'));

            DesoutterResult headerResult = _util.HeaderToASC(header);

            byte[] bytes = _util.FormatMessage(headerResult, dataFeildResult);
            return bytes;
        }

        //获取报警
        public byte[] MID0070_Alarm_Subscribe()
        {
            Header header = new Header();
            header.MID[0] = 48;
            header.MID[1] = 48;
            header.MID[2] = 55;
            header.MID[3] = 48;

            header.Revision[0] = 48;
            header.Revision[1] = 48;
            header.Revision[2] = 48;

            DataField dataField = new DataField();
            DesoutterResult dataFeildResult = _util.DataFieldToASC(dataField);

            var msgLength = 20 + dataFeildResult.Data.Length;
            header.Length = _util.StringToASCII(msgLength.ToString().PadLeft(4, '0'));

            DesoutterResult headerResult = _util.HeaderToASC(header);

            byte[] bytes = _util.FormatMessage(headerResult, dataFeildResult);
            return bytes;
        }

        //获取报警ACK
        public byte[] MID0072_Alarm_Acknowledge()
        {
            Header header = new Header();
            header.MID[0] = 48;
            header.MID[1] = 48;
            header.MID[2] = 55;
            header.MID[3] = 50;

            header.Revision[0] = 48;
            header.Revision[1] = 48;
            header.Revision[2] = 48;

            DataField dataField = new DataField();
            DesoutterResult dataFeildResult = _util.DataFieldToASC(dataField);

            var msgLength = 20 + dataFeildResult.Data.Length;
            header.Length = _util.StringToASCII(msgLength.ToString().PadLeft(4, '0'));

            DesoutterResult headerResult = _util.HeaderToASC(header);

            byte[] bytes = _util.FormatMessage(headerResult, dataFeildResult);
            return bytes;
        }

        //获取报警&状态ACK
        public byte[] MID0077_Alarm_Status_Acknowledge()
        {
            Header header = new Header();
            header.MID[0] = 48;
            header.MID[1] = 48;
            header.MID[2] = 55;
            header.MID[3] = 55;

            header.Revision[0] = 48;
            header.Revision[1] = 48;
            header.Revision[2] = 48;

            DataField dataField = new DataField();
            DesoutterResult dataFeildResult = _util.DataFieldToASC(dataField);

            var msgLength = 20 + dataFeildResult.Data.Length;
            header.Length = _util.StringToASCII(msgLength.ToString().PadLeft(4, '0'));

            DesoutterResult headerResult = _util.HeaderToASC(header);

            byte[] bytes = _util.FormatMessage(headerResult, dataFeildResult);
            return bytes;
        }

        public byte[] MID0701_Tool_List_Upload_Reply()
        {
            Header header = new Header();
            header.MID[0] = 48;
            header.MID[1] = 55;
            header.MID[2] = 48;
            header.MID[3] = 49;

            header.Revision[0] = 48;
            header.Revision[1] = 48;
            header.Revision[2] = 49;

            DataField dataField = new DataField();
            DesoutterResult dataFeildResult = _util.DataFieldToASC(dataField);

            var msgLength = 20 + dataFeildResult.Data.Length;
            header.Length = _util.StringToASCII(msgLength.ToString().PadLeft(4, '0'));

            DesoutterResult headerResult = _util.HeaderToASC(header);

            byte[] bytes = _util.FormatMessage(headerResult, dataFeildResult);
            return bytes;
        }

        //MID0082 同步时间
        public byte[] MID0082_Set_Time()
        {
            Header header = new Header();
            header.MID[0] = 48;
            header.MID[1] = 48;
            header.MID[2] = 56;
            header.MID[3] = 50;

            header.Revision[0] = 48;
            header.Revision[1] = 48;
            header.Revision[2] = 49;

            DataField dataField = new DataField();
            
            var stamp = DateTime.Now.ToString("yyyy-MM-dd:hh:mm:ss");
            List<string> param = new List<string>();
            param.Add(stamp);
            dataField.Parameter = param;
            DesoutterResult dataFeildResult = _util.DataFieldToASC(dataField);
            var msgLength = 20 + dataFeildResult.Data.Length;
            header.Length = _util.StringToASCII(msgLength.ToString().PadLeft(4, '0'));

            DesoutterResult headerResult = _util.HeaderToASC(header);

            byte[] bytes = _util.FormatMessage(headerResult, dataFeildResult);
            return bytes;
        }

        public byte[] MID9999_Keep_Alive_Message()
        {

            Header header = new Header();
            header.MID[0] = 57;
            header.MID[1] = 57;
            header.MID[2] = 57;
            header.MID[3] = 57;

            header.Revision[0] = 48;
            header.Revision[1] = 48;
            header.Revision[2] = 48;

            DataField dataField = new DataField();
            DesoutterResult dataFeildResult = _util.DataFieldToASC(dataField);

            var msgLength = 20 + dataFeildResult.Data.Length;
            header.Length = _util.StringToASCII(msgLength.ToString().PadLeft(4, '0'));

            DesoutterResult headerResult = _util.HeaderToASC(header);

            byte[] bytes = _util.FormatMessage(headerResult, dataFeildResult);
            return bytes;
        }

    }
}
