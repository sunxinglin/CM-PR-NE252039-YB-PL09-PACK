using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace RogerTech.Common
{
    /// <summary>
    /// 公共帮助类
    /// 版本：2.0
    /// <author>kurt</author>
    ///	<date>2013.09.27</date>
    ///	<email>qqliukk@hotmail.com</email>
    /// </summary>
    public class CommonHelper
    {
        #region 数据判断

        /// <summary>
        /// 判断文本obj是否为空值。
        /// </summary>
        /// <param name="obj">对象。</param>
        /// <returns>Boolean值。</returns>
        public static bool IsEmpty(string obj)
        {
            return ConvertHelper.ToObjectString(obj).Trim() == String.Empty ? true : false;
        }

        /// <summary>
        /// 判断对象是否为正确的日期值。
        /// </summary>
        /// <param name="obj">对象。</param>
        /// <returns>Boolean。</returns>
        public static bool IsDateTime(object obj)
        {
            try
            {
                DateTime dt = DateTime.Parse(ConvertHelper.ToObjectString(obj));
                if (dt > DateTime.MinValue && DateTime.MaxValue > dt)
                    return true;
                return false;
            }
            catch
            { return false; }
        }

        /// <summary>
        /// 判断对象是否为正确的Int32值。
        /// </summary>
        /// <param name="obj">对象。</param>
        /// <returns>Int32值。</returns>
        public static bool IsInt(object obj)
        {
            try
            {
                int.Parse(ConvertHelper.ToObjectString(obj));
                return true;
            }
            catch
            { return false; }
        }

        /// <summary>
        /// 判断对象是否为正确的Long值。
        /// </summary>
        /// <param name="obj">对象。</param>
        /// <returns>Long值。</returns>
        public static bool IsLong(object obj)
        {
            try
            {
                long.Parse(ConvertHelper.ToObjectString(obj));
                return true;
            }
            catch
            { return false; }
        }

        /// <summary>
        /// 判断对象是否为正确的Float值。
        /// </summary>
        /// <param name="obj">对象。</param>
        /// <returns>Float值。</returns>
        public static bool IsFloat(object obj)
        {
            try
            {
                float.Parse(ConvertHelper.ToObjectString(obj));
                return true;
            }
            catch
            { return false; }
        }

        /// <summary>
        /// 判断对象是否为正确的Double值。
        /// </summary>
        /// <param name="obj">对象。</param>
        /// <returns>Double值。</returns>
        public static bool IsDouble(object obj)
        {
            try
            {
                double.Parse(ConvertHelper.ToObjectString(obj));
                return true;
            }
            catch
            { return false; }
        }

        /// <summary>
        /// 判断对象是否为正确的Decimal值。
        /// </summary>
        /// <param name="obj">对象。</param>
        /// <returns>Decimal值。</returns>
        public static bool IsDecimal(object obj)
        {
            try
            {
                decimal.Parse(ConvertHelper.ToObjectString(obj));
                return true;
            }
            catch
            { return false; }
        }
        /// <summary>
        /// 判定对象是否为数字
        /// </summary>
        /// <param name="psd"></param>
        public static bool IsPassword(object obj)
        {
            try
            {
                Regex reg = new Regex("^[0-9]*$");
                return reg.IsMatch(ConvertHelper.ToObjectString(obj));
            }
            catch
            { return false; }
        }

        /// 判断DataTale中判断某个字段中包含某个数据
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="columnName"></param>
        /// <param name="fieldData"></param>
        /// <returns></returns>
        public static Boolean IsColumnIncludeData(DataTable dt, string columnName, string fieldData)
        {
            if (dt == null)
            {
                return false;
            }
            else
            {
                DataRow[] dataRows = dt.Select(columnName + "='" + fieldData + "'");
                if (dataRows.Length.Equals(1))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 判断是否为英文字母
        /// </summary>
        /// <param name="str">判断字串</param>
        /// <returns></returns>
        public static bool IsDigit(string str)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(str, @"^[A-Za-z]+$")) return true;
            else return false;
        }
        /// <summary>
        /// 判断是否为数字
        /// </summary>
        /// <param name="str">判断字串</param>
        /// <returns></returns>
        public static bool IsNumber(string str)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(str, @"^[0-9]+$")) return true;
            else return false;
        }
        /// <summary>
        /// 检测字符串是否为正整数,true为正整数
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNumeric(string str)
        {
            Regex reg = new Regex(@"^[0-9]\d*$");
            return reg.IsMatch(str);
        }
        public static bool IsNumberRegex(String strNumber)
        {
            Regex objNotNumberPattern = new Regex("[^0-9.-]");
            Regex objTwoDotPattern = new Regex("[0-9]*[.][0-9]*[.][0-9]*");
            Regex objTwoMinusPattern = new Regex("[0-9]*[-][0-9]*[-][0-9]*");
            String strValidRealPattern = "^([-]|[.]|[-.]|[0-9])[0-9]*[.]*[0-9]+$";
            String strValidIntegerPattern = "^([-]|[0-9])[0-9]*$";
            Regex objNumberPattern = new Regex("(" + strValidRealPattern + ")|(" + strValidIntegerPattern + ")");

            return !objNotNumberPattern.IsMatch(strNumber) && !objTwoDotPattern.IsMatch(strNumber) && !objTwoMinusPattern.IsMatch(strNumber) && objNumberPattern.IsMatch(strNumber);
        }
        #endregion

        #region "全球唯一码GUID"

        /// <summary>
        /// 获取一个全球唯一码GUID字符串
        /// </summary>
        public static string GetGuid
        {
            get
            {
                return Guid.NewGuid().ToString().ToLower();
            }
        }
        #endregion

        #region 日期操作
        /// <summary>
        /// 返回标准日期格式string
        /// </summary>
        public static string GetDate()
        {
            return DateTime.Now.ToString("yyyy-MM-dd");
        }
        /// <summary>
        /// 昨天
        /// </summary>
        /// <returns></returns>
        public static string GetLDate()
        {
            return DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd");
        }
        /// <summary>
        /// 下月
        /// </summary>
        /// <returns></returns>
        public static string GetNDate()
        {
            return DateTime.Now.AddMonths(1).ToString("yyyy-MM-dd");
        }
        /// <summary>
        /// 上月最后1天
        /// </summary>
        /// <returns></returns>
        public static string GetLEndDate()
        {
            DateTime now_first = Convert.ToDateTime(GetDate().Substring(0, 8) + "01");
            return now_first.AddDays(-1).ToString("yyyy-MM-dd");
        }
        /// <summary>
        /// 上月最后1天
        /// </summary>
        /// <param name="s_d"></param>
        /// <returns></returns>
        public static string GetLEndDate(string s_d)
        {
            return ParseDateTime(s_d).AddMonths(1).AddDays(-1).ToString("yyyy/MM/dd");
        }
        public static string GetNextDateS(string ym)
        {
            DateTime next_first = Convert.ToDateTime(ym + "-01").AddMonths(1);
            return next_first.ToString("yyyy-MM-dd");
        }
        /// <summary>
        /// 下月最后1天
        /// </summary>
        /// <param name="ym"></param>
        /// <returns></returns>
        public static string GetNextDateE(string ym)
        {
            DateTime nn_first = Convert.ToDateTime(ym + "-01").AddMonths(2);
            return nn_first.AddDays(-1).ToString("yyyy-MM-dd");
        }
        /// <summary>
        /// 返回指定日期格式
        /// </summary>
        public static string GetDate(string datetimestr, string replacestr)
        {
            if (datetimestr == null)
                return replacestr;

            if (datetimestr.Equals(""))
                return replacestr;
            try
            {
                datetimestr = Convert.ToDateTime(datetimestr).ToString("yyyy-MM-dd").Replace("1900-01-01", replacestr);
            }
            catch
            {
                return replacestr;
            }
            return datetimestr;
        }
        /// <summary>
        /// 返回标准时间格式string
        /// </summary>
        public static string GetTime()
        {
            return DateTime.Now.ToString("HH:mm:ss");
        }

        /// <summary>
        /// 返回标准时间格式string
        /// </summary>
        public static string GetDateTime()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 返回相对于当前时间的相对天数
        /// </summary>
        public static string GetDateTime(int relativeday)
        {
            return DateTime.Now.AddDays(relativeday).ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 返回标准时间格式string
        /// </summary>
        public static string GetDateTimeF()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fffffff");
        }

        /// <summary>
        /// 返回标准时间格式,精确到毫秒,17位
        /// </summary>
        public static string GetDateTimeInt()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmssfff");
        }

        /// <summary>
        /// 返回标准时间 
        /// </sumary>
        public static string GetStandardDateTime(string fDateTime, string formatStr)
        {
            if (fDateTime == "0000-0-0 0:00:00")
                return fDateTime;
            DateTime time = new DateTime(1900, 1, 1, 0, 0, 0, 0);
            if (DateTime.TryParse(fDateTime, out time))
                return time.ToString(formatStr);
            else
                return "";
        }

        /// <summary>
        /// 返回标准时间 yyyy-MM-dd HH:mm:ss
        /// </sumary>
        public static string GetStandardDateTime(string fDateTime)
        {
            return GetStandardDateTime(fDateTime, "yyyy-MM-dd HH:mm:ss");
        }
        /// <summary>
        /// 根據日期函數獲取上周日期
        /// DayNum:1:周日 2:周六 3:周五 4:周四 5:周三 6:周二 7:周一
        /// </summary>
        public static DateTime GetLastSaturdayDate(DateTime someDate, int DayNum)
        {
            int i = someDate.DayOfWeek - DayOfWeek.Monday;
            if (i == -1)// i值 > = 0 ，因為枚舉原因，Sunday排在最前，此能Sunday-Monday=-1，必須+7=6。 
                i = 6;
            i += DayNum;
            TimeSpan ts = new TimeSpan(i, 0, 0, 0);
            return someDate.Subtract(ts);
        }
        /// <summary>
        /// 返回差值天數前日期
        /// </summary>
        /// <param name="someDate"></param>
        /// <param name="diffSum"></param>
        /// <returns></returns>
        public static DateTime GetDiffDate(DateTime someDate, int diffSum)
        {
            DateTime dt = someDate.AddDays(diffSum);
            return dt;
        }
        /// <summary>
        /// 字符串转时间
        /// </summary>
        /// <param name="strDateTime"></param>
        /// <returns></returns>
        public static DateTime ParseDateTime(string strDateTime)
        {
            if (strDateTime.Trim() == "")
            {
                return DateTime.Now;
            }
            try
            {
                return Convert.ToDateTime(strDateTime);
            }
            catch
            {
                return DateTime.Now;
            }
        }
        #endregion

        #region 自动生成日期编号

        /// <summary>
        /// 自动生成编号  201008251145409865
        /// </summary>
        /// <returns></returns>
        public static string CreateNo()
        {
            Random random = new Random();
            string strRandom = random.Next(1000, 10000).ToString(); //生成编号 
            string code = DateTime.Now.ToString("yyyyMMddHHmmss") + strRandom;//形如
            return code;
        }

        #endregion

        #region 生成0-9随机数

        /// <summary>
        /// 生成0-9随机数
        /// </summary>
        /// <param name="codeNum">生成长度</param>
        /// <returns></returns>
        public static string RndNum(int codeNum)
        {
            StringBuilder sb = new StringBuilder(codeNum);
            Random rand = new Random(Guid.NewGuid().GetHashCode());
            for (int i = 1; i < codeNum + 1; i++)
            {
                int t = rand.Next(9);
                sb.AppendFormat("{0}", t);
            }
            return sb.ToString();

        }

        #endregion

        #region 计时器

        /// <summary>
        /// 计时器开始
        /// </summary>
        /// <returns></returns>
        public static Stopwatch TimerStart()
        {
            Stopwatch watch = new Stopwatch();
            watch.Reset();
            watch.Start();
            return watch;
        }

        /// <summary>
        /// 计时器结束
        /// </summary>
        /// <param name="watch"></param>
        /// <returns></returns>
        public static string TimerEnd(Stopwatch watch)
        {
            watch.Stop();
            double costtime = watch.ElapsedMilliseconds;
            return costtime.ToString();
        }

        #endregion

        #region INI配置档操作
        [DllImport("kernel32")] //引入“shell32.dll”API文件
        public static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal,
                                                         int size, string filePath);
        [DllImport("kernel32")]//返回0表示失败，非0为成功
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        /// <summary>
        ///   从INI文件中读取指定节点的内容
        /// </summary>
        /// <param name="section"> INI节点 </param>
        /// <param name="key"> 节点下的项 </param>
        /// <param name="def"> 没有找到内容时返回的默认值 </param>
        /// <param name="filePath"> 要读取的INI文件 </param>
        /// <returns> 读取的节点内容 </returns>
        public static string GetIniFileString(string section, string key, string def, string filePath)
        {
            StringBuilder temp = new StringBuilder(1024);
            GetPrivateProfileString(section, key, def, temp, 1024, filePath);
            return temp.ToString();
        }
        /// <summary>
        ///写入配置文件节点值
        /// </summary>
        /// <param name="section">欲在其中写入的节点名称</param>
        /// <param name="key">欲设置的项名</param>
        /// <param name="def">要写入的新字符串</param>
        /// <param name="filePath">要读取的INI文件 </param>
        public static void WirteIniFileStringstring(string section, string key, string value, string filePath)
        {
            WritePrivateProfileString(section, key, value, filePath);
        }
        public static bool WriteIniFileString(string Section, string Key, string Value, string iniFilePath)
        {
            if (File.Exists(iniFilePath))
            {
                long OpStation = WritePrivateProfileString(Section, Key, Value, iniFilePath);
                if (OpStation == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region 字符串操作
        /// <summary>
        /// 说明：获取字符串长度       
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int GetStringLength(string str)
        {
            if (!string.IsNullOrWhiteSpace(str))
            {
                return Regex.Replace(str, "[\u4e00-\u9fa5]", "zz", RegexOptions.IgnoreCase).Length;
            }
            else
            {
                return 0;
            }
        }
        public static string[] CutStringToParam(string _str, int _long)
        {
            int length = GetStringLength(_str);
            int count = (length % _long) > 0 ? (length / _long) + 1 : (length / _long);
            string[] sp = new string[count];
            int i = 0;
            while (_str.Length > 0)
            {
                if (i >= count) break;
                string cutStr = NewSubString(_str, _long);
                sp[i] = cutStr;
                _str = _str.Replace(cutStr, "");
                i++;
            }
            return sp;
        }

        /// <summary>截取指定字节长度的字符串(中文按2字节计算)</summary> 
        /// <param name="str">原字符串</param>
        ///<param name="len">截取字节长度</param> 
        /// <returns>string</returns>
        public static string NewSubString(string str, int len)
        {
            string result = string.Empty;// 最终返回的结果
            if (string.IsNullOrWhiteSpace(str))
            {
                return result;
            }
            int byteLen = System.Text.Encoding.Default.GetByteCount(str);
            // 单字节字符长度
            int charLen = str.Length;
            // 把字符平等对待时的字符串长度
            int byteCount = 0;
            // 记录读取进度 
            int pos = 0;
            // 记录截取位置 
            if (byteLen > len)
            {
                for (int i = 0; i < charLen; i++)
                {
                    if (Convert.ToInt32(str.ToCharArray()[i]) > 255)
                    // 按中文字符计算加 2 
                    {
                        byteCount += 2;
                    }
                    else
                    // 按英文字符计算加 1 
                    {
                        byteCount += 1;
                    }
                    if (byteCount > len)
                    // 超出时只记下上一个有效位置
                    {
                        pos = i;
                        break;
                    }
                    else if (byteCount == len)// 记下当前位置
                    {
                        pos = i + 1; break;
                    }
                }
                if (pos >= 0)
                {
                    result = str.Substring(0, pos);
                }
            }
            else { result = str; }
            return result;
        }
        #endregion
    }
}