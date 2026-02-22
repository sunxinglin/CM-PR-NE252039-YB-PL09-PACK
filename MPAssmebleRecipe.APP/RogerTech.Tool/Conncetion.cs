using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections;

namespace RogerTech.Tool
{
    public class Connection
    {
        private readonly int MaxLength = 200;
        private static object locker = new object();
        public string IP { get; private set; }
        public int Port { get; private set; }
        public bool Connected { get { return ComProtocol.Connected; } }
        public List<Tag> Tags { get; private set; }
        Dictionary<Tag, TagResult> tagDic;
        public int CycleTime { get; private set; }
        private List<InterPreter> TagGroups;
        IComProtocol ComProtocol;
        public Connection(IComProtocol comProtocol, string ip, int port)
        {
            ComProtocol = comProtocol;
            this.IP = ip;
            this.Port = port;
            Tags = new List<Tag>();
        }
        public void AddTag(Tag tag)
        {
            lock (locker)
            {
                bool flag = true;
                foreach (var item in Tags)
                {
                    if (tag.Equals(item))
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    Tags.Add(tag);
                }
                Tags.Sort();
                InterPerterTags(Tags);
            }

        }
        public void WriteTagValue(Tag tag, object value)
        {
            int index = 0;
            do
            {
                index++;
                byte[] buffer = DataConvert.GetTagBytes(tag, value);
                bool flag = false;
                if (tag.DataType == DataType.BIT)
                {
                    ComProtocol.WriteBit(tag.Dbnr, tag.StartAddress, buffer, tag.DataBit, out flag);
                }
                else
                {
                    ComProtocol.WriteBytes(tag.Dbnr, tag.StartAddress, buffer, out flag);
                }
               

                TagResult obj = ReadTag(tag);

                if (obj.Value.ToString() == value.ToString())
                    break;
            } while (index < retryTimes);
        }

        #region 获取buffer

        #endregion

        private void Connect()
        {
            ComProtocol.Connect();
        }
        public TagResult GetTagValue(Tag tag)
        {
            return tagDic[tag];
        }
        //将要写入的tag转成byte[] 

        public TagResult ReadTag(Tag tag)
        {
            TagResult result = new TagResult();
            bool flag = false;


            byte[] tempBytes = ComProtocol.ReadBytes(tag.Dbnr, tag.StartAddress, tag.DataLength, out flag);
            object obj = DataConvert.GetTagValue(tag, tempBytes);
            result.SetValue(obj);
            result.SetAviliable(flag);

            return result;

        }
        public void ReadTags()
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            if (Connected)
            {
                List<Task> Tasks = new List<Task>();
                for (int i = TagGroups.Count - 1; i >= 0; i--)
                {
                    bool flag = true;
                    byte[] buffer = ComProtocol.ReadBytes(TagGroups[i].DbNr, TagGroups[i].Start, TagGroups[i].Length, out flag);



                    int index = i;
                    Tasks.Add(Task.Run(() =>
                    {
                        TagGroups[index].GetResult(buffer, flag);
                    }));
                }

                if (Task.WaitAll(Tasks.ToArray(), 5000))
                {
                    //throw new TimeoutException();
                }
                else
                {

                }
            }
            else
            {
                Connect();
            }
            // Console.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff")+ "\t"+ "Read:" + IP + ":" + sw.ElapsedMilliseconds);
            if (sw.ElapsedMilliseconds > 300)
            {
            }
            sw.Reset();
        }
        bool busy = false;
        public void Run()
        {
            lock (locker)
            {
                if (busy)
                    return;
                else
                    busy = true;
            }
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            while (true)
            {
                sw.Start();
                try
                {
                    ReadTags();
                }
                catch
                {

                }
                System.Threading.Thread.Sleep(30);
                //   Console.WriteLine(sw.ElapsedMilliseconds);
                sw.Reset();
            }
        }

        internal class InterPreter
        {
            public InterPreter(List<Tag> tags)
            {
                results = new List<TagResult>();
                this.tags = tags;
                this.Start = tags[0].StartAddress;
                this.Length = tags[tags.Count - 1].StartAddress - tags[0].StartAddress + tags[tags.Count - 1].DataLength;
                this.DbNr = tags[0].Dbnr;
            }
            public List<Tag> tags { get; private set; }
            public List<TagResult> results { get; private set; }
            public int Start { get; private set; }

