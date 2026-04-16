using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

using AsZero.Core.Services.Messages;

using Ctp0600P.Shared;

using FutureTech.Mvvm;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Yee.Entitys.Production;

namespace Ctp0600P.Client.UserControls.DebugTools
{
    public class AnyLoadUserControlVM : ViewModelBase
    {
        private readonly ILogger<AnyLoadUserControlVM> _logger;
        private readonly StepStationSetting _stationSetting;
        private const int LOGS_MAX = 100;
        public AnyLoadUserControlVM(ILogger<AnyLoadUserControlVM> logger, IOptionsMonitor<StepStationSetting> stationSetting)
        {
            _logger = logger;
            _stationSetting = stationSetting.CurrentValue;


            this.AddMidMsg = new AsyncRelayCommand<LogMessage>(
                s =>
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
                },
                o => true
            );

            //var list = _aPIHelper.LoadStationProResource_Desoutter(_stationSetting);
            //if(list != null)
            //{
            //    foreach (var item in list.StationProResourceList)
            //    {
            //        StationDesoutterList.Add(item);
            //    }
            //}


            this.MID0001 = new AsyncRelayCommand<object>(
                async o =>
                {
                    //await _desoutterApi.SendMID0001(SelectedDesoutter.Name);
                },
                o => true
            );

            this.MID0014 = new AsyncRelayCommand<object>(
                async o =>
                {
                    //await _desoutterApi.SendMID0014(SelectedDesoutter.Name);
                },
                o => true
            );

            this.MID0016 = new AsyncRelayCommand<object>(
                async o =>
                {
                    //await _desoutterApi.SendMID0016(SelectedDesoutter.Name);
                },
                o => true
            );
            this.MID0018 = new AsyncRelayCommand<object>(
                async o =>
                {
                    //await _desoutterApi.SendMID0018(SelectedDesoutter.Name);
                },
                o => true
            );
            this.MID0042 = new AsyncRelayCommand<object>(
                async o =>
                {
                    //await _desoutterApi.SendMID0042(SelectedDesoutter.Name);
                },
                o => true
            );
            this.MID0043 = new AsyncRelayCommand<object>(
                async o =>
                {
                    //await _desoutterApi.SendMID0043(SelectedDesoutter.Name);
                },
                o => true
            );
            this.MID0060 = new AsyncRelayCommand<object>(
                async o =>
                {
                    //await _desoutterApi.SendMID0060(SelectedDesoutter.Name);
                },
                o => true
            );
            this.MID0062 = new AsyncRelayCommand<object>(
                async o =>
                {
                    //await _desoutterApi.SendMID0062(SelectedDesoutter.Name);
                },
                o => true
            );
            this.MID0070 = new AsyncRelayCommand<object>(
                async o =>
                {
                    //await _desoutterApi.SendMID0070(SelectedDesoutter.Name);
                },
                o => true
            );
            this.MID0082 = new AsyncRelayCommand<object>(
                async o =>
                {
                    //await _desoutterApi.SendMID0082(SelectedDesoutter.Name);
                },
                o => true
            );
            this.MID0701 = new AsyncRelayCommand<object>(
                async o =>
                {
                    //await _desoutterApi.SendMID0701(SelectedDesoutter.Name);
                },
                o => true
            );
        }

        public ICommand MID0001 { get; }
        public ICommand MID0014 { get; }
        public ICommand MID0016 { get; }
        public ICommand MID0018 { get; }

        public ICommand MID0042 { get; }
        public ICommand MID0043 { get; }
        public ICommand MID0060 { get; }
        public ICommand MID0062 { get; }
        public ICommand MID0070 { get; }

        public ICommand MID0082 { get; }
        public ICommand MID0701 { get; }

        public Base_ProResource SelectedDesoutter { get; set; }

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

        private List<Base_ProResource> _StationDesoutterList = new List<Base_ProResource>();

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
    }
}
