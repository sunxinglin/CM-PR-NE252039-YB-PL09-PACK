using System;
using System.Collections.Generic;
using System.Configuration;
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
using MPAssmebleRecipe.Apps.ViewModels;

namespace MPAssmebleRecipe.Apps.Views
{
    /// <summary>
    /// CardLoginDialog.xaml 的交互逻辑
    /// </summary>
    public partial class CardLoginDialog : UserControl
    {
        CardLoginDialogViewModel vm;

        public CardLoginDialog()
        {
            InitializeComponent();


            Loaded += (s, e) =>
            {
                vm = (CardLoginDialogViewModel)this.DataContext;
                tbx_CardNum.Focus();
            };
            //this.mouse
        }

        private DateTime _lastKeyTime = DateTime.Now;
        private int _fastInputCount = 0;
        private int _keyPressCount = 0;

        private void PreviewKeyDownLogin(object sender, KeyEventArgs e)
        {
            string LoginOrCard = ConfigurationManager.AppSettings["LoginByCard"];
            string CardTime = ConfigurationManager.AppSettings["CardTime"];
            int cardTime = int.Parse(CardTime);
            bool flag = false;
            if (bool.TryParse(LoginOrCard, out flag))
            {

            }

            if (!flag)
            {
                if (vm.CardNumber.Length == 0)
                {
                    _lastKeyTime = DateTime.Now;
                }
                if (e.Key == Key.V && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                {
                    e.Handled = true;
                    return;
                }
                _keyPressCount++;
                var now = DateTime.Now;
                
                var interval = (now - _lastKeyTime).TotalMilliseconds;
                _lastKeyTime = now;
                Console.WriteLine(  interval);
                if (interval < cardTime)
                {
                    _fastInputCount++;
                }
                else
                {
                    _fastInputCount = 0;
                }
                if (_keyPressCount <= 5)
                {
                    // 前5位不拦截
                    e.Handled = false;
                }
                else
                {
                    if (_fastInputCount > 5)
                    {
                        // 正常刷卡
                        e.Handled = false;
                    }
                    else
                    {
                        //人工输入
                        e.Handled = true;
                    }
                }
            }

            if (e.Key == Key.Enter)
            {
                vm.OnCardRead(vm.CardNumber);

                Console.WriteLine(vm.CardNumber);
            }
        }
    }
}
