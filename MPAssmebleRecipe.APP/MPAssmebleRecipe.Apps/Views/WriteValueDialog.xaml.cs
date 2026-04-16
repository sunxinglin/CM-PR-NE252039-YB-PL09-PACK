using Prism.Events;
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

namespace MPAssmebleRecipe.Apps.Views
{
    /// <summary>
    /// WriteValueDialog.xaml 的交互逻辑
    /// </summary>
    public partial class WriteValueDialog : UserControl
    {
        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            // 注册当前页面的所有按钮
            PermissionController.RegisterPageControls(this);
            AppManager appManager = AppManager.GetInstance();
            PermissionController.UpdatePagePermissions(this, appManager.UserInfo);
        }
        public IEventAggregator EventAggregator { get; }

        public WriteValueDialog(IEventAggregator eventAggregator)
        {
            InitializeComponent();
            Loaded += OnPageLoaded;
            EventAggregator = eventAggregator;
            EventAggregator.GetEvent<UserInfoEvent>().Subscribe(user =>
            {
                PermissionController.UpdatePagePermissions(this, user);
            });
        }
    }
}
