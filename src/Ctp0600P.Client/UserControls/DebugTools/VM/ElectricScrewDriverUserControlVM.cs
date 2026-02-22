using AsZero.Core.Services.Messages;
using Ctp0600P.Client.Apis;
using Ctp0600P.Client.PLC.Context;
using Ctp0600P.Client.Protocols.BoltGun.Models;
using Ctp0600P.Shared;
using FutureTech.Mvvm;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Yee.Entitys.Production;

namespace Ctp0600P.Client.UserControls.DebugTools
{
    public class ElectricScrewDriverUserControlVM : ViewModelBase
    {
        private readonly ILogger<ElectricScrewDriverUserControlVM> _logger;
        private readonly APIHelper _aPIHelper;
        private readonly StepStationSetting _stationSetting;
        private const int LOGS_MAX = 100;
        private readonly StationPLCContext _stationPLCContext;


        public ElectricScrewDriverUserControlVM(ILogger<ElectricScrewDriverUserControlVM> logger, IOptionsMonitor<StepStationSetting> stationSetting, APIHelper aPIHelper, StationPLCContext stationPLCContext)
        {
            _logger = logger;
            _aPIHelper = aPIHelper;
            _stationSetting = stationSetting.CurrentValue;
            _stationPLCContext = stationPLCContext;
            App.DebugTool_Screw_VM = this;
            LoadProtocols();

            App.BoltGunRequestSubject.ToEvent().OnNext += UpdateScrewResult;
            this.AddMidMsg = new AsyncRelayCommand<LogMessage>(s =>
            {
                try
                {
                    var logcount = this.Mids.Count;
                    if (logcount >= LOGS_MAX)
                    {
                        this.Mids.RemoveAt(0);
                    }
                    this.Mids.Add(s);
                    this.IsMidsChanged = true;
                }
                catch (Exception e)
                {
                    this._logger.LogError($"添加日志出错：{e.Message}\n{e.StackTrace}");
                }
                return Task.CompletedTask;
            }, o => true);

            this.TightenGunEnabled = new AsyncRelayCommand<object>(async o =>
            {
                _ = ushort.TryParse(SelectedDesoutter.DeviceNo, out ushort deviceNo);
                _ = ushort.TryParse(SelectedBrand.Value, out ushort deviceBrand);

                if (string.IsNullOrWhiteSpace(SelectedDesoutter.DeviceNo))
                {
                    MessageBox.Show("请选择拧紧枪");
                    return;
                }
                if (string.IsNullOrWhiteSpace(SelectedBrand.Value))
                {
                    MessageBox.Show("请选择拧紧枪品牌");
                    return;
                }

                _stationPLCContext.StationTightenStartReqs.Add(new StationPLCContext.StationTightenStartReq
                {
                    DeviceNo = deviceNo,
                    DeviceBrand = deviceBrand,
                    ProgramNo = Convert.ToUInt16(pset),
                    SortNo = 1,
                    ScrewCountBeforeCurrentTask = 1,
                    Resource = "拧紧调试"
                });
            }, o => true);


        }

        public async void LoadProtocols()
        {

            var list = await _aPIHelper.LoadStationProResourceConfig(Yee.Common.Library.CommonEnum.ProResourceTypeEnum.拧紧枪);
            if (list != null)
            {
                foreach (var item in list)
                {
                    StationDesoutterList.Add(item);
                }
            }

            //拧紧枪品牌
            DeviceBrandList.Add(new DeviceBrandClass { Name = "马头", Value = "1" });
            DeviceBrandList.Add(new DeviceBrandClass { Name = "博世", Value = "2" });
        }

        public ICommand TightenGunEnabled { get; set; }

        public Base_ProResource SelectedDesoutter { get; set; }
        public DeviceBrandClass SelectedBrand { get; set; }

        public ObservableCollection<LogMessage> Mids { get; } = new ObservableCollection<LogMessage>();

        public ICommand AddMidMsg { get; }

        private bool _isMidsChanged;
        public bool IsMidsChanged
        {
            get => _isMidsChanged;
            set
            {
                this._isMidsChanged = value;
                this.OnPropertyChanged(nameof(IsMidsChanged));
            }
        }

        private int pset = 1;

        /// <summary>
        /// 程序号
        /// </summary>
        public int Pset
        {
            get => pset;
            set
            {
                this.pset = value;
                this.OnPropertyChanged(nameof(Pset));
            }
        }

        private List<Base_ProResource> _StationDesoutterList = new();
        public List<Base_ProResource> StationDesoutterList
        {
            get => _StationDesoutterList;
            set
            {
                if (_StationDesoutterList != value)
                {
                    _StationDesoutterList = value;
                    OnPropertyChanged(nameof(StationDesoutterList));
                }
            }
        }

        private List<DeviceBrandClass> _DeviceBrandList = new();
        public List<DeviceBrandClass> DeviceBrandList
        {
            get => _DeviceBrandList;
            set
            {
                if (_DeviceBrandList != value)
                {
                    _DeviceBrandList = value;
                    OnPropertyChanged(nameof(DeviceBrandList));
                }
            }
        }

        private string _角度;
        public string 角度
        {
            get => _角度;
            set
            {
                if (_角度 != value)
                {
                    _角度 = value;
                    OnPropertyChanged(nameof(角度));
                }
            }
        }

        private string _扭力;
        public string 扭力
        {
            get => _扭力;
            set
            {
                if (_扭力 != value)
                {
                    _扭力 = value;
                    OnPropertyChanged(nameof(扭力));
                }
            }
        }

        public void UpdateScrewResult(BoltGunRequest requset)
        {
            try
            {
                _logger.LogInformation($"拧紧结果展示：{requset.FinalTorque.ToString() + "N"}\n{requset.FinalAngle.ToString() + "°"}");
                App.Current.Dispatcher.Invoke(() =>
                {
                    扭力 = requset.FinalTorque.ToString() + "N";
                    角度 = requset.FinalAngle.ToString() + "°";
                });
            }
            catch (Exception e)
            {
                this._logger.LogError($"添加日志出错：{e.Message}\n{e.StackTrace}");
            }
        }
    }


    public class DeviceBrandClass
    {
        public string Value { get; set; }
        public string Name { get; set; }
    }
}
