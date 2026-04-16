using Prism.Events;
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

namespace MPAssmebleRecipe.Apps.Views
{
    /// <summary>
    /// UserView.xaml 的交互逻辑
    /// </summary>
    public partial class UserView : UserControl
    {
        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            // 注册当前页面的所有按钮
            PermissionController.RegisterPageControls(this);
            AppManager appManager = AppManager.GetInstance();
            PermissionController.UpdatePagePermissions(this, appManager.UserInfo);
        }
        public UserView(IEventAggregator eventAggregator)
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
    public class PermissionController
    {
        public static void RegisterPageControls(UserControl currentPage)
        {
            var authService = new RogerTech.AuthService.AuthService();
            string pageName = currentPage.GetType().Name;

            // 获取页面所有按钮控件
            var buttons = GetChildControls<Button>(currentPage);

            foreach (var button in buttons)
            {
                if (string.IsNullOrEmpty(button.Name))
                {
                    continue;
                }
                // 注册页面菜单权限
                authService.AddMenu(new Menu
                {
                    Page = pageName,
                    SubPage = "",
                    ElementName = button.Name
                });
            }
        }

        public static void UpdatePagePermissions(UserControl currentPage, UserInfo user)
        {
            string pageName = currentPage.GetType().Name;
            var buttons = GetChildControls<Button>(currentPage);

            foreach (var button in buttons)
            {
                if (string.IsNullOrEmpty(button.Name))
                {
                    button.IsEnabled = true;
                    continue;
                }
                // 检查用户是否有该按钮的权限
                bool hasPermission = user?.UserMenus?.Any(m =>
                    m.Page == pageName &&
                    m.ElementName == button.Name) ?? false;

                button.IsEnabled = hasPermission;
            }
        }

        private static IEnumerable<T> GetChildControls<T>(DependencyObject depObj)
            where T : DependencyObject
        {
            if (depObj == null) yield break;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);

                // 跳过模板部件容器
                if (child is FrameworkElement fe &&
                    fe.TemplatedParent != null &&
                    fe.Name.StartsWith("PART_"))
                {
                    continue;
                }
                if (child is T t)
                    yield return t;

                foreach (var grandChild in GetChildControls<T>(child))
                    yield return grandChild;
            }
        }
    }
}
