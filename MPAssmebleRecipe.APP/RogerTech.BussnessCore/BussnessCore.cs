using System;
using System.Threading;
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
            this.PlcComm.PlcTriggerChanged += TriggerChanged;
            this.PlcComm.UserChanged += UserChanged;

            DbContext.ProgressErrorChange += ProgressErrorChange;
            DbContext.ProgressInfoChange += ProgressInfoChange;

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


        private void UserChanged(object sender, PlcCommArgs e)
        {
            ThreadPool.QueueUserWorkItem(OnUserChange, e);
        }

        private void TriggerChanged(object sender, PlcCommArgs e)
        {
            ThreadPool.QueueUserWorkItem(OnTriggerChange, e);
        }
        private void OnUserChange(object obj)
        {
            PlcCommArgs e = (PlcCommArgs)obj;
            if (BussnessDic.ProcessDics.ContainsKey(e.PlcGroup.GroupName))
            {
                string result = string.Empty;
                try
                {
                    result = (string)e.TagValue;
                }
                catch (Exception)
                {
                    ErrorOccured(e.PlcGroup.GroupName, "无法将PLC请求值转换为string类型");
                }
                if (string.IsNullOrEmpty(result))
                    return;
                IPlcInProcess plcProgress = BussnessDic.ProcessDics[e.PlcGroup.GroupName];
                if (plcProgress.ActionChanged?.GetInvocationList().Length > 0)
                {

                }
                else
                {
                    plcProgress.ActionChanged += PlcActionChange;
                }

                if (plcProgress.ErrorOccured?.GetInvocationList().Length > 0)
                {

                }
                else
                {
                    plcProgress.ErrorOccured += PlcErrorOccured;
                }
                plcProgress.Execute(e.PlcGroup);

            }
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
                    plcProgress.ActionChanged += PlcActionChange;
                }

                if (plcProgress.ErrorOccured?.GetInvocationList().Length > 0)
                {

                }
                else
                {
                    plcProgress.ErrorOccured += PlcErrorOccured;
                }
                plcProgress.Execute(e.PlcGroup);

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
