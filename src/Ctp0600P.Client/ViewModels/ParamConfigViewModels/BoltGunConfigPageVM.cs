using Ctp0600P.Client.Apis;
using Ctp0600P.Client.Command.NoticeHelp.Enum;
using Ctp0600P.Client.Command.NoticeHelp.Handle;
using Ctp0600P.Client.CommonEntity;
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
    public class BoltGunConfigPageVM : ViewModelBase
    {
        public readonly APIHelper _APIHepler;
        private readonly IMediator mediator;

        public BoltGunConfigPageVM(APIHelper apiHepler,IMediator mediator)
        {
            _APIHepler = apiHepler;
            this.mediator = mediator;
            LoadProtocols();
        }

        private async void LoadProtocols()
        {
            var resourceList = await _APIHepler.LoadStationProResourceConfig(ProResourceTypeEnum.拧紧枪);
            if (resourceList != null)
                StationBoltGunList = new ObservableCollection<Base_ProResource>(resourceList);
        }
        public ObservableCollection<Base_ProResource> StationBoltGunList
        {
            get => _StationBoltGunList;
            set
            {
                if (_StationBoltGunList != value)
                {
                    _StationBoltGunList = value;
                    OnPropertyChanged(nameof(StationBoltGunList));
                }
            }
        }
        private ObservableCollection<Base_ProResource> _StationBoltGunList = new ObservableCollection<Base_ProResource>();

        private ActionCommand add;
        public ICommand Add => add ??= new ActionCommand(PerformAdd);

        private void PerformAdd()
        {
            var maxDeviceNo = StationBoltGunList.Max(a => a.DeviceNo);
            if (string.IsNullOrEmpty(maxDeviceNo)) maxDeviceNo = "0";
            StationBoltGunList.Add(new Base_ProResource
            {
                ProResourceType = ProResourceTypeEnum.拧紧枪,
                ProtocolType = ProtocolTypeEnum.TCPIP,
                StationCode = App._StepStationSetting.StationCode,
                Name = App._StepStationSetting.Step.Code + ProResourceTypeEnum.拧紧枪.ToString() + (StationBoltGunList.Count + 1).ToString(),
                Code = App._StepStationSetting.Step.Code + "Bolt" + (StationBoltGunList.Count + 1).ToString(),
                DeviceNo= (int.Parse(maxDeviceNo) +1).ToString()
            });
        }

        private ActionCommand del;
        public ICommand Del => del ??= new ActionCommand(PerformDel);

        private void PerformDel()
        {
            for (var i = StationBoltGunList.Count - 1; i >= 0; i--)
            {
                if (StationBoltGunList[i].IsSelected)
                    StationBoltGunList.RemoveAt(i);
            }
        }

        private ActionCommand save;
        public ICommand Save => save ??= new ActionCommand(PerformSave);

        private void PerformSave()
        {
            foreach (var config in StationBoltGunList)
            {
                if (config.Id == 0)
                {
                    config.CreateUserID = App.UserId;
                    config.CreateTime = DateTime.Now;
                }
                config.UpdateUserID = App.UserId;
                config.UpdateTime = DateTime.Now;

                var portValidate = ValidationHelper.PositiveInteger(config.Port);
                var programeValidate = ValidationHelper.PositiveInteger(config.DeviceNo.ToString());
                var ipValidate = ValidationHelper.IsIpAddress(config.IpAddress);
                if (!portValidate)
                {
                    var notice = new MessageNotice() { messageType = MessageTypeEnum.提示, showType = MessageShowType.右下角弹窗, MessageStr = $"端口号需为正整数！" };
                    this.mediator.Publish(notice);
                    return;
                }
                if (!programeValidate)
                {
                    var notice = new MessageNotice() { messageType = MessageTypeEnum.提示, showType = MessageShowType.右下角弹窗, MessageStr = $"设备号需为正整数！" };
                    this.mediator.Publish(notice);

                    return;
                }
                if (!programeValidate)
                {
                    var notice = new MessageNotice() { messageType = MessageTypeEnum.提示, showType = MessageShowType.右下角弹窗, MessageStr = $"请输入合法的IP地址！" };
                    this.mediator.Publish(notice);
                    return;
                }
            }

            var disIPCount = StationBoltGunList.DistinctBy(bolt => bolt.IpAddress).ToList().Count;
            var configCount = StationBoltGunList.ToList().Count;
            //if (configCount == 0)
            //{
            //    var notice = new MessageNotice() { messageType = MessageTypeEnum.提示, showType = MessageShowType.右下角弹窗, MessageStr = $"至少配置一台拧紧枪才能保存！" };
            //    this.mediator.Publish(notice);
            //    return;
            //}

            if (configCount > disIPCount)
            {
                var notice = new MessageNotice() { messageType = MessageTypeEnum.提示, showType = MessageShowType.右下角弹窗, MessageStr = $"一个IP地址号只能配置一台拧紧枪！" };
                this.mediator.Publish(notice);
                return;
            }

            SaveBoltGunConifgData();
        }

        /// <summary>
        /// 保存拧紧枪配置
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        private async void SaveBoltGunConifgData()
        {
            var response = await _APIHepler.SaveStationEquipmentConifgData(StationBoltGunList.ToList(),ProResourceTypeEnum.拧紧枪);
            if (response.Code == 200)
            {
                StationBoltGunList = new ObservableCollection<Base_ProResource>(response.Result);
                var notice = new MessageNotice() { messageType = MessageTypeEnum.提示, showType = MessageShowType.右下角弹窗, MessageStr = $"保存成功！！" };
                await this.mediator.Publish(notice);
            }
            else
            {
                var notice = new MessageNotice() { messageType = MessageTypeEnum.提示, showType = MessageShowType.右下角弹窗, MessageStr = $"保存失败！！" };
                await this.mediator.Publish(notice);
                await mediator.Publish(new AlarmSYSNotification() { Code = AlarmCode.拧紧枪错误, Name = AlarmCode.拧紧枪错误.ToString(), Module = AlarmModule.DESOUTTER_MODULE, Description = $"拧紧枪数据保存失败！" });
            }
        }
    }
}
