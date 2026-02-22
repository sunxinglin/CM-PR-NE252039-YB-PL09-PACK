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
using System.Windows.Shapes;

namespace Ctp0600P.Client.Views.Windows
{
    /// <summary>
    /// InputWindow.xaml 的交互逻辑
    /// </summary>
    public partial class InputWindow : Window
    {
        public InputWindow()
        {
            InitializeComponent();
        }

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    DialogResult = false;
        //}

        //private void Button_Click_1(object sender, RoutedEventArgs e)
        //{
        //    DialogResult = true;
        //}
        public string GetInputValue()
        {
            return this.inputBox.Text;
        }

        private void inputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                DialogResult = true;
            }

            if(e.Key == Key.Escape)
            {
                DialogResult = false;
            }
        }
    }
}
