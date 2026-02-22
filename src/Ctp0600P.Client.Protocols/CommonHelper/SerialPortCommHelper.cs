using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ctp0600P.Client.Protocols
{
    public class SerialPortCommHelper
    {
        public delegate void EventHandle(string content, string portName);
        public event EventHandle DataReceived;

        public delegate void ErrorEventHandler(string message);
        public event ErrorEventHandler ComError;

        public SerialPort serialPort;
        Thread thread;
        volatile bool _keepReading;
        private string PortName;
        private int ReadTimeSpan;

        public SerialPortCommHelper(string portName, int baudRate)
        {
            ReadTimeSpan = 300;
            PortName = portName;
            serialPort = new SerialPort(PortName, baudRate);
            //数据位
            serialPort.DataBits = 8;
            //两个停止位
            serialPort.StopBits = System.IO.Ports.StopBits.One;
            //无奇偶校验位
            serialPort.Parity = System.IO.Ports.Parity.None;
            serialPort.ReadTimeout = 100;
            serialPort.WriteTimeout = -1;

            thread = null;
            _keepReading = false;
        }

        public bool IsOpen
        {
            get
            {
                return serialPort.IsOpen;
            }
        }

        private void StartReading()
        {
            if (!_keepReading)
            {
                _keepReading = true;
                thread = new Thread(new ThreadStart(ReadPort));
                thread.IsBackground = true;
                thread.Start();
            }
        }

        private void StopReading()
        {
            if (_keepReading)
            {
                _keepReading = false;
                thread.Join();
                thread = null;
            }
        }

        private void ReadPort()
        {
            ByteBuffer resultBuffe = new ByteBuffer();

            while (_keepReading)
            {
                try
                {
                    if (serialPort.IsOpen)
                    {
                        byte[] m_recvBytes = new byte[serialPort.BytesToRead]; //定义缓冲区大小 
                        int result = serialPort.Read(m_recvBytes, 0, m_recvBytes.Length); //从串口读取数据
                        if (result > 0)
                        {
                            resultBuffe.PushByteArray(m_recvBytes);
                        }

                        var resultBytes = resultBuffe.ToByteArray();
                        if (resultBytes.Length > 1)
                        {
                            string strResult = Encoding.ASCII.GetString(resultBytes, 0, resultBytes.Length); //对数据进行转换 
                            if (this.DataReceived != null && strResult.Length > 0 && strResult.EndsWith("\r"))
                            {
                                resultBuffe = new ByteBuffer();

                                this.DataReceived(strResult, PortName);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (this.ComError != null)
                        ComError($"{PortName},{ex.Message}");
                }
                Task.Delay(ReadTimeSpan);
            }
        }

        public void Open()
        {
            try
            {
                Close();
                serialPort.Open();
                if (serialPort.IsOpen)
                {
                    StartReading();
                }
                else
                {
                    if (this.ComError != null)
                        ComError($"串口{PortName}打开失败！");
                }
            }
            catch (Exception ex)
            {
                if (this.ComError != null)
                    ComError($"{PortName},{ex.Message}");
            }
        }

        public void Close()
        {
            StopReading();
            serialPort.Close();
        }

        public void WritePort(byte[] send, int offSet, int count)
        {
            if (IsOpen)
            {
                serialPort.Write(send, offSet, count);
            }
        }
    }


}
