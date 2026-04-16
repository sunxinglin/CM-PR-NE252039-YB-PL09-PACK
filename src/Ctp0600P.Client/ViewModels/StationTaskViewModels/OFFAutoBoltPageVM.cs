using System.Collections.ObjectModel;
using System.Windows.Input;

using FutureTech.Mvvm;

using MediatR;

using Microsoft.Xaml.Behaviors.Core;

using Yee.Entitys.Production;

namespace Ctp0600P.Client.ViewModels.StationTaskViewModels
{
    public class OFFAutoBoltPageVM : ViewModelBase
    {
        private readonly IMediator mediator;

        public OFFAutoBoltPageVM(IMediator mediator)
        {
            this.mediator = mediator;

            //foreach (var bolt in _DesoutterMgr.DesoutterCtrls.Values)
            //{
            //    StationBoltGunList.Add(new BoltOffAutoDTO
            //    {
            //        DeviceNo=bolt.DeviceNo,
            //        ProgramNo=0,
            //        IsSelected=false
            //    });
            //}
        }
        public ObservableCollection<BoltOffAutoDTO> StationBoltGunList
        {
            get => _StationBoltGunList;
            set
            {
                if (_StationBoltGunList != value)
                {
                    _StationBoltGunList = value;
                    OnPropertyChanged(nameof(StationBoltGunList));
                }
            }
        }
        private ObservableCollection<BoltOffAutoDTO> _StationBoltGunList = new ObservableCollection<BoltOffAutoDTO>();

        #region 【拧紧枪 使能 去使能】

        public void EnableBoltGuns()
        {
            //foreach (var screwCtrl in _DesoutterMgr.DesoutterCtrls)
            //{
            //    if (!string.IsNullOrEmpty(screwCtrl.Value.DeviceNo))
            //    {
            //        var task = _StationBoltGunList.FirstOrDefault(s => s.DeviceNo == screwCtrl.Value.DeviceNo);
            //        if (task.IsSelected)
            //        {
            //            screwCtrl.Value.Enable(task.ProgramNo);
            //        }
            //    }
            //}
        }

        public void DisableBoltGuns(Base_StationTaskScrew screw)
        {
            //foreach (var screwCtrl in _DesoutterMgr.DesoutterCtrls)
            //{
            //    if (screwCtrl.Value != null && !string.IsNullOrEmpty(screwCtrl.Value.DeviceNo))
            //        if (screw.DeviceNos.Contains(screwCtrl.Value.DeviceNo))
            //            screwCtrl.Value.Disable();
            //}
        }

        #endregion

        private ActionCommand save;
        public ICommand Save => save ??= new ActionCommand(PerformSave);

        private void PerformSave()
        {
        }

        /// <summary>
        /// 保存拧紧枪配置
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        private void SaveBoltGunConifgData()
        {

        }


    }

    public class BoltOffAutoDTO
    {
        public string DeviceNo { get; set; }
        public int ProgramNo { get; set; }
        public bool IsSelected { get; set; }
    }
}
