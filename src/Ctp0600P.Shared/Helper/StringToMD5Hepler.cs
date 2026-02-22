using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Ctp0600P.Shared.Helper
{
    public class StringToMD5Hepler
    {
        public static string StrConversionMD5(string convertStr)
        {
            MD5 md5 = MD5.Create();
            byte[] strByte = System.Text.Encoding.Default.GetBytes(convertStr);
            byte[] hashByte = md5.ComputeHash(strByte);//用来计算指定数组的hash值

            //将每一个字节数组中的元素都tostring，在转成16进制
            string newStr = null;
            for (int i = 0; i < hashByte.Length; i++)
            {
                newStr += hashByte[i].ToString("x2");  //ToString(param);//传入不同的param可以转换成不同的效果
            }
            return newStr;
        }
    }
}
