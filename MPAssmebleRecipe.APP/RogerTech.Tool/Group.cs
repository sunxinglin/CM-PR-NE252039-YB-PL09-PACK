using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace RogerTech.Tool
{
    public class Group
    {
        public List<Tag> Tags { get; private set; } 
        public string GroupName { get; private set; }
        public List<Connection> Connections { get; private set; }
        //read taglist from file, 每个group中的变量不能同名;
        private readonly string filePath = Directory.GetCurrentDirectory();
        private Server server;
        public Group(string fileName, Server server)
        {
            this.server = server;
            Tags = new List<Tag>();
            Connections = new List<Connection>();
            string configPath = Path.Combine(filePath,"Config");
            if(!Directory.Exists(configPath))
            {
                Directory.CreateDirectory(configPath);
            }
            if(string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException();
            }
            string fullName = Path.Combine(configPath, fileName);
            if(File.Exists(fullName))
            {
                int index = fileName.IndexOf('.');
                GroupName = fileName.Substring(0,index);
                FileStream fs = null;
                StreamReader sr = null;
                try
                {
                    fs = new FileStream(fullName, FileMode.Open);
                    sr = new StreamReader(fs);
                    string line = sr.ReadLine();
                    while((line = sr.ReadLine())!= null)
                    {
                        Tag tag = null;
                        ParseTagInfo(line,ref tag);
                        if(tag == null)
                        {
                            throw new ArgumentOutOfRangeException($"In group,{line} can't parse to tag");
                        }
                        foreach (var item in Tags)
                        {
                            if (item.Equals(tag))
                                throw new ArgumentOutOfRangeException($"Tag {tag.TagName} exist");
                        }
                        //
                        if(!Connections.Contains(tag.Connection))
                        {
                            System.Threading.Tasks.Task.Run(()=>tag.Connection.Run());
                            Connections.Add(tag.Connection);
                        }
                        //从connection中查询tag是否存在,如果存在,使用connection中的tag
                        foreach (var item in tag.Connection.Tags)
                        {
                            if(item.Connection.IP == tag.Connection.IP && item.Dbnr == tag.Dbnr && tag.TagName == item.TagName && tag.StartAddress == item.StartAddress)
                            {
                                tag = item;
                            }
                        }
                        Tags.Add(tag);
                    }
                    //添加tag完成后，完成connections列表
                    server.AddGroup(this);
                }
                catch(Exception ex)
                {
                    throw new Exception($"In Class Group Ctor: {ex.Message}");
                }
                finally
                {
                    if(sr != null)
                    {
                        sr.Close();
                    }
                    if(fs != null)
                    {
                        fs.Close();
                    }
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException("fileName not exits");
            }
        }
        /// <summary>
        /// 将字符串解析成plcTag
        /// </summary>
        /// <param name="content"></param>
        /// <param name="tag"></param>
        private void ParseTagInfo(string content, ref Tag tag)       
        {
            if(string.IsNullOrEmpty(content))
            {
                throw new ArgumentNullException("In func ParseTagInfo, content string is null");
            }
            string[] boys = content.Split(',');
            if(boys != null && boys.Length == 14)
            {
                string tagName   = boys[0];
                string protocol  = boys[1].ToUpper();
                string ip          = boys[2];
                string datatype     = boys[3];
                string dbNr = boys[4];
                string startAddress = boys[5];
                string dataLength = boys[6];
                string dataBit = boys[7];             
                string MesName = boys[8];   
                if (Enum.TryParse<ParameterDataType>(boys[9], out ParameterDataType mesDataType))
                {

                }
                else
                {
                    mesDataType = ParameterDataType.TEXT;
                }
                string LowerLimit = boys[10];
                string UpperLimit = boys[11];
                string IsChecked = boys[12];
                string IsUpload = boys[13];
                IComProtocol comProtocol = null;
                IPAddress iPAddress = null;
                int port = 0;
                if(IPAddress.TryParse(ip, out iPAddress))
                {

                }
                else
                {
                    throw new ArgumentOutOfRangeException($"In func ParseTagInfo, {ip} is invalid format");
                }
                switch (protocol)
                {
                    case "SIEMENSS7":
                        comProtocol = new SiemensS7(ip);
                        port = 102;
                        break;
                    case "SiemensS7N":
                        comProtocol = new SiemensS7N(ip);
                        port = 102;
                        break;
                    default:
                        break;
                }
                if(comProtocol == null)
                {
                    throw new ArgumentOutOfRangeException($"In func ParseTagInfo: {protocol} not exist!");
                }
                Connection conn = server.GetConnection(comProtocol, ip, port);
                
                DataType dataType = DataType.NONE;
                if(Enum.TryParse(datatype, out dataType))
                {

                }
                else
                {
                    throw new ArgumentOutOfRangeException($"In func ParseTagInfo: {datatype} can't convert to enum");
                }
                int idbNr = 0;
                int istartAddr = 0;
                int idataLength = 0;
                byte idataBit = 0;
                bool isUpload = false;
                bool isChecked = false;            
                double lowerLimit = 0;
                double upperLimit = 0;
                if(bool.TryParse(IsUpload, out isUpload))
                {

                }
                else
                {
                    throw new ArgumentOutOfRangeException($"In func ParseTagInfo: {IsUpload} can't convert to bool");
                }
                if(bool.TryParse(IsChecked, out isChecked))
                {

                }
                else
                {
                    throw new ArgumentOutOfRangeException($"In func ParseTagInfo: {IsChecked} can't convert to bool");
                }
                if(double.TryParse(LowerLimit, out lowerLimit))
                {

                }
                else
                {
                    throw new ArgumentOutOfRangeException($"In func ParseTagInfo: {LowerLimit} can't convert to double");
                }
                if (double.TryParse(UpperLimit, out upperLimit))
                {

                }
                else
                {

                    throw new ArgumentOutOfRangeException($"In func ParseTagInfo: {upperLimit} can't convert to double");

                }
                if (int.TryParse(dbNr, out idbNr))
                {

                }
                else
                {
                    throw new ArgumentOutOfRangeException($"In func ParseTagInfo: {dbNr} can't convert to Dbnr");
                }
                if(int.TryParse(startAddress, out istartAddr))
                {

                }
                else
                {
                    throw new ArgumentOutOfRangeException($"In func ParseTagInfo: {dbNr} can't convert to StartAddress");
                }
                if(int.TryParse(dataLength, out idataLength))
                {

                }
                else
                {
                    throw new ArgumentOutOfRangeException($"In func ParseTagInfo: {dbNr} can't convert to DataLength");
                }
                if(byte.TryParse(dataBit, out idataBit))
                {

                }
                else
                {
                    throw new ArgumentOutOfRangeException($"In func ParseTagInfo: {dbNr} can't convert to DataBit");
                }
                tag = new Tag(conn,tagName, idbNr, istartAddr, idataLength, dataType, idataBit, MesName, mesDataType, lowerLimit, upperLimit, isChecked,isUpload);
                conn.AddTag(tag);
            }
            else
            {
                throw new ArgumentOutOfRangeException($"In func ParseTagInfo, Can't parse {content} to PlcTag and connection");
            }
        }
        public Tag GetTag(string tagName)
        {
            Tag result = null;
            foreach (var item in Tags)
            {
                if(item.TagName == tagName)
                {
                    result = item;
                    break;
                }
            }
            return result;
        }

        //获取类似名称
        public List<Tag> GetContaisTag(string tagName)
        {
            List<Tag> result = null;
            foreach (var item in Tags)
            {
                if (tagName.Contains(item.TagName))
                {
                    result.Add(item);
                }
            }
            return result;
        }
    }
}