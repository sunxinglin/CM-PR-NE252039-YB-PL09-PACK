using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using RogerTech.Tool;
using SqlSugar;
using RogerTech.Common;
namespace RogerTech.BussnessCore
{
    public class BussnessCore
    {
        public PlcCommucation PlcComm { get; private set; }
        public BussnessDic BussnessDic { get; private set; }

        public BussnessCore()
        {
            //初始化数据库
            BussnessDic = new BussnessDic();
            this.PlcComm = new PlcCommucation(BussnessDic.PlcGroups, BussnessDic.PlcServer);
            this.PlcComm.PlcTriggerChanged += new EventHandler<PlcCommArgs>(TriggerChanged);
     

            DbContext.ProgressErrorChange += new Action<string>(ProgressErrorChange);
            DbContext.ProgressInfoChange += new Action(ProgressInfoChange);
      
        }

        private void ProgressInfoChange()
        {
            InfoChanged?.Invoke("", "");
        }

        private void ProgressErrorChange(string obj)
        {
            ErrorOccured?.Invoke("", obj);
        }

        public Action<string, string> InfoChanged;
        public Action<string, string> ErrorOccured;
 

        private void TriggerChanged(object sender, PlcCommArgs e)
        {         
            ThreadPool.QueueUserWorkItem(OnTriggerChange, e);
        }

        private void OnTriggerChange(object obj)
        {         
            PlcCommArgs e = (PlcCommArgs)obj;
            if (BussnessDic.ProcessDics.ContainsKey(e.PlcGroup.GroupName))
            {
                bool result = false;
                try
                {
                    result = (bool)e.TagValue;
                }
                catch (Exception)
                {
                    ErrorOccured(e.PlcGroup.GroupName, "无法将PLC请求值转换为Bool类型");
                }
                if (!result)
                    return;
                IPlcInProcess plcProgress = BussnessDic.ProcessDics[e.PlcGroup.GroupName];
                if (plcProgress.ActionChanged?.GetInvocationList().Length > 0)
                {

                }
                else
                {
                    plcProgress.ActionChanged += new Action<string, string>(PlcActionChange);
                }

                if (plcProgress.ErrorOccured?.GetInvocationList().Length > 0)
                {

                }
                else
                {
                    plcProgress.ErrorOccured += new Action<string, string>(PlcErrorOccured);
                }
                plcProgress.Excute(e.PlcGroup);

            }
        }

        private void PlcErrorOccured(string groupName, string message)
        {
            ErrorOccured(groupName, message);
        }

        private void PlcActionChange(string groupName, string message)
        {
            InfoChanged(groupName, message);
        }

    }
}
