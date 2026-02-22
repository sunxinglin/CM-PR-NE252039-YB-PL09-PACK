using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yee.Common.Library;
using Yee.Common.Library.CommonEnum;
using Yee.Entitys.BaseData;
using Yee.Entitys.DBEntity.Production;
using Yee.Entitys.Production;
using Yee.Tools;

namespace Yee.Entitys.DTOS
{
    /// <summary>
    /// 网页端导入工位任务类
    /// </summary>
    public class StationTaskExcel
    {


        [ExcelToEntity(1)]
        [EntityToexcel("工艺编码", 1)]
        public string? FlowCode { get; set; }
 
        [ExcelToEntity(2)]
        [EntityToexcel("产品PN", 2)]
        public string ProductCode { get; set; } = "";

        [ExcelToEntity(3)]
        [EntityToexcel("工序", 3)]
        public string StepCode { get; set; } = "";


        [ExcelToEntity(4)]
        [EntityToexcel("步序", 4)]
        public int Sequence { get; set; }

        [ExcelToEntity(5)]
        [EntityToexcel("任务名称", 5)]
        public string TaskName { get; set; } = "";

        [ExcelToEntity(6)]
        [EntityToexcel("任务类型", 6)]
        public StationTaskTypeEnum Type { get; set; }
     
        [ExcelToEntity(7)]
        [EntityToexcel("节拍",7)]
        public int? Clock { get; set; }
      
        //public Boolean? HasPage { get { return true; } set {  } }
        
        [ExcelToEntity(8)]
        [EntityToexcel("返工限制次数", 8)]
        public int? ReworkLimitTimes { get; set; }
  
        [ExcelToEntity(9)]
        [EntityToexcel("使用数量", 9)]
        public int? UseNum { get; set; }

        [ExcelToEntity(10)]
        [EntityToexcel("上传代码", 10)]
        public string? UpMesCode { get; set; }

        [ExcelToEntity(11)]
        [EntityToexcel("任务详情名称", 11)]
        public string? TaskDetailName {  get; set; }

        #region 扫码
        [ExcelToEntity(12)]
        [EntityToexcel("物料PN", 12)]
        public string? TaskBom_GoodsPN { get; set; }
        [ExcelToEntity(13)]
        [EntityToexcel("追溯类型", 13)]
        public TracingTypeEnum TracingType { get; set; }

        [ExcelToEntity(14)]
        [EntityToexcel("内部条码规则", 14)]
        public string? TaskBom_GoodsPNRex { get; set; }

        [ExcelToEntity(15)]
        [EntityToexcel("外部条码规则", 15)]
        public string? TaskBom_OuterGoodsPNRex { get; set; }

        [ExcelToEntity(16)]
        [EntityToexcel("是否合并上传MES数据", 16)]
        public bool? NeedCollectMESData { get; set; }
        #endregion

        #region 拧紧枪

        [ExcelToEntity(17)]
        [EntityToexcel("拧紧程序号", 17)]
        public int? TaskScrew_ProgramNo { get; set; }

        [ExcelToEntity(18)]
        [EntityToexcel("拧紧枪号", 18)]
        public string? TaskScrew_DeviceNos { get; set; }
     
       
        [ExcelToEntity(19)]
        [EntityToexcel("套筒号", 19)]
        public int? TaskScrew_TaoTongNo { get; set; }

        [ExcelToEntity(20)]
        [EntityToexcel("扭矩下限", 20)]
        public decimal? TaskScrew_TorqueMinLimit { get; set; }
        [ExcelToEntity(21)]
        [EntityToexcel("扭矩上限", 21)]
        public decimal? TaskScrew_TorqueMaxLimit { get; set; }


        [ExcelToEntity(22)]
        [EntityToexcel("角度下限", 22)]
        public decimal? TaskScrew_AngleMinLimit { get; set; }


        [ExcelToEntity(23)]
        [EntityToexcel("角度上限", 23)]
        public decimal? TaskScrew_AngleMaxLimit { get; set; }

        
        [ExcelToEntity(24)]
        [EntityToexcel("拧紧上传代码计数开始数值", 24)]
        public int? TaskScrew_UpMESCodeStartNo { get; set; }
        #endregion

