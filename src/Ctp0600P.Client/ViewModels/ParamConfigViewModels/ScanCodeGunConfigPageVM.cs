using Ctp0600P.Client.Apis;
using Ctp0600P.Client.Command.NoticeHelp.Enum;
using Ctp0600P.Client.Command.NoticeHelp.Handle;
using Ctp0600P.Client.CommonEntity;
using Ctp0600P.Client.CommonHelper;
using Ctp0600P.Client.Protocols;
using Ctp0600P.Client.Validations;
using FutureTech.Mvvm;
using MediatR;
using Microsoft.Xaml.Behaviors.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Yee.Common.Library.CommonEnum;
using Yee.Entitys.AlarmMgmt;
using Yee.Entitys.DBEntity;
using Yee.Entitys.DTOS;
using Yee.Entitys.Production;

namespace Ctp0600P.Client.ViewModels.StationTaskViewModels
{
    public class ScanCodeGunConfigPageVM : ViewModelBase
    {
        public readonly APIHelper _APIHepler;
        private readonly IMediator mediator;

        public ScanCodeGunConfigPageVM(APIHelper apiHepler, IMediator mediator)
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
            var resourceList = await _APIHepler.LoadStationProResourceConfig(ProResourceTypeEnum.扫码枪);
            if (resourceList != null)
                StationScanCodeGunList = new ObservableCollection<Base_ProResource>(resourceList);
        }

        public ObservableCollection<Base_ProResource> StationScanCodeGunList
        {
            get => _StationScanCodeGunList;
            set
            {
                if (_StationScanCodeGunList != value)
                {
                    _StationScanCodeGunList = value;
                    OnPropertyChanged(nameof(StationScanCodeGunList));
                }
            }
        }
        private ObservableCollection<Base_ProResource> _StationScanCodeGunList = new ObservableCollection<Base_ProResource>();


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
            var maxDeviceNo = StationScanCodeGunList.Max(a => a.DeviceNo);
            if (string.IsNullOrEmpty(maxDeviceNo)) maxDeviceNo = "0";

