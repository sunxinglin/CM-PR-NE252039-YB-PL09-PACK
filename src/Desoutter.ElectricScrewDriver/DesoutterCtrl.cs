using AsZero.Core.Services.Messages;
using Ctp0600P.Client.Protocols;
using Desoutter.ElectricScrewDriver.MessageHandler;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Yee.Entitys.AlarmMgmt;
using Yee.Entitys.DTOS;
using Yee.Entitys.Production;

namespace Desoutter.ElectricScrewDriver
{

    public class DesoutterCtrl
    {
        private readonly ILogger<DesoutterCtrl> _logger;
        private readonly IMediator _mediator;
        private readonly SendMessage _sendMessage;
        private readonly RecvMessage _recvMessage;
        public TcpClient _tcpClient { get; set; }

        /// <summary>
        /// 拧紧枪设备号
        /// </summary>
        public string DeviceNo { get; set; }

        public Base_ProResource _desoutterResource;

        public bool HasAlarm { get; set; } = false;
        public DesoutterCtrl(Base_ProResource resource, ILogger<DesoutterCtrl> logger, IMediator mediator, SendMessage sendMessage, RecvMessage recvMessage)
        {
            if (resource == null)
            {
                throw new ArgumentException("Desoutter Resource is invaild");
            }
            DeviceNo = resource.DeviceNo;
            _desoutterResource = resource;
            DeviceNo = resource.DeviceNo;
            _logger = logger;
            _mediator = mediator;
            _sendMessage = sendMessage;
            _recvMessage = recvMessage;
            _tcpClient = new TcpClient();
        }

        public async Task ConnectAsync()
        {

            if (!string.IsNullOrEmpty(_desoutterResource.IpAddress) && !string.IsNullOrEmpty(_desoutterResource.Port))
            {
                try
                {
                    _tcpClient.Close();
                    _tcpClient = new TcpClient();
                    await _tcpClient.ConnectAsync(_desoutterResource.IpAddress, int.Parse(_desoutterResource.Port));
                    await _mediator.Publish(new UILogNotification(new MidLogMessage { Content = "连接拧紧枪[" + _desoutterResource.Name + ":" + _desoutterResource.IpAddress + "]成功" }));
                    await this.Send("MID0001_CommunicationStart", _sendMessage.MID0001_CommunicationStart());

                    await _mediator.Publish(new UILogNotification(new LogMessage { Timestamp = DateTime.Now, Content = "连接拧紧枪[" + _desoutterResource.Name + "]成功" }));
                }
                catch(Exception ex)
                {
                    //await _mediator.Publish(new AlarmSYS() { Code = AlarmCode.拧紧枪错误, Name = AlarmCode.拧紧枪错误.ToString(), Module = AlarmModule.DESOUTTER_MODULE, Description = $"{_desoutterResource.Name},无法连接，请检查拧紧枪" });
                }
            }

        }

