using System.Windows;

using Ctp0600P.Client.ViewModels;

namespace Ctp0600P.Client.Views.PopupPages
{
    /// <summary>
    /// ReTightenNoConfirmPage.xaml 的交互逻辑
    /// </summary>
    public partial class ReTightenNoConfirmPage : Window
    {
        private readonly ReTightenNoConfirmViewModel _vm;

        public ReTightenNoConfirmPage(ReTightenNoConfirmViewModel vm)
        {
            InitializeComponent();
            _vm = vm;
            this.DataContext = _vm;

            Topmost = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Topmost = true;
        }
    }
}
