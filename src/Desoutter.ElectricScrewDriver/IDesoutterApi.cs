using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desoutter.ElectricScrewDriver
{
    public interface IDesoutterApi
    {
        Task SendMID0001(string key);

        Task SendMID0014(string key);

        Task SendMID0016(string key);

        Task SendMID0018(string key,int pset=0);

        Task SendMID0042(string key);

        Task SendMID0043(string key);

        Task SendMID0060(string key);

        Task SendMID0062(string key);

        Task SendMID0701(string key);

        Task SendMID0070(string key);

        Task SendMID0082(string key);
    }
}