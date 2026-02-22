using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace CatlMesBase
{
    public class MesInvokeBase:IMesInterface
    {
        ILog logger = LogManager.GetLogger("");
        protected TxtFile txtFile;
        public string ConfigFileName { get; protected set; }
        public string SectionNmme { get; protected set; }

        public string InterfaceName { get; protected set; }
        public string ParmamsConfigFile { get; protected set; }
        public MesModel Model { get; protected set; }
        public List<KeyValuePair<string, string>> paramsDic;
        public MesInvokeBase(string configfile, string SectionName, string interfaceName, string paramsconigfile ="")
        {
            this.ConfigFileName = configfile;
            this.SectionNmme = SectionName;
            this.InterfaceName = interfaceName;
            txtFile = new TxtFile("D:\\MESLOG");
            if (string.IsNullOrEmpty(paramsconigfile))
            {
               
            }
            else
            {
                paramsDic = new List<KeyValuePair<string, string>>();
                //read key value paries form configfile
            }
            //读取配置信息
            this.Model = new MesModel(configfile,SectionName);
            Model.ReadConfig();
        }
        public Dictionary<string, object> GetConfigValues()
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            //通过反射读取model的全部属性
            string[] keys = IniFileHelper.ReadKeys(SectionNmme,ConfigFileName);
            foreach (var item in keys)
            {
                object value = IniFileHelper.ReadValue(SectionNmme,item,ConfigFileName);
                result.Add(item, value);
            }
            return result;
        }
        public void SetConfigValue(string key, object value)
        {
            IniFileHelper.WriteValue(SectionNmme, key, value.ToString(), ConfigFileName);
        }
        public virtual List<object> GetResult(List<object> objs)
        {
            throw new NotImplementedException();
        }

        public virtual List<object> GetResultSimulation(List<object> objs)
        {

            throw new NotImplementedException();
        }

    }
}
