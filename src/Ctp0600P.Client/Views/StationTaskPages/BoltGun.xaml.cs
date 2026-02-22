using Ctp0600P.Client.Protocols.BoltGun.Models;
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

namespace Ctp0600P.Client.Views.StationTaskPages
{
    /// <summary>
    /// BoltGun.xaml 的交互逻辑
    /// </summary>
    public partial class BoltGun : StationTaskCommonPage
    {
        public BoltGunViewModel _VM;
        public BoltGun(BoltGunViewModel vm)
        {
            InitializeComponent();
            _VM = vm;
            this.DataContext = _VM;
        }

        private void Btn_Test(object sender, RoutedEventArgs e)
        {
            _VM.CatchBoltGunMessage(new BoltGunRequest
            {
                DeviceNo = "1",
                BoltGunValue = "2.0 N",
                BoltGunState = "NG",
                ResultIsOK=false,
                IpAddress = "192.168.10.102",
                ProgramNo =1
            });
        }

        private void Btn_Test2(object sender, RoutedEventArgs e)
        {
            _VM.CatchBoltGunMessage(new BoltGunRequest
            {
                DeviceNo = "2",
                BoltGunValue = "10 N",
                BoltGunState = "OK",
                IpAddress = "192.168.130.102",
                ProgramNo = 10
            });
        }

        private void TextBlock_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            int programNo = int.Parse(((TextBlock)sender).Tag.ToString());
            var screw = _VM.StationTaskScrewList.FirstOrDefault(s => s.ProgramNo == programNo);

            PopupAllClose();
            if (screw.ScrewResultList.Count > 0 && !screw.ShowPop)
                screw.ShowPop = true;

            LastClickPageTIme = DateTime.Now;
        }

        private void ListBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PopupAllClose();
        }

        private void PopupAllClose()
        {
            if ((DateTime.Now - LastClickPageTIme).TotalMilliseconds < 100) return;
            if (_VM.StationTaskScrewList != null && _VM.StationTaskScrewList.Count > 0)
                foreach (var screw in _VM.StationTaskScrewList)
                {
                    screw.ShowPop = false;
                }
        }

        private DateTime LastClickPageTIme = DateTime.Now;

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PopupAllClose();
        }



    }
}
