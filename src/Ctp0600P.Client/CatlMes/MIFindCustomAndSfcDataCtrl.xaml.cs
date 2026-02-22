using Ctp0600P.Shared.CatlMes;
using Ctp0600P.Client.Apis;
using Reactive.Bindings;
using System;
using System.Windows;
using System.Windows.Controls;
using MediatR;
using Ctp0600P.Client.Command.NoticeHelp.Handle;
using Ctp0600P.Client.Command.NoticeHelp.Enum;
using Catl.WebServices.MIFindCustomAndSfcData;
using Yee.Services.CatlMesInvoker;

namespace Ctp0600P.Client.CatlMes
{
    /// <summary>
    /// Interaction logic for MIFindCustomAndSfcDataCtrl.xaml
    /// </summary>
    public partial class MIFindCustomAndSfcDataCtrl : UserControl
    {
        public MIFindCustomAndSfcDataCtrl()
        {
            InitializeComponent();
        }
    }


    public class MIFindCustomAndSfcDataCtrlVM
    {
        private readonly CatlMesIniConfigApi _configApi;
        private readonly ICatlMesInvoker _catlMesInvoker;
        private readonly IMediator mediator;

        public MIFindCustomAndSfcDataCtrlVM(CatlMesIniConfigApi configApi, ICatlMesInvoker catlMesInvoker, IMediator mediator)
        {
            this._configApi = configApi;
            this._catlMesInvoker = catlMesInvoker;
            this.mediator = mediator;


            this.StationCode = new ReactiveProperty<string>() { Value = App._StepStationSetting.StationCode };

            this.Url = new ReactiveProperty<string>();
            this.Timeout = new ReactiveProperty<int>();
            this.Site = new ReactiveProperty<string>();
            this.User = new ReactiveProperty<string>();
            this.UserName = new ReactiveProperty<string>();
            this.Password = new ReactiveProperty<string>();
            this.Operation = new ReactiveProperty<string>();
            this.OperationRevision = new ReactiveProperty<string>();
            this.ActivityId = new ReactiveProperty<string>();
            this.Resource = new ReactiveProperty<string>();
            this.sfcOrder = new ReactiveProperty<string>();
            this.targetOrder = new ReactiveProperty<string>();
            this.dcGroup = new ReactiveProperty<string>("*A");
            this.dcGroupRevision = new ReactiveProperty<string>("#");
            this.modeProcessSfc = new ReactiveProperty<modeProcessSFC>();
            this.PackSFC = new ReactiveProperty<string>();

            this.usage = new ReactiveProperty<string>();
            this.category = new ReactiveProperty<ObjectAliasEnum>(ObjectAliasEnum.ITEM);
            this.dataField = new ReactiveProperty<string>();
            this.masterDataArray  = new ReactiveProperty<ObjectAliasEnum>(ObjectAliasEnum.ITEM);
            this.findSfcByInventory =  new ReactiveProperty<bool>(true);
            
            this.inventory = new ReactiveProperty<string>();

#pragma warning disable CA1416 // 验证平台兼容性

            this.CmdReload = new ReactiveCommand<PasswordBox>()
                .WithSubscribe(async (passwdBox) =>
                {
                    try
                    {
                        var config = await this._configApi.GetMIFindCustomAndSfcDataConfig(this.StationCode.Value);
                        if (config != null)
                        {
                            BindDtoToVM(config, passwdBox);
                        }
                        else
                        {
                            var notice = new MessageNotice() { messageType = MessageTypeEnum.警告, showType = MessageShowType.右下角弹窗, MessageStr = "当前INI文件不含有装配模组的配置段" };
                            await this.mediator.Publish(notice);
                            
                        }
                    }
                    catch (Exception ex)
                    {
                        var notice = new MessageNotice() { messageType = MessageTypeEnum.警告, showType = MessageShowType.右下角弹窗, MessageStr = $"{ex.Message}" };
                        await this.mediator.Publish(notice);
                      
                    }
                });


            this.CmdSave = new ReactiveCommand<PasswordBox>()
                .WithSubscribe(async passwdBox =>
                {
                    try
                    {
                        if (string.IsNullOrEmpty(passwdBox.Password))
                        {
                            var notice = new MessageNotice() { messageType = MessageTypeEnum.错误, showType = MessageShowType.右下角弹窗, MessageStr = "没有配置密码，无法保存" };
                            await this.mediator.Publish(notice);
                            return;
                        }

                        var config = this.MapVMToDto(passwdBox);
                        var x = await this._configApi.SetMIFindCustomAndSfcDataConfig(config,this.StationCode.Value);
                        if (x.Code == 0)
                        {
                            var notice = new MessageNotice() { messageType = MessageTypeEnum.提示, showType = MessageShowType.右下角弹窗, MessageStr = "保存成功" };
                            await this.mediator.Publish(notice);
                        }
                        else
                        {
                            var notice = new MessageNotice() { messageType = MessageTypeEnum.警告, showType = MessageShowType.右下角弹窗, MessageStr = $"保存失败：{x.Msg}" };
                            await this.mediator.Publish(notice);
                        }
                    }
                    catch (Exception ex)
                    {
                        var notice = new MessageNotice() { messageType = MessageTypeEnum.错误, showType = MessageShowType.右下角弹窗, MessageStr = $"{ex.Message}" };
                        await this.mediator.Publish(notice);
                    }
                });

            this.CmdTest = new ReactiveCommand<PasswordBox>()
                .WithSubscribe(async passwdBox =>
                {
                    try
                    {
                        //var reponse = await this._catlMesInvoker.miFindCustomAndSfcData(PackSFC.Value);
                        //if (reponse.code == 0)
                        //{
                        //    var notice = new MessageNotice() { messageType = MessageTypeEnum.提示, showType = MessageShowType.右下角弹窗, MessageStr = "进站完成" };
                        //    await this.mediator.Publish(notice);
                        //}
                        //else
                        //{
                        //    var notice = new MessageNotice() { messageType = MessageTypeEnum.警告, showType = MessageShowType.右下角弹窗, MessageStr = $"进站失败：{reponse.message}" };
                        //    await this.mediator.Publish(notice);
                        //}
                    }
                    catch (Exception ex)
                    {
                        var notice = new MessageNotice() { messageType = MessageTypeEnum.错误, showType = MessageShowType.右下角弹窗, MessageStr = $"{ex.Message}" };
                        await this.mediator.Publish(notice);
                    }
                });
            
        }

