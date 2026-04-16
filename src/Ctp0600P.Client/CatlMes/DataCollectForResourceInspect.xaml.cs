using System;
using System.Windows;
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
    /// DataCollectForResourceInspect.xaml 的交互逻辑
    /// </summary>
    public partial class DataCollectForResourceInspect : UserControl
    {
        public DataCollectForResourceInspect()
        {
            InitializeComponent();
        }
    }
    public class DataCollectForResourceInspectVM
    {
        private readonly CatlMesIniConfigApi _configApi;
        private readonly ICatlMesInvoker _catlMesInvoker;
        private readonly IMediator _mediator;
        public  string AutoOpName = "OP130";

        public DataCollectForResourceInspectVM(CatlMesIniConfigApi configApi, ICatlMesInvoker catlMesInvoker, IMediator mediator)
        {
            _configApi = configApi;
            _catlMesInvoker = catlMesInvoker;
            this._mediator = mediator;
            StationCode = new ReactiveProperty<string>() { Value = App._StepStationSetting.StationCode};
            Url = new ReactiveProperty<string>();
            Timeout = new ReactiveProperty<int>();
            Site = new ReactiveProperty<string>();
            User = new ReactiveProperty<string>();
            UserName = new ReactiveProperty<string>();
            Password = new ReactiveProperty<string>();
            Operation = new ReactiveProperty<string>();
            OperationRevision = new ReactiveProperty<string>();
            
            dcGroup = new ReactiveProperty<string>("*A");
            dcGroupRevision = new ReactiveProperty<string>("#");
            dcSequence = new ReactiveProperty<string>("A");
            executeMode = new ReactiveProperty<string>("FAI");
            dcMode = new ReactiveProperty<string>("AUTO_DCG");




            Resource = new ReactiveProperty<string>();
            ActivityId = new ReactiveProperty<string>();
#pragma warning disable CA1416 // 验证平台兼容性
            CmdReload = new ReactiveCommand<PasswordBox>()
                .WithSubscribe(async (passwdBox) =>
                {
                    try
                    {
                        var config = await _configApi.GetDataCollectForResourceInspectConfig(this.StationCode.Value);
                        if (config != null)
                        {
                            BindDtoToVM(config, passwdBox);
                        }
                        else
                        {
                            var notice = new MessageNotice() { messageType = MessageTypeEnum.警告, showType = MessageShowType.右下角弹窗, MessageStr = $"当前INI文件不含有{AutoOpName}_首件的配置段" };
                            await this._mediator.Publish(notice);
                  


                        }
                    }
                    catch (Exception ex)
                    {
                        var notice = new MessageNotice() { messageType = MessageTypeEnum.警告, showType = MessageShowType.右下角弹窗, MessageStr = $"{ex.Message}" };
                        await this._mediator.Publish(notice);


                    }
                });


            CmdSave = new ReactiveCommand<PasswordBox>()
                .WithSubscribe(async passwdBox =>
                {
                    try
                    {
                        if (string.IsNullOrEmpty(passwdBox.Password))
                        {
                            var notice = new MessageNotice() { messageType = MessageTypeEnum.错误, showType = MessageShowType.右下角弹窗, MessageStr = "没有配置密码，无法保存" };
                            await this._mediator.Publish(notice);
                            return;
                        }

                        var config = this.MapVMToDto(passwdBox);
                        var x = await _configApi.SetDataCollectForResourceInspectConfig(config, this.StationCode.Value);
                        if (x.Code == 0)
                        {
                            var notice = new MessageNotice() { messageType = MessageTypeEnum.提示, showType = MessageShowType.右下角弹窗, MessageStr = "保存成功" };
                            await this._mediator.Publish(notice);


                        }
                        else
                        {
                            var notice = new MessageNotice() { messageType = MessageTypeEnum.警告, showType = MessageShowType.右下角弹窗, MessageStr = $"保存失败：{x.Msg}" };
                            await this._mediator.Publish(notice);

                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"错误:{ex.Message}");

                    }
                });

            CmdTest = new ReactiveCommand<PasswordBox>()
                .WithSubscribe(async passwdBox =>
                {
                    try
                    {
                        //var reponse = await _catlMesInvoker.MICheckSfcStatusEx();
                        //if (reponse.code == 0)
                        //{
                        //    var notice = new MessageNotice() { messageType = MessageTypeEnum.提示, showType = MessageShowType.右下角弹窗, MessageStr = "测试完成" };
                        //    await this._mediator.Publish(notice);
                        //}
                        //else
                        //{
                        //    var notice = new MessageNotice() { messageType = MessageTypeEnum.警告, showType = MessageShowType.右下角弹窗, MessageStr = $"测试失败：{reponse.message}" };
                        //    await this._mediator.Publish(notice);
                        //}
                    }
                    catch (Exception ex)
                    {
                        var notice = new MessageNotice() { messageType = MessageTypeEnum.错误, showType = MessageShowType.右下角弹窗, MessageStr = $"{ex.Message}" };
                        await this._mediator.Publish(notice);
                    }
                });

        }
       
        private void BindDtoToVM(DataCollectForResourceInspectConfig section, PasswordBox passwdBox)
        {
            passwdBox.Password = section.ConnectionParams.Password;
            Url.Value = section.ConnectionParams.Url;
            UserName.Value = section.ConnectionParams.UserName;
            Timeout.Value = section.ConnectionParams.Timeout;
            Site.Value = section.InterfaceParams.Site;
            User.Value = section.InterfaceParams.User;
            Operation.Value = section.InterfaceParams.Operation;
            OperationRevision.Value = section.InterfaceParams.OperationRevision;
            dcGroup.Value = section.InterfaceParams.DcGroup;
            Resource.Value = section.InterfaceParams.Resource;
            ActivityId.Value = section.InterfaceParams.ActivityId;
            dcGroupRevision.Value = section.InterfaceParams.DcGroupRevision;
            dcMode.Value = section.InterfaceParams.DcMode;
            dcSequence.Value= section.InterfaceParams.DcSequence;
            executeMode.Value= section.InterfaceParams.ExecuteMode;
        }


        private DataCollectForResourceInspectConfig MapVMToDto(PasswordBox passwdBox)
        {
            var conn = new CatlMesConnectionParams
            {
                Url = Url.Value,
                UserName = UserName.Value,
                Password = passwdBox.Password,
                Timeout = Timeout.Value,
            };
            var inter = new DataCollectForResourceInspectParams
            {
                Site = Site.Value,
                User = User.Value,
                Operation = Operation.Value,
                OperationRevision = OperationRevision.Value,
                DcGroup = dcGroup.Value,
                DcGroupRevision = dcGroupRevision.Value,
                ActivityId = ActivityId.Value,
                Resource = Resource.Value,
                DcMode= dcMode.Value,
                DcSequence = dcSequence.Value,
                ExecuteMode= executeMode.Value,
                
            };

            return new DataCollectForResourceInspectConfig
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
   
        public ReactiveProperty<string> Resource { get; }
        public ReactiveProperty<string> ActivityId { get; }
        public ReactiveProperty<string> dcGroup { get; }
        public ReactiveProperty<string> dcGroupRevision { get; }
        public ReactiveProperty<string> dcSequence { get; }
        public ReactiveProperty<string> dcMode { get; }
        public ReactiveProperty<string> executeMode { get; }
     
        public ReactiveCommand<PasswordBox> CmdReload { get; }
        public ReactiveCommand<PasswordBox> CmdSave { get; }
        public ReactiveCommand<PasswordBox> CmdTest { get; }
    }
}
