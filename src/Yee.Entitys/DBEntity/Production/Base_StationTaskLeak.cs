﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Yee.Entitys.DBEntity;

namespace Yee.Entitys.Production
{
    [Table("Base_StationTaskLeak")]
    public class Base_StationTaskLeak : CommonData, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public Base_StationTask? StationTask { get; set; }
        public int StationTaskId { get; set; }
        public int? LeakTimes { get; set; }
        public int? KeepTimes { get; set; }
        public decimal? LeakPress { get; set; }
        public decimal? KeepPress { get; set; }
        public string? ParameterName { get; set; }
        public string? UpMesCodePN { get; set; }

        [NotMapped]
        public string? ParamValue { get; set; }

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