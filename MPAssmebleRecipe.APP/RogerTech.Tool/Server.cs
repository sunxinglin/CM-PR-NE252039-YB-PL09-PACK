using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace RogerTech.Tool
{
    public class Server
    {
        private Server()
        {
            
        }
        private static Server instance = new Server();
        public static Server GetInstance()
        {
            return instance;
        }
        public int MyProperty { get; private set; }
        private bool serverState = false;
        public List<Connection> Connections {get; private set;}
        public List<Group> Groups {get; private set;}
        //如果没有connection 创建添加到connections，如果已有，从已有中返回
        internal Connection GetConnection(IComProtocol comProtocol,string ip,int port)
        {
            if(Connections == null)
            {
                Connections = new List<Connection>();
            }
            bool flag = true;
            Connection conn = null;
            foreach (var item in Connections)
            {
                if(item.IP == ip)
                {
                    flag = false;
                    conn = item;
                    break;
                }
            }
            if(flag)
            {
                conn = new Connection(comProtocol,ip, port);
                Connections.Add(conn);
                
            }
            return conn;
        }
        public void AddGroup(Group group)
        {            
            if(Groups == null)
            {
                Groups = new List<Group>();
            }
            bool flag = true;
            foreach (var item in Groups)
            {
                if (item.GroupName == group.GroupName)
                {
                    flag = false;
                    break;
                }
            }
            if(flag)
            {
                Groups.Add(group);
            }
        }
        static bool flag = false;
        static object locker = new object();
    }
}