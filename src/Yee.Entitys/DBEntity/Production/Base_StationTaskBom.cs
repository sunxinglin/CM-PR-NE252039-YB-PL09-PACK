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
    [Table("Base_StationTaskBom")]
    public class Base_StationTaskBom : CommonData, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        [MaxLength(200)]
        public string? GoodsPN { get; set; }
        [MaxLength(200)]
        public string? GoodsName { get; set; }
        public int UseNum { get; set; }


        public Base_StationTask? StationTask { get; set; }
        public int? StationTaskId { get; set; }

        /// <summary>
        /// 是否在此任务所在工位 进行MES数据收集
        /// </summary>
        public bool? NeedCollectMESData { get; set; }
        public bool HasOuterParam { get; set; }
        public OuterParamTypeEnum OuterParam1 { get; set; }
        public Decimal? MinOuterParam1 { get; set; }
        public Decimal? MaxOuterParam1 { get; set; }
        [MaxLength(200)]
        public string? UpMesCode1 { get; set; }
        public OuterParamTypeEnum OuterParam2 { get; set; }
        public Decimal? MinOuterParam2 { get; set; }
        public Decimal? MaxOuterParam2 { get; set; }
        [MaxLength(200)]
        public string? UpMesCode2 { get; set; }

        public OuterParamTypeEnum OuterParam3 { get; set; }
        public Decimal? MinOuterParam3 { get; set; }
        public Decimal? MaxOuterParam3 { get; set; }

        [MaxLength(200)]
        public string? UpMesCode3 { get; set; }


        /// <summary>
        /// 是否需要和MES校验
        /// </summary>
        public bool? NeedValidate { get; set; }

        /// <summary>
        /// 追溯类型
        /// </summary>
        public TracingTypeEnum TracingType { get; set; }

        /// <summary>
        /// 条码规则
        /// </summary>
        [MaxLength(200)]
        public string? GoodsPNRex { get; set; }

        /// <summary>
        /// 外部条码规则
        /// </summary>
        [MaxLength(200)]
        public string? OuterGoodsPNRex { get; set; }

        #region  【NotMapped】
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
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Status"));
            }
        }

        [NotMapped]
        private string? _ScanCodeText = "";
        /// <summary>
        /// 扫码
        /// </summary>
        [NotMapped]
        public string? ScanCodeText
        {
            get => _ScanCodeText;
            set
            {


                _ScanCodeText = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ScanCodeText"));
            }
        }

        [NotMapped]
        private ObservableCollection<ScanBarCode> _ScanBarCodeList = new ObservableCollection<ScanBarCode>();

        /// <summary>
        /// 扫码数据集
        /// </summary>
        [NotMapped]
        public ObservableCollection<ScanBarCode> ScanBarCodeList
        {
            get => _ScanBarCodeList;
            set
            {
                _ScanBarCodeList = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ScanBarCodeList"));
            }
        }


        [NotMapped]
        public int HasOuterParamHeight
        {
            get
            {
                if (HasOuterParam) return 45;
                else return 0;
            }
        }

        [NotMapped]
        public int PopWidth
        {
            get
            {
                if (HasOuterParam) return 250;
                else return 100;
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
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ShowPop"));
            }
        }



        [NotMapped]
        private int _CurNo;
        /// <summary>
        /// 当前物料扫码进度
        /// </summary>
        [NotMapped]
        public int CurNo
        {
            get => _CurNo;
            set
            {
                _CurNo = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurNo"));
            }
        }

        [NotMapped]
        public string? _UniBarCode = "";

        [NotMapped]
        public string? UniBarCode
        {
            get => _UniBarCode;
            set
            {
                _UniBarCode = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UniBarCode)));
            }
        }
        [NotMapped]
        public string? _GoodsOuterCode = "";

        [NotMapped]
        public string? GoodsOuterCode
        {
            get => _GoodsOuterCode;
            set
            {
                _GoodsOuterCode = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(GoodsOuterCode)));
            }
        }

        [NotMapped]
        public string? _BatchBarCode = "";
        [NotMapped]
        public string? BatchBarCode
        {
            get => _BatchBarCode;
            set
            {
                _BatchBarCode = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BatchBarCode)));
            }
        }
        #endregion


        [NotMapped]
        public int StationID { get; set; }
        [NotMapped]
        public int StepID { get; set; }
    }

    public class ScanBarCode : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string? ViewBarCode { get; set; }
        public string? UniBarCode { get; set; }
        public string? GoodsOuterCode { get; set; }
        public string? BatchBarCode { get; set; }

        [NotMapped]
        private string _OuterParam1Result;
        [NotMapped]
        public string? OuterParam1Result
        {
            get => _OuterParam1Result;
            set
            {
                _OuterParam1Result = value;
                OuterParamResult = GetOuterResultString();
            }
        }

        [NotMapped]
        private string _OuterParam2Result;
        [NotMapped]
        public string? OuterParam2Result
        {
            get => _OuterParam2Result;
            set
            {
                _OuterParam2Result = value;
                OuterParamResult = GetOuterResultString();
            }
        }

        [NotMapped]
        private string _OuterParam3Result;
        [NotMapped]
        public string? OuterParam3Result
        {
            get => _OuterParam3Result;
            set
            {
                _OuterParam3Result = value;
                OuterParamResult = GetOuterResultString();
            }
        }


        [NotMapped]
        private string _OuterParamResult = "请扫描物料码后输入";

        [NotMapped]
        public string OuterParamResult
        {
            get => _OuterParamResult;
            set
            {
                _OuterParamResult = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("OuterParamResult"));
                }
            }
        }


        private string GetUnit(OuterParamTypeEnum outerParam1)
        {
            switch (outerParam1)
            {
                case OuterParamTypeEnum.无输入:
                    return "";
                case OuterParamTypeEnum.压力:
                    return "Kpa";
                case OuterParamTypeEnum.电阻:
                    return "Ω";
                case OuterParamTypeEnum.电压:
                    return "V";
                case OuterParamTypeEnum.电流:
                    return "A";
            }
            return "";
        }

        public OuterParamTypeEnum OuterParam1 { get; set; }

        public OuterParamTypeEnum OuterParam2 { get; set; }
        public OuterParamTypeEnum OuterParam3 { get; set; }

        private string GetOuterResultString()
        {
            string resultContent = string.Empty;
            if (!string.IsNullOrEmpty(OuterParam1Result))
                resultContent += "  " + $"{OuterParam1}：" + OuterParam1Result + " " + GetUnit(OuterParam1);
            if (!string.IsNullOrEmpty(OuterParam2Result))
                resultContent += "  " + $"{OuterParam2}：" + OuterParam2Result + " " + GetUnit(OuterParam2);
            if (!string.IsNullOrEmpty(OuterParam3Result))
                resultContent += "  " + $"{OuterParam3}：" + OuterParam3Result + " " + GetUnit(OuterParam3);
            return resultContent;
        }
    }
}
