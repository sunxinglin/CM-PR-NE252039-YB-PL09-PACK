using AsZero.Core.Services.Messages;
using AsZero.DbContexts;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Ctp0600P.Client.ViewModels;
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
