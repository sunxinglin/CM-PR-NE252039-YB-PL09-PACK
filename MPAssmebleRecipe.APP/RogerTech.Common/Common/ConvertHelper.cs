using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace RogerTech.Common
{
    /// <summary>
    /// 处理数据类型转换，数制转换、编码转换相关的类
    /// </summary>    
    public sealed class ConvertHelper
    {
        #region 补足位数
        /// <summary>
        /// 指定字符串的固定长度，如果字符串小于固定长度，
        /// 则在字符串的前面补足零，可设置的固定长度最大为9位
        /// </summary>
        /// <param name="text">原始字符串</param>
        /// <param name="limitedLength">字符串的固定长度</param>
        public static string RepairZero(string text, int limitedLength)
        {
            //补足0的字符串
            string temp = "";

            //补足0
            for (int i = 0; i < limitedLength - text.Length; i++)
            {
                temp += "0";
            }

            //连接text
            temp += text;

            //返回补足0的字符串
            return temp;
        }
        #endregion

        #region 各进制数间转换
        /// <summary>
        /// 实现各进制数间的转换。ConvertBase("15",10,16)表示将十进制数15转换为16进制的数。
        /// </summary>
        /// <param name="value">要转换的值,即原值</param>
        /// <param name="from">原值的进制,只能是2,8,10,16四个值。</param>
        /// <param name="to">要转换到的目标进制，只能是2,8,10,16四个值。</param>
        public static string ConvertBase(string value, int from, int to)
        {
            try
            {
                int intValue = Convert.ToInt32(value, from);  //先转成10进制
                string result = Convert.ToString(intValue, to);  //再转成目标进制
                if (to == 2)
                {
                    int resultLength = result.Length;  //获取二进制的长度
                    switch (resultLength)
                    {
                        case 7:
                            result = "0" + result;
                            break;
                        case 6:
                            result = "00" + result;
                            break;
                        case 5:
                            result = "000" + result;
                            break;
                        case 4:
                            result = "0000" + result;
                            break;
                        case 3:
                            result = "00000" + result;
                            break;
                    }
                }
                return result;
            }
            catch
            {

                //LogHelper.WriteTraceLog(TraceLogLevel.Error, ex.Message);
                return "0";
            }
        }
        #endregion

        #region 使用指定字符集将string转换成byte[]
        /// <summary>
        /// 使用指定字符集将string转换成byte[]
        /// </summary>
        /// <param name="text">要转换的字符串</param>
        /// <param name="encoding">字符编码</param>
        public static byte[] StringToBytes(string text, Encoding encoding)
        {
            return encoding.GetBytes(text);
        }
        #endregion

        #region 使用指定字符集将byte[]转换成string
        /// <summary>
        /// 使用指定字符集将byte[]转换成string
        /// </summary>
        /// <param name="bytes">要转换的字节数组</param>
        /// <param name="encoding">字符编码</param>
        public static string BytesToString(byte[] bytes, Encoding encoding)
        {
            return encoding.GetString(bytes);
        }
        #endregion

        #region 将byte[]转换成int
        /// <summary>
        /// 将byte[]转换成int
        /// </summary>
        /// <param name="data">需要转换成整数的byte数组</param>
        public static int BytesToInt32(byte[] data)
        {
            //如果传入的字节数组长度小于4,则返回0
            if (data.Length < 4)
            {
                return 0;
            }

            //定义要返回的整数
            int num = 0;

            //如果传入的字节数组长度大于4,需要进行处理
            if (data.Length >= 4)
            {
                //创建一个临时缓冲区
                byte[] tempBuffer = new byte[4];

                //将传入的字节数组的前4个字节复制到临时缓冲区
                Buffer.BlockCopy(data, 0, tempBuffer, 0, 4);

                //将临时缓冲区的值转换成整数，并赋给num
                num = BitConverter.ToInt32(tempBuffer, 0);
            }

            //返回整数
            return num;
        }
        #endregion

        #region 复制实体类（开辟新空间）设置实体对象的修改属性
        /// <summary>
        /// 复制实体类（开辟新空间）设置实体对象的修改属性
        /// </summary>
        /// <param name="srcObj">源实体 </param>
        /// <param name="desObj">目标实体</param>
        public static void CloneEntityObject(object srcObj, object desObj)
        {
            if (srcObj.Equals(desObj))
            {
                return;
            }
            //if (srcObj.GetType() != desObj.GetType())
            //{
            //    return;
            //}
            System.Reflection.PropertyInfo[] info = srcObj.GetType().GetProperties();
            foreach (System.Reflection.PropertyInfo property in info)
            {
                desObj.GetType().GetProperty(property.Name).SetValue(desObj,
                srcObj.GetType().GetProperty(property.Name).GetValue(srcObj, null), null);
            }
        }
        #endregion

        #region 转换成DataTable表
        /// <summary>
        /// DataRow[] 数组转换成表
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        public static DataTable RowsToDataTable(DataRow[] rows)
        {
            if (rows == null || rows.Length == 0) return null;
            DataTable tmp = rows[0].Table.Clone(); // 复制DataRow的表结构
            foreach (DataRow row in rows)
            {
                tmp.ImportRow(row); // 将DataRow添加到DataTable中
            }
            return tmp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static DataTable EnumToDataTable<T>(List<T> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            var table = new DataTable();

            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);

            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties) row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static DataTable EnumToDataTableC<T>(ObservableCollection<T> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            var table = new DataTable();

            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);

            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties) row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static DataTable EnumToDataTable(IEnumerable list)
        {
            Type type = list.AsQueryable().ElementType;
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(type);
            var table = new DataTable();

            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);

            foreach (var item in list)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties) row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }
        /// <summary>
        ///将枚举装换为DataTable 
        /// </summary>
        /// <param name="enumType"></param>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static DataTable EnumToDataTable(Type enumType, string key, string val)
        {
            string[] Names = System.Enum.GetNames(enumType);
            Array Values = System.Enum.GetValues(enumType);
            DataTable table = new DataTable();
            table.Columns.Add(key, System.Type.GetType("System.String"));
            table.Columns.Add(val, System.Type.GetType("System.Int32"));
            table.Columns[key].Unique = true;
            for (int i = 0; i < Values.Length; i++)
            {
                DataRow DR = table.NewRow();
                DR[key] = Names[i].ToString();
                DR[val] = (int)Values.GetValue(i);
                table.Rows.Add(DR);
            }
            return table;
        }
        /// <summary>
        /// 将枚举装换为DataTable 
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static DataTable EnumToDataTable(Type enumType)
        {
            return EnumToDataTable(enumType, "DisplayMember", "ValueMember");
        }
        #endregion

        #region 将int转成bool 1转true 其它转false
        /// <summary>
        /// 将int转成bool 1转true 其它转false
        /// </summary>
        /// <param name="intValue"></param>
        /// <returns></returns>
        public static bool ParseBool(int intValue)
        {
            bool targetValue;
            if (intValue == 1)
            {
                targetValue = true;
            }
            else
            {
                targetValue = false;
            }
            return targetValue;
        }
        /// <summary>
        /// 将int转成bool 1转true 或者"True"转True 其它转false
        /// </summary>
        /// <param name="intValue"></param>
        /// <returns></returns>
        public static bool ParseBool(string intValue)
        {
            bool targetValue;
            if (intValue == null)
            {
                targetValue = false;
            }
            else if (intValue == "1" || intValue.ToLower() == "true" || intValue.ToLower() == "on" || intValue.ToLower() == "yes")
            {
                targetValue = true;
            }
            else
            {
                targetValue = false;
            }
            return targetValue;
        }

        /// <summary>
        /// 将object转成bool 1转true 其它转false
        /// </summary>
        /// <param name="objValue"></param>
        /// <returns></returns>
        public static bool ParseBool(object objValue)
        {
            bool targetValue;
            if (objValue == null)
            {
                targetValue = false;
            }
            else if (objValue.ToString() == "1" || objValue.ToString().ToLower() == "true" || objValue.ToString().ToLower() == "on" || objValue.ToString().ToLower() == "yes")
            {
                targetValue = true;
            }
            else
            {
                targetValue = false;
            }
            return targetValue;
        }

        public static string BoolToIntString(bool targetValue)
        {
            return targetValue ? "1" : "0";
        }

        /// <summary>
        /// 将Bool转成int true转1 其它转0
        /// </summary>
        /// <param name="objValue"></param>
        /// <returns></returns>
        public static int ParseBoolToInt(object objValue)
        {
            return ParseBoolToInt(objValue.ToString());
        }

        /// <summary>
        /// 将Bool转成int true转1 其它转0
        /// </summary>
        /// <param name="objValue"></param>
        /// <returns></returns>
        public static int ParseBoolToInt(string objValue)
        {
            int targetValue;

            if (objValue == "True")
            {
                targetValue = 1;
            }
            else
            {
                targetValue = 0;
            }

            return targetValue;
        }
        #endregion

        #region 数据类型转换
        /// <summary>
        /// 将object类型值转换成int值，失败返回预设的缺省值
        /// </summary>
        /// <param name="originalValue"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int ParseInt(object originalValue, int defaultValue)
        {
            int targetValue = 0;

            try
            {
                if (originalValue == null)
                {
                    targetValue = defaultValue;
                }
                else
                    targetValue = int.Parse(originalValue.ToString().Trim());
            }
            catch
            {
                targetValue = defaultValue;
            }

            return targetValue;
        }
        /// <summary>
        /// 将object类型值转换成float值，失败返回预设的缺省值
        /// </summary>
        /// <param name="originalValue"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static float ParseFloat(object originalValue, float defaultValue)
        {
            float targetValue;
            try
            {
                if (originalValue == null)
                {
                    targetValue = defaultValue;
                }
                else
                    targetValue = float.Parse(originalValue.ToString().Trim());
            }
            catch
            {
                targetValue = defaultValue;
            }
            return targetValue;
        }
        /// <summary>
        /// 将object类型值转换成long值，失败返回预设的缺省值
        /// </summary>
        /// <param name="originalValue"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static long ParseLong(object originalValue, long defaultValue)
        {
            long targetValue;
            try
            {
                if (originalValue == null)
                {
                    targetValue = defaultValue;
                }
                else
                    targetValue = long.Parse(originalValue.ToString().Trim());
            }
            catch
            {
                targetValue = defaultValue;
            }
            return targetValue;
        }
        /// <summary>
        /// 将object类型值转换成double值，失败返回预设的缺省值
        /// </summary>
        /// <param name="originalValue"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static double ParseDouble(object originalValue, double defaultValue)
        {
            double targetValue;
            try
            {
                if (originalValue == null)
                {
                    targetValue = defaultValue;
                }
                else
                    targetValue = double.Parse(originalValue.ToString().Trim());
            }
            catch
            {
                targetValue = defaultValue;
            }
            return targetValue;
        }
        /// <summary>
        /// 将object类型值转换成decimal值，失败返回预设的缺省值
        /// </summary>
        /// <param name="originalValue"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static decimal ParseDecimal(object originalValue, decimal defaultValue)
        {
            decimal targetValue;
            try
            {
                if (originalValue == null)
                {
                    targetValue = defaultValue;
                }
                else
                    targetValue = decimal.Parse(originalValue.ToString().Trim());
            }
            catch
            {
                targetValue = defaultValue;
            }
            return targetValue;
        }
        /// <summary>
        /// 将object类型值转换成string值，失败返回预设的缺省值
        /// </summary>
        /// <param name="objValue"></param>
        /// <returns></returns>
        public static string ParseString(object originalValue, string defaultValue)
        {
            string targetValue;
            if (originalValue == null)
            {
                targetValue = defaultValue;
            }
            else
            {
                targetValue = originalValue.ToString();
            }
            return targetValue;
        }
        /// <summary>
        /// 分割字符串
        /// </summary>
        public static string[] strSplit(string strContent, string strSplit)
        {
            if (!CommonHelper.IsEmpty(strContent))
            {
                if (strContent.IndexOf(strSplit) < 0)
                    return new string[] { strContent };

                return Regex.Split(strContent, Regex.Escape(strSplit), RegexOptions.IgnoreCase);
            }
            else
                return new string[0] { };
        }
        public static bool StrIsNullOrEmpty(object str)
        {
            try
            {
                if (str == null || str.ToString().Trim() == string.Empty)
                    return true;

                return false;
            }
            catch { return true; }
        }
        #endregion

        #region 类型转换

        /// <summary>
        /// 返回对象obj的String值,obj为null时返回空值。
        /// </summary>
        /// <param name="obj">对象。</param>
        /// <returns>字符串。</returns>
        public static string ToObjectString(object obj)
        {
            return null == obj ? string.Empty : obj.ToString();
        }

        /// <summary>
        /// 取得Int值,如果为Null 则返回０
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int GetInt(object obj)
        {
            if (obj != null)
            {
                int i;
                int.TryParse(obj.ToString(), out i);
                return i;
            }
            else
                return 0;
        }

        public static float GetFloat(object obj)
        {
            float i;
            float.TryParse(obj.ToString(), out i);
            return i;
        }

        /// <summary>
        /// 取得Int值,如果不成功则返回指定exceptionvalue值
        /// </summary>
        /// <param name="obj">要计算的值</param>
        /// <param name="exceptionvalue">异常时的返回值</param>
        /// <returns></returns>
        public static int GetInt(object obj, int exceptionvalue)
        {
            if (obj == null)
                return exceptionvalue;
            if (string.IsNullOrWhiteSpace(obj.ToString()))
                return exceptionvalue;
            int i = exceptionvalue;
            try { i = Convert.ToInt32(obj); }
            catch { i = exceptionvalue; }
            return i;
        }

        /// <summary>
        /// 取得byte值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte Getbyte(object obj)
        {
            if (obj != null && obj.ToString() != "")
                return byte.Parse(obj.ToString());
            else
                return 0;
        }

        /// <summary>
        /// 获得Long值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static long GetLong(object obj)
        {
            if (obj != null && obj.ToString() != "")
                return long.Parse(obj.ToString());
            else
                return 0;
        }

        /// <summary>
        /// 取得Long值,如果不成功则返回指定exceptionvalue值
        /// </summary>
        /// <param name="obj">要计算的值</param>
        /// <param name="exceptionvalue">异常时的返回值</param>
        /// <returns></returns>
        public static long GetLong(object obj, long exceptionvalue)
        {
            if (obj == null)
            {
                return exceptionvalue;
            }
            if (string.IsNullOrWhiteSpace(obj.ToString()))
            {
                return exceptionvalue;
            }
            long i = exceptionvalue;
            try
            {
                i = Convert.ToInt64(obj);
            }
            catch
            {
                i = exceptionvalue;
            }
            return i;
        }

        /// <summary>
        /// 取得Decimal值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static decimal GetDecimal(object obj)
        {
            if (obj != null && obj.ToString() != "")
                return decimal.Parse(obj.ToString());
            else
                return 0;
        }

        /// <summary>
        /// 取得DateTime值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static DateTime GetDateTime(object obj)
        {
            if (obj != null && obj.ToString() != "")
                return DateTime.Parse(obj.ToString());
            else
                return DateTime.Now;
            //return DateTime.MinValue;
        }

        /// <summary>
        /// 计算耗时 
        /// </summary>
        /// <param name="t">毫秒</param>
        /// <returns></returns>
        public static string CostTime(long t)
        {
            long hour = t / (1000 * 60 * 24);
            long min = (t - hour * (1000 * 60 * 24)) / (1000 * 60);
            long sec = (t - hour * (1000 * 60 * 24) - min * (1000 * 60)) / 1000;
            long msec = t - hour * (1000 * 60 * 24) - min * (1000 * 60) - sec * 1000;
            string timeString = hour.ToString() + ":" + min.ToString() + ":" + sec.ToString() + "." + msec.ToString();
            string Time = GetDateTime(timeString).ToString("HH:mm:ss");
            if (Time == "00:00:00")
            {
                Time = "00:00:01";
            }
            return Time;
        }

        /// <summary>
        /// 取得DateTime值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static DateTime? ToDateTime(object obj)
        {
            if (obj != null && obj.ToString() != "")
                return DateTime.Parse(obj.ToString());
            else
                return null;
        }

        /// <summary>
        /// 格式化日期 yyyy-MM-dd HH:mm
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetFormatDateTime(object obj, string Format)
        {
            if (obj != null && obj.ToString() != null && obj.ToString() != "")
                return DateTime.Parse(obj.ToString()).ToString(Format);
            else
                return "";
        }

        /// <summary>
        /// Json 的日期格式与.Net DateTime类型的转换
        /// </summary>
        /// <param name="jsonDate">Date(1242357713797+0800)</param>
        /// <returns></returns>
        public static DateTime JsonToDateTime(string jsonDate)
        {
            string value = jsonDate.Substring(5, jsonDate.Length - 6) + "+0800";
            DateTimeKind kind = DateTimeKind.Utc;
            int index = value.IndexOf('+', 1);
            if (index == -1)
                index = value.IndexOf('-', 1);
            if (index != -1)
            {
                kind = DateTimeKind.Local;
                value = value.Substring(0, index);
            }
            long javaScriptTicks = long.Parse(value, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
            long InitialJavaScriptDateTicks = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).Ticks;
            DateTime utcDateTime = new DateTime((javaScriptTicks * 10000) + InitialJavaScriptDateTicks, DateTimeKind.Utc);
            DateTime dateTime;
            switch (kind)
            {
                case DateTimeKind.Unspecified:
                    dateTime = DateTime.SpecifyKind(utcDateTime.ToLocalTime(), DateTimeKind.Unspecified);
                    break;
                case DateTimeKind.Local:
                    dateTime = utcDateTime.ToLocalTime();
                    break;
                default:
                    dateTime = utcDateTime;
                    break;
            }
            return dateTime;
        }

        /// <summary>
        /// 取得bool值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool GetBool(object obj)
        {
            if (obj != null)
            {
                bool flag;
                bool.TryParse(obj.ToString(), out flag);
                return flag;
            }
            else
                return false;
        }

        /// <summary>
        /// 取得byte[]
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Byte[] GetByte(object obj)
        {
            if (obj.ToString() != null && obj.ToString() != "")
            {
                return (Byte[])obj;
            }
            else
                return null;
        }

        /// <summary>
        /// 取得string值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetString(object obj)
        {
            if (obj != null && obj != DBNull.Value)
                return obj.ToString();
            else
                return "";
        }

        /// <summary>   
        /// 判断用户输入是否为日期   
        /// </summary>   
        /// <param ></param>   
        /// <returns></returns>   
        /// <remarks>   
        /// 可判断格式如下（其中-可替换为.，不影响验证)   
        /// YYYY | YYYY-MM |YYYY.MM| YYYY-MM-DD|YYYY.MM.DD | YYYY-MM-DD HH:MM:SS | YYYY.MM.DD HH:MM:SS | YYYY-MM-DD HH:MM:SS.FFF | YYYY.MM.DD HH:MM:SS:FF (年份验证从1000到2999年)
        /// </remarks>   
        public static bool IsDateTime(string strValue)
        {
            if (strValue == null || strValue == "")
            {
                return false;
            }
            string regexDate = @"[1-2]{1}[0-9]{3}((-|[.]){1}(([0]?[1-9]{1})|(1[0-2]{1}))((-|[.]){1}((([0]?[1-9]{1})|([1-2]{1}[0-9]{1})|(3[0-1]{1})))( (([0-1]{1}[0-9]{1})|2[0-3]{1}):([0-5]{1}[0-9]{1}):([0-5]{1}[0-9]{1})(\.[0-9]{3})?)?)?)?$";
            if (Regex.IsMatch(strValue, regexDate))
            {
                //以下各月份日期验证，保证验证的完整性   
                int _IndexY = -1;
                int _IndexM = -1;
                int _IndexD = -1;
                if (-1 != (_IndexY = strValue.IndexOf("-")))
                {
                    _IndexM = strValue.IndexOf("-", _IndexY + 1);
                    _IndexD = strValue.IndexOf(":");
                }
                else
                {
                    _IndexY = strValue.IndexOf(".");
                    _IndexM = strValue.IndexOf(".", _IndexY + 1);
                    _IndexD = strValue.IndexOf(":");
                }
                //不包含日期部分，直接返回true   
                if (-1 == _IndexM)
                {
                    return true;
                }
                if (-1 == _IndexD)
                {
                    _IndexD = strValue.Length + 3;
                }
                int iYear = Convert.ToInt32(strValue.Substring(0, _IndexY));
                int iMonth = Convert.ToInt32(strValue.Substring(_IndexY + 1, _IndexM - _IndexY - 1));
                int iDate = Convert.ToInt32(strValue.Substring(_IndexM + 1, _IndexD - _IndexM - 4));
                //判断月份日期   
                if ((iMonth < 8 && 1 == iMonth % 2) || (iMonth > 8 && 0 == iMonth % 2))
                {
                    if (iDate < 32)
                    { return true; }
                }
                else
                {
                    if (iMonth != 2)
                    {
                        if (iDate < 31)
                        { return true; }
                    }
                    else
                    {
                        //闰年   
                        if ((0 == iYear % 400) || (0 == iYear % 4 && 0 < iYear % 100))
                        {
                            if (iDate < 30)
                            { return true; }
                        }
                        else
                        {
                            if (iDate < 29)
                            { return true; }
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 转换值，例如：1,2,3转换成'1','2','3'
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public static string ConvertString(string ids)
        {
            StringBuilder str = new StringBuilder();
            foreach (string id in ids.Split(','))
            {
                str.Append("'" + id.Trim() + "',");
            }

            return str.ToString().Length > 0 ? str.ToString().TrimEnd(',') : str.ToString();
        }

        #endregion

        #region 路径转换(转换成绝对路径)

        /// <summary>
        /// 路径转换（转换成绝对路径）
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string WebPathTran(string path)
        {
            try
            {
                return HttpContext.Current.Server.MapPath(path);
            }
            catch
            {
                return path;
            }
        }
        #endregion

    }
}