using MPAssmebleRecipe.Apps;
using Prism.Events;
using RogerTech.Common;
using RogerTech.Common.AuthService;
using SqlSugar;
using System.Collections.Generic;
using System.Windows.Controls;
using Menu = RogerTech.AuthService.Models.Menu;
using System.Linq;
using MPAssmebleRecipe.Apps.Views;
using System.Windows;

namespace UserTest.Views
{
    public partial class RoleManagerView : UserControl
    {
        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            // 注册当前页面的所有按钮
            PermissionController.RegisterPageControls(this);
            AppManager appManager = AppManager.GetInstance();
            PermissionController.UpdatePagePermissions(this, appManager.UserInfo);
        }
        public RoleManagerView(IEventAggregator eventAggregator)
        {
            InitializeComponent();
            Loaded += OnPageLoaded;
            EventAggregator = eventAggregator;
            AppManager appManager = AppManager.GetInstance();
            EventAggregator.GetEvent<UserInfoEvent>().Subscribe(user =>
            {
                PermissionController.UpdatePagePermissions(this, user);
            });
        }
        public IEventAggregator EventAggregator { get; }

    }
}