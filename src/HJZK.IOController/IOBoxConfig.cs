using NModbus;
using System.Net.Sockets;

namespace HJZK.IOController
{
    public class IOBoxConfig
    {
        public BoxTypeEnum BoxType { get; set; }
        public string StepCode { get; set; }
        public bool Enable { get; set; }
        public string Name { get; set; }
        public string IP { get; set; }
        public int Port { get; set; }
        public byte SlaveAddr { get; set; } = 2;

        public List<DoItem> DoItems { get; set; }
        public List<DiItem> DiItems { get; set; }

        public TcpClient Client { get; set; }//IO盒的TCP连接
        public IModbusMaster Master { get; set; }

        // public IOStatues IOStatues { get; set; } = new IOStatues();
    }

    public class DoItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public int Adress { get; set; }
        public string Describe { get; set; } = "";
        public bool statue { get; set; }
    }

    public class DiItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string ControlName { get; set; } = "";
        public int Adress { get; set; }
        public string Describe { get; set; } = "";
        public bool statue { get; set; }
    }


    public enum BoxTypeEnum
    {
        通用IO盒子 = 1,
        单独放行IO盒子 = 2
    }
}