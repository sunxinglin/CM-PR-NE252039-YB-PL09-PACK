using MPAssmebleRecipe.Apps;
using MPAssmebleRecipe.Apps.Views;
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
        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            // 注册当前页面的所有按钮
            PermissionController.RegisterPageControls(this);
            AppManager appManager = AppManager.GetInstance();
            PermissionController.UpdatePagePermissions(this, appManager.UserInfo);
        }

        public RoleView(IEventAggregator eventAggregator)
        {
            InitializeComponent();
            Loaded += OnPageLoaded;
            EventAggregator = eventAggregator;     
            EventAggregator.GetEvent<UserInfoEvent>().Subscribe(user =>
            {
                PermissionController.UpdatePagePermissions(this, user);
            });
        }

        public IEventAggregator EventAggregator { get; }

    }
}
