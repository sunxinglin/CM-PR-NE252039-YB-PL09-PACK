using FutureTech.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ctp0600P.Client.CommonEntity
{

    public class ComDataDTO : ViewModelBase
    {
        private List<ComData> _ComDataList;
        public List<ComData> ComDataList
        {
            get => _ComDataList;
            set
            {
                if (_ComDataList != value)
                {
                    _ComDataList = value;
                    this.OnPropertyChanged(nameof(ComDataList));
                }
            }
        }

        private List<BaudRateData> _BaudRateDataList;
        public List<BaudRateData> BaudRateDataList
        {
            get => _BaudRateDataList;
            set
            {
                if (_BaudRateDataList != value)
                {
                    _BaudRateDataList = value;
                    this.OnPropertyChanged(nameof(BaudRateDataList));
                }
            }
        }
    }

    public class ComData : ViewModelBase
    {
        private string _PortName;
        public string PortName
        {
            get => _PortName;
            set
            {
                if (_PortName != value)
                {
                    _PortName = value;
                    this.OnPropertyChanged(nameof(PortName));
                }
            }
        }
    }

    public class BaudRateData : ViewModelBase
    {
        private int _BaudRate;
        public int BaudRate
        {
            get => _BaudRate;
            set
            {
                if (_BaudRate != value)
                {
                    _BaudRate = value;
                    this.OnPropertyChanged(nameof(BaudRate));
                }
            }
        }
    }
}
