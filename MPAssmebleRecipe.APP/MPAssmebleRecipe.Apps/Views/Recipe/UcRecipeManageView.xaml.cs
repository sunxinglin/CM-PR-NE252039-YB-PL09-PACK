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
        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            // 注册当前页面的所有按钮
            PermissionController.RegisterPageControls(this);
            AppManager appManager = AppManager.GetInstance();
            PermissionController.UpdatePagePermissions(this, appManager.UserInfo);
        }
        public IEventAggregator EventAggregator { get; }
        public UcRecipeManageView(ILoggerHelper logger, IEventAggregator eventAggregator)
        {
            InitializeComponent();
            Loaded += OnPageLoaded;
            this.EventAggregator = eventAggregator;
            AppManager appManager = AppManager.GetInstance();
        
            EventAggregator.GetEvent<UserInfoEvent>().Subscribe(user =>
            {
                PermissionController.UpdatePagePermissions(this, user);
            });

        }

      
    }
}
