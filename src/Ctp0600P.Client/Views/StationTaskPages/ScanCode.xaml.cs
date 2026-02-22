using Ctp0600P.Client.CommonEntity;
using Ctp0600P.Client.DTOS;
using Ctp0600P.Client.Protocols.ScanCode.Models;
using Ctp0600P.Client.ViewModels.StationTaskViewModels;
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
using Yee.Common.Library;
using Yee.Entitys.DBEntity;
using Yee.Entitys.DTOS;
using Yee.Entitys.Production;

namespace Ctp0600P.Client.Views.StationTaskPages
{
    /// <summary>
    /// ScanCode.xaml 的交互逻辑
    /// </summary>
    public partial class ScanCode : StationTaskCommonPage
    {
        public readonly ScanCodeViewModel _VM;
        private DateTime LastClickPageTIme = DateTime.Now;

        public ScanCode(ScanCodeViewModel vm) : base()
        {
            InitializeComponent();
            _VM = vm;
            this.DataContext = _VM;
        }

        private void TextBlock_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string goodsPN =((TextBlock)sender).Tag.ToString();
            var goods = _VM.StationTaskBomList.FirstOrDefault(s => s.GoodsPN == goodsPN);

            PopupAllClose();
            if (goods.ScanBarCodeList.Count > 0 && !goods.ShowPop)
                goods.ShowPop = true;

            LastClickPageTIme = DateTime.Now;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PopupAllClose();
        }

        private void ListBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PopupAllClose();
        }

        private void PopupAllClose()
        {
            if ((DateTime.Now - LastClickPageTIme).TotalMilliseconds < 100) return;
            if (_VM.StationTaskBomList != null && _VM.StationTaskBomList.Count > 0)
                foreach (var bom in _VM.StationTaskBomList)
                {
                    bom.ShowPop = false;
                }
        }
    }
}