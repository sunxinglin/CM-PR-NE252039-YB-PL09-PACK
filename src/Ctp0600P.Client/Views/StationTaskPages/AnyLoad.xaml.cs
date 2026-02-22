
using Ctp0600P.Client.ViewModels.StationTaskViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Shapes;
using Yee.Entitys.DBEntity;
using Yee.Entitys.DTOS;
using Yee.Entitys.Production;
using Ctp0600P.Client.Protocols;
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
