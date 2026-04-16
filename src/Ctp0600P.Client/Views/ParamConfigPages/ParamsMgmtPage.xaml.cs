using System;
using System.Windows.Controls;

using Ctp0600P.Client.ViewModels;

namespace Ctp0600P.Client.Views.Pages
{
    /// <summary>
    /// Interaction logic for UseMgmtPage.xaml
    /// </summary>
    public partial class ParamsMgmtPage : Page
    {
        private readonly ParamsMgmtPageViewModel _vm;
        private readonly IServiceProvider _sp;
        private readonly AppViewModel _appvm;

        public ParamsMgmtPage(ParamsMgmtPageViewModel vm, IServiceProvider sp, AppViewModel appvm)
        {
            InitializeComponent();
            this.DataContext = vm;
            this._vm = vm;
            this._sp = sp;
            this._appvm = appvm;
        }


    }
}
