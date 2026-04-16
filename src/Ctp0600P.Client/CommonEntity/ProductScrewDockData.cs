using System.Collections.Generic;
using System.Windows;

using FutureTech.Mvvm;

namespace Ctp0600P.Client.CommonEntity;

public class ProductScrewDockData
{
    public string ProductType { get; set; }
    public string StepCode { get; set; }

    public string ImageUrl { get; set; }
    public int LayOut_Width { get; set; }
    public int LayOut_Height { get; set; }
    public int LimitCount { get; set; }
    public List<ScrewLocation> ScrewLocationList { get; set; }

}

public class ScrewLocation : ViewModelBase
{
    public int OrderNo { get; set; }
    public ThicknessSelf Margin { get; set; }
    public Thickness MarginSelf { get; set; }
    private string _Status;
    public string Status
    {
        get => _Status;
        set
        {
            this._Status = value;
            OnPropertyChanged(nameof(Status));
        }
    }
}

public class ThicknessSelf
{
    public double Left { get; set; }
    public double Top { get; set; }
    public double Right { get; set; }
    public double Bottom { get; set; }
}