using System;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Ctp0600P.Client.CommonHelper
{
    public class ClientHelper
    {
        /// <summary>
        /// 获取打开网卡的mac地址
        /// </summary>
        /// <returns></returns>
        public static string GetMAC()
        {
            string mac = string.Empty;
            ManagementClass mcMAC = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection mocMAC = mcMAC.GetInstances();
            foreach (ManagementObject m in mocMAC)
            {

                if ((bool)m["IPEnabled"])
                {
                    mac = m["MacAddress"].ToString();
                    break;
                }
            }
            return mac;
        }

        /// <summary>
        /// 获取mac地址
        /// </summary>
        /// <returns></returns>
        public static string GetMacByNetworkInterface()
        {
            try
            {
                NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();

                foreach (NetworkInterface ni in interfaces)
                {
                    if (ni.OperationalStatus == OperationalStatus.Up && ni.Name == "以太网")
                    {
                        return BitConverter.ToString(ni.GetPhysicalAddress().GetAddressBytes());
                    }
                }
            }
            catch (Exception)
            {
            }
            return "00-00-00-00-00-00";
        }

        /// <summary>
        /// 获取IP 地址
        /// </summary>
        /// <param name="interfaceName">指定网卡名称（如 "以太网"、"以太网 2"），为空则获取第一个可用IP</param>
        /// <returns></returns>
        public static string GetIpAddress(string interfaceName = null)
        {
            try
            {
                // 优先查找活动的以太网或无线网卡
                var query = NetworkInterface.GetAllNetworkInterfaces()
                    .Where(n => n.OperationalStatus == OperationalStatus.Up &&
                                (n.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
                                 n.NetworkInterfaceType == NetworkInterfaceType.Wireless80211));

                // 如果指定了网卡名称，则进行过滤
                if (!string.IsNullOrWhiteSpace(interfaceName))
                {
                    query = query.Where(n => n.Name.Equals(interfaceName, StringComparison.OrdinalIgnoreCase));
                }

                var interfaces = query.ToList();

                foreach (var ni in interfaces)
                {
                    var props = ni.GetIPProperties();
                    var result = props.UnicastAddresses
                        .FirstOrDefault(x => x.Address.AddressFamily == AddressFamily.InterNetwork);
                    
                    if (result != null)
                    {
                        return result.Address.ToString();
                    }
                }

                // 如果没找到，回退到DNS方式
                string hostName = Dns.GetHostName();
                IPHostEntry iPHostEntry = Dns.GetHostEntry(hostName);
                var addressV = iPHostEntry.AddressList.FirstOrDefault(q => q.AddressFamily == AddressFamily.InterNetwork);
                if (addressV != null)
                    return addressV.ToString();
                
                return "127.0.0.1";
            }
            catch (Exception)
            {
                return "127.0.0.1";
            }
        }
    }
}
