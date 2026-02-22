using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yee.Common.Library;
using Yee.Common.Library.CommonEnum;
using Yee.Entitys.BaseData;
using Yee.Entitys.Common;
using Yee.Tools;

namespace Yee.Entitys.Production
{
    [Table("Base_StationTaskScanCollect")]
    public class Base_StationTaskScanCollect : CommonData, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
     
        [MaxLength(200)]
        public string? ScanCollectName { get; set; }
        public Base_StationTask? StationTask { get; set; }
        public int? StationTaskId { get; set; }

        public string? CodeRule { get; set; } = "";

        /// <summary>
        /// 实际使用的生产资源
        /// </summary>
        [NotMapped]
        public int? UseResourceId { get; set; }


        [NotMapped]
        private bool? _HasPassed ;
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
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("HasPassed"));
                }
            }
        }

        [NotMapped]
        private string? _Status = "未完成";
        /// <summary>
        /// 颜色设置
        /// </summary>
        [NotMapped]
        public string? Status
        {
            get => _Status;
            set
            {
                _Status = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Status"));
                }
            }
        }

        /// <summary>
        /// 输入数据
        /// </summary>
        [NotMapped]
        public string? ScanInputData { get; set; }

        /// <summary>
        /// 是否需要和MES校验
        /// </summary>
        public bool? NeedValidate { get; set; }
        public string UpMesCode { get; set; }

        [NotMapped]
        public string? PackPN { get; set; }

        [NotMapped]
        public int StationID { get; set; }
        [NotMapped]
        public int StepID { get; set; }
    }
}