        #region 补拧
        [ExcelToEntity(25)]
        [EntityToexcel("补拧类型", 25)]
        public TightenReworkType? TightenReworkType { get; set; }

        [ExcelToEntity(26)]
        [EntityToexcel("补拧程序号", 26)]
        public int? TaskRepair_ProgramNo { get; set; }

        [ExcelToEntity(27)]
        [EntityToexcel("补拧枪号", 27)]
        public string? TaskRepair_DeviceNos { get; set; }

        [ExcelToEntity(28)]
        [EntityToexcel("补拧扭矩下限", 28)]
        public decimal? TaskRepair_TorqueMinLimit { get; set; }
        [ExcelToEntity(29)]
        [EntityToexcel("补拧扭矩上限", 29)]
        public decimal? TaskRepair_TorqueMaxLimit { get; set; }

         [ExcelToEntity(30)]
        [EntityToexcel("补拧角度下限", 30)]
        public decimal? TaskRepair_AngleMinLimit { get; set; }

        [ExcelToEntity(31)]
        [EntityToexcel("补拧角度上限", 31)]
        public decimal? TaskRepair_AngleMaxLimit { get; set; }

       
        #endregion

        #region 扫码输入
        [ExcelToEntity(32)]
        [EntityToexcel("扫码输入规则", 32)]
        public string? SacnCollectRule { get; set; }
        #endregion


        #region 用户输入

        [ExcelToEntity(33)]
        [EntityToexcel("用户输入最小值", 33)]
        public decimal? TaskUserInput_MinRange { get; set; }

        [ExcelToEntity(34)]
        [EntityToexcel("用户输入最大值", 34)]
        public decimal? TaskUserInput_MaxRange { get; set; }
        #endregion

        #region 时间记录

        [ExcelToEntity(35)]
        [EntityToexcel("时间记录标志", 35)]
        public string TimeFlag { get; set; } = "";
        #endregion


        #region 超时检测

        [ExcelToEntity(36)]
        [EntityToexcel("超时检测标志", 36)]
        public string TimeoutFlag { get; set; } = "";
        [ExcelToEntity(37)]
        [EntityToexcel("最小超时时间", 37)]
        public decimal? TimeoutMin { get; set; }

        [ExcelToEntity(38)]
        [EntityToexcel("最大超时时间", 38)]
        public decimal? TimeoutMax { get; set; }
        #endregion

        #region 称重
        [ExcelToEntity(39)]
        [EntityToexcel("最小重量", 39)]
        public decimal? TaskAnyLoad_MinWeight { get; set; }
        [ExcelToEntity(40)]
        [EntityToexcel("最大重量", 40)]
        public decimal? TaskAnyLoad_MaxWeight { get; set; }
        #endregion

        #region 自动涂胶
        [ExcelToEntity(41)]
        [EntityToexcel("涂胶数据类型", 41)]
        public GlueType? GlueType { get; set; }

        [ExcelToEntity(42)]
        [EntityToexcel("涂胶位置", 42)]
        public int? GlueLocate { get; set; }

        [ExcelToEntity(43)]
        [EntityToexcel("涂胶数据下限", 43)]
        public decimal? GlueMin { get; set; }

        [ExcelToEntity(44)]
        [EntityToexcel("涂胶数据上限", 44)]
        public decimal? GlueMax { get; set; }
        #endregion

        #region 自动加压
        [ExcelToEntity(45)]
        [EntityToexcel("加压位置", 45)]
        public int? PressureLocate { get; set; }

        [ExcelToEntity(46)]
        [EntityToexcel("加压数据类型", 46)]
        public PressurizeDataType? PressurizeDataType { get; set; }

        [ExcelToEntity(47)]
        [EntityToexcel("压力下限", 47)]
        public decimal? PressureMin { get; set; }

        [ExcelToEntity(48)]
        [EntityToexcel("压力上限", 48)]
        public decimal? PressureMax { get; set; }
        #endregion

        #region 模组入箱

        [ExcelToEntity(49)]
        [EntityToexcel("模组位置", 49)]
        public int? ModuleLocation { get; set; }

