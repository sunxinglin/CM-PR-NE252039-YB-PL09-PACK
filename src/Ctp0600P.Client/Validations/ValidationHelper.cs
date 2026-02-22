using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ctp0600P.Client.Validations
{
    public class ValidationHelper
    {
        public static bool IsIpAddress(string iPAddress)
        {
            if (string.IsNullOrWhiteSpace(iPAddress)) return false;
            string IPAddressFormartRegex = @"^(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])$";

            // 检查输入的字符串是否符合IP地址格式
            if (!Regex.IsMatch(iPAddress, IPAddressFormartRegex))
            {
                return false;
            }
            return true;
        }

        public static bool NotNullValidation(string str)
        {
            if (string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str))
            {
                return false;
            }
            return true;
        }

        public static bool PositiveInteger(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return false;
            string IPAddressFormartRegex = @"^[0-9]*[1-9][0-9]*$";

            // 检查输入的字符串是否符合IP地址格式
            if (!Regex.IsMatch(str, IPAddressFormartRegex))
            {
                return false;
            }
            return true;
        }
    }
}
