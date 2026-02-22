using System;
using System.Collections.Generic;

namespace RogerTech.Tool
{
    public enum ParameterDataType
    {

        /// <remarks/>
        NUMBER,

        /// <remarks/>
        TEXT,

        /// <remarks/>
        FORMULA,

        /// <remarks/>
        BOOLEAN,
    }
    public class Tag : IComparable
    {

        public Connection Connection { get; private set; }
        public int Dbnr { get; private set; }
        public int StartAddress { get; private set; }
        public int DataLength { get; private set; }
        public DataType DataType { get; private set; }
        public byte DataBit { get; private set; }
        public string TagName { get; private set; }


        public string MesName { get; set; }
        public ParameterDataType MesDataType { get; set; }
        public double LowerLimit { get; set; }
        public double UpperLimit { get; set; }

        public bool IsChecked { get; set; }
        public bool IsUpload { get; set; }
        public TagResult Result { get; private set; }

        public Tag(Connection connection, string tagName, int dbNr, int startAddress, int dateLength, DataType datatype, byte dateBit, string mesName,ParameterDataType mesDataType, double lowerLimit, double upperLimit, bool isChecked = false, bool isUpload = false)
        {
            this.Connection = connection;
            this.Dbnr = dbNr;
            this.DataLength = dateLength;
            this.DataType = datatype;
            this.DataBit = dateBit;
            this.StartAddress = startAddress;
            this.TagName = tagName;
            this.MesName = mesName;
            this.MesDataType = mesDataType;
            this.LowerLimit = lowerLimit;
            this.UpperLimit = upperLimit;
            this.IsChecked = isChecked;
            this.IsUpload = isUpload;
            //Todo:根据协议和数据类型，对长度进行优化和修正
            switch (datatype)
            {
                case DataType.NONE:
                    break;
                case DataType.BIT:
                    DataLength = 1;
                    break;
                case DataType.BYTE:
                    DataLength = 1;
                    break;
                case DataType.USHORT:
                    DataLength = 2;
                    break;
                case DataType.SHORT:
                    DataLength = 2;
                    break;
                case DataType.INT:
                    DataLength = 4;
                    break;
                case DataType.UINT:
                    DataLength = 4;
                    break;
                case DataType.FLOAT:
                    DataLength = 4;
                    break;
                case DataType.LFLOAT:
                    DataLength = 8;
                    break;
                case DataType.STRING:
                    DataLength = dateLength;
                    break;
                default:
                    break;
            }
            //init
            Result = new TagResult();
            Result.SetAviliable(false);
            Result.SetValue(new object());
        }
        public void WriteValue(object obj)
        {
            Connection.WriteTagValue(this, obj);

        }


        public TagResult GetValue()
        {
            return Connection.ReadTag(this);
        }
        public override bool Equals(object obj)
        {
            if (obj is Tag)
            {
                Tag temp = (Tag)obj;
                return temp.TagName == TagName && temp.Dbnr == Dbnr && temp.StartAddress == StartAddress && temp.DataType == DataType && temp.Connection.Equals(Connection) && temp.DataBit == DataBit;
            }
            else
            {
                //throw new ArgumentOutOfRangeException("In class Tag, func equal");
                return false;
            }
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
                return 1;
            else
            {
                Tag temp = obj as Tag;
                if (temp.Dbnr > Dbnr)
                {
                    return 1;
                }
                else if (temp.Dbnr == Dbnr)
                {
                    if (temp.StartAddress == StartAddress)
                    {
                        return 0;
                    }
                    else
                    {
                        return StartAddress.CompareTo(temp.StartAddress);
                    }
                }
                else
                {
                    return -1;
                }
            }
        }
    }
}