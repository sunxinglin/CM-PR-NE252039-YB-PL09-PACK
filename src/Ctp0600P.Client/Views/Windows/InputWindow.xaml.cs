using System.Windows;
using System.Windows.Input;

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