        [ExcelToEntity(50)]
        [EntityToexcel("模组入箱数据类型", 50)]
        public ModuleInBoxDataTypeEnum? ModuleInBoxDataType { get; set; }

        [ExcelToEntity(51)]
        [EntityToexcel("模组PN", 51)]
        public string? ModulePn { get; set; }

        [ExcelToEntity(52)]
        [EntityToexcel("模组入箱数据最小值", 53)]
        public decimal? BlockMinValue { get; set; }

        [ExcelToEntity(53)]
        [EntityToexcel("模组入箱数据最大值", 54)]
        public decimal? BlockMaxValue { get; set; }

        #endregion
    }

    /// <summary>
    /// 网页端Pack数据导出
    /// </summary>
    public class PackTaskExcel
    {
        /// <summary>
        /// Pack码
        /// </summary>
        [ExcelToEntity(1)]
        [EntityToexcel("Pack码", 1)]
        public string? PackCode { get; set; }

        /// <summary>
        /// 工位
        /// </summary>
        [ExcelToEntity(2)]
        [EntityToexcel("工位", 2)]
        public string? StationCode { get; set; }

        [ExcelToEntity(3)]
        [EntityToexcel("任务时间", 3)]
        public string? 任务时间 { get; set; }

        [ExcelToEntity(4)]
        [EntityToexcel("任务类型", 4)]
        public string? TaskType { get; set; }

        [ExcelToEntity(5)]
        [EntityToexcel("任务名称", 5)]
        public string? TaskName { get; set; }
        [ExcelToEntity(6)]
        [EntityToexcel("序号", 6)]
        public string OrderNo { get; set; } = "1";

        [ExcelToEntity(7)]
        [EntityToexcel("内部码", 7)]
        public string? 内部码 { get; set; }

        [ExcelToEntity(8)]
        [EntityToexcel("外部码", 8)]
        public string? 外部码 { get; set; }

        [ExcelToEntity(9)]
        [EntityToexcel("拧紧结果", 9)]
        public string? 拧紧结果 { get; set; }

        [ExcelToEntity(10)]
        [EntityToexcel("数据1", 10)]
        public string? 数据1 { get; set; }

        [ExcelToEntity(11)]
        [EntityToexcel("数据2", 11)]
        public string? 数据2 { get; set; }

        [ExcelToEntity(12)]
        [EntityToexcel("上传代码1", 12)]
        public string? UpMesCode1 { get; set; }

        [ExcelToEntity(13)]
        [EntityToexcel("上传代码2", 13)]
        public string? UpMesCode2 { get; set; }
    }
    /// <summary>
    /// 网页端涂胶数据导出
    /// </summary>
    public class GulingInfoToExcel
    {
        /// <summary>
        /// pack码
        /// </summary>
        [ExcelToEntity(1)]
        [EntityToexcel("Pack码", 1)]
        public string? PackPN { get; set; }

        /// <summary>
        /// 参数名称
        /// </summary>
        [ExcelToEntity(2)]
        [EntityToexcel("任务名称", 2)]
        public string? ParameterName { get; set; }
        /// <summary>
        /// 上传代码
        /// </summary>
        [ExcelToEntity(3)]
        [EntityToexcel("上传代码", 3)]
        public string? UpMesCodePN { get; set; }
        /// <summary>
        /// 涂胶数据
        /// </summary>
        [ExcelToEntity(4)]
        [EntityToexcel("任务数据", 4)]
        public string? UpValue { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        [ExcelToEntity(5)]
        [EntityToexcel("创建日期", 5)]
        public string? CreateTime { get; set; }
    }
    /// <summary>
    /// 网页人工拧紧数据导出
    /// </summary>
    public class BlotGunInfoToExcel
    {
        /// <summary>
        /// PackPN
        /// </summary>
        [ExcelToEntity(1)]
        [EntityToexcel("PackPN", 1)]
        [Description("PackPN")]
        public string? PackPN { get; set; }

        /// <summary>
        /// 工序
        /// </summary>
        [ExcelToEntity(2)]
        [EntityToexcel("工序名称", 2)]
        [Description("工序名称")]
        public string? StepName { get; set; }

