using System;
using System.Collections.Generic;
using System.Threading;
using System.Collections;

namespace RogerTech.Tool
{
    class SiemensS7 : IComProtocol
    {

        libnodave.daveOSserialType fds;
        libnodave.daveInterface di;
        libnodave.daveConnection dc;
        int rack = 0;
        int slot = 1;
        int Port = 102;
        //300-400 slot=2, 1200/1500 slot=1;

        private static object locker = new object();

        private readonly int retryTimes = 3;

        public string IP { get; private set; }
        public SiemensS7(string ip)
        {
            this.IP = ip;
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
                        //fds.rfd = libnodave.closeSocket(102);
                        fds.rfd = libnodave.closeSocket(Port);
                        Thread.Sleep(1000);
                        fds.rfd = libnodave.openSocket(Port, IP);
                        fds.wfd = fds.rfd;
                        if (fds.rfd > 0)
                        {
                            di = new libnodave.daveInterface(fds, "IF1", 0, libnodave.daveProtoISOTCP, libnodave.daveSpeed187k);
                            di.setTimeout(3000);
                            dc = new libnodave.daveConnection(di, 0, rack, slot);
                            Connected = (dc.connectPLC() == 0);
                        }
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
                        int res = -1;

                        res = dc.readBytes(libnodave.daveDB, dbNr, startAddr, Length, null);
                        if (res == 0)
                        {
                            for (int i = 0; i < Length; i++)
                            {
                                bytes.Add((byte)dc.getS8());
                            }
                            Connected = true;
                        }
                        else
                        {

                            Connected = (dc.connectPLC() == 0);
                            avilid = Connected ? true : false;
                            return bytes.ToArray();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Connected = (dc.connectPLC() == 0);
                    avilid = Connected ? true : false;
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
                            //if (bytes.Length != tag.DataLength)
                            //return;
                            int res = dc.writeBytes(libnodave.daveDB, dbNr, startAddr, content.Length, content);
                            if (res == 0)
                            {
                                Connected = true;
                            }
                            else
                            {
                                Connected = (dc.connectPLC() == 0);
                                avilid = Connected ? true : false;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Connected = (dc.connectPLC() == 0);
                        avilid = Connected ? true : false;
                        throw new Exception($"In SiemensS7 ReadBytes: {ex.Message}");
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
                            //if (bytes.Length != tag.DataLength)
                            //return;
                            BitArray ba = new BitArray(content);
                            bool value = ba.Get(bit);
                            //  int res = dc.writeBits(libnodave.daveDB, dbNr, startAddr, content.Length, content);
                            int res = dc.writeBits(libnodave.daveDB, dbNr, startAddr * 8 + bit, 1, value ? new byte[] { 0x1 } : new byte[] { 0x00 });
                           
                            //dc.writeBits(address.Area, address.DBNumber, address.Start * 8 + address.Bit, 1, b  it ? new byte[] { 0x1 } : new byte[] { 0x00 });
                            // dc.writeBits(address.Area, address.DBNumber, address.Start * 8 + address.Bit, 1, value ? new byte[] { 0x1 } : new byte[] { 0x00 });
                            if (res == 0)
                            {
                                Connected = true;
                            }
                            else
                            {
                                Connected = (dc.connectPLC() == 0);
                                avilid = Connected ? true : false;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Connected = (dc.connectPLC() == 0);
                        avilid = Connected ? true : false;
                        throw new Exception($"In SiemensS7 ReadBytes: {ex.Message}");
                    }
                }
            }
        }


        public void WriteTag(Tag tag, byte[] bytes)
        {

        }

        public void AddTag(Tag tag)
        {
            throw new NotImplementedException();
        }
    }
}
