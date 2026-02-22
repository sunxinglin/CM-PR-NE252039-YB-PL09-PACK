using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Regions;
using UserTest.ViewModels;
using UserTest.Views;

namespace UserManager
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve< UserTest.Views.MainWindow> ();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<AuthMainView>();
            containerRegistry.RegisterForNavigation<UserView>();
            containerRegistry.RegisterForNavigation<RoleView>();
            containerRegistry.RegisterForNavigation<RoleManagerView>();

            containerRegistry.RegisterDialog<AddRoleView>();
            containerRegistry.RegisterDialog<ConfirmDialog, ConfirmDialogViewModel>();
            containerRegistry.RegisterDialog<AddUserDialog, AddUserDialogViewModel>();
            containerRegistry.RegisterDialog<MenuSelectionDialog, MenuSelectionDialogViewModel>();
            containerRegistry.RegisterDialog<LoginDialog, LoginDialogViewModel>();
            containerRegistry.RegisterDialog<AddRoleManagerDialog, AddRoleManagerDialogViewModel>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            if (Container != null)
            {
                var regionManager = Container.Resolve<IRegionManager>();
                var region = regionManager.Regions["UserRegion"];
                region.RequestNavigate("AuthMainView");
            }
            base.OnStartup(e);
        }
    }
}
