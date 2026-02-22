using Prism.Mvvm;
using RogerTech.AuthService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogerTech.Common.AuthService
{
    /// <summary>
    /// 用于
    /// </summary>
    public class UserInfo:BindableBase
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private string employeeId;

        public string EmployeeId
        {
            get { return employeeId; }
            set { employeeId = value; }
        }

        public List<Menu> UserMenus { get; set; } //用户可用操作的菜单
        public List<Workstation> Workstations { get; set; } //用户可用操作的工位

        private string roleName;

        public string RoleName
        {
            get { return roleName; }
            set { roleName = value; }
        }

        private int logOutTime;

        public int LogOutTime
        {
            get { return logOutTime; }
            set { logOutTime = value; }
        }
    }
}
