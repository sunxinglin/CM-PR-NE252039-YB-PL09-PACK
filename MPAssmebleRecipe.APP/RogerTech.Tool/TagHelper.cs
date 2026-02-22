using System;
using System.Collections;

namespace RogerTech.Tool
{
    public enum DataType
    {
        NONE,
        BIT,
        USHORT,
        SHORT,
        INT,
        UINT,
        FLOAT,
        LFLOAT,
        STRING,
        DTL,
        BYTE,
        CHARARRY
    }
    public static class DataConvert
    {
        public static object GetBoolValue(Tag tag, byte[] buffer)
        {
            object result = new object();
            if (buffer == null)
            {
                throw new ArgumentNullException();
            }
            /*if (buffer.Length != 1)
            {
                throw new ArgumentOutOfRangeException();
            }*/
            BitArray bitArray = new BitArray(buffer);
            result = bitArray.Get(tag.DataBit);
            return result;

        }
        public static object GetByteVlaue(Tag tag, byte[] buffer)
        {
            object result = new object();
            if (buffer == null)
            {
                throw new ArgumentNullException();
            }
            if (buffer.Length != 1)
            {
                throw new ArgumentOutOfRangeException();
            }
            result = buffer[0];
            return result;
        }
        public static object GetShortVlaue(Tag tag, byte[] buffer)
        {
            object result = new object();
            if (buffer == null)
            {
                throw new ArgumentNullException();
            }
            if (buffer.Length != 2)
            {
                throw new ArgumentOutOfRangeException();
            }
            Array.Reverse(buffer);
            try
            {
                result = BitConverter.ToInt16(buffer, 0);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {

            }
            return result;
        }
        public static object GetUShortValue(Tag tag, byte[] buffer)
        {
            object result = new object();
            if (buffer == null)
            {
                throw new ArgumentNullException();
            }
            if (buffer.Length != 2)
            {
                throw new ArgumentOutOfRangeException();
            }
            Array.Reverse(buffer);
            try
            {
                result = BitConverter.ToUInt16(buffer, 0);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {

            }
            return result;
        }
        public static object GetIntValue(Tag tag, byte[] buffer)
        {
            object result = new object();
            if (buffer == null)
            {
                throw new ArgumentNullException();
            }
            /*if (buffer.Length != 2)
            {
                throw new ArgumentOutOfRangeException();
            }*/
            Array.Reverse(buffer);
            try
            {
                result = BitConverter.ToInt32(buffer, 0);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {

            }
            return result;
        }
        public static object GetUIntValue(Tag tag, byte[] buffer)
        {
            object result = new object();
            if (buffer == null)
            {
                throw new ArgumentNullException();
            }
            if (buffer.Length != 2)
            {
                throw new ArgumentOutOfRangeException();
            }
            Array.Reverse(buffer);
            try
            {
                result = BitConverter.ToUInt32(buffer, 0);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {

            }
            return result;
        }
        public static object GetFloatValue(Tag tag, byte[] buffer)
        {
            object result = new object();
            if (buffer == null)
            {
                throw new ArgumentNullException();
            }
            if (buffer.Length != 4)
            {
                throw new ArgumentOutOfRangeException();
            }
            Array.Reverse(buffer);
            try
            {
                result = BitConverter.ToSingle(buffer, 0);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {

            }
            return result;
        }
        public static object GetStringValue(Tag tag, byte[] buffer)
        {
            object result = new object();
            if (buffer == null)
            {
                throw new ArgumentNullException();
            }
            if (buffer.Length < 2)
            {
                //throw new ArgumentOutOfRangeException();
                return result;
            }
            ////判断字符串长队是否正确
            //if (buffer.Length != buffer[0]+2)
            //{
            //    //throw new ArgumentOutOfRangeException();
            //    return result;
            //}
            byte[] temp = new byte[buffer[1]];
            Array.Copy(buffer, 2, temp, 0, buffer[1]);
            try
            {
                result = System.Text.Encoding.Default.GetString(temp);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {

            }
            return result;
        }
        public static object GetCharArrayValue(Tag tag, byte[] buffer)
        {
            object result = new object();
            if (buffer == null)
            {
                throw new ArgumentNullException();
            }
            