            public int Length { get; private set; }
            public int DbNr { get; private set; }
            //将结果解析写入到tags中
            public void GetResult(byte[] bytes, bool avilid)
            {
                if (bytes != null&& bytes.Length>0)
                {
                    int length = tags[tags.Count - 1].StartAddress + tags[tags.Count - 1].DataLength - tags[0].StartAddress;
                    if (bytes.Length != length)
                        throw new ArgumentOutOfRangeException("In connection InterPreter, Func GetResult tag length not equal byte.length");
                    int offset = tags[0].StartAddress;
                    //将所有result的结果写为不可用状态
                    foreach (var tag in tags)
                    {
                        if (avilid)
                        {
                            byte[] tempBytes = new byte[tag.DataLength];
                            Array.Copy(bytes, tag.StartAddress - offset, tempBytes, 0, tag.DataLength);
                            object obj = DataConvert.GetTagValue(tag, tempBytes);
                            tag.Result.SetValue(obj);
                            tag.Result.SetAviliable(true);
                        }
                        else
                        {
                            tag.Result.SetAviliable(false);
                            tag.Result.SetValue(null);
                        }
                    }
                }
            }

        }
        public override bool Equals(object obj)
        {
            if (obj is Connection)
            {
                Connection conn = (Connection)obj;
                return conn.IP == IP && conn.ComProtocol.Equals(ComProtocol);
            }
            else
            {
                return false;
            }
        }
        private void InterPerterTags(List<Tag> tags)
        {
            TagGroups = new List<InterPreter>();

            if (tags == null)
            {
                return;
            }
            Tag tempTag = tags[0];
            int tagIndex = 0;
            List<Tag> tagList = new List<Tag>();
            tagList.Add(tempTag);
            if (tags.Count == 1)
            {
                InterPreter tempInterPreter = new InterPreter(tagList);
                TagGroups.Add(tempInterPreter);
                return;
            }
            for (int i = 1; i < tags.Count; i++)
            {
                if (tempTag.Dbnr != tags[i].Dbnr)
                {
                    //创建taggroup，
                    InterPreter tempInterPreter = new InterPreter(tagList);
                    TagGroups.Add(tempInterPreter);
                    //重置tempTag
                    tempTag = tags[i];
                    tagIndex = i;
                    tagList = new List<Tag>();
                    tagList.Add(tempTag);
                }
                else
                {
                    if (tags[i].StartAddress - tempTag.StartAddress + tags[i].DataLength <= MaxLength)
                    {
                        tagList.Add(tags[i]);
                    }
                    else
                    {
                        InterPreter tempInterPreter = new InterPreter(tagList);
                        TagGroups.Add(tempInterPreter);
                        //重置tempTag
                        tempTag = tags[i];
                        tagIndex = i;
                        tagList = new List<Tag>();
                        tagList.Add(tags[i]);
                    }
                }
                if (i == tags.Count - 1)
                {
                    InterPreter tempInterPreter = new InterPreter(tagList);
                    TagGroups.Add(tempInterPreter);
                }
            }
        }
        private object SetTagValue(Tag tag, byte[] bytes, bool avilid)
        {
            object result = new object();

            return result;
        }
        #region InterpreterTagValue



        #endregion
        #region  Interpreter Value to Byte Array
        //private byte[] GetBytesFormBool(Tag tag, object value)
        //{
        //    byte[] result = new byte[1];
        //    //先读取当前byte内容，然后写入
        //    if(bool.TryParse(value.ToString(), out bool flag))
        //    {

        //    }
        //    return result;
        //}
        //private byte[] GetBytesFromByte(Tag tag, object value)
        //{
        //    byte[] result = new byte[1];

        //}
        #endregion
        #region 读取方法
        public TagResult GetTagResult(Tag tag)
        {
            TagResult result = new TagResult();


            return result;
        }
        #endregion
        private readonly int retryTimes = 3;
    }
}