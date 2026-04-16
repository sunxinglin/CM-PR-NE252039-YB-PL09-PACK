using System;

using Ctp0600P.Client.ViewModels.StationTaskViewModels;

using MediatR;

namespace Ctp0600P.Client.Views.StationTaskPages;

public partial class RepairBoltGunCommon : StationTaskCommonPage
{

    public RepairBoltGunCommon(RepairBoltGun_CommonViewModel vm, IServiceProvider service, IMediator mediator) : base(service, mediator)
    {
        InitializeComponent();
        _VM = vm;
        this.DataContext = _VM;
    }
}