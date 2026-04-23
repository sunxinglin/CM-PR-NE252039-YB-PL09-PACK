using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetParametricValue.GetParametricValue;
using log4net;
using RogerTech.Common;
using RogerTech.Tool;

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
        protected readonly string StationName = ConfigurationManager.AppSettings["StationName"];
        protected bool bSimulation = false;

        private readonly string _filePath = Directory.GetCurrentDirectory();
        private readonly string _fileName = "MESCFG.ini";
        protected string fullName;
        public PlcInProgressBase(string groupName, MesInterface mesInterface)
        {
            Logger = LogManager.GetLogger("ErrorLog");
            bool.TryParse(Simulation, out bSimulation);
            fullName = Path.Combine(_filePath, _fileName);
        }
        
        public virtual void Execute(Group group)
        {
            this.PlcGroup = group;
            string message = string.Empty;
            // Console.WriteLine($"Plc触发值改变:[{PlcGroup.GroupName}],And HashCodeIS: [{group.GetHashCode()}]");
            Logger.Info(message);
        }

        /// <summary>
        /// 统一读取并校验“必填字符串点位”
        /// </summary>
        /// <param name="group">config</param>
        /// <param name="tagName">标签名</param>
        /// <param name="message">日志消息</param>
        /// <param name="resultCode">回写给PLC的代码</param>
        /// <param name="readErrorCode">读取异常代码</param>
        /// <param name="emptyErrorCode">空值异常代码</param>
        /// <param name="readMessage">读取异常消息</param>
        /// <param name="emptyMessage">空值异常消息</param>
        /// <param name="value">Tag对应的值</param>
        /// <returns>true：取值成功且非空；false：已写入message与resultCode，调用方直接return即可</returns>
        protected bool TryGetRequiredStringTagValue(
            Group group,
            string tagName,
            StringBuilder message,
            ref int resultCode,
            int readErrorCode,
            int emptyErrorCode,
            string readMessage,
            string emptyMessage,
            out string value)
        {
            value = string.Empty;

            // 1.点位是否存在：若配置缺失，记录具体缺失点位与配置文件路径
            Tag tag = group.GetTag(tagName);
            if (tag == null)
            {
                OnTagNullError(tagName, group.GroupName);
                resultCode = readErrorCode;
                return false;
            }

            // 2.本轮读取是否有效：Available=false 通常表示通讯异常/读失败/长度不匹配
            if (!tag.Result.Available)
            {
                if (!string.IsNullOrEmpty(readMessage))
                {
                    message.Append(readMessage);
                }
                resultCode = readErrorCode;
                return false;
            }

            // 3.取值与判空：拦截 null、空串、全空格
            value = tag.Result.Value?.ToString() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(value))
            {
                if (!string.IsNullOrEmpty(emptyMessage))
                {
                    message.Append(emptyMessage);
                }
                resultCode = emptyErrorCode;
                return false;
            }

            return true;
        }
        
        protected void OnTagNullError(string tagName, string groupName)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "configs");
            string fullName = Path.Combine(filePath, $"{groupName}.csv");
            string message = $"PLC变量;[{tagName}]不存在，请检查配置文件:{fullName}";

            DbContext.Info(tagName, message, 9999, PlcGroup.GroupName);
            Logger.Error(message);
        }
        protected void WriteResult(object[] result,string GetParametricValueUpMesCode)
        {
            GetParametricValueData[] dataArray = result.Cast<GetParametricValueData>().ToArray();
            for (int i = 1; i <= result.Length; i++)
            {
                Tag ResCode = PlcGroup.GetTag(GetParametricValueUpMesCode + i);
                if (ResCode == null)
                {
                    OnTagNullError(GetParametricValueUpMesCode + i, PlcGroup.GroupName);
                }
                else
                {
                    if (dataArray[i - 1].parameter == ResCode.TagName)
                    {
                        int currentIndex = i;
                        string currentTagName = GetParametricValueUpMesCode + i;
                        string currentValue = dataArray[i - 1].value;

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


        protected void WriteResult(string result, string tagName)
        {
            Tag Code = PlcGroup.GetTag(tagName);
            if (Code == null)
            {
                OnTagNullError(tagName, PlcGroup.GroupName);
            }
            else
            {
                Code.WriteValue(result);
                Task.Run(() => { DbContext.Info("Code", $"写入PLC成功:{result}", 0, PlcGroup.GroupName); });
            }
        }
        protected void WriteResult(int result, string tagName)
        {
            Tag Code = PlcGroup.GetTag(tagName);
            if (Code == null)
            {
                OnTagNullError(tagName, PlcGroup.GroupName);
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
