using System;
using System.Windows.Controls;

using Ctp0600P.Client.Apis;
using Ctp0600P.Client.Command.NoticeHelp.Enum;
using Ctp0600P.Client.Command.NoticeHelp.Handle;
using Ctp0600P.Shared.CatlMes;

using MediatR;

using Reactive.Bindings;

using Yee.Services.CatlMesInvoker;

namespace Ctp0600P.Client.CatlMes
{
    /// <summary>
    /// MiCheckSFCStatusEx.xaml 的交互逻辑
    /// </summary>
    public partial class MiCheckSFCStatusEx : UserControl
    {
        public MiCheckSFCStatusEx()
        {
            InitializeComponent();
        }
    }

    public class MiCheckSFCStatusExVM
    {
        private readonly CatlMesIniConfigApi _configApi;
        private readonly ICatlMesInvoker _catlMesInvoker;
        private readonly IMediator mediator;

        public MiCheckSFCStatusExVM(CatlMesIniConfigApi configApi, ICatlMesInvoker catlMesInvoker, IMediator mediator)
        {
            this._configApi = configApi;
            this._catlMesInvoker = catlMesInvoker;
            this.mediator = mediator;
            this.Url = new ReactiveProperty<string>();
            this.Timeout = new ReactiveProperty<int>();
            this.Site = new ReactiveProperty<string>();
            this.UserName = new ReactiveProperty<string>();
            this.Password = new ReactiveProperty<string>();
            this.Operation = new ReactiveProperty<string>();
            this.OperationRevision = new ReactiveProperty<string>();
            this.Resource = new ReactiveProperty<string>();

            this.StationCode = new ReactiveProperty<string>() { Value = App._StepStationSetting.StationCode };
#pragma warning disable CA1416 // 验证平台兼容性
            this.CmdReload = new ReactiveCommand<PasswordBox>()
                .WithSubscribe(async (passwdBox) =>
                {
                    try
                    {
                        var config = await this._configApi.GetMICheckSfcStatusExConfig(this.StationCode.Value);
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
                        var x = await this._configApi.SetMICheckSfcStatusExConfig(config, this.StationCode.Value);
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
#pragma warning restore CA1416 // 验证平台兼容性
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

        private void BindDtoToVM(MICheckSFCStatusExConfig section, PasswordBox passwdBox)
        {
            passwdBox.Password = section.ConnectionParams.Password;
            this.Url.Value = section.ConnectionParams.Url;
            this.UserName.Value = section.ConnectionParams.UserName;
            this.Timeout.Value = section.ConnectionParams.Timeout;
            this.Site.Value = section.InterfaceParams.Site;
            this.Operation.Value = section.InterfaceParams.Operation;
            this.OperationRevision.Value = section.InterfaceParams.OperationRevision;
        }


        private MICheckSFCStatusExConfig MapVMToDto(PasswordBox passwdBox)
        {
            var conn = new CatlMesConnectionParams
            {
                Url = this.Url.Value,
                UserName = this.UserName.Value,
                Password = passwdBox.Password,
                Timeout = this.Timeout.Value,
            };
            var inter = new MICheckSFCStatusExParams
            {
                Site = this.Site.Value,
                Operation = this.Operation.Value,
                OperationRevision = this.OperationRevision.Value,
            };

            return new MICheckSFCStatusExConfig
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
     
        public ReactiveProperty<string> Resource { get; }
        public ReactiveProperty<string> ActivityId { get; }
        public ReactiveProperty<string> dcGroup { get; }
        public ReactiveProperty<string> dcGroupRevision { get; }
        public ReactiveCommand<PasswordBox> CmdReload { get; }
        public ReactiveCommand<PasswordBox> CmdSave { get; }
        public ReactiveCommand<PasswordBox> CmdTest { get; }
    }
}
