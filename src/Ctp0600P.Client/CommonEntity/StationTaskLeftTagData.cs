using FutureTech.Mvvm;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Yee.Entitys.DBEntity;
using Yee.Entitys.DTOS;
using Yee.Entitys.Production;

namespace Ctp0600P.Client.CommonEntity
{
    public class StationTaskLeftTagData : ViewModelBase
    {
        public Brush _BackBrush;
        public Brush BackBrush
        {
            get => _BackBrush;
            set
            {
                _BackBrush = value;
                OnPropertyChanged(nameof(BackBrush));
            }
        }

        private Visibility _NeedShow = Visibility.Visible;
        public Visibility NeedShow
        {
            get => _NeedShow;
            set
            {
                if (_NeedShow != value)
                {
                    _NeedShow = value;
                    OnPropertyChanged(nameof(NeedShow));
                }
            }
        }
        private int _Status;
        public int Status
        {
            get => _Status;
            set
            {
                if (_Status != value)
                {
                    _Status = value;
                    switch (value)
                    {
                        case 0:
                            BackBrush = Brushes.WhiteSmoke;
                            break;
                        case 1:
                            BackBrush = Brushes.LightSkyBlue;
                            break;
                        case 2:
                            BackBrush = Brushes.LightGreen;
                            break;
                    }
                    OnPropertyChanged(nameof(Status));
                }
            }
        }


        private int _StepNo;
        public int StepNo
        {
            get => _StepNo;
            set
            {
                if (_StepNo != value)
                {
                    _StepNo = value;
                    OnPropertyChanged(nameof(StepNo));
                }
            }
        }
        public string _Header;
        public string Header
        {
            get => _Header;
            set
            {
                if (_Header != value)
                {
                    _Header = value;
                    OnPropertyChanged(nameof(Header));
                }
            }
        }

    }

    public class StationTaskData : ViewModelBase
    {
        public Base_StationTaskBom StationTaskBom { get; set; } = new Base_StationTaskBom();

        private ObservableCollection<Base_StationTaskScrew> _StationTaskScrewList = new ObservableCollection<Base_StationTaskScrew>() { };

        public ObservableCollection<Base_StationTaskScrew> StationTaskScrewList
        {
            get => _StationTaskScrewList;
            set
            {
                if (_StationTaskScrewList != value)
                {
                    _StationTaskScrewList = value;

                    //if (_StationTaskScrewList.Count > 0)
                    //{
                    //    foreach (var screw in _StationTaskScrewList)
                    //    {
                    //        screw.NeedReWordSource = new List<int>();
                    //        for (var i = 0; i <= (int)screw.ReworkLimitTimes; i++)
                    //        {
                    //            screw.NeedReWordSource.Add(i);
                    //        }
                    //    }
                    //}

                    OnPropertyChanged(nameof(StationTaskScrewList));
                }
            }

        }

        private int _Status;
        public int Status
        {
            get => _Status;
            set
            {
                if (_Status != value)
                {
                    _Status = value;
                    OnPropertyChanged(nameof(Status));
                }
            }
        }

        private int _StepNo;
        public int StepNo
        {
            get => _StepNo;
            set
            {
                if (_StepNo != value)
                {
                    _StepNo = value;
                    OnPropertyChanged(nameof(StepNo));
                }
            }
        }
        public string _Header;
        public string Header
        {
            get => _Header;
            set
            {
                if (_Header != value)
                {
                    _Header = value;
                    OnPropertyChanged(nameof(Header));
                }
            }
        }

        public object _Content;
        public object Content
        {
            get => _Content;
            set
            {
                if (_Content != value)
                {
                    _Content = value;
                    OnPropertyChanged(nameof(Content));
                }
            }
        }
        public StationTaskDTO StationTaskDTO { get; set; }

        private bool _IsSelected;
        public bool IsSelected
        {
            get => _IsSelected;
            set
            {
                if (_IsSelected != value)
                {
                    _IsSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }


        private bool _HasFinish = false;
        public bool HasFinish
        {
            get => _HasFinish;
            set
            {
                if (_HasFinish != value)
                {
                    _HasFinish = value;
                    OnPropertyChanged(nameof(HasFinish));
                }
            }
        }

        public string _FinishText;
        public string FinishText
        {
            get => _FinishText;
            set
            {
                if (_FinishText != value)
                {
                    _FinishText = value;
                    OnPropertyChanged(nameof(FinishText));
                }
            }
        }



        public Visibility _Visibility;
        public Visibility Visibility
        {
            get => _Visibility;
            set
            {
                if (_Visibility != value)
                {
                    _Visibility = value;
                    OnPropertyChanged(nameof(Visibility));
                }
            }
        }

        public bool _NeedReWord;
        public bool NeedReWord
        {
            get => _NeedReWord;
            set
            {
                if (_NeedReWord != value)
                {
                    _NeedReWord = value;
                    OnPropertyChanged(nameof(NeedReWord));
                }
            }
        }




    }
}
