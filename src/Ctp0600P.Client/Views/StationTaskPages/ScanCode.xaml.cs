using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

using Ctp0600P.Client.ViewModels.StationTaskViewModels;

namespace Ctp0600P.Client.Views.StationTaskPages
{
    /// <summary>
    /// ScanCode.xaml 的交互逻辑
    /// </summary>
    public partial class ScanCode : StationTaskCommonPage
    {
        public readonly ScanCodeViewModel _VM;
        private DateTime LastClickPageTIme = DateTime.Now;

        public ScanCode(ScanCodeViewModel vm)
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