        public async Task<byte[]> Read()
        {
            Byte[] readBuffer = new Byte[4098];
            try
            {
                if (_tcpClient.Available > 0)
                {
                    var stream = _tcpClient.GetStream();

                    await stream.ReadAsync(readBuffer, 0, 4098);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            return readBuffer;
        }

        public async Task Send(string messageType, byte[] bytes)
        {
            try
            {
                if (!_tcpClient.Connected)
                {
                    await ConnectAsync();
                }
                NetworkStream stream = _tcpClient.GetStream();
                if (stream.CanWrite)
                {
                    await stream.WriteAsync(bytes, 0, bytes.Length);
                    await this.LogMessage(DesoutterSendOrRecv.Send, messageType, bytes);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            return;
        }

        public async Task SendNoLog(string messageType, byte[] bytes)
        {
            try
            {
                NetworkStream stream = _tcpClient.GetStream();
                if (stream.CanWrite)
                {
                    await stream.WriteAsync(bytes, 0, bytes.Length);
                    //await this.LogMessage(DesoutterSendOrRecv.Send, messageType, bytes);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            return;
        }

        public async Task LogMessage(DesoutterSendOrRecv sr, string messageType, Byte[] bytes)
        {
            string log = "【" + messageType + "】【";
            for (int i = 0; i < bytes.Length; i++)
            {
                log += bytes[i].ToString();
            }
            string SendOrRecv = string.Empty;
            if (sr == DesoutterSendOrRecv.Send)
            {
                SendOrRecv = "【Send】";
            }
            else if (sr == DesoutterSendOrRecv.Recv)
            {
                SendOrRecv = "【Recv】";
            }
            else
            {
                SendOrRecv = string.Empty;
            }
            log += "】【";

            var devName = "【" + _desoutterResource.Name + "】";

            var msg = devName + SendOrRecv + log + this.ASCIIToString(bytes) + "】";
            _logger.LogInformation(msg);
            await _mediator.Publish(new UILogNotification(new MidLogMessage { Content = msg }));

        }

        private byte[] StringToASCII(string str)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            char[] chars = str.ToArray();
            byte[] bytes = new byte[chars.Length];
            bytes = encoding.GetBytes(chars);
            return bytes;
        }

        private string ASCIIToString(byte[] bytes)
        {
            string str = string.Empty;
            ASCIIEncoding encoding = new ASCIIEncoding();
            for (int i = 0; i < bytes.Length; i++)
            {
                char[] buf = encoding.GetChars(bytes);
                str += buf[i];
            }
            return str;

        }

        public async Task DispatchMessage(byte[] bytes)
        {
            if (bytes[0] != 0)
            {
                int index = 0;
                for (index = 0; index < bytes.Length; index++)
                {
                    if (bytes[index] == 0)
                    {
                        break;
                    }
                }
                byte[] message = bytes.Skip(0).Take(index).ToArray();

                byte[] byteMID = message.Skip(4).Take(4).ToArray();

                string strMID = this.ASCIIToString(byteMID);
                switch (strMID)
                {
                    case DesoutterMessage.CommunicationStartAcknowledge:
                        await SendSubcribe();
                        break;
                    case DesoutterMessage.ApplicationCommunicationNegativeAcknowledge:
                        break;
                    case DesoutterMessage.CommandAccepted:
                        break;
                    case DesoutterMessage.ParameterSetSelected:
                        await this.Send("MID0016_ParameterSetSelectedAcknowledge", _sendMessage.MID0016_Parameter_Set_Selected_Acknowledge());
                        break;
                    case DesoutterMessage.Alarm:
                        MID0071_AlarmNotifaction notifaction71 = new MID0071_AlarmNotifaction();
                        notifaction71.DeviceNo = _desoutterResource.DeviceNo;
                        notifaction71.Alarm = _recvMessage.MID0071_Alarm(message);

                        await this.Send("MID0072_AlarmAcknowledge", _sendMessage.MID0072_Alarm_Acknowledge());
                        break;
                    case DesoutterMessage.AlarmStatus:
                        await this.Send("MID0077_AlarmStatusAcknowledge", _sendMessage.MID0077_Alarm_Status_Acknowledge());
                        break;
                    case DesoutterMessage.LastTighteningResultData:
                        await this.LogMessage(DesoutterSendOrRecv.Recv, "MID61_LastTighteningResultData", message);
                        await this.Send("MID0062_LastTighteningResultDataAcknowledge", _sendMessage.MID0062_Last_Tightening_Result_Data_Acknowledge());
                        await PublishTightenData(message);
                        break;
                    case DesoutterMessage.KeepAliveMessage:
                        break;
                    default:
                        await this.LogMessage(DesoutterSendOrRecv.Recv, strMID, message);
                        break;
                }
            }
            return;
        }

        public List<Byte[]> SplitMessage(Byte[] bytes)
        {
            List<Byte[]> listBytes = new List<Byte[]>();

            int lastIndex = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                if (bytes[i] == 0)
                {
                    byte[] tmp = new byte[1024];
                    Array.Copy(bytes, lastIndex, tmp, 0, i - lastIndex);
                    listBytes.Add(tmp);
                    lastIndex = i + 1;
                }
                if (bytes[i] == 0 && bytes[i + 1] == 0)
                {
                    break;
                }
            }

            return listBytes;
        }

        //public async Task DoWork(int pSet)
        //{
        //    await this.Send("MID0043_EnableTool", _sendMessage.MID0043_Enable_Tool());
        //    await this.Send("MID0043_MID0018SelectParameterSet:" + "{" + pSet.ToString().PadLeft(3, '0') + "}", _sendMessage.MID0018_Select_Parameter_Set(pSet));
        //}
        public async Task SendSubcribe()
        {
            //马头订阅
            switch(this._desoutterResource.DeviceBrand)
            {
                case DeviceBrand.马头:
               
                    await this.Send("MID0060_LastTighteningResultDataSubscribe", _sendMessage.MID0060_Last_Tightening_Result_Data_Subscribe());
                    await _mediator.Publish(new UILogNotification(new LogMessage { Timestamp = DateTime.Now, Content = "马头拧紧结果订阅成功" }));
                    break;
                case DeviceBrand.博世:
                    await this.Send("MID0060_LastTighteningResultDataSubscribe", _sendMessage.MID0060_Last_Tightening_Result_Data_Subscribe_BS());
                    await _mediator.Publish(new UILogNotification(new LogMessage { Timestamp = DateTime.Now, Content = "博世拧紧结果订阅成功" }));
                    break;
                default:
                    return;
            }
            //博世订阅拧紧结果
            
        }

        public async Task Enable(int pSet)
        {
            await this.Send("MID0018_SelectParameterSet:" + "{" + pSet.ToString().PadLeft(3, '0') + "}", _sendMessage.MID0018_Select_Parameter_Set(pSet));
            await this.Send("MID0043_EnableTool", _sendMessage.MID0043_Enable_Tool());
        }

        public async Task Disable()
        {
            await this.Send("MID0042_DisableTool", _sendMessage.MID0042_Disable_Tool());
        }

        public async Task SendAlarm(string stationCode)
        {
            //AlarmDTO alarmDTO = new AlarmDTO();
            //alarmDTO.StepCode = stationCode;
            //alarmDTO.Code = AlarmCode.拧紧枪连接失败 ;
            //alarmDTO.Name = "拧紧枪【" + this.DeviceNo + "】";
            //alarmDTO.Module = AlarmModule.DESOUTTER_MODULE;
            //alarmDTO.Description = "拧紧枪无法连接，请检查拧紧枪";
            //var alarmNotification = new Alarm_IONotification();
            //alarmNotification.action = AlarmAction.Occur;
            //alarmNotification.alarmDTO = alarmDTO;
            //await this._mediator.Publish(alarmNotification);

            await _mediator.Publish(new AlarmSYSNotification() { Code = AlarmCode.拧紧枪错误, Name = AlarmCode.拧紧枪错误.ToString(), Module = AlarmModule.DESOUTTER_MODULE, Description = $"{_desoutterResource.Name},无法连接，请检查拧紧枪" });
            
        }

        public async Task ClearAlarm(string stationCode)
        {
            AlarmDTO alarmDTO = new AlarmDTO();
            alarmDTO.StepCode = stationCode;
            alarmDTO.Code = AlarmCode.拧紧枪连接失败;
            alarmDTO.Name = "拧紧枪【" + this.DeviceNo + "】";
            alarmDTO.Module = AlarmModule.DESOUTTER_MODULE;
            alarmDTO.Description = "拧紧枪无法连接，请检查拧紧枪";
            var alarmNotification = new Alarm_IONotification();
            alarmNotification.action = AlarmAction.Clear;
            alarmNotification.alarmDTO = alarmDTO;
            await this._mediator.Publish(alarmNotification);
        }

        public async Task PublishTightenData(byte[] message)
        {
            switch (this._desoutterResource.DeviceBrand)
            {
                case DeviceBrand.马头:
                    MID0061_LastTighteningResultDataNotifaction notifaction61_mt = new MID0061_LastTighteningResultDataNotifaction();
                    notifaction61_mt.DeviceNo = _desoutterResource.DeviceNo;
                    notifaction61_mt.LastTighteningResultData = _recvMessage.MID0061_Last_Tightening_Result_Data(message);
                    await _mediator.Publish(notifaction61_mt);
                   
                    break;
                case DeviceBrand.博世:
                    MID0061_LastTighteningResultData_BSNotifaction notifaction61_bs = new MID0061_LastTighteningResultData_BSNotifaction();
                    notifaction61_bs.DeviceNo = _desoutterResource.DeviceNo;
                    notifaction61_bs.LastTighteningResultData = _recvMessage.MID0061_Last_Tightening_Result_Data_BS(message);
                    await _mediator.Publish(notifaction61_bs);
                    break;
                default: 
                    return;
            }
        }

    }

}
