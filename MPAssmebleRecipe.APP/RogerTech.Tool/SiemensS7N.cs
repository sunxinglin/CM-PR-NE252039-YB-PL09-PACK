using HslCommunication;
using HslCommunication.Profinet.Siemens;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RogerTech.Tool
{
    class SiemensS7N : IComProtocol
    {
        private static object locker = new object();
        private readonly int retryTimes = 3;
        private SiemensS7Net siemensTcpNet;

        public string IP { get; private set; } = "127.0.0.1";

        public int Port
        {
            get;
            set;
        } = 102;

        public int TimeOut
        {
            get;
            set;
        } = 3000;
        public SiemensS7N(string ip)
        {
            this.IP = ip;

            siemensTcpNet = new SiemensS7Net(SiemensPLCS.S1500);
            siemensTcpNet.IpAddress = IP;
            siemensTcpNet.Port = Port;
            siemensTcpNet.ConnectTimeOut = TimeOut;
        }
        public bool Connected { get; private set; }

        public void Connect()
        {
            lock (locker)
            {
                if (Connected)
                {

                }
                else
                {
                    try
                    {
                        Connected = siemensTcpNet.ConnectServer().IsSuccess;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"In class SiemensS7,Func: Connect, Error:{ex.Message}");
                    }
                }
            }
        }

        public byte[] ReadBytes(int dbNr, int startAddr, int Length, out bool avilid)
        {
            lock (locker)
            {
                avilid = true;
                List<byte> bytes = new List<byte>();
                try
                {
                    if (Connected)
                    {
                        for (int i = 0; i < Length; i++)
                        {
                            string siteName = string.Format($"DB{dbNr}.{startAddr + i}");
                            OperateResult or = siemensTcpNet.ReadByte(siteName);
                            if (or.IsSuccess)
                            {
                                byte by = ((OperateResult<Byte>)or).Content;
                                bytes.Add(by);
                            }
                            else
                            {
                                avilid = false;
                                break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    avilid = false;
                    throw new Exception($"In SiemensS7 ReadBytes: {ex.Message}");
                }
                return bytes.ToArray();
            }
        }


        public void WriteBytes(int dbNr, int startAddr, byte[] content, out bool avilid)
        {
            lock (locker)
            {
                avilid = true;
                if (content != null)
                {
                    try
                    {
                        if (Connected)
                        {
                            string siteName = string.Format($"DB{dbNr}.{startAddr}");
                            OperateResult or = siemensTcpNet.Write(siteName, content);
                            if (or.IsSuccess)
                            {
                               
                            }
                            else
                            {
                                avilid = false;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        avilid = false;
                        throw new Exception($"In SiemensS7 WriteBytes: {ex.Message}");
                    }
                }
            }
        }


        public void WriteBit(int dbNr, int startAddr, byte[] content, byte bit, out bool avilid)
        {
            lock (locker)
            {
                avilid = true;
                if (content != null)
                {
                    try
                    {
                        if (Connected)
                        {
                            BitArray ba = new BitArray(content);
                            bool value = ba.Get(bit);
                            string siteName = string.Format($"DB{dbNr}.{startAddr}");
                            OperateResult or = siemensTcpNet.Write(siteName, value);
                            if (or.IsSuccess)
                            {
                              
                            }
                            else
                            {
                                avilid = false;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        avilid = false;
                        throw new Exception($"In SiemensS7 WriteBit: {ex.Message}");
                    }
                }
            }
        }


        public void WriteTag(Tag tag, byte[] bytes)
        {
            lock (locker)
            {
                if (tag!=null && bytes != null)
                {
                    try
                    {
                        if (Connected)
                        {
                            string siteName = string.Format($"DB{tag.Dbnr}.{tag.StartAddress}");
                            OperateResult or = siemensTcpNet.Write(siteName, bytes);
                            if (or.IsSuccess)
                            {
                              
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"In SiemensS7 WriteTag: {ex.Message}");
                    }
                }
            }
        }

        public void AddTag(Tag tag)
        {
           
        }

        public bool DisConnect()
        {
            try
            {
                if (siemensTcpNet != null)
                {
                    siemensTcpNet.ConnectClose();
                    siemensTcpNet = null;
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"In class SiemensS7,Func: DisConnect, Error:{ex.Message}");
            }
        }
    }
}
