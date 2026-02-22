using NModbus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace HJZK.IOController
{
    /// <summary>
    /// IO盒子连接
    /// </summary>
    public class IOBoxConnect
    {
        public async Task EnsureConnect(IOBoxConfig box)
        {
            if (Connected(box.Client))
            {
                return;
            }
            tryConnect(box);
        }

        public async void tryConnect(IOBoxConfig box)
        {
            await CreateConnect(box);
        }

        public bool Connected(TcpClient client)
        {
            //if (client == null || client.Client == null)
            //    return false;
            if (client?.Client?.Connected != true)
                return false;

            var part1 = client.Client.Poll(1000, SelectMode.SelectRead);
            var part2 = client.Client.Available == 0;
            if (part1 && part2)
            {
                client.Dispose();
                return false;
            }
            else return true;
        }

        private SemaphoreSlim SemaphoreSlim = new SemaphoreSlim(1, 1);
        public async Task CreateConnect(IOBoxConfig box)
        {
            var entered = await this.SemaphoreSlim.WaitAsync(3000);
            if (!entered)
            {
                return;
                //throw new TimeoutException($"Modbus 设备名：{box.Name}获取锁超时");
            }
            try
            {
                box.Client = new TcpClient();

                box.Client.Connect(box.IP, box.Port);
                var factory = new ModbusFactory();
                box.Master = factory.CreateMaster(box.Client);
                box.Master.Transport.ReadTimeout = 1000;
                box.Master.Transport.WriteTimeout = 1000;
            }
            catch
            {
            }
            finally
            {
                this.SemaphoreSlim.Release();
            }
        }
    }
}
