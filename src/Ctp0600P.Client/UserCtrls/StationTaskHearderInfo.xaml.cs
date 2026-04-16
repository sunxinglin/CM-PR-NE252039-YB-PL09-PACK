using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Ctp0600P.Client.UserCtrls
{
    /// <summary>
    /// Info.xaml 的交互逻辑
    /// </summary>
    public partial class StationTaskHearderInfo : UserControl
    {
        public StationTaskHearderInfo()
        {
            InitializeComponent();
        }
    }
    public class StationTaskHearderInfoVm : INotifyPropertyChanged
    {
        public string Info
        {
            get => _Info;
            set
            {
                if (_Info != value)
                {
                    _Info = value;
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Info)));
                }
            }
        }
        private string _Info;
        public Brush BackBrush
        {
            get => _BackBrush;
            set
            {
                if (_BackBrush != value)
                {
                    _BackBrush = value;
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BackBrush)));
                }
            }
        }
        private Brush _BackBrush;

        public event PropertyChangedEventHandler? PropertyChanged;

        //public StationTaskHearderInfoVm(StationTaskViewDTO info, Brush brush, bool arrowVisi)
        //{
        //    this.Info = info.Header;
        //    this.BackBrush = brush;
        //    if (!arrowVisi)
        //    {
        //        this.ArrowVisi = Visibility.Hidden;
        //    }

        //}
        public Visibility ArrowVisi { get; }
    }
}
