using System.Windows;

namespace Ctp0600P.Client.Views.StationTaskPages
{
    /// <summary>
    /// BoltGun.xaml 的交互逻辑
    /// </summary>
    public partial class OFFAutoBolt : Window
    {
        public OFFAutoBolt()
        {
            InitializeComponent();
        }

        private void Button_Click_Submit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_Cancle(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
