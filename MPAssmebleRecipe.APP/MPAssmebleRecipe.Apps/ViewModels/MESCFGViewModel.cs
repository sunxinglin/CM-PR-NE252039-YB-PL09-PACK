using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using RogerTech.Common;
using System.Windows.Input;
using RogerTech.Common.Models;
using RogerTech.BussnessCore;
using Prism.Services.Dialogs;
using System.Linq.Expressions;
using RogerTech.Common.AuthService.Services;
using CatlMesBase;
using System.Reflection;
using ImTools;
using System.Windows.Controls;
using System.IO;
using System.Data;



namespace MPAssmebleRecipe.Apps.ViewModels
{
    public class MESCFGViewModel : BindableBase, IRegionMemberLifetime
    {
        private readonly IDialogService _dialogService;

        #region 属性
        private string _interfaceName;
        public string InterfaceName
        {
            get { return _interfaceName; }
            set { SetProperty(ref _interfaceName, value); } // 触发属性变更通知
        }
        private ObservableCollection<MenuItem> _menuItems;
        public ObservableCollection<MenuItem> MenuItems
        {
            get { return _menuItems; }
            set { SetProperty(ref _menuItems, value); }
        }

        private MenuItem _selectedMenuItem;
        public MenuItem SelectedMenuItem
        {
            get { return _selectedMenuItem; }
            set
            {
                if (SetProperty(ref _selectedMenuItem, value) && value != null)
                {

                    LoadSettings();
                }
            }
        }

        #endregion

        #region 命令
        public ICommand SaveCommand { get; private set; }



        public bool KeepAlive => true;

        #endregion
        public void LoadSettings()
        {
            // 加载菜单项
            BussnessUtility bussness = BussnessUtility.GetInstanse();
            MesInterface interfacemes;
            try
            {
                interfacemes = (MesInterface)Enum.Parse(typeof(MesInterface), SelectedMenuItem.Title);
                MesInvokeBase mes = bussness.MesInvoke(interfacemes);
                InterfaceName = mes.InterfaceName;
            }
            catch (Exception ex)
            {
                InterfaceName = "配置文件接口名称与枚举类型不匹配";
            }
        }

      

        public MESCFGViewModel(IDialogService dialogService)
        {
     
            InitializeMenuItems();
            _dialogService = dialogService;
        }

        private void InitializeMenuItems()
        {
            MenuItems = new ObservableCollection<MenuItem>();
            string[] sections = IniFileHelper.ReadSections(Path.Combine(Directory.GetCurrentDirectory(), "MESCFG.ini"));
            // 遍历DayOfWeek枚举类型的所有值
            foreach (var item in sections)
            {
                MenuItem menuItem = new MenuItem { Title = item.ToString(), Icon = "API" };
                MenuItems.Add(menuItem);
            }
            SelectedMenuItem = MenuItems.FirstOrDefault();
        }
    
    }
}
