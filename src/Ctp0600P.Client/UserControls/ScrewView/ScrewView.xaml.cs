using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Ctp0600P.Client.UserControls.ScrewView
{
    /// <summary>
    /// ScrewView.xaml 的交互逻辑
    /// </summary>
    public partial class ScrewView : UserControl
    {
        public ScrewView()
        {
            InitializeComponent();
        }

        private static void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {

            var screwView = (ScrewView)sender;
            if (e.NewValue == null)
            {
                screwView.bkImgOK.Visibility = Visibility.Hidden;
                screwView.bkImgNG.Visibility = Visibility.Hidden;
                screwView.bkImgDoing.Visibility = Visibility.Hidden;
            }
            else
            {
                screwView.bkImgOK.Visibility = e.NewValue.ToString() == "OK" ? Visibility.Visible : Visibility.Hidden;
                screwView.bkImgUndo.Visibility = e.NewValue.ToString() == "Undo" ? Visibility.Visible : Visibility.Hidden;
                screwView.bkImgNG.Visibility = e.NewValue.ToString() == "NG" ? Visibility.Visible : Visibility.Hidden;
                screwView.bkImgDoing.Visibility = e.NewValue.ToString() == "Doing" ? Visibility.Visible : Visibility.Hidden;
                screwView.bkImgWait.Visibility = e.NewValue.ToString() == "Wait" ? Visibility.Visible : Visibility.Hidden;
                if (screwView.Status == "OK") screwView.txb_OrderNo.Foreground = Brushes.White;
                if (screwView.Status == "NG") screwView.txb_OrderNo.Foreground = Brushes.Black;
            }
        }

        public static readonly DependencyProperty StatusProperty = DependencyProperty.Register("Status", typeof(string), typeof(ScrewView), new PropertyMetadata("", OnValueChanged));

        public string Status
        {
            get { return (string)this.GetValue(StatusProperty); }
            set
            {
                this.SetValue(StatusProperty, value);
            }
        }

        public static readonly DependencyProperty OrderNoProperty = DependencyProperty.Register("OrderNo", typeof(string), typeof(ScrewView), new PropertyMetadata("", OnOrderNoValueChanged));

        public string OrderNo
        {
            get { return (string)this.GetValue(OrderNoProperty); }
            set
            {
                this.SetValue(OrderNoProperty, value);
            }
        }

        private static void OnOrderNoValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var screwView = (ScrewView)sender;
            screwView.txb_OrderNo.Text = e.NewValue.ToString();
        }

        public double TextScaleX
        {
            get { return (double)GetValue(TextScaleXProperty); }
            set { SetValue(TextScaleXProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextScaleX.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextScaleXProperty =
            DependencyProperty.Register("TextScaleX", typeof(double), typeof(ScrewView), new PropertyMetadata(1.0));

        public double TextScaleY
        {
            get { return (double)GetValue(TextScaleYProperty); }
            set { SetValue(TextScaleYProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextScaleY.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextScaleYProperty =
            DependencyProperty.Register("TextScaleY", typeof(double), typeof(ScrewView), new PropertyMetadata(1.0));
    }
}