        /// <summary>
        /// 任务
        /// </summary>
        [ExcelToEntity(3)]
        [EntityToexcel("工位名称", 3)]
        [Description("工位名称")]
        public string? StationName { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [ExcelToEntity(4)]
        [EntityToexcel("拧紧结果", 4)]
        [Description("拧紧结果")]
        public string? ResultIsOk { get; set; }

        /// <summary>
        /// 程序号
        /// </summary>
        [ExcelToEntity(5)]
        [EntityToexcel("程序号", 5)]
        [Description("程序号")]
        public string? ProgramNO { get; set; }

        /// <summary>
        /// 扭力上传代码
        /// </summary>
        [ExcelToEntity(6)]
        [EntityToexcel("扭力上传代码", 6)]
        [Description("扭力上传代码")]
        public string? UploadCode { get; set; }

        /// <summary>
        /// 角度上传代码
        /// </summary>
        [ExcelToEntity(7)]
        [EntityToexcel("角度上传代码", 7)]
        [Description("角度上传代码")]
        public string? UploadCode_JD { get; set; }

        /// <summary>
        /// 角度值
        /// </summary>
        [ExcelToEntity(8)]
        [EntityToexcel("角度值", 8)]
        [Description("角度值")]
        public string? FinalAngle { get; set; }
        /// <summary>
        /// 扭矩值
        /// </summary>
        [ExcelToEntity(9)]
        [EntityToexcel("扭矩值", 9)]
        [Description("扭矩值")]
        public string? FinalTorque { get; set; }

        /// <summary>
        /// 使用设备
        /// </summary>
        [ExcelToEntity(10)]
        [EntityToexcel("使用设备", 10)]
        [Description("使用设备")]
        public string? Base_ProResource { get; set; }

        [Description("任务名称")]
        public string? ScrewName { get; set; }

        /// <summary>
        /// 操作员
        /// </summary>
        [ExcelToEntity(11)]
        [EntityToexcel("操作员", 11)]
        [Description("操作员")]
        public string? CreateUser { get; set; }

        [Description("创建时间")]
        public string? CreateDate { get; set; }
    }
    /// <summary>
    /// 网页自动拧紧数据导出
    /// </summary>
    public class AutoBlotGunInfoToExcel
    {
        /// <summary>
        /// pack编码
        /// </summary>
        [ExcelToEntity(1)]
        [EntityToexcel("Pack编码", 1)]
        [Description("Pack编码")]
        public string? PackPN { get; set; }

        /// <summary>
        /// 螺丝号
        /// </summary>
        [ExcelToEntity(2)]
        [EntityToexcel("螺丝号", 2)]
        [Description("螺丝号")]
        public string? OrderNO { get; set; }
        /// <summary>
        /// 程序号
        /// </summary>
        [ExcelToEntity(3)]
        [EntityToexcel("程序号", 3)]
        [Description("程序号")]
        public string? ProgramNO { get; set; }
        /// <summary>
        /// 扭力上传代码
        /// </summary>
        [ExcelToEntity(4)]
        [EntityToexcel("扭力上传代码", 4)]
        [Description("扭力上传代码")]
        public string? UploadCode { get; set; }

        /// <summary>
        /// 角度上传代码
        /// </summary>
        [ExcelToEntity(5)]
        [EntityToexcel("角度上传代码", 4)]
        [Description("角度上传代码")]
        public string? UploadCode_JD { get; set; }



        /// <summary>
        /// 状态
        /// </summary>
        [ExcelToEntity(6)]
        [EntityToexcel("拧紧结果", 6)]
        [Description("拧紧结果")]
        public string? ResultIsOk { get; set; }

        /// <summary>
        /// 角度值
        /// </summary>
        [ExcelToEntity(7)]
        [EntityToexcel("角度值", 7)]
        [Description("角度值")]
        public string? FinalAngle { get; set; }
        /// <summary>
        /// 扭矩值
        /// </summary>
        [ExcelToEntity(8)]
        [EntityToexcel("扭矩值", 8)]
        [Description("扭矩值")]
        public string? FinalTorque { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        [ExcelToEntity(9)]
        [EntityToexcel("创建日期", 9)]
        [Description("创建日期")]
        public string? CreateTime { get; set; }

    }
    /// <summary>
    /// 网页组装数据导出
    /// </summary>
    public class BomInfoToExcel
    {
        /// <summary>
        /// pack编码
        /// </summary>
        [ExcelToEntity(1)]
        [EntityToexcel("Pack编码", 1)]
        public string? PackPN { get; set; }

