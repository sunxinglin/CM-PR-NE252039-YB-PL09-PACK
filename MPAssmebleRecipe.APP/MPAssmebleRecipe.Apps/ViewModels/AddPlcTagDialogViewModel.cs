using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.FormulaExpressions;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using RogerTech.AuthService;
using RogerTech.AuthService.Models;
using RogerTech.Common;
using RogerTech.Tool;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace MPAssmebleRecipe.Apps.ViewModels
{
    public class AddPlcTagDialogViewModel : BindableBase, IDialogAware,IDisposable
    {
        private readonly RogerTech.AuthService.AuthService _authService;
        private readonly IEventAggregator _eventAggregator;
        private SubscriptionToken _closeToken;
        Group group;
        Tag tag;

        private string _TagName;
        public string TagName
        {
            get => _TagName;
            set => SetProperty(ref _TagName, value);
        }

        private string _IP;
        public string IP
        {
            get => _IP;
            set => SetProperty(ref _IP, value);
        }

        private int _Dbnr;
        public int Dbnr
        {
            get => _Dbnr;
            set => SetProperty(ref _Dbnr, value);
        }

        private int _StartAddress;
        public int StartAddress
        {
            get => _StartAddress;
            set => SetProperty(ref _StartAddress, value);
        }

        private int _DataLength;
        public int DataLength
        {
            get => _DataLength;
            set => SetProperty(ref _DataLength, value);
        }

        private RogerTech.Tool.DataType _DateTypeValue;
        public RogerTech.Tool.DataType DateTypeValue
        {
            get => _DateTypeValue;
            set => SetProperty(ref _DateTypeValue, value);
        }

        private byte _DataBit;
        public byte DataBit
        {
            get => _DataBit;
            set => SetProperty(ref _DataBit, value);
        }


        private string _MesName;
        public string MesName
        {
            get => _MesName;
            set => SetProperty(ref _MesName, value);
        }
        private ParameterDataType _MesDataType;
        public ParameterDataType MesDataType
        {
            get => _MesDataType;
            set => SetProperty(ref _MesDataType, value);
        }
        private bool _IsUpload;
        public bool IsUpload
        {
            get => _IsUpload;
            set => SetProperty(ref _IsUpload, value);
        }
        private double _LowerLimit;
        public double LowerLimit
        {
            get => _LowerLimit;
            set => SetProperty(ref _LowerLimit, value);
        }
        private double _UpperLimit;
        public double UpperLimit
        {
            get => _UpperLimit;
            set => SetProperty(ref _UpperLimit, value);
        }
        private bool _IsChecked;
        public bool IsChecked
        {
            get => _IsChecked;
            set => SetProperty(ref _IsChecked, value);
        }

        private Array _mesDataTypes;
        public Array MesDataTypes
        {
            get => _mesDataTypes;
            set => SetProperty(ref _mesDataTypes, value);
        }
        /// <summary>   
        /// 数据类型数据源
        /// </summary>
        private Array _lsDateType;
        public Array lsDateType
        {
            get => _lsDateType;
            set => SetProperty(ref _lsDateType, value);
        }

        public string Title => "添加点位";

        public event Action<IDialogResult> RequestClose;

        public DelegateCommand SaveCommand { get; }
        public DelegateCommand CancelCommand { get; }

        public void Dispose()
        {
            // 取消订阅
            if (_closeToken != null)
            {
                _eventAggregator.GetEvent<CloseAllDialogsEvent>().Unsubscribe(_closeToken);
                _closeToken = null;
            }
        }


        public AddPlcTagDialogViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _closeToken = _eventAggregator.GetEvent<CloseAllDialogsEvent>().Subscribe(() => RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel)));

          
            SaveCommand = new DelegateCommand(Save);
            CancelCommand = new DelegateCommand(() => RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel)));
            lsDateType = Enum.GetValues(typeof(RogerTech.Tool.DataType));
            MesDataTypes = Enum.GetValues(typeof(ParameterDataType));
        }
       
        private void Save()
        {
            if (string.IsNullOrWhiteSpace(TagName) ||  Dbnr <= 0 || StartAddress < 0 || DataLength <= 0 || DataBit < 0)
            {
                
               MessageBox.Show("请输入正确的信息！");
                
                // TODO: 显示错误提示
                return;
            }

            Tag tagEmpty = group.GetTag(TagName);
            if (tagEmpty == null)
            {
                tag = new Tag(group.Connections[0], TagName, Dbnr, StartAddress, DataLength, DateTypeValue, DataBit, MesName, MesDataType, LowerLimit, UpperLimit, IsChecked, IsUpload);
                group.Tags.Add(tag);
            }
            else
            {
                tag = new Tag(group.Connections[0], TagName, Dbnr, StartAddress, DataLength, DateTypeValue, DataBit, MesName, MesDataType, LowerLimit, UpperLimit, IsChecked, IsUpload);
                group.Tags.Add(tag);
                group.Tags.Remove(tagEmpty);
            }
            var parameters = new DialogParameters
            {
                { "tag", tag },
                { "group", group }
            };
            RequestClose?.Invoke(new DialogResult(ButtonResult.OK, parameters));
        }

        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {
            Dispose();
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            group = parameters.GetValue<Group>("group");
            tag = parameters.GetValue<Tag>("tag");
            if (tag != null)
            {
                IP = tag.Connection.IP;
                TagName = tag.TagName;
                Dbnr = tag.Dbnr;
                StartAddress = tag.StartAddress;
                DataLength = tag.DataLength;
                DateTypeValue = tag.DataType;
                DataBit = tag.DataBit;
                MesName = tag.MesName;
                MesDataType = tag.MesDataType;
                LowerLimit = tag.LowerLimit;
                UpperLimit = tag.UpperLimit;
                IsChecked = tag.IsChecked;
                IsUpload = tag.IsUpload;
            }
        }
    }
}
