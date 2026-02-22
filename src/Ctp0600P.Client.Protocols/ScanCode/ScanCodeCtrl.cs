using AsZero.Core.Services.Messages;
using Ctp0600P.Client.Protocols.ScanCode.Models;
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

namespace Ctp0600P.Client.Protocols.ScanCode
{

    public class ScanCodeCtrl
    {
        private readonly ILogger<ScanCodeCtrl> _logger;
        private readonly IMediator _mediator;

        public string? PortName { get; }
        public int Baud { get; }
        public SerialPort _serialPort { get; set; }
        volatile bool _keepReading;
        private int ReadTimeSpan;

        public Base_ProResource _scanCodeResource;
        public ScanCodeCtrl(Base_ProResource resource, ILogger<ScanCodeCtrl> logger, IMediator mediator)
        {
            ReadTimeSpan = 500;

            if (resource == null)
            {
                throw new ArgumentException("ScanCode Resource is invaild");
            }

            _scanCodeResource = resource;
            _logger = logger;
            _mediator = mediator;
            PortName = _scanCodeResource.Port;
            Baud = _scanCodeResource.Baud;
            _serialPort = new SerialPort(_scanCodeResource.Port, _scanCodeResource.Baud);
        }

        public async Task ConnectAsync()
        {
            try
            {
                if (!string.IsNullOrEmpty(_scanCodeResource.Port) && !string.IsNullOrEmpty(_scanCodeResource.Baud.ToString()))
                {
                    Open();
                   await _mediator.Publish(new UILogNotification(new LogMessage { Timestamp = DateTime.Now, Content = "连接扫码枪[" + _scanCodeResource.Name + ":" + _scanCodeResource.Port + "]成功" }));
                }
            }
            catch (Exception ex)
            {

                throw;
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
                        if (_serialPort.BytesToRead == 0) continue;

                        int result = 0;
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
                        m_recvBytes = null;

                        var resultBytes = resultBuffe.ToByteArray();
                        if (resultBytes.Length > 1)
                        {
                            string strResult = Encoding.ASCII.GetString(resultBytes, 0, resultBytes.Length); //对数据进行转换 
                            if (strResult.Length > 0 && strResult.EndsWith("\r"))
                            {
                                resultBuffe.Initialize();
                                ScanCodeGunRequest request = new ScanCodeGunRequest
                                {
                                    ScanCodeContext = strResult.Replace("\r", ""),
                                    ScanCodePortName = PortName
                                };
                                await _mediator.Publish(request);
                            }
                            else if (strResult.Length > 0 && strResult.EndsWith("\t"))
                            {
                                resultBuffe.Initialize();
                                ScanCodeGunRequest request = new ScanCodeGunRequest
                                {
                                    ScanCodeContext = strResult.Replace("\n", "").Replace("\r", "").Replace("\t", ""),
                                    ScanCodePortName = PortName
                                };
                                await _mediator.Publish(request);
                            }
                        }
                        resultBytes = null;
                    }
                }
                catch (Exception ex)
                {
                    await _mediator.Publish(new AlarmSYSNotification() { Code = AlarmCode.扫码枪错误, Name = AlarmCode.扫码枪错误.ToString(), Module = AlarmModule.DESOUTTER_MODULE, Description = $"{PortName},{ex.Message}" });
                }
                await Task.Delay(ReadTimeSpan);
                await Task.CompletedTask;
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
                    _mediator.Publish(new AlarmSYSNotification() { Code = AlarmCode.扫码枪错误, Name = AlarmCode.扫码枪错误.ToString(), Module = AlarmModule.DESOUTTER_MODULE, Description = $"串口{PortName}打开失败！" });
                
            }
            catch (Exception ex)
            {
                _mediator.Publish(new AlarmSYSNotification() { Code = AlarmCode.扫码枪错误, Name = AlarmCode.扫码枪错误.ToString(), Module = AlarmModule.DESOUTTER_MODULE, Description  =$"{PortName},{ex.Message}" });
                
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
        public void WritePort(byte[] send, int offSet, int count)
        {
            if (IsOpen)
            {
                _serialPort.Write(send, offSet, count);
            }
        }
    }

}
