using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Automatic.Client.ViewModels
{
    public class MainViewModel : ReactiveObject
    {
        public MainViewModel(AppViewModel appViewModel)
        {
            AppViewModel = appViewModel;
        }

        public AppViewModel AppViewModel { get; }

    }
}
