using Ctp0600P.Shared.CatlMes;
using Ctp0600P.Client.Apis;
using Reactive.Bindings;
using System;
using System.Windows;
using System.Windows.Controls;
using MediatR;
using Ctp0600P.Client.Command.NoticeHelp.Handle;
using Ctp0600P.Client.Command.NoticeHelp.Enum;
using Catl.WebServices.MICheckSFCStatusEx;
using Catl.WebServices.AssembleAndCollectDataForSfc;
using Yee.Services.CatlMesInvoker;
using NPOI.SS.Formula.Functions;

namespace Ctp0600P.Client.CatlMes
{
    /// <summary>
    /// Interaction logic for MiAssembleAndCollectDataForSfcCtrl.xaml
    /// </summary>
    public partial class MiAssembleAndCollectDataForSfcCtrl : UserControl
    {
        public MiAssembleAndCollectDataForSfcCtrl()
        {
            InitializeComponent();
        }
    }


    public class MiAssembleAndCollectDataForSfcCtrlVM
    {
        private readonly CatlMesIniConfigApi _configApi;
        private readonly ICatlMesInvoker _catlMesInvoker;
        private readonly IMediator mediator;

        public MiAssembleAndCollectDataForSfcCtrlVM(CatlMesIniConfigApi configApi, ICatlMesInvoker catlMesInvoker, IMediator mediator)
        {
            this._configApi = configApi;
            this._catlMesInvoker = catlMesInvoker;
            this.mediator = mediator;
            this.StationCode = new ReactiveProperty<string>() {  Value = App._StepStationSetting.StationCode };
            this.Url = new ReactiveProperty<string>();
            this.Timeout = new ReactiveProperty<int>();
            this.Site = new ReactiveProperty<string>();
            this.User = new ReactiveProperty<string>();
            this.UserName = new ReactiveProperty<string>();
            this.Password = new ReactiveProperty<string>();
            this.Operation = new ReactiveProperty<string>();
            this.OperationRevision = new ReactiveProperty<string>();
            this.sfc = new ReactiveProperty<string>();
            this.modeProcessSFC = new ReactiveProperty<dataCollectForSfcModeProcessSfc>();
            this.partialAssembly = new ReactiveProperty<bool>();
            this.Resource = new ReactiveProperty<string>();
            this.ActivityId = new ReactiveProperty<string>();
            this.dcGroup = new ReactiveProperty<string>("*A");
            this.dcGroupRevision = new ReactiveProperty<string>("#");
#pragma warning disable CA1416 // 验证平台兼容性

            this.CmdReload = new ReactiveCommand<PasswordBox>()
                .WithSubscribe(async (passwdBox) =>
                {
                    try
                    {
                        var config = await this._configApi.GetMiAssembleAndCollectDataForSfcConfig(this.StationCode.Value);
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
                        var x = await this._configApi.SetMiAssembleAndCollectDataForSfcConfig(config, this.StationCode.Value);
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
                        //var reponse = await this._catlMesInvoker.MICheckSfcStatusEx();
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

        private void BindDtoToVM(MIAssembleAndCollectDataForSfcConfig section, PasswordBox passwdBox)
        {
            passwdBox.Password = section.ConnectionParams.Password;
            this.Url.Value = section.ConnectionParams.Url;
            this.UserName.Value = section.ConnectionParams.UserName;
            this.Timeout.Value = section.ConnectionParams.Timeout;
            this.Site.Value = section.InterfaceParams.Site;
            this.User.Value = section.InterfaceParams.User;
            this.Operation.Value = section.InterfaceParams.Operation;
            this.OperationRevision.Value = section.InterfaceParams.OperationRevision;

            this.sfc.Value = section.InterfaceParams.sfc;
            this.modeProcessSFC.Value = section.InterfaceParams.modeProcessSFC;
            this.partialAssembly.Value = section.InterfaceParams.partialAssembly;
            this.Resource.Value = section.InterfaceParams.resource;
            this.ActivityId.Value = section.InterfaceParams.ActivityId;
            this.dcGroupRevision.Value = section.InterfaceParams.dcGroupRevision;
            this.dcGroup.Value = section.InterfaceParams.dcGroup;
        }


        private MIAssembleAndCollectDataForSfcConfig MapVMToDto(PasswordBox passwdBox)
        {
            var conn = new CatlMesConnectionParams
            {
                Url = this.Url.Value,
                UserName = this.UserName.Value,
                Password = passwdBox.Password,
                Timeout = this.Timeout.Value,
            };
            var inter = new MIAssembleAndCollectDataForSfcParams
            {
                Site = this.Site.Value,
                sfc = this.sfc.Value,
                Operation = this.Operation.Value,
                OperationRevision = this.OperationRevision.Value,
                ActivityId = this.ActivityId.Value,
                partialAssembly = this.partialAssembly.Value,
                modeProcessSFC = this.modeProcessSFC.Value,
                resource = this.Resource.Value,
                User = this.User.Value,
                dcGroup = this.dcGroup.Value,
                dcGroupRevision = this.dcGroupRevision.Value,
            };

            return new MIAssembleAndCollectDataForSfcConfig
            {
                ConnectionParams = conn,
                InterfaceParams = inter,
            };

        }


        public ReactiveProperty<string> StationCode { get; }
        public ReactiveProperty<string> Url { get; }
        public ReactiveProperty<string> UserName { get; }
        public ReactiveProperty<string> Password { get; }

        public ReactiveProperty<int> Timeout { get; }
        public ReactiveProperty<string> Site { get; }
        public ReactiveProperty<string> User { get; }
        public ReactiveProperty<string> Operation { get; }
        public ReactiveProperty<string> OperationRevision { get; }
        public ReactiveProperty<string> sfc { get; }
        public ReactiveProperty<dataCollectForSfcModeProcessSfc> modeProcessSFC { get; }
        public ReactiveProperty<bool> partialAssembly { get; }
        public ReactiveProperty<string> Resource { get; }
        public ReactiveProperty<string> ActivityId { get; }
        public ReactiveProperty<string> dcGroup { get; }
        public ReactiveProperty<string> dcGroupRevision { get; }
        public ReactiveCommand<PasswordBox> CmdReload { get; }
        public ReactiveCommand<PasswordBox> CmdSave { get; }
        public ReactiveCommand<PasswordBox> CmdTest { get; }
    }
}
