using System.Windows;

using Ctp0600P.Client.Protocols;
using Ctp0600P.Client.ViewModels.StationTaskViewModels;

namespace Ctp0600P.Client.Views.StationTaskPages
{
    /// <summary>
    /// AnyLoad.xaml 的交互逻辑
    /// </summary>
    public partial class AnyLoad : StationTaskCommonPage
    {
        public AnyLoadViewModel _VM;
        public AnyLoad(AnyLoadViewModel vm)
        {
            InitializeComponent();
            _VM = vm;
            this.DataContext = _VM;
        }

        private void Btn_Test(object sender, RoutedEventArgs e)
        {
            _VM.Test();
        }

        private void Btn_Test2(object sender, RoutedEventArgs e)
        {
            _VM.CatchAnyLoadMessage(new AnyLoadRequest
            {
               
            });
        }
    }
}
