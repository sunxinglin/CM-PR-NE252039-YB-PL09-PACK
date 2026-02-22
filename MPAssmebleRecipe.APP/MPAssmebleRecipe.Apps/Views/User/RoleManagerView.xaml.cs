using MPAssmebleRecipe.Apps;
using Prism.Events;
using RogerTech.Common;
using RogerTech.Common.AuthService;
using SqlSugar;
using System.Collections.Generic;
using System.Windows.Controls;
using Menu = RogerTech.AuthService.Models.Menu;
using System.Linq;

namespace UserTest.Views
{
    public partial class RoleManagerView : UserControl
    {
        public RoleManagerView(IEventAggregator eventAggregator)
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
            // List<Menu> menus = DbContext.GetInstance().Queryable<Menu>().Where(p => p.Page == "RoleManagerView" && p.SubPage == "B").ToList();
            List<Menu> menus = new List<Menu>()
            {
                new Menu(){ Page = "AuthMainWidow",SubPage = "RoleManagerView", ElementName = "管理角色关系"},       
            };
            RogerTech.AuthService.AuthService authService = new RogerTech.AuthService.AuthService();
            foreach (var item in menus)
            {
                authService.AddMenu(item);
            }

        }

        public IEventAggregator EventAggregator { get; }

        private void ChangUser(UserInfo user)
        {
            if (user == null)
            {
                管理角色关系.IsEnabled = false;
                删除管理关系.IsEnabled = false;
                return;
            }
            var add = user.UserMenus.Where(x=> x.SubPage == "RoleManagerView" && x.ElementName == "管理角色关系").FirstOrDefault();
            if (add != null)
            {
                管理角色关系.IsEnabled = true;
            }
            else
            {
                管理角色关系.IsEnabled = false;
            }

            var del = user.UserMenus.Where(x => x.SubPage == "RoleManagerView" && x.ElementName == "删除管理关系").FirstOrDefault();
            if (del != null)
            {
                删除管理关系.IsEnabled = true;
            }
            else
            {
                删除管理关系.IsEnabled = false;
            }
        }
    }
} 