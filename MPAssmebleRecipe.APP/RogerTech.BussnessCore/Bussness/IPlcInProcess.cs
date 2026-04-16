using System;
using log4net;
using RogerTech.Tool;

namespace RogerTech.BussnessCore
{
    public interface IPlcInProcess
    {
        ILog Logger { get; }
        Group PlcGroup { get; set; }

        //任务开始
        Action<string,string> ActionChanged { get; set; }
        Action<string,string> ErrorOccured { get; set; }

        void Execute(Group group);
    }
}