        private void BindDtoToVM(MIFindCustomAndSfcDataConfig section, PasswordBox passwdBox)
        {
            passwdBox.Password = section.ConnectionParams.Password;
            this.Url.Value = section.ConnectionParams.Url;
            this.UserName.Value = section.ConnectionParams.UserName;
            this.Timeout.Value = section.ConnectionParams.Timeout;
            this.Site.Value = section.InterfaceParams.Site;
            this.User.Value = section.InterfaceParams.User;
            this.Operation.Value = section.InterfaceParams.Operation;
            this.OperationRevision.Value = section.InterfaceParams.OperationRevision;
            this.ActivityId.Value = section.InterfaceParams.ActivityId;
            this.Resource.Value = section.InterfaceParams.Resource;
            this.sfcOrder.Value = section.InterfaceParams.sfcOrder;
            this.targetOrder.Value = section.InterfaceParams.targetOrder;
            this.dcGroup.Value = section.InterfaceParams.dcGroup;
            this.dcGroupRevision.Value = section.InterfaceParams.dcGroupRevision;
            this.modeProcessSfc.Value = section.InterfaceParams.modeProcessSfc;
            this.category.Value = section.InterfaceParams.category;
            this.dataField.Value = section.InterfaceParams.dataField;
            this.findSfcByInventory.Value = section.InterfaceParams.findSfcByInventory;
            this.masterDataArray.Value = section.InterfaceParams.masterDataArray;
            this.usage.Value= section.InterfaceParams.usage;
            this.inventory.Value= section.InterfaceParams.inventory;
        }


        private MIFindCustomAndSfcDataConfig MapVMToDto(PasswordBox passwdBox)
        {
            var conn = new CatlMesConnectionParams
            {
                Url = this.Url.Value,
                UserName = this.UserName.Value,
                Password = passwdBox.Password,
                Timeout = this.Timeout.Value,
            };
            var inter = new MIFindCustomAndSfcParams
            {
                Site = this.Site.Value,
                User = this.User.Value,
                Operation = this.Operation.Value,
                OperationRevision = this.OperationRevision.Value,
                ActivityId = this.ActivityId.Value,
                Resource=this.Resource.Value,
                dcGroup=this.dcGroup.Value,
                dcGroupRevision=this.dcGroupRevision.Value,
                modeProcessSfc=this.modeProcessSfc.Value,
                sfcOrder=this.sfcOrder.Value,
                targetOrder=this.targetOrder.Value,
                category=this.category.Value,
                dataField=this.dataField.Value,
                findSfcByInventory = this.findSfcByInventory.Value,
                masterDataArray =this.masterDataArray.Value,
                usage=this.usage.Value,
                inventory=this.inventory.Value,
            };

            return new MIFindCustomAndSfcDataConfig
            {
                ConnectionParams = conn,
                InterfaceParams = inter,
            };

        }

        public ReactiveProperty<string> StationCode { get; set; }
        public ReactiveProperty<string> Url { get; }
        public ReactiveProperty<string> UserName { get; }
        public ReactiveProperty<string> Password { get; }

        public ReactiveProperty<int> Timeout { get; }
        public ReactiveProperty<string> Site { get; }
        public ReactiveProperty<string> User { get; }
        public ReactiveProperty<string> Operation { get; }
        public ReactiveProperty<string> OperationRevision { get; }
        public ReactiveProperty<string> ActivityId { get; }
        public ReactiveProperty<string> Resource { get; }
        public ReactiveProperty<string> sfcOrder { get; }
        public ReactiveProperty<string> targetOrder { get; }
        public ReactiveProperty<string> dcGroup { get; }
        public ReactiveProperty<string> dcGroupRevision { get; }
        public ReactiveProperty<modeProcessSFC> modeProcessSfc { get; }
        public ReactiveProperty<string> PackSFC { get; }
        
        public ReactiveCommand<PasswordBox> CmdReload { get; }
        public ReactiveCommand<PasswordBox> CmdSave { get; }
        public ReactiveCommand<PasswordBox> CmdTest { get; }

        public ReactiveProperty<bool> findSfcByInventory { get; } 

        public ReactiveProperty<ObjectAliasEnum> masterDataArray { get; }
        public ReactiveProperty<string> dataField { get; }
        public ReactiveProperty<string> usage { get; }
        public ReactiveProperty<string> inventory { get; }
        public ReactiveProperty<ObjectAliasEnum> category { get; } 
    }
}
