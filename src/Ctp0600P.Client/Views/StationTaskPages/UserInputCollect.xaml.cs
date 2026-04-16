using Ctp0600P.Client.ViewModels.StationTaskViewModels;

namespace Ctp0600P.Client.Views.StationTaskPages;

public partial class UserInputCollect : StationTaskCommonPage
{
    private readonly UserInputCollectViewModel _Vm;

    public UserInputCollect(UserInputCollectViewModel Vm)
    {
        InitializeComponent();
        _Vm = Vm;
        this.DataContext = _Vm;
    }
}