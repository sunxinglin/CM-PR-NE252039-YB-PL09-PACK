using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Xml.Linq;
using CatlMesBase;
using MPAssmebleRecipe.Apps.ViewModels;
using Prism.Events;
using RogerTech.BussnessCore;
using RogerTech.Common.AuthService;
using RogerTech.Common.Models;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
namespace MPAssmebleRecipe.Apps.Views
{
    /// <summary>
    /// LogView.xaml 的交互逻辑
    /// </summary>
    public partial class MESCFGView : UserControl
    {
        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            // 注册当前页面的所有按钮
            PermissionController.RegisterPageControls(this);
            AppManager appManager = AppManager.GetInstance();
            PermissionController.UpdatePagePermissions(this, appManager.UserInfo);
        }
        string path = Path.Combine(Directory.GetCurrentDirectory(), "MESCFG.ini");
        BussnessUtility bussness = BussnessUtility.GetInstance();
        public IEventAggregator EventAggregator { get; }
        public MESCFGView(IEventAggregator eventAggregator)
        {
            InitializeComponent();           
            EventAggregator =eventAggregator;
            Loaded += OnPageLoaded;
            EventAggregator.GetEvent<UserInfoEvent>().Subscribe(user =>
            {
                PermissionController.UpdatePagePermissions(this, user);
            });
        }
     
      
        private void SelectionChanged(object sender, EventArgs e)
        {
            NameGrid.Children.Clear();
            NameGrid.ColumnDefinitions.Clear();
            NameGrid.RowDefinitions.Clear();
            string[] result = IniFileHelper.ReadKeyValuePairs(((MenuItem)InterfaceList.SelectedItem).Title.ToString(), path);
            NameGrid.HorizontalAlignment = HorizontalAlignment.Stretch;
            NameGrid.VerticalAlignment = VerticalAlignment.Stretch;
            var NameGridColumns = 2;
            var NameGridRows = Math.Ceiling(result.Count() / 1.0);
            //添加grid列
            for (int i = 0; i < NameGridColumns; i++)
            {
                var nameColumnDefinition = new ColumnDefinition();
                NameGrid.ColumnDefinitions.Add(nameColumnDefinition);
                nameColumnDefinition.Width = GridLength.Auto;
            }
            //添加grid行
            for (int i = 0; i < NameGridRows; i++)
            {
                var successRowDefinition = new RowDefinition();
                NameGrid.RowDefinitions.Add(successRowDefinition);
                successRowDefinition.Height = new GridLength(30, GridUnitType.Pixel);
                //绝对尺寸
            }

            int rowIndex = 0;
            int columnIndex = 0;
            UIElement uIElement = new UIElement();
            //int i = 0;
            foreach (var item in result)
            {
                string name = item.Split('=')[0].ToString();
                string value = item.Split('=')[1].ToString();
                Label label = new Label();
                label.HorizontalAlignment = HorizontalAlignment.Center;
                label.VerticalAlignment = VerticalAlignment.Center;
                label.Width = double.NaN;
                label.Content = name;
                label.FontSize = 14;
                label.SetValue(Grid.RowProperty, rowIndex);
                label.SetValue(Grid.ColumnProperty, columnIndex);
                uIElement = label;
                NameGrid.Children.Add(uIElement);
                if (bussness.propertyDictionary.TryGetValue(name, out Type propertyType))
                {
                    ComboBox combox = new ComboBox();
                    combox.HorizontalAlignment = HorizontalAlignment.Left;
                    combox.VerticalAlignment = VerticalAlignment.Center;
                    combox.Width = double.NaN;
                    combox.FontSize = 14;
                    combox.MinWidth = 100;
                    combox.ItemsSource = Enum.GetValues(propertyType)
                                .Cast<Enum>()
                                .Select(x => x.ToString())
                                .ToList();
                    combox.SelectedValue = value;
                    if (combox.SelectedItem is null)
                    {
                        combox.SelectedIndex = 0;
                    }
                    combox.SetValue(Grid.RowProperty, rowIndex);
                    combox.SetValue(Grid.ColumnProperty, columnIndex + 1);
                    uIElement = combox;
                    NameGrid.Children.Add(uIElement);
                }
                else
                {
                    TextBox textBox = new TextBox();
                    textBox.HorizontalAlignment = HorizontalAlignment.Left;
                    textBox.VerticalAlignment = VerticalAlignment.Center;
                    textBox.MinWidth = 120;
                    textBox.FontSize = 14;
                    textBox.Width = double.NaN;
                    textBox.Text = value;
                    textBox.SetValue(Grid.RowProperty, rowIndex);
                    textBox.SetValue(Grid.ColumnProperty, columnIndex + 1);
                    uIElement = textBox;
                    NameGrid.Children.Add(uIElement);
                }
                rowIndex++;
            }

        }


        private void SaveSetting(object sender, EventArgs e)
        {            
            foreach (UIElement item in this.NameGrid.Children)
            {
                // 只处理输入控件（跳过Label）
                if (!(item is ComboBox || item is TextBox))
                    continue;

                // 通过布局定位关联Label
                int row = (int)item.GetValue(Grid.RowProperty);
                int labelColumn = (int)item.GetValue(Grid.ColumnProperty) - 1;

                Label associatedLabel = NameGrid.Children
                    .OfType<Label>()
                    .FirstOrDefault(l =>
                        (int)l.GetValue(Grid.RowProperty) == row &&
                        (int)l.GetValue(Grid.ColumnProperty) == labelColumn);

                if (associatedLabel == null) continue;

                string name = associatedLabel.Content.ToString();
                string value = null;

                // 获取输入值
                if (item is ComboBox comboBox)
                {
                    value = comboBox.SelectedValue?.ToString() ?? comboBox.Text;
                }
                else if (item is TextBox textBox)
                {
                    value = textBox.Text;
                }

                // 写入INI
                if (!string.IsNullOrEmpty(value))
                {
                    IniFileHelper.WriteValue(
                        ((MenuItem)InterfaceList.SelectedItem).Title.ToString(),
                        name,
                        value,
                        Path.Combine(Directory.GetCurrentDirectory(), "MESCFG.ini")
                    );
                }
            }
            MessageBox.Show("保存成功！");
        }


    }
}
