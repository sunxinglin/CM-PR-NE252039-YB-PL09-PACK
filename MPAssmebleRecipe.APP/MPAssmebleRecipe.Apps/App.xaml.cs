using Prism.DryIoc;
using Prism.Ioc;
using System.Windows;
using MPAssmebleRecipe.Apps.Views;
using MPAssmebleRecipe.Apps.ViewModels;
using Prism.Modularity;
using MPAssmebleRecipe.Logger.Interfaces;
using MPAssmebleRecipe.Logger;
using MPAssmebleRecipe.Apps.Views.PackManage;
using MPAssmebleRecipe.Apps.ViewModels.PackManage;
using System;
using DryIoc;
using MPAssmebleRecipe.Apps.Views.BlockManage;
using MPAssmebleRecipe.Apps.ViewModels.BlockManage;
using MPAssmebleRecipe.Apps.ViewModels.ModuleManage;
using MPAssmebleRecipe.Apps.Views.ModuleManage;
using MPAssmebleRecipe.Apps.ViewModels.CellManage;
using MPAssmebleRecipe.Apps.Views.CellManage;
using System.Diagnostics;
using UserTest.Views;
using UserTest.ViewModels;
using MPAssmebleRecipe.Apps.Views.Recipe;
using MPAssmebleRecipe.Apps.ViewModels.Recipe;
using Prism.Events;

namespace MPAssmebleRecipe.Apps
{
    public partial class App : PrismApplication
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
        }
        /// <summary>
        /// 应用退出时清理资源
        /// </summary>
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
        }
        protected override Window CreateShell()
        {
            if (ProcessExist())
            {
                MessageBox.Show("程序已经在运行");
                Application.Current.Shutdown();
            }
            return Container.Resolve<MainView>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // 确保所有依赖都已正确注册
            containerRegistry.RegisterSingleton<ILoggerHelper>(() => NLogManager.GetLogger("App"));
            containerRegistry.RegisterSingleton<IEventAggregator, EventAggregator>();

            // 注册视图和VM
            containerRegistry.RegisterForNavigation<UcPackManageView, UcPackManageViewModel>();                 //配方管理界面
            containerRegistry.RegisterForNavigation<UserView, UserViewModel>();                                //用户管理界面
            containerRegistry.RegisterForNavigation<LogView, LogViewModel>();                                    //新日志界面
            containerRegistry.RegisterForNavigation<MESCFGView, MESCFGViewModel>();                              //MES配置界面
            containerRegistry.RegisterForNavigation<UploadDataView, UploadDataViewModel>();                      //数据重传界面      
            containerRegistry.RegisterForNavigation<PlcTagView, PlcTagViewModel>();                              //PLC配置界面
            containerRegistry.RegisterForNavigation<UcOperationView, UcOperationViewModel>();                   //日志查询界面             

            containerRegistry.RegisterForNavigation<RoleManagerView, RoleManagerViewModel>();                    //角色管理
            containerRegistry.RegisterForNavigation<RoleView, RoleViewModel>();                                  //用户管理
            containerRegistry.RegisterForNavigation<AuthMainView, AuthMainViewModel>();                          //用户管理
            //配方页面
            containerRegistry.RegisterForNavigation<UcPackManageView, UcPackManageViewModel>();
            containerRegistry.RegisterForNavigation<UcRecipeManageView, UcRecipeManageViewModel>();
            containerRegistry.RegisterForNavigation<UcCurrentCellOrderView, UcCurrentCellOrderViewModel>();

            containerRegistry.RegisterDialog<UcBlockAddView, UcBlockAddViewModel>();
            containerRegistry.RegisterDialog<UcBlockEditView, UcBlockEditViewModel>();
            containerRegistry.RegisterDialog<UcCellEditView, UcCellEditViewModel>();
            containerRegistry.RegisterDialog<UcModuleAddView, UcModuleAddViewModel>();
            containerRegistry.RegisterDialog<UcModuleEditView, UcModuleEditViewModel>();
            containerRegistry.RegisterDialog<UcPackAddView, UcPackAddViewModel>();
            containerRegistry.RegisterDialog<CellRecipeReplaceView, CellRecipeReplaceViewModel>();

            containerRegistry.RegisterDialog<UcRecipeAddView, UcRecipeAddViewModel>();


            containerRegistry.RegisterDialog<WriteValueDialog, WriteValueDialogViewModel>();  //写入变量



            containerRegistry.RegisterDialog<AddUserDialog, AddUserDialogViewModel>();                          //添加用户
            containerRegistry.RegisterDialog<AddRoleManagerDialog, AddRoleManagerDialogViewModel>();             //管理角色
            containerRegistry.RegisterDialog<AddRoleView, AddRoleViewModel>();                                  //添加角色
            containerRegistry.RegisterDialog<MenuSelectionDialog, MenuSelectionDialogViewModel>();              //菜单管理
            containerRegistry.RegisterDialog<AddPlcTagDialog, AddPlcTagDialogViewModel>();                      //添加变量
            containerRegistry.RegisterDialog<ConfirmDialog, ConfirmDialogViewModel>();                          //删除用户
            containerRegistry.RegisterDialog<CardLoginDialog, CardLoginDialogViewModel>();                      //刷卡登录         
            containerRegistry.RegisterDialog<CodeDetailView, CodeDetailViewModel>();                            //MES 5位错误代码展示         
        }

        static bool ProcessExist()
        {
            bool result = false;
            var processName = Process.GetCurrentProcess().ProcessName;
            var items = Process.GetProcessesByName(processName);
            if (items != null && items.Length >= 2)
            {
                result = true;
                Console.WriteLine();
            }
            return result;
        }
    }
}