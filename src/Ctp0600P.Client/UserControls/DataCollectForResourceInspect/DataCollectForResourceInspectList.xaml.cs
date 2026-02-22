using FutureTech.Mvvm;
using Microsoft.Extensions.DependencyInjection;
using Reactive.Bindings;
using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace Ctp0600P.Client.UserControls.DataCollectForResourceInspect
{
    /// <summary>
    /// DataCollectForResourceInspectList.xaml 的交互逻辑
    /// </summary>
    public partial class DataCollectForResourceInspectList : UserControl
    {
        public DataCollectForResourceInspectList()
        {
            InitializeComponent();
        }
       
    }
    public class DataCollectForResourceInspectListViewModle : ViewModelBase
    {
        private readonly IServiceProvider _serviceprovider;

        public DataCollectForResourceInspectListViewModle(IServiceProvider serviceProvider)
        {
            _serviceprovider = serviceProvider;
            DataCollectForResourceInspectStations = new ObservableCollection<DataCollectForResourceInspectStation>()
            {
                new DataCollectForResourceInspectStation()
                {
                    Name="OP120"
                },
                new DataCollectForResourceInspectStation()
                {
                    Name="OP170"
                },
                new DataCollectForResourceInspectStation()
                {
                    Name="OP240"
                }
            };
            this.ChangeDataCommand = new AsyncRelayCommand<object>(async s =>
            {
                var window = _serviceprovider.GetService<DataCollectForResourceInspectWindow>();
                var viewmodle = _serviceprovider.GetService<DataCollectForResourceInspectWindowViewModle>();
                viewmodle.DataCollectForResourceInspectVM.AutoOpName = SelectData.Name;
                window.DataContext = viewmodle;
                window.ShowDialog();
            },
                o => true);
        }
        public ICommand ChangeDataCommand { get; }
        public ObservableCollection<DataCollectForResourceInspectStation> DataCollectForResourceInspectStations { get; set; }

        private DataCollectForResourceInspectStation _selectdata;
        public DataCollectForResourceInspectStation SelectData
        {
            set {
                if (_selectdata!=value)
                {
                    _selectdata = value;
                    OnPropertyChanged(nameof(SelectData));
                }
            }
            get { 
                
                return _selectdata;
            }
        }

        public class DataCollectForResourceInspectStation
        {
            public string Name { get; set; }    
        }
    }
}
