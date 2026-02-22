using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;

namespace CatlMesBase
{
    public class MesModel
    {
        protected string FileName;
        protected string SectionName;
        public MesModel(string FileName, string SectionName)
        {
            this.FileName = FileName;
            this.SectionName = SectionName;
        }
        public virtual void ReadConfig()
        {

        }
        protected void Initialize()
        {
            if (File.Exists(FileName))
            {
                //判断section是否存在
                string[] sections = IniFileHelper.ReadSections(FileName);
                if(sections == null)
                {
                    using (FileStream fs = new FileStream(FileName, FileMode.Append))
                    {
                        using (StreamWriter sw = new StreamWriter(fs))
                        {
                            sw.WriteLine();
                            sw.WriteLine($"[{SectionName}]");
                            Type t = this.GetType();
                            PropertyInfo[] infos = t.GetProperties();
                            foreach (var item in infos)
                            {
                                bool value = IniFileHelper.WriteValue(SectionName, item.Name, "1", FileName);
                                Console.WriteLine($"Value:{value}, Name:{item.Name}");
                                string valueStr = item.Name.Replace("Field", "");
                                sw.WriteLine($"{valueStr}=");
                            }
                        }
                    }
                    return;
                }
                if (sections.ToList().Contains(SectionName))
                {

                }
                else
                {
                    using (FileStream fs = new FileStream(FileName, FileMode.Append))
                    {
                        using (StreamWriter sw = new StreamWriter(fs))
                        {
                            sw.WriteLine();
                            sw.WriteLine($"[{SectionName}]");
                            Type t = this.GetType();
                            PropertyInfo[] infos = t.GetProperties();
                            foreach (var item in infos)
                            {
                                bool value = IniFileHelper.WriteValue(SectionName, item.Name, "1", FileName);
                                Console.WriteLine($"Value:{value}, Name:{item.Name}");
                                string valueStr = item.Name.Replace("Field", "");
                                sw.WriteLine($"{valueStr}=");
                            }
                        }
                    }
                }
            }
            else
            {
                using (FileStream fs = new FileStream(FileName, FileMode.Create))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        sw.WriteLine();
                        sw.WriteLine($"[{SectionName}]");
                        Type t = this.GetType();
                        PropertyInfo[] infos = t.GetProperties();
                        foreach (var item in infos)
                        {
                            bool value = IniFileHelper.WriteValue(SectionName, item.Name, "1", FileName);
                            Console.WriteLine($"Value:{value}, Name:{item.Name}");
                            string valueStr = item.Name.Replace("Field", "");
                            sw.WriteLine($"{valueStr}=");
                        }
                    }
                }
            }
        }
    }
}
