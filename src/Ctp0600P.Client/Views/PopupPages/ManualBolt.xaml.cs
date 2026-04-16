using System.Windows;

using Ctp0600P.Client.ViewModels.StationTaskViewModels;

namespace Ctp0600P.Client.Views.StationTaskPages
{
    /// <summary>
    /// 手动设置拧紧枪
    /// </summary>
    public partial class ManualBolt : Window
    {
        public readonly ManualBoltPageVM _vm;

        public ManualBolt(ManualBoltPageVM vm)
        {
            this._vm = vm;
            this.DataContext = _vm;
            InitializeComponent();
        }

        private void Button_Click_Submit(object sender, RoutedEventArgs e)
        {
            _vm.EnableBoltGuns();
        }

        
    }
}
