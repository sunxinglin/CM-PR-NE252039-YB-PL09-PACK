using AsZero.Core.Services.Messages;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Yee.Entitys.AlarmMgmt;
using Yee.Entitys.Production;

namespace Ctp0600P.Client.Protocols.AnyLoad_Wifi
{

    public class AnyLoadCtrl
    {
        private readonly ILogger<AnyLoadCtrl> _logger;
        private readonly IMediator _mediator;

        public string? PortName { get; }
        public int Baud { get; }
        public SerialPort _serialPort { get; set; }
        volatile bool _keepReading;
        private readonly AnyLoadSendMessage _sendMessage;

        private readonly string ResultCommandStartStr = "FF AA ";
        private readonly List<string> ResultCommandEndStr = new List<string> { "30 30 F0 F0", "30 30 00 00", "30 30 F0 00", "30 30 00 F0" };
        private readonly string ResultCommandUnitStr = " 4B 47";
        private readonly string ResultUnitStr = "KG";


        public Base_ProResource _AnyLoadResource;
        public AnyLoadCtrl(Base_ProResource resource, ILogger<AnyLoadCtrl> logger, IMediator mediator, AnyLoadSendMessage sendMessage)
        {
            if (resource == null)
            {
                throw new ArgumentException("AnyLoad Resource is invaild");
            }
            _sendMessage = sendMessage;

            _AnyLoadResource = resource;
            _logger = logger;
            _mediator = mediator;
            PortName = _AnyLoadResource.Port;
            Baud = _AnyLoadResource.Baud;
            _serialPort = new SerialPort(_AnyLoadResource.Port, _AnyLoadResource.Baud);
        }

        public async Task ConnectAsync()
        {
            if (!string.IsNullOrEmpty(_AnyLoadResource.Port) && !string.IsNullOrEmpty(_AnyLoadResource.Baud.ToString()))
            {
                Open();
                _mediator.Publish(new UILogNotification(new LogMessage { Content = "连接电子秤[" + _AnyLoadResource.Name + ":" + _AnyLoadResource.Port + "]成功" }));
            }

        }



        public bool IsOpen
        {
            get
            {
                return _serialPort.IsOpen;
            }
        }

        public async void ReadPort()
        {
            ByteBuffer resultBuffe = new ByteBuffer();

            while (_keepReading)
            {
                try
                {
                    if (_serialPort.IsOpen)
                    {
                        if (_serialPort.BytesToRead == 0)
                            continue;
                        //int result = 0;
                        byte[] m_recvBytes = new byte[_serialPort.BytesToRead]; //定义缓冲区大小 
                        var responseTask = await _serialPort.BaseStream.ReadAsync(m_recvBytes, 0, m_recvBytes.Length); //从串口读取数据

                        //if (await Task.WhenAny(responseTask, Task.Delay(300)) == responseTask)
                        //{
                        //    result += responseTask.Result;
                        //}

                        //if (result > 0)
                        //{
                        //    resultBuffe.PushByteArray(m_recvBytes);
                        //}
                        if (responseTask > 0)
                        {
                            resultBuffe.PushByteArray(m_recvBytes);
                        }
                        var resultBytes = resultBuffe.ToByteArray();
                        if (resultBytes.Length >= 14)
                        {
                            var resultStr = _sendMessage.byteToHexStr(resultBytes);
                            var startStrindex = resultStr.LastIndexOf(ResultCommandStartStr);
                            if (startStrindex >= 0)
                            {
                                resultStr = resultStr.Substring(startStrindex);

                                if (resultStr.StartsWith(ResultCommandStartStr) && resultStr.Length >= 35)
                                {
                                    int endStrindex = -1;

                                    foreach (var endStr in ResultCommandEndStr)
                                    {
                                        endStrindex = resultStr.IndexOf(endStr);
                                        if (endStrindex >= 0) break;
                                    }

                                    if (endStrindex >= 0)
                                    {
                                        resultStr = resultStr.Substring(0, endStrindex);
                                        ///结束符在单位后面，此处只需判断结束符是否存在即可
                                        //resultStr = resultStr.Substring(0, resultStr.IndexOf(ResultCommandUnitStr));
                                        resultStr = resultStr.Substring(ResultCommandStartStr.Length);

                                        if (resultStr.Length == 18)
                                        {
                                            string valueStr = resultStr.Substring(0, 14);
                                            resultBytes = _sendMessage.strToToHexByte(valueStr);
                                            string strResult = Encoding.ASCII.GetString(resultBytes, 0, resultBytes.Length); //对数据进行转换 

                                            if (resultStr.Substring(15, 1) == "F")
                                            {
                                                //strResult = "-" + strResult;
                                                strResult ="0";
                                            }
                                            var pointCount = int.Parse(resultStr.Substring(16, 1));
                                            if (pointCount > 0)
                                            {
                                                strResult = strResult.Substring(0, strResult.Length - pointCount) + "." + strResult.Substring(strResult.Length - pointCount);
                                            }


                                            resultBuffe = new ByteBuffer();
                                            AnyLoadRequest request = new AnyLoadRequest
                                            {
                                                AnyLoadContext = double.Parse(strResult).ToString() + ResultUnitStr,
                                                AnyLoadPortName = PortName
                                            };
                                            await _mediator.Publish(request);
                                        }
                                    }
                                }
                            }
                        }
                        if (resultBuffe.Length > 90)
                            resultBuffe = new ByteBuffer();
                    }
                }
                catch (Exception ex)
                {

                    await _mediator.Publish(new AlarmSYSNotification() { Code = AlarmCode.称重错误, Name = AlarmCode.称重错误.ToString(), Module = AlarmModule.DESOUTTER_MODULE, Description = $"{PortName},{ex.Message}" });

                }
                await Task.Delay(500);
            }
        }

        public void Open()
        {
            try
            {
                Close();
                _keepReading = true;
                _serialPort.Open();
                if (!_serialPort.IsOpen)
                    _mediator.Publish(new AlarmSYSNotification() { Code = AlarmCode.称重错误, Name = AlarmCode.称重错误.ToString(), Module = AlarmModule.DESOUTTER_MODULE, Description = $"串口{PortName}打开失败！" });

            }
            catch (Exception ex)
            {
                _mediator.Publish(new AlarmSYSNotification() { Code = AlarmCode.称重错误, Name = AlarmCode.称重错误.ToString(), Module = AlarmModule.DESOUTTER_MODULE, Description = $"{PortName},{ex.Message}" });

            }
        }

        public void Close()
        {
            StopReading();
            _serialPort.Close();
        }

        private void StopReading()
        {
            if (_keepReading)
            {
                _keepReading = false;
            }
        }
        public void WritePort(byte[] send)
        {
            if (IsOpen)
            {
                _serialPort.Write(send, 0, send.Length);
            }
        }

        public Task ReadWeightData(byte[] message)
        {
            WritePort(message);
            return Task.CompletedTask;
        }
    }

}
