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

namespace MPAssmebleRecipe.Apps.ViewModels
{
    public class LogViewModel : BindableBase, IRegionMemberLifetime
    {
        private readonly IDialogService _dialogService;
        #region 属性

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
                    LoadLogs();
                    LoadLogsByType(value.Title);
                }
            }
        }

        private bool _isTimeFilterEnabled;
        public bool IsTimeFilterEnabled
        {
            get { return _isTimeFilterEnabled; }
            set { SetProperty(ref _isTimeFilterEnabled, value); }
        }

        private DateTime? _startDate;
        public DateTime? StartDate
        {
            get { return _startDate; }
            set { SetProperty(ref _startDate, value); }
        }

        private TimeSpan? _startTime;
        public TimeSpan? StartTime
        {
            get { return _startTime; }
            set { SetProperty(ref _startTime, value); }
        }

        private DateTime? _endDate;
        public DateTime? EndDate
        {
            get { return _endDate; }
            set { SetProperty(ref _endDate, value); }
        }

        private TimeSpan? _endTime;
        public TimeSpan? EndTime
        {
            get { return _endTime; }
            set { SetProperty(ref _endTime, value); }
        }

        private ObservableCollection<LogModel> _allLogs;
        public ObservableCollection<LogModel> AllLogs
        {
            get { return _allLogs; }
            set { SetProperty(ref _allLogs, value); }
        }

        private ObservableCollection<LogModel> _filteredLogs;
        public ObservableCollection<LogModel> FilteredLogs
        {
            get { return _filteredLogs; }
            set { SetProperty(ref _filteredLogs, value); }
        }

        #endregion

        #region 命令

        public ICommand ApplyFilterCommand { get; private set; }
        public ICommand RefreshCommand { get; private set; }

        public bool KeepAlive => true;

        #endregion

        public LogViewModel(IDialogService dialogService)
        {
            InitializeCommands();
            InitializeMenuItems();
            LoadLogs();
            InitializeDefaultValues();
            _dialogService = dialogService;
            DbContext.ProgressInfoChange += new Action(() =>
            {
                LoadLogs();
            });
        }

        private void InitializeCommands()
        {
            ApplyFilterCommand = new DelegateCommand(ApplyFilter);
            RefreshCommand = new DelegateCommand(RefreshLogs);
        }

        private void InitializeMenuItems()
        {
            MenuItems = new ObservableCollection<MenuItem>
            {
                new MenuItem { Title = "全部日志", Icon = "ViewList" },
            };
            AppManager appManager = AppManager.GetInstance();
            foreach (var item in appManager.bussness.BussnessDic.PlcGroups)
            {
                MenuItem menuItem = new MenuItem { Title = item.GroupName, Icon = "API" };
                MenuItems.Add(menuItem);
            }
            SelectedMenuItem = MenuItems.FirstOrDefault();
        }

        private void InitializeDefaultValues()
        {
            IsTimeFilterEnabled = false;
            StartDate = DateTime.Today;
            StartTime = new TimeSpan(0, 0, 0);
            EndDate = DateTime.Today;
            EndTime = new TimeSpan(23, 59, 59);
        }

        private void LoadLogs()
        {
            // 这里应该从数据库加载日志数据
            // 示例数据
            AllLogs = new ObservableCollection<LogModel>();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            // 添加一些示例数据,全部日志过滤；
            List<LogModel> logs = null;
            if (SelectedMenuItem == null || SelectedMenuItem?.Title == "全部日志")
            {
                List<string> menuItemList = new List<string>();
                foreach (var menu in MenuItems)
                {
                    menuItemList.Add(menu.Title);
                }
                logs = DbContext.GetInstance().Queryable<LogModel>().Where(x => menuItemList.Contains(x.InterfaceName)).OrderByDescending(x => x.RecordTime).Take(200).ToList();
            }
            else
            {
                logs = DbContext.GetInstance().Queryable<LogModel>().Where(x => SelectedMenuItem.Title.Equals(x.InterfaceName)).OrderByDescending(x => x.RecordTime).Take(200).ToList();
            }
            
            AllLogs = new ObservableCollection<LogModel>(logs);
            Console.WriteLine(sw.ElapsedMilliseconds);
            FilteredLogs = new ObservableCollection<LogModel>(AllLogs);
        }

        private void LoadLogsByType(string logType)
        {
            if (string.IsNullOrEmpty(logType) || logType == "全部日志")
            {
                FilteredLogs = new ObservableCollection<LogModel>(AllLogs);
                return;
            }

            string type = logType;
            var filtered = AllLogs.Where(x => x.InterfaceName == type).ToList();
            if (filtered.Count < 200)
            {
                filtered = DbContext.GetInstance().Queryable<LogModel>().OrderByDescending(x => x.RecordTime).Where(x => x.InterfaceName == type).Take(200).ToList();
            }

            FilteredLogs = new ObservableCollection<LogModel>(filtered);
        }

        private void ApplyFilter()
        {
            if (!IsTimeFilterEnabled)
            {
                LoadLogsByType(SelectedMenuItem?.Title);
                return;
            }

            DateTime startDateTime = StartDate.GetValueOrDefault(DateTime.Today);
            if (StartTime.HasValue)
            {
                startDateTime = startDateTime.Date + StartTime.Value;
            }

            DateTime endDateTime = EndDate.GetValueOrDefault(DateTime.Today);
            if (EndTime.HasValue)
            {
                endDateTime = endDateTime.Date + EndTime.Value;
            }

            var filtered = AllLogs.Where(log =>
                log.RecordTime >= startDateTime &&
                log.RecordTime <= endDateTime
            ).ToList();
            FilteredLogs = new ObservableCollection<LogModel>(filtered);
        }

        private void RefreshLogs()
        {
            // 在实际应用中，这里应该重新从数据库加载日志
            LoadLogs();
            ApplyFilter();
        }

        public void ShowCodeDetail(int code)
        {
            var parameters = new DialogParameters { { "logEntry", code } };
            _dialogService.ShowDialog("CodeDetailView", parameters, _ => { });
        }
    }
}
