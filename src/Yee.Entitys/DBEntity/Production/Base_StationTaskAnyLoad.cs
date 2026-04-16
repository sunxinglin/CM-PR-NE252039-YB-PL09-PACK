using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Yee.Entitys.Common;

namespace Yee.Entitys.Production
{
    [Table("Base_StationTaskAnyLoad")]
    public class Base_StationTaskAnyLoad : CommonData, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
     
        [MaxLength(200)]
        public string AnyLoadName { get; set; }
        public Base_StationTask? StationTask { get; set; }
        public int? StationTaskId { get; set; }

        /// <summary>
        /// 实际使用的生产资源
        /// </summary>
        [NotMapped]
        public int? UseResourceId { get; set; }


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

        /// <summary>
        /// 重量
        /// </summary>
        [NotMapped]
        public decimal WeightData { get; set; }

        /// <summary>
        /// 最大重量
        /// </summary>
        public decimal? MaxWeight { get; set; }

        /// <summary>
        /// 最小重量
        /// </summary>
        public decimal? MinWeight { get; set; }

        /// <summary>
        /// 是否需要和MES校验
        /// </summary>
        public bool? NeedValidate { get; set; }
        public string UpMesCode { get; set; }

        [NotMapped]
        public int StationID { get; set; }
        [NotMapped]
        public int StepID { get; set; }
    }
}
