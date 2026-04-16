using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogerTech.Tool
{
    public static class CSVFileHelper
    {
        /// <summary>
        /// 将对象列表写入CSV文件
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="data">数据列表</param>
        /// <param name="filePath">文件路径</param>
        public static void WriteFromList<T>(IEnumerable<T> data, string filePath)
        {
            var sb = new StringBuilder();

            // 添加CSV标题行（属性名称）
            var properties = typeof(T).GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                sb.Append(properties[i].Name);
                if (i < properties.Length - 1) sb.Append(",");
            }
            sb.AppendLine();

            // 添加数据行
            foreach (var item in data)
            {
                for (int i = 0; i < properties.Length; i++)
                {
                    var value = properties[i].GetValue(item, null);
                    sb.Append(Convert.ToString(value, CultureInfo.InvariantCulture));   
                    if (i < properties.Length - 1) sb.Append(",");
                }
                sb.AppendLine();
            }
            var dir = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }

        /// <summary>
        /// 转义字符串中的双引号
        /// </summary>
        private static string EscapeQuotes(string value)
            => value.Replace("\"", "\"\"");
    }
}