            byte[] temp = new byte[tag.DataLength];
            Array.Copy(buffer, 0, temp, 0, tag.DataLength);
            try
            {
                result = System.Text.Encoding.Default.GetString(temp);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {

            }
            return result;
        }
        public  static object GetDtlValue(Tag tag, byte[] buffer)
        {
            object result = new object();
            if (buffer == null)
            {
                throw new ArgumentNullException();
            }
            if (buffer.Length != 12)
            {
                //throw new ArgumentOutOfRangeException();
                return result;
            }
            try
            {
                byte[] temp = new byte[2];
                Array.Copy(buffer, 0, temp, 0, 2);
                Array.Reverse(temp);
                UInt16 year = BitConverter.ToUInt16(temp, 0);
                if (year == 0)
                    year = 1970;
                int month = buffer[2];
                if (month == 0)
                    month = 1;
                
                int day = buffer[3];
                if (day == 0)
                    day = 1;
                int hour = buffer[5];
                int minute = buffer[6];
                int second = buffer[7];
                temp = new byte[4];
                Array.Copy(buffer, 8, temp, 0, 4);
                Array.Reverse(temp);
                int msecond = (int)(BitConverter.ToUInt32(temp, 0) / 1000);
                msecond = 0;
                result = new DateTime(year, month, day, hour, minute, second, msecond);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {

            }
            return result;
        }
        public static object GetTagValue(Tag tag, byte[] buffer)
        {
            object result = new object();
            switch (tag.DataType)
            {
                case DataType.NONE:
                    break;
                case DataType.BIT:
                    result = GetBoolValue(tag, buffer);
                    break;
                case DataType.BYTE:
                    result = GetByteVlaue(tag, buffer);
                    break;
                case DataType.USHORT:
                    result = GetUShortValue(tag, buffer);
                    break;
                case DataType.SHORT:
                    result = GetShortVlaue(tag, buffer);
                    break;
                case DataType.INT:
                    result = GetIntValue(tag, buffer);
                    break;
                case DataType.UINT:
                    result = GetUIntValue(tag, buffer);
                    break;
                case DataType.FLOAT:
                    result = GetFloatValue(tag, buffer);
                    break;
                case DataType.LFLOAT:
                    result = GetFloatValue(tag, buffer);
                    break;
                case DataType.STRING:
                    result = GetStringValue(tag, buffer);
                    break;
                case DataType.CHARARRY:
                    result = GetCharArrayValue(tag, buffer);
                    break;
                case DataType.DTL:
                    result = GetDtlValue(tag, buffer);
                    break;
                default:
                    break;
            }
            return result;
        }

        public static byte[] GetTagBytes(Tag tag, object value)
        {
            byte[] resultBuffer = new byte[tag.DataLength];
            switch (tag.DataType)
            {
                case DataType.NONE:
                    break;
                case DataType.BIT:
                    resultBuffer = GetBitBuffer(tag, value);
                    break;
                case DataType.BYTE:
                    resultBuffer = GetByteBuffer(tag, value);
                    break;
                case DataType.USHORT:
                    resultBuffer = GetUshortBuffer(tag, value);
                    break;
                case DataType.SHORT:
                    resultBuffer = GetShortBuffer(tag, value);
                    break;
                case DataType.INT:
                    resultBuffer = GetIntBuffer(tag, value);
                    break;
                case DataType.UINT:
                    resultBuffer = GetUintBuffer(tag, value);
                    break;
                case DataType.FLOAT:
                    resultBuffer = GetFloatBuffer(tag, value);
                    break;
                case DataType.LFLOAT:
                    resultBuffer = GetFloatBuffer(tag, value);
                    break;
                case DataType.STRING:
                    resultBuffer = GetStringBuffer(tag, value);
                    break;
                case DataType.CHARARRY:
                    resultBuffer = GetCharArrayBuffer(tag, value);
                    break;
                case DataType.DTL:
                    resultBuffer = GetDtlBuffer(tag, value);
                    break;
                default:
                    break;
            }
            return resultBuffer;
        }
        private static byte[] GetBitBuffer(Tag tag, object value)
        {


            bool flag;
            if(bool.TryParse(value.ToString(), out flag))
            {
                byte[] values = new byte[] { 0 };
                BitArray ba = new BitArray(values);
                ba.Set(tag.DataBit, flag);
                ba.CopyTo(values, 0);
                return values;
            }
            else
            {
                throw new ArgumentOutOfRangeException("输入变量类型错误");
            }
        }

        private static byte[] GetByteBuffer(Tag tag, object value)
        {
            byte flag;
            if (byte.TryParse(value.ToString(), out flag))
            {
                //byte[] buffer = BitConverter.GetBytes(flag);
                //Array.Reverse(buffer);
                byte[] buffer = new byte[] { flag };
            
                return buffer;
            }
            else
            {
                throw new ArgumentOutOfRangeException("输入变量类型错误");
            }
        }

