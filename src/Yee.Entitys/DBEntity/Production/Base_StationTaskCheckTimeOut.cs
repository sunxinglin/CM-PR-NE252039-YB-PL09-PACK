using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Yee.Entitys.Common;

namespace Yee.Entitys.Production
{
    [Table("Base_StationTaskCheckTimeOut")]
    public class Base_StationTaskCheckTimeOut : CommonData, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        [MaxLength(200)]
        public string TimeOutTaskName { get; set; } = "";
        public Base_StationTask? StationTask { get; set; }
        public int? StationTaskId { get; set; }

        public string? UpMesCode { get; set; }

        [Column(TypeName = ("decimal(10,3)"))]
        public decimal MinDuration { get; set; }

        [Column(TypeName = ("decimal(10,3)"))]
        public decimal MaxDuration { get; set; }

        public string TimeOutFlag { get; set; } = "";

        [NotMapped]
        private bool _HasPassed;
        [NotMapped]
        public bool HasPassed
        {
            get => _HasPassed;
            set
            {
                _HasPassed = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("HasPassed"));
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
        private Decimal _RealDuration;
        [NotMapped]
        public Decimal RealDuration
        {
            get => _RealDuration;
            set
            {
                _RealDuration = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("GluingTime"));
            }
        }

        [NotMapped]
        public DateTime? StartTime { get; set; }
        [NotMapped]
        public DateTime? CheckTime { get; set; }

        [NotMapped]
        public int? StationId { get; set; }
        [NotMapped]
        public int? StepId { get; set; }

    }
}
