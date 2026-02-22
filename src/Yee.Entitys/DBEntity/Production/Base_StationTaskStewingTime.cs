using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Yee.Entitys.Common;

namespace Yee.Entitys.Production
{
    /// <summary>
    /// 工序时间差
    /// </summary>
    [Table("Base_StationTaskStewingTime")]
    public class Base_StationTaskStewingTime : CommonData, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [MaxLength(200)]
        public string TaskStewingTimedName { get; set; }
        public string TaskEndStepCode { get; set; }

        public Base_StationTask? StationTask { get; set; }
        public int? StationTaskId { get; set; }

     


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
        private bool _HasFinish = false;

        [NotMapped]
        public bool HasFinish
        {
            get => _HasFinish;
            set
            {
                _HasFinish = value;
            }
        }


        [NotMapped]
        private Decimal _StewingTime;
        [NotMapped]
        public Decimal StewingTime
        {
            get => _StewingTime;
            set
            {
                _StewingTime = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("StewingTime"));
                }
            }
        }
        /// </summary>
        /// <summary>
        /// 静置最短时间
        /// </summary>
        public decimal MinTime { get; set; }

        public string? UpMesCode { get; set; }

        [NotMapped]
        public DateTime? StewingStartTime { get; set; }
        [NotMapped]
        public DateTime? StewingCollectTime { get; set; }

        [NotMapped]
        public int? StationId { get; set; }
        [NotMapped]
        public int? StepId { get; set; }

    }
}