        /// <summary>
        /// 任务
        /// </summary>
        [ExcelToEntity(2)]
        [EntityToexcel("工位名称", 2)]
        public string? StationId { get; set; }
        /// <summary>
        /// 工序
        /// </summary>
        [ExcelToEntity(3)]
        [EntityToexcel("工序名称", 3)]
        public string? StepId { get; set; }

        /// <summary>
        /// 物料名
        /// </summary>
        [ExcelToEntity(4)]
        [EntityToexcel("物料名", 4)]
        public string? GoodsName { get; set; }
        /// <summary>
        /// 物料号
        /// </summary>
        [ExcelToEntity(5)]
        [EntityToexcel("物料号", 5)]
        public string? GoodsPN { get; set; }

        /// <summary>
        /// 批次码
        /// </summary>
        [ExcelToEntity(6)]
        [EntityToexcel("批次码", 6)]
        public string? BatchBarCode { get; set; }

        /// <summary>
        /// 外部条码
        /// </summary>
        [ExcelToEntity(7)]
        [EntityToexcel("外部条码", 7)]
        public string? GoodsOuterCode { get; set; }
        
        /// <summary>
        /// 库存唯一码
        /// </summary>
        [ExcelToEntity(8)]
        [EntityToexcel("库存唯一码", 8)]
        public string? UniBarCode { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        [ExcelToEntity(9)]
        [EntityToexcel("数量", 9)]
        public string? UseNum { get; set; }
        /// <summary>
        /// 操作员
        /// </summary>
        [ExcelToEntity(10)]
        [EntityToexcel("操作员", 10)]
        public string? CreaterUser { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        [ExcelToEntity(9)]
        [EntityToexcel("创建日期", 11)]
        public string? CreateTime { get; set; }



    }

    /// <summary>
    /// 网页端Block数据导出
    /// </summary>
    public class ModuleInfoRecordToExcel
    {
        /// <summary>
        /// pack码
        /// </summary>
        [ExcelToEntity(1)]
        [EntityToexcel("Pack码", 1)]
        public string? PackCode { get; set; }

        /// <summary>
        /// 涂胶数据
        /// </summary>
        [ExcelToEntity(2)]
        [EntityToexcel("Module码", 2)]
        public string? ModuleCode { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        [ExcelToEntity(3)]
        [EntityToexcel("创建日期", 3)]
        public string? CreateTime { get; set; }
    }

    /// <summary>
    /// 网页端拉铆数据导出
    /// </summary>
    public class LaMaoGunInfoRecordToExcel
    {
        /// <summary>
        /// pack码
        /// </summary>
        [ExcelToEntity(1)]
        [EntityToexcel("Pack码", 1)]
        public string? PackCode { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        [ExcelToEntity(2)]
        [EntityToexcel("序号", 2)]
        public int? OrderNo { get; set; }
        /// <summary>
        /// 程序号
        /// </summary>
        [ExcelToEntity(3)]
        [EntityToexcel("程序号", 3)]
        public int? ProgramNo { get; set; }

        /// <summary>
        /// 拉力
        /// </summary>
        [ExcelToEntity(4)]
        [EntityToexcel("拉力", 4)]
        public decimal? MaxPullPower { get; set; }

        /// <summary>
        /// 行程
        /// </summary>
        [ExcelToEntity(5)]
        [EntityToexcel("行程", 5)]
        public decimal? MaxDistance { get; set; }
        /// <summary>
        /// 拉力上传代码
        /// </summary>
        [ExcelToEntity(6)]
        [EntityToexcel("拉力上传代码", 6)]
        public string? UploadCode_LL { get; set; }
        /// <summary>
        /// 行程上传代码
        /// </summary>
        [ExcelToEntity(7)]
        [EntityToexcel("行程上传代码", 7)]
        public string? UploadCode_XC { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [ExcelToEntity(8)]
        [EntityToexcel("创建时间", 8)]
        public string? CreateTime { get; set; }

    }


}
