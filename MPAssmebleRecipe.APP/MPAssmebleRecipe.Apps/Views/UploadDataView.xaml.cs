using MPAssmebleRecipe.Apps.ViewModels;
using Prism.Events;
using RogerTech.AuthService.Models;
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
    /// 
 
    public partial class UploadDataView : UserControl
    {
        private UploadDataViewModel _ViewModel;
        public IEventAggregator EventAggregator { get; }
        public UploadDataView(IEventAggregator eventAggregator)
        {
            InitializeComponent();           
            EventAggregator = eventAggregator;
            this.Loaded += UploadDataView_Loaded;
            EventAggregator.GetEvent<UserInfoEvent>().Subscribe(user =>
            {
                PermissionController.UpdatePagePermissions(this, user);
            });
        }



      

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UploadDataView_Loaded(object sender, RoutedEventArgs e)
        {
            _ViewModel = DataContext as UploadDataViewModel;

            PermissionController.RegisterPageControls(this);
            AppManager appManager = AppManager.GetInstance();
            PermissionController.UpdatePagePermissions(this, appManager.UserInfo);
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
                List<UploadData> lstSelectData = new List<UploadData>();
                foreach (var item in items)
                {
                    if (item is UploadData model)
                        lstSelectData.Add(model);
                }
                _ViewModel.SelectDatas = lstSelectData;
                _ViewModel.UploadCommand.Execute(sender);
            }
            else
            {
                System.Windows.MessageBox.Show("请选择您要上传的行数据！");
            }
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

      
    }
}
