using MPAssmebleRecipe.Apps.ViewModels;
using Newtonsoft.Json;
using Prism.Events;
using RogerTech.Common;
using RogerTech.Common.AuthService;
using RogerTech.Common.AuthService.Model;
using RogerTech.Common.AuthService.Services;
using RogerTech.Tool;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
    /// PlcTagView.xaml 的交互逻辑
    /// </summary>
    public partial class PlcTagView : UserControl
    {
        bool isreadonly = false;
        private PlcTagViewModel _viewModel;

        public PlcTagView(IEventAggregator eventAggregator)
        {
            InitializeComponent();
            this.Loaded += PlcTagView_Loaded;
            this.Unloaded += PlcTagView_Unloaded;
            EventAggregator = eventAggregator;
            AppManager appManager = AppManager.GetInstance();
            ChangUser(appManager.UserInfo);
            EventAggregator.GetEvent<UserInfoEvent>().Subscribe(user =>
            {
                ChangUser(user);
            });
            RegistMenus();
        }
        private void RegistMenus()
        {
            List<Menu> menuList = new List<Menu>()
            {
                new Menu() { Page = "UserView", SubPage = "",ElementName="AddTag" },
                new Menu() { Page = "UserView", SubPage = "",ElementName="EditTag" },
                new Menu(){ Page = "UserView", SubPage = "",ElementName="SaveTag" },
                new Menu(){ Page = "UserView", SubPage = "",ElementName="EditAllTag" }



            };

            // List<Menu> menuList = DbContext.GetInstance().Queryable<Menu>().Where(p => p.Page == "UserView" && p.SubPage == "B").ToList();
            RogerTech.AuthService.AuthService authService = new RogerTech.AuthService.AuthService();
            foreach (var item in menuList)
            {
                authService.AddMenu(item);
            }

        }
        private ObservableCollection<Tag> _tagsSnapshot;
        private void PlcTagView_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel = DataContext as PlcTagViewModel;
        }
        private void PlcTagView_Unloaded(object sender, RoutedEventArgs e)
        {

            _viewModel.SaveTagCommand.Execute();

        }
        private void EditAllTag_Click(object sender, EventArgs e)
        {
            isreadonly = !isreadonly;
            if (isreadonly)
            {
                EditAllTag.Background = new SolidColorBrush(Color.FromRgb(63, 81, 181));
                _viewModel.SaveTagCommand.Execute();
            }
            else
            {
                EditAllTag.Background = new SolidColorBrush(Colors.Green);


            }
            SetColumnsReadOnly(isreadonly);

        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (_viewModel != null && e.NewValue is Group group)
            {
                // 保存当前修改
                if (!isreadonly)
                {
                    _viewModel.SaveTagCommand.Execute();
                }
                _viewModel.SelectedGroup = group;
            }
            isreadonly = true;
            EditAllTag.Background = new SolidColorBrush(Color.FromRgb(63, 81, 181));
            SetColumnsReadOnly(isreadonly);
        }
        private void SetColumnsReadOnly(bool isReadOnly)
        {
            MesNameColumn.IsReadOnly = isreadonly;
            MesTypeColumn.IsReadOnly = isreadonly;
            LowerLimitColumn.IsReadOnly = isreadonly;
            UpperLimitColumn.IsReadOnly = isreadonly;
            IsCheckedColumn.IsReadOnly = isreadonly;
            IsUploadColumn.IsReadOnly = isreadonly;
            Editing.Visibility = isReadOnly ? Visibility.Hidden : Visibility.Visible;
        }

        public IEventAggregator EventAggregator { get; }

        private void ChangUser(UserInfo user)
        {
            if (user == null)
            {
                AddTag.IsEnabled = false;
                EditTag.IsEnabled = false;
                return;
            }
            var addUser = user.UserMenus.Where(x => x.Page == "PlcTagView" && x.ElementName == "AddTag").FirstOrDefault();
            if (addUser != null)
            {
                AddTag.IsEnabled = true;
            }
            else
            {
                 AddTag.IsEnabled = false;
            }
            addUser = user.UserMenus.Where(x => x.Page == "PlcTagView" && x.ElementName == "EditTag").FirstOrDefault();
            if (addUser != null)
            {
                EditTag.IsEnabled = true;
            }
            else
            {
               EditTag.IsEnabled = false;
            }
            addUser = user.UserMenus.Where(x => x.Page == "PlcTagView" && x.ElementName == "SaveTag").FirstOrDefault();
            if (addUser != null)
            {
                SaveTag.IsEnabled = true;
            }
            else
            {
                SaveTag.IsEnabled = false;
            }
            addUser = user.UserMenus.Where(x => x.Page == "PlcTagView" && x.ElementName == "EditAllTag").FirstOrDefault();
            if (addUser != null)
            {
                EditAllTag.IsEnabled = true;
            }
            else
            {
                EditAllTag.IsEnabled = false;
            }

        }


        private void SaveTag_Click(object sender, RoutedEventArgs e)
        {
            isreadonly = true;
            EditAllTag.Background = new SolidColorBrush(Color.FromRgb(63, 81, 181));
            SetColumnsReadOnly(isreadonly);
        }

           

        private class EditContext
        {
            public string TagName { get; set; }
            public string ColumnName { get; set; }
            public string PropertyName { get; set; }
            public object OldValue { get; set; }
        }

        private EditContext _currentEdit;

        private void tagGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            if (e.Row == null || e.Row.Item == null || e.Column == null) return;

            var tag = e.Row.Item as Tag;
            if (tag == null) return;

            _currentEdit = new EditContext
            {
                TagName = tag.TagName,
                ColumnName = e.Column.Header != null ? e.Column.Header.ToString() : "未知列",
                PropertyName = GetColumnPropertyName(e.Column),
                OldValue = GetCurrentCellValue(e.Row, e.Column)
            };
        }

        private void tagGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (_currentEdit == null || e.Row == null || e.Row.Item == null) return;

            var newValue = GetEditedCellValue(e.EditingElement);
            if (!object.Equals(_currentEdit.OldValue, newValue))
            {
                string log = string.Format("{0}修改[{1}]中的{2}：{3}=>{4}",
                    AppManager.GetInstance().UserInfo?.Name ?? "System",
                    _currentEdit.TagName,
                    _currentEdit.ColumnName,
                    _currentEdit.OldValue ?? "null",
                    newValue ?? "null");

                _viewModel?.FlushLogs(log);
            }
            _currentEdit = null;
        }

        #region 辅助方法（完全兼容.NET 4.8）
        private string GetColumnPropertyName(DataGridColumn column)
        {
            var boundColumn = column as DataGridBoundColumn;
            if (boundColumn != null && boundColumn.Binding is Binding binding)
            {
                return binding.Path.Path;
            }
            return null;
        }

        private object GetCurrentCellValue(DataGridRow row, DataGridColumn column)
        {
            var cellContent = column.GetCellContent(row);
            if (cellContent == null) return null;

            if (cellContent is TextBlock)
                return ((TextBlock)cellContent).Text;
            if (cellContent is CheckBox)
                return ((CheckBox)cellContent).IsChecked;
            if (cellContent is ComboBox)
                return ((ComboBox)cellContent).SelectedItem;
            // 通用处理
            var textProp = cellContent.GetType().GetProperty("Text");
            if (textProp != null)
                return textProp.GetValue(cellContent, null);

            var contentProp = cellContent.GetType().GetProperty("Content");
            return contentProp?.GetValue(cellContent, null);
        }

        private object GetEditedCellValue(FrameworkElement editingElement)
        {
            if (editingElement == null) return null;

            if (editingElement is TextBox)
                return ((TextBox)editingElement).Text;
            if (editingElement is CheckBox)
                return ((CheckBox)editingElement).IsChecked;
            if (editingElement is ComboBox)
                return ((ComboBox)editingElement).SelectedItem;

            // 通用处理
            var textProp = editingElement.GetType().GetProperty("Text");
            if (textProp != null)
                return textProp.GetValue(editingElement, null);

            var contentProp = editingElement.GetType().GetProperty("Content");
            return contentProp?.GetValue(editingElement, null);
        }
        #endregion


    }
}
