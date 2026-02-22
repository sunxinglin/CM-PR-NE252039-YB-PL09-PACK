using Prism.Mvvm;
using Prism.Regions;
using RogerTech.BussnessCore;
using RogerTech.PlcCore;
using RogerTech.Tool;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MPAssmebleRecipe.Apps.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;
        private MenuItem _selectedMenuItem;

        public ObservableCollection<MenuItem> MenuItems { get; private set; }

        public MenuItem SelectedMenuItem
        {
            get => _selectedMenuItem;
            set
            {
                if (SetProperty(ref _selectedMenuItem, value))
                {
                    NavigateToSelectedView();
                }
            }
        }

        // 修改构造函数，只依赖IRegionManager
        public MainWindowViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            InitializeMenuItems();
            IniPlcConnet();
        }

        private void InitializeMenuItems()
        {
            MenuItems = new ObservableCollection<MenuItem>
            {
                new MenuItem { Icon = "📊", Title = "生产日志", NavigationPath = "UcLogView" },
                new MenuItem { Icon = "📋", Title = "当前下发电芯顺序", NavigationPath = "UcCurrentCellOrderView" },
                new MenuItem { Icon = "⚙️", Title = "Pack配方管理", NavigationPath = "UcPackManageView" },
                new MenuItem { Icon = "🔄", Title = "转线配方管理", NavigationPath = "UcChangeLineManagerView" },
                new MenuItem { Icon = "⚡", Title = "生产配置管理", NavigationPath = "UcProductionView" },
                new MenuItem { Icon = "🔬", Title = "转线托盘模拟", NavigationPath = "UcTraySimulationView" }
            };
        }

        private void NavigateToSelectedView()
        {
            if (_selectedMenuItem != null)
            {
                // 先清除当前区域的所有视图
                _regionManager.Regions["ContentRegion"].RemoveAll();
                _regionManager.RequestNavigate("ContentRegion", _selectedMenuItem.NavigationPath, NavigationComplete);
            }
        }

        private void NavigationComplete(NavigationResult result)
        {
            if ((bool)!result.Result)
            {
                // 如果导航失败，导航到NotFoundView
                _regionManager.RequestNavigate("ContentRegion", "UcEmptyView");
            }
        }
        public  BussnessCore BussnessCore;
        Server plc = Server.GetInstace();
        private void IniPlcConnet()
        {
//            #region Plc添加
//            List<Group> Groups = new List<Group>();
//            Groups.Add(new Group("大包装电芯校验A.csv", plc));
//            Groups.Add(new Group("大包装电芯校验B.csv", plc));
//            Groups.Add(new Group("电芯校验.csv", plc));
//            Groups.Add(new Group("电芯校验NG.csv", plc));
//            Groups.Add(new Group("电芯配方下发.csv", plc));
//            Groups.Add(new Group("大面贴胶NG替换.csv", plc));
///*            Groups.Add(new Group("转线配方下发A.csv", plc));
//            Groups.Add(new Group("转线配方下发B.csv", plc));
//            Groups.Add(new Group("申请模组码A.csv", plc));
//            Groups.Add(new Group("申请模组码B.csv", plc));
//            Groups.Add(new Group("模组电芯校验装配.csv", plc));
//            Groups.Add(new Group("模组出站.csv", plc));*/
//            PlcCommucation PlcComm = new PlcCommucation(Groups, plc);
//            BussnessCore = new BussnessCore(PlcComm);
//            #endregion
        }
    }

    public class MenuItem
    {
        public string Icon { get; set; }
        public string Title { get; set; }
        public string NavigationPath { get; set; }
    }
}