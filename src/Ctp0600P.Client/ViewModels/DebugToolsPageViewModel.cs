using System.ComponentModel;
using System.Reactive.Linq;

using Ctp0600P.Client.UserControls.DebugTools;

using Reactive.Bindings;

namespace Ctp0600P.Client.ViewModels
{
    public class DebugToolsPageViewModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public DebugToolsPageViewModel( ElectricScrewDriverUserControlVM electricScrewDriverUserControlVM)
        {
            this.ElectricScrewDriverUserControlVM = new ReactiveProperty<ElectricScrewDriverUserControlVM>(electricScrewDriverUserControlVM);
            this.ScanCodeText = App.ScanCodeGunRequestSubject.Select(s => s.ScanCodeContext).ToReactiveProperty();
            this.ScanCodeTextLength = App.ScanCodeGunRequestSubject.Select(s => s.ScanCodeContext.Length).ToReactiveProperty();
        }

        public ReactiveProperty<ElectricScrewDriverUserControlVM> ElectricScrewDriverUserControlVM { get; }
        public ReactiveProperty<string> ScanCodeText { get; }
        public ReactiveProperty<int> ScanCodeTextLength { get; }
       
    }
}
