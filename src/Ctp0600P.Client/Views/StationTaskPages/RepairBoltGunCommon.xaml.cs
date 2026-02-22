using Ctp0600P.Client.ViewModels.StationTaskViewModels;
using MediatR;
using System;

namespace Ctp0600P.Client.Views.StationTaskPages
{
    /// <summary>
    /// AnyLoad.xaml 的交互逻辑
    /// </summary>
    public partial class RepairBoltGunCommon : StationTaskCommonPage
    {

        public RepairBoltGunCommon(RepairBoltGun_CommonViewModel vm, IServiceProvider service, IMediator mediator) : base(service, mediator)
        {
            InitializeComponent();
            _VM = vm;
            this.DataContext = _VM;



        }
    }
}
