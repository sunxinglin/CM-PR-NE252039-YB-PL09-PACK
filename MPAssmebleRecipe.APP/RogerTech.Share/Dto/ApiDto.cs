using System;
using System.Collections.Generic;

namespace RogerTech.Share.Dto
{
    #region 通用响应模型

    /// <summary>
    /// API通用响应模型
    /// </summary>
    /// <typeparam name="T">返回数据类型</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// 返回代码
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 返回信息
        /// </summary>
        public string Msg { get; set; }

        /// <summary>
        /// 返回数据
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// 接口业务
        /// </summary>
        public string BizCode { get; set; }

        /// <summary>
        /// 响应时间
        /// </summary>
        public string ResTime { get; set; }

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess => Code == 200;
    }

    #endregion

    #region 1、SFC站点绑定

    /// <summary>
    /// SFC站点绑定请求DTO
    /// </summary>
    public class SfcBindRequestDto
    {
        /// <summary>
        /// SFC编号
        /// </summary>
        public string SfcNo { get; set; }

        /// <summary>
        /// 站点编号（固定值）
        /// </summary>
        public string SiteNo { get; set; }

        /// <summary>
        /// SFC类型
        /// </summary>
        public string SfcType { get; set; }

        /// <summary>
        /// AGV编号
        /// </summary>
        public string AgvNo { get; set; }

        /// <summary>
        /// 物料编号
        /// </summary>
        public string MaterialPn { get; set; }

        /// <summary>
        /// 系统类型（固定值）
        /// </summary>
        public string SystemType { get; set; }
    }

    #endregion

    #region 2、SFC站点更新

    /// <summary>
    /// SFC站点更新请求DTO
    /// </summary>
    public class SfcUpdateRequestDto
    {
        /// <summary>
        /// SFC编号
        /// </summary>
        public string SfcNo { get; set; }

        /// <summary>
        /// 站点编号（固定值）
        /// </summary>
        public string SiteNo { get; set; }

        /// <summary>
        /// SFC类型
        /// </summary>
        public string SfcType { get; set; }

        /// <summary>
        /// AGV编号
        /// </summary>
        public string AgvNo { get; set; }

        /// <summary>
        /// 物料编号
        /// </summary>
        public string MaterialPn { get; set; }

        /// <summary>
        /// 系统类型（固定值）
        /// </summary>
        public string SystemType { get; set; }
    }

    #endregion

    #region 3、设备信息同步

    /// <summary>
    /// 设备信息同步请求DTO
    /// </summary>
    public class DeviceSyncRequestDto
    {
        /// <summary>
        /// 设备编号（固定值）
        /// </summary>
        public string DeviceNo { get; set; }

        /// <summary>
        /// 设备状态
        /// 1:空闲, 2:工作中, 3:故障, 4:下线
        /// 自动状态下有料为工作中; 自动状态下无料为空闲;
        /// 自动状态下设备故障无法正常工作为故障; 非自动状态即为下线
        /// </summary>
        public string DeviceStatus { get; set; }

        /// <summary>
        /// 设备描述
        /// </summary>
        public string DeviceDescription { get; set; }

        /// <summary>
        /// 是否为删除记录
        /// Y-是, N-否
        /// 当该字段为Y时,根据设备编号进行新增或者更新
        /// 当该字段为N时,删除设备记录信息
        /// </summary>
        public string IsDel { get; set; }
    }

    /// <summary>
    /// 设备状态枚举
    /// </summary>
    public enum DeviceStatus
    {
        /// <summary>
        /// 空闲
        /// </summary>
        Idle = 1,

        /// <summary>
        /// 工作中
        /// </summary>
        Working = 2,

        /// <summary>
        /// 故障
        /// </summary>
        Fault = 3,

        /// <summary>
        /// 下线
        /// </summary>
        Offline = 4
    }

    #endregion

    #region 4、获取SFC类型

    /// <summary>
    /// 获取SFC类型请求DTO
    /// </summary>
    public class SfcTypeRequestDto
    {
        /// <summary>
        /// SFC编号
        /// </summary>
        public string SfcNo { get; set; }

        /// <summary>
        /// AGV编号
        /// </summary>
        public string AgvNo { get; set; }

        /// <summary>
        /// 系统类型（固定值）
        /// </summary>
        public string SystemType { get; set; }
    }

    #endregion

    #region 5、SFC校验

    /// <summary>
    /// SFC校验请求DTO
    /// </summary>
    public class SfcCheckRequestDto
    {
        /// <summary>
        /// SFC编号
        /// </summary>
        public string SfcNo { get; set; }

        /// <summary>
        /// 站点编号
        /// </summary>
        public string SiteNo { get; set; }

        /// <summary>
        /// AGV编号
        /// </summary>
        public string AgvNo { get; set; }

        /// <summary>
        /// 系统类型（固定值）
        /// </summary>
        public string SystemType { get; set; }
    }

    #endregion

    #region 6、NG任务下发

    /// <summary>
    /// NG任务下发请求DTO
    /// </summary>
    public class NgTaskAddRequestDto
    {
        /// <summary>
        /// AGV编号
        /// </summary>
        public string AgvNo { get; set; }

        /// <summary>
        /// 任务类型
        /// NG1-返工, NG2-报废, NG3-离线
        /// </summary>
        public string TaskType { get; set; }

        /// <summary>
        /// 站点编号
        /// </summary>
        public string SiteNo { get; set; }

        /// <summary>
        /// 系统类型（固定值）
        /// </summary>
        public string SystemType { get; set; }
    }

    /// <summary>
    /// NG任务类型枚举
    /// </summary>
    public enum NgTaskType
    {
        /// <summary>
        /// 返工
        /// </summary>
        Rework,  // NG1

        /// <summary>
        /// 报废
        /// </summary>
        Scrap,   // NG2

        /// <summary>
        /// 离线
        /// </summary>
        Offline  // NG3
    }

    #endregion

    #region 7、获取任务类型

    /// <summary>
    /// 获取任务类型请求DTO
    /// </summary>
    public class TaskTypeRequestDto
    {
        /// <summary>
        /// AGV编号
        /// </summary>
        public string AgvNo { get; set; }

        /// <summary>
        /// 系统类型（固定值）
        /// </summary>
        public string SystemType { get; set; }
    }

    /// <summary>
    /// 任务类型枚举
    /// </summary>
    public enum TaskType
    {
        /// <summary>
        /// 生产任务
        /// </summary>
        ProductionTask,  // PT

        /// <summary>
        /// 换型任务
        /// </summary>
        ExchangeTask,    // ET

        /// <summary>
        /// 首件任务
        /// </summary>
        FirstPieceTask   // FP
    }

    #endregion

    #region 8、换型任务下发

    /// <summary>
    /// 换型任务下发请求DTO
    /// </summary>
    public class ExchangeTaskAddRequestDto
    {
        /// <summary>
        /// AGV编号（非必填）
        /// </summary>
        public string AgvNo { get; set; }

        /// <summary>
        /// 任务类型
        /// 3-焊接换型送空, 4-焊接换型送满
        /// 5-拧紧换型送空, 6-拧紧换型送满
        /// 7-入箱换型送满
        /// </summary>
        public string TaskType { get; set; }

        /// <summary>
        /// 站点编号
        /// </summary>
        public string SiteNo { get; set; }

        /// <summary>
        /// 系统类型（固定值）
        /// </summary>
        public string SystemType { get; set; }
    }

    /// <summary>
    /// 换型任务类型枚举
    /// </summary>
    public enum ExchangeTaskType
    {
        /// <summary>
        /// 焊接换型送空
        /// </summary>
        WeldingExchangeEmpty = 3,

        /// <summary>
        /// 焊接换型送满
        /// </summary>
        WeldingExchangeFull = 4,

        /// <summary>
        /// 拧紧换型送空
        /// </summary>
        TighteningExchangeEmpty = 5,

        /// <summary>
        /// 拧紧换型送满
        /// </summary>
        TighteningExchangeFull = 6,

        /// <summary>
        /// 入箱换型送满
        /// </summary>
        BoxingExchangeFull = 7
    }

    #endregion

    #region 9、首件任务下发

    /// <summary>
    /// 首件任务下发请求DTO
    /// </summary>
    public class FirstTaskAddRequestDto
    {
        /// <summary>
        /// AGV编号
        /// </summary>
        public string AgvNo { get; set; }

        /// <summary>
        /// 任务类型
        /// F1-焊接首件
        /// </summary>
        public string TaskType { get; set; }

        /// <summary>
        /// 站点编号
        /// </summary>
        public string SiteNo { get; set; }

        /// <summary>
        /// 系统类型（固定值）
        /// </summary>
        public string SystemType { get; set; }
    }

    /// <summary>
    /// 首件任务类型枚举
    /// </summary>
    public enum FirstTaskType
    {
        /// <summary>
        /// 焊接首件
        /// </summary>
        WeldingFirstPiece  // F1
    }

    #endregion

    #region 10、拧紧数据通用DTO

    /// <summary>
    /// 拧紧数据通用DTO
    /// </summary>
    public class TightenDataDto
    {
        /// <summary>
        /// Pack码
        /// </summary>
        public string SFC { get; set; }

        /// <summary>
        /// 拧紧结果
        /// </summary>
        public int Result { get; set; }
        /// <summary>
        /// 程序号
        /// </summary>
        public int ProgramNo { get; set; }
        /// <summary>
        /// 序号
        /// </summary>
        public int OrderNo { get; set; }

        /// <summary>
        /// 扭矩结果
        /// </summary>
        public float TorqueResult { get; set; }
        /// <summary>
        /// 角度结果
        /// </summary>
        public float AngleResult { get; set; }
    }

    /// <summary>
    /// 拧紧数据上传请求DTO
    /// </summary>
    public class TightenDataRequestDto
    {
        /// <summary>
        /// Pack码
        /// </summary>
        public string SFC { get; set; }

        /// <summary>
        /// 拧紧数据列表
        /// </summary>
        public List<TightenDataDto> TightenDataList { get; set; }
    }

    #endregion

    #region 扩展方法

    /// <summary>
    /// 枚举扩展方法
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// NG任务类型转字符串
        /// </summary>
        public static string ToTaskTypeString(this NgTaskType taskType)
        {
            return "NG" + ((int)taskType + 1);
        }

        /// <summary>
        /// 任务类型转字符串
        /// </summary>
        public static string ToTaskTypeString(this TaskType taskType)
        {
            switch (taskType)
            {
                case TaskType.ProductionTask: return "PT";
                case TaskType.ExchangeTask: return "ET";
                case TaskType.FirstPieceTask: return "FP";
                default: return "";
            }
        }

        /// <summary>
        /// 首件任务类型转字符串
        /// </summary>
        public static string ToTaskTypeString(this FirstTaskType taskType)
        {
            return "F" + ((int)taskType + 1);
        }

        /// <summary>
        /// 设备状态转字符串
        /// </summary>
        public static string ToStatusString(this DeviceStatus status)
        {
            return ((int)status).ToString();
        }
    }

    #endregion
}
