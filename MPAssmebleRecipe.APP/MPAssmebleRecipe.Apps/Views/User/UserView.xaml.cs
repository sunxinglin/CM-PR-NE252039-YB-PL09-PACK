using Prism.Events;
using RogerTech.Common;
using RogerTech.Common.AuthService;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Menu = RogerTech.AuthService.Models.Menu;

namespace MPAssmebleRecipe.Apps.Views
{
    /// <summary>
    /// UserView.xaml 的交互逻辑
    /// </summary>
    public partial class UserView : UserControl
    {

        public UserView(IEventAggregator eventAggregator)
        {
            InitializeComponent();
            EventAggregator = eventAggregator;
            AppManager appManager = AppManager.GetInstance();
            ChangUser(appManager.UserInfo);
            EventAggregator.GetEvent<UserInfoEvent>().Subscribe(user =>
            {
                ChangUser(user);
            });

            RegistMenus();
        }
        private void RegistMenus()
        {
            List<Menu> menuList = new List<Menu>()
            {
                new Menu() { Page = "UserView", SubPage = "",ElementName="添加用户" },
                new Menu() { Page = "UserView", SubPage = "",ElementName="删除用户" }
            };

           // List<Menu> menuList = DbContext.GetInstance().Queryable<Menu>().Where(p => p.Page == "UserView" && p.SubPage == "B").ToList();
            RogerTech.AuthService.AuthService authService = new RogerTech.AuthService.AuthService();
            foreach (var item in menuList)
            {
                authService.AddMenu(item);
            }

        }

        public IEventAggregator EventAggregator { get; }

        private void ChangUser(UserInfo user)
        {
            if (user == null)
            {
                添加用户.IsEnabled = false;
                删除用户.IsEnabled = false;
                return;
            }
            var addUser = user.UserMenus.Where(x => x.Page == "UserView" && x.ElementName == "添加用户").FirstOrDefault();
            if (addUser != null)
            {
                添加用户.IsEnabled = true;
            }
            else
            {
                添加用户.IsEnabled = false;
            }
            var delUser = user.UserMenus.Where(x => x.Page == "UserView" && x.ElementName == "删除用户").FirstOrDefault();
            if (delUser != null)
            {
                删除用户.IsEnabled = true;
            }
            else
            {
                删除用户.IsEnabled = false;
            }
        }
    }
}