            StationScanCodeGunList.Add(new Base_ProResource
            {
                ProResourceType = ProResourceTypeEnum.扫码枪,
                ProtocolType = ProtocolTypeEnum.RS232,
                StationCode = App._StepStationSetting.StationCode,
                Name = App._StepStationSetting.Step.Code + ProResourceTypeEnum.扫码枪.ToString() + (StationScanCodeGunList.Count + 1).ToString(),
                Code = App._StepStationSetting.Step.Code + "Scan" + (StationScanCodeGunList.Count + 1).ToString(),
                DeviceNo = (int.Parse(maxDeviceNo) + 1).ToString()
            });
        }

        private ActionCommand del;
        public ICommand Del => del ??= new ActionCommand(PerformDel);

        private void PerformDel()
        {
            for (var i = StationScanCodeGunList.Count - 1; i >= 0; i--)
            {
                if (StationScanCodeGunList[i].IsSelected)
                    StationScanCodeGunList.RemoveAt(i);
            }
        }

        private ActionCommand save;
        public ICommand Save => save ??= new ActionCommand(PerformSave);

        private void PerformSave()
        {
            foreach (var config in StationScanCodeGunList)
            {
                if (config.Id == 0)
                {
                    config.CreateUserID = App.UserId;
                    config.CreateTime = DateTime.Now;
                }
                config.UpdateUserID = App.UserId;
                config.UpdateTime = DateTime.Now;

                var portValidate = ValidationHelper.NotNullValidation(config.Port);
                var programeValidate = ValidationHelper.PositiveInteger(config.DeviceNo.ToString());
                var baudValidate = ValidationHelper.PositiveInteger(config.Baud.ToString());
                if (!portValidate)
                {
                    var notice = new MessageNotice() { messageType = MessageTypeEnum.提示, showType = MessageShowType.右下角弹窗, MessageStr = $"请选择串口号！" };
                    this.mediator.Publish(notice);
                    mediator.Publish(new AlarmSYSNotification() { Code = AlarmCode.扫码枪错误, Name = AlarmCode.扫码枪错误.ToString(), Module = AlarmModule.DESOUTTER_MODULE, Description = $"请选择串口号！" });
                    return;
                }
                if (!programeValidate)
                {
                    var notice = new MessageNotice() { messageType = MessageTypeEnum.提示, showType = MessageShowType.右下角弹窗, MessageStr = $"设备号需为正整数！" };
                    this.mediator.Publish(notice);
                    mediator.Publish(new AlarmSYSNotification() { Code = AlarmCode.扫码枪错误, Name = AlarmCode.扫码枪错误.ToString(), Module = AlarmModule.DESOUTTER_MODULE, Description = $"设备号需为正整数！" });

                    return;
                }
                if (!baudValidate)
                {
                    var notice = new MessageNotice() { messageType = MessageTypeEnum.提示, showType = MessageShowType.右下角弹窗, MessageStr = $"请选择波特率！" };
                    this.mediator.Publish(notice);
                    mediator.Publish(new AlarmSYSNotification() { Code = AlarmCode.扫码枪错误, Name = AlarmCode.扫码枪错误.ToString(), Module = AlarmModule.DESOUTTER_MODULE, Description = $"请选择波特率！" });
                    return;
                }
            }

            var disComCount = StationScanCodeGunList.Where(scan => !string.IsNullOrEmpty(scan.Port) && scan.Baud > 0).DistinctBy(scan => scan.Port).ToList().Count;
            var configCount = StationScanCodeGunList.Where(scan => !string.IsNullOrEmpty(scan.Port) && scan.Baud > 0).ToList().Count;
            //if (configCount == 0)
            //{
            //    var notice = new MessageNotice() { messageType = MessageTypeEnum.提示, showType = MessageShowType.右下角弹窗, MessageStr = $"至少配置一台扫码枪才能保存！" };
            //    this.mediator.Publish(notice);
            //    mediator.Publish(new AlarmSYSNotification() { Code = AlarmCode.扫码枪错误, Name = AlarmCode.扫码枪错误.ToString(), Module = AlarmModule.DESOUTTER_MODULE, Description = $"至少配置一台扫码枪才能保存！" });
            //    return;
            //}

            if (configCount > disComCount)
            {
                var notice = new MessageNotice() { messageType = MessageTypeEnum.提示, showType = MessageShowType.右下角弹窗, MessageStr = $"一个串口号只能配置一台扫码枪！" };
                this.mediator.Publish(notice);
                mediator.Publish(new AlarmSYSNotification() { Code = AlarmCode.扫码枪错误, Name = AlarmCode.扫码枪错误.ToString(), Module = AlarmModule.DESOUTTER_MODULE, Description = $"一个串口号只能配置一台扫码枪！" });
                return;
            }

            for (var i = StationScanCodeGunList.Count - 1; i >= 0; i--)
            {
                var scan = StationScanCodeGunList[i];
                if (string.IsNullOrEmpty(scan.Port) || scan.Baud == 0)
                    StationScanCodeGunList.RemoveAt(i);
            }

            SaveScanCodeConifgData();
        }

        /// <summary>
        /// 保存扫码枪配置
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        private async void SaveScanCodeConifgData()
        {
            var response = await _APIHepler.SaveStationEquipmentConifgData(StationScanCodeGunList.ToList(), ProResourceTypeEnum.扫码枪);
            if (response.Code == 200)
            {
                StationScanCodeGunList = new ObservableCollection<Base_ProResource>(response.Result);
                var notice = new MessageNotice() { messageType = MessageTypeEnum.提示, showType = MessageShowType.右下角弹窗, MessageStr = $"保存成功！！" };
                await this.mediator.Publish(notice);
            }
            else
            {
                var notice = new MessageNotice() { messageType = MessageTypeEnum.提示, showType = MessageShowType.右下角弹窗, MessageStr = $"保存失败！！" };
                await this.mediator.Publish(notice!);
                await mediator.Publish(new AlarmSYSNotification() { Code = AlarmCode.扫码枪错误, Name = AlarmCode.扫码枪错误.ToString(), Module = AlarmModule.DESOUTTER_MODULE, Description = $"扫码数据保存失败！" });

            }
        }
    }
}
