using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RogerTech.Tool;
using log4net;

namespace RogerTech.BussnessCore
{
    public interface IPlcInProcess
    {
        ILog Logger { get; }
        Group PlcGroup { get; set; }

        //任务开始
        Action<string,string> ActionChanged { get; set; }
        Action<string,string> ErrorOccured { get; set; }

        void Excute(Group group);
    }
}
