using System.Windows;
using System.Windows.Controls;

using Ctp0600P.Client.ViewModels;

namespace Ctp0600P.Client.Views.Pages
{
    /// <summary>
    /// Interaction logic for DebugToolsPage.xaml
    /// </summary>
    public partial class DebugToolsPage : Page
    {
        private readonly DebugToolsPageViewModel _debugvm;

        public DebugToolsPage(DebugToolsPageViewModel debugvm)
        {
            InitializeComponent();
            this.DataContext = debugvm;
            this._debugvm = debugvm;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            App.DebugToolsPageVm = _debugvm;
        }
    }
}
