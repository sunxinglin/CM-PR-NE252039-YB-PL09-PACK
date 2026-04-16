using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

using Ctp0600P.Client.Apis;
using Ctp0600P.Client.Command.NoticeHelp.Enum;
using Ctp0600P.Client.Command.NoticeHelp.Handle;
using Ctp0600P.Client.CommonEntity;
using Ctp0600P.Client.CommonHelper;
using Ctp0600P.Client.Protocols;

using FutureTech.Mvvm;

using MediatR;

using Microsoft.Xaml.Behaviors.Core;

using Yee.Common.Library.CommonEnum;
using Yee.Entitys.AlarmMgmt;
using Yee.Entitys.Production;

namespace Ctp0600P.Client.ViewModels.StationTaskViewModels
{
    public class AnyLoadConfigPageVM : ViewModelBase
    {
        public readonly APIHelper _APIHepler;
        private readonly IMediator mediator;

        public AnyLoadConfigPageVM(APIHelper apiHepler,IMediator mediator)
        {
            _APIHepler = apiHepler;
            this.mediator = mediator;

            LoadProtocols();
            ComList = new ObservableCollection<ComData>(LocalMachineHelper.LoadSerialPortList());

            BaudRateList.Add(new BaudRateData { BaudRate = 4800 });
            BaudRateList.Add(new BaudRateData { BaudRate = 9600 });
            BaudRateList.Add(new BaudRateData { BaudRate = 19200 });
            BaudRateList.Add(new BaudRateData { BaudRate = 115200 });
        }

        public async void LoadProtocols()
        {
            var resourceList = await _APIHepler.LoadStationProResourceConfig(ProResourceTypeEnum.电子秤);
            if (resourceList != null)
                StationAnyLoadList = new ObservableCollection<Base_ProResource>(resourceList);
        }

        public ObservableCollection<Base_ProResource> StationAnyLoadList
        {
            get => _StationAnyLoadList;
            set
            {
                if (_StationAnyLoadList != value)
                {
                    _StationAnyLoadList = value;
                    OnPropertyChanged(nameof(StationAnyLoadList));
                }
            }
        }
        private ObservableCollection<Base_ProResource> _StationAnyLoadList = new ObservableCollection<Base_ProResource>();


        public ObservableCollection<ComData> ComList
        {
            get => _ComList;
            set
            {
                if (_ComList != value)
                {
                    _ComList = value;
                    OnPropertyChanged(nameof(ComList));
                }
            }
        }
        private ObservableCollection<ComData> _ComList = new ObservableCollection<ComData>();

        public ObservableCollection<BaudRateData> BaudRateList
        {
            get => _BaudRateList;
            set
            {
                if (_BaudRateList != value)
                {
                    _BaudRateList = value;
                    OnPropertyChanged(nameof(BaudRateList));
                }
            }
        }
        private ObservableCollection<BaudRateData> _BaudRateList = new ObservableCollection<BaudRateData>();

        private ActionCommand add;
        public ICommand Add => add ??= new ActionCommand(PerformAdd);

        private void PerformAdd()
        {
            var maxDeviceNo = StationAnyLoadList.Max(a => a.DeviceNo);
            if (string.IsNullOrEmpty(maxDeviceNo)) maxDeviceNo = "0";

            StationAnyLoadList.Add(new Base_ProResource
            {
                ProResourceType = ProResourceTypeEnum.电子秤,
                ProtocolType = ProtocolTypeEnum.RS232,
                StationCode = App._StepStationSetting.StationCode,
                Name = App._StepStationSetting.Step.Code + ProResourceTypeEnum.电子秤.ToString() + (StationAnyLoadList.Count + 1).ToString(),
                Code = App._StepStationSetting.Step.Code + "AnyLoad" + (StationAnyLoadList.Count + 1).ToString(),
                DeviceNo= (int.Parse(maxDeviceNo) +1).ToString()
            });
        }

        private ActionCommand del;
        public ICommand Del => del ??= new ActionCommand(PerformDel);

        private void PerformDel()
        {
            for (var i = StationAnyLoadList.Count - 1; i >= 0; i--)
            {
                if (StationAnyLoadList[i].IsSelected)
                    StationAnyLoadList.RemoveAt(i);
            }
        }

        private ActionCommand save;
        public ICommand Save => save ??= new ActionCommand(PerformSave);

        private void PerformSave()
        {
            foreach (var config in StationAnyLoadList)
            {
                if (config.Id == 0)
                {
                    config.CreateUserID = App.UserId;
                    config.CreateTime = DateTime.Now;
                }
                config.UpdateUserID = App.UserId;
                config.UpdateTime = DateTime.Now;
            }

            var disComCount = StationAnyLoadList.Where(scan => !string.IsNullOrEmpty(scan.Port.TrimEnd()) && scan.Baud > 0).DistinctBy(scan => scan.Port).ToList().Count;
            var configCount = StationAnyLoadList.Where(scan => !string.IsNullOrEmpty(scan.Port.TrimEnd()) && scan.Baud > 0).ToList().Count;
            if (configCount == 0)
            {
                var notice = new MessageNotice() { messageType = MessageTypeEnum.提示, showType = MessageShowType.右下角弹窗, MessageStr = $"至少配置一台电子秤才能保存！" };
                this.mediator.Publish(notice);
                mediator.Publish(new AlarmSYSNotification() { Code = AlarmCode.称重错误, Name = AlarmCode.称重错误.ToString(), Module = AlarmModule.DESOUTTER_MODULE, Description = $"至少配置一台电子秤才能保存！" });
                return;
            }

            if (configCount > disComCount)
            {
                var notice = new MessageNotice() { messageType = MessageTypeEnum.提示, showType = MessageShowType.右下角弹窗, MessageStr = $"一个串口号只能配置一台电子秤！！" };
                this.mediator.Publish(notice);
                mediator.Publish(new AlarmSYSNotification() { Code = AlarmCode.称重错误, Name = AlarmCode.称重错误.ToString(), Module = AlarmModule.DESOUTTER_MODULE, Description = $"一个串口号只能配置一台电子秤！！" });
                return;
            }

            for (var i = StationAnyLoadList.Count - 1; i >= 0; i--)
            {
                var scan = StationAnyLoadList[i];
                if (string.IsNullOrEmpty(scan.Port.TrimEnd()) || scan.Baud == 0)
                    StationAnyLoadList.RemoveAt(i);
            }

            SaveAnyLoadConifgData();
        }

        /// <summary>
        /// 保存电子秤配置
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        private async void SaveAnyLoadConifgData()
        {
            var response = await _APIHepler.SaveStationEquipmentConifgData(StationAnyLoadList.ToList(),ProResourceTypeEnum.电子秤);
            if (response.Code == 200)
            {
                StationAnyLoadList = new ObservableCollection<Base_ProResource>(response.Result);
                var notice = new MessageNotice() { messageType = MessageTypeEnum.提示, showType = MessageShowType.右下角弹窗, MessageStr = $"称重数据保存成功！！" };
                await this.mediator.Publish(notice);
            }
            else
            {
                await mediator.Publish(new AlarmSYSNotification() { Code = AlarmCode.称重错误, Name = AlarmCode.称重错误.ToString(), Module = AlarmModule.DESOUTTER_MODULE, Description = $"称重数据保存失败！！" });
            }
        }
    }
}
