using Ctp0600P.Client.CommonEntity;
using Ctp0600P.Client.DTOS;
using Ctp0600P.Client.ViewModels;
using Ctp0600P.Client.ViewModels.StationTaskViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Yee.Common.Library;
using Yee.Entitys.Production;

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
