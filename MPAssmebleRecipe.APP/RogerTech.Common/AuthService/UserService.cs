using RogerTech.AuthService.Models;
using RogerTech.Common.AuthService.Services;
using RogerTech.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogerTech.Common.AuthService
{

    /// <summary>
    /// 使用userservice管理用户信息，以及用户注册
    /// </summary>
    public class UserService
    {

        public UserInfo UserInfo { get; set; } = null;

        //用户名密码登录
        public UserInfo Login(string username, string password)
        {
            return Login(password);
        }
        //用密码登录
        public UserInfo Login( string password)
        {
            UserInfo result = null;
            var user = DbContext.GetInstance().Queryable<User>().Where(x => x.PasswordHash == password).First();
            if (user != null)
            {
                result = new UserInfo();
                result.Name = user.UserName;
                result.RoleName = user.RoleName;
                result.EmployeeId = user.EmployeeId;
                var role = DbContext.GetInstance().Queryable<Role>().Where(x => x.RoleName == user.RoleName).First();
                if (role != null)
                {
                    var roleMenus = DbContext.GetInstance().Queryable<RoleMenu>().Where(x => x.RoleId == role.Id).ToList();
                    
                    if (roleMenus != null)
                    {
                        var ids = roleMenus.Select(x => x.MenuId).ToList();
                        var menus = DbContext.GetInstance().Queryable<Menu>().Where(x => ids.Contains(x.Id)).ToList();
                        result.UserMenus = menus;
                    }
               
                }
                if (user.UserName == "SystemAdmin")
                {
                    var menus = DbContext.GetInstance().Queryable<Menu>().ToList();
                    result.UserMenus = menus;
                }
            }
            UserChange?.Invoke(this, new EventArgs());
            return result;
        }
        public UserInfo LogOut()
        {
            UserChange?.Invoke(this, new EventArgs());
            return new UserInfo();
        }
        //用户切换
        public event EventHandler<EventArgs> UserChange;
    }
}
