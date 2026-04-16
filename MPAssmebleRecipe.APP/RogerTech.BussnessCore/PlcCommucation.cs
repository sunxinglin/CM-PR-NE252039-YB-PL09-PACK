using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RogerTech.Common;
using RogerTech.Tool;

namespace RogerTech.BussnessCore
{
    public class PlcCommucation
    {
        //定义触发器变化事件
        public EventHandler<PlcCommArgs> PlcTriggerChanged;
        public EventHandler<PlcCommArgs> UserChanged;
        public EventHandler ConnectedChangd;

        Dictionary<int, Group> GroupDic = new Dictionary<int, Group>();
        //plc变量组
        public List<Group> Groups { get; private set; }
        public Server PlcServer { get; private set; }


        public List<Tag> HeartBeat { get; private set; }
        public PlcCommucation(List<Group> groups, Server server)
        {
            this.PlcServer = server;
            this.Groups = groups;
            triggersTemp = new List<object>();
            triggers = new List<Tag>();
            CardIdTemp = new List<object>();
            cardId = new List<Tag>();


            heartBeats = new List<Tag>();
            //一个线程刷新变量值
            foreach (var group in Groups)
            {
                foreach (var tag in group.Tags)
                {
                    if (tag.TagName.Contains("StartSignal"))
                    {
                        triggers.Add(tag);
                        triggersTemp.Add(new object());
                        GroupDic.Add(tag.GetHashCode(), group);
                    }
                    if (tag.TagName.Contains("HeartBeat"))
                    {
                        heartBeats.Add(tag);
                        GroupDic.Add(tag.GetHashCode(), group);
                    }
                    if (tag.TagName.Contains("UserCardId"))
                    {
                        cardId.Add(tag);
                        CardIdTemp.Add(new object());
                        GroupDic.Add(tag.GetHashCode(), group);
                    }

                }
            }
            //一个线程处理公共信号

            Task.Run(() => PlcTagMonitor());
            Task.Run(() => PlcCommonTag());

        }
        List<object> triggersTemp;
        List<Tag> triggers;
        List<Tag> heartBeats;
        bool isFirstCycle = true;
        bool heartBeatFlag;
        bool conntcted = false;

        List<object> CardIdTemp;
        List<Tag> cardId;

        public bool Connected
        {
            get
            {
                bool connected = true;
                foreach (var item in PlcServer.Connections)
                {
                    if (!item.Connected)
                    {
                        connected = false;
                    }
                }
                return connected;
            }
        }
        public void PlcTagMonitor()
        {
            DbContext.Info("心跳", $"系统重启,连接中:PLC IP：{string.Join(",", PlcServer.Connections.Select(c => c.IP))}", 9999, "全部日志");

            while (true)
            {

                try
                {
                    if (conntcted != Connected)
                    {
                        conntcted = Connected;
                        if (!conntcted)
                        {
                            DbContext.Error("心跳", $"系统连接失败：PLC IP：{string.Join(",", PlcServer.Connections.Select(c => c.IP))}", 9999, "全部日志");
                        }
                        else
                        {
                            DbContext.Info("心跳", $"系统已连接：PLC IP：{string.Join(",", PlcServer.Connections.Select(c => c.IP))}", 9999, "全部日志");
                        }
                        ConnectedChangd?.Invoke(this, EventArgs.Empty);
                    }
                    if (!Connected)
                    {
                        isFirstCycle = true;
                    }
                    else
                    {
                        if (isFirstCycle)
                        {
                            isFirstCycle = false;
                            for (int i = 0; i < triggers.Count; i++)
                            {
                                if (triggers[i].Result.Available)
                                {
                                    triggersTemp[i] = triggers[i].Result.Value.ToString();
                                }
                                else
                                {
                                    isFirstCycle = true;
                                    continue;
                                }

                            }
                            for (int i = 0; i < cardId.Count; i++)
                            {
                                if (cardId[i].Result.Available)
                                {
                                    CardIdTemp[i] = cardId[i].Result.Value.ToString();
                                }
                                else
                                {
                                    isFirstCycle = true;
                                    continue;
                                }

                            }
                        }
                        else
                        {
                            ProcessTagChange();
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    Thread.Sleep(50);
                }
            }
        }
        public void PlcCommonTag()
        {
            DateTime dtNow = DateTime.Now;
            while (true)
            {
                try
                {
                    if (Connected)
                    {
                        TimeSpan sp = DateTime.Now.Subtract(dtNow);
                        if (sp.TotalMilliseconds > 1000)
                        {
                            dtNow = DateTime.Now;
                            heartBeatFlag = !heartBeatFlag;
                            foreach (var item in heartBeats)
                            {
                                item.WriteValue(heartBeatFlag);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    Thread.Sleep(100);
                }
            }
        }

        private void ProcessTagChange()
        {
            for (int i = 0; i < triggers.Count; i++)
            {
                if (triggersTemp[i].ToString() != triggers[i].Result.Value.ToString())
                {
                    triggersTemp[i] = triggers[i].Result.Value.ToString();
                    PlcTriggerChanged?.Invoke(this, new PlcCommArgs(GroupDic[triggers[i].GetHashCode()], triggers[i].Result.Value));

                }
                else
                {
                    triggersTemp[i] = triggers[i].Result.Value.ToString();
                }

            }
            for (int i = 0; i < cardId.Count; i++)
            {
                if (CardIdTemp[i].ToString() != cardId[i].Result.Value.ToString())
                {
                    CardIdTemp[i] = cardId[i].Result.Value.ToString();
                    UserChanged?.Invoke(this, new PlcCommArgs(GroupDic[cardId[i].GetHashCode()], cardId[i].Result.Value));

                }
                else
                {
                    CardIdTemp[i] = cardId[i].Result.Value.ToString();
                }

            }

        }
    }
}
