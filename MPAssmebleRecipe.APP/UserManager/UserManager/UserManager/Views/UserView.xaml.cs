using Prism.Regions;
using RogerTech.AuthService;
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
using RogerTech.AuthService.Models;
using Menu = RogerTech.AuthService.Models.Menu;

namespace UserTest.Views
{
    /// <summary>
    /// UserView.xaml 的交互逻辑
    /// </summary>
    public partial class UserView : UserControl
    {
        public UserView(IRegionManager region)
        {
            InitializeComponent();
            Console.WriteLine();
            RegistMenus();
        }

        //程序启动时候，注册菜单
        private void RegistMenus()
        {
            AuthService authService = new AuthService();
            List<Menu> menus = new List<Menu>()
            {
                new Menu(){ Page = "AuthMainWidow",SubPage = "UserView"},
                new Menu(){ Page = "AuthMainWidow",SubPage = "UserView", ElementName = "AddRole"},
                new Menu(){ Page = "AuthMainWidow",SubPage = "UserView", ElementName = "RemoveRole"},
            };
            foreach (Menu menu in menus)
            {
                authService.AddMenu(menu);
            }
        }
    }
}
