using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Ctp0600P.Client.Protocols;

public class ClientHelp
{
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
                if (ni is { OperationalStatus: OperationalStatus.Up, Name: "以太网" })
                {
                    return BitConverter.ToString(ni.GetPhysicalAddress().GetAddressBytes());
                }
            }
        }
        catch (Exception)
        {
            // ignored
        }

        return "00-00-00-00-00-00";
    }
    /// <summary>
    /// 获取IP 地址
    /// </summary>
    /// <returns></returns>
    public static string GetIpAddress()
    {
        try
        {
            string hostName = Dns.GetHostName();
            IPHostEntry iPHostEntry = Dns.GetHostEntry(hostName);
            var addressV = iPHostEntry.AddressList.FirstOrDefault(q => q.AddressFamily == AddressFamily.InterNetwork);//ip4地址
            if (addressV != null)
                return addressV.ToString();
            return "0.0.0.0";
        }
        catch (Exception)
        {
            return "0.0.0.0";
        }

    }
    
}