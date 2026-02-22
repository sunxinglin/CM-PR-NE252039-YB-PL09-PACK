using GetParametricValue.GetParametricValue;
using log4net;
using RogerTech.Common;
using RogerTech.Tool;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogerTech.BussnessCore
{
    public class PlcInProgressBase : IPlcInProcess
    {
        public ILog Logger { get; }

        public Group PlcGroup { get; set; }

        public Action<string, string> ActionChangedErrorOccured { get; set; }


        public Action<string, string> ErrorOccured { get; set; }

        public Action<string, string> ActionChanged { get; set; }


        protected string Simulation = ConfigurationManager.AppSettings["Simulation"];
        protected bool bSimulation = false;

        string filePath = Directory.GetCurrentDirectory();
        string fileName = "MESCFG.ini";
        protected string fullName;
        public PlcInProgressBase(string groupName, MesInterface mesInterface)
        {
            Logger = log4net.LogManager.GetLogger("ErrorLog");
            bool.TryParse(Simulation, out bSimulation);
            fullName = Path.Combine(filePath, fileName);
        }
        public virtual void Excute(Group group)
        {
            this.PlcGroup = group;
            string message = string.Empty;
            // Console.WriteLine($"Plc触发值改变:[{PlcGroup.GroupName}],And HashCodeIS: [{group.GetHashCode()}]");
            Logger.Info(message);
        }
        protected void OnTagNullError(string tagName, string groupName)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "configs");
            string fullName = Path.Combine(filePath, $"{groupName}.csv");
            string message = $"PLC变量;[{tagName}]不存在，请检查配置文件:{fullName}";

            DbContext.Info(tagName, message, 9999, PlcGroup.GroupName);
            Logger.Error(message);
        }

        protected void WriteResult(object[] result)
        {
            GetParametricValueData[] dataArray = result.Cast<GetParametricValueData>().ToArray();
            for (int i = 1; i <= result.Count(); i++)
            {
                Tag ResCode = PlcGroup.GetTag("GCCS" + i);
                if (ResCode == null)
                {
                    OnTagNullError("GCCS" + i, PlcGroup.GroupName);
                }
                else
                {
                    if (dataArray[i - 1].parameter == ResCode.TagName)
                    {
                        int currentIndex = i;
                        string currentTagName = "GCCS" + i;
                        string currentValue = dataArray[i - 1].value.ToString();

                        ResCode.WriteValue(currentValue);

                        Task.Run(() =>
                        {
                            DbContext.Info(currentTagName, $"写入PLC成功:{currentValue}", 0, PlcGroup.GroupName);
                        });
                    }
                }
            }
        }

        protected void WriteResult(int result)
        {
            Tag ResCode = PlcGroup.GetTag("ResCode");
            if (ResCode == null)
            {
                OnTagNullError("ResCode", PlcGroup.GroupName);
            }
            else
            {
                ResCode.WriteValue(result);
                Task.Run(() => { DbContext.Info("ResCode", $"写入PLC成功:{result}", 0, PlcGroup.GroupName); });
            }
        }

        protected void WriteResult(string result)
        {
            Tag Code = PlcGroup.GetTag("Code");
            if (Code == null)
            {
                OnTagNullError("Code", PlcGroup.GroupName);
            }
            else
            {
                Code.WriteValue(result);
                Task.Run(() => { DbContext.Info("Code", $"写入PLC成功:{result}", 0, PlcGroup.GroupName); });
            }
        }


        protected void WriteFinishSignal(bool result)
        {
            Tag FinishSignal = PlcGroup.GetTag("FinishSignal");

            if (FinishSignal == null)
            {
                OnTagNullError("FinishSignal", PlcGroup.GroupName);
            }
            else
            {
                FinishSignal.WriteValue(result);
                Task.Run(() => { DbContext.Info("FinishSignal", $"写入PLC成功:{result}", 0, PlcGroup.GroupName); });
            }

        }
    }
}
