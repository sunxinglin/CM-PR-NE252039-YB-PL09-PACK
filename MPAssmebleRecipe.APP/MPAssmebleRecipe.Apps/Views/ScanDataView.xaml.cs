using MPAssmebleRecipe.Apps.ViewModels;
using Prism.Events;
using RogerTech.Common.AuthService;
using RogerTech.Common.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
namespace MPAssmebleRecipe.Apps.Views
{
    /// <summary>
    /// LogView.xaml 的交互逻辑
    /// </summary>
    public partial class ScanDataView : UserControl
    {
        private ScanDataViewModel _ViewModel;
        public IEventAggregator EventAggregator { get; }
        public ScanDataView(IEventAggregator eventAggregator)
        {
            InitializeComponent();

            this.Loaded += ScanDataView_Loaded;
            UpLoadBtn.Click += UpLoadBtn_Click;
            UnBindBtn.Click += UnBindBtn_Click;
            ReplaceMateBtn.Click += ReplaceMateBtn_Click;

            EventAggregator = eventAggregator;
            AppManager appManager = AppManager.GetInstance();
            ChangUser(appManager.UserInfo);
            EventAggregator.GetEvent<UserInfoEvent>().Subscribe(user =>
            {
                ChangUser(user);
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReplaceMateBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_ViewModel == null) return;

            var items = dataGrid.SelectedItems;
            if (items != null && items.Count > 0)
            {
                List<TbAssembleTempScan> lstSelectData = new List<TbAssembleTempScan>();
                foreach (var item in items)
                {
                    if (item is TbAssembleTempScan model)
                        lstSelectData.Add(model);
                }
                _ViewModel.SelectDatas = lstSelectData;
                _ViewModel.ReplaceCommand.Execute(sender);
            }
            else
            {
                System.Windows.MessageBox.Show("请选择要更换的行数据！");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UnBindBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_ViewModel == null) return;

            var items = dataGrid.SelectedItems;
            if (items != null && items.Count > 0)
            {
                List<TbAssembleTempScan> lstSelectData = new List<TbAssembleTempScan>();
                foreach (var item in items)
                {
                    if (item is TbAssembleTempScan model)
                        lstSelectData.Add(model);
                }
                _ViewModel.SelectDatas = lstSelectData;
                _ViewModel.UnBindCommand.Execute(sender);
            }
            else
            {
                System.Windows.MessageBox.Show("请选择要解绑的行数据！");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpLoadBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_ViewModel == null) return;

            var items = dataGrid.SelectedItems;
            if (items != null && items.Count > 0)
            {
                List<TbAssembleTempScan> lstSelectData = new List<TbAssembleTempScan>();
                foreach (var item in items)
                {
                    if (item is TbAssembleTempScan model)
                        lstSelectData.Add(model);
                }
                _ViewModel.SelectDatas = lstSelectData;
                _ViewModel.UploadCommand.Execute(sender);
            }
            else
            {
                System.Windows.MessageBox.Show("请选择要上传的行数据！");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScanDataView_Loaded(object sender, RoutedEventArgs e)
        {
            _ViewModel = DataContext as ScanDataViewModel;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var dataGrid = sender as DataGrid;
            if (dataGrid == null) return;

            // 确保点击的对象是 DataGridCell
            var source = e.OriginalSource as DependencyObject;
            while (source != null && !(source is DataGridCell))
            {
                source = VisualTreeHelper.GetParent(source);
            }

            // 如果点击的不是 DataGridCell，则直接返回
            if (source == null) return;

            var selectedLogEntry = dataGrid.SelectedItem as LogModel;
            if (selectedLogEntry == null) return;

            var viewModel = DataContext as LogViewModel;
            if (viewModel == null) return;
            if (selectedLogEntry.Result == null) return;
            int code = -1;
            if (!int.TryParse(selectedLogEntry.Result, out code)) return;

            if (code < 100) return;
            viewModel.ShowCodeDetail(code);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        private void ChangUser(UserInfo user)
        {
            if (user == null)
            {
                UpLoadBtn.IsEnabled = false;
                UnBindBtn.IsEnabled = false;
                ReplaceMateBtn.IsEnabled = false;
                return;
            }
            var addUser = user.UserMenus.Where(x => x.Page == "ScanDataView" && x.ElementName == "重新上传").FirstOrDefault();
            if (addUser != null)
            {
                UpLoadBtn.IsEnabled = true;
            }
            else
            {
                UpLoadBtn.IsEnabled = false;
            }
            addUser = user.UserMenus.Where(x => x.Page == "ScanDataView" && x.ElementName == "物料解绑").FirstOrDefault();
            if (addUser != null)
            {
                UnBindBtn.IsEnabled = true;
            }
            else
            {
                UnBindBtn.IsEnabled = false;
            }
            addUser = user.UserMenus.Where(x => x.Page == "ScanDataView" && x.ElementName == "物料更换").FirstOrDefault();
            if (addUser != null)
            {
                ReplaceMateBtn.IsEnabled = true;
            }
            else
            {
                ReplaceMateBtn.IsEnabled = false;
            }
        }
    }
}
