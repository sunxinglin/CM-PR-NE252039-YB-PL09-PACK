using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Yee.Entitys.Common;

namespace Yee.Entitys.Production
{
    [Table("Base_StationTaskUserInput")]
    public class Base_StationTaskUserInput : CommonData, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [MaxLength(200)]
        public string? UserScanName { get; set; }
        public Base_StationTask? StationTask { get; set; }
        public int? StationTaskId { get; set; }

        /// <summary>
        /// 实际使用的生产资源
        /// </summary>
        [NotMapped]
        public int? UseResourceId { get; set; }


        [NotMapped]
        private bool? _HasPassed;
        /// <summary>
        /// 物料校验、测试等全部通过完成
        /// </summary>
        [NotMapped]
        public bool? HasPassed
        {
            get => _HasPassed;
            set
            {
                _HasPassed = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("HasPassed"));
            }
        }

        [NotMapped]
        private string? _Status = "未完成";
        /// <summary>
        /// 物料校验、测试等全部通过完成
        /// </summary>
        [NotMapped]
        public string? Status
        {
            get => _Status;
            set
            {
                _Status = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Status"));
            }
        }

        /// <summary>
        /// 输入数据
        /// </summary>
        [NotMapped]
        public string? ScanData { get; set; }

        /// <summary>
        /// 最大范围
        /// </summary>
        public Decimal? MaxRange { get; set; }

        /// <summary>
        /// 最小范围
        /// </summary>
        public Decimal? MinRange { get; set; }

        public string UpMesCode { get; set; } = "";

        [NotMapped]
        public string? PackPN { get; set; }

        [NotMapped]
        public int StationID { get; set; }
        [NotMapped]
        public int StepID { get; set; }
    }
}
