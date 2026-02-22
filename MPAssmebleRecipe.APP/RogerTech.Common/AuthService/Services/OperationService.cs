using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;
using RogerTech.Common.AuthService.Model;
using RogerTech.Common;

namespace RogerTech.Common.AuthService.Services
{
    public class OperationService
    {
        public static UserInfo userInfo;  //这个内容在mian传递过来
        // 获取Ip地址
        public static string GetLocalIpAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var item in host.AddressList)
            {
                if (item.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return item.ToString();
                }
            }
            return "127.0.0.1";
        }

        /// <summary>
        /// 用户操作记录
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="operation">操作</param>
        /// <param name="description">记录内容内容</param>
        public static void OperationRecord(Operation operation, string description)
        {
            if (userInfo == null || userInfo.Name == null)
            {
                return;
            }
            #region 获取类名
            var stackTrace = new StackTrace();
            var frame = stackTrace.GetFrame(1);
            var method = frame.GetMethod();
            var declaringType = method.DeclaringType;
            string callerClassName = declaringType?.FullName;
            #endregion

            string ip = GetLocalIpAddress();
            UserOperationLog log = new UserOperationLog()
            {
                UserName = userInfo.Name,
                EmployeeId = userInfo.EmployeeId,
                Operation = operation,
                ViewName = callerClassName + "--" + method.Name,
                Description = description,
                IpAddress = ip,
                CreatTime = DateTime.Now,
            };
            DbContext.GetInstance().Insertable<UserOperationLog>(log).ExecuteCommand();
        }
    }
}
