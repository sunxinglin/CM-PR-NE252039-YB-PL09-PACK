using Ctp0600P.Client.ViewModels;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

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
