using MPAssmebleRecipe.Apps;
using Prism.Events;
using RogerTech.AuthService;
using RogerTech.AuthService.Models;
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

namespace UserTest.Views
{
    /// <summary>
    /// RoleView.xaml 的交互逻辑
    /// </summary>
    public partial class RoleView : UserControl
    {

        public RoleView(IEventAggregator eventAggregator)
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
            AuthService authService = new AuthService();
            List<Menu> menus = new List<Menu>()
            {
                new Menu(){ Page = "AuthMainWidow",SubPage = "RoleView", ElementName = "编辑管理菜单"},
                new Menu(){ Page = "AuthMainWidow",SubPage = "RoleView", ElementName = "添加角色"},
                new Menu(){ Page = "AuthMainWidow",SubPage = "RoleView", ElementName = "删除角色"},
            };

            //   List<Menu> menus = DbContext.GetInstance().Queryable<Menu>().Where(p => p.Page == "AuthMainWidow" && p.SubPage == "RoleView").ToList();
            foreach (Menu menu in menus)
            {
                authService.AddMenu(menu);
            }
        }


        public IEventAggregator EventAggregator { get; }

        private void ChangUser(UserInfo user)
        {
            if (user == null)
            {
                添加角色.IsEnabled = false;
                删除角色.IsEnabled = false;
                编辑管理菜单.IsEnabled = false;
                return;
            }
            var add = user.UserMenus.Where(x => x.SubPage == "RoleView" && x.ElementName == "添加角色").FirstOrDefault();
            if (add != null)
            {
                添加角色.IsEnabled = true;
            }
            else
            {
                添加角色.IsEnabled = false;
            }

            var del = user.UserMenus.Where(x => x.SubPage == "RoleView" && x.ElementName == "删除角色").FirstOrDefault();
            if (del != null)
            {
                删除角色.IsEnabled = true;
            }
            else
            {
                删除角色.IsEnabled = false;
            }

            var edit = user.UserMenus.Where(x => x.SubPage == "RoleView" && x.ElementName == "编辑管理菜单").FirstOrDefault();
            if (edit != null)
            {
                编辑管理菜单.IsEnabled = true;
            }
            else
            {
                编辑管理菜单.IsEnabled = false;
            }
        }
    }
}
