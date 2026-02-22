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
    /// RoleView.xaml 的交互逻辑
    /// </summary>
    public partial class RoleView : UserControl
    {
        public RoleView()
        {
            InitializeComponent();
            RegistMenus();
        }

        private void RegistMenus()
        {
            AuthService authService = new AuthService();
            List<Menu> menus = new List<Menu>()
            {
                new Menu(){ Page = "AuthMainWidow",SubPage = "RoleView"},
                new Menu(){ Page = "AuthMainWidow",SubPage = "RoleView", ElementName = "AddRole"},
                new Menu(){ Page = "AuthMainWidow",SubPage = "RoleView", ElementName = "RemoveRole"},
            };
            foreach (Menu menu in menus)
            {
                authService.AddMenu(menu);
            }
        }
    }
}
