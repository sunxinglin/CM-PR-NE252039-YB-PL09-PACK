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

namespace Automatic.Client.Common
{
    /// <summary>
    /// Interaction logic for LedSwitchCtrl.xaml
    /// </summary>
    public partial class LedSwitchCtrl : UserControl
    {
        public LedSwitchCtrl()
        {
            InitializeComponent();
        }

        #region Tip
        public string Tip
        {
            get { return (string)GetValue(TipProperty); }
            set { SetValue(TipProperty, value); }
        }
        public static readonly DependencyProperty TipProperty = DependencyProperty.Register("Tip", typeof(string), typeof(LedSwitchCtrl), new PropertyMetadata(default));
        #endregion

        #region IsChecked
        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register(nameof(IsChecked), typeof(bool), typeof(LedSwitchCtrl), new PropertyMetadata(false));
        #endregion


        #region On
        public Brush OnColor
        {
            get { return (Brush)GetValue(OnColorProperty); }
            set { SetValue(OnColorProperty, value); }
        }
        public static readonly DependencyProperty OnColorProperty = DependencyProperty.Register("OnColor", typeof(Brush), typeof(LedSwitchCtrl), new PropertyMetadata(Brushes.Green));
        #endregion


        #region Off
        public Brush OffColor
        {
            get { return (Brush)GetValue(OffColorProperty); }
            set { SetValue(OffColorProperty, value); }
        }
        public static readonly DependencyProperty OffColorProperty = DependencyProperty.Register("OffColor", typeof(Brush), typeof(LedSwitchCtrl), new PropertyMetadata(Brushes.Red));
        #endregion
    }
}
