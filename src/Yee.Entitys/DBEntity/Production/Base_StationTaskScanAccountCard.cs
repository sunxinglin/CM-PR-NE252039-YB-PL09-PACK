using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Yee.Entitys.Common;

namespace Yee.Entitys.Production
{
    [Table("Base_StationTaskScanAccountCard")]
    public class Base_StationTaskScanAccountCard : CommonData, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
     
        [MaxLength(200)]
        public string? ScanAccountCardName { get; set; }
        public Base_StationTask? StationTask { get; set; }
        public int? StationTaskId { get; set; }
        public string? UpMesCode { get; set; } = string.Empty;

        [NotMapped]
        public string? AccountCardValue { get; set; }

        [NotMapped]
        private bool _HasPassed;
        /// <summary>
        /// 物料校验、测试等全部通过完成
        /// </summary>
        [NotMapped]
        public bool HasPassed
        {
            get => _HasPassed;
            set
            {
                _HasPassed = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("HasPassed"));
                }
            }
        }


        [NotMapped]
        public int StationID { get; set; }
        [NotMapped]
        public int StepID { get; set; }
    }
}
