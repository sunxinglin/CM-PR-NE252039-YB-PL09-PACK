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
    /// AuthMainView.xaml 的交互逻辑
    /// </summary>
    public partial class AuthMainView : UserControl
    {
        private IRegionManager region;
        public AuthMainView(IRegionManager regionManager)
        {
            InitializeComponent();
            //regionManager.RegisterViewWithRegion("UserRegion", typeof(UserView));
            //regionManager.RegisterViewWithRegion("UserRegion", typeof(RoleView));
            //var region = regionManager.Regions["UserRegion"];
            //region.Activate(RoleView);
            this.region = regionManager;

            RegionManager.SetRegionManager(this, regionManager);
            RegionManager.UpdateRegions();
            this.Unloaded += (s, e) =>
            {
                var region = RegionManager.GetRegionManager(this);
                if (region!=null)
                {
                    region.Regions.Remove("UserRegion");
                }             
            };
            region.Regions["UserRegion"].RequestNavigate("RoleView");
            Console.WriteLine();
            //Loaded += new RoutedEventHandler((s, e) =>
            //{
            //    Console.WriteLine();
            //    var item = region;
            //    r
            //    //Console.WriteLine(item.Regions.ToList().Count);
            //    //Console.WriteLine("-----------------------------");
            //});


            //RegistMenus();
        }

        private void RegistMenus()
        {
            AuthService authService = new AuthService();
            List<Menu> menus = new List<Menu>()
            {
                new Menu(){ Page = "AuthMainWidow"},
            };
            foreach (Menu menu in menus)
            {
                authService.AddMenu(menu);
            }
        }
    }
}