        private static byte[] GetUshortBuffer(Tag tag, object value)
        {
            UInt16 flag;
            if (UInt16.TryParse(value.ToString(), out flag))
            {
                byte[] buffer = BitConverter.GetBytes(flag);
                Array.Reverse(buffer);
                return buffer;
            }
            else
            {
                throw new ArgumentOutOfRangeException("输入变量类型错误");
            }
        }

        private static byte[] GetShortBuffer(Tag tag, object value)
        {
            Int16 flag;
            if (Int16.TryParse(value.ToString(), out flag))
            {
                byte[] buffer = BitConverter.GetBytes(flag);
                Array.Reverse(buffer);
                return buffer;
            }
            else
            {
                throw new ArgumentOutOfRangeException("输入变量类型错误");
            }
        }

        private static byte[] GetUintBuffer(Tag tag, object value)
        {
            UInt32 flag;
            if (UInt32.TryParse(value.ToString(), out flag))
            {
                byte[] buffer = BitConverter.GetBytes(flag);
                Array.Reverse(buffer);
                return buffer;
            }
            else
            {
                throw new ArgumentOutOfRangeException("输入变量类型错误");
            }
        }

        private static byte[] GetIntBuffer(Tag tag, object value)
        {
            UInt32 flag;
            if (UInt32.TryParse(value.ToString(), out flag))
            {
                byte[] buffer = BitConverter.GetBytes(flag);
                Array.Reverse(buffer);
                return buffer;
            }
            else
            {
                throw new ArgumentOutOfRangeException("输入变量类型错误");
            }
        }

        private static byte[] GetFloatBuffer(Tag tag, object value)
        {
            Single flag;
            if (Single.TryParse(value.ToString(), out flag))
            {
                byte[] buffer = BitConverter.GetBytes(flag);
                Array.Reverse(buffer);
                return buffer;
            }
            else
            {
                throw new ArgumentOutOfRangeException("输入变量类型错误");
            }
        }
        private static byte[] GetStringBuffer(Tag tag, object value)
        {
            if(value is string)
            {
                byte[] buffer = new byte[tag.DataLength];
                buffer[0] = (byte)(tag.DataLength-2);
                buffer[1] = (byte)value.ToString().Length;
                byte[] temp = System.Text.Encoding.Default.GetBytes(value.ToString());
                Array.Copy(temp, 0, buffer, 2, buffer[1]);
                return buffer;
            }
            else
            {
                throw new ArgumentOutOfRangeException("输入变量类型错误");
            }
        }

        private static byte[] GetCharArrayBuffer(Tag tag, object value)
        {
            if (value is string)
            {
                byte[] buffer = new byte[tag.DataLength];
                byte[] temp = System.Text.Encoding.Default.GetBytes(value.ToString());
                Array.Copy(temp, 0, buffer, 0, temp.Length);
                return buffer;
            }
            else
            {
                throw new ArgumentOutOfRangeException("输入变量类型错误");
            }
        }
        private static byte[] GetDtlBuffer(Tag tag, object value)
        {
            value = DateTime.Now;
            if(value is DateTime)
            {
                byte[] buffer = new byte[tag.DataLength];

                DateTime dt = (DateTime)value;
                UInt16 year = (UInt16)dt.Year;
                UInt16 month = (UInt16)dt.Month;
                UInt16 day = (UInt16)dt.Day;
                UInt16 dayofWeek = (UInt16)dt.DayOfWeek;
                UInt16 hour = (UInt16)dt.Hour;
                UInt16 minute = (UInt16)dt.Minute;
                UInt16 second = (UInt16)dt.Second;
                UInt32 nsecond = (UInt32)dt.Millisecond * 1000;

                byte[] temp = BitConverter.GetBytes(year);
                Array.Reverse(temp);
                Array.Copy(temp, 0, buffer, 0, 2);
                temp = BitConverter.GetBytes(month);
                buffer[2] = temp[0];
                temp = BitConverter.GetBytes(day);
                buffer[3] = temp[0];
                temp = BitConverter.GetBytes(dayofWeek);
                buffer[4] = temp[0];

                temp = BitConverter.GetBytes(hour);
                buffer[5] = temp[0];
                temp = BitConverter.GetBytes(minute);
                buffer[6] = temp[0];
                temp = BitConverter.GetBytes(second);
                buffer[7] = temp[0];


                temp = BitConverter.GetBytes(nsecond);
                Array.Reverse(temp);
                Array.Copy(temp, 0, buffer, 8, 4);

                return buffer;
            }
            else
            {
                throw new ArgumentOutOfRangeException("输入变量类型错误");
            }
        }
    }    
}