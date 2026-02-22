using System;
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

namespace Yee.Entitys.Production
{
    [Table("Base_StationTaskScrew")]
    public class Base_StationTaskScrew : CommonData, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 螺丝规格
        /// </summary>
        [MaxLength(200)]
        public string? ScrewSpecs { get; set; }


        /// <summary>
        /// 设备号
        /// </summary>
        [MaxLength(200)]
        public string? DeviceNos
        {
            get { return deviceNos; }
            set
            {
                deviceNos = value;
            }
        }
        private string? deviceNos;

        [NotMapped]
        public List<string>? DeviceNoList { get; set; }

        /// <summary>
        /// 安装数
        /// </summary>
        public int UseNum { get; set; }

        /// <summary>
        /// 上传代码计数开始数值
        /// </summary>
        public int UpMESCodeStartNo { get; set; }


        /// <summary>
        /// 所属任务
        /// </summary>
        public Base_StationTask? StationTask { get; set; }
        public int? StationTaskId { get; set; }


        #region 【拧紧任务时可用】

        /// <summary>
        /// 上传MES代码
        /// </summary>
        public string? UpMesCodePN { get; set; }

        /// <summary>
        /// 程序号
        /// </summary>
        public int? ProgramNo { get; set; }

        /// <summary>
        /// 套筒号
        /// </summary>
        public int? TaoTongNo { get; set; }


        /// <summary>
        /// 返工限制次数
        /// </summary>
        public int? ReworkLimitTimes { get; set; }

        /// <summary>
        /// 扭矩下限
        /// </summary>
        public decimal? TorqueMinLimit { get; set; }
        /// <summary>
        /// 扭矩上限
        /// </summary>
        public decimal? TorqueMaxLimit { get; set; }

        /// <summary>
        /// 角度下限
        /// </summary>
        public decimal? AngleMinLimit { get; set; }
        /// <summary>
        /// 角度上限
        /// </summary>
        public decimal? AngleMaxLimit { get; set; }

        #endregion



        /// <summary>
        /// 实际使用的生产资源
        /// </summary>
        [NotMapped]
        public int? UseResourceId { get; set; }

        [NotMapped]
        private string _Status = "未完成";
        /// <summary>
        /// 颜色设置
        /// </summary>
        [NotMapped]
        public string Status
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
        /// 物料校验、测试等全部通过完成
        /// </summary>
        [NotMapped]
        public bool HasPassed { get; set; }

        /// <summary>
        /// 当前螺丝扭力
        /// </summary>
        [NotMapped]
        private string _CurScrewValueStr;
        [NotMapped]
        public string? CurScrewValueStr
        {
            get => _CurScrewValueStr;
            set
            {
                _CurScrewValueStr = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("CurScrewValueStr"));
                }
            }
        }
        [NotMapped]
        private string _CurFinalAngleStr;
        [NotMapped]
        public string? CurFinalAngleStr
        {
            get => _CurFinalAngleStr;
            set
            {
                _CurFinalAngleStr = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("CurFinalAngleStr"));
                }
            }
        }
        [NotMapped]
        private string _CurAngle_MaxStr;
        [NotMapped]
        public string? CurAngle_MaxStr
        {
            get => _CurAngle_MaxStr;
            set
            {
                _CurAngle_MaxStr = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("CurAngle_MaxStr"));
                }
            }
        }
        [NotMapped]
        private string _CurAngle_MinStr;
        [NotMapped]
        public string? CurAngle_MinStr
        {
            get => _CurAngle_MinStr;
            set
            {
                _CurAngle_MinStr = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("CurAngle_MinStr"));
                }
            }
        }


        [NotMapped]
        public decimal CurScrewValue { get; set; }


        /// <summary>
        /// 当前螺丝状态 NG OK
        /// </summary>
        [NotMapped]
        private string _CurScrewState;

        [NotMapped]
        public string? CurScrewState
        {
            get => _CurScrewState;
            set
            {
                _CurScrewState = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("CurScrewState"));
                }
            }
        }

        [NotMapped]
        public decimal CurAngle_Max { get; set; }
        [NotMapped]
        public decimal CurAngle_Min { get; set; }
        [NotMapped]
        public int CurAngleStatus { get; set; }
        [NotMapped]
        public decimal CurTargetAngle { get; set; }
        [NotMapped]
        public decimal CurFinalAngle { get; set; }
        [NotMapped]
        public decimal CurFinalTorque { get; set; }
        [NotMapped]
        public decimal CurTargetTorqueRate { get; set; }
        [NotMapped]
        public decimal CurTorqueRate_Max { get; set; }
        [NotMapped]
        public decimal CurTorqueRate_Min { get; set; }
        [NotMapped]
        public int CurTorqueStatus { get; set; }
        [NotMapped]
        public bool ResultIsOK { get; set; }
        [NotMapped]
        public int DeviceNo { get; set; }

        [NotMapped]
        private int _CurNo;

        /// <summary>
        /// 当前螺丝进度
        /// </summary>
        [NotMapped]
        public int CurNo
        {
            get => _CurNo;
            set
            {
                _CurNo = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("CurNo"));
                }
            }
        }

        [NotMapped]
        private List<ScrewResult> _ScrewResultList = new List<ScrewResult>();

        /// <summary>
        /// 螺丝拧紧数据集
        /// </summary>
        [NotMapped]
        public List<ScrewResult> ScrewResultList
        {
            get => _ScrewResultList;
            set
            {
                _ScrewResultList = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("ScrewResultList"));
                }
            }
        }

        /// <summary>
        /// 是否显示Pop
        /// </summary>
        [NotMapped]
        private bool _ShowPop;
        [NotMapped]
        public bool ShowPop
        {
            get => _ShowPop;
            set
            {
                _ShowPop = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("ShowPop"));
                }
            }
        }


        [NotMapped]
        public int StationID { get; set; }
        [NotMapped]
        public int StepID { get; set; }

        [NotMapped]
        private List<int> _NeedReWordSource  = new List<int>();
        [NotMapped]
        public List<int> NeedReWordSource
        {
            get => _NeedReWordSource;
            set
            {
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("NeedReWordSource"));
                }

                if (_NeedReWordSource != value)
                {
                    _NeedReWordSource = value;
                    ReWorkScrewNum = _NeedReWordSource.Count;
                }
            }
        }


        [NotMapped]
        private int _ReWorkScrewNum;
        [NotMapped]
        public int ReWorkScrewNum
        {
            get => _ReWorkScrewNum;
            set
            {
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("ReWorkScrewNum"));
                }

                if (_ReWorkScrewNum != value)
                {
                    _ReWorkScrewNum = value;
                }
            }
        }

    }

    public class ScrewResult
    {
        public string DeviceNo { get; set; }
        public string ScrewValue { get; set; }
        public string ScrewState { get; set; }

    }
}
