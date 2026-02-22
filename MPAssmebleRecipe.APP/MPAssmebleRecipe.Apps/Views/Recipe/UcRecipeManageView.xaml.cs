using MPAssmebleRecipe.Logger.Interfaces;
using Prism.Events;
using Prism.Ioc;
using RogerTech.Common;
using RogerTech.Common.AuthService;
using RogerTech.Common.Models;
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
using System.Windows.Shapes;
using Menu = RogerTech.AuthService.Models.Menu;

namespace MPAssmebleRecipe.Apps.Views.Recipe
{
    /// <summary>
    /// UcRecipeManageView.xaml 的交互逻辑
    /// </summary>
    public partial class UcRecipeManageView : UserControl
    {
        public IEventAggregator EventAggregator { get; }
        public UcRecipeManageView(ILoggerHelper logger, IEventAggregator eventAggregator)
        {
            InitializeComponent();

            this.EventAggregator = eventAggregator;
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
            List<RogerTech.AuthService.Models.Menu> menuList = new List<RogerTech.AuthService.Models.Menu>()
            {
                new RogerTech.AuthService.Models.Menu() { Page = "UcRecipeManageView", SubPage = "",ElementName="配方下发" },
                new RogerTech.AuthService.Models.Menu() { Page = "UcRecipeManageView", SubPage = "",ElementName="选中行下方插入" },
                new RogerTech.AuthService.Models.Menu() { Page = "UcRecipeManageView", SubPage = "",ElementName="配方清空" },
            };
            RogerTech.AuthService.AuthService authService = new RogerTech.AuthService.AuthService();
            foreach (var item in menuList)
            {
                authService.AddMenu(item);
            }
        }
        private void ChangUser(UserInfo user)
        {
            if (user == null)
            {
                配方下发.IsEnabled = false;
                选中行下方插入.IsEnabled = false;
                配方清空.IsEnabled = false;
                return;
            }
            var addUser = user.UserMenus.Where(x => x.Page == "UcRecipeManageView" && x.ElementName == "配方下发").FirstOrDefault();
            配方下发.IsEnabled = addUser != null ? true : false;
            addUser = user.UserMenus.Where(x => x.Page == "UcRecipeManageView" && x.ElementName == "选中行下方插入").FirstOrDefault();
            选中行下方插入.IsEnabled = addUser != null ? true : false;
            addUser = user.UserMenus.Where(x => x.Page == "UcRecipeManageView" && x.ElementName == "配方清空").FirstOrDefault();
            配方清空.IsEnabled = addUser != null ? true : false;
        }
    }
}
