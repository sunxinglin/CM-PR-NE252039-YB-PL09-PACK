using System.Windows;
using System.Windows.Controls;

using Ctp0600P.Client.ViewModels;

using Yee.Entitys;

namespace Ctp0600P.Client.Views.StationTaskPages;

/// <summary>
/// BoltGun.xaml 的交互逻辑
/// </summary>
public partial class ReWorkPage : Window
{
    private readonly ReWorkPageViewModel _vm;
    public ReWorkPage(ReWorkPageViewModel VM)
    {
        InitializeComponent();
        this._vm = VM;
        this.DataContext = VM;
    }

    private void Button_Click_Cancle(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        this._vm.SetRework.Execute(this);
        this.Close();
    }

    private void Button_Click_1(object sender, RoutedEventArgs e)
    {
        var value = (ReWorkData)((Button)sender).DataContext;
        this._vm.SetWorking.Execute(value);
    }

    private void Button_Click_ChooseAll(object sender, RoutedEventArgs e)
    {
        this._vm.ReWorkDatas.Value.ForEach(f => f.WorkRecords.ForEach(fo => fo.IsChecked = true));
    }
}