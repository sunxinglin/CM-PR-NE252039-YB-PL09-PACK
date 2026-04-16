using System.Windows;
using System.Windows.Controls;

using Ctp0600P.Client.ViewModels;

namespace Ctp0600P.Client.Views.Pages
{
    /// <summary>
    /// Interaction logic for HistoryPage.xaml
    /// </summary>
    public partial class HistoryPage : Page
    {
        private HistoryPageViewModel _VM;

        public HistoryPage(HistoryPageViewModel vm)
        {
            this._VM = vm;
            this.DataContext = _VM;
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _VM.InitSearchFilter();
            _VM.SearchAsync();
        }
    }
}